using System;
using System.IO;
using System.Threading.Tasks;

namespace PokeAPI
{
    public class FileBackend : IDataBackend
    {
        readonly static string DEF_BASE = Environment.CurrentDirectory;

        readonly string pbase;

        public FileBackend(string basepath)
        {
            pbase = String.IsNullOrWhiteSpace(basepath) ? DEF_BASE : basepath;
        }

        // for crying out loud, how hard does it have to be?!
        static Task<T> MakeAsync<T>(T t)
        {
            var r = new Task<T>(() => t);
            r.RunSynchronously();
            return r;
        }

        public Task<Stream> GetStreamAsync(string path) =>
            // and why is this cast even required?
            MakeAsync((Stream)File.OpenRead   (Path.Combine(pbase, path)));
        public Task<string> GetStringAsync(string path) =>
            MakeAsync(        File.ReadAllText(Path.Combine(pbase, path)));

        public string MakePathRelative(string path)
        {
            if (path.StartsWith(pbase))
            {
                int ind = pbase.Length;
                if (path.Length > ind && path[ind] == Path.DirectorySeparatorChar)
                    ind++;

                return path.Substring(ind);
            }

            return path; // relative or invalid
        }
        public Uri    MakeUriRelative (Uri    uri )
        {
            if (!uri.IsAbsoluteUri) return uri; // might be invalid
            return new Uri(MakePathRelative(uri.AbsolutePath), UriKind.Relative);
        }
    }
}

