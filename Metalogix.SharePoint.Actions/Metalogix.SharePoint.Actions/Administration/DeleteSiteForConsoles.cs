using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using System;
using System.Collections;

namespace Metalogix.SharePoint.Actions.Administration
{
	[LicensedProducts(ProductFlags.CMCSharePoint | ProductFlags.CMCFileShare | ProductFlags.CMCPublicFolder | ProductFlags.UnifiedContentMatrixExpressKey | ProductFlags.CMCWebsite | ProductFlags.CMCeRoom | ProductFlags.CMCOracleAndStellent | ProductFlags.CMCDocumentum | ProductFlags.CMCBlogsAndWikis | ProductFlags.CMCGoogle | ProductFlags.SRM)]
	public class DeleteSiteForConsoles : DeleteSite
	{
		public DeleteSiteForConsoles()
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
					if (((SPNode)enumerator.Current).Parent != null)
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