using System;
using System.IO;
using System.Threading.Tasks;

namespace PokeAPI
{
    public interface IDataBackend
    {
        // path is relative to eg. "https://pokeapi.co/api/v2/"
        Task<string> GetStringAsync(string path);
        Task<Stream> GetStreamAsync(string path);

        string MakePathRelative(string path);
        Uri    MakeUriRelative (Uri    uri );
    }
}

