using System;

namespace Metalogix.SharePoint.Adapters
{
    public class FeatureGuids
    {
        public readonly static Guid SharePointDocumentId;

        static FeatureGuids()
        {
            FeatureGuids.SharePointDocumentId = new Guid("b50e3104-6812-424f-a011-cc90e6327318");
        }

        public FeatureGuids()
        {
        }
    }
}