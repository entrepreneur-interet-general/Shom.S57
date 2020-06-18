using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using S57;
using Shom.ISO8211;
using System.Diagnostics;

namespace Shom.S57.Tests
{
    class Program
    {
        //static void Main(string[] args)
        static void Main()
        {
            S57Reader reader;
            string ZipName = "NC_ENCs.zip";
            string zipRootPath = "D:/Familie/Documents/Programming/test";
            string zipPath = Path.Combine(zipRootPath, ZipName);
            string VolumeRootPath = "D:/Familie/Documents/Programming/test/ENC_ROOT";
            string MapName = "US5NC51M.000"; //US5NC12M.000  US5NC51M.000
            Stopwatch timer = new Stopwatch();

            //timer.Start();
            //var reader = new S57Reader();
            //reader.ReadCatalogue(VolumeRootPath);
            //foreach (var map in reader.BaseFiles)
            //{
            //    string name = map.Value.fileName;
            //    string _MapName = name.Substring(name.IndexOf("\\") + 1);
            //    reader.Read(VolumeRootPath, MapName, false);
            //    Console.WriteLine(_MapName);
            //    var bla = reader.GetFeaturesOfClass(S57Obj.M_COVR);
            //};
            //timer.Stop();
            //Console.WriteLine(((double)(timer.Elapsed.TotalMilliseconds)).ToString("0.00 ms"));


            ZipArchive archive = ZipFile.OpenRead(zipPath);
            timer.Reset();
            timer.Start();
            reader = new S57Reader();
            reader.ReadCatalogue(archive);
            foreach (var map in reader.BaseFiles)
            {
                string name = map.Value.fileName;
                string _MapName = name.Substring(name.IndexOf("\\") + 1);
                reader.Read(archive, _MapName, false);
                Console.WriteLine(_MapName);
                var bla = reader.GetFeaturesOfClass(S57Obj.DEPARE);
            };
            //reader.Read(archive, MapName, true);
            archive.Dispose();
            timer.Stop();
            Console.WriteLine(((double)(timer.Elapsed.TotalMilliseconds)).ToString("0.00 ms"));

            //ListRelationships(reader, S57Obj.BOYSAW);
            //ListFeatures(reader);
            //QalityOfPosition(reader);

            //FlatPolygon TempSet;
            //var features = reader.GetFeaturesOfClass(S57Obj.DEPARE);
            //for (int i = 0; i < features.Count; i++)
            //{
            //    if (features[i].Primitive == GeometricPrimitive.Area)
            //    {
            //        TempSet = features[i].GetGeometry(true) as FlatPolygon;
            //    }
            //}        
            //var features = reader.GetFeaturesOfClass(S57Obj.LNDMRK);
            //for (int i = 0; i < features.Count; i++) 
            //{
            //    string tempString;
            //    features[i].Attributes.TryGetValue(S57Att.SCAMIN, out tempString);
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
                    var features = reader.GetFeaturesOfClass(obj.Key);
                    int count=0;
                    for (int i = 0; i < features.Count; i++)
                    {
                        if(features[i].Attributes!=null && features[i].Attributes.ContainsKey(S57Att.SCAMIN))
                            count++;
                    }
                    string tempString=null;
                    if (features[0].Attributes != null)
                        features[0].Attributes.TryGetValue(S57Att.SCAMIN, out tempString);
                    Console.WriteLine(test + ": " + count + "/" +a + " "+ tempString);
                    //Console.WriteLine(test + ": " + "{0:G}", a.ToString());
                }
            }
        }

    }
}
