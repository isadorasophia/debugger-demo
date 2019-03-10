using System;
using System.Collections.Generic;
using System.Linq;

namespace LitJson
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class JsonPropertyNameAttribute : Attribute
    {
        public string Name
        {
            get;
        }

        public JsonPropertyNameAttribute(string name)
        {
            Name = name;
        }
    }
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public sealed class JsonConverterAttribute : Attribute
    {
        public Type Converter
        {
            get;
        }

        public JsonConverterAttribute(Type conv)
        {
            Converter = conv;
        }
    }

    public interface IJsonConverter
    {
        bool Deserialize(JsonData j, Type t, out object value);
    }
}
