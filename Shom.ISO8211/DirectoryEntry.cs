using System;
using System.Text;

namespace Shom.ISO8211
{
    public class DirectoryEntry
    {
        public int FieldLength;
        public int FieldPosition;
        public string FieldTag;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Field Tag: " + FieldTag + Environment.NewLine);
            sb.Append("Field Length: " + FieldLength + Environment.NewLine);
            sb.Append("Field Position: " + FieldPosition + Environment.NewLine);

            return sb.ToString();
        }
    }
}