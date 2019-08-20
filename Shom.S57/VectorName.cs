using System;

namespace S57
{
    public struct VectorName : IEquatable<VectorName>
    {
        public uint Type;
        public uint RecordIdentificationNumber;
        public VectorName(uint rcnm, uint rcid)
        {
            Type = rcnm;
            RecordIdentificationNumber = rcid;
        }
        public bool Equals(VectorName other)
        {
            return this.Type == other.Type && this.RecordIdentificationNumber == other.RecordIdentificationNumber;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is VectorName && Equals((VectorName)obj);
        }
        public override int GetHashCode()
        {
            return ((int)this.Type * 397) ^ (int)this.RecordIdentificationNumber;
        }
    }
}
