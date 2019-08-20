using System;
using System.Collections.Generic;
using Shom.ISO8211;

namespace S57.File
{
    public enum LexicalLevel : uint
    {
        ASCIIText = 0,  // ASCII
        ISO8859 = 1,    // Latin-1?
        ISO10646 = 2    // Unicode
    }

    public class BaseFile
    {
        public DataRecord DataSetGeneralInformationRecord = null;
        public DataRecord DataSetGeographicReferenceRecord = null;

        public List<DataRecord> FeatureRecords;
        public List<DataRecord> VectorRecords;

        // DSSI
        public VectorDataStructure vectorDataStructure;
        public LexicalLevel ATTFLexicalLevel;
        public LexicalLevel NATFLexicalLevel;
        public uint numberOfMetaRecords;
        public uint numberOfCartographicRecords;
        public uint numberOfGeoRecords;
        public uint numberOfCollectionRecords;
        public uint numberOfIsolatedNodeRecords;
        public uint numberOfConnectedNodeRecords;
        public uint numberOfEdgeRecords;
        public uint numberOfFaceRecords;

        // DSPM
        public CoordinateUnits coordinateUnits;
        public uint coordinateMultiplicationFactor;
        public uint soundingMultiplicationFactor;
        public uint compilationScaleOfData;
        public uint unitsOfPositionalAccuracy;
        public uint unitsOfDepthMeasurement;
        public uint unitsOfHeightMeasurement;
        public uint horizontalGeodeticDatum;
        public uint verticalDatum;
        public uint soundingDatum;

        
        public BaseFile(Iso8211Reader reader)
        {
            //Current this works because we know the two records are special
            DataSetGeneralInformationRecord = reader.ReadDataRecord();
            var dssi = DataSetGeneralInformationRecord.Fields.GetFieldByTag("DSSI");
            if (dssi != null)
            {
                vectorDataStructure = (VectorDataStructure)dssi.GetUInt32("DSTR");
                ATTFLexicalLevel = (LexicalLevel)dssi.GetUInt32("AALL");
                NATFLexicalLevel = (LexicalLevel)dssi.GetUInt32("NALL");
                numberOfMetaRecords = dssi.GetUInt32("NOMR");
                numberOfCartographicRecords = dssi.GetUInt32("NOCR");
                numberOfGeoRecords = dssi.GetUInt32("NOGR");
                numberOfCollectionRecords = dssi.GetUInt32("NOLR");
                numberOfIsolatedNodeRecords = dssi.GetUInt32("NOIN");
                numberOfConnectedNodeRecords = dssi.GetUInt32("NOCN");
                numberOfEdgeRecords = dssi.GetUInt32("NOED");
                numberOfFaceRecords = dssi.GetUInt32("NOFA");
            }

            DataSetGeographicReferenceRecord = reader.ReadDataRecord();
            var dspm = DataSetGeographicReferenceRecord.Fields.GetFieldByTag("DSPM");
            if (dspm != null)
            {
                horizontalGeodeticDatum = dspm.GetUInt32("HDAT");
                verticalDatum = dspm.GetUInt32("VDAT");
                soundingDatum = dspm.GetUInt32("SDAT");
                compilationScaleOfData = dspm.GetUInt32("CSCL");
                unitsOfDepthMeasurement = dspm.GetUInt32("DUNI");
                unitsOfHeightMeasurement = dspm.GetUInt32("HUNI");
                unitsOfPositionalAccuracy = dspm.GetUInt32("PUNI");
                coordinateUnits = (CoordinateUnits)dspm.GetUInt32("COUN");
                coordinateMultiplicationFactor = dspm.GetUInt32("COMF");
                soundingMultiplicationFactor = dspm.GetUInt32("SOMF");
                // COMT
            }

            // DSPR Dataset projection
            // DSRC Dataset registration control
            // DSHT Dataset history
            // DSAC Dataset accuracy
            // CATD catalogue directory
            // CATX Catalogue cross reference

            List<DataRecord> fr = new List<DataRecord>();
            List<DataRecord> vr = new List<DataRecord>();

            var nextRec = reader.ReadDataRecord();

            while (nextRec != null)
            {
                if(nextRec.Fields.FindFieldByTag("VRID"))
                {
                    vr.Add(nextRec);
                }
                else
                {
                    if (nextRec.Fields.FindFieldByTag("FRID"))
                    {
                        fr.Add(nextRec);
                    }
                }
                nextRec = reader.ReadDataRecord();
            }

            FeatureRecords = fr;
            VectorRecords = vr;
        }

        public void ApplyUpdateFile(UpdateFile updateFile)
        {
            throw new NotImplementedException("ApplyUpdateFile not implemented");
        }
    }
}
