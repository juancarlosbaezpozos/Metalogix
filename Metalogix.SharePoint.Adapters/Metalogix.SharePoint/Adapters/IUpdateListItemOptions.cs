using System;

namespace Metalogix.SharePoint.Adapters
{
    public interface IUpdateListItemOptions
    {
        bool PreserveSharePointDocumentIDs { get; set; }

        bool ShallowCopyExternalizedData { get; set; }
    }
}