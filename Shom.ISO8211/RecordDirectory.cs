using System;
using System.Collections.Generic;
using System.Text;

namespace Shom.ISO8211
{
    public class RecordDirectory : List<DirectoryEntry>
    {
        public RecordDirectory(int sizeOfTagField, int sizeOfLengthField, int sizeOfPositionField, byte[] bytes)
        {
            int currentIndex = 0;
            int start = 0;
            while (currentIndex < bytes.Length - 1) // -1 excludes the FieldTerminator
            {        
                start = currentIndex;
                string tag  = Encoding.ASCII.GetString(bytes, start, sizeOfTagField);
                currentIndex += sizeOfTagField;

                var entry = new DirectoryEntry();
                entry.FieldTag = tag;

                for (int i = 0; i < sizeOfLengthField; i++)
                {
                    entry.FieldLength += ((bytes[currentIndex] - '0') * (int)Math.Pow(10, sizeOfLengthField - i - 1));
                    currentIndex++;
                }
                for (int i = 0; i < sizeOfPositionField; i++)
                {
                    entry.FieldPosition += ((bytes[currentIndex] - '0') * (int)Math.Pow(10, sizeOfPositionField - i - 1));
                    currentIndex++;
                }
                Add(entry);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (DirectoryEntry entry in this)
            {
                sb.Append(entry);
            }

            return sb.ToString();
        }
    }
}