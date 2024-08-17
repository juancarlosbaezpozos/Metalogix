using Metalogix.Actions;
using System;

namespace Metalogix.SharePoint.Actions.Administration
{
	[BasicModeViewAllowed(true)]
	[Image("Metalogix.SharePoint.Actions.Icons.Administration.AddConnection.ico")]
	[MenuText("Add Connection {3-Connect}")]
	[ShowInMenus(true)]
	[TargetCardinality(Cardinality.Zero)]
	public class AddConnectionHeader : ActionHeader
	{
		public AddConnectionHeader()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			return base.AppliesTo(sourceSelections, targetSelections);
		}
	}
}