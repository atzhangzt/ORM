//
// https://github.com/ServiceStack/ServiceStack.Text
// ServiceStack.Text: .NET C# POCO JSON, JSV and CSV Text Serializers.
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2012 ServiceStack, Inc. All Rights Reserved.
//
// Licensed under the same terms of ServiceStack.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ServiceStack.Text.Common
{
    public static class DeserializeType<TSerializer>
        where TSerializer : ITypeSerializer
    {
        private static readonly ITypeSerializer Serializer = JsWriter.GetTypeSerializer<TSerializer>();

        internal static ParseStringDelegate GetParseMethod(TypeConfig typeConfig) => v => GetParseStringSpanMethod(typeConfig)(v.AsSpan());

        internal static ParseStringSpanDelegate GetParseStringSpanMethod(TypeConfig typeConfig)
        {
            var type = typeConfig.Type;

            if (!type.IsStandardClass()) return null;
            var accessors = DeserializeTypeRef.GetTypeAccessors(typeConfig, Serializer);

            var ctorFn = JsConfig.ModelFactory(type);
            if (accessors == null)
                return value => ctorFn();
            
            if (typeof(TSerializer) == typeof(Json.JsonTypeSerializer))
                return new StringToTypeContext(typeConfig, ctorFn, accessors).DeserializeJson;

            return new StringToTypeContext(typeConfig, ctorFn, accessors).DeserializeJsv;
        }

        internal struct StringToTypeContext
        {
            private readonly TypeConfig typeConfig;
            private readonly EmptyCtorDelegate ctorFn;
            private readonly KeyValuePair<string, TypeAccessor>[] accessors;
            
            public StringToTypeContext(TypeConfig typeConfig, EmptyCtorDelegate ctorFn, KeyValuePair<string, TypeAccessor>[] accessors)
            {
                this.typeConfig = typeConfig;
                this.ctorFn = ctorFn;
                this.accessors = accessors;
            }

            internal object DeserializeJson(ReadOnlySpan<char> value) => DeserializeTypeRefJson.StringToType(value, typeConfig, ctorFn, accessors);

            internal object DeserializeJsv(ReadOnlySpan<char> value) => DeserializeTypeRefJsv.StringToType(value, typeConfig, ctorFn, accessors);
        }

        public static object ObjectStringToType(ReadOnlySpan<char> strType)
        {
            var type = ExtractType(strType);
            if (type != null)
            {
                var parseFn = Serializer.GetParseStringSegmentFn(type);
                var propertyValue = parseFn(strType);
                return propertyValue;
            }

            if (JsConfig.ConvertObjectTypesIntoStringDictionary && !strType.IsNullOrEmpty())
            {
                if (strType[0] == JsWriter.MapStartChar)
                {
                    var dynamicMatch = DeserializeDictionary<TSerializer>.ParseDictionary<string, object>(strType, null, v => Serializer.UnescapeString(v).ToString(), v => Serializer.UnescapeString(v).ToString());
                    if (dynamicMatch != null && dynamicMatch.Count > 0)
                    {
                        return dynamicMatch;
                    }
                }

                if (strType[0] == JsWriter.ListStartChar)
                {
                    return DeserializeList<List<object>, TSerializer>.ParseStringSpan(strType);
                }
            }

            var primitiveType = JsConfig.TryToParsePrimitiveTypeValues ? ParsePrimitive(strType) : null;
            if (primitiveType != null)
                return primitiveType;

            if (Serializer.ObjectDeserializer != null)
                return Serializer.ObjectDeserializer(strType);

            return Serializer.UnescapeString(strType).ToString();
        }

        public static Type ExtractType(string strType) => ExtractType(strType.AsSpan());

        //TODO: optimize ExtractType
        public static Type ExtractType(ReadOnlySpan<char> strType)
        {
            if (strType.IsEmpty || strType.Length <= 1) return null;

            var hasWhitespace = Json.JsonUtils.WhiteSpaceChars.Contains(strType[1]);
            if (hasWhitespace)
            {
                var pos = strType.IndexOf('"');
                if (pos >= 0)
                    strType = ("{" + strType.Substring(pos, strType.Length - pos)).AsSpan();
            }

            var typeAttrInObject = Serializer.TypeAttrInObject;
            if (strType.Length > typeAttrInObject.Length
                && strType.Slice(0, typeAttrInObject.Length).EqualsOrdinal(typeAttrInObject))
            {
                var propIndex = typeAttrInObject.Length;
                var typeName = Serializer.UnescapeSafeString(Serializer.EatValue(strType, ref propIndex)).ToString();

                var type = JsConfig.TypeFinder(typeName);

                JsWriter.AssertAllowedRuntimeType(type);

                if (type == null)
                {
                    Tracer.Instance.WriteWarning("Could not find type: " + typeName);
                    return null;
                }

                return PclExport.Instance.UseType(type);
            }
            return null;
        }

        public static object ParseAbstractType<T>(string value) => ParseAbstractType<T>(value.AsSpan());

        public static object ParseAbstractType<T>(ReadOnlySpan<char> value)
        {
            if (typeof(T).IsAbstract)
            {
                if (value.IsNullOrEmpty()) return null;
                var concreteType = ExtractType(value);
                if (concreteType != null)
                {
                    return Serializer.GetParseStringSegmentFn(concreteType)(value);
                }
                Tracer.Instance.WriteWarning(
                    "Could not deserialize Abstract Type with unknown concrete type: " + typeof(T).FullName);
            }
            return null;
        }

        public static object ParseQuotedPrimitive(string value)
        {
            var fn = JsConfig.ParsePrimitiveFn;
            var result = fn?.Invoke(value);
            if (result != null)
                return result;

            if (string.IsNullOrEmpty(value))
                return null;

            if (Guid.TryParse(value, out Guid guidValue)) return guidValue;

            if (value.StartsWith(DateTimeSerializer.EscapedWcfJsonPrefix, StringComparison.Ordinal) || value.StartsWith(DateTimeSerializer.WcfJsonPrefix, StringComparison.Ordinal))
                return DateTimeSerializer.ParseWcfJsonDate(value);

            if (JsConfig.DateHandler == DateHandler.ISO8601)
            {
                // check that we have UTC ISO8601 date:
                // YYYY-MM-DDThh:mm:ssZ
                // YYYY-MM-DDThh:mm:ss+02:00
                // YYYY-MM-DDThh:mm:ss-02:00
                if (value.Length > 14 && value[10] == 'T' &&
                    (value.EndsWithInvariant("Z")
                    || value[value.Length - 6] == '+'
                    || value[value.Length - 6] == '-'))
                {
                    return DateTimeSerializer.ParseShortestXsdDateTime(value);
                }
            }

            if (JsConfig.DateHandler == DateHandler.RFC1123)
            {
                // check that we have RFC1123 date:
                // ddd, dd MMM yyyy HH:mm:ss GMT
                if (value.Length == 29 && (value.EndsWithInvariant("GMT")))
                {
                    return DateTimeSerializer.ParseRFC1123DateTime(value);
                }
            }

            return Serializer.UnescapeString(value);
        }

        public static object ParsePrimitive(string value) => ParsePrimitive(value.AsSpan());

        public static object ParsePrimitive(ReadOnlySpan<char> value)
        {
            var fn = JsConfig.ParsePrimitiveFn;
            var result = fn?.Invoke(value.ToString());
            if (result != null)
                return result;

            if (value.IsNullOrEmpty())
                return null;

            if (value.TryParseBoolean(out bool boolValue))
                return boolValue;

            return value.ParseNumber();
        }

        internal static object ParsePrimitive(string value, char firstChar)
        {
            if (typeof(TSerializer) == typeof(Json.JsonTypeSerializer))
            {
                return firstChar == JsWriter.QuoteChar
                    ? ParseQuotedPrimitive(value)
                    : ParsePrimitive(value);
            }
            return (ParsePrimitive(value) ?? ParseQuotedPrimitive(value));
        }
    }
        
    internal static class TypeAccessorUtils
    {
        internal static TypeAccessor Get(this KeyValuePair<string, TypeAccessor>[] accessors, ReadOnlySpan<char> propertyName, bool lenient)
        {
            if (lenient)
            {
                //TODO: optimize
                propertyName = propertyName.ToString().Replace("-", string.Empty).Replace("_", string.Empty).AsSpan();
            }

            //Binary Search
            var lo = 0;
            var hi = accessors.Length - 1;
            var mid = (lo + hi + 1) / 2; 

            while (lo <= hi)
            {
                var test = accessors[mid];
                var cmp = propertyName.CompareTo(test.Key.AsSpan(), StringComparison.OrdinalIgnoreCase);
                if (cmp == 0)
                    return test.Value;

                if (cmp < 0)
                    hi = mid - 1;
                else
                    lo = mid + 1;

                mid = (lo + hi + 1) / 2;  
            }
            return null;
        }
    }
    
   

    public static class DeserializeTypeExensions
    {
        public static bool Has(this ParseAsType flags, ParseAsType flag)
        {
            return (flag & flags) != 0;
        }

        public static object ParseNumber(this ReadOnlySpan<char> value) => ParseNumber(value, JsConfig.TryParseIntoBestFit);
        public static object ParseNumber(this ReadOnlySpan<char> value, bool bestFit)
        {
            if (value.Length == 1)
            {
                int singleDigit = value[0];
                if (singleDigit >= 48 || singleDigit <= 57) // 0 - 9
                {
                    var result = singleDigit - 48;
                    if (bestFit)
                        return (byte) result;
                    return result;
                }
            }

            // Parse as decimal
            var acceptDecimal = JsConfig.ParsePrimitiveFloatingPointTypes.Has(ParseAsType.Decimal);
            var isDecimal = value.TryParseDecimal(out decimal decimalValue);

            // Check if the number is an Primitive Integer type given that we have a decimal
            if (isDecimal && decimalValue == decimal.Truncate(decimalValue))
            {
                // Value is a whole number
                var parseAs = JsConfig.ParsePrimitiveIntegerTypes;
                if (parseAs.Has(ParseAsType.Byte) && decimalValue <= byte.MaxValue && decimalValue >= byte.MinValue)
                    return (byte)decimalValue;
                if (parseAs.Has(ParseAsType.SByte) && decimalValue <= sbyte.MaxValue && decimalValue >= sbyte.MinValue)
                    return (sbyte)decimalValue;
                if (parseAs.Has(ParseAsType.Int16) && decimalValue <= Int16.MaxValue && decimalValue >= Int16.MinValue)
                    return (Int16)decimalValue;
                if (parseAs.Has(ParseAsType.UInt16) && decimalValue <= UInt16.MaxValue && decimalValue >= UInt16.MinValue)
                    return (UInt16)decimalValue;
                if (parseAs.Has(ParseAsType.Int32) && decimalValue <= Int32.MaxValue && decimalValue >= Int32.MinValue)
                    return (Int32)decimalValue;
                if (parseAs.Has(ParseAsType.UInt32) && decimalValue <= UInt32.MaxValue && decimalValue >= UInt32.MinValue)
                    return (UInt32)decimalValue;
                if (parseAs.Has(ParseAsType.Int64) && decimalValue <= Int64.MaxValue && decimalValue >= Int64.MinValue)
                    return (Int64)decimalValue;
                if (parseAs.Has(ParseAsType.UInt64) && decimalValue <= UInt64.MaxValue && decimalValue >= UInt64.MinValue)
                    return (UInt64)decimalValue;
                return decimalValue;
            }

            // Value is a floating point number

            // Return a decimal if the user accepts a decimal
            if (isDecimal && acceptDecimal)
                return decimalValue;

            var acceptFloat = JsConfig.ParsePrimitiveFloatingPointTypes.HasFlag(ParseAsType.Single);
            var isFloat = value.TryParseFloat(out float floatValue);
            if (acceptFloat && isFloat)
                return floatValue;

            var acceptDouble = JsConfig.ParsePrimitiveFloatingPointTypes.HasFlag(ParseAsType.Double);
            var isDouble = value.TryParseDouble(out double doubleValue);
            if (acceptDouble && isDouble)
                return doubleValue;

            if (isDecimal)
                return decimalValue;
            if (isFloat)
                return floatValue;
            if (isDouble)
                return doubleValue;

            return null;
        }
    }


}
