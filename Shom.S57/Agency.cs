using System;

namespace S57
{
    public class Agency
    {
        internal Agency(uint code)
        {
            _code = code;
        }

        private uint _code;

        public uint Code
        {
            get
            {
                return _code;
            }
        }

        public string Name
        {
            get
            {
                switch (_code)
                {
                    case 550: return "NOAA";
                    case 170: return "Shom";
                }

                throw new NotImplementedException("Producing Agencies");
            }
        }
    }
}
