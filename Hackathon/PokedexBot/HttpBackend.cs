using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace PokeAPI
{
    public class HttpBackend : IDataBackend
    {
        public readonly static string
            DefaultSiteURL = "https://pokeapi.co",
            DefaultBaseURL = DefaultSiteURL + "/api/v2/",
            DefaultUserAgent =
        "PokeAPI.NET (https://gitlab.com/PoroCYon/PokeApi.NET or a fork of it)",

            SLASH = "/";

        readonly HttpClient client = new HttpClient();
        readonly string ubase;

        public HttpBackend(string baseurl = null, string userAgent = null)
        {
            ubase = String.IsNullOrWhiteSpace(baseurl)
                ? DefaultBaseURL : baseurl;

            if (ubase[ubase.Length - 1] != '/') ubase += SLASH;

            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                    String.IsNullOrWhiteSpace(userAgent)
                        ? DefaultUserAgent : userAgent);
        }

        public Task<Stream> GetStreamAsync(string path) =>
            client.GetStreamAsync(ubase + path);
        public Task<string> GetStringAsync(string path) =>
            client.GetStringAsync(ubase + path);

        public string MakePathRelative(string path)
        {
            if (path.StartsWith(ubase)) return path.Substring(ubase.Length);

            return path; // relative or invalid
        }
        public Uri    MakeUriRelative (Uri    uri )
        {
            // fuck it.
            if (!uri.IsAbsoluteUri) return uri; // might be invalid
            return new Uri(MakePathRelative(uri.AbsolutePath), UriKind.Relative);
        }
    }
}
