using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Interfaces;
using System;
using System.Collections;

namespace Metalogix.SharePoint.Actions.Migration
{
	[MenuText("2:Copy Farm Managed Metadata{1-CopyServerElements}")]
	[Name("Copy Farm Managed Metadata")]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPServer))]
	public class CopyManagedMetadataServicesAction : CopyAction
	{
		public CopyManagedMetadataServicesAction()
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
				ITaxonomyConnection item = targetSelections[0] as ITaxonomyConnection;
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
			ManagedMetadataServicesServerNodeCollection managedMetadataServicesServerNodeCollection = new ManagedMetadataServicesServerNodeCollection();
			foreach (ITaxonomyConnection taxonomyConnection in target)
			{
				managedMetadataServicesServerNodeCollection.Add(taxonomyConnection.Node);
			}
			base.RunAction(source, managedMetadataServicesServerNodeCollection);
		}
	}
}