using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using HReader.Base;

namespace HReader.Builtin.Native
{
    public class ContentSource : IContentSource
    {
        public string Name    => "Offline Repository";
        public string Author  => "HReader";
        public string Version => "0.1.0-alpha";

        public Uri Website { get; } = new Uri("https://github.com/HReader/HReader.Builtin.Native", UriKind.Absolute);

        public ContentSource(string directory)
        {
            this.directory = directory;
        }

        private readonly string directory;

        public bool CanHandle(Uri uri)
        {
            return uri.Scheme.Equals("hrz", StringComparison.OrdinalIgnoreCase)
                && Contains(uri)
                && uri.Fragment.Length > 0;
        }

        public async Task HandleAsync(Uri uri, Func<Stream, Task> consumer)
        {
            var fileName = Resolve(uri);

            using (var zip = ZipFile.OpenRead(fileName))
            {
                using (var stream = zip.GetEntry(uri.Fragment).Open())
                {
                    await consumer(stream);
                }
            }
        }

        private bool Contains(Uri uri)
        {
            return File.Exists(Resolve(uri));
        }

        private string Resolve(Uri uri)
        {
            return Path.Combine(directory, uri.Host + ".hrz");
        }
    }
}
