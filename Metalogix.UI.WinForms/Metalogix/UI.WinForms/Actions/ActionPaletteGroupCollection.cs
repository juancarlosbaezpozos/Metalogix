using System;
using System.Collections.Generic;

namespace Metalogix.UI.WinForms.Actions
{
	internal class ActionPaletteGroupCollection : List<ActionPaletteGroup>
	{
		public ActionPaletteGroupCollection()
		{
		}

		public ActionPaletteGroup Find(string sGroupText)
		{
			ActionPaletteGroup actionPaletteGroup;
			List<ActionPaletteGroup>.Enumerator enumerator = base.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ActionPaletteGroup current = enumerator.Current;
					if (current.GroupName != sGroupText)
					{
						continue;
					}
					actionPaletteGroup = current;
					return actionPaletteGroup;
				}
				return null;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return actionPaletteGroup;
		}

		public ActionPaletteGroup FindOrAdd(string sGroupText)
		{
			ActionPaletteGroup actionPaletteGroup = this.Find(sGroupText);
			if (actionPaletteGroup == null)
			{
				actionPaletteGroup = new ActionPaletteGroup(sGroupText);
				base.Add(actionPaletteGroup);
			}
			return actionPaletteGroup;
		}
	}
}