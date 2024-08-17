using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Properties
{
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
	internal sealed class Settings : ApplicationSettingsBase
	{
		private static Metalogix.SharePoint.Properties.Settings defaultInstance;

		public static Metalogix.SharePoint.Properties.Settings Default
		{
			get
			{
				return Metalogix.SharePoint.Properties.Settings.defaultInstance;
			}
		}

		[ApplicationScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("http://w2k8r2x64rsut1/_vti_bin/StoragePoint/RestoreBLOB.asmx")]
		[SpecialSetting(SpecialSetting.WebServiceUrl)]
		public string Metalogix_SharePoint_RestoreWebService_StoragePointRestoreBLOBWebService
		{
			get
			{
				return (string)this["Metalogix_SharePoint_RestoreWebService_StoragePointRestoreBLOBWebService"];
			}
		}

		static Settings()
		{
			Metalogix.SharePoint.Properties.Settings.defaultInstance = (Metalogix.SharePoint.Properties.Settings)SettingsBase.Synchronized(new Metalogix.SharePoint.Properties.Settings());
		}

		public Settings()
		{
		}
	}
}