using System;
using S57.File;

namespace S57
{
    public class Cell
    {
        private readonly BaseFile _baseFile;

        public Cell(BaseFile baseFile)
        {
            _baseFile = baseFile;
        }

        public BaseFile BaseFile
        {
            get { return _baseFile;  }
        }

        public int EditionNumber
        {
            get
            {
                return Int32.Parse(_baseFile.DataSetGeneralInformationRecord.Fields.GetFieldByTag("DSID").GetString("EDTN"));
            }
        }

        public int UpdateNumber
        {
            get
            {
                return Int32.Parse(_baseFile.DataSetGeneralInformationRecord.Fields.GetFieldByTag("DSID").GetString("UPDN"));
            }
        }

        public uint IntendedUsage
        {
            get
            {
                return _baseFile.DataSetGeneralInformationRecord.Fields.GetFieldByTag("DSID").GetUInt32("INTU");
            }
        }

        public string DataSetName
        {
            get
            {
                return _baseFile.DataSetGeneralInformationRecord.Fields.GetFieldByTag("DSID").GetString("DSNM");
            }
        }

        public DateTime UpdateApplicationDate
        {
            get
            {
                return ConvertToDateTime(_baseFile.DataSetGeneralInformationRecord.Fields.GetFieldByTag("DSID").GetString("UADT"));
            }
        }

        public DateTime IssueDate
        {
            get
            {
                return ConvertToDateTime(_baseFile.DataSetGeneralInformationRecord.Fields.GetFieldByTag("DSID").GetString("ISDT"));
            }
        }

        public Agency ProducingAgency
        {
            get
            {
                return new Agency(_baseFile.DataSetGeneralInformationRecord.Fields.GetFieldByTag("DSID").GetUInt32("AGEN"));
            }
        }

        public string Comment
        {
            get
            {
                return _baseFile.DataSetGeneralInformationRecord.Fields.GetFieldByTag("DSID").GetString("COMT");
            }
        }

        public Datum VerticalDatum
        {
            get
            {
                return new Datum(_baseFile.DataSetGeographicReferenceRecord.Fields.GetFieldByTag("DSPM").GetUInt32("VDAT"));
            }
        }

        public Datum SoundingDatum
        {
            get
            {
                return new Datum(_baseFile.DataSetGeographicReferenceRecord.Fields.GetFieldByTag("DSPM").GetUInt32("SDAT"));
            }
        }

        private DateTime ConvertToDateTime(string date)
        {
            return new DateTime(Int32.Parse(date.Substring(0, 4)), 
                Int32.Parse(date.Substring(4, 2)), 
                Int32.Parse(date.Substring(6, 2)));
        }

        public Feature GetFeature(int index)
        {
            return null;
        }

        public Vector GetVector(int index)
        {
            return null;
        }

    }
}