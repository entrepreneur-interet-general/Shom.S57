using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace S57
{
    public class S57Object
    {
        public S57Object(uint code, string acronym, string name )
        {
            Name = name;
            Acronym = acronym;
            Code = code;
        }

        public string Name { get; private set; }
        public string Acronym { get; private set; }
        public uint Code { get; private set; }
    }

    public static class S57Objects
    {
        //source: http://www.s-57.com/
        //retrieved on January 25, 2019
        public const uint ADMARE = 1;
        public const uint AIRARE = 2;
        public const uint ACHBRT = 3;
        public const uint ACHARE = 4;
        public const uint BCNCAR = 5;
        public const uint BCNISD = 6;
        public const uint BCNLAT = 7;
        public const uint BCNSAW = 8;
        public const uint BCNSPP = 9;
        public const uint BERTHS = 10;
        public const uint BRIDGE = 11;
        public const uint BUISGL = 12;
        public const uint BUAARE = 13;
        public const uint BOYCAR = 14;
        public const uint BOYINB = 15;
        public const uint BOYISD = 16;
        public const uint BOYLAT = 17;
        public const uint BOYSAW = 18;
        public const uint BOYSPP = 19;
        public const uint CBLARE = 20;
        public const uint CBLOHD = 21;
        public const uint CBLSUB = 22;
        public const uint CANALS = 23;
        //public const uint CANBNK = 24;  //use in ENC prohibited
        public const uint CTSARE = 25;
        public const uint CAUSWY = 26;
        public const uint CTNARE = 27;
        public const uint CHKPNT = 28;
        public const uint CGUSTA = 29;
        public const uint COALNE = 30;
        public const uint CONZNE = 31;
        public const uint COSARE = 32;
        public const uint CTRPNT = 33;
        public const uint CONVYR = 34;
        public const uint CRANES = 35;
        public const uint CURENT = 36;
        public const uint CUSZNE = 37;
        public const uint DAMCON = 38;
        public const uint DAYMAR = 39;
        public const uint DWRTCL = 40;
        public const uint DWRTPT = 41;
        public const uint DEPARE = 42;
        public const uint DEPCNT = 43;
        public const uint DISMAR = 44;
        public const uint DOCARE = 45;
        public const uint DRGARE = 46;
        public const uint DRYDOC = 47;
        public const uint DMPGRD = 48;
        public const uint DYKCON = 49;
        public const uint EXEZNE = 50;
        public const uint FAIRWY = 51;
        public const uint FNCLNE = 52;
        public const uint FERYRT = 53;
        public const uint FSHZNE = 54;
        public const uint FSHFAC = 55;
        public const uint FSHGRD = 56;
        public const uint FLODOC = 57;
        public const uint FOGSIG = 58;
        public const uint FORSTC = 59;
        public const uint FRPARE = 60;
        public const uint GATCON = 61;
        public const uint GRIDRN = 62;
        public const uint HRBARE = 63;
        public const uint HRBFAC = 64;
        public const uint HULKES = 65;
        public const uint ICEARE = 66;
        public const uint ICNARE = 67;
        public const uint ISTZNE = 68;
        public const uint LAKARE = 69;
        //public const uint LAKSHR = 70;  //use in ENC prohibited
        public const uint LNDARE = 71;
        public const uint LNDELV = 72;
        public const uint LNDRGN = 73;
        public const uint LNDMRK = 74;
        public const uint LIGHTS = 75;
        public const uint LITFLT = 76;
        public const uint LITVES = 77;
        public const uint LOCMAG = 78;
        public const uint LOKBSN = 79;
        public const uint LOGPON = 80;
        public const uint MAGVAR = 81;
        public const uint MARCUL = 82;
        public const uint MIPARE = 83;
        public const uint MORFAC = 84;
        public const uint NAVLNE = 85;
        public const uint OBSTRN = 86;
        public const uint OFSPLF = 87;
        public const uint OSPARE = 88;
        public const uint OILBAR = 89;
        public const uint PILPNT = 90;
        public const uint PILBOP = 91;
        public const uint PIPARE = 92;
        public const uint PIPOHD = 93;
        public const uint PIPSOL = 94;
        public const uint PONTON = 95;
        public const uint PRCARE = 96;
        public const uint PRDARE = 97;
        public const uint PYLONS = 98;
        public const uint RADLNE = 99;
        public const uint RADRNG = 100;
        public const uint RADRFL = 101;
        public const uint RADSTA = 102;
        public const uint RTPBCN = 103;
        public const uint RDOCAL = 104;
        public const uint RDOSTA = 105;
        public const uint RAILWY = 106;
        public const uint RAPIDS = 107;
        public const uint RCRTCL = 108;
        public const uint RECTRC = 109;
        public const uint RCTLPT = 110;
        public const uint RSCSTA = 111;
        public const uint RESARE = 112;
        public const uint RETRFL = 113;
        public const uint RIVERS = 114;
        //public const uint RIVBNK = 115;  //use in ENC prohibited
        public const uint ROADWY = 116;
        public const uint RUNWAY = 117;
        public const uint SNDWAV = 118;
        public const uint SEAARE = 119;
        public const uint SPLARE = 120;
        public const uint SBDARE = 121;
        public const uint SLCONS = 122;
        public const uint SISTAT = 123;
        public const uint SISTAW = 124;
        public const uint SILTNK = 125;
        public const uint SLOTOP = 126;
        public const uint SLOGRD = 127;
        public const uint SMCFAC = 128;
        public const uint SOUNDG = 129;
        public const uint SPRING = 130;
        //public const uint SQUARE = 131;  //use in ENC prohibited
        public const uint STSLNE = 132;
        public const uint SUBTLN = 133;
        public const uint SWPARE = 134;
        public const uint TESARE = 135;
        public const uint TS_FEB = 160;
        public const uint TS_PRH = 136;
        public const uint TS_PNH = 137;
        public const uint TS_PAD = 138;
        public const uint TS_TIS = 139;
        public const uint T_HMON = 140;
        public const uint T_NHMN = 141;
        public const uint T_TIMS = 142;
        public const uint TIDEWY = 143;
        public const uint TOPMAR = 144;
        public const uint TSELNE = 145;
        public const uint TSSBND = 146;
        public const uint TSSCRS = 147;
        public const uint TSSLPT = 148;
        public const uint TSSRON = 149;
        public const uint TSEZNE = 150;
        public const uint TUNNEL = 151;
        public const uint TWRTPT = 152;
        public const uint UWTROC = 153;
        public const uint UNSARE = 154;
        public const uint VEGATN = 155;
        public const uint WATTUR = 156;
        public const uint WATFAL = 157;
        public const uint WEDKLP = 158;
        public const uint WRECKS = 159;
        public const uint ARCSLN = 161;
        public const uint ASLXIS = 162;
        public const uint NEWOBJ = 163;
        public const uint M_ACCY = 300;
        public const uint M_CSCL = 301;
        public const uint M_COVR = 302;
        //public const uint M_HDAT = 303;  //use in ENC prohibited
        public const uint M_HOPA = 304;
        public const uint M_NPUB = 305;
        public const uint M_NSYS = 306;
        //public const uint M_PROD = 307;  //use in ENC prohibited
        public const uint M_QUAL = 308;
        public const uint M_SDAT = 309;
        public const uint M_SREL = 310;
        //public const uint M_UNIT = 311;  //use in ENC prohibited
        public const uint M_VDAT = 312;
        public const uint C_AGGR = 400;
        public const uint C_ASSO = 401;
        //public const uint C_STAC = 402;  //use in ENC prohibited
        //public const uint $AREAS = 500;  //use in ENC prohibited
        //public const uint $LINES = 501;  //use in ENC prohibited
        //public const uint $CSYMB = 502;  //use in ENC prohibited
        //public const uint $COMPS = 503;  //use in ENC prohibited
        //public const uint $TEXTS = 504;  //use in ENC prohibited



        private static List<S57Object>_objectClassInfoList = new List<S57Object>()
        {
            new S57Object(ADMARE, "ADMARE", "Administration area (Named)"),
            new S57Object(AIRARE, "AIRARE", "Airport / airfield"),
            new S57Object(ACHBRT, "ACHBRT", "Anchor berth"),
            new S57Object(ACHARE, "ACHARE", "Anchorage area"),
            new S57Object(BCNCAR, "BCNCAR", "Beacon, cardinal"),
            new S57Object(BCNISD, "BCNISD", "Beacon, isolated danger"),
            new S57Object(BCNLAT, "BCNLAT", "Beacon, lateral"),
            new S57Object(BCNSAW, "BCNSAW", "Beacon, safe water"),
            new S57Object(BCNSPP, "BCNSPP", "Beacon, special purpose/general"),
            new S57Object(BERTHS, "BERTHS", "Berth"),
            new S57Object(BRIDGE, "BRIDGE", "Bridge"),
            new S57Object(BUISGL, "BUISGL", "Building, single"),
            new S57Object(BUAARE, "BUAARE", "Built-up area"),
            new S57Object(BOYCAR, "BOYCAR", "Buoy, cardinal"),
            new S57Object(BOYINB, "BOYINB", "Buoy, installation"),
            new S57Object(BOYISD, "BOYISD", "Buoy, isolated danger"),
            new S57Object(BOYLAT, "BOYLAT", "Buoy, lateral"),
            new S57Object(BOYSAW, "BOYSAW", "Buoy, safe water"),
            new S57Object(BOYSPP, "BOYSPP", "Buoy, special purpose/general"),
            new S57Object(CBLARE, "CBLARE", "Cable area"),
            new S57Object(CBLOHD, "CBLOHD", "Cable, overhead"),
            new S57Object(CBLSUB, "CBLSUB", "Cable, submarine"),
            new S57Object(CANALS, "CANALS", "Canal"),
            //new S57Object(CANBNK, "CANBNK", "Canal bank"),  //use in ENC prohibited
            new S57Object(CTSARE, "CTSARE", "Cargo transshipment area"),
            new S57Object(CAUSWY, "CAUSWY", "Causeway"),
            new S57Object(CTNARE, "CTNARE", "Caution area"),
            new S57Object(CHKPNT, "CHKPNT", "Checkpoint"),
            new S57Object(CGUSTA, "CGUSTA", "Coastguard station"),
            new S57Object(COALNE, "COALNE", "Coastline"),
            new S57Object(CONZNE, "CONZNE", "Contiguous zone"),
            new S57Object(COSARE, "COSARE", "Continental shelf area"),
            new S57Object(CTRPNT, "CTRPNT", "Control point"),
            new S57Object(CONVYR, "CONVYR", "Conveyor"),
            new S57Object(CRANES, "CRANES", "Crane"),
            new S57Object(CURENT, "CURENT", "Current - non - gravitational"),
            new S57Object(CUSZNE, "CUSZNE", "Custom zone"),
            new S57Object(DAMCON, "DAMCON", "Dam"),
            new S57Object(DAYMAR, "DAYMAR", "Daymark"),
            new S57Object(DWRTCL, "DWRTCL", "Deep water route centerline"),
            new S57Object(DWRTPT, "DWRTPT", "Deep water route part"),
            new S57Object(DEPARE, "DEPARE", "Depth area"),
            new S57Object(DEPCNT, "DEPCNT", "Depth contour"),
            new S57Object(DISMAR, "DISMAR", "Distance mark"),
            new S57Object(DOCARE, "DOCARE", "Dock area"),
            new S57Object(DRGARE, "DRGARE", "Dredged area"),
            new S57Object(DRYDOC, "DRYDOC", "Dry dock"),
            new S57Object(DMPGRD, "DMPGRD", "Dumping ground"),
            new S57Object(DYKCON, "DYKCON", "Dyke"),
            new S57Object(EXEZNE, "EXEZNE", "Exclusive Economic Zone"),
            new S57Object(FAIRWY, "FAIRWY", "Fairway"),
            new S57Object(FNCLNE, "FNCLNE", "Fence/wall"),
            new S57Object(FERYRT, "FERYRT", "Ferry route"),
            new S57Object(FSHZNE, "FSHZNE", "Fishery zone"),
            new S57Object(FSHFAC, "FSHFAC", "Fishing facility"),
            new S57Object(FSHGRD, "FSHGRD", "Fishing ground"),
            new S57Object(FLODOC, "FLODOC", "Floating dock"),
            new S57Object(FOGSIG, "FOGSIG", "Fog signal"),
            new S57Object(FORSTC, "FORSTC", "Fortified structure"),
            new S57Object(FRPARE, "FRPARE", "Free port area"),
            new S57Object(GATCON, "GATCON", "Gate"),
            new S57Object(GRIDRN, "GRIDRN", "Gridiron"),
            new S57Object(HRBARE, "HRBARE", "Harbour area (administrative)"),
            new S57Object(HRBFAC, "HRBFAC", "Harbour facility"),
            new S57Object(HULKES, "HULKES", "Hulk"),
            new S57Object(ICEARE, "ICEARE", "Ice area"),
            new S57Object(ICNARE, "ICNARE", "Incineration area"),
            new S57Object(ISTZNE, "ISTZNE", "Inshore traffic zone"),
            new S57Object(LAKARE, "LAKARE", "Lake"),
            //new S57Object(LAKSHR, "LAKSHR", "Lake shore"),  //use in ENC prohibited
            new S57Object(LNDARE, "LNDARE", "Land area"),
            new S57Object(LNDELV, "LNDELV", "Land elevation"),
            new S57Object(LNDRGN, "LNDRGN", "Land region"),
            new S57Object(LNDMRK, "LNDMRK", "Landmark"),
            new S57Object(LIGHTS, "LIGHTS", "Light"),
            new S57Object(LITFLT, "LITFLT", "Light float"),
            new S57Object(LITVES, "LITVES", "Light vessel"),
            new S57Object(LOCMAG, "LOCMAG", "Local magnetic anomaly"),
            new S57Object(LOKBSN, "LOKBSN", "Lock basin"),
            new S57Object(LOGPON, "LOGPON", "Log pond"),
            new S57Object(MAGVAR, "MAGVAR", "Magnetic variation"),
            new S57Object(MARCUL, "MARCUL", "Marine farm/culture"),
            new S57Object(MIPARE, "MIPARE", "Military practice area"),
            new S57Object(MORFAC, "MORFAC", "Mooring/warping facility"),
            new S57Object(NAVLNE, "NAVLNE", "Navigation line"),
            new S57Object(OBSTRN, "OBSTRN", "Obstruction"),
            new S57Object(OFSPLF, "OFSPLF", "Offshore platform"),
            new S57Object(OSPARE, "OSPARE", "Offshore production area"),
            new S57Object(OILBAR, "OILBAR", "Oil barrier"),
            new S57Object(PILPNT, "PILPNT", "Pile"),
            new S57Object(PILBOP, "PILBOP", "Pilot boarding place"),
            new S57Object(PIPARE, "PIPARE", "Pipeline area"),
            new S57Object(PIPOHD, "PIPOHD", "Pipeline, overhead"),
            new S57Object(PIPSOL, "PIPSOL", "Pipeline, submarine/on land"),
            new S57Object(PONTON, "PONTON", "Pontoon"),
            new S57Object(PRCARE, "PRCARE", "Precautionary area"),
            new S57Object(PRDARE, "PRDARE", "Production / storage area"),
            new S57Object(PYLONS, "PYLONS", "Pylon/bridge support"),
            new S57Object(RADLNE, "RADLNE", "Radar line"),
            new S57Object(RADRNG, "RADRNG", "Radar range"),
            new S57Object(RADRFL, "RADRFL", "Radar reflector"),
            new S57Object(RADSTA, "RADSTA", "Radar station"),
            new S57Object(RTPBCN, "RTPBCN", "Radar transponder beacon"),
            new S57Object(RDOCAL, "RDOCAL", "Radio calling-in point"),
            new S57Object(RDOSTA, "RDOSTA", "Radio station"),
            new S57Object(RAILWY, "RAILWY", "Railway"),
            new S57Object(RAPIDS, "RAPIDS", "Rapids"),
            new S57Object(RCRTCL, "RCRTCL", "Recommended route centerline"),
            new S57Object(RECTRC, "RECTRC", "Recommended track"),
            new S57Object(RCTLPT, "RCTLPT", "Recommended Traffic Lane Part"),
            new S57Object(RSCSTA, "RSCSTA", "Rescue station"),
            new S57Object(RESARE, "RESARE", "Restricted area"),
            new S57Object(RETRFL, "RETRFL", "Retro-reflector"),
            new S57Object(RIVERS, "RIVERS", "River"),
            //new S57Object(RIVBNK, "RIVBNK", "River bank"),  //use in ENC prohibited
            new S57Object(ROADWY, "ROADWY", "Road"),
            new S57Object(RUNWAY, "RUNWAY", "Runway"),
            new S57Object(SNDWAV, "SNDWAV", "Sand waves"),
            new S57Object(SEAARE, "SEAARE", "Sea area / named water area"),
            new S57Object(SPLARE, "SPLARE", "Sea-plane landing area"),
            new S57Object(SBDARE, "SBDARE", "Seabed area"),
            new S57Object(SLCONS, "SLCONS", "Shoreline Construction"),
            new S57Object(SISTAT, "SISTAT", "Signal station, traffic"),
            new S57Object(SISTAW, "SISTAW", "Signal station, warning"),
            new S57Object(SILTNK, "SILTNK", "Silo / tank"),
            new S57Object(SLOTOP, "SLOTOP", "Slope topline"),
            new S57Object(SLOGRD, "SLOGRD", "Sloping ground"),
            new S57Object(SMCFAC, "SMCFAC", "Small craft facility"),
            new S57Object(SOUNDG, "SOUNDG", "Sounding"),
            new S57Object(SPRING, "SPRING", "Spring"),
            //new S57Object(SQUARE, "SQUARE", "Square"),  //use in ENC prohibited
            new S57Object(STSLNE, "STSLNE", "Straight territorial sea baseline"),
            new S57Object(SUBTLN, "SUBTLN", "Submarine transit lane"),
            new S57Object(SWPARE, "SWPARE", "Swept Area"),
            new S57Object(TESARE, "TESARE", "Territorial sea area"),
            new S57Object(TS_FEB, "TS_FEB", "Tidal stream - flood/ebb"),
            new S57Object(TS_PRH, "TS_PRH", "Tidal stream - harmonic prediction"),
            new S57Object(TS_PNH, "TS_PNH", "Tidal stream - non-harmonic prediction"),
            new S57Object(TS_PAD, "TS_PAD", "Tidal stream panel data"),
            new S57Object(TS_TIS, "TS_TIS", "Tidal stream - time series"),
            new S57Object(T_HMON, "T_HMON", "Tide - harmonic prediction"),
            new S57Object(T_NHMN, "T_NHMN", "Tide - non-harmonic prediction"),
            new S57Object(T_TIMS, "T_TIMS", "Tidal stream - time series"),
            new S57Object(TIDEWY, "TIDEWY", "Tideway"),
            new S57Object(TOPMAR, "TOPMAR", "Top mark"),
            new S57Object(TSELNE, "TSELNE", "Traffic Separation Line"),
            new S57Object(TSSBND, "TSSBND", "Traffic Separation Scheme  Boundary"),
            new S57Object(TSSCRS, "TSSCRS", "Traffic Separation Scheme Crossing"),
            new S57Object(TSSLPT, "TSSLPT", "Traffic Separation Scheme  Lane part"),
            new S57Object(TSSRON, "TSSRON", "Traffic Separation Scheme  Roundabout"),
            new S57Object(TSEZNE, "TSEZNE", "Traffic Separation Zone"),
            new S57Object(TUNNEL, "TUNNEL", "Tunnel"),
            new S57Object(TWRTPT, "TWRTPT", "Two-way route  part"),
            new S57Object(UWTROC, "UWTROC", "Underwater rock / awash rock"),
            new S57Object(UNSARE, "UNSARE", "Unsurveyed area"),
            new S57Object(VEGATN, "VEGATN", "Vegetation"),
            new S57Object(WATTUR, "WATTUR", "Water turbulence"),
            new S57Object(WATFAL, "WATFAL", "Waterfall"),
            new S57Object(WEDKLP, "WEDKLP", "Weed/Kelp"),
            new S57Object(WRECKS, "WRECKS", "Wreck"),
            new S57Object(ARCSLN, "ARCSLN", "Archipelagic Sea Lane"),
            new S57Object(ASLXIS, "ASLXIS", "Archipelagic Sea Lane axis"),
            new S57Object(NEWOBJ, "NEWOBJ", "New object"),
            new S57Object(M_ACCY, "M_ACCY", "Accuracy of data"),
            new S57Object(M_CSCL, "M_CSCL", "Compilation scale of data"),
            new S57Object(M_COVR, "M_COVR", "Coverage"),
            //new S57Object(M_HDAT, "M_HDAT", "Horizontal datum of data"),  //use in ENC prohibited
            new S57Object(M_HOPA, "M_HOPA", "Horizontal datum shift parameters"),
            new S57Object(M_NPUB, "M_NPUB", "Nautical publication information"),
            new S57Object(M_NSYS, "M_NSYS", "Navigational system of marks"),
            //new S57Object(M_PROD, "M_PROD", "Production information"),  //use in ENC prohibited
            new S57Object(M_QUAL, "M_QUAL", "Quality of data"),
            new S57Object(M_SDAT, "M_SDAT", "Sounding datum"),
            new S57Object(M_SREL, "M_SREL", "Survey reliability"),
            //new S57Object(M_UNIT, "M_UNIT", "Units of measurement of data"),  //use in ENC prohibited
            new S57Object(M_VDAT, "M_VDAT", "Vertical datum of data"),
            new S57Object(C_AGGR, "C_AGGR", "Aggregation"),
            new S57Object(C_ASSO, "C_ASSO", "Association"),
            //new S57Object(C_STAC, "C_STAC", "Stacked on/stacked under"),  //use in ENC prohibited
            //new S57Object($AREAS, "$AREAS", "Cartographic area"),  //use in ENC prohibited
            //new S57Object($LINES, "$LINES", "Cartographic line"),  //use in ENC prohibited
            //new S57Object($CSYMB, "$CSYMB", "Cartographic symbol"),  //use in ENC prohibited
            //new S57Object($COMPS, "$COMPS", "Compass"),  //use in ENC prohibited
            //new S57Object($TEXTS, "$TEXTS", "Text"),  //use in ENC prohibited


        };

        public static bool IsIn(uint code)
        {
            foreach (var objectClassInfo in _objectClassInfoList)
            {
                if (code == objectClassInfo.Code)
                {
                    return true;
                }
            }
            return false;
        }

        public static S57Object Get(uint code)
        {
            foreach (var objectClassInfo in _objectClassInfoList)
            {
                if (objectClassInfo.Code == code)
                {
                    return objectClassInfo;
                }
            }
            return null;
        }

        public static S57Object Get(string acronym)
        {
            foreach (var objectClassInfo in _objectClassInfoList)
            {
                if (objectClassInfo.Acronym == acronym )
                {
                    return objectClassInfo;
                }
            }
            return null;
        }

        public static bool IsGeoObjectClass(uint code)
        {
            return (code >= 1 && code <= 159);
        }

        public static bool IsMetaObjectClass(uint code)
        {
            return (code >= 300 && code <= 312);
        }

        public static bool IsCollectionObjectClass(uint code)
        {
            return (code >= 400 && code <= 402);
        }

        public static bool IsCartographicObjectClass(uint code)
        {
            return (code >= 500 && code <= 504);
        }
    }
}
