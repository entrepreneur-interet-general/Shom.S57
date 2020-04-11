using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using S57;
using Shom.ISO8211;

namespace Shom.S57.Tests
{
    class Program
    {
        //static void Main(string[] args)
        static void Main()
        {
            var reader = new S57Reader();
            string ZipName = "NC_ENCs.zip";
            string rootPath = "D:/Familie/Documents/Programming/test";
            string zipPath = Path.Combine(rootPath, ZipName);
            ZipArchive archive = ZipFile.OpenRead(zipPath);
            //reader.ReadCatalogue(S57map);
            //string MapName = "CATALOG.031";
            //reader.ReadArchiveCatalogue(archive, MapName);

            //string MapName = "US5NC12M.000";
            string MapName = "US5NC18M.000";
            //string MapName = "US5NC51M.000";
            reader.ReadFromArchive(archive, MapName, true);
            archive.Dispose();
            //ListRelationships(reader, S57Obj.BOYSAW);
            //ListFeatures(reader);
            //QalityOfPosition(reader);

            //reader.Read(new FileStream(path, FileMode.Open));

            FlatPolygon TempSet;
            var features = reader.GetFeaturesOfClass(S57Obj.DEPARE);
            for (int i = 0; i < features.Count; i++)
            {
                //if (features[i].namekey.RecordIdentificationNumber == 6163)
                //{
                if (features[i].Primitive == GeometricPrimitive.Area)
                {
                    TempSet = features[i].GetGeometry(true) as FlatPolygon;
                }
                //}
            }
            //var test1 = features.First(x => x.RecordName == 6140u);
            //var test2 = features.First(x => x.RecordName == 6140u).GetGeometry() as PolygonSet;
            //var test1 = features.First(x => x.RecordName == 6156u);
            //var test2 = features.First(x => x.RecordName == 6156u).GetGeometry() as PolygonSet;
            //var test1 = features.First(x => x.RecordName == 6134u);
            //var test2 = features.First(x => x.RecordName == 6134u).GetGeometry() as PolygonSet;
            //var test1 = features.First(x => x.RecordName == 6155u);
            //var test2 = features.First(x => x.RecordName == 6155u).GetGeometry() as Area;
            //var features = reader.GetFeaturesOfClass(S57Objects.SOUNDG);
            //for (int i = 0; i < features.Count; i++)
            //{
            //    if (features[i].Primitive == GeometricPrimitive.Point)
            //    {
            //        var TempSet = features[i].ExtractSoundings();
            //        var TempSet2 = features[i].VectorPtrs[0].Vector.SoundingList;
            //    }
            //    else
            //    {
            //        continue;
            //    }
            //}

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
        private static void ListRelationships(S57Reader reader, S57Obj type)
        {
            var features = reader.GetFeaturesOfClass(type);
            for (int i = 0; i < features.Count; i++)
            {
                string description = null;
                features[i].Attributes.TryGetValue(S57Att.OBJNAM, out description);
                string rcid = features[i].namekey.RecordIdentificationNumber.ToString();
                string targetinfo = null;
                int counter = 0;
                if (features[i].enhFeaturePtrs != null)
                {
                    foreach (var bla in features[i].enhFeaturePtrs.FeatureList)
                    {
                        string relationship = ((Relationship)features[i].FeatureRecord.Fields.GetFieldByTag("FFPT").subFields.GetUInt32(counter++, "RIND")).ToString();
                        string targettype = ((S57Obj)(bla.ObjectCode)).ToString();
                        string targetname = bla.namekey.RecordIdentificationNumber.ToString();
                        targetinfo = targetinfo + " |" + targetname + "." + targettype + "." + relationship;
                    }
                }
                Console.WriteLine(rcid + "." + description + targetinfo);
            }
        }

        private static void QalityOfPosition(S57Reader reader)
        {
            foreach (var bla in Enum.GetValues(typeof(S57Obj)))
            {
                var features = reader.GetFeaturesOfClass((S57Obj)bla);
                for (int i = 0; i < features.Count; i++)
                {
                    var bla2 = features[i].GetSpacialAttributes();
                }
            }
        }
        private static void ListFeatures(S57Reader reader)
        {
            foreach (var obj in S57ObjectInfo.S57Dict)
            {
                int a = reader.GetFeaturesOfClass(obj.Key).Count;
                string test = obj.Key.ToString();
                if (a > 0)
                {
                    Console.WriteLine(test + ": " + "{0:G}", a.ToString());
                }
            }
        }

    }
}
