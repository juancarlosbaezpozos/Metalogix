using Metalogix;
using Metalogix.UI.WinForms.Properties;
using System;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Metalogix.UI.WinForms.Tooltips
{
	public class TooltipManager
	{
		public TooltipManager()
		{
		}

		public static string GetToolTip(string resourceName, Assembly assembly = null)
		{
			string str;
			try
			{
				if (assembly == null)
				{
					assembly = ApplicationData.MainAssembly;
				}
				string str1 = assembly.GetManifestResourceNames().Single<string>((string n) => n.Contains("Tooltips"));
				str1 = str1.Replace(".resources", "");
				string str2 = (new ResourceManager(str1, assembly)).GetString(resourceName);
				if (str2 != null)
				{
					str = str2;
					return str;
				}
			}
			catch
			{
			}
			try
			{
				str = (new ResourceManager(typeof(Metalogix.UI.WinForms.Properties.Tooltips))).GetString(resourceName);
			}
			catch
			{
				str = null;
			}
			return str;
		}

		public static string GetTypedToolTip(Type type)
		{
			string str;
			try
			{
				Assembly assembly = Assembly.GetAssembly(type);
				string str1 = assembly.GetManifestResourceNames().Single<string>((string n) => n.EndsWith("Tooltips.resources"));
				str1 = str1.Replace(".resources", "");
				string resourceName = TooltipManager.TypeNameToResourceName(type.FullName);
				str = (new ResourceManager(str1, assembly)).GetString(resourceName);
			}
			catch (Exception exception)
			{
				str = null;
			}
			return str;
		}

		public static string TypeNameToResourceName(string typeName)
		{
			return typeName.Replace(".", "");
		}
	}
}