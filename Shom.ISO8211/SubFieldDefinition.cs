using System;

namespace Shom.ISO8211
{
    public class SubFieldDefinition
    {
        private readonly FormatTypeCode _formatTypeCode;
        public int BinaryFormPrecision;
        public ExtendedBinaryForm BinaryFormSubType;
        public string Format;
        public int SubFieldWidth;
        public string Tag;

        public SubFieldDefinition(string tag, string format)
        {
            Tag = tag;
            Format = format;

            switch (Format[0])
            {
                case 'A':
                    _formatTypeCode = FormatTypeCode.CharacterData;
                    ParseSubFieldWidthOctets(format);
                    break;
                case 'I':                               //case I added
                    _formatTypeCode = FormatTypeCode.ImplicitPoint;
                    ParseSubFieldWidthOctets(format);
                    break;
                case 'R':
                    _formatTypeCode = FormatTypeCode.ExplicitPoint;
                    ParseSubFieldWidthOctets(format);
                    break;
                case 'B':
                    if (format.Contains("("))
                    {
                        _formatTypeCode = FormatTypeCode.BitStringData;
                        ParseSubFieldWidthOctets(format);
                    }
                    else
                    {
                        _formatTypeCode = FormatTypeCode.MsofBinaryForm;
                        ParseBinaryFormData(format);
                    }

                    break;
                case 'b':
                    _formatTypeCode = FormatTypeCode.LsofBinaryForm;
                    ParseBinaryFormData(format);
                    break;
                default:
                    throw new NotImplementedException("Different Format Type Code");
            }
        }

        public FormatTypeCode FormatTypeCode
        {
            get { return _formatTypeCode; }
        }

        private void ParseSubFieldWidthOctets(string format)
        {
            if (format.Contains("("))
            {
                string temp = format.Substring(1);
                temp = temp.Trim(new[] {'(', ')'});
                SubFieldWidth = Int32.Parse(temp);
            }
        }

        private void ParseBinaryFormData(string format)
        {
            BinaryFormSubType = (ExtendedBinaryForm) Int32.Parse(format[1].ToString());
            BinaryFormPrecision = Int32.Parse(format[2].ToString());
        }

        public override string ToString()
        {
            string output = Tag + ":" + Format + ":" + FormatTypeCode.ToString();
            if (SubFieldWidth != 0)
            {
                output += ":SFWidth-" + SubFieldWidth;
            }

            if (BinaryFormSubType != 0)
            {
                output += ":BFSubType-" + BinaryFormSubType;
            }

            if (BinaryFormPrecision != 0)
            {
                output += ":BFPrecision-" + BinaryFormPrecision;
            }

            return output;
        }
    }
}