using System;

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
        public char UnitTerminatorPrintableGraphic;
        int val;

        public DataDescriptiveRecordField(string tag, byte[] fieldControls) : base(tag)
        {
            val = (fieldControls[0]-'0');
            DataStructureCode = (DataStructureCode) val;

            val = (fieldControls[1] - '0');
            DataTypeCode = (DataTypeCode) val;

            if (DataTypeCode == DataTypeCode.BitStringIncludingBinaryForms && (char) fieldControls[2] != '0' &&
                (char) fieldControls[3] != '0')
            {
                throw new NotImplementedException("Processing Auxillary controls in Field Controls");
            }

            FieldTerminatorPrintableGraphic = (char) fieldControls[4];
            UnitTerminatorPrintableGraphic = (char) fieldControls[5];

            if (fieldControls.Length > 6)
            {
                TruncatedEscapeSequence[0] = (char)fieldControls[6];
                TruncatedEscapeSequence[1] = (char)fieldControls[7];
                TruncatedEscapeSequence[2] = (char)fieldControls[8];
                     if (fieldControls[6] == 0x20 && fieldControls[7] == 0x20 && fieldControls[8] == 0x20) //Space Space Space 
                    iso8211LexicalLevel = ISO8211LexicalLevel.ASCIIText;
                else if (fieldControls[6] == 0x2D && fieldControls[7] == 0x41 && fieldControls[8] == 0x20) //hyphen A Space
                    iso8211LexicalLevel = ISO8211LexicalLevel.ISO8859;
                else if (fieldControls[6] == 0x25 && fieldControls[7] == 0x2F && fieldControls[8] == 0x41) // percent slash A
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