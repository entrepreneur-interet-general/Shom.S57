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

            while (currentIndex < bytes.Length - 1) // -1 excludes the FieldTerminator
            {
                var sb = new StringBuilder();
                for (int i = 0; i < sizeOfTagField; i++)
                {
                    sb.Append((char) bytes[currentIndex]);
                    currentIndex++;
                }
                string tag = sb.ToString();

                var entry = new DirectoryEntry();
                entry.FieldTag = tag;

                var sb2 = new StringBuilder();
                for (int i = 0; i < sizeOfLengthField; i++)
                {
                    sb2.Append((char) bytes[currentIndex]);
                    currentIndex++;
                }
                string s2 = sb2.ToString();
                entry.FieldLength = Int32.Parse(s2);

                var sb3 = new StringBuilder();
                for (int i = 0; i < sizeOfPositionField; i++)
                {
                    sb3.Append((char) bytes[currentIndex]);
                    currentIndex++;
                }
                string s3 = sb3.ToString();
                entry.FieldPosition = Int32.Parse(s3);

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