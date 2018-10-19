using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Shom.ISO8211
{
    public class DataField : Field
    {
        public static byte UnitTerminator = 0x1F;
        public static byte FieldTerminator = 0x1E;

        private readonly byte[] _bytes;
        private DataDescriptiveField _fieldDescription;

        public Dictionary<string, object> SubFields = new Dictionary<string, object>();
   
        public byte[] Bytes { get { return _bytes;  } }
        public DataDescriptiveField FieldDescription { get { return _fieldDescription;  } }

        public DataField(string tag, DataDescriptiveField fieldDescription, byte[] bytes) : base(tag)
        {
            _fieldDescription = fieldDescription;
            _bytes = bytes;

            int currentIndex = 0;
            foreach (SubFieldDefinition subFieldDefinition in _fieldDescription.SubFieldDefinitions)
            {
                if (subFieldDefinition.FormatTypeCode == FormatTypeCode.CharacterData)
                {
                    var sb = new StringBuilder();
                    if (subFieldDefinition.SubFieldWidth == 0)
                    {
                        while (_bytes[currentIndex] != UnitTerminator)
                        {
                            sb.Append((char)_bytes[currentIndex]);
                            currentIndex++;
                        }
                        //Consume the Terminator
                        currentIndex++;
                    }
                    else
                    {
                        for (int i = 0; i < subFieldDefinition.SubFieldWidth; i++)
                        {
                            sb.Append((char)_bytes[currentIndex]);
                            currentIndex++;
                        }
                    }
                    var s = sb.ToString();
                    SubFields.Add(subFieldDefinition.Tag, s);
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
                                int tempVal = _bytes[currentIndex++];
                                for (int j = 0; j < i; j++)
                                {
                                    tempVal = tempVal << 8;
                                }
                                signedValue += tempVal;
                            }
                            SubFields.Add(subFieldDefinition.Tag, signedValue);
                            break;
                        case ExtendedBinaryForm.IntegerUnsigned:
                            if (subFieldDefinition.BinaryFormPrecision > 4)
                            {
                                throw new NotImplementedException("Only handle unsigned Ints 4 bytes or less");
                            }
                            UInt32 unsignedValue = 0;
                            for (int i = 0; i < subFieldDefinition.BinaryFormPrecision; i++)
                            {
                                UInt32 tempVal = _bytes[currentIndex++];
                                for (int j = 0; j < i; j++)
                                {
                                    tempVal = tempVal << 8;
                                }
                                unsignedValue += tempVal;
                            }
                            SubFields.Add(subFieldDefinition.Tag, unsignedValue);
                            break;
                        default:
                            throw new NotImplementedException("Unhandled LsofBinaryForm");
                    }
                }
                else if (subFieldDefinition.FormatTypeCode == FormatTypeCode.ExplicitPoint)
                {
                    if (subFieldDefinition.SubFieldWidth == 0)
                    {
                        throw new Exception("Expected a subfield width for Explicit Point Type");
                    }
                    var tempSb = new StringBuilder();
                    for (int i = 0; i < subFieldDefinition.SubFieldWidth; i++)
                    {
                        tempSb.Append((char)_bytes[currentIndex]);
                        currentIndex++;
                    }
                    double value = 0;
                    value = Double.Parse(tempSb.ToString(), CultureInfo.InvariantCulture);
                    SubFields.Add(subFieldDefinition.Tag, value);
                }
                else if (subFieldDefinition.FormatTypeCode == FormatTypeCode.BitStringData)
                {
                    if (subFieldDefinition.SubFieldWidth == 0)
                    {
                        throw new Exception("Expected a subfield width for Bit String Data");
                    }
                    //divide by 8 and round up
                    int bytesToRead = (subFieldDefinition.SubFieldWidth + (8 - 1)) / 8;
                    byte[] newByteArray = new byte[bytesToRead];
                    for (int i = 0; i < bytesToRead; i++)
                    {
                        newByteArray[i] = _bytes[currentIndex];
                        currentIndex++;
                    }
                    SubFields.Add(subFieldDefinition.Tag, newByteArray);
                }
                else
                {
                    throw new Exception("Unhandled subField type :" + subFieldDefinition.FormatTypeCode);
                }

                //if (bytes[bytes.Length - 1] != FieldTerminator) throw new Exception("Expected Field Terminator");
            }
        }

        public Int32 GetInt32(string tag)
        {
            object val;
            if (!SubFields.TryGetValue(tag, out val))
            {
                throw new Exception("Could not find subfield " + tag);
            }
            return (Int32)val;
        }

        public UInt32 GetUInt32(string tag)
        {
            object val;
            if (!SubFields.TryGetValue(tag, out val))
            {
                throw new Exception("Could not find subfield " + tag);
            }
            return (UInt32)val;
        }

        public String GetString(string tag)
        {
            object val;
            if (!SubFields.TryGetValue(tag, out val))
            {
                throw new Exception("Could not find subfield " + tag);
            }
            return (String)val;
        }

        public Double GetDouble(string tag)
        {
            object val;
            if (!SubFields.TryGetValue(tag, out val))
            {
                throw new Exception("Could not find subfield " + tag);
            }
            return Convert.ToDouble(val);
        }

        public byte[] GetBytes(string tag)
        {
            object val;
            if (!SubFields.TryGetValue(tag, out val))
            {
                throw new Exception("Could not find subfield " + tag);
            }
            return (byte[])val;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(base.ToString());
            foreach (var value in SubFields)
            {
                if (value.Value.GetType() == typeof(byte[]))
                {
                    byte[] bytes = (byte[])value.Value;
                    sb.Append(value.Key + ":");    
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        sb.Append("[" + bytes[i] + "]");    
                    }
                    sb.Append(Environment.NewLine);    
                }
                else
                {
                    sb.Append(value.Key + ":" + value.Value + Environment.NewLine);    
                }
            }
            return sb.ToString();
        }
    }
}