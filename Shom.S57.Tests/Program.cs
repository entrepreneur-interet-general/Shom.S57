using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using S57;

namespace Shom.S57.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = args[0];
            var reader = new S57Reader();
            reader.Read(new FileStream(path, FileMode.Open));
            var features = reader.GetFeaturesOfClass(119 );
        }
    }
}
