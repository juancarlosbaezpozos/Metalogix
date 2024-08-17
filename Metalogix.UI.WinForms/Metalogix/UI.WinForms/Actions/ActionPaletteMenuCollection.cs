using Metalogix.Actions;
using System;
using System.Collections.Generic;

namespace Metalogix.UI.WinForms.Actions
{
	internal class ActionPaletteMenuCollection : List<ActionPaletteMenu>
	{
		public ActionPaletteMenuCollection()
		{
		}

		public ActionPaletteMenu Find(string sMenuText)
		{
			ActionPaletteMenu actionPaletteMenu;
			List<ActionPaletteMenu>.Enumerator enumerator = base.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ActionPaletteMenu current = enumerator.Current;
					if (current.MenuText != sMenuText)
					{
						continue;
					}
					actionPaletteMenu = current;
					return actionPaletteMenu;
				}
				return null;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return actionPaletteMenu;
		}

		public ActionPaletteMenu FindOrAdd(string sMenuText, Metalogix.Actions.Action a)
		{
			ActionPaletteMenu actionPaletteMenu = this.Find(sMenuText);
			if (actionPaletteMenu == null)
			{
				actionPaletteMenu = new ActionPaletteMenu(sMenuText, a);
				base.Add(actionPaletteMenu);
			}
			else if (actionPaletteMenu.Action == null && a != null)
			{
				actionPaletteMenu.Action = a;
			}
			return actionPaletteMenu;
		}
	}
}