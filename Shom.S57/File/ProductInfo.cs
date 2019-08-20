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


        public ProductInfo(Iso8211Reader reader)
        {
            DataSetGeneralInformationRecord = reader.ReadDataRecord();
            var dsid = DataSetGeneralInformationRecord.Fields.GetFieldByTag("DSID");
            if (dsid != null)
            {
                IntendedUsage = dsid.GetUInt32("INTU");
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
        }
    }
}
