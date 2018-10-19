using System.Collections.Generic;
using System.Text;

namespace Shom.ISO8211
{
    public class DataDescriptiveRecordFields : List<DataDescriptiveRecordField>
    {
        public DataDescriptiveRecordField GetFieldByTag(string tag)
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

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (DataDescriptiveRecordField entry in this)
            {
                sb.Append(entry);
            }

            return sb.ToString();
        }
    }
}