using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Actions;
using System;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Reporting
{
	[Image("Metalogix.SharePoint.UI.WinForms.Icons.Administration.Fwd16.ico")]
	[MenuText("Navigate to Search Result{5-Navigate}")]
	[Name("Navigate Search Result")]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(Node), true)]
	public class NavigateSearchResult : NavigateAction
	{
		public NavigateSearchResult()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			return false;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			Node item = target[0] as Node;
			if (item != null)
			{
				base.FireNavigationRequest(item.Location, NavigationPreference.Left);
				return;
			}
			FlatXtraMessageBox.Show("Could not locate the requested location within the current set of connections", "Navigation Failed", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		}
	}
}