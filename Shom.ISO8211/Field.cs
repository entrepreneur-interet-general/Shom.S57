using System;

namespace Shom.ISO8211
{
    public abstract class Field
    {
        private readonly string _tag;

        public Field(string tag)
        {
            _tag = tag;
        }

        public string Tag
        {
            get { return _tag; }
        }

        public override string ToString()
        {
            return "FIELD TAG: " + _tag + Environment.NewLine;
        }
    }
}