using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Administration;
using Metalogix.SharePoint.Properties;
using Metalogix.UI.WinForms;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
	[ActionConfig(new Type[] { typeof(DeleteBase) })]
	public class DeleteConfig : IActionConfig
	{
		private static Dictionary<Type, string[]> dct;

		static DeleteConfig()
		{
			Dictionary<Type, string[]> types = new Dictionary<Type, string[]>();
			Type type = typeof(SPSite);
			string[] strArrays = new string[] { Resources._SiteCollections, "Title" };
			types.Add(type, strArrays);
			Type type1 = typeof(SPWeb);
			string[] strArrays1 = new string[] { Resources._Sites, "Title" };
			types.Add(type1, strArrays1);
			Type type2 = typeof(SPList);
			string[] strArrays2 = new string[] { Resources._Lists, "Title" };
			types.Add(type2, strArrays2);
			Type type3 = typeof(SPDiscussionList);
			string[] strArrays3 = new string[] { Resources._Lists, "Title" };
			types.Add(type3, strArrays3);
			Type type4 = typeof(SPFolder);
			string[] strArrays4 = new string[] { Resources._Folders, "Name" };
			types.Add(type4, strArrays4);
			Type type5 = typeof(SPListItem);
			string[] strArrays5 = new string[] { Resources._Items, "Name" };
			types.Add(type5, strArrays5);
			Type type6 = typeof(SPDiscussionItem);
			string[] strArrays6 = new string[] { Resources._Items, "Name" };
			types.Add(type6, strArrays6);
			DeleteConfig.dct = types;
		}

		public DeleteConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			string str = null;
			Type type = context.ActionContext.Targets[0].GetType();
			str = (context.ActionContext.Targets.Count <= 1 ? string.Format(Resources.DeleteItem, type.GetProperty(DeleteConfig.dct[type][1]).GetValue(context.ActionContext.Targets[0], null)) : string.Format(Resources.DeleteItems, context.ActionContext.Targets.Count, DeleteConfig.dct[type][0]));
			if (FlatXtraMessageBox.Show(str, Resources.ConfirmDelete, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return ConfigurationResult.Cancel;
			}
			return ConfigurationResult.Run;
		}
	}
}