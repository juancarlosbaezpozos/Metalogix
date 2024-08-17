using System;

namespace Metalogix.SharePoint.Adapters
{
    public interface IUpdateWebOptions
    {
        bool ApplyMasterPage { get; set; }

        bool ApplyTheme { get; set; }

        bool CopyAccessRequestSettings { get; set; }

        bool CopyAssociatedGroupSettings { get; set; }

        bool CopyCoreMetaData { get; set; }

        bool CopyFeatures { get; set; }

        bool CopyNavigation { get; set; }

        bool MergeFeatures { get; set; }
    }
}