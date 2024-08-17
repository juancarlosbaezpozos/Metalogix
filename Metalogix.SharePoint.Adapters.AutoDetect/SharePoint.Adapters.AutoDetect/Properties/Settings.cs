using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Adapters.AutoDetect.Properties
{
    [CompilerGenerated]
    [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
    internal sealed class Settings : ApplicationSettingsBase
    {
        private static Settings defaultInstance;

        public static Settings Default
        {
            get { return Settings.defaultInstance; }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("http://d-saltspring-10/_vti_bin/SiteData.asmx")]
        [SpecialSetting(SpecialSetting.WebServiceUrl)]
        public string Metalogix_SharePoint_Adapters_AutoDetect_SiteData_SiteData
        {
            get { return (string)this["Metalogix_SharePoint_Adapters_AutoDetect_SiteData_SiteData"]; }
        }

        static Settings()
        {
            Settings.defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
        }

        public Settings()
        {
        }
    }
}