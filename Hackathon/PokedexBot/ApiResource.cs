using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LitJson;

namespace PokeAPI
{
    public interface IResource<T>
    {
        Uri Url
        {
            get;
        }

        Task<T> GetObject();
    }

    public class StructResource<T> : IResource<T>
    {
        /// <summary>
        /// The path to the referenced data.
        /// </summary>
        public string Path
        {
            get;
            internal set;
        }

        public Uri Url
        {
            get
            {
                if (Path.StartsWith("http"))
                    // TODO: make this relative
                    //return new Uri(Path, UriKind.Absolute);
                    throw new NotImplementedException("Absolute URLs in resources not implemented yet.");

                return new Uri(Path, UriKind.Relative);
            }
        }

        public virtual async Task<T> GetObject() => await DataFetcher.GetAny<T>(Url);
    }
    internal class StructResourceFromStringConverter<T> : IJsonConverter
    {
        public bool Deserialize(JsonData j, Type t /* StructResource<T> */, out object value)
        {
            if (j.JsonType != JsonType.String)
            {
                value = null;
                return false;
            }

            value = new StructResource<T> { Path = (string)j };
            return true;
        }
    }

    public class ApiResource<T> : IResource<T>
        where T : ApiObject
    {
        /// <summary>
        /// The URL of the referenced resource.
        /// </summary>
        public Uri Url
        {
            get;
            internal set;
        }

        public virtual async Task<T> GetObject() => await DataFetcher.GetApiObject<T>(Url);
    }
    public class NamedApiResource<T> : ApiResource<T>
        where T : NamedApiObject
    {
        /// <summary>
        /// The name of the referenced resource.
        /// </summary>
        public string Name
        {
            get;
            internal set;
        }
    }
    internal class ApiResourceFromStringConverter<T> : IJsonConverter
        where T : ApiObject
    {
        public bool Deserialize(JsonData j, Type t /* ApiResource<T> */, out object value)
        {
            if (j.JsonType != JsonType.String)
            {
                value = null;
                return false;
            }

            value = new ApiResource<T> { Url = new Uri((string)j, UriKind.Absolute) };
            return true;
        }
    }
}
