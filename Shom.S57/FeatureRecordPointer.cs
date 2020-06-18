using Shom.ISO8211;
using System.Collections.Generic;

namespace S57
{
    public class FeatureObjectPointer : SubFields
    {
        public List<Feature> FeatureList;
        public FeatureObjectPointer(SubFields ffpt)
        {
            this.TagIndex = ffpt.TagIndex;
            this.Values = ffpt.Values;
            FeatureList = new List<Feature>(); //consider intializing with 2 for speadup
        }
    }    
}
