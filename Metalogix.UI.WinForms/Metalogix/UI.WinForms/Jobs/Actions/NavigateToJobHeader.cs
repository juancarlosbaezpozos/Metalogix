using Metalogix.Actions;
using Metalogix.Jobs;
using Metalogix.UI.WinForms.Actions;
using System;

namespace Metalogix.UI.WinForms.Jobs.Actions
{
	[Image("Metalogix.UI.WinForms.Resources.Navigate16.png")]
	[MenuText("Navigate {5-Navigate}")]
	[Name("Navigate Log Item Menu Header")]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(Job))]
	public class NavigateToJobHeader : NavigateAction
	{
		public NavigateToJobHeader()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (!base.AppliesTo(sourceSelections, targetSelections))
			{
				return false;
			}
			Job item = (Job)targetSelections[0];
			if (string.IsNullOrEmpty(item.Source) && string.IsNullOrEmpty(item.Target))
			{
				return false;
			}
			return true;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
		}
	}
}