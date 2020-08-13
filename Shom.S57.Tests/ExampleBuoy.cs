using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using S57;
using Shom.ISO8211;

// This example show how we gan get information about 
// buoy

namespace Shom.S57.Tests
{
    class Program
    {
        //static void Main(string[] args)
        static void Main()
        {
            var reader = new S57Reader();
            string ZipName = "FR571370.zip";
            string rootPath = "/home/gabin/Projet_gabin/Shom.S57/Shom.S57.Tests/";
            string zipPath = Path.Combine(rootPath, ZipName);
            ZipArchive archive = ZipFile.OpenRead(zipPath);

            string MapName = "FR571370.000";
            
            reader.ReadFromArchive(archive, MapName, true);
            archive.Dispose();

            ListRelationships(reader, S57Obj.BOYSAW);

            ListFeatures(reader);

            QalityOfPosition(reader);

            FlatPolygon TempSet;
            var features = reader.GetFeaturesOfClass(S57Obj.BOYCAR);
            for (int i = 0; i < features.Count; i++)
            {
                if (features[i].Primitive == GeometricPrimitive.Area)
                {
                    TempSet = features[i].GetGeometry(true) as FlatPolygon;
                }
            }

            var features1 = reader.GetFeaturesOfClass(S57Obj.BOYCAR);
            string description1 = null;
            Console.WriteLine(" BUOY Car extraction");
            for(int i =0; i <features1.Count; i++){
                Console.WriteLine( features1[i].Attributes.TryGetValue(S57Att.COLOUR, out description1 ) ) ; //http://www.s-57.com/
                Console.WriteLine( description1 );

                var test3 = features1[0].GetGeometry(false) as Point;
                Console.WriteLine(test3.Y);
                Console.WriteLine(test3.X);

            }

            // information liées aux boués latérale.
            var features2 = reader.GetFeaturesOfClass(S57Obj.BOYLAT);
            Console.WriteLine(" BUOY Lat extraction");
            for(int i =0; i <features2.Count; i++){
                Console.WriteLine( features2[i].Attributes.TryGetValue(S57Att.COLOUR, out description1) ) ; //http://www.s-57.com/
                Console.WriteLine( description1 );
            }

            var test2 = features1[0].GetGeometry(false) as Line;

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
