using System;
using System.Collections.Generic;
using Shom.ISO8211;
using S57.File;

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
        public CatalogueFile catalogueFile;
        public ProductInfo productInfo;

        public Dictionary<uint, Catalogue> ExchangeSetFiles = new Dictionary<uint, Catalogue>();
        public Dictionary<uint, Catalogue> BaseFiles = new Dictionary<uint, Catalogue>();
        public Dictionary<LongName, Feature> Features = new Dictionary<LongName, Feature>();
        public Dictionary<VectorName, Vector> Vectors = new Dictionary<VectorName, Vector>();
        public Dictionary<LongName, Feature> newFeatures = new Dictionary<LongName, Feature>();

        public List<LongName> existingLNAMs = new List<LongName>();
        public List<VectorName> existingVectors = new List<VectorName>();
        public List<LongName> notFoundGeometries = new List<LongName>();

        private Feature FindFeatureByLnam(LongName lnam)
        {
            if (Features.ContainsKey(lnam))
            {
                return Features[lnam];
            }
            else
            {
                return null;
            }
        }

        /*
        private void ReadMapFields(DataRecord record)
        {
            var dspm = record.Fields.GetFieldByTag("DSPM");
            if (dspm != null)
            {
                var coun = dspm.GetUInt32("COUN");
                var nCOMF = dspm.GetUInt32("COMF");
                var nSOMF = dspm.GetUInt32("SOMF");
                var nCSCL = dspm.GetUInt32("CSCL");
            }

            var dsid = record.Fields.GetFieldByTag("DSID");
            if (dsid != null)
            {
                var pszDSNM = dsid.GetString("DSNM");
                var dssi = record.Fields.GetFieldByTag("DSSI");
                if (dssi != null)
                {
                    var Nall = dssi.GetUInt32("NALL");
                    var Aall = dssi.GetUInt32("AALL");
                    var dstr = dssi.GetUInt32("DSTR");
                }
            }


            // "DSPR" dataset projection. subfields FEAS FNOR
            // "DSRC" dataset registration control CURP
            // "CATD" catalogue directory CRCS checksum
        }
        */

        private string VectorKey(Vector vector)
        {
            if (vector == null)
            {
                return null;
            }
            return string.Format("{0}-{1}", mapIndex, vector.vectorName.ToString());
        }

        private string VectorKey(VectorName vectorName)
        {
            return string.Format("{0}-{1}", mapIndex, vectorName.ToString());
        }

        private string VectorKey(string vectorName)
        {
            if (vectorName == null)
            {
                return null;
            }
            return string.Format("{0}-{1}", mapIndex, vectorName);
        }

        public void NewMap()
        {
            mapIndex++;
        }

        public void ReadCatalogue(System.IO.Stream stream)
        {
            newFeatures.Clear();
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

        public void Read(System.IO.Stream stream)
        {
            newFeatures.Clear();
            using (var reader = new Iso8211Reader(stream))
            {
                baseFile = new BaseFile(reader);
                cell = new Cell(baseFile);
                BuildVectors();
                BuildFeatures();
            }
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
        private void BuildFeatures()
        {
            foreach (var fr in baseFile.FeatureRecords) // cell.fr)
            {
                Feature feature = new Feature(fr, baseFile, cell);
                if (feature.RecordUpdateInstruction == RecordUpdate.Delete)
                {
                    DeleteFeatureByFRID(feature);
                }
                else if (!Features.ContainsKey(feature.lnam))
                {
                    AddFeature(feature);
                }
                else
                {
                    // Existing LNAM
                    existingLNAMs.Add(feature.lnam);
                    var existingFeature = Features[feature.lnam];
                    if (feature.RecordUpdateInstruction == RecordUpdate.Modify)
                    {
                        ApplyFeatureModification(feature, existingFeature);
                    }
                    else if (feature.Attributes != null && existingFeature.Attributes != null)
                    {
                        UpdateFeaturePtrs(feature);
                        UpdateVectorPtrs(feature);
                        MergeAttributes(existingFeature, feature);
                        ChangeGeometry(feature, existingFeature);
                    }
                }
            }
        }

        private void AddFeature(Feature feature)
        {
            Features.Add(feature.lnam, feature);
            newFeatures.Add(feature.lnam, feature);
            UpdateFeaturePtrs(feature);
            UpdateVectorPtrs(feature);
        }

        private void DeleteFeatureByFRID(Feature feature)
        {
            LongName lnam = new LongName();
            foreach (Feature f in Features.Values)
            {
                if (f.cell.DataSetName == feature.cell.DataSetName &&
                    f.RecordIdentificationNumber == feature.RecordIdentificationNumber)
                {
                    lnam = f.lnam;
                }
            }
            if (lnam.ProducingAgency != 0 && lnam.FeatureIdentificationNumber !=0 && lnam.FeatureIdentificationSubdivision!=0)
            {
                Features.Remove(lnam);
            }
        }

        private static void ApplyFeatureModification(Feature feature, Feature existingFeature)
        {
            existingFeature.RecordVersion = feature.RecordVersion;
            // Modify Attributes
            if (feature.Attributes != null)
            {
            }
            if (feature.FeatureObjectPointerUpdateInstruction != RecordUpdate.__None__)
            {
                UpdateRelationsToFeaturePtrs(feature, existingFeature);
            }
            if (feature.FeatureToSpatialRecordPointerUpdateInstruction != RecordUpdate.__None__)
            {
                UpdateRelationToVectorPtrs(feature, existingFeature);
            }
        }

        private void ChangeGeometry(Feature feature, Feature existingFeature)
        {
            // For points, if newer info then update the position
            if (feature.Primitive == GeometricPrimitive.Point)
            {
                if (feature.cell.IssueDate > existingFeature.cell.IssueDate)
                {
                    existingFeature.VectorPtrs = feature.VectorPtrs;
                }
            }
            else
            {
                if (existingFeature.baseFile.compilationScaleOfData == feature.baseFile.compilationScaleOfData)
                {
                    existingFeature.VectorPtrs.AddRange(feature.VectorPtrs);
                }
            }
        }

        private void BuildVectors()
        {
            foreach (var vr in baseFile.VectorRecords) // cell.vr)
            {
                Vector vector = new Vector(this, vr, baseFile);
                if (!Vectors.ContainsKey(vector.vectorName))
                {
                    Vectors.Add(vector.vectorName, vector);
                    // VECTORS ASSOCIATION
                    if (vector.VectorRecordPointers != null)
                    {
                        BindVectorToVectorRecordPointsOf(vector);
                    }
                    // LAT/LONG UPDATE
                    if (vector.Geometry is Point) //assemble all points for a given vector record, checks if geometry return type is point or line
                    {
                        ((Point)vector.Geometry).X /= baseFile.coordinateMultiplicationFactor;
                        ((Point)vector.Geometry).Y /= baseFile.coordinateMultiplicationFactor;
                    }
                }
                else
                {
                    // Vector already exists
                    existingVectors.Add(vector.vectorName);
                }
            }
        }




        public void BindVectorToVectorRecordPointsOf(Vector vector)
        {
            foreach (var v in vector.VectorRecordPointers)
            {
                if (Vectors.ContainsKey(v.Name))
                {
                    v.Vector = Vectors[v.Name];
                }
            }
        }

        private static void UpdateRelationToVectorPtrs(Feature feature, Feature existingFeature)
        {
            switch (feature.FeatureToSpatialRecordPointerUpdateInstruction)
            {
                case RecordUpdate.Insert:
                    {
                        if (existingFeature.VectorPtrs != null)
                        {
                            for (int i = (int)feature.NumberOfFeatureToSpatialRecordPointers - 1; i >= 0; i--)
                            {
                                existingFeature.VectorPtrs.Insert((int)feature.FeatureToSpatialRecordPointerIndex - 1, feature.VectorPtrs[i]);
                            }
                        }
                        else
                        {
                            existingFeature.VectorPtrs = feature.VectorPtrs;
                        }
                        break;
                    }
                case RecordUpdate.Delete:
                    {
                        existingFeature.VectorPtrs.RemoveRange((int)feature.FeatureToSpatialRecordPointerIndex - 1, (int)feature.NumberOfFeatureToSpatialRecordPointers);
                        break;
                    }
                case RecordUpdate.Modify:
                    {
                        existingFeature.VectorPtrs.RemoveRange((int)feature.FeatureToSpatialRecordPointerIndex - 1, (int)feature.NumberOfFeatureToSpatialRecordPointers);
                        for (int i = (int)feature.NumberOfFeatureToSpatialRecordPointers - 1; i >= 0; i--)
                        {
                            existingFeature.VectorPtrs.Insert((int)feature.FeatureToSpatialRecordPointerIndex - 1, feature.VectorPtrs[i]);
                        }
                        break;
                    }
            }
        }

        private static void UpdateRelationsToFeaturePtrs(Feature feature, Feature existingFeature)
        {
            switch (feature.FeatureObjectPointerUpdateInstruction)
            {
                case RecordUpdate.Insert:
                    {
                        for (int i = (int)feature.NumberOfFeatureObjectPointers - 1; i >= 0; i--)
                        {
                            existingFeature.FeaturePtrs.Insert((int)feature.FeatureObjectPointerIndex - 1, feature.FeaturePtrs[i]);
                        }
                        break;
                    }
                case RecordUpdate.Delete:
                    {
                        existingFeature.FeaturePtrs.RemoveRange((int)feature.FeatureObjectPointerIndex - 1, (int)feature.NumberOfFeatureObjectPointers);
                        break;
                    }
                case RecordUpdate.Modify:
                    {
                        existingFeature.FeaturePtrs.RemoveRange((int)feature.FeatureObjectPointerIndex - 1, (int)feature.NumberOfFeatureObjectPointers);
                        for (int i = (int)feature.NumberOfFeatureObjectPointers - 1; i >= 0; i--)
                        {
                            existingFeature.FeaturePtrs.Insert((int)feature.FeatureObjectPointerIndex - 1, feature.FeaturePtrs[i]);
                        }
                        break;
                    }
            }
        }

        private void MergeAttributes(Feature existingFeature, Feature feature)
        {
            foreach (uint key in feature.Attributes.Keys)
            {
                if (existingFeature.Attributes.ContainsKey(key))
                {
                    if (feature.Attributes[key] != existingFeature.Attributes[key])
                    {
                        // SCAMIN 
                        if (key == 133)
                        {
                            existingFeature.Attributes[key] = Math.Min(double.Parse(existingFeature.Attributes[key]), double.Parse(feature.Attributes[key])).ToString();
                        }
                        // Otherwise, we consider that latest cell has better info
                        else if (feature.cell.IssueDate > existingFeature.cell.IssueDate)
                        {
                            existingFeature.Attributes[key] = feature.Attributes[key];
                        }
                    }
                }
                else
                {
                    existingFeature.Attributes.Add(key, feature.Attributes[key]);
                }
            }
        }

        private void UpdateFeaturePtrs(Feature feature)
        {
            if (feature.FeaturePtrs != null)
            {
                foreach (var featurePtr in feature.FeaturePtrs)
                {
                    if (featurePtr.Feature == null)
                    {
                        if (Features.ContainsKey(featurePtr.LNAM))
                        {
                            featurePtr.Feature = Features[featurePtr.LNAM];
                        }
                    }
                }
            }
        }

        private void UpdateVectorPtrs(Feature feature)
        {
            if (feature.VectorPtrs != null)
            {
                foreach (var vectorPtr in feature.VectorPtrs)
                {
                    if (vectorPtr.Vector == null)
                    {
                        if (Vectors.ContainsKey(vectorPtr.Name))
                        {
                            var vector = Vectors[vectorPtr.Name];
                            vectorPtr.Vector = vector;
                        }
                        else
                        {
                            notFoundGeometries.Add(feature.lnam);
                        }
                    }
                }
            }
        }

        public int Filter(string filter)
        {
            if (string.IsNullOrEmpty(filter) )
            {
                return 0;
            }
            string[] codes = filter.Split(new char[] { ' ' });
            for (int i = 0; i < codes.Length; i++)
            {
                codes[i] = codes[i].Trim();
                var isNumeric = int.TryParse(codes[i], out int n);
                if (!isNumeric)
                {
                    codes[i] = S57Objects.Get(codes[i]).Code.ToString(); //performance-killer: convert uint to string, to later compare strings. Better: compare uints
                }
            }
            List<string> lCodes = new List<string>();
            lCodes.AddRange(codes);
            List<Feature> features = new List<Feature>();
            features.AddRange(Features.Values);
            Features = new Dictionary<LongName, Feature>();
            foreach (var feature in features)
            {
                foreach (var code in lCodes)
                {
                    if (code == feature.Code.ToString()) { }
                    Features.Add(feature.lnam, feature);
                }
            }

            return features.Count;
        }

        public struct PurgeResults
        {
            public int NbFeaturesBefore;
            public int NbVectorsBefore;
            public int NbFeatures;
            public int NbVectors;
            public int NbRemovedFeatures;
            public int NbRemovedVectors;
        }

        public PurgeResults PurgeBasedOnS57KnownObjects()
        {
            PurgeResults result = new PurgeResults();
            result.NbFeaturesBefore = Features.Count;
            result.NbVectorsBefore = Vectors.Count;
            List<VectorName> usedVectors = new List<VectorName>();
            List<LongName> unusedLnams = new List<LongName>();
            foreach (Feature feature in newFeatures.Values)
            {
                var featureInfo = S57Objects.Get(feature.Code);
                if (featureInfo != null && feature.Code < 300 )
                {
                    if (feature.VectorPtrs != null && feature.VectorPtrs.Count > 0)
                    {
                        foreach (var vectorPtr in feature.VectorPtrs)
                        {
                            var vector = vectorPtr.Vector;
                            if (vector == null)
                            {
                                UpdateVectorPtrs(feature);
                            }
                            if (!usedVectors.Contains(vectorPtr.Vector.vectorName))
                            {
                                usedVectors.Add(vectorPtr.Vector.vectorName);
                            }
                        }
                    }
                }
                else
                {
                    unusedLnams.Add(feature.lnam);
                }
            }
            result.NbRemovedFeatures = unusedLnams.Count;
            result.NbRemovedVectors = 0;
            foreach (var lnam in unusedLnams )
            {
                Features.Remove(lnam);
            }
            int index = Vectors.Count - 1;
            List<Vector> vectors = new List<Vector>();
            vectors.AddRange(Vectors.Values);
            while (index >= 0)
            {
                if (!usedVectors.Contains(vectors[index].vectorName))
                {
                    if (Vectors.Remove(vectors[index].vectorName))
                    {
                        result.NbRemovedVectors++;
                    }
                }
                index--;
            }
            baseFile.FeatureRecords.Clear();
            baseFile.VectorRecords.Clear();
            GC.Collect();
            result.NbFeatures = Features.Count;
            result.NbVectors = Vectors.Count;
            return result;
        }

        public List<Feature> GetFeaturesOfClass(string acronym)
        {
            return GetFeaturesOfClass(S57Objects.Get(acronym).Code);
        }

        public List<Feature> GetFeaturesOfClass(uint code)
        {
            List<Feature> tempList = new List<Feature>();
            foreach (var feat in Features)
            {
                if (feat.Value.Code == code)
                    tempList.Add(feat.Value);
            }
            return tempList;
        }

        public List<Feature> GetFeaturesOfClasses( string[] acronyms)
        {
            List<uint> codes = new List<uint>();
            foreach (var acronym in acronyms)
            {
                uint code = S57Objects.Get(acronym).Code;
                codes.Add(code);
            }
            return GetFeaturesOfClasses( codes.ToArray() );
        }

        public List<Feature> GetFeaturesOfClasses( uint[] codes)
        {
            List<Feature> tempList = new List<Feature>();
            foreach (var feat in Features)
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
