using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shom.ISO8211;

namespace S57.File
{
    public class CatalogueFile
    {

        public List<DataRecord> CatalogueRecords = new List<DataRecord>(); 
        public CatalogueFile(Iso8211Reader reader)
        {
            var nextRec = reader.ReadDataRecord();
            while (nextRec != null)
            {
                DataField field = nextRec.Fields.GetFieldByTag("CATD");
                if (field != null)
                {
                    CatalogueRecords.Add(nextRec);
                }
                nextRec = reader.ReadDataRecord();
            }
        }
    }
}
