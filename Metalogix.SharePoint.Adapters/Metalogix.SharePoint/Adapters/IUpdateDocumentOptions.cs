using System;

namespace Metalogix.SharePoint.Adapters
{
    public interface IUpdateDocumentOptions
    {
        bool ShallowCopyExternalizedData { get; set; }
    }
}