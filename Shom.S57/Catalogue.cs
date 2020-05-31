using Shom.ISO8211;
using S57.File;
using System.Collections.Generic;

namespace S57
{
    public class Catalogue
    {
        public DataRecord _cr;

        public uint RecordIdentificationNumber;
        public string fileName;
        public string fileLongName;
        public uint NavigationalPurpose;
        public uint CompilationScale;
        public double southernMostLatitude;
        public double westernMostLongitude;
        public double northernMostLatitude;
        public double easternMostLongitude;

        // some private variables  
        SFcontainer[] subFieldRow;
        List<string> tagLookup;

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
            var catd = cr.Fields.GetFieldByTag("CATD");
            if (catd != null)
            {
                subFieldRow = catd.subFields.Values[0];
                tagLookup = catd.subFields.TagIndex;
                RecordIdentificationNumber = (uint)subFieldRow.GetInt32(tagLookup.IndexOf("RCID")); //this one ist stored as integer, so implementing GetUint32 to do merely a cast will fail
                fileName = subFieldRow.GetString(tagLookup.IndexOf("FILE"));
                fileLongName = subFieldRow.GetString(tagLookup.IndexOf("LFIL"));                
                southernMostLatitude = subFieldRow.GetDouble(tagLookup.IndexOf("SLAT"));
                westernMostLongitude = subFieldRow.GetDouble(tagLookup.IndexOf("WLON"));
                northernMostLatitude = subFieldRow.GetDouble(tagLookup.IndexOf("NLAT"));
                easternMostLongitude = subFieldRow.GetDouble(tagLookup.IndexOf("ELON"));
            }   
        }        
    }
}
