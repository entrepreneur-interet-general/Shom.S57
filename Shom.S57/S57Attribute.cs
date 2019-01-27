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
            //source: http://www.s-57.com/
            //retrieved on January 25, 2019

            //new S57Attribute(1, "AGENCY", "Agency responsible for production"),  //use in ENC prohibited
            new S57Attribute(2, "BCNSHP", "Beacon shape"),
            new S57Attribute(3, "BUISHP", "Building shape"),
            new S57Attribute(4, "BOYSHP", "Buoy shape"),
            new S57Attribute(5, "BURDEP", "Buried depth"),
            new S57Attribute(6, "CALSGN", "Call sign"),
            new S57Attribute(7, "CATAIR", "Category of airport/airfield"),
            new S57Attribute(8, "CATACH", "Category of anchorage"),
            new S57Attribute(9, "CATBRG", "Category of bridge"),
            new S57Attribute(10, "CATBUA", "Category of built-up area"),
            new S57Attribute(11, "CATCBL", "Category of cable"),
            new S57Attribute(12, "CATCAN", "Category of canal"),
            new S57Attribute(13, "CATCAM", "Category of cardinal mark"),
            new S57Attribute(14, "CATCHP", "Category of checkpoint"),
            new S57Attribute(15, "CATCOA", "Category of coastline"),
            new S57Attribute(16, "CATCTR", "Category of control point"),
            new S57Attribute(17, "CATCON", "Category of conveyor"),
            new S57Attribute(18, "CATCOV", "Category of coverage"),
            new S57Attribute(19, "CATCRN", "Category of crane"),
            new S57Attribute(20, "CATDAM", "Category of dam"),
            new S57Attribute(21, "CATDIS", "Category of distance mark"),
            new S57Attribute(22, "CATDOC", "Category of dock"),
            new S57Attribute(23, "CATDPG", "Category of dumping ground"),
            new S57Attribute(24, "CATFNC", "Category of fence/wall"),
            new S57Attribute(25, "CATFRY", "Category of ferry"),
            new S57Attribute(26, "CATFIF", "Category of fishing facility"),
            new S57Attribute(27, "CATFOG", "Category of fog signal"),
            new S57Attribute(28, "CATFOR", "Category of fortified structure"),
            new S57Attribute(29, "CATGAT", "Category of gate"),
            new S57Attribute(30, "CATHAF", "Category of harbour facility"),
            new S57Attribute(31, "CATHLK", "Category of hulk"),
            new S57Attribute(32, "CATICE", "Category of ice"),
            new S57Attribute(33, "CATINB", "Category of installation buoy"),
            new S57Attribute(34, "CATLND", "Category of land region"),
            new S57Attribute(35, "CATLMK", "Category of landmark"),
            new S57Attribute(36, "CATLAM", "Category of lateral mark"),
            new S57Attribute(37, "CATLIT", "Category of light"),
            new S57Attribute(38, "CATMFA", "Category of marine farm/culture"),
            new S57Attribute(39, "CATMPA", "Category of military practice area"),
            new S57Attribute(40, "CATMOR", "Category of mooring/warping facility"),
            new S57Attribute(41, "CATNAV", "Category of navigation line"),
            new S57Attribute(42, "CATOBS", "Category of obstruction"),
            new S57Attribute(43, "CATOFP", "Category of offshore platform"),
            new S57Attribute(44, "CATOLB", "Category of oil barrier"),
            new S57Attribute(45, "CATPLE", "Category of pile"),
            new S57Attribute(46, "CATPIL", "Category of pilot boarding place"),
            new S57Attribute(47, "CATPIP", "Category of pipeline/pipe"),
            new S57Attribute(48, "CATPRA", "Category of production area"),
            new S57Attribute(49, "CATPYL", "Category of pylon"),
            //new S57Attribute(50, "CATQUA", "Category of quality of data"),  //use in ENC prohibited
            new S57Attribute(51, "CATRAS", "Category of radar station"),
            new S57Attribute(52, "CATRTB", "Category of radar transponder beacon"),
            new S57Attribute(53, "CATROS", "Category of radio station"),
            new S57Attribute(54, "CATTRK", "Category of recommended track"),
            new S57Attribute(55, "CATRSC", "Category of rescue station"),
            new S57Attribute(56, "CATREA", "Category of restricted area"),
            new S57Attribute(57, "CATROD", "Category of road"),
            new S57Attribute(58, "CATRUN", "Category of runway"),
            new S57Attribute(59, "CATSEA", "Category of sea area"),
            new S57Attribute(60, "CATSLC", "Category of shoreline construction"),
            new S57Attribute(61, "CATSIT", "Category of signal station, traffic"),
            new S57Attribute(62, "CATSIW", "Category of signal station, warning"),
            new S57Attribute(63, "CATSIL", "Category of silo/tank"),
            new S57Attribute(64, "CATSLO", "Category of slope"),
            new S57Attribute(65, "CATSCF", "Category of small craft facility"),
            new S57Attribute(66, "CATSPM", "Category of special purpose mark"),
            new S57Attribute(67, "CATTSS", "Category of Traffic Separation Scheme"),
            new S57Attribute(68, "CATVEG", "Category of vegetation"),
            new S57Attribute(69, "CATWAT", "Category of water turbulence"),
            new S57Attribute(70, "CATWED", "Category of weed/kelp"),
            new S57Attribute(71, "CATWRK", "Category of wreck"),
            new S57Attribute(72, "CATZOC", "Category of zone of confidence in data"),
            //new S57Attribute(73, "$SPACE", "Character spacing"),  //use in ENC prohibited
            //new S57Attribute(74, "$CHARS", "Character specification"),  //use in ENC prohibited
            new S57Attribute(75, "COLOUR", "Colour"),
            new S57Attribute(76, "COLPAT", "Colour pattern"),
            new S57Attribute(77, "COMCHA", "Communication channel"),
            //new S57Attribute(78, "$CSIZE", "Compass size"),  //use in ENC prohibited
            //new S57Attribute(79, "CPDATE", "Compilation date"),  //use in ENC prohibited
            new S57Attribute(80, "CSCALE", "Compilation scale"),
            new S57Attribute(81, "CONDTN", "Condition"),
            new S57Attribute(82, "CONRAD", "Conspicuous, radar"),
            new S57Attribute(83, "CONVIS", "Conspicuous, visually"),
            new S57Attribute(84, "CURVEL", "Current velocity"),
            new S57Attribute(85, "DATEND", "Date end"),
            new S57Attribute(86, "DATSTA", "Date start"),
            new S57Attribute(87, "DRVAL1", "Depth range value 1"),
            new S57Attribute(88, "DRVAL2", "Depth range value 2"),
            //new S57Attribute(89, "DUNITS", "Depth units"),  //use in ENC prohibited
            new S57Attribute(90, "ELEVAT", "Elevation"),
            new S57Attribute(91, "ESTRNG", "Estimated range of transmission"),
            new S57Attribute(92, "EXCLIT", "Exhibition condition of light"),
            new S57Attribute(93, "EXPSOU", "Exposition of sounding"),
            new S57Attribute(94, "FUNCTN", "Function"),
            new S57Attribute(95, "HEIGHT", "Height"),
            new S57Attribute(96, "HUNITS", "Height/length units"),
            new S57Attribute(97, "HORACC", "Horizontal accuracy"),
            new S57Attribute(98, "HORCLR", "Horizontal clearance"),
            new S57Attribute(99, "HORLEN", "Horizontal length"),
            new S57Attribute(100, "HORWID", "Horizontal width"),
            new S57Attribute(101, "ICEFAC", "Ice factor"),
            new S57Attribute(102, "INFORM", "Information"),
            new S57Attribute(103, "JRSDTN", "Jurisdiction"),
            //new S57Attribute(104, "$JUSTH", "Justification - horizontal"),  //use in ENC prohibited
            //new S57Attribute(105, "$JUSTV", "Justification - vertical"),  //use in ENC prohibited
            new S57Attribute(106, "LIFCAP", "Lifting capacity"),
            new S57Attribute(107, "LITCHR", "Light characteristic"),
            new S57Attribute(108, "LITVIS", "Light visibility"),
            new S57Attribute(109, "MARSYS", "Marks navigational - System of"),
            new S57Attribute(110, "MLTYLT", "Multiplicity of lights"),
            new S57Attribute(111, "NATION", "Nationality"),
            new S57Attribute(112, "NATCON", "Nature of construction"),
            new S57Attribute(113, "NATSUR", "Nature of surface"),
            new S57Attribute(114, "NATQUA", "Nature of surface - qualifying terms"),
            //new S57Attribute(115, "NMDATE", "Notice to Mariners date"),  //use in ENC prohibited
            new S57Attribute(116, "OBJNAM", "Object name"),
            new S57Attribute(117, "ORIENT", "Orientation"),
            new S57Attribute(118, "PEREND", "Periodic date end"),
            new S57Attribute(119, "PERSTA", "Periodic date start"),
            new S57Attribute(120, "PICREP", "Pictorial representation"),
            new S57Attribute(121, "PILDST", "Pilot district"),
            //new S57Attribute(122, "PRCTRY", "Producing country"),  //use in ENC prohibited
            new S57Attribute(123, "PRODCT", "Product"),
            new S57Attribute(124, "PUBREF", "Publication reference"),
            new S57Attribute(125, "QUASOU", "Quality of sounding measurement"),
            new S57Attribute(126, "RADWAL", "Radar wave length"),
            new S57Attribute(127, "RADIUS", "Radius"),
            //new S57Attribute(128, "RECDAT", "Recording date"),  //use in ENC prohibited
            //new S57Attribute(129, "RECIND", "Recording indication"),  //use in ENC prohibited
            new S57Attribute(130, "RYRMGV", "Reference year for magnetic variation"),
            new S57Attribute(131, "RESTRN", "Restriction"),
            //new S57Attribute(132, "SCAMAX", "Scale maximum"),  //use in ENC prohibited
            new S57Attribute(133, "SCAMIN", "Scale minimum"),
            new S57Attribute(134, "SCVAL1", "Scale value one"),
            new S57Attribute(135, "SCVAL2", "Scale value two"),
            new S57Attribute(136, "SECTR1", "Sector limit one"),
            new S57Attribute(137, "SECTR2", "Sector limit two"),
            new S57Attribute(138, "SHIPAM", "Shift parameters"),
            new S57Attribute(139, "SIGFRQ", "Signal frequency"),
            new S57Attribute(140, "SIGGEN", "Signal generation"),
            new S57Attribute(141, "SIGGRP", "Signal group"),
            new S57Attribute(142, "SIGPER", "Signal period"),
            new S57Attribute(143, "SIGSEQ", "Signal sequence"),
            new S57Attribute(144, "SOUACC", "Sounding accuracy"),
            new S57Attribute(145, "SDISMX", "Sounding distance - maximum"),
            new S57Attribute(146, "SDISMN", "Sounding distance - minimum"),
            new S57Attribute(147, "SORDAT", "Source date"),
            new S57Attribute(148, "SORIND", "Source indication"),
            new S57Attribute(149, "STATUS", "Status"),
            new S57Attribute(150, "SURATH", "Survey authority"),
            new S57Attribute(151, "SUREND", "Survey date - end"),
            new S57Attribute(152, "SURSTA", "Survey date - start"),
            new S57Attribute(153, "SURTYP", "Survey type"),
            //new S57Attribute(154, "$SCALE", "Symbol scaling factor"),  //use in ENC prohibited
            //new S57Attribute(155, "$SCODE", "Symbolization code"),  //use in ENC prohibited
            new S57Attribute(156, "TECSOU", "Technique of sounding measurement"),
            //new S57Attribute(157, "$TXSTR", "Text string"),  //use in ENC prohibited
            new S57Attribute(158, "TXTDSC", "Textual description"),
            new S57Attribute(159, "TS_TSP", "Tidal stream - panel values"),
            new S57Attribute(160, "TS_TSV", "Tidal stream, current - time series values"),
            new S57Attribute(161, "T_ACWL", "Tide - accuracy of water level"),
            new S57Attribute(162, "T_HWLW", "Tide - high and low water values"),
            new S57Attribute(163, "T_MTOD", "Tide - method of tidal prediction"),
            new S57Attribute(164, "T_THDF", "Tide - time and height differences"),
            new S57Attribute(165, "T_TINT", "Tide, current - time interval of values"),
            new S57Attribute(166, "T_TSVL", "Tide - time series values"),
            new S57Attribute(167, "T_VAHC", "Tide - value of harmonic constituents"),
            new S57Attribute(168, "TIMEND", "Time end"),
            new S57Attribute(169, "TIMSTA", "Time start"),
            //new S57Attribute(170, "$TINTS", "Tint"),  //use in ENC prohibited
            new S57Attribute(171, "TOPSHP", "Topmark/daymark shape"),
            new S57Attribute(172, "TRAFIC", "Traffic flow"),
            new S57Attribute(173, "VALACM", "Value of annual change in magnetic variation"),
            new S57Attribute(174, "VALDCO", "Value of depth contour"),
            new S57Attribute(175, "VALLMA", "Value of local magnetic anomaly"),
            new S57Attribute(176, "VALMAG", "Value of magnetic variation"),
            new S57Attribute(177, "VALMXR", "Value of maximum range"),
            new S57Attribute(178, "VALNMR", "Value of nominal range"),
            new S57Attribute(179, "VALSOU", "Value of sounding"),
            new S57Attribute(180, "VERACC", "Vertical accuracy"),
            new S57Attribute(181, "VERCLR", "Vertical clearance"),
            new S57Attribute(182, "VERCCL", "Vertical clearance, closed"),
            new S57Attribute(183, "VERCOP", "Vertical clearance, open"),
            new S57Attribute(184, "VERCSA", "Vertical clearance, safe"),
            new S57Attribute(185, "VERDAT", "Vertical datum"),
            new S57Attribute(186, "VERLEN", "Vertical length"),
            new S57Attribute(187, "WATLEV", "Water level effect"),
            new S57Attribute(188, "CAT_TS", "Category of Tidal Stream"),
            //new S57Attribute(189, "PUNITS", "Positional accuracy units"),  //use in ENC prohibited
            new S57Attribute(190, "CLSDEF", "Object class definition"),
            new S57Attribute(191, "CLSNAM", "Object class name"),
            new S57Attribute(192, "SYMINS", "Symbol instruction"),
            new S57Attribute(300, "NINFOM", "Information in national language"),
            new S57Attribute(301, "NOBJNM", "Object name in national language"),
            new S57Attribute(302, "NPLDST", "Pilot district in national language"),
            //new S57Attribute(303, "$NTXST", "Text string in national language"),  //use in ENC prohibited
            new S57Attribute(304, "NTXTDS", "Textual description in national language"),
            new S57Attribute(400, "HORDAT", "Horizontal datum"),
            new S57Attribute(401, "POSACC", "Positional Accuracy"),
            new S57Attribute(402, "QUAPOS", "Quality of position"),
            new S57Attribute(1001, "CATBUI", "Category of building, single"),
            new S57Attribute(1002, "CATDYK", "Category of dyke"),
            new S57Attribute(1003, "CATMST", "Category of mast"),
            new S57Attribute(1004, "CATPRI", "Category of production installation"),
            new S57Attribute(1005, "CATREB", "Category of religious building"),
            new S57Attribute(1006, "CATTOW", "Category of tower"),
            new S57Attribute(1007, "CATTRE", "Category of tree"),
            new S57Attribute(1008, "COLMAR", "Colour of navigational mark"),
            new S57Attribute(1009, "QUAVEM", "Quality of vertical measurement"),
            new S57Attribute(1010, "SUPLIT", "Supervision of light"),
            new S57Attribute(1011, "CATMNT", "Category of monument"),
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
