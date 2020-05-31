using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Shom.ISO8211
{
    public class DataField : Field
    {
        public static byte UnitTerminator = 0x1F;
        public static byte FieldTerminator = 0x1E;

        private DataDescriptiveField _fieldDescription;

        public SubFields subFields;
        SFcontainer[] subFieldRow;

        public DataDescriptiveField FieldDescription { get { return _fieldDescription;  } }


        public DataField(string tag, DataDescriptiveField fieldDescription, ArraySegment<byte> bytes) : base(tag)
        {
            _fieldDescription = fieldDescription;

            int currentIndex = 0;
            int start = 0;
            int tagCount = _fieldDescription.SubFieldDefinitions.Count;
            if (tagCount > 0)
            {
                List<string> tags = new List<string>(tagCount);
                for (int i = 0; i < tagCount; i++)
                {
                    tags.Add(_fieldDescription.SubFieldDefinitions[i].Tag);
                }
                subFields = new SubFields(tags);
                //while (bytes[currentIndex] != FieldTerminator && currentIndex < Bytes.Length) //need to ignore fieldterminator as it can be part of a valid coordiante in sg2d field
                while (currentIndex < bytes.Count - 1) 
                {
                    subFieldRow = new SFcontainer[tagCount];
                    parseSubfields(ref tag, ref _fieldDescription, ref start, ref currentIndex, ref bytes);
                    subFields.Values.Add(subFieldRow);
                }
            }
        }
        private void parseSubfields(ref string tag, ref DataDescriptiveField _fieldDescription, ref int start, ref int currentIndex, ref ArraySegment<byte> _bytes)
        {
            int _offset = _bytes.Offset;
            int subFieldCounter = 0;
            int end = _fieldDescription.SubFieldDefinitions.Count;
            for (int count =0; count < end; count++)
            {
                SubFieldDefinition subFieldDefinition = _fieldDescription.SubFieldDefinitions[count];
                if (subFieldDefinition.FormatTypeCode == FormatTypeCode.CharacterData)
                {
                    string s = null;
                    start = currentIndex;
                    if (subFieldDefinition.SubFieldWidth == 0)
                    {
                        while (_bytes.Array[_offset+currentIndex] != UnitTerminator)
                        {
                            currentIndex++;
                        }
                        if (_fieldDescription.iso8211LexicalLevel == ISO8211LexicalLevel.ISO8859)
                            s = _fieldDescription.iso8859Encoding.GetString(_bytes.Array, _offset+ start, currentIndex - start);
                        else if (_fieldDescription.iso8211LexicalLevel == ISO8211LexicalLevel.ASCIIText)
                            s = Encoding.ASCII.GetString(_bytes.Array, _offset + start, currentIndex - start);
                        else if (_fieldDescription.iso8211LexicalLevel == ISO8211LexicalLevel.ISO10646)
                        {
                            s = Encoding.Unicode.GetString(_bytes.Array, _offset+start, currentIndex + 1 - start);
                            currentIndex++; //unicode Terminator has 2 bytes, consume first one
                        }
                        currentIndex++; //Consume the Terminator
                    }
                    else
                    {
                        currentIndex += subFieldDefinition.SubFieldWidth;
                        if (_fieldDescription.iso8211LexicalLevel == ISO8211LexicalLevel.ASCIIText)
                            s = Encoding.ASCII.GetString(_bytes.Array, _offset+start, subFieldDefinition.SubFieldWidth);
                        else if (_fieldDescription.iso8211LexicalLevel == ISO8211LexicalLevel.ISO8859)
                            s = _fieldDescription.iso8859Encoding.GetString(_bytes.Array, _offset+start, subFieldDefinition.SubFieldWidth);
                        else if (_fieldDescription.iso8211LexicalLevel == ISO8211LexicalLevel.ISO10646)
                        {
                            s = Encoding.Unicode.GetString(_bytes.Array, _offset+start, subFieldDefinition.SubFieldWidth);
                        }
                    }
                    subFieldRow[subFieldCounter].otherValue = s;

                }
                else if (subFieldDefinition.FormatTypeCode == FormatTypeCode.LsofBinaryForm)
                {
                    switch (subFieldDefinition.BinaryFormSubType)
                    {
                        case ExtendedBinaryForm.IntegerSigned:
                            if (subFieldDefinition.BinaryFormPrecision != 4)
                            {
                                throw new NotImplementedException("Only handle Signed Ints of 4 bytes");
                            }
                            int signedValue = 0;
                            for (int i = 0; i < subFieldDefinition.BinaryFormPrecision; i++)
                            {
                                int tempVal = _bytes.Array[_offset+currentIndex++];
                                for (int j = 0; j < i; j++)
                                {
                                    tempVal = tempVal << 8;
                                }
                                signedValue += tempVal;
                            }
                            subFieldRow[subFieldCounter].intValue = signedValue;
                            break;
                        case ExtendedBinaryForm.IntegerUnsigned:
                            if (subFieldDefinition.BinaryFormPrecision > 4)
                            {
                                throw new NotImplementedException("Only handle unsigned Ints 4 bytes or less");
                            }
                            UInt32 unsignedValue = 0;
                            for (int i = 0; i < subFieldDefinition.BinaryFormPrecision; i++)
                            {
                                UInt32 tempVal = _bytes.Array[_offset+currentIndex++];
                                for (int j = 0; j < i; j++)
                                {
                                    tempVal = tempVal << 8;
                                }
                                unsignedValue += tempVal;
                            }
                            subFieldRow[subFieldCounter].uintValue = unsignedValue;
                            break;
                        default:
                            throw new NotImplementedException("Unhandled LsofBinaryForm");
                    }
                }
                else if (subFieldDefinition.FormatTypeCode == FormatTypeCode.ExplicitPoint)
                {
                    string s;
                    start = currentIndex;
                    if (subFieldDefinition.SubFieldWidth == 0)
                    {
                        //throw new Exception("Expected a subfield width for Explicit Point Type");
                        //no need to throw exception, open ended floating point values (terminated by Unit terminator) are permitted, 
                        //see S57 specification (3.1 Main, section 7.4.1)
                        while (_bytes.Array[_offset+currentIndex] != UnitTerminator)
                        {
                            currentIndex++;
                        }
                        s = Encoding.ASCII.GetString(_bytes.Array, _offset+start, currentIndex - start);
                        //Consume the Terminator
                        currentIndex++;
                    }
                    else
                    {
                        currentIndex += subFieldDefinition.SubFieldWidth;
                        s = Encoding.ASCII.GetString(_bytes.Array, _offset+start, subFieldDefinition.SubFieldWidth);
                    }
                    double value = 0;
                    Double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
                    subFieldRow[subFieldCounter].otherValue = value;
                }
                else if (subFieldDefinition.FormatTypeCode == FormatTypeCode.ImplicitPoint) //added begin
                {
                    if (subFieldDefinition.SubFieldWidth == 0)
                    {
                        throw new Exception("Expected a subfield width for Implicit Point Type");
                    }
                    int value = 0;
                    for (int i = 0; i < subFieldDefinition.SubFieldWidth; i++)
                    {
                        value += ((_bytes.Array[_offset+currentIndex] - '0') * (int)Math.Pow(10, subFieldDefinition.SubFieldWidth - i - 1));
                        currentIndex++;
                    }
                    subFieldRow[subFieldCounter].intValue = value;
                }//added end
                else if (subFieldDefinition.FormatTypeCode == FormatTypeCode.BitStringData)
                {
                    if (subFieldDefinition.SubFieldWidth == 0)
                    {
                        throw new Exception("Expected a subfield width for Bit String Data");
                    }
                    //divide by 8 and round up
                    int bytesToRead = (subFieldDefinition.SubFieldWidth + (8 - 1)) / 8;
                    byte[] newByteArray = new byte[bytesToRead];
                    Array.Copy(_bytes.Array, _offset + currentIndex, newByteArray, 0, bytesToRead);
                    currentIndex += bytesToRead;
                    subFieldRow[subFieldCounter].otherValue = newByteArray;
                }
                else
                {
                    throw new Exception("Unhandled subField type :" + subFieldDefinition.FormatTypeCode);
                }
                subFieldCounter++;
                //if (bytes[bytes.Length - 1] != FieldTerminator) throw new Exception("Expected Field Terminator");
            }
        }       

        //public override string ToString()
        //{
        //    var sb = new StringBuilder();
        //    sb.Append(base.ToString());
        //    foreach (var value in SubFields)
        //    {
        //        if (value.Value.GetType() == typeof(byte[]))
        //        {
        //            byte[] bytes = (byte[])value.Value;
        //            sb.Append(value.Key + ":");    
        //            for (int i = 0; i < bytes.Length; i++)
        //            {
        //                sb.Append("[" + bytes[i] + "]");    
        //            }
        //            sb.Append(Environment.NewLine);    
        //        }
        //        else
        //        {
        //            sb.Append(value.Key + ":" + value.Value + Environment.NewLine);    
        //        }
        //    }
        //    return sb.ToString();
        //}
    }
    public static class MyExtensions
    {
        public static Int32 GetInt32(this SubFields val, int row, string Tag)
        {            
            return val.Values[row][val.TagIndex.IndexOf(Tag)].intValue;
        }
        public static UInt32 GetUInt32(this SubFields val, int row, string Tag)
        {
            return val.Values[row][val.TagIndex.IndexOf(Tag)].uintValue;
        }
        public static String GetString(this SubFields val, int row, string Tag)
        {
            return (String)val.Values[row][val.TagIndex.IndexOf(Tag)].otherValue;
        }
        public static Double GetDouble(this SubFields val, int row, string Tag)
        {
            return Convert.ToDouble(val.Values[row][val.TagIndex.IndexOf(Tag)].otherValue);
        }
        public static byte[] GetBytes(this SubFields val, int row, string Tag)
        {
            return (byte[])val.Values[row][val.TagIndex.IndexOf(Tag)].otherValue;
        }
        public static Int32 GetInt32(this SFcontainer[] val, int TagIndex)
        {
            return val[TagIndex].intValue;
        }
        public static UInt32 GetUInt32(this SFcontainer[] val, int TagIndex)
        {
            return val[TagIndex].uintValue; //this is necessary as some uints are stored as int, e.g. RCID as I(10) (instead b14) in Catalogue file 
        }
        public static String GetString(this SFcontainer[] val, int TagIndex)
        {
            return (String)val[TagIndex].otherValue;
        }
        public static Double GetDouble(this SFcontainer[] val, int TagIndex)
        {
            return Convert.ToDouble(val[TagIndex].intValue);
        }
        public static byte[] GetBytes(this SFcontainer[] val, int TagIndex)
        {
            return (byte[])val[TagIndex].otherValue;
        }
    }
}