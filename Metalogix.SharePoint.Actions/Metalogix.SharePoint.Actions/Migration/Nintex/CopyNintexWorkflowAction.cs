using Metalogix.Actions;
using Metalogix.Licensing;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Nintex;
using System;

namespace Metalogix.SharePoint.Actions.Migration.Nintex
{
	[LicensedProducts(ProductFlags.CMCSharePoint)]
	[MenuText("Copy Workflow {1-Copy}")]
	[MenuTextPlural("Copy Workflows {1-Copy}", PluralCondition.MultipleTargets)]
	[Name("Copy Nintex Workflow")]
	[Shortcut(ShortcutAction.Copy)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(SPNintexWorkflow), false)]
	public class CopyNintexWorkflowAction : CopyAction
	{
		public CopyNintexWorkflowAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			return false;
		}
	}
}