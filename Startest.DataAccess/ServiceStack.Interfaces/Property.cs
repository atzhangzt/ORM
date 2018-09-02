//Copyright (c) Service Stack LLC. All Rights Reserved.
//License: https://raw.github.com/ServiceStack/ServiceStack/master/license.txt

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ServiceStack
{
    [DataContract]
    public class Property
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Value { get; set; }
    }
}