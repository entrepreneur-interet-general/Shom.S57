using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shom.ISO8211;
using S57.File;
using System.Diagnostics;

namespace S57
{
    public class Catalogue
    {
        public DataRecord _cr;

        public uint RecordIdentificationNumber;
        public string fileName;
        public string fileLongName;
        public double southernMostLatitude;
        public double westernMostLongitude;
        public double northernMostLatitude;
        public double easternMostLongitude;

        public DataRecord DataRecord
        {
            get { return _cr; }
        }

        
        public Catalogue(S57Reader reader, DataRecord cr, CatalogueFile catalogueFile)
        {
            _cr = cr;
            BuildFromDataRecord(reader, cr, catalogueFile);
        }

        public void BuildFromDataRecord(S57Reader reader, DataRecord cr, CatalogueFile catalogueFile)
        {
            // Record Identifier Field
            var rcid = cr.Fields.GetFieldByTag("CATD");
            if (rcid != null)
            {
                RecordIdentificationNumber = (uint)rcid.GetInt32("RCID");
                fileName = rcid.GetString("FILE");
                fileLongName = rcid.GetString("LFIL");
                southernMostLatitude = rcid.GetDouble("SLAT");
                westernMostLongitude = rcid.GetDouble("WLON");
                northernMostLatitude = rcid.GetDouble("NLAT");
                easternMostLongitude = rcid.GetDouble("ELON");
            }   
        }        
    }
}
