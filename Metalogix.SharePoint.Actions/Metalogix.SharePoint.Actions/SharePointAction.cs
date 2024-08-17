using Metalogix.Actions;
using System;

namespace Metalogix.SharePoint.Actions
{
	[ShowInMenus(false)]
	public sealed class SharePointAction : SharePointAction<Metalogix.Actions.ActionOptions>
	{
		public SharePointAction()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			throw new NotImplementedException();
		}
	}
}