using System.Collections.Generic;

namespace S57
{
    public class ObjectClassInfo
    {
        public ObjectClassInfo(string name, string acronym, uint code)
        {
            Name = name;
            Acronym = acronym;
            Code = code;
        }

        public string Name { get; private set; }
        public string Acronym { get; private set; }
        public uint Code { get; private set; }
    }

    public static class ObjectClasses
    {
        public const uint M_COVR = 302;
        public const uint M_NSYS = 306;
        public const uint M_QUAL = 308;
        public const uint C_AGGR = 400;
        public const uint C_ASSO = 401;
        

        private static List<ObjectClassInfo>_objectClassInfoList = new List<ObjectClassInfo>()
                                                                       {
                                                                           new ObjectClassInfo("Coverage", "M_COVR", M_COVR),
                                                                           new ObjectClassInfo("Navigational system of marks", "M_NSYS", M_NSYS),
                                                                           new ObjectClassInfo("Quality of data", "M_QUAL", M_QUAL),
                                                                           new ObjectClassInfo("Association", "C_ASSO", C_ASSO),
                                                                           new ObjectClassInfo("Aggregation", "C_AGGR", C_AGGR)
                                                                       };


        /*
new ObjectClassInfo("Administration Area (Named)", "ADMARE	1	1.3
new ObjectClassInfo("Airport/airfield", "AIRARE	2	1.4
new ObjectClassInfo("Anchor", "ACHPNT	1.5
new ObjectClassInfo("Anchor berth", "ACHBRT	3	1.6
new ObjectClassInfo("Anchorage area", "ACHARE	4	1.7
new ObjectClassInfo("Beacon, cardinal", "BCNCAR	5	1.8
new ObjectClassInfo("Beacon, isolated danger", "BCNISD	6	1.9
new ObjectClassInfo("Beacon, lateral", "BCNLAT	7	1.10
new ObjectClassInfo("Beacon, safe water", "BCNSAW	8	1.11
new ObjectClassInfo("Beacon, special purpose/general", "BCNSPP	9	1.12
new ObjectClassInfo("Berth", "BERTHS	10	1.13
new ObjectClassInfo("Berthing facility", "BRTFAC	1.14
new ObjectClassInfo("Bridge", "BRIDGE	11	1.15
new ObjectClassInfo("Building, religious", "BUIREL	1.16
new ObjectClassInfo("Building, single", "BUISGL	12	1.17
new ObjectClassInfo("Built-up area", "BUAARE	13	1.18
new ObjectClassInfo("Buoy, cardinal", "BOYCAR	14	1.19
new ObjectClassInfo("Buoy, installation", "BOYINB	15	1.20
new ObjectClassInfo("Buoy, isolated danger", "BOYISD	16	1.21
new ObjectClassInfo("Buoy, lateral", "BOYLAT	17	1.22
new ObjectClassInfo("Buoy, safe water", "BOYSAW	18	1.23
new ObjectClassInfo("Buoy, special purpose/general", "BOYSPP	19	1.24
new ObjectClassInfo("Cable area", "CBLARE	20	1.25
new ObjectClassInfo("Cable, overhead", "CBLOHD	21	1.26
new ObjectClassInfo("Cable, submarine", "CBLSUB	22	1.27
new ObjectClassInfo("Cairn", "CAIRNS	1.28
new ObjectClassInfo("Canal        	CANALS	23	1.29
new ObjectClassInfo("Canal bank       	CANBNK	24	1.30
new ObjectClassInfo("Cargo transhipment area     	CTSARE	25	1.31
new ObjectClassInfo("Causeway          	CAUSWY	26	1.32
new ObjectClassInfo("Caution area      	CTNARE	27	1.33
new ObjectClassInfo("Cemetery          	CEMTRY	1.34
new ObjectClassInfo("Chain/Wire        	CHNWIR	1.35
new ObjectClassInfo("Checkpoint        	CHKPNT	28	1.36
new ObjectClassInfo("Chimney      	CHIMNY	1.37
new ObjectClassInfo("Coastguard station     	CGUSTA	29	1.38
new ObjectClassInfo("Coastline         	COALNE	30	1.39
new ObjectClassInfo("Contiguous zone        	CONZNE	31	1.40
new ObjectClassInfo("Continental shelf area      	COSARE	32	1.41
new ObjectClassInfo("Control point     	CTRPNT	33	1.42
new ObjectClassInfo("Conveyor          	CONVYR	34	1.43
new ObjectClassInfo("Crane        	CRANES	35	1.44
new ObjectClassInfo("Current - non-gravitational	CURENT	36	1.45
new ObjectClassInfo("Custom zone       	CUSZNE	37	1.46
new ObjectClassInfo("Dam          	DAMCON	38	1.47
new ObjectClassInfo("Daymark      	DAYMAR	39	1.48
new ObjectClassInfo("Deep water route centerline      	DWRTCL	40	1.49
new ObjectClassInfo("Deep water route part       	DWRTPT	41	1.50
new ObjectClassInfo("Depth area        	DEPARE	42	1.51
        */

        public static ObjectClassInfo GetObjectClassInfo(uint code)
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

        public static bool IsCartograpicObjectClass(uint code)
        {
            return (code >= 500 && code <= 504);
        }
    }
}
