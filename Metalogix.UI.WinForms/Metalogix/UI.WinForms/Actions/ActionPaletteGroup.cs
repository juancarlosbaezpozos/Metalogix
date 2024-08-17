using System;

namespace Metalogix.UI.WinForms.Actions
{
	public class ActionPaletteGroup : IComparable
	{
		private string m_sGroupName;

		private ActionPaletteMenuCollection m_subMenus = new ActionPaletteMenuCollection();

		public string GroupName
		{
			get
			{
				return this.m_sGroupName;
			}
		}

		internal ActionPaletteMenuCollection SubMenus
		{
			get
			{
				return this.m_subMenus;
			}
		}

		public ActionPaletteGroup(string sGroupName)
		{
			this.m_sGroupName = sGroupName;
		}

		public int CompareTo(object obj)
		{
			return this.GroupName.CompareTo(((ActionPaletteGroup)obj).GroupName);
		}
	}
}