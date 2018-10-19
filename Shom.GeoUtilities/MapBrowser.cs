using System.Collections.Generic;
using System.IO;

namespace Shom.GeoUtilities
{
    public class MapBrowser
    {
        public MapBrowser(string directory, string filter = null)
        {
            _directory = directory;
            _filter = filter;
        }

        public IEnumerable<string> Maps()
        {
            string[] directories = Directory.GetDirectories(_directory, _filter != null ? _filter : "*.*");
            foreach (string directory in directories)
            {
                string[] file = Directory.GetFiles(directory, "*.000");
                yield return file[0];
            }
        }

        private string _directory;
        private string _filter;
    }
}
