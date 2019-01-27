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
			ListFeatures(reader);
            var features = reader.GetFeaturesOfClass(119 );
			
        }
		private static void ListFeatures(S57Reader reader)
        {
            for (uint i = 1; i < 500; i++)
            {
                if (S57Objects.IsIn(i))
                {
                    int a = reader.GetFeaturesOfClass(i).Count();
                    string test = S57Objects.Get(i).Acronym.ToString();
                    if (a > 0)
                    {
                        Console.WriteLine(test + ": " + "{0:G}", a.ToString());
                    }
                }
            }
        }

    }
}
