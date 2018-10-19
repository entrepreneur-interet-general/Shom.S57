using System;
using System.Collections.Generic;
using System.Text;

namespace Shom.ISO8211
{
    public class DataDescriptiveField : DataDescriptiveRecordField
    {
        private const char SubfieldDelimiter = '!';
        private const char VectorLabelDelimiter = '*';

        private readonly string _dataFieldName;
        private char[] ArrayDescriptorDelimiter = new[] {'\\', '\\'};

        public bool IsVector;
        public List<SubFieldDefinition> SubFieldDefinitions = new List<SubFieldDefinition>();

        public DataDescriptiveField(string tag, byte[] fieldControls, string dataFieldName, string arrayDescriptor,
                                    string formatControls) : base(tag, fieldControls)
        {
            _dataFieldName = dataFieldName;

            if (DataStructureCode == DataStructureCode.Concatenated)
            {
                throw new NotImplementedException("Concatenated Data Structure Code");
            }

            //assume just one of these groups
            formatControls = formatControls.Trim(new[] {'(', ')'});

            if (DataStructureCode == DataStructureCode.SingleItem)
            {
                if (arrayDescriptor != "")
                {
                    throw new Exception("Did not expect an ArrayDescriptor for a SingleItem DataStructureCode");
                }

                SubFieldDefinitions.Add(new SubFieldDefinition("", formatControls));
            }
            else
            {
                List<string> expandedFormats = ExpandFormats(formatControls);

                string currentLabel = "";
                int currentFormatIndex = 0;

                foreach (char c in arrayDescriptor)
                {
                    switch (c)
                    {
                        case SubfieldDelimiter:
                            SubFieldDefinitions.Add(new SubFieldDefinition(currentLabel, expandedFormats[currentFormatIndex++]));
                            currentLabel = "";
                            break;
                        case VectorLabelDelimiter:
                            IsVector = true;
                            break;
                        default:
                            currentLabel += c;
                            break;
                    }
                }

                if (!String.IsNullOrEmpty(currentLabel))
                {
                    SubFieldDefinitions.Add(new SubFieldDefinition(currentLabel, expandedFormats[currentFormatIndex++]));
                }

                if (SubFieldDefinitions.Count != expandedFormats.Count)
                {
                    throw new Exception("Different number of subfields and descriptors!");
                }
            }
        }

        public string DataFieldName
        {
            get { return _dataFieldName; }
        }

        private static List<string> ExpandFormats(string formatControls)
        {
            var expandedFormats = new List<string>();

            string[] formats = formatControls.Split(new[] {','});

            //expand descriptors
            foreach (string format in formats)
            {
                if (Char.IsDigit(format[0]))
                {
                    //assumes there will be less than 10...
                    for (int i = 0; i < Int32.Parse(new string(new[] {format[0]})); i++)
                    {
                        expandedFormats.Add(format.Substring(1, format.Length - 1));
                    }
                }
                else
                {
                    expandedFormats.Add(format);
                }
            }
            return expandedFormats;
        }


        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(base.ToString() + DataFieldName + Environment.NewLine);
            foreach (SubFieldDefinition subfield in SubFieldDefinitions)
            {
                sb.Append("--" + subfield.ToString() + Environment.NewLine);
            }

            return sb.ToString();
        }
    }
}