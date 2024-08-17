using Metalogix.Actions;
using Metalogix.SharePoint;
using System;

namespace Metalogix.SharePoint.Actions.Migration
{
	[MenuText("Copy Tenant Managed Metadata {1-Copy}")]
	[Name("Copy Tenant Managed Metadata")]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPTenant))]
	public class CopyTenantManagedMetadataAction : CopyAction
	{
		public CopyTenantManagedMetadataAction()
		{
		}
	}
}