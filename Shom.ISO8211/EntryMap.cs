using System;
using System.Text;

namespace Shom.ISO8211
{
    public class EntryMap
    {
        private readonly int _sizeOfLengthField;
        private readonly int _sizeOfPositionField;
        private readonly int _sizeOfTagField;

        public EntryMap(byte[] bytes)
        {
            _sizeOfLengthField = ByteToCharToInt(bytes[0]);
            _sizeOfPositionField = ByteToCharToInt(bytes[1]);
            var reserved = (char) bytes[2];
            _sizeOfTagField = ByteToCharToInt(bytes[3]);
        }

        public int SizeOfLengthField
        {
            get { return _sizeOfLengthField; }
        }

        public int SizeOfPositionField
        {
            get { return _sizeOfPositionField; }
        }

        public int SizeOfTagField
        {
            get { return _sizeOfTagField; }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("Entry Map" + Environment.NewLine);
            sb.Append("Size of Length Field " + _sizeOfLengthField + Environment.NewLine);
            sb.Append("Size of Position Field " + _sizeOfPositionField + Environment.NewLine);
            sb.Append("Size of Tag Field " + _sizeOfTagField + Environment.NewLine);

            return sb.ToString();
        }

        private int ByteToCharToInt(byte b)
        {
            return Int32.Parse(new string(new[] {(char) b}));
        }
    }
}