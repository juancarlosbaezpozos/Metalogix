using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.ObjectResolution;
using System;
using System.Threading;

namespace Metalogix.UI.WinForms.Actions
{
	[Batchable(false, "")]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[RequiresWriteAccess(false)]
	[RunAsync(false)]
	[ShowInMenus(true)]
	[TargetCardinality(Cardinality.One)]
	public abstract class NavigateAction : Metalogix.Actions.Action
	{
		protected NavigateAction()
		{
		}

		public void FireNavigationRequest(Location location, NavigationPreference preference)
		{
			NavigationRequestEventArgs navigationRequestEventArg = new NavigationRequestEventArgs()
			{
				NavPreference = preference,
				XmlLocation = location.ToXML()
			};
			this.FireNavigationRequest(navigationRequestEventArg);
		}

		public void FireNavigationRequest(NavigationRequestEventArgs e)
		{
			if (NavigateAction.NavigationRequest != null)
			{
				NavigateAction.NavigationRequest(e);
			}
		}

		public static event NavigationRequestHandler NavigationRequest;
	}
}