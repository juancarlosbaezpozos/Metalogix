using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.UI.WinForms;
using System;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Actions
{
	[MenuText("Navigate {5-Navigate} > To Target")]
	[Name("Navigate to Target")]
	[TargetType(typeof(LogItem))]
	public class NavigateToLogItemTargetAction : NavigateAction, ILogAction
	{
		public NavigateToLogItemTargetAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			bool flag = base.AppliesTo(sourceSelections, targetSelections);
			if (flag)
			{
				string target = (targetSelections[0] as LogItem).Target;
				if (target == null || target.Length == 0)
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
				string str = item.Target;
				nodeByUrl = Settings.ActiveConnections.GetNodeByUrl(str);
			}
			catch
			{
			}
			if (nodeByUrl != null)
			{
				base.FireNavigationRequest(nodeByUrl.Location, NavigationPreference.Right);
				return;
			}
			FlatXtraMessageBox.Show("Could not locate the requested location within the current set of connections", "Navigation Failed", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		}
	}
}