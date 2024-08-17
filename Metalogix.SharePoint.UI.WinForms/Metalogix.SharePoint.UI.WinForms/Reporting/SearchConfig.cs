using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Reporting;
using Metalogix.SharePoint.Adapters;
using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Reporting
{
	[ActionConfig(new Type[] { typeof(SearchAction) })]
	public class SearchConfig : IActionConfig
	{
		protected static Metalogix.SharePoint.UI.WinForms.Reporting.SearchDialog m_dlgSearch;

		public static Metalogix.SharePoint.UI.WinForms.Reporting.SearchDialog SearchDialog
		{
			get
			{
				if (SearchConfig.m_dlgSearch == null || SearchConfig.m_dlgSearch.IsDisposed)
				{
					SearchConfig.m_dlgSearch = new Metalogix.SharePoint.UI.WinForms.Reporting.SearchDialog()
					{
						Parameters = new SPSearchParameters()
					};
					if (Application.OpenForms.Count > 0)
					{
						Form form = (Application.OpenForms["MainForm"] != null ? Application.OpenForms["MainForm"] : Application.OpenForms[0]);
						int x = form.Location.X;
						int width = form.Size.Width;
						Size size = SearchConfig.SearchDialog.Size;
						int num = x + (width - size.Width) / 2;
						int y = form.Location.Y;
						int height = form.Size.Height;
						Size size1 = SearchConfig.SearchDialog.Size;
						int height1 = y + (height - size1.Height) / 2;
						SearchConfig.SearchDialog.StartLocation = new Point(num, height1);
					}
				}
				return SearchConfig.m_dlgSearch;
			}
		}

		static SearchConfig()
		{
		}

		public SearchConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			SearchConfig.SearchDialog.SearchNode = context.ActionContext.Targets[0] as SPNode;
			if (SearchConfig.SearchDialog.Visible)
			{
				SearchConfig.SearchDialog.Focus();
			}
			else
			{
				SearchConfig.SearchDialog.Show();
			}
			return ConfigurationResult.Run;
		}
	}
}