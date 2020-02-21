
using Shom.ISO8211;
using System.Collections.Generic;

namespace S57
{
    public class VectorRecordPointer : SubFields
    {
        public List<Vector> VectorList;
        public VectorRecordPointer(SubFields vrpt)
        {
            this.TagIndex = vrpt.TagIndex;
            this.Tags = vrpt.Tags;
            this.Values = vrpt.Values;
            VectorList = new List<Vector>(); //consider intializing with 2 for speadup
        }
    }
}
