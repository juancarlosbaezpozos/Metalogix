using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Actions.Administration
{
	[CmdletEnabled(false, "Remove-MLSharePointSite", new string[] { "Metalogix.SharePoint.Commands" })]
	[Image("Metalogix.SharePoint.Actions.Icons.Administration.Delete.ico")]
	[LicensedProducts(ProductFlags.CMWebComponents)]
	[MenuText("Delete Site {2-Delete}")]
	[MenuTextPlural("Delete Sites {2-Delete}", PluralCondition.MultipleTargets)]
	[Name("Delete Site")]
	[RequiresWriteAccess(true)]
	[Shortcut(ShortcutAction.Delete)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(SPWeb), false)]
	public class DeleteSite : DeleteBase
	{
		public DeleteSite()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (!SharePointAction<Metalogix.Actions.ActionOptions>.SharePointActionAppliesTo(this, sourceSelections, targetSelections))
			{
				return false;
			}
			return targetSelections.Count != 0;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			Type type = typeof(SPSite);
			List<SPWeb> sPWebs = new List<SPWeb>(target.Count);
			foreach (SPWeb sPWeb in target)
			{
				if (!type.IsAssignableFrom(sPWeb.GetType()))
				{
					sPWebs.Add(sPWeb);
				}
				else
				{
					DeleteBase.DeleteSPSite((SPSite)sPWeb);
					return;
				}
			}
			Type type1 = typeof(SPWeb);
		Label1:
			foreach (SPWeb sPWeb1 in target)
			{
				Node parent = sPWeb1.Parent;
				while (parent != null)
				{
					if (!type1.IsAssignableFrom(parent.GetType()) || !sPWebs.Contains((SPWeb)parent))
					{
						parent = parent.Parent;
					}
					else
					{
						sPWebs.Remove(sPWeb1);
						goto Label1;
					}
				}
			}
			foreach (SPWeb sPWeb2 in sPWebs)
			{
				sPWeb2.Delete();
				if (sPWeb2.Parent == null)
				{
					continue;
				}
				SPWebCollection subWebs = ((SPWeb)sPWeb2.Parent).SubWebs;
				subWebs.RemoveAt(subWebs.IndexOf(sPWeb2));
			}
		}
	}
}