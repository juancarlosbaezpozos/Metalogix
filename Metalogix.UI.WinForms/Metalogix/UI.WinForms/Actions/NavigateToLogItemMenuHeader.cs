using Metalogix.Actions;
using System;

namespace Metalogix.UI.WinForms.Actions
{
	[Image("Metalogix.UI.WinForms.Resources.Navigate16.png")]
	[MenuText("Navigate {5-Navigate}")]
	[Name("Navigate Log Item Menu Header")]
	[TargetType(typeof(LogItem))]
	public class NavigateToLogItemMenuHeader : NavigateAction, ILogAction
	{
		public NavigateToLogItemMenuHeader()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			bool flag = base.AppliesTo(sourceSelections, targetSelections);
			if (flag)
			{
				LogItem item = targetSelections[0] as LogItem;
				string source = item.Source;
				string target = item.Target;
				if ((source == null || source.Length == 0) && (target == null || target.Length == 0))
				{
					flag = false;
				}
			}
			return flag;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
		}
	}
}