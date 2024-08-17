using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Metalogix.Licensing.Properties
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
        [DefaultSettingValue("http://localhost:8081/LicenseService.asmx")]
        [SpecialSetting(SpecialSetting.WebServiceUrl)]
        public string Metalogix_Licensing_LicenseServer_Service_LicenseService
        {
            get { return (string)this["Metalogix_Licensing_LicenseServer_Service_LicenseService"]; }
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