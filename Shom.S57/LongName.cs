using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace S57
{
    public class LongName
    {
        public LongName(byte[] bytes)
        {
            if (bytes.Length != 8)
            {
                throw new ArgumentException("Expected byte array with 8 items");
            }

            ProducingAgency = + (uint)(bytes[1] << 8)
                              + (uint)(bytes[0]);

            FeatureIdentificationNumber = (uint)(bytes[5] << 24)
                                        + (uint)(bytes[4] << 16)
                                        + (uint)(bytes[3] << 8)
                                        + (uint)(bytes[2]);

            FeatureIdentificationSubdivision = + (uint)(bytes[7] << 8)
                                               + (uint)(bytes[6]);
        }

        public LongName(uint agen, uint fidn, uint fids)
        {
            ProducingAgency = agen;
            FeatureIdentificationNumber = fidn;
            FeatureIdentificationSubdivision = fids;
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}-{2}", ProducingAgency, FeatureIdentificationNumber, FeatureIdentificationSubdivision);
        }

        public string ToCarisString()
        {
            if (ProducingAgency == 170)
            {
                return string.Format("FR {0} {1}", FeatureIdentificationNumber, FeatureIdentificationSubdivision);
            }
            else
            {
                return string.Format("{0} {1} {2}", ProducingAgency, FeatureIdentificationNumber, FeatureIdentificationSubdivision);
            }
        }

        public uint ProducingAgency { get; private set; }

        public uint FeatureIdentificationNumber { get; private set; }

        public uint FeatureIdentificationSubdivision { get; private set; }
    }
}
