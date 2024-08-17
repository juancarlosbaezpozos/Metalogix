using System;

namespace Metalogix.SharePoint.Adapters
{
    public enum SharePointAdapterCommands
    {
        Unknown,
        GetListByName,
        GetWebApplicationForExpert,
        GetInstalledLanguagePacksForExpert,
        GetSiteCollRecyclebinStatisticsForExpert,
        GetColumnDefaultSettings,
        SetColumnDefaultSettings,
        SetCurrentUserLanguage,
        GetCurrentUserLanguage,
        CopyLanguageResourcesForViews,
        GetSupportedWebCultures,
        GetFullTrustSolutionsForExpert
    }
}