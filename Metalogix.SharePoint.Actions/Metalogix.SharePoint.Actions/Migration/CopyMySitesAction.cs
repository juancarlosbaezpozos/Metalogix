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
	[MenuText("1:Copy MySites{1-CopyServerElements}")]
	[Name("Copy MySites")]
	[TargetType(typeof(SPBaseServer), true)]
	public class CopyMySitesAction : CopyAction
	{
		public CopyMySitesAction()
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
				if (targetSelections[0] is SPTenantMySiteHost)
				{
					return true;
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
			MySitesServerNodeCollection mySitesServerNodeCollection = new MySitesServerNodeCollection();
			foreach (SPBaseServer sPBaseServer in target)
			{
				mySitesServerNodeCollection.Add(sPBaseServer);
			}
			base.RunAction(source, mySitesServerNodeCollection);
		}
	}
}