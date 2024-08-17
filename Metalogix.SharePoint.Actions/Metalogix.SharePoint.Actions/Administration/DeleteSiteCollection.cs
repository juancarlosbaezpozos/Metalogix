using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using System;
using System.Collections;

namespace Metalogix.SharePoint.Actions.Administration
{
	[CmdletEnabled(false, "Remove-MLSharePointSiteCollection", new string[] { "Metalogix.SharePoint.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Administration.Delete.ico")]
	[MenuText("Delete Site Collection {2-Delete}")]
	[MenuTextPlural("Delete Site Collections {2-Delete}", PluralCondition.MultipleTargets)]
	[Name("Delete Site Collection")]
	[RequiresWriteAccess(true)]
	[Shortcut(ShortcutAction.Delete)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(SPSite), false)]
	public class DeleteSiteCollection : DeleteBase
	{
		public DeleteSiteCollection()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			bool flag;
			if (!SharePointAction<Metalogix.Actions.ActionOptions>.SharePointActionAppliesTo(this, sourceSelections, targetSelections))
			{
				return false;
			}
			IEnumerator enumerator = targetSelections.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					SPNode current = (SPNode)enumerator.Current;
					if (current.Parent != null && !(current.Parent is SPTenantMySiteHost))
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

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			foreach (SPSite sPSite in target)
			{
				DeleteBase.DeleteSPSite(sPSite);
			}
		}
	}
}