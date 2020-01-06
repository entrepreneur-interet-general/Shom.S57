using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using Shom.ISO8211;
using S57.File;
using System.Diagnostics;

namespace S57
{
    public class S57Reader
    {
        public S57Reader()
        { }

        public static int mapIndex = 0;

        public List<Cell> cells = new List<Cell>();
        public Cell cell;
        public BaseFile baseFile;
        public UpdateFile updateFile;
        public CatalogueFile catalogueFile;
        public ProductInfo productInfo;

        public Dictionary<uint, Catalogue> ExchangeSetFiles = new Dictionary<uint, Catalogue>();
        public Dictionary<uint, Catalogue> BaseFiles = new Dictionary<uint, Catalogue>();

        //private Feature FindFeatureByLnam(LongName lnam)
        //{
        //    if (Features.ContainsKey(lnam))
        //    {
        //        return Features[lnam];
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        public void ReadCatalogue(System.IO.Stream stream)
        {
            using (var reader = new Iso8211Reader(stream))
            {
                catalogueFile = new CatalogueFile(reader);
                BuildCatalogue();
            }
        }

        public void ReadProductInfo(System.IO.Stream stream)
        {
            using (var reader = new Iso8211Reader(stream))
            {
                productInfo = new ProductInfo(reader);
            }
        }

        public void ReadFromArchive(ZipArchive archive, string MapName, bool ApplyUpdates)
        {
            //Stopwatch timer = new Stopwatch();
            //timer.Start();
            string basename = MapName.Remove(MapName.Length - 4);
            Stream S57map=null;
            ZipArchiveEntry baseentry=null;
            SortedList<uint, ZipArchiveEntry> updatefiles = new SortedList<uint, ZipArchiveEntry>();
            Stream S57update;
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if (entry.Name.Contains(basename))
                {
                    if (entry.Name.Equals(MapName))
                    {
                        baseentry = entry;
                    }
                    else
                    {
                        int val;
                        string end = entry.Name.Substring(entry.Name.Length - 3);
                        if (char.IsDigit(end[0]) && char.IsDigit(end[1]) && char.IsDigit(end[2]))
                        {
                            int.TryParse(end, out val); 
                            updatefiles.Add(Convert.ToUInt32(end.ToString()),entry);
                        }
                    }
                }
            }
            //Stream S57map = archive.GetEntry(MapName).Open();
            S57map = baseentry.Open();
            using (var reader = new Iso8211Reader(S57map))
            {
                baseFile = new BaseFile(reader);
            }
            S57map.Dispose();
            //timer.Stop();
            //Console.WriteLine(((double)(timer.Elapsed.TotalMilliseconds)).ToString("0.00 ms"));
            //timer.Start();
            //for (uint i =1; i<updatefiles.Count+1; i++)
            if (ApplyUpdates)
            {
                foreach (var entry in updatefiles)
                {
                    //S57update = updatefiles[i].Open();
                    S57update = entry.Value.Open();
                    using (var updatereader = new Iso8211Reader(S57update))
                    {
                        UpdateFile updateFile = new UpdateFile(updatereader);
                        baseFile.ApplyUpdateFile(updateFile);
                    }
                    S57update.Dispose();
                }
            }
            //timer.Stop();
            //Console.WriteLine(((double)(timer.Elapsed.TotalMilliseconds)).ToString("0.00 ms"));

            //cell = new Cell(baseFile);
            //timer.Start();
            baseFile.BindVectorPointersOfVectors();
            baseFile.BindVectorPointersOfFeatures();
            baseFile.BuildVectorGeometry();
        }

        public void Read(System.IO.Stream stream)
        {
            //Stopwatch timer = new Stopwatch();
            //timer.Start();
            using (var reader = new Iso8211Reader(stream))
            {
                baseFile = new BaseFile(reader);
            }
            //timer.Stop();
            //Console.WriteLine(((double)(timer.Elapsed.TotalMilliseconds)).ToString("0.00 ms"));
            cell = new Cell(baseFile);
            //timer.Start();
            baseFile.BindVectorPointersOfVectors();
            baseFile.BindVectorPointersOfFeatures();
            baseFile.BuildVectorGeometry();
            //baseFile.BindFeatureObjectPointers();
            //timer.Stop();
            //Console.WriteLine(((double)(timer.Elapsed.TotalMilliseconds)).ToString("0.00 ms"));
        }

        //private void UpdateFeaturePtrs(oldFeature feature)
        //{
        //    if (feature.FeaturePtrs != null)
        //    {
        //        foreach (var featurePtr in feature.FeaturePtrs)
        //        {
        //            if (featurePtr.Feature == null)
        //            {
        //                if (oldFeatures.ContainsKey(featurePtr.LNAM))
        //                {
        //                    featurePtr.Feature = oldFeatures[featurePtr.LNAM];
        //                }
        //            }
        //        }
        //    }
        //}        

        private void BuildCatalogue()
        {
            foreach (var cr in catalogueFile.CatalogueRecords)
            {
                Catalogue catalog = new Catalogue(this, cr, catalogueFile);
                uint key = catalog.RecordIdentificationNumber;
                if (!ExchangeSetFiles.ContainsKey(key) || !BaseFiles.ContainsKey(key))
                {
                    if (catalog.fileName.EndsWith(".000"))
                    {
                        BaseFiles.Add(key, catalog);
                    }
                    else
                        ExchangeSetFiles.Add(key, catalog);
                }
            }
        }

        public List<Feature> GetFeaturesOfClass(string acronym)
        {
            return GetFeaturesOfClass(S57Objects.Get(acronym).Code);
        }
        public List<Feature> GetFeaturesOfClass(uint code)
        {
            List<Feature> tempList = new List<Feature>();
            foreach (var feat in baseFile.eFeatureRecords)
            {
                if (feat.Value.Code == code)
                    tempList.Add(feat.Value);
            }
            return tempList;
        }

        public List<Feature> GetFeaturesOfClasses(string[] acronyms)
        {
            List<uint> codes = new List<uint>();
            foreach (var acronym in acronyms)
            {
                uint code = S57Objects.Get(acronym).Code;
                codes.Add(code);
            }
            return GetFeaturesOfClasses(codes.ToArray());
        }

        public List<Feature> GetFeaturesOfClasses(uint[] codes)
        {
            List<Feature> tempList = new List<Feature>();
            foreach (var feat in baseFile.eFeatureRecords)
            {
                foreach (var code in codes)
                {
                    if (code == feat.Value.Code)
                        tempList.Add(feat.Value);
                }
            }
            return tempList;
        }
    }
}
