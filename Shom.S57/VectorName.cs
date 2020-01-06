using System;

namespace S57
{
    public struct NAMEkey : IEquatable<NAMEkey>
    {
        public uint RecordName;
        public uint RecordIdentificationNumber;
        public NAMEkey(byte[] bytes)
        {
            if (bytes.Length != 5)
                throw new ArgumentException("Expected byte array with 5 items");
            RecordName = bytes[0];
            RecordIdentificationNumber = (uint)(bytes[1] + (bytes[2] * 256) + (bytes[3] * 65536) + (bytes[4] * 16777216));
        }
        public NAMEkey(uint rcnm, uint rcid)
        {
            RecordName = rcnm;
            RecordIdentificationNumber = rcid;
        }
        public bool Equals(NAMEkey other)
        {
            return this.RecordName == other.RecordName && this.RecordIdentificationNumber == other.RecordIdentificationNumber;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is NAMEkey && Equals((NAMEkey)obj);
        }
        public override int GetHashCode()
        {
            return ((int)this.RecordName * 397) ^ (int)this.RecordIdentificationNumber;
        }
    }
}
