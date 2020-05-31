using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Shom.ISO8211
{
    //Subfield class to be able to deal with 
    //non repeated subfields,e.g. VRID Vector Record Identifier (Values list has one row)
    //and repeated subfields, e.g. <R> ATTV Vector Record Attributes (Values list has many rows)

public struct SFcontainer
    {
        public int intValue;
        public uint uintValue;
        public object otherValue;
    }
    public class SubFields
    {
        public List<SFcontainer[]> Values;
        public List<string> TagIndex;

        //constructor
        public SubFields()
        {
        }
        public SubFields(List<string> tags)
        {
            Values = new List<SFcontainer[]>();
            TagIndex = tags;
        }
    }
}  