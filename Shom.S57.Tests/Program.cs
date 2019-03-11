using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using S57;

namespace Shom.S57.Tests
{
    class Program
    {
        //static void Main(string[] args)
        static void Main()
        {
            //string path = args[0];
            var reader = new S57Reader();
            string ZipName = "NC_ENCs.zip";
            //string MapName = "ENC_ROOT/CATALOG.031";
            string MapName = "ENC_ROOT/US5SC34M/US5SC34M.000";
            string rootPath = "D:/Familie/Documents/Programming/test";
            string zipPath = Path.Combine(rootPath, ZipName);
            var bla = ZipFile.OpenRead(zipPath);
            Stream S57map = bla.GetEntry(MapName).Open();
            reader.ReadCatalogue(S57map);
            //reader.Read(S57map);

            //reader.Read(new FileStream(path, FileMode.Open));





            //ListFeatures(reader);
            //var features = reader.GetFeaturesOfClass(S57Objects.DEPARE);

            //var test1 = features.First(x => x.RecordName == 6140u);
            //var test2 = features.First(x => x.RecordName == 6140u).GetGeometry() as PolygonSet;
            //var test1 = features.First(x => x.RecordName == 6156u);
            //var test2 = features.First(x => x.RecordName == 6156u).GetGeometry() as PolygonSet;
            //var test1 = features.First(x => x.RecordName == 6134u);
            //var test2 = features.First(x => x.RecordName == 6134u).GetGeometry() as PolygonSet;
            //var test1 = features.First(x => x.RecordName == 6155u);
            //var test2 = features.First(x => x.RecordName == 6155u).GetGeometry() as Area;
            //var features = reader.GetFeaturesOfClass(S57Objects.DEPCNT);
            //var test1 = features.First(x => x.RecordName == 6345u);
            //var test2 = features.First(x => x.RecordName == 6345u).GetGeometry() as Line;
            //foreach (var xyz in test2.Areas)
            //{
            //foreach (var xy in xyz.points)
            //foreach (var xy in test2.points)
            //{
            //    var a = xy.X;
            //    var b = xy.Y;
            //    Console.WriteLine($"{a:0.0######}" + " , " + $"{b:0.0######}");
            //}
            //}
            Console.ReadKey();

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
