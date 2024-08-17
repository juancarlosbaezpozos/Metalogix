using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Adapters.MLWS.Properties
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
        [DefaultSettingValue("http://localhost/_vti_bin/ml/VERSION/MLSPExtensions.asmx")]
        [SpecialSetting(SpecialSetting.WebServiceUrl)]
        public string Metalogix_SharePoint_Adapters_MLWS_MLSPExtensionsWebServiceRef_MLSPExtensionsWebService
        {
            get
            {
                return (string)this[
                    "Metalogix_SharePoint_Adapters_MLWS_MLSPExtensionsWebServiceRef_MLSPExtensionsWebService"];
            }
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