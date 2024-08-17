using Metalogix.Actions;
using Metalogix.SharePoint.Actions.Administration;
using System;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
	[ActionConfig(new Type[] { typeof(ConnectToSharePoint) })]
	public class ConnectToSharePointWriteable : ConnectToSharePointConfig
	{
		protected override bool ConnectAsReadOnly
		{
			get
			{
				return false;
			}
		}

		public ConnectToSharePointWriteable()
		{
		}
	}
}