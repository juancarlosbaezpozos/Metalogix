using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.UI.WinForms;
using System;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Actions
{
	[MenuText("Navigate {5-Navigate} > To Source")]
	[Name("Navigate to Source")]
	[TargetType(typeof(LogItem))]
	public class NavigateToLogItemSourceAction : NavigateAction, ILogAction
	{
		public NavigateToLogItemSourceAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			bool flag = base.AppliesTo(sourceSelections, targetSelections);
			if (flag)
			{
				string source = (targetSelections[0] as LogItem).Source;
				if (source == null || source.Length == 0)
				{
					flag = false;
				}
			}
			return flag;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			LogItem item = target[0] as LogItem;
			Node nodeByUrl = null;
			try
			{
				string str = item.Source;
				nodeByUrl = Settings.ActiveConnections.GetNodeByUrl(str);
			}
			catch
			{
			}
			if (nodeByUrl != null)
			{
				base.FireNavigationRequest(nodeByUrl.Location, NavigationPreference.Left);
				return;
			}
			FlatXtraMessageBox.Show("Could not locate the requested location within the current set of connections", "Navigation Failed", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		}
	}
}