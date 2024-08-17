using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Adapters
{
    public static class TenantSettingManager
    {
        public static Dictionary<string, TenantSetting> TenantSettingCollection { get; set; }

        static TenantSettingManager()
        {
            TenantSettingManager.TenantSettingCollection =
                new Dictionary<string, TenantSetting>(StringComparer.OrdinalIgnoreCase);
        }

        public static TenantSetting GetTenantSetting(string adminURL)
        {
            if (TenantSettingManager.TenantSettingCollection != null &&
                TenantSettingManager.TenantSettingCollection.ContainsKey(adminURL))
            {
                return TenantSettingManager.TenantSettingCollection[adminURL];
            }

            TenantSetting tenantSetting = new TenantSetting(adminURL);
            TenantSettingManager.TenantSettingCollection.Add(adminURL, tenantSetting);
            return tenantSetting;
        }
    }
}