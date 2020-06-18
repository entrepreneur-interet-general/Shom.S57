using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Shom.ISO8211
{
    public class EntryMap
    {
        private readonly int _sizeOfLengthField;
        private readonly int _sizeOfPositionField;
        private readonly int _sizeOfTagField;

        public EntryMap(ArraySegment<byte> bytessegment)
        {
            int _offset = bytessegment.Offset;
            _sizeOfLengthField = ByteToCharToInt(bytessegment.Array[_offset+20]);
            _sizeOfPositionField = ByteToCharToInt(bytessegment.Array[_offset + 21]);
            var reserved = (char)bytessegment.Array[_offset + 22];
            _sizeOfTagField = ByteToCharToInt(bytessegment.Array[_offset + 23]);
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
            //return Int32.Parse(new string(new[] {(char) b}));
            return (b - '0');
        }
    }
}