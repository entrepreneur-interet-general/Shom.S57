using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shom.ISO8211;

namespace S57.File
{
    public class UpdateFile
    {
        public List<DataRecord> UpdateFeatureRecords;
        public List<DataRecord> UpdateVectorRecords;

        public UpdateFile(Iso8211Reader reader)
        {
            //throw new NotImplementedException("UpdateFile not implemented");
            List<DataRecord> fr = new List<DataRecord>();
            List<DataRecord> vr = new List<DataRecord>();

            var nextRec = reader.ReadDataRecord();

            while (nextRec != null)
            {
                if (nextRec.Fields.FindFieldByTag("VRID"))
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

            UpdateFeatureRecords = fr;
            UpdateVectorRecords = vr;
        }
    }
}
