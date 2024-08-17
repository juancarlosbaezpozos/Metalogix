using Metalogix.Actions;
using System;

namespace Metalogix.SharePoint.Actions
{
	public abstract class SharePointActionheader : ActionHeader
	{
		protected SharePointActionheader()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			return SharePointAction<ActionOptions>.SharePointActionAppliesTo(this, sourceSelections, targetSelections);
		}
	}
}