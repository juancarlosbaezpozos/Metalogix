using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Nintex;
using System;

namespace Metalogix.SharePoint.Actions.Migration.Nintex
{
	[SourceType(typeof(SPNintexWorkflow), false)]
	[TargetType(typeof(SPList))]
	public class PasteNintexListWorkflow : PasteNintexWorkflow
	{
		public PasteNintexListWorkflow()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			return false;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			SPNintexWorkflow item = source[0] as SPNintexWorkflow;
			SPList sPList = target[0] as SPList;
			if (sPList != null)
			{
				base.MigrateWorkflow(item, sPList.ParentWeb, sPList);
			}
		}

		protected override void RunOperation(object[] oParams)
		{
			SPNintexWorkflow sPNintexWorkflow = oParams[0] as SPNintexWorkflow;
			SPList sPList = oParams[1] as SPList;
			if (sPList != null)
			{
				base.MigrateWorkflow(sPNintexWorkflow, sPList.ParentWeb, sPList);
			}
		}
	}
}