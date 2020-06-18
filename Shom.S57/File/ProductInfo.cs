using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shom.ISO8211;

namespace S57.File
{
    public class ProductInfo
    {
        public DataRecord DataSetGeneralInformationRecord = null;
        public DataRecord DataSetGeographicReferenceRecord = null;

        //DSID
        public uint IntendedUsage;

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

        // some private variables  
        SFcontainer[] subFieldRow;
        List<string> tagLookup;

        public ProductInfo(Iso8211Reader reader)
        {
            DataSetGeneralInformationRecord = reader.ReadDataRecord();
            var dsid = DataSetGeneralInformationRecord.Fields.GetFieldByTag("DSID");
            if (dsid != null)
            {                
                IntendedUsage = dsid.subFields.GetUInt32(0, "INTU"); 
            }
            DataSetGeographicReferenceRecord = reader.ReadDataRecord();
            var dspm = DataSetGeographicReferenceRecord.Fields.GetFieldByTag("DSPM");
            if (dspm != null)
            {
                subFieldRow = dspm.subFields.Values[0];
                tagLookup = dspm.subFields.TagIndex;
                horizontalGeodeticDatum = subFieldRow.GetUInt32(tagLookup.IndexOf("HDAT"));
                verticalDatum = subFieldRow.GetUInt32(tagLookup.IndexOf("VDAT"));
                soundingDatum = subFieldRow.GetUInt32(tagLookup.IndexOf("SDAT"));
                compilationScaleOfData = subFieldRow.GetUInt32(tagLookup.IndexOf("CSCL"));
                unitsOfDepthMeasurement = subFieldRow.GetUInt32(tagLookup.IndexOf("DUNI"));
                unitsOfHeightMeasurement = subFieldRow.GetUInt32(tagLookup.IndexOf("HUNI"));
                unitsOfPositionalAccuracy = subFieldRow.GetUInt32(tagLookup.IndexOf("PUNI"));
                coordinateUnits = (CoordinateUnits)subFieldRow.GetUInt32(tagLookup.IndexOf("COUN"));
                coordinateMultiplicationFactor = subFieldRow.GetUInt32(tagLookup.IndexOf("COMF"));
                soundingMultiplicationFactor = subFieldRow.GetUInt32(tagLookup.IndexOf("SOMF"));
                // COMT
            }
        }
    }
}
