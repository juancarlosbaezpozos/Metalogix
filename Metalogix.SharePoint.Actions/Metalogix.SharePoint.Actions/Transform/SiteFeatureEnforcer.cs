using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Adapters;
using System;

namespace Metalogix.SharePoint.Actions.Transform
{
	public class SiteFeatureEnforcer : SiteFeatureEnforcerBase<PasteSiteAction, SPWebCollection>
	{
		public override string Name
		{
			get
			{
				return "Enforce Site Features";
			}
		}

		public SiteFeatureEnforcer()
		{
		}

		public override void BeginTransformation(PasteSiteAction action, SPWebCollection sources, SPWebCollection targets)
		{
		}

		public override void EndTransformation(PasteSiteAction action, SPWebCollection sources, SPWebCollection targets)
		{
		}

		public override SPWeb Transform(SPWeb dataObject, PasteSiteAction action, SPWebCollection sources, SPWebCollection targets)
		{
			return base.UpdateWeb(dataObject, targets.ParentWeb.Adapter.SharePointVersion);
		}
	}
}