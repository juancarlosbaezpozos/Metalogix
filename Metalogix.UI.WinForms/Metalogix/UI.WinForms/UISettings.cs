using Metalogix;
using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms
{
	public class UISettings
	{
		private static UIApplicationSettings s_AppSettings;

		public static UIApplicationSettings AppSettings
		{
			get
			{
				if (UISettings.s_AppSettings == null)
				{
					UISettings.s_AppSettings = new UIApplicationSettings();
				}
				return UISettings.s_AppSettings;
			}
		}

		public static System.Drawing.Icon Icon
		{
			get
			{
				if (Application.OpenForms.Count <= 0)
				{
					return null;
				}
				return ((Application.OpenForms["MainForm"] != null ? Application.OpenForms["MainForm"] : Application.OpenForms[0])).Icon;
			}
		}

		public static bool IsRemoteSession
		{
			get
			{
				return SystemInformation.TerminalServerSession;
			}
		}

		internal static string SettingsXMLFile
		{
			get
			{
				return Path.Combine(ApplicationData.ApplicationPath, "Settings.xml");
			}
		}

		public UISettings()
		{
		}
	}
}