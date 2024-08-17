using Metalogix;
using Metalogix.Actions;
using Metalogix.Interfaces;
using Metalogix.Jobs;
using Metalogix.UI.WinForms.Actions;
using System;

namespace Metalogix.UI.WinForms.Jobs.Actions
{
	[MenuText("Navigate {5-Navigate} > To Target")]
	[Name("Navigate to Target")]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(Job))]
	public class NavigateToJobTargetAction : NavigateAction
	{
		public NavigateToJobTargetAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (!base.AppliesTo(sourceSelections, targetSelections))
			{
				return false;
			}
			if (string.IsNullOrEmpty(((Job)targetSelections[0]).Target))
			{
				return false;
			}
			return true;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			try
			{
				Job item = (Job)target[0];
				NavigationRequestEventArgs navigationRequestEventArg = new NavigationRequestEventArgs()
				{
					XmlLocation = item.TargetXml,
					NavPreference = NavigationPreference.Right
				};
				base.FireNavigationRequest(navigationRequestEventArg);
			}
			catch (Exception exception)
			{
				GlobalServices.ErrorHandler.HandleException(exception);
			}
		}
	}
}