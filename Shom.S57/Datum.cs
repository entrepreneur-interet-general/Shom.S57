using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace S57
{
    public class Datum
    {
        internal Datum(uint code)
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
                    case 1: return "Mean low water springs";
                    case 2: return "Mean lower low water springs";
	                case 3: return "Mean sea level";
	                case 4: return "Lowest low water";
	                case 5: return "Mean low water";
	                case 6: return "Lowest low water springs";
	                case 7: return "Approximate mean low water springs";
	                case 8: return "Indian spring low water";
	                case 9: return "Low water springs";
	                case 10: return "Approximate lowest astronomical tide";
	                case 11: return "Nearly lowest low water";
	                case 12: return "Mean lower low water";
	                case 13: return "Low water";
	                case 14: return "Approximate mean low water";
	                case 15: return "Approximate mean lower low water";
	                case 16: return "Mean high water";
	                case 17: return "Mean high water springs";
	                case 18: return "High water";
	                case 19: return "Approximate mean sea level";
	                case 20: return "High water springs";
	                case 21: return "Mean higher high water";
	                case 22: return "Equinoctial spring low water";
	                case 23: return "Lowest astronomical tide";
	                case 24: return "Local datum";
	                case 25: return "International Great Lakes Datum 1985";
	                case 26: return "Mean water level";
	                case 27: return "Lower low water large tide";
	                case 28: return "Higher high water large tide";
	                case 29: return "Nearly highest high water";
	                case 30: return "Highest astronomical tide (HAT)";
                }
                throw new Exception("Unexpected Vertical Datum Code");
            } 
        }
    }
}
