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

    public class SubFields
    {
        public string[] Tags;
        public List<object[]> Values;
        public Dictionary<string, int> TagIndex;

        //constructor
        public SubFields()
        {
        }
        public SubFields(string[] tags)
        {
            Tags = tags;
            Values = new List<object[]>();
            TagIndex = new Dictionary<string, int>();
            for (int i =0; i<tags.Length; i++)
            {
                TagIndex.Add(tags[i], i);
            }
        }
        public object[] GetRow(int row)
        {
            return Values[row];
        }
        public object GetSubFieldbyTag(object[] row, string tag)
        {
            return row[TagIndex[tag]];
        }
        public bool FindSubFieldByTag(string tag)
        {
            if (TagIndex.ContainsKey(tag))
                return true;
            else
                return false;
        }

        //public override string ToString()
        //{
        //    var sb = new StringBuilder();

        //    foreach (object[] entry in this)
        //    {
        //        sb.Append(entry);
        //    }
        //    return sb.ToString();
        //}
    }
}