using System;
using System.Text;

namespace Shom.ISO8211
{
    public enum ISO8211LexicalLevel : uint
    {
        ASCIIText = 0,  // ASCII
        ISO8859 = 1,    // ISO 8859 part 1, Latin alphabet 1 repertoire (i.e. Western European Latin alphabet based languages.
        ISO10646 = 2    // Unicode (Universal Character Set repertoire UCS-2 implementation level 1)
    }
    public abstract class DataDescriptiveRecordField : Field
    {
        public DataStructureCode DataStructureCode;
        public DataTypeCode DataTypeCode;
        public char FieldTerminatorPrintableGraphic;
        public char[] TruncatedEscapeSequence = new char[3];
        public ISO8211LexicalLevel iso8211LexicalLevel;
        public Encoding iso8859Encoding; // to chache iso8859 encoding (get encoding is pretty expensive)
        public char UnitTerminatorPrintableGraphic;
        int val;

        public DataDescriptiveRecordField(string tag, ArraySegment<byte> fieldControls) : base(tag)
        {
            int _offset = fieldControls.Offset;
            val = (fieldControls.Array[_offset+0] -'0');
            DataStructureCode = (DataStructureCode) val;

            val = (fieldControls.Array[_offset+1] - '0');
            DataTypeCode = (DataTypeCode) val;

            if (DataTypeCode == DataTypeCode.BitStringIncludingBinaryForms && (char) fieldControls.Array[_offset+2] != '0' &&
                (char) fieldControls.Array[_offset+3] != '0')
            {
                throw new NotImplementedException("Processing Auxillary controls in Field Controls");
            }

            FieldTerminatorPrintableGraphic = (char) fieldControls.Array[_offset+4];
            UnitTerminatorPrintableGraphic = (char) fieldControls.Array[_offset + 5];

            if (fieldControls.Count > 6)
            {
TruncatedEscapeSequence[0] = (char)fieldControls.Array[_offset + 6];
                TruncatedEscapeSequence[1] = (char)fieldControls.Array[_offset + 7];
                TruncatedEscapeSequence[2] = (char)fieldControls.Array[_offset + 8];
                if (fieldControls.Array[_offset + 6] == 0x20 && fieldControls.Array[_offset + 7] == 0x20 && fieldControls.Array[_offset + 8] == 0x20) //Space Space Space 
                    iso8211LexicalLevel = ISO8211LexicalLevel.ASCIIText;
                else if (fieldControls.Array[_offset + 6] == 0x2D && fieldControls.Array[_offset + 7] == 0x41 && fieldControls.Array[_offset + 8] == 0x20) //hyphen A Space
                {
                    iso8859Encoding = Encoding.GetEncoding("iso-8859-1");
                    iso8211LexicalLevel = ISO8211LexicalLevel.ISO8859;
                }
                else if (fieldControls.Array[_offset + 6] == 0x25 && fieldControls.Array[_offset + 7] == 0x2F && fieldControls.Array[_offset + 8] == 0x41) // percent slash A
                    iso8211LexicalLevel = ISO8211LexicalLevel.ISO10646;
            }
        }

        public override string ToString()
        {
            return base.ToString() + "FieldControls:" + DataStructureCode.ToString() + ":" + DataTypeCode.ToString() + ":" +
                   FieldTerminatorPrintableGraphic + ":" + UnitTerminatorPrintableGraphic + ":" +
                   new string(TruncatedEscapeSequence) + Environment.NewLine;
        }
    }
}