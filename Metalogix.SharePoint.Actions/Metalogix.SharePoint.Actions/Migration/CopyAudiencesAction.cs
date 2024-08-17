using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Adapters;
using System;
using System.Collections;

namespace Metalogix.SharePoint.Actions.Migration
{
	[MenuText("3:Copy Farm Audiences{1-CopyServerElements}")]
	[Name("Copy Farm Audiences")]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPServer))]
	public class CopyAudiencesAction : CopyAction
	{
		public CopyAudiencesAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			bool flag = base.AppliesTo(sourceSelections, targetSelections);
			if (flag)
			{
				if (targetSelections.Count != 1)
				{
					return false;
				}
				SPServer item = targetSelections[0] as SPServer;
				if (item == null)
				{
					return false;
				}
				if (item.Adapter.IsDB)
				{
					return false;
				}
			}
			return flag;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			AudienceServerNodeCollection audienceServerNodeCollection = new AudienceServerNodeCollection();
			foreach (SPServer sPServer in target)
			{
				audienceServerNodeCollection.Add(sPServer);
			}
			base.RunAction(source, audienceServerNodeCollection);
		}
	}
}