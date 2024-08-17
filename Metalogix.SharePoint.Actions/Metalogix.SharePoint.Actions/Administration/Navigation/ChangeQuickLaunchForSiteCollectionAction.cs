using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Options.Administration.Navigation;
using System;
using System.Collections;

namespace Metalogix.SharePoint.Actions.Administration.Navigation
{
	[MenuText("Change Site Collection Settings {3-Update} > 3:Quick Launch Settings... ")]
	[ShowInMenus(true)]
	public class ChangeQuickLaunchForSiteCollectionAction : ChangeQuickLaunchAction
	{
		public ChangeQuickLaunchForSiteCollectionAction()
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
						continue;
					}
					flag = false;
					return flag;
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