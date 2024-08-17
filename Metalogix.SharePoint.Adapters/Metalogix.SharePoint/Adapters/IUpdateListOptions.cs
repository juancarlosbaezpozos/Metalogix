using System;

namespace Metalogix.SharePoint.Adapters
{
    public interface IUpdateListOptions
    {
        bool CopyEnableAssignToEmail { get; set; }

        bool CopyFields { get; set; }

        bool CopyViews { get; set; }

        bool DeletePreExistingViews { get; set; }

        bool UpdateFieldTypes { get; set; }
    }
}