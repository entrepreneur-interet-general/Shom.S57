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

        public void ReadArchiveCatalogue(ZipArchive archive, string MapName)
        {
            string basename = MapName.Remove(MapName.Length - 4);
            Stream S57map = null;
            ZipArchiveEntry catalogueentry = null;
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if (entry.Name.Contains(basename))
                {
                    if (entry.Name.Equals(MapName))
                    {
                        catalogueentry = entry;
                    }                    
                }
            }
            S57map = catalogueentry.Open();
            using (var reader = new Iso8211Reader(S57map))
            {
                catalogueFile = new CatalogueFile(reader);
                BuildCatalogue();
            }
            S57map.Dispose();            
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
            S57map = baseentry.Open();
            using (var reader = new Iso8211Reader(S57map))
            {
                baseFile = new BaseFile(reader);
            }
            S57map.Dispose();
            //timer.Stop();
            //Console.WriteLine(((double)(timer.Elapsed.TotalMilliseconds)).ToString("0.00 ms"));
            //timer.Start();
            if (ApplyUpdates)
            {
                foreach (var entry in updatefiles)
                {
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
            baseFile.BindFeatureObjectPointers();
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
            baseFile.BindFeatureObjectPointers();
            //timer.Stop();
            //Console.WriteLine(((double)(timer.Elapsed.TotalMilliseconds)).ToString("0.00 ms"));
        }    

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

        public List<Feature> GetFeaturesOfClass(S57Obj ObjectCode)
        {
            List<Feature> tempList = new List<Feature>();
            //foreach (var feat in baseFile.eFeatureRecords)
            foreach (var feat in baseFile.eFeatureObjects)
            {
                if (feat.Value.ObjectCode == ObjectCode)
                    tempList.Add(feat.Value);
            }
            return tempList;
        }

        public List<Feature> GetFeaturesOfClasses(S57Obj[] ObjectCodes)
        {
            List<Feature> tempList = new List<Feature>();
            //foreach (var feat in baseFile.eFeatureRecords)
            foreach (var feat in baseFile.eFeatureObjects)
            {
                foreach (var ObjectCode in ObjectCodes)
                {
                    if (ObjectCode == feat.Value.ObjectCode)
                        tempList.Add(feat.Value);
                }
            }
            return tempList;
        }
    }
}
