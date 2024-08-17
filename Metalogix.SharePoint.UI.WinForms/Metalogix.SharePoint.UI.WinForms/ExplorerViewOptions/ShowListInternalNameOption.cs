using Metalogix;
using Metalogix.SharePoint;
using Metalogix.SharePoint.UI.WinForms.Properties;
using Metalogix.UI.WinForms.Explorer;
using System;
using System.Drawing;
using System.Reflection;

namespace Metalogix.SharePoint.UI.WinForms.ExplorerViewOptions
{
	public class ShowListInternalNameOption : ExplorerViewOption
	{
		public static string OPTION_NAME;

		static ShowListInternalNameOption()
		{
			ShowListInternalNameOption.OPTION_NAME = "ViewShowListInternalName";
			ExplorerViewOption.Initialize(ShowListInternalNameOption.OPTION_NAME);
		}

		public ShowListInternalNameOption()
		{
			SPList.SHOW_LIST_INTERNAL_NAME = base.GetSetting();
		}

		public override Type GetApplicableType()
		{
			return typeof(SPList);
		}

		public override Image GetImage()
		{
			return ImageCache.GetImage("Metalogix.SharePoint.UI.WinForms.Icons.Administration.ShowInternalListNames16.png", Assembly.GetExecutingAssembly());
		}

		public override Image GetLargeImage()
		{
			return ImageCache.GetImage("Metalogix.SharePoint.UI.WinForms.Icons.Administration.ShowInternalListNames32.png", Assembly.GetExecutingAssembly());
		}

		public override string GetName()
		{
			return Resources.ViewShowListInternalName;
		}

		public override string GetOptionName()
		{
			return ShowListInternalNameOption.OPTION_NAME;
		}

		public override void OnSettingsChanged(string sOptionName, bool bValue)
		{
			SPList.SHOW_LIST_INTERNAL_NAME = bValue;
		}
	}
}