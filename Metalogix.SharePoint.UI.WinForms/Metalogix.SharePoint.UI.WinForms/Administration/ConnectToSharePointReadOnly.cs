using Metalogix.Actions;
using Metalogix.SharePoint.Actions.Administration;
using System;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
	[ActionConfig(new Type[] { typeof(Metalogix.SharePoint.Actions.Administration.ConnectToSharePointReadOnly) })]
	public class ConnectToSharePointReadOnly : ConnectToSharePointConfig
	{
		protected override bool ConnectAsReadOnly
		{
			get
			{
				return true;
			}
		}

		public ConnectToSharePointReadOnly()
		{
		}
	}
}