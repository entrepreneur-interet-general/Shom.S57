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
        public const uint M_COVR = 302;
        public const uint M_NSYS = 306;
        public const uint M_QUAL = 308;
        public const uint C_AGGR = 400;
        public const uint C_ASSO = 401;

        public const uint BCNCAR = 5;
        public const uint BCNISD = 6;
        public const uint BCNLAT = 7;
        public const uint BCNSAW = 8;
        public const uint BCNSPP = 9;
        public const uint BUAARE = 13;
        public const uint BOYCAR = 14;
        public const uint BOYINB = 15;
        public const uint BOYISD = 16;
        public const uint BOYLAT = 17;
        public const uint BOYSAW = 18;
        public const uint BOYSPP = 19;

        public const uint DAYMAR = 39;
        public const uint LNDMRK = 74;
        public const uint LIGHTS = 75;
        public const uint NAVLNE = 85;
        public const uint PYLONS = 98;
        public const uint RECTRC = 109;
        public const uint TOPMAR = 144;


        private static List<S57Object>_objectClassInfoList = new List<S57Object>()
        {
            new S57Object( M_COVR, "M_COVR", "Coverage" ),
            new S57Object( M_NSYS, "M_NSYS", "Navigational system of marks" ),
            new S57Object( M_QUAL, "M_QUAL", "Quality of data" ),

            new S57Object( C_ASSO, "C_ASSO", "Association"),
            new S57Object( C_AGGR, "C_AGGR", "Aggregation" ),

            new S57Object( BCNCAR, "BCNCAR", "Beacon, cardinal"  ), 
            new S57Object( BCNISD, "BCNISD", "Beacon, isolated danger"  ), 
            new S57Object( BCNLAT, "BCNLAT", "Beacon, lateral" ),
            new S57Object( BCNSAW, "BCNSAW", "Beacon, safe water" ),
            new S57Object( BCNSPP, "BCNSPP", "Beacon, special purpose/general" ),
            new S57Object( BOYCAR, "BOYCAR", "Buoy, cardinal" ),
            new S57Object( BOYINB, "BOYINB", "Buoy, installation" ),
            new S57Object( BOYISD, "BOYISD", "Buoy, isolated danger" ),
            new S57Object( BOYLAT, "BOYLAT", "Buoy, lateral" ),
            new S57Object( BOYSAW, "BOYSAW", "Buoy, safe water" ),
            new S57Object( BOYSPP, "BOYSPP", "Buoy, special purpose/general"  ),

            new S57Object( DAYMAR, "DAYMAR", "Daymark" ),
            new S57Object( TOPMAR, "TOPMAR", "Topmark" ),

            new S57Object( LNDMRK, "LNDMRK", "Landmark"),

            new S57Object( NAVLNE, "NAVLNE", "Navigation line"),
            new S57Object( RECTRC, "RECTRC", "Recommended track"),

            new S57Object( LIGHTS, "LIGHTS", "Lights" ),

            new S57Object( PYLONS, "PYLONS", "Pylon/bridge support" )

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


    /*
new ObjectClassInfo("Administration Area (Named)", "ADMARE	1	1.3
new ObjectClassInfo("Airport/airfield", "           AIRARE	2	1.4
new ObjectClassInfo("Anchor", "                     ACHPNT	    1.5
new ObjectClassInfo("Anchor berth", "               ACHBRT	3	1.6
new ObjectClassInfo("Anchorage area",               ACHARE	4	1.7
new ObjectClassInfo("Berth",                        BERTHS	10	1.13
new ObjectClassInfo("Berthing facility",            BRTFAC	    1.14
new ObjectClassInfo("Bridge", "                     BRIDGE	11	1.15
new ObjectClassInfo("Building, religious", "        BUIREL	    1.16
new ObjectClassInfo("Building, single", "           BUISGL	12	1.17
new ObjectClassInfo("Cable area", "                 CBLARE	20	1.25
new ObjectClassInfo("Cable, overhead",              CBLOHD	21	1.26
new ObjectClassInfo("Cable, submarine",             CBLSUB	22	1.27
new ObjectClassInfo("Cairn",                        CAIRNS	    1.28
new ObjectClassInfo("Canal        	                CANALS	23	1.29
new ObjectClassInfo("Canal bank       	            CANBNK	24	1.30
new ObjectClassInfo("Cargo transhipment area     	CTSARE	25	1.31
new ObjectClassInfo("Causeway          	            CAUSWY	26	1.32
new ObjectClassInfo("Caution area      	            CTNARE	27	1.33
new ObjectClassInfo("Cemetery          	            CEMTRY	    1.34
new ObjectClassInfo("Chain/Wire        	            CHNWIR	    1.35
new ObjectClassInfo("Checkpoint        	            CHKPNT	28	1.36
new ObjectClassInfo("Chimney      	                CHIMNY	    1.37
new ObjectClassInfo("Coastguard station     	    CGUSTA	29	1.38
new ObjectClassInfo("Coastline         	            COALNE	30	1.39
new ObjectClassInfo("Contiguous zone        	    CONZNE	31	1.40
new ObjectClassInfo("Continental shelf area      	COSARE	32	1.41
new ObjectClassInfo("Control point     	            CTRPNT	33	1.42
new ObjectClassInfo("Conveyor          	            CONVYR	34	1.43
new ObjectClassInfo("Crane        	                CRANES	35	1.44
new ObjectClassInfo("Current - non-gravitational	CURENT	36	1.45
new ObjectClassInfo("Custom zone       	            CUSZNE	37	1.46
new ObjectClassInfo("Dam          	                DAMCON	38	1.47
new ObjectClassInfo("Daymark      	                DAYMAR	39	1.48
new ObjectClassInfo("Deep water route centerline    DWRTCL	40	1.49
new ObjectClassInfo("Deep water route part       	DWRTPT	41	1.50
new ObjectClassInfo("Depth area        	            DEPARE	42	1.51
    */

}
