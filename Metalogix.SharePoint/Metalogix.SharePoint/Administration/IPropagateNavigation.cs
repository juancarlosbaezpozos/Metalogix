using System;

namespace Metalogix.SharePoint.Administration
{
	public interface IPropagateNavigation
	{
		void StartNavigationPropagationHandler();

		void StopNavigationPropagationHandler();
	}
}