using System.Collections.Generic;
using System.Text;

namespace Shom.ISO8211
{
    public class DataRecordFields : List<DataField>
    {
        public DataField GetFieldByTag(string tag)
        {
            foreach (var field in this)
            {
                if (field.Tag == tag)
                {
                    return field; 
                }
            }

            return null;
        }
        public bool FindFieldByTag(string tag)
        {
            foreach (var field in this)
            {
                if (field.Tag == tag)
                {
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (DataField entry in this)
            {
                sb.Append(entry);
            }

            return sb.ToString();
        }
    }
}