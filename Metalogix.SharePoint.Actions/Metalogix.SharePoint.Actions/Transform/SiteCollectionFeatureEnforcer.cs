using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Adapters;
using System;

namespace Metalogix.SharePoint.Actions.Transform
{
	public class SiteCollectionFeatureEnforcer : SiteFeatureEnforcerBase<PasteSiteCollectionAction, SPSiteCollection>
	{
		public override string Name
		{
			get
			{
				return "Enforce Site Collection Features";
			}
		}

		public SiteCollectionFeatureEnforcer()
		{
		}

		public override void BeginTransformation(PasteSiteCollectionAction action, SPWebCollection sources, SPSiteCollection targets)
		{
		}

		public override void EndTransformation(PasteSiteCollectionAction action, SPWebCollection sources, SPSiteCollection targets)
		{
		}

		public override SPWeb Transform(SPWeb dataObject, PasteSiteCollectionAction action, SPWebCollection sources, SPSiteCollection targets)
		{
			return base.UpdateWeb(dataObject, targets.ParentServer.Adapter.SharePointVersion);
		}
	}
}