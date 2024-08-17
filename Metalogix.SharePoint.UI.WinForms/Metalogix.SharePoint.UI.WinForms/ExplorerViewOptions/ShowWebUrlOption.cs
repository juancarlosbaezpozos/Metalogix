using Metalogix;
using Metalogix.SharePoint;
using Metalogix.SharePoint.UI.WinForms.Properties;
using Metalogix.UI.WinForms.Explorer;
using System;
using System.Drawing;
using System.Reflection;

namespace Metalogix.SharePoint.UI.WinForms.ExplorerViewOptions
{
	public class ShowWebUrlOption : ExplorerViewOption
	{
		public static string OPTION_NAME;

		static ShowWebUrlOption()
		{
			ShowWebUrlOption.OPTION_NAME = "ViewShowWebURL";
			ExplorerViewOption.Initialize(ShowWebUrlOption.OPTION_NAME);
		}

		public ShowWebUrlOption()
		{
			SPWeb.SHOW_WEB_URL = base.GetSetting();
		}

		public override Type GetApplicableType()
		{
			return typeof(SPWeb);
		}

		public override Image GetImage()
		{
			return ImageCache.GetImage("Metalogix.SharePoint.UI.WinForms.Icons.Administration.ShowWebUrls16.png", Assembly.GetExecutingAssembly());
		}

		public override Image GetLargeImage()
		{
			return ImageCache.GetImage("Metalogix.SharePoint.UI.WinForms.Icons.Administration.ShowWebUrls32.png", Assembly.GetExecutingAssembly());
		}

		public override string GetName()
		{
			return Resources.ViewShowWebURLOptionName;
		}

		public override string GetOptionName()
		{
			return ShowWebUrlOption.OPTION_NAME;
		}

		public override void OnSettingsChanged(string sOptionName, bool bValue)
		{
			SPWeb.SHOW_WEB_URL = bValue;
		}
	}
}