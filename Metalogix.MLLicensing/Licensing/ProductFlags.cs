using System;

namespace Metalogix.Licensing
{
    [Flags]
    public enum ProductFlags
    {
        Unknown = 0,
        CMCSharePoint = 1,
        CMCFileShare = 2,
        CMCPublicFolder = 4,
        UnifiedContentMatrixExpressKey = 7,
        CMCWebsite = 8,
        CMCeRoom = 16,
        CMCOracleAndStellent = 32,
        CMCDocumentum = 64,
        CMCBlogsAndWikis = 128,
        CMCGoogle = 256,
        SRM = 512,
        CMWebComponents = 2048,
        UnifiedContentMatrixKey = 4095
    }
}