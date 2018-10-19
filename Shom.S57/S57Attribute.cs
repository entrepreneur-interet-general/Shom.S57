using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace S57
{
    public class S57Attribute
    {
        public S57Attribute(uint code, string acronym, string name )
        {
            Name = name;
            Acronym = acronym;
            Code = code;
        }
        public string Name { get; private set; }
        public string Acronym { get; private set; }
        public uint Code { get; private set; }
}

    public static class S57Attributes
    {
        public static List<S57Attribute> Attributes
        {
            get { return _attributes;  }
        }

        private static List<S57Attribute> _attributes = new List<S57Attribute>()
        {
            new S57Attribute( 2,    "BCNSHP",  "Beacon Shape" ),
            new S57Attribute( 4,    "BOYSHP",  "Buoy Shape" ),
            new S57Attribute( 10,   "CATBUA",  "Category of built-up area" ),
            new S57Attribute( 13,   "CATCAM",  "Category of cardinal mark" ),
            new S57Attribute( 18,   "CATCOV",  "Category of coverage" ),
            new S57Attribute( 35,   "CATLMK",  "Category of landmark" ),
            new S57Attribute( 36,   "CATLAM",  "Category of lateral mark"),
            new S57Attribute( 37,   "CATLIT",  "Category of light" ),
            new S57Attribute( 41,   "CATNAV",  "Category of navigation line"),
            new S57Attribute( 54,   "CATTRK",  "Category of recommended track"),
            new S57Attribute( 66,   "CATSPM",  "Category of special purpose mark"),
            new S57Attribute( 72,   "CATZOC",  "Category of zone of confidence in data"),
            new S57Attribute( 75,   "COLOUR",  "Colour"),
            new S57Attribute( 76,   "COLPAT",  "Colour Pattern"),
            new S57Attribute( 81,   "CONDTN",  "Condition"),
            new S57Attribute( 82,   "CONRAD",  "Conspicuous, radar"),
            new S57Attribute( 83,   "CONVIS",  "Conspicuous, visually"),
            new S57Attribute( 85,   "DATEND",  "Date end" ),
            new S57Attribute( 90,   "ELEVAT",  "Elevation"),
            new S57Attribute( 92,   "EXCLIT",  "Exhibition condition of light" ),
            new S57Attribute( 94,   "FUNCTN",  "Function" ),
            new S57Attribute( 95,   "HEIGHT",  "Height" ),
            new S57Attribute( 102,  "INFORM",  "Infomation"),
            new S57Attribute( 109,  "MARSYS",  "Marks navigational - System of" ),
            new S57Attribute( 111,  "NATION",  "Nationality"),
            new S57Attribute( 112,  "NATCON",  "Nature of construction" ),
            new S57Attribute( 116,  "OBJNAM", "Object name" ),
            new S57Attribute( 117,  "ORNT",   "Orientation" ),
            new S57Attribute( 118,  "PEREND", "Periodic date end" ),
            new S57Attribute( 119,  "PERSTA", "Periodic date start" ),
            new S57Attribute( 133,  "SCAMIN", "Scale minimum" ),
            new S57Attribute( 107,  "LITCHR", "Light characteristic" ),
            new S57Attribute( 108,  "LITVIS", "Light visibility" ),
            new S57Attribute( 110,  "MLTYLT", "Multiplicity of lights" ),
            new S57Attribute( 136,  "SECTR1", "Sector limit one" ),
            new S57Attribute( 137,  "SECTR2", "Sector limit two" ),
            new S57Attribute( 141,  "SIGGRP",  "Signal group" ),
            new S57Attribute( 142,  "SIGPER",  "Signal period" ),
            new S57Attribute( 143,  "SIGSEQ",  "Signal sequence" ),
            new S57Attribute( 147,  "SORDAT",  "Source date" ),
            new S57Attribute( 148,  "SORIND",  "Source indication" ),
            new S57Attribute( 149,  "STATUS",  "Status" ),
            new S57Attribute( 151,  "SUREND",  "Survey date - end" ),
            new S57Attribute( 152,  "SURSTA",  "Survey date - start" ),
            new S57Attribute( 156,  "TECSOU",  "Technique of sounding measurement" ),
            new S57Attribute( 158,  "TXTDSC",  "Textual description" ),
            new S57Attribute( 171,  "TOPSHP",  "Topmark/daymark shape"),
            new S57Attribute( 172,  "TRAFIC",  "Traffic flow" ),
            new S57Attribute( 178,  "VALNMR",  "Value of nominal range" ),
            new S57Attribute( 180,  "VERACC",  "Vertical accuracy" ),
            new S57Attribute( 185,  "VERDAT",  "Vertical datum" ),
            new S57Attribute( 186,  "VERLEN",  "Vertical length" ),
            new S57Attribute( 300,  "NINFOM",  "Information in national language" ),
            new S57Attribute( 301,  "NOBJNM",  "Object name in national language" ),
            new S57Attribute( 401,  "POSACC",  "Positional Accuracy" ),
            new S57Attribute( 402,  "QUAPOS",  "Quality of position" ),
        };

        public static S57Attribute Get(uint code)
        {
            foreach (var attribute in _attributes)
            {
                if (attribute.Code == code)
                {
                    return attribute;
                }
            }
            return null;
        }

        public static S57Attribute Get(string acronym)
        {
            foreach (var attribute in _attributes)
            {
                if (attribute.Acronym == acronym)
                {
                    return attribute;
                }
            }
            return null;
        }

        public static bool IsGeo(uint code)
        {
            return (code >= 1 && code <= 159);
        }

        public static bool IsMeta(uint code)
        {
            return (code >= 300 && code <= 312);
        }

        public static bool IsCollection(uint code)
        {
            return (code >= 400 && code <= 402);
        }

        public static bool IsCartographic(uint code)
        {
            return (code >= 500 && code <= 504);
        }

    }
}
