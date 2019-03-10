#pragma warning disable 1591

#region Header
/*
 * JsonMapper.cs
 *   JSON to .Net object and object to JSON conversions.
 *
 * The authors disclaim copyright to this source code. For more details, see
 * the COPYING file included with this distribution.
 */
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace LitJson
{
    struct PropertyMetadata
    {
        public MemberInfo Info;
        public bool IsField;
        public Type Type;
    }

    struct ArrayMetadata
    {
        Type element_type;
        bool is_array;
        bool is_list;

        public Type ElementType
        {
            get
            {
                if (element_type == null)
                    return typeof(JsonData);

                return element_type;
            }

            set
            {
                element_type = value;
            }
        }

        public bool IsArray
        {
            get
            {
                return is_array;
            }
            set
            {
                is_array = value;
            }
        }

        public bool IsList
        {
            get
            {
                return is_list;
            }
            set
            {
                is_list = value;
            }
        }
    }

    struct ObjectMetadata
    {
        Type element_type;
        bool is_dictionary;

        IDictionary<string, PropertyMetadata> properties;

        public Type ElementType
        {
            get
            {
                if (element_type == null)
                    return typeof(JsonData);

                return element_type;
            }

            set
            {
                element_type = value;
            }
        }

        public bool IsDictionary
        {
            get
            {
                return is_dictionary;
            }
            set
            {
                is_dictionary = value;
            }
        }

        public IDictionary<string, PropertyMetadata> Properties
        {
            get
            {
                return properties;
            }
            set
            {
                properties = value;
            }
        }
    }

    internal delegate void ExporterFunc(object obj, JsonWriter writer);
    public delegate void ExporterFunc<T>(T obj, JsonWriter writer);

    internal delegate object ImporterFunc(object input);
    public delegate TValue ImporterFunc<TJson, TValue>(TJson input);

    public delegate IJsonWrapper WrapperFactory();

    public static class JsonMapper
    {
        const BindingFlags BFlags = BindingFlags.IgnoreCase | BindingFlags.Instance /*| BindingFlags.FlattenHierarchy*/ | BindingFlags.Public | BindingFlags.NonPublic;

        static int max_nesting_depth;

        static IFormatProvider datetime_format;
        static string timespan_format = "c";

        static IDictionary<Type, ExporterFunc> base_exporters_table;
        static IDictionary<Type, ExporterFunc> custom_exporters_table;

        static IDictionary<Type, IDictionary<Type, ImporterFunc>> base_importers_table;
        static readonly IDictionary<Type, IDictionary<Type, ImporterFunc>> custom_importers_table;

        static IDictionary<Type, ArrayMetadata> array_metadata;
        static readonly object array_metadata_lock = new object();

        static IDictionary<Type, IDictionary<Type, MethodInfo>> conv_ops;
        static readonly object conv_ops_lock = new object();

        static IDictionary<Type, ObjectMetadata> object_metadata;
        static readonly object object_metadata_lock = new object();

        static IDictionary<Type, IList<PropertyMetadata>> type_properties;
        static readonly object type_properties_lock = new object();

        static JsonWriter static_writer;
        static readonly object static_writer_lock = new object();

        static JsonMapper()
        {
            max_nesting_depth = 0x100;

            array_metadata = new Dictionary<Type, ArrayMetadata>();
            conv_ops = new Dictionary<Type, IDictionary<Type, MethodInfo>>();
            object_metadata = new Dictionary<Type, ObjectMetadata>();
            type_properties = new Dictionary<Type, IList<PropertyMetadata>>();

            static_writer = new JsonWriter();

            datetime_format = DateTimeFormatInfo.InvariantInfo;

            base_exporters_table = new Dictionary<Type, ExporterFunc>();
            custom_exporters_table = new Dictionary<Type, ExporterFunc>();

            base_importers_table = new Dictionary<Type, IDictionary<Type, ImporterFunc>>();
            custom_importers_table = new Dictionary<Type, IDictionary<Type, ImporterFunc>>();

            RegisterBaseExporters();
            RegisterBaseImporters();
        }

        static void AddArrayMetadata (Type type)
        {
            if (array_metadata.ContainsKey(type))
                return;

            var data = new ArrayMetadata();

            data.IsArray = type.IsArray;

            data.IsList |= Array.IndexOf(type.GetInterfaces(), typeof(IList)) != -1;

            foreach (PropertyInfo p_info in type.GetProperties())
            {
                if (p_info.Name != "Item")
                    continue;

                ParameterInfo[] parameters = p_info.GetIndexParameters();

                if (parameters.Length != 1)
                    continue;

                if (parameters[0].ParameterType == typeof(int))
                    data.ElementType = p_info.PropertyType;
            }

            lock (array_metadata_lock)
            {
                try
                {
                    array_metadata.Add(type, data);
                }
                catch (ArgumentException)
                {
                    return;
                }
            }
        }
        static void AddObjectMetadata(Type type)
        {
            if (object_metadata.ContainsKey(type))
                return;

            var data = new ObjectMetadata();

            if (Array.IndexOf(type.GetInterfaces(), typeof(IDictionary)) != -1)
                data.IsDictionary = true;

            data.Properties = new Dictionary<string, PropertyMetadata>();

            foreach (var p_info in type.GetProperties(BFlags))
            {
                if (p_info.Name == "Item" && (p_info.Attributes & PropertyAttributes.SpecialName) != 0)
                {
                    var parameters = p_info.GetIndexParameters();

                    if (parameters.Length != 1)
                        continue;

                    if (parameters[0].ParameterType == typeof(string))
                        data.ElementType = p_info.PropertyType;

                    continue;
                }

                var p_data = new PropertyMetadata();
                p_data.Info = p_info;
                p_data.Type = p_info.PropertyType;

                PropertyMetadata existing;
                if (data.Properties.TryGetValue(p_info.Name, out existing) && existing.Info is PropertyInfo)
                {
                    var foo = existing.Info as PropertyInfo;
                    // already exists!
                    if ((p_info.GetMethod.Attributes & MethodAttributes.NewSlot) != 0 &&
                            (foo.GetMethod.Attributes & MethodAttributes.NewSlot) == 0)
                    {
                        // new one overrides the old one
                        data.Properties[p_info.Name] = p_data;
                    }
                }
                else
                    data.Properties.Add(p_info.Name, p_data);

                foreach (var o in p_info.GetCustomAttributes(typeof(JsonPropertyNameAttribute), true))
                {
                    var jpn = (JsonPropertyNameAttribute)o;

                    if (!data.Properties.ContainsKey(jpn.Name))
                        data.Properties.Add(jpn.Name, p_data);
                }
            }

            foreach (var f_info in type.GetFields(BFlags))
            {
                var p_data = new PropertyMetadata();
                p_data.Info = f_info;
                p_data.IsField = true;
                p_data.Type = f_info.FieldType;

                data.Properties.Add(f_info.Name, p_data);

                foreach (var o in f_info.GetCustomAttributes(typeof(JsonPropertyNameAttribute), true))
                {
                    var jpn = (JsonPropertyNameAttribute)o;

                    if (!data.Properties.ContainsKey(jpn.Name))
                        data.Properties.Add(jpn.Name, p_data);
                }
            }

            lock (object_metadata_lock)
            {
                try
                {
                    object_metadata.Add(type, data);
                }
                catch (ArgumentException)
                {
                    return;
                }
            }
        }
        static void AddTypeProperties(Type type)
        {
            if (type_properties.ContainsKey(type))
                return;

            IList<PropertyMetadata> props = new List<PropertyMetadata>();

            foreach (PropertyInfo p_info in type.GetProperties(BFlags))
            {
                if (p_info.Name == "Item")
                    continue;

                var p_data = new PropertyMetadata();
                p_data.Info = p_info;
                p_data.IsField = false;
                props.Add(p_data);
            }

            foreach (FieldInfo f_info in type.GetFields(BFlags))
            {
                var p_data = new PropertyMetadata();
                p_data.Info = f_info;
                p_data.IsField = true;

                props.Add(p_data);
            }

            lock (type_properties_lock)
            {
                try
                {
                    type_properties.Add(type, props);
                }
                catch (ArgumentException)
                {
                    return;
                }
            }
        }

        static MethodInfo GetConvOp(Type t1, Type t2)
        {
            lock (conv_ops_lock)
            {
                if (!conv_ops.ContainsKey(t1))
                    conv_ops.Add(t1, new Dictionary<Type, MethodInfo>());
            }

            if (conv_ops[t1].ContainsKey(t2))
                return conv_ops[t1][t2];

            MethodInfo op = t1.GetMethod("op_Implicit", new[] { t2 });

            lock (conv_ops_lock)
            {
                try
                {
                    conv_ops[t1].Add(t2, op);
                }
                catch (ArgumentException)
                {
                    return conv_ops[t1][t2];
                }
            }

            return op;
        }

        static bool TryGetValueIgnCase<T>(IDictionary<string, T> dict, string key, out T value)
        {
            foreach (var kvp in dict)
                if (kvp.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    value = kvp.Value;
                    return true;
                }

            value = default(T);
            return false;
        }

        static object ConvertTo(Type to, object o, Type from = null)
        {
            if (o != null)
                from = from ?? o.GetType();

            if (o == null || (o is string && String.IsNullOrEmpty((string)o) && !(to == typeof(string))))
            {
                if (to.IsGenericType && to.GetGenericTypeDefinition() == typeof(Nullable<>))
                    return Activator.CreateInstance(to); // empty nullable
                if (!to.IsClass && !to.IsArray)
                    throw new JsonException(String.Format("Can't assign null to an instance of type {0}", to));

                return null;
            }

            if (to.IsAssignableFrom(from))
                return o;

            // If there's a custom importer that fits, use it
            if (custom_importers_table.ContainsKey(from) && custom_importers_table[from].ContainsKey(to))
                return custom_importers_table[from][to](o);

            // Maybe there's a base importer that works
            if (base_importers_table.ContainsKey(from) && base_importers_table[from].ContainsKey(to))
                return base_importers_table[from][to](o);

            // Maybe it's an enum
            if (to.IsEnum)
                return Enum.ToObject(to, o);

            // Try using an implicit conversion operator
            MethodInfo conv_op = GetConvOp(to, from);

            if (conv_op != null)
                return conv_op.Invoke(null, new[] { o });

            throw new JsonException(String.Format("Can't assign value '{0}' (type {1}) to type {2}", o, from, to));
        }

        static object ReadValue(Type inst_type, JsonReader reader, bool readBegin = true)
        {
            if (readBegin)
                reader.Read();

            if (reader.Token == JsonToken.ArrayEnd)
                return null;

            if (reader.Token == JsonToken.Null || (reader.Token == JsonToken.String && String.IsNullOrEmpty((string)reader.Value) && !(inst_type == typeof(string))))
            {
                if (!inst_type.IsClass && !inst_type.IsArray)
                    throw new JsonException(String.Format("Can't assign null to an instance of type {0}", inst_type));

                return null;
            }

            if (inst_type.IsGenericType && inst_type.GetGenericTypeDefinition() == typeof(Nullable<>)) // isn't null -> has a value
                return Activator.CreateInstance(inst_type, ReadValue(inst_type.GetGenericArguments()[0], reader, false));

            if (reader.Token == JsonToken.Double ||
                    reader.Token == JsonToken.Int    ||
                    reader.Token == JsonToken.Long   ||
                    reader.Token == JsonToken.String ||
                    reader.Token == JsonToken.Boolean)
                return ConvertTo(inst_type, reader.Value, reader.Value.GetType());

            object instance = null;

            if (reader.Token == JsonToken.ArrayStart)
            {
                AddArrayMetadata(inst_type);
                ArrayMetadata t_data = array_metadata[inst_type];

                if (!t_data.IsArray && !t_data.IsList)
                    throw new JsonException(String.Format("Type {0} can't act as an array", inst_type));

                IList list;
                Type elem_type;

                if (!t_data.IsArray)
                {
                    list = (IList)Activator.CreateInstance(inst_type);
                    elem_type = t_data.ElementType;
                }
                else
                {
                    list = new List<object>();
                    elem_type = inst_type.GetElementType();
                }

                while (true)
                {
                    object item = ReadValue(elem_type, reader);
                    if (item == null && reader.Token == JsonToken.ArrayEnd)
                        break;

                    list.Add(item);
                }

                if (t_data.IsArray)
                {
                    int n = list.Count;
                    instance = Array.CreateInstance(elem_type, n);

                    for (int i = 0; i < n; i++)
                        ((Array)instance).SetValue(list[i], i);
                }
                else
                    instance = list;
            }
            else if (reader.Token == JsonToken.ObjectStart)
            {
                AddObjectMetadata(inst_type);
                ObjectMetadata t_data = object_metadata[inst_type];

                instance = Activator.CreateInstance(inst_type);

                while (true)
                {
                    reader.Read();

                    if (reader.Token == JsonToken.ObjectEnd)
                        break;

                    var property = (string)reader.Value;

                    PropertyMetadata prop_data;
                    if (TryGetValueIgnCase(t_data.Properties, property, out prop_data))
                    {
                        if (prop_data.IsField)
                            ((FieldInfo)prop_data.Info).SetValue(instance, ReadValue(prop_data.Type, reader));
                        else
                        {
                            var p_info = (PropertyInfo)prop_data.Info;

                            var v = ReadValue(prop_data.Type, reader);

                            if (p_info.CanWrite)
                                p_info.SetValue(instance, ConvertTo(p_info.PropertyType, v), null);
                        }
                    }
                    else
                    {
                        if (!t_data.IsDictionary)
                        {
                            if (!reader.SkipNonMembers)
                                throw new JsonException(String.Format("The type {0} doesn't have the property '{1}'", inst_type, property));

                            ReadSkip(reader);
                            continue;
                        }

                        ((IDictionary)instance).Add(property, ReadValue(t_data.ElementType, reader));
                    }
                }
            }

            return instance;
        }
        static object ReadValue(Type inst_type, JsonData   json  )
        {
            if (json == null || json.JsonType == JsonType.None || (json.JsonType == JsonType.String && String.IsNullOrEmpty((string)json.Value) && !(inst_type == typeof(string))))
            {
                if (inst_type.IsGenericType && inst_type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    return Activator.CreateInstance(inst_type); // empty nullable
                if (!inst_type.IsClass && !inst_type.IsArray)
                    throw new JsonException(String.Format("Can't assign null to an instance of type {0}", inst_type));

                return null;
            }

            object[] attrs;
            if ((attrs = inst_type.GetCustomAttributes(typeof(JsonConverterAttribute), false)) != null && attrs.Length == 1)
            {
                var jca = (JsonConverterAttribute)attrs[0];

                var jc = (IJsonConverter)Activator.CreateInstance(jca.Converter);

                object v_;
                if (jc.Deserialize(json, inst_type, out v_))
                    return v_;
            }

            if (inst_type.IsGenericType && inst_type.GetGenericTypeDefinition() == typeof(Nullable<>)) // 'json' isn't null -> has a value
                return Activator.CreateInstance(inst_type, ReadValue(inst_type.GetGenericArguments()[0], json));

            var v = json.Value;

            switch (json.JsonType)
            {
                case JsonType.Double:
                case JsonType.Int:
                case JsonType.Long:
                case JsonType.String:
                case JsonType.Boolean:
                    return ConvertTo(inst_type, json.Value, json.NetType(inst_type));
                case JsonType.Array:
                    {
                        AddArrayMetadata(inst_type);
                        ArrayMetadata t_data = array_metadata[inst_type];

                        if (!t_data.IsArray && !t_data.IsList)
                            throw new JsonException(String.Format("Type {0} can't act as an array", inst_type));

                        var inList = (IList<JsonData>)v;

                        IList list;
                        Type elem_type;

                        if (!t_data.IsArray)
                        {
                            list = (IList)Activator.CreateInstance(inst_type);
                            elem_type = t_data.ElementType;
                        }
                        else
                        {
                            list = new List<object>();
                            elem_type = inst_type.GetElementType();
                        }

                        for (int i = 0; i < inList.Count; i++)
                            list.Add(ReadValue(elem_type, inList[i]));

                        object inst;
                        if (t_data.IsArray)
                        {
                            int n = list.Count;
                            inst = Array.CreateInstance(elem_type, n);

                            for (int i = 0; i < n; i++)
                                ((Array)inst).SetValue(list[i], i);
                        }
                        else
                            inst = list;

                        return inst;
                    }
                case JsonType.Object:
                    {
                        AddObjectMetadata(inst_type);
                        ObjectMetadata t_data = object_metadata[inst_type];

                        //throw new Exception("type is " + inst_type);
                        var inst = Activator.CreateInstance(inst_type);

                        var dict = (IDictionary<string, JsonData>)v;

                        foreach (var kvp in dict)
                        {
                            var prop  = kvp.Key  ;
                            var value = kvp.Value;

                            PropertyMetadata prop_data;
                            if (TryGetValueIgnCase(t_data.Properties, prop, out prop_data))
                            {
                                if (prop_data.IsField)
                                    ((FieldInfo)prop_data.Info).SetValue(inst, ReadValue(prop_data.Type, value));
                                else
                                {
                                    var p_info = (PropertyInfo)prop_data.Info;

                                    var v_ = ReadValue(prop_data.Type, value);
                                    object converted = null;

                                    if ((attrs = p_info.GetCustomAttributes(typeof(JsonConverterAttribute), false)) != null && attrs.Length == 1)
                                    {
                                        var jca = (JsonConverterAttribute)attrs[0];

                                        var jc = (IJsonConverter)Activator.CreateInstance(jca.Converter);

                                        jc.Deserialize(value, p_info.PropertyType, out converted);
                                    }

                                    if (p_info.CanWrite)
                                        p_info.SetValue(inst, converted ?? ConvertTo(p_info.PropertyType, v_), null);
                                    else
                                        throw new MemberAccessException("Cannot set property '" + p_info + "' to '" + v_ + "'.");
                                }
                            }
                            else
                            {
                                if (!t_data.IsDictionary)
                                    throw new JsonException(String.Format("The type {0} doesn't have the property '{1}'", inst_type, prop));

                                ((IDictionary)inst).Add(prop, ReadValue(t_data.ElementType, value));
                            }
                        }

                        return inst;
                    }
                default:
                    return null;
            }
        }

        static IJsonWrapper ReadValue(WrapperFactory factory, JsonReader reader)
        {
            reader.Read();

            if (reader.Token == JsonToken.ArrayEnd || reader.Token == JsonToken.Null)
                return null;

            IJsonWrapper instance = factory();

            if (reader.Token == JsonToken.String)
            {
                instance.SetString((string)reader.Value);
                return instance;
            }

            if (reader.Token == JsonToken.Double)
            {
                instance.SetDouble((double)reader.Value);
                return instance;
            }

            if (reader.Token == JsonToken.Int)
            {
                instance.SetInt((int)reader.Value);
                return instance;
            }

            if (reader.Token == JsonToken.Long)
            {
                instance.SetLong((long)reader.Value);
                return instance;
            }

            if (reader.Token == JsonToken.Boolean)
            {
                instance.SetBoolean((bool)reader.Value);
                return instance;
            }

            if (reader.Token == JsonToken.ArrayStart)
            {
                instance.SetJsonType(JsonType.Array);

                while (true)
                {
                    IJsonWrapper item = ReadValue(factory, reader);
                    if (item == null && reader.Token == JsonToken.ArrayEnd)
                        break;

                    instance.Add(item);
                }
            }
            else if (reader.Token == JsonToken.ObjectStart)
            {
                instance.SetJsonType(JsonType.Object);

                while (true)
                {
                    reader.Read();

                    if (reader.Token == JsonToken.ObjectEnd)
                        break;

                    var property = (string)reader.Value;

                    instance[property] = ReadValue(factory, reader);
                }

            }

            return instance;
        }

        static void ReadSkip(JsonReader reader)
        {
            ToWrapper(() => new JsonMockWrapper(), reader);
        }

        static void RegisterBaseExporters()
        {
            base_exporters_table[typeof(char)] = (obj, writer) => writer.Write(obj.ToString());

            base_exporters_table[typeof(DateTime)] = (obj, writer) => writer.Write(((DateTime)obj).ToString(datetime_format));
            base_exporters_table[typeof(TimeSpan)] = (obj, writer) => writer.Write(((TimeSpan)obj).ToString(timespan_format));

            base_exporters_table[typeof(Version)] = (obj, writer) => writer.Write(((Version)obj).ToString(4));
            base_exporters_table[typeof(Guid   )] = (obj, writer) => writer.Write(((Guid   )obj).ToString( ));

            base_exporters_table[typeof(Uri)] = (obj, writer) => writer.Write(((Uri)obj).OriginalString);

            base_exporters_table[typeof(byte   )] = (obj, writer) => writer.Write((byte   )obj);
            base_exporters_table[typeof(decimal)] = (obj, writer) => writer.Write((decimal)obj);
            base_exporters_table[typeof(float  )] = (obj, writer) => writer.Write((float  )obj);
            base_exporters_table[typeof(sbyte  )] = (obj, writer) => writer.Write((sbyte  )obj);
            base_exporters_table[typeof(short  )] = (obj, writer) => writer.Write((short  )obj);
            base_exporters_table[typeof(ushort )] = (obj, writer) => writer.Write((ushort )obj);
            base_exporters_table[typeof(uint   )] = (obj, writer) => writer.Write((uint   )obj);
            base_exporters_table[typeof(ulong  )] = (obj, writer) => writer.Write((ulong  )obj);
        }
        static void RegisterBaseImporters()
        {
            RegisterImporter(base_importers_table, typeof(int), typeof(byte   ), input => (byte   )(int)input);
            RegisterImporter(base_importers_table, typeof(int), typeof(ulong  ), input => (ulong  )(int)input);
            RegisterImporter(base_importers_table, typeof(int), typeof(sbyte  ), input => (sbyte  )(int)input);
            RegisterImporter(base_importers_table, typeof(int), typeof(short  ), input => (short  )(int)input);
            RegisterImporter(base_importers_table, typeof(int), typeof(ushort ), input => (ushort )(int)input);
            RegisterImporter(base_importers_table, typeof(int), typeof(uint   ), input => unchecked((uint)(int)input));
            RegisterImporter(base_importers_table, typeof(int), typeof(float  ), input => (float  )(int)input);
            RegisterImporter(base_importers_table, typeof(int), typeof(double ), input => (double )(int)input);
            RegisterImporter(base_importers_table, typeof(int), typeof(decimal), input => (decimal)(int)input);

            RegisterImporter(base_importers_table, typeof(double), typeof(decimal), input => (decimal)(double)input);
            RegisterImporter(base_importers_table, typeof(double), typeof(float  ), input => (float  )(double)input);

            RegisterImporter(base_importers_table, typeof(long), typeof(uint), input => unchecked((uint)(long)input));

            RegisterImporter(base_importers_table, typeof(string), typeof(char    ), input => Convert.ToChar((string)input));

            RegisterImporter(base_importers_table, typeof(string), typeof(DateTime), input => DateTime.Parse((string)input, datetime_format));
            RegisterImporter(base_importers_table, typeof(string), typeof(TimeSpan), input => TimeSpan.Parse((string)input                 ));

            RegisterImporter(base_importers_table, typeof(string), typeof(Version), input => Version.Parse((string)input));
            RegisterImporter(base_importers_table, typeof(string), typeof(Guid   ), input => Guid   .Parse((string)input));

            RegisterImporter(base_importers_table, typeof(string), typeof(Uri), input => new Uri((string)input, UriKind.RelativeOrAbsolute));
        }

        static void RegisterImporter(IDictionary<Type, IDictionary<Type, ImporterFunc>> table, Type json_type, Type value_type, ImporterFunc importer)
        {
            if (!table.ContainsKey(json_type))
                table.Add(json_type, new Dictionary<Type, ImporterFunc>());

            table[json_type][value_type] = importer;
        }

        static void WriteValue(object obj, JsonWriter writer, bool writer_is_private, int depth)
        {
            if (depth > max_nesting_depth)
                throw new JsonException(String.Format("Max allowed object depth reached while trying to export from type {0}", obj.GetType()));

            if (obj == null)
            {
                writer.Write(null);
                return;
            }

            if (obj is IJsonWrapper)
            {
                if (writer_is_private)
                    writer.TextWriter.Write(((IJsonWrapper)obj).ToJson());
                else
                    ((IJsonWrapper)obj).ToJson(writer);

                return;
            }

            if (obj is string)
            {
                writer.Write((string)obj);
                return;
            }

            if (obj is double)
            {
                writer.Write((double)obj);
                return;
            }

            if (obj is int)
            {
                writer.Write((int)obj);
                return;
            }

            if (obj is bool)
            {
                writer.Write((bool)obj);
                return;
            }

            if (obj is long)
            {
                writer.Write((long)obj);
                return;
            }

            if (obj is Array)
            {
                writer.WriteArrayStart();

                foreach (object elem in (Array)obj)
                    WriteValue(elem, writer, writer_is_private, depth + 1);

                writer.WriteArrayEnd();

                return;
            }

            if (obj is IList)
            {
                writer.WriteArrayStart();
                foreach (object elem in (IList)obj)
                    WriteValue(elem, writer, writer_is_private, depth + 1);
                writer.WriteArrayEnd();

                return;
            }

            if (obj is IDictionary)
            {
                writer.WriteObjectStart();
                foreach (DictionaryEntry entry in (IDictionary)obj)
                {
                    writer.WritePropertyName((string)entry.Key);
                    WriteValue(entry.Value, writer, writer_is_private,depth + 1);
                }
                writer.WriteObjectEnd();

                return;
            }

            Type obj_type = obj.GetType();

            // See if there's a custom exporter for the object
            if (custom_exporters_table.ContainsKey(obj_type))
            {
                custom_exporters_table[obj_type](obj, writer);

                return;
            }

            // If not, maybe there's a base exporter
            if (base_exporters_table.ContainsKey(obj_type))
            {
                base_exporters_table[obj_type](obj, writer);

                return;
            }

            // Last option, let's see if it's an enum
            if (obj is Enum)
            {
                Type e_type = Enum.GetUnderlyingType(obj_type);

                if (e_type == typeof(long) || e_type == typeof(uint) || e_type == typeof(ulong))
                    writer.Write((ulong)obj);
                else
                    writer.Write((int)obj);

                return;
            }

            // Okay, so it looks like the input should be exported as an
            // object
            AddTypeProperties(obj_type);
            IList<PropertyMetadata> props = type_properties[obj_type];

            writer.WriteObjectStart();
            foreach (PropertyMetadata p_data in props)
                if (p_data.IsField)
                {
                    writer.WritePropertyName(p_data.Info.Name);
                    WriteValue(((FieldInfo)p_data.Info).GetValue(obj), writer, writer_is_private, depth + 1);
                }
                else
                {
                    var p_info = (PropertyInfo)p_data.Info;

                    if (p_info.CanRead)
                    {
                        writer.WritePropertyName(p_data.Info.Name);
                        WriteValue(p_info.GetValue(obj, null), writer, writer_is_private, depth + 1);
                    }
                }

            writer.WriteObjectEnd();
        }

        public static string ToJson(object obj)
        {
            lock (static_writer_lock)
            {
                static_writer.Reset();

                WriteValue(obj, static_writer, true, 0);

                return static_writer.ToString();
            }
        }
        public static void   ToJson(object obj, JsonWriter writer)
        {
            WriteValue(obj, writer, false, 0);
        }

        public static JsonData ToObject(JsonReader reader) => (JsonData)ToWrapper(() => new JsonData(), reader);
        public static JsonData ToObject(TextReader reader) => (JsonData)ToWrapper(() => new JsonData(), new JsonReader(reader));

        public static JsonData ToObject(string json) => (JsonData)ToWrapper(() => new JsonData(), json);

        public static T ToObject<T>(JsonReader reader) => (T)ReadValue(typeof(T), reader);
        public static T ToObject<T>(TextReader reader) => (T)ReadValue(typeof(T), new JsonReader(reader));
        public static T ToObject<T>(string     json  ) => (T)ReadValue(typeof(T), new JsonReader(json  ));
        public static T ToObject<T>(JsonData   json  ) => (T)ReadValue(typeof(T), json  );

        public static IJsonWrapper ToWrapper(WrapperFactory factory, JsonReader reader) => ReadValue(factory, reader);
        public static IJsonWrapper ToWrapper(WrapperFactory factory, string json) => ReadValue(factory, new JsonReader(json));

        public static void RegisterExporter<T>(ExporterFunc<T> exporter)
        {
            custom_exporters_table[typeof(T)] = (obj, writer) => exporter((T)obj, writer);
        }
        public static void RegisterImporter<TJson, TValue>(ImporterFunc<TJson, TValue> importer)
        {
            RegisterImporter(custom_importers_table, typeof(TJson), typeof(TValue), input => importer((TJson)input));
        }

        public static void UnregisterExporters()
        {
            custom_exporters_table.Clear();
        }
        public static void UnregisterImporters()
        {
            custom_importers_table.Clear();
        }
    }
}
