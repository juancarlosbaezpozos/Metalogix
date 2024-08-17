using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.UI.WinForms.Properties
{
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
	internal sealed class Settings : ApplicationSettingsBase
	{
		private static Metalogix.SharePoint.UI.WinForms.Properties.Settings defaultInstance;

		public static Metalogix.SharePoint.UI.WinForms.Properties.Settings Default
		{
			get
			{
				return Metalogix.SharePoint.UI.WinForms.Properties.Settings.defaultInstance;
			}
		}

		static Settings()
		{
			Metalogix.SharePoint.UI.WinForms.Properties.Settings.defaultInstance = (Metalogix.SharePoint.UI.WinForms.Properties.Settings)SettingsBase.Synchronized(new Metalogix.SharePoint.UI.WinForms.Properties.Settings());
		}

		public Settings()
		{
		}
	}
}