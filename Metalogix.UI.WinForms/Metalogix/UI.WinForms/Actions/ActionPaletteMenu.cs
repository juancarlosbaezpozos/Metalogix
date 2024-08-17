using Metalogix;
using Metalogix.Actions;
using Metalogix.Jobs;
using Metalogix.UI.WinForms;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Metalogix.UI.WinForms.Actions
{
	internal class ActionPaletteMenu : IComparable
	{
		private bool m_bFlat;

		private Metalogix.Actions.Action m_action;

		private string m_sMenuText;

		private ActionPaletteGroupCollection m_subGroups = new ActionPaletteGroupCollection();

		public Metalogix.Actions.Action Action
		{
			get
			{
				return this.m_action;
			}
			internal set
			{
				this.m_action = value;
			}
		}

		public bool IsGroup
		{
			get
			{
				return this.Action == null;
			}
		}

		public string MenuText
		{
			get
			{
				return this.m_sMenuText;
			}
		}

		public ActionPaletteGroupCollection SubGroups
		{
			get
			{
				return this.m_subGroups;
			}
		}

		public ActionPaletteMenu(ActionContext actionContext, ActionCollection actions, bool bFlat)
		{
			this.m_bFlat = bFlat;
			bool flag = (UIConfigurationVariables.ShowAdvanced ? false : ApplicationData.IsSharePointEdition);
			foreach (Metalogix.Actions.Action action in (IEnumerable<Metalogix.Actions.Action>)actions)
			{
				if (!flag || action.TargetType.Name.Equals(typeof(Job).Name, StringComparison.OrdinalIgnoreCase) || action.TargetType.Name.Equals(typeof(LogItem).Name, StringComparison.OrdinalIgnoreCase))
				{
					if (!UIConfigurationVariables.ShowAdvanced && action.IsAdvanced)
					{
						continue;
					}
					this.AddSubMenu(action.GetMenuText(actionContext), action);
				}
				else
				{
					if (!action.IsBasicModeAllowed)
					{
						continue;
					}
					this.AddSubMenu(action.GetMenuText(actionContext), action);
				}
			}
		}

		public ActionPaletteMenu(string sMenuText, Metalogix.Actions.Action a)
		{
			this.m_action = a;
			this.m_sMenuText = sMenuText;
		}

		private void AddSubMenu(string sMenuTextPath, Metalogix.Actions.Action a)
		{
			char[] chrArray = new char[] { '>' };
			string str = sMenuTextPath.Trim(chrArray);
			string str1 = str.Trim();
			string str2 = "";
			int num = str.IndexOfAny(chrArray);
			if (num >= 0)
			{
				str1 = str.Substring(0, num);
				str2 = str.Substring(num);
			}
			string str3 = str1;
			string str4 = "";
			int num1 = str1.IndexOf("{");
			if (num1 >= 0)
			{
				int num2 = str1.LastIndexOf("}");
				int num3 = (num2 < 0 ? str1.Length - num1 : num2 - num1);
				str3 = str1.Substring(0, num1);
				str4 = str1.Substring(num1, num3);
			}
			str3 = str3.Trim();
			str4 = str4.Trim();
			ActionPaletteGroup actionPaletteGroup = this.SubGroups.FindOrAdd(str4);
			if (str2 == "")
			{
				actionPaletteGroup.SubMenus.FindOrAdd(str3, a);
				return;
			}
			if (this.m_bFlat)
			{
				this.AddSubMenu(str2, a);
				return;
			}
			ActionPaletteMenu actionPaletteMenu = actionPaletteGroup.SubMenus.FindOrAdd(str3, null);
			actionPaletteMenu.AddSubMenu(str2, a);
		}

		public int CompareTo(object obj)
		{
			return this.MenuText.CompareTo(((ActionPaletteMenu)obj).MenuText);
		}
	}
}