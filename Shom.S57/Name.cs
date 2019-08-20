using System;

namespace S57
{
    public class Name
    {
        public Name(byte[] bytes)
        {
            if (bytes.Length != 5)
            {
                throw new ArgumentException("Expected byte array with 5 items");
            }

            RecordName = bytes[0];
            RecordIdentificationNumber = (uint) (bytes[4] << 24)
                                       + (uint) (bytes[3] << 16)
                                       + (uint) (bytes[2] << 8)
                                       + (uint) (bytes[1]);
        }

        public uint RecordName { get; private set; }

        public uint RecordIdentificationNumber { get; private set; }
    }
}
