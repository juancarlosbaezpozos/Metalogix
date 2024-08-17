using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Administration.Navigation;
using System;
using System.Collections;

namespace Metalogix.SharePoint.Actions.Administration.Navigation
{
	[MenuText("Change Site Settings {3-Update} > 2:Global Navigation Settings... ")]
	[ShowInMenus(true)]
	public class ChangeGlobalNavigationForSiteAction : ChangeGlobalNavigationAction
	{
		public ChangeGlobalNavigationForSiteAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			bool flag;
			if (!base.AppliesTo(sourceSelections, targetSelections))
			{
				return false;
			}
			IEnumerator enumerator = targetSelections.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					SPWeb current = (SPWeb)enumerator.Current;
					if (current is SPSite || current.Parent == null)
					{
						flag = false;
						return flag;
					}
					else
					{
						if (current.Adapter.DisplayedShortName != "CSOM")
						{
							continue;
						}
						flag = false;
						return flag;
					}
				}
				return true;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return flag;
		}
	}
}