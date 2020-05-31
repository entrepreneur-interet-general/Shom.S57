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

        public Cell cellInfo;
        public BaseFile baseFile;
        public UpdateFile updateFile;
        public CatalogueFile catalogueFile;
        public ProductInfo productInfo;

        public Dictionary<uint, Catalogue> ExchangeSetFiles = new Dictionary<uint, Catalogue>();
        public Dictionary<uint, Catalogue> BaseFiles = new Dictionary<uint, Catalogue>();

        byte[] fileByteArray;

        //public void ReadCatalogue(System.IO.Stream stream)
        //{
        //    using (var reader = new Iso8211Reader(stream))
        //    {
        //        catalogueFile = new CatalogueFile(reader);
        //        BuildCatalogue();
        //    }
        //}

        public void ReadCatalogue(ZipArchive archive)
        {
            Stream S57map = null;
            ZipArchiveEntry catalogueentry = null;
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if (entry.Name.Equals("CATALOG.031"))
                {
                    catalogueentry = entry;
                }
            }
            S57map = catalogueentry.Open();
            var count = catalogueentry.Length;
            byte[] fileByteArray = new byte[count]; //consider re-using same byte array for next file to minimize new allocations
            MemoryStream memoryStream = new MemoryStream(fileByteArray);
            S57map.CopyTo(memoryStream);
            memoryStream.Dispose();
            using (var reader = new Iso8211Reader(fileByteArray))
            {
                catalogueFile = new CatalogueFile(reader);
                BuildCatalogue();
                foreach (var bla in reader.tagcollector)
                    Console.WriteLine(bla);
            }
            S57map.Dispose();
        }



        //public void ReadCatalogue(string RootDirectory)
        //{
        //    if (!RootDirectory.EndsWith("ENC_ROOT"))
        //    {
        //        //Console.WriteLine("Selected folder is not ENC_ROOT folder of a Volume");
        //        return;
        //    }
        //    Stream S57map = null;
        //    string[] filePaths = Directory.GetFiles(@RootDirectory, "CATALOG.031", SearchOption.AllDirectories);
        //    string catalogueentry = null;
        //    if (filePaths.Length > 1)
        //    {
        //        //Console.WriteLine("More than one Catalogue file found. Please selected MapSet root folder");
        //        return;
        //    }
        //    catalogueentry = filePaths[0];
        //    S57map = new FileStream(@catalogueentry, FileMode.Open, FileAccess.Read, FileShare.Read, 65536);
        //    using (var reader = new Iso8211Reader(S57map))
        //    {
        //        catalogueFile = new CatalogueFile(reader);
        //        BuildCatalogue();
        //    }
        //    S57map.Dispose();       
        //}

        //public void ReadProductInfo(System.IO.Stream stream)
        //{
        //    using (var reader = new Iso8211Reader(stream))
        //    {
        //        productInfo = new ProductInfo(reader);
        //    }
        //}

        public void ReadProductInfo(ZipArchive archive, string MapName)
        {
            Stream S57map = null;
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if (entry.Name.Equals(MapName))
                {
                    S57map = entry.Open();
                    int count = (int)entry.Length;
                    if (fileByteArray == null)
                        fileByteArray = new byte[count];
                    else
                    {
                        Array.Clear(fileByteArray, 0, fileByteArray.Length);
                        Array.Resize(ref fileByteArray, count);
                    }
                    MemoryStream memoryStream = new MemoryStream(fileByteArray);
                    S57map.CopyTo(memoryStream);
                    memoryStream.Dispose();

                    using (var reader = new Iso8211Reader(fileByteArray))
                    {
                        productInfo = new ProductInfo(reader);
                    }
                }
            }
        }
        public void Read(ZipArchive archive, string MapName, bool ApplyUpdates)
        {
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
            int count = (int)baseentry.Length;
            if (fileByteArray == null)
                fileByteArray = new byte[count];
            else
            {
                Array.Clear(fileByteArray,0, fileByteArray.Length);
                Array.Resize(ref fileByteArray, count);
            }
            MemoryStream memoryStream = new MemoryStream(fileByteArray);
            S57map.CopyTo(memoryStream);
            memoryStream.Dispose();

            using (var reader = new Iso8211Reader(fileByteArray))
            {
                baseFile = new BaseFile(reader);
                foreach (var bla in reader.tagcollector)
                    Console.WriteLine(bla);
            }
            S57map.Dispose();

            if (ApplyUpdates)
            {
                foreach (var entry in updatefiles)
                {
                    S57update = entry.Value.Open();
                    count = (int)entry.Value.Length;
                    Array.Clear(fileByteArray, 0, fileByteArray.Length);
                    Array.Resize(ref fileByteArray, count);
                    memoryStream = new MemoryStream(fileByteArray);
                    S57update.CopyTo(memoryStream);
                    memoryStream.Dispose();
                    using (var updatereader = new Iso8211Reader(fileByteArray))
                    {
                        UpdateFile updateFile = new UpdateFile(updatereader);
                        baseFile.ApplyUpdateFile(updateFile);
                    }
                    S57update.Dispose();
                }
            }
            cellInfo = new Cell(baseFile);
            baseFile.BindVectorPointersOfVectors();
            baseFile.BindVectorPointersOfFeatures();
            baseFile.BuildVectorGeometry();
            baseFile.BindFeatureObjectPointers();

        }
        //public void Read(string RootDirectory, string MapName, bool ApplyUpdates)
        //{
        //    if (!RootDirectory.EndsWith("ENC_ROOT"))
        //    {
        //        //Console.WriteLine("Selected folder is not ENC_ROOT folder of a Volume");
        //        return;
        //    }
        //    string basename = MapName.Remove(MapName.Length - 4);
        //    Stream S57map = null;
        //    string baseentry = null;
        //    SortedList<uint, string> updatefiles = new SortedList<uint, string>();
        //    Stream S57update;
        //    string[] filePaths = Directory.GetFiles(@RootDirectory, "*.*", SearchOption.AllDirectories);
            

        //    foreach (string entry in filePaths)
        //    {
        //        if (entry.Contains(basename))
        //        {
        //            if (entry.EndsWith(MapName))
        //            {
        //                baseentry = entry;
        //            }
        //            else
        //            {
        //                int val;
        //                string end = entry.Substring(entry.Length - 3);
        //                if (char.IsDigit(end[0]) && char.IsDigit(end[1]) && char.IsDigit(end[2]))
        //                {
        //                    int.TryParse(end, out val);
        //                    updatefiles.Add(Convert.ToUInt32(end.ToString()), entry);
        //                }
        //            }
        //        }
        //    }
        //    S57map = new FileStream(@baseentry, FileMode.Open, FileAccess.Read, FileShare.Read, 65536);
        //    using (var reader = new Iso8211Reader(S57map))
        //    {
        //        baseFile = new BaseFile(reader);
        //    }

        //    S57map.Dispose();
        //    if (ApplyUpdates)
        //    {
        //        foreach (var entry in updatefiles)
        //        {
        //            S57update = new FileStream(@entry.Value, FileMode.Open);
        //            using (var updatereader = new Iso8211Reader(S57update))
        //            {
        //                UpdateFile updateFile = new UpdateFile(updatereader);
        //                baseFile.ApplyUpdateFile(updateFile);
        //            }
        //            S57update.Dispose();
        //        }
        //    }
        //    cellInfo = new Cell(baseFile);
        //    baseFile.BindVectorPointersOfVectors();
        //    baseFile.BindVectorPointersOfFeatures();
        //    baseFile.BuildVectorGeometry();
        //    baseFile.BindFeatureObjectPointers();
        //}


        //public void Read(System.IO.Stream stream)
        //{
        //    //Stopwatch timer = new Stopwatch();
        //    //timer.Start();
        //    using (var reader = new Iso8211Reader(stream))
        //    {
        //        baseFile = new BaseFile(reader);
        //    }
        //    //timer.Stop();
        //    //Console.WriteLine(((double)(timer.Elapsed.TotalMilliseconds)).ToString("0.00 ms"));
        //    cellInfo = new Cell(baseFile);
        //    //timer.Start();
        //    baseFile.BindVectorPointersOfVectors();
        //    baseFile.BindVectorPointersOfFeatures();
        //    baseFile.BuildVectorGeometry();
        //    baseFile.BindFeatureObjectPointers();
        //    //timer.Stop();
        //    //Console.WriteLine(((double)(timer.Elapsed.TotalMilliseconds)).ToString("0.00 ms"));
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
