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
            //re-use same stringbuilder to reduce garbage generation and collection by ~30%
            var sb = new StringBuilder();
            while (currentIndex < bytes.Length - 1) // -1 excludes the FieldTerminator
            {
                sb.Clear();
                //var sb = new StringBuilder();
                for (int i = 0; i < sizeOfTagField; i++)
                {
                    sb.Append((char) bytes[currentIndex]);
                    currentIndex++;
                }
                string tag = sb.ToString();

                var entry = new DirectoryEntry();
                entry.FieldTag = tag;

                sb.Clear();
                //var sb2 = new StringBuilder();
                for (int i = 0; i < sizeOfLengthField; i++)
                {
                    //sb2.Append((char) bytes[currentIndex]);
                    sb.Append((char)bytes[currentIndex]);
                    currentIndex++;
                }
                //string s2 = sb2.ToString();
                string s2 = sb.ToString();
                entry.FieldLength = Int32.Parse(s2);

                sb.Clear();
                //var sb3 = new StringBuilder();
                for (int i = 0; i < sizeOfPositionField; i++)
                {
                    //sb3.Append((char) bytes[currentIndex]);
                    sb.Append((char)bytes[currentIndex]);
                    currentIndex++;
                }
                //string s3 = sb3.ToString();
                string s3 = sb.ToString();
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