using System.Collections.Generic;

namespace S57
{
    //source: http://www.s-57.com/
    //retrieved on January 25, 2019
    public struct MyS57ObjComparer : IEqualityComparer<S57Obj>
    {
        public bool Equals(S57Obj x, S57Obj y)
        {
            return x == y;
        }

        public int GetHashCode(S57Obj obj)
        {
            return (int)obj;
        }
    }
    public static class S57ObjectInfo
    { 
        public static Dictionary<S57Obj, string> S57Dict = new Dictionary<S57Obj, string>(new MyS57ObjComparer())
        {
            {S57Obj.ADMARE, "Administration area (Named)"},
            {S57Obj.AIRARE,  "Airport / airfield"},
            {S57Obj.ACHBRT,  "Anchor berth"},
            {S57Obj.ACHARE,  "Anchorage area"},
            {S57Obj.BCNCAR,  "Beacon, cardinal"},
            {S57Obj.BCNISD,  "Beacon, isolated danger"},
            {S57Obj.BCNLAT,  "Beacon, lateral"},
            {S57Obj.BCNSAW,  "Beacon, safe water"},
            {S57Obj.BCNSPP, "Beacon, special purpose/general"},
            {S57Obj.BERTHS, "Berth"},
            {S57Obj.BRIDGE, "Bridge"},
            {S57Obj.BUISGL, "Building, single"},
            {S57Obj.BUAARE, "Built-up area"},
            {S57Obj.BOYCAR, "Buoy, cardinal"},
            {S57Obj.BOYINB, "Buoy, installation"},
            {S57Obj.BOYISD, "Buoy, isolated danger"},
            {S57Obj.BOYLAT, "Buoy, lateral"},
            {S57Obj.BOYSAW, "Buoy, safe water"},
            {S57Obj.BOYSPP, "Buoy, special purpose/general"},
            {S57Obj.CBLARE, "Cable area"},
            {S57Obj.CBLOHD, "Cable, overhead"},
            {S57Obj.CBLSUB, "Cable, submarine"},
            {S57Obj.CANALS, "Canal"},
            //{S57Obj.CANBNK, "Canal bank"},  //use in ENC prohibited
            {S57Obj.CTSARE, "Cargo transshipment area"},
            {S57Obj.CAUSWY, "Causeway"},
            {S57Obj.CTNARE, "Caution area"},
            {S57Obj.CHKPNT, "Checkpoint"},
            {S57Obj.CGUSTA, "Coastguard station"},
            {S57Obj.COALNE, "Coastline"},
            {S57Obj.CONZNE, "Contiguous zone"},
            {S57Obj.COSARE, "Continental shelf area"},
            {S57Obj.CTRPNT, "Control point"},
            {S57Obj.CONVYR, "Conveyor"},
            {S57Obj.CRANES, "Crane"},
            {S57Obj.CURENT, "Current - non - gravitational"},
            {S57Obj.CUSZNE, "Custom zone"},
            {S57Obj.DAMCON, "Dam"},
            {S57Obj.DAYMAR, "Daymark"},
            {S57Obj.DWRTCL, "Deep water route centerline"},
            {S57Obj.DWRTPT, "Deep water route part"},
            {S57Obj.DEPARE, "Depth area"},
            {S57Obj.DEPCNT, "Depth contour"},
            {S57Obj.DISMAR, "Distance mark"},
            {S57Obj.DOCARE, "Dock area"},
            {S57Obj.DRGARE, "Dredged area"},
            {S57Obj.DRYDOC, "Dry dock"},
            {S57Obj.DMPGRD, "Dumping ground"},
            {S57Obj.DYKCON, "Dyke"},
            {S57Obj.EXEZNE, "Exclusive Economic Zone"},
            {S57Obj.FAIRWY, "Fairway"},
            {S57Obj.FNCLNE, "Fence/wall"},
            {S57Obj.FERYRT, "Ferry route"},
            {S57Obj.FSHZNE, "Fishery zone"},
            {S57Obj.FSHFAC, "Fishing facility"},
            {S57Obj.FSHGRD, "Fishing ground"},
            {S57Obj.FLODOC, "Floating dock"},
            {S57Obj.FOGSIG, "Fog signal"},
            {S57Obj.FORSTC, "Fortified structure"},
            {S57Obj.FRPARE, "Free port area"},
            {S57Obj.GATCON, "Gate"},
            {S57Obj.GRIDRN, "Gridiron"},
            {S57Obj.HRBARE, "Harbour area (administrative)"},
            {S57Obj.HRBFAC, "Harbour facility"},
            {S57Obj.HULKES, "Hulk"},
            {S57Obj.ICEARE, "Ice area"},
            {S57Obj.ICNARE, "Incineration area"},
            {S57Obj.ISTZNE, "Inshore traffic zone"},
            {S57Obj.LAKARE, "Lake"},
            //{S57Obj.LAKSHR, "Lake shore"},  //use in ENC prohibited
            {S57Obj.LNDARE, "Land area"},
            {S57Obj.LNDELV, "Land elevation"},
            {S57Obj.LNDRGN, "Land region"},
            {S57Obj.LNDMRK, "Landmark"},
            {S57Obj.LIGHTS, "Light"},
            {S57Obj.LITFLT, "Light float"},
            {S57Obj.LITVES, "Light vessel"},
            {S57Obj.LOCMAG, "Local magnetic anomaly"},
            {S57Obj.LOKBSN, "Lock basin"},
            {S57Obj.LOGPON, "Log pond"},
            {S57Obj.MAGVAR, "Magnetic variation"},
            {S57Obj.MARCUL, "Marine farm/culture"},
            {S57Obj.MIPARE, "Military practice area"},
            {S57Obj.MORFAC, "Mooring/warping facility"},
            {S57Obj.NAVLNE, "Navigation line"},
            {S57Obj.OBSTRN, "Obstruction"},
            {S57Obj.OFSPLF, "Offshore platform"},
            {S57Obj.OSPARE, "Offshore production area"},
            {S57Obj.OILBAR, "Oil barrier"},
            {S57Obj.PILPNT, "Pile"},
            {S57Obj.PILBOP, "Pilot boarding place"},
            {S57Obj.PIPARE, "Pipeline area"},
            {S57Obj.PIPOHD, "Pipeline, overhead"},
            {S57Obj.PIPSOL, "Pipeline, submarine/on land"},
            {S57Obj.PONTON, "Pontoon"},
            {S57Obj.PRCARE, "Precautionary area"},
            {S57Obj.PRDARE, "Production / storage area"},
            {S57Obj.PYLONS, "Pylon/bridge support"},
            {S57Obj.RADLNE, "Radar line"},
            {S57Obj.RADRNG, "Radar range"},
            {S57Obj.RADRFL, "Radar reflector"},
            {S57Obj.RADSTA, "Radar station"},
            {S57Obj.RTPBCN, "Radar transponder beacon"},
            {S57Obj.RDOCAL, "Radio calling-in point"},
            {S57Obj.RDOSTA, "Radio station"},
            {S57Obj.RAILWY, "Railway"},
            {S57Obj.RAPIDS, "Rapids"},
            {S57Obj.RCRTCL, "Recommended route centerline"},
            {S57Obj.RECTRC, "Recommended track"},
            {S57Obj.RCTLPT, "Recommended Traffic Lane Part"},
            {S57Obj.RSCSTA, "Rescue station"},
            {S57Obj.RESARE, "Restricted area"},
            {S57Obj.RETRFL, "Retro-reflector"},
            {S57Obj.RIVERS, "River"},
            //{S57Obj.RIVBNK, "River bank"},  //use in ENC prohibited
            {S57Obj.ROADWY, "Road"},
            {S57Obj.RUNWAY, "Runway"},
            {S57Obj.SNDWAV, "Sand waves"},
            {S57Obj.SEAARE, "Sea area / named water area"},
            {S57Obj.SPLARE, "Sea-plane landing area"},
            {S57Obj.SBDARE, "Seabed area"},
            {S57Obj.SLCONS, "Shoreline Construction"},
            {S57Obj.SISTAT, "Signal station, traffic"},
            {S57Obj.SISTAW, "Signal station, warning"},
            {S57Obj.SILTNK, "Silo / tank"},
            {S57Obj.SLOTOP, "Slope topline"},
            {S57Obj.SLOGRD, "Sloping ground"},
            {S57Obj.SMCFAC, "Small craft facility"},
            {S57Obj.SOUNDG, "Sounding"},
            {S57Obj.SPRING, "Spring"},
            //{S57Obj.SQUARE, "Square"},  //use in ENC prohibited
            {S57Obj.STSLNE, "Straight territorial sea baseline"},
            {S57Obj.SUBTLN, "Submarine transit lane"},
            {S57Obj.SWPARE, "Swept Area"},
            {S57Obj.TESARE, "Territorial sea area"},
            {S57Obj.TS_FEB, "Tidal stream - flood/ebb"},
            {S57Obj.TS_PRH, "Tidal stream - harmonic prediction"},
            {S57Obj.TS_PNH, "Tidal stream - non-harmonic prediction"},
            {S57Obj.TS_PAD, "Tidal stream panel data"},
            {S57Obj.TS_TIS, "Tidal stream - time series"},
            {S57Obj.T_HMON, "Tide - harmonic prediction"},
            {S57Obj.T_NHMN, "Tide - non-harmonic prediction"},
            {S57Obj.T_TIMS, "Tidal stream - time series"},
            {S57Obj.TIDEWY, "Tideway"},
            {S57Obj.TOPMAR, "Top mark"},
            {S57Obj.TSELNE, "Traffic Separation Line"},
            {S57Obj.TSSBND, "Traffic Separation Scheme  Boundary"},
            {S57Obj.TSSCRS, "Traffic Separation Scheme Crossing"},
            {S57Obj.TSSLPT, "Traffic Separation Scheme  Lane part"},
            {S57Obj.TSSRON, "Traffic Separation Scheme  Roundabout"},
            {S57Obj.TSEZNE, "Traffic Separation Zone"},
            {S57Obj.TUNNEL, "Tunnel"},
            {S57Obj.TWRTPT, "Two-way route  part"},
            {S57Obj.UWTROC, "Underwater rock / awash rock"},
            {S57Obj.UNSARE, "Unsurveyed area"},
            {S57Obj.VEGATN, "Vegetation"},
            {S57Obj.WATTUR, "Water turbulence"},
            {S57Obj.WATFAL, "Waterfall"},
            {S57Obj.WEDKLP, "Weed/Kelp"},
            {S57Obj.WRECKS, "Wreck"},
            {S57Obj.ARCSLN, "Archipelagic Sea Lane"},
            {S57Obj.ASLXIS, "Archipelagic Sea Lane axis"},
            {S57Obj.NEWOBJ, "New object"},
            {S57Obj.M_ACCY, "Accuracy of data"},
            {S57Obj.M_CSCL, "Compilation scale of data"},
            {S57Obj.M_COVR, "Coverage"},
            //{S57Obj.M_HDAT, "Horizontal datum of data"},  //use in ENC prohibited
            {S57Obj.M_HOPA, "Horizontal datum shift parameters"},
            {S57Obj.M_NPUB, "Nautical publication information"},
            {S57Obj.M_NSYS, "Navigational system of marks"},
            //{S57Obj.M_PROD, "Production information"},  //use in ENC prohibited
            {S57Obj.M_QUAL, "Quality of data"},
            {S57Obj.M_SDAT, "Sounding datum"},
            {S57Obj.M_SREL, "Survey reliability"},
            //{S57Obj.M_UNIT, "Units of measurement of data"},  //use in ENC prohibited
            {S57Obj.M_VDAT, "Vertical datum of data"},
            {S57Obj.C_AGGR, "Aggregation"},
            {S57Obj.C_ASSO, "Association"},
            //{S57Obj.C_STAC, "Stacked on/stacked under"},  //use in ENC prohibited
            //{S57Obj.$AREAS, "Cartographic area"},  //use in ENC prohibited
            //{S57Obj.$LINES, "Cartographic line"},  //use in ENC prohibited
            //{S57Obj.$CSYMB, "Cartographic symbol"},  //use in ENC prohibited
            //{S57Obj.$COMPS, "Compass"},  //use in ENC prohibited
            //{S57Obj.$TEXTS, "Text"},  //use in ENC prohibited


        };             

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
