using ServiceStack.Text.Common;
using ServiceStack.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ServiceStack.Text
{
    internal class TypeAccessor
    {
        internal ParseStringDelegate GetProperty;
        internal SetPropertyDelegate SetProperty;
        internal Type PropertyType;

        public static Type ExtractType(ITypeSerializer Serializer, string strType)
        {
            if (strType == null || strType.Length <= 1)
                return (Type)null;
            if (((IEnumerable<char>)JsonUtils.WhiteSpaceChars).Contains<char>(strType[1]))
            {
                int startIndex = strType.IndexOf('"');
                if (startIndex >= 0)
                    strType = "{" + strType.Substring(startIndex);
            }
            string typeAttrInObject = Serializer.TypeAttrInObject;
            if (strType.Length <= typeAttrInObject.Length || !(strType.Substring(0, typeAttrInObject.Length) == typeAttrInObject))
                return (Type)null;
            int length = typeAttrInObject.Length;
            string str = Serializer.EatValue(strType, ref length);
            Type type = JsConfig.TypeFinder(str);
            if (!(type == (Type)null))
                return type;
            Tracer.Instance.WriteWarning("Could not find type: " + str);
            return type;
        }

        public static TypeAccessor Create(ITypeSerializer serializer, TypeConfig typeConfig, PropertyInfo propertyInfo)
        {
            return new TypeAccessor()
            {
                PropertyType = propertyInfo.PropertyType,
                GetProperty = serializer.GetParseFn(propertyInfo.PropertyType),
                SetProperty = TypeAccessor.GetSetPropertyMethod(typeConfig, propertyInfo)
            };
        }

        private static SetPropertyDelegate GetSetPropertyMethod(TypeConfig typeConfig, PropertyInfo propertyInfo)
        {
            if (propertyInfo.ReflectedType() != propertyInfo.DeclaringType)
                propertyInfo = propertyInfo.DeclaringType.GetPropertyInfo(propertyInfo.Name);
            if (!propertyInfo.CanWrite && !typeConfig.EnableAnonymousFieldSetters)
                return (SetPropertyDelegate)null;
            FieldInfo fieldInfo = (FieldInfo)null;
            if (!propertyInfo.CanWrite)
            {
                string str = string.Format(Env.IsMono ? "<{0}>" : "<{0}>i__Field", (object)propertyInfo.Name);
                foreach (FieldInfo writableField in typeConfig.Type.GetWritableFields())
                {
                    if (writableField.IsInitOnly && writableField.FieldType == propertyInfo.PropertyType && writableField.Name == str)
                    {
                        fieldInfo = writableField;
                        break;
                    }
                }
                if (fieldInfo == (FieldInfo)null)
                    return (SetPropertyDelegate)null;
            }
            return PclExport.Instance.GetSetMethod(propertyInfo, fieldInfo);
        }

        internal static SetPropertyDelegate GetSetPropertyMethod(Type type, PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanWrite || ((IEnumerable<ParameterInfo>)propertyInfo.GetIndexParameters()).Any<ParameterInfo>())
                return (SetPropertyDelegate)null;
            return PclExport.Instance.GetSetPropertyMethod(propertyInfo);
        }

        internal static SetPropertyDelegate GetSetFieldMethod(Type type, FieldInfo fieldInfo)
        {
            return PclExport.Instance.GetSetFieldMethod(fieldInfo);
        }

        public static TypeAccessor Create(ITypeSerializer serializer, TypeConfig typeConfig, FieldInfo fieldInfo)
        {
            return new TypeAccessor()
            {
                PropertyType = fieldInfo.FieldType,
                GetProperty = serializer.GetParseFn(fieldInfo.FieldType),
                SetProperty = TypeAccessor.GetSetFieldMethod(typeConfig, fieldInfo)
            };
        }

        private static SetPropertyDelegate GetSetFieldMethod(TypeConfig typeConfig, FieldInfo fieldInfo)
        {
            if (fieldInfo.ReflectedType() != fieldInfo.DeclaringType)
                fieldInfo = fieldInfo.DeclaringType.GetFieldInfo(fieldInfo.Name);
            return PclExport.Instance.GetSetFieldMethod(fieldInfo);
        }
    }
}
