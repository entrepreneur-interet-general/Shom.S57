using System;

namespace S57
{
    public enum RecordName1
    {
        DataSetGeneralInformation = 10,
        DS = 10,
        DataSetGeographicReference = 20,
        DP = 20,
        DataSetHistory = 30,
        DH = 30,
        DataSetAccuracy = 40, // DA
        CatalogueDirectory = 50, // CD
        CatalogueCrossReference = 60, // CR
        DataDictionaryDefinition = 70, // ID
        DataDictionaryDomain  = 80, // IO
        DataDictionarySchema = 90, // IS
        Feature =100, // FE          
        Isolatednode = 110, // VI
        Connectednode = 120, // VC
        Edge = 130, // VE
        Face = 140 //VF
}

    public static class RecordName
    {
        public const uint DS = 10;
        public const uint DP = 20;
        public const uint FE = 100;
        public const uint VI = 110;
        public const uint VC = 120;
        public const uint VE = 130;

        public static string GetStringValue(uint recordName)
        {
            switch (recordName)
            {
                case DS:
                    return "DS";
                case DP:
                    return "DP";
                case FE:
                    return "FE";
                case VI:
                    return "VI";
                case VC:
                    return "VC";
                case VE:
                    return "VE";
                default:
                    throw new Exception("Unknown Record Name");

            }
        }
    }
}
