using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace S57
{
    public class VectorName
    {
        public VectorName(uint rcnm, uint rcid)
        {
            Type = (VectorType)rcnm;
            RecordIdentificationNumber = rcid; 
        }

        public VectorType Type;
        public VectorType RecordName { get { return Type; } }
        public uint RecordIdentificationNumber;

        public override string ToString()
        {
            return string.Format( "{0}-{1}", Type.ToString() , RecordIdentificationNumber );
        }
    }

}
