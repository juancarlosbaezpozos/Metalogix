using Metalogix;
using Metalogix.Actions;
using Metalogix.Interfaces;
using Metalogix.Jobs;
using Metalogix.UI.WinForms.Actions;
using System;

namespace Metalogix.UI.WinForms.Jobs.Actions
{
	[MenuText("Navigate {5-Navigate} > To Source")]
	[Name("Navigate to Source")]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(Job))]
	public class NavigateToJobSourceAction : NavigateAction
	{
		public NavigateToJobSourceAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (!base.AppliesTo(sourceSelections, targetSelections))
			{
				return false;
			}
			if (string.IsNullOrEmpty(((Job)targetSelections[0]).Source))
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
					XmlLocation = item.SourceXml,
					NavPreference = NavigationPreference.Left
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