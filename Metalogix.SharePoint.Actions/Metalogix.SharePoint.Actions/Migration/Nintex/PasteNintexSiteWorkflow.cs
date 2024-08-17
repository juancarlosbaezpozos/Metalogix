using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Nintex;
using System;

namespace Metalogix.SharePoint.Actions.Migration.Nintex
{
	[SourceType(typeof(SPNintexWorkflow), false)]
	[TargetType(typeof(SPWeb))]
	public class PasteNintexSiteWorkflow : PasteNintexWorkflow
	{
		public PasteNintexSiteWorkflow()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			return false;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			SPNintexWorkflow item = source[0] as SPNintexWorkflow;
			base.MigrateWorkflow(item, target[0] as SPWeb, null);
		}

		protected override void RunOperation(object[] oParams)
		{
			SPNintexWorkflow sPNintexWorkflow = oParams[0] as SPNintexWorkflow;
			base.MigrateWorkflow(sPNintexWorkflow, oParams[1] as SPWeb, null);
		}
	}
}