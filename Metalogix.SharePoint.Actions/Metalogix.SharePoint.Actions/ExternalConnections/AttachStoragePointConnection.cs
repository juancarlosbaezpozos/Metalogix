using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.ExternalConnections;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Options.ExternalConnections;
using System;
using System.Collections.Generic;
using Metalogix.SharePoint.ExternalConnections;

namespace Metalogix.SharePoint.Actions.ExternalConnections
{
	[Image("Metalogix.SharePoint.Actions.Icons.Administration.StoragePoint.ico")]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.CMCSharePoint | ProductFlags.SRM | ProductFlags.CMWebComponents)]
	[MenuText("Attach External Connections {2-ExternalConnections} > StoragePoint Connection...")]
	[Name("Attach StoragePoint Connection")]
	[RunAsync(false)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPConnection), true)]
	public class AttachStoragePointConnection : SharePointAction<AttachStoragePointConnectionsOptions>
	{
		public AttachStoragePointConnection()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (!base.AppliesTo(sourceSelections, targetSelections))
			{
				return false;
			}
			if (targetSelections.Count > 0 && targetSelections[0] is SPTenant)
			{
				return false;
			}
			if ((targetSelections[0] as SPConnection).Parent != null)
			{
				return false;
			}
			return true;
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			Node item = (Node)target[0];
			Dictionary<int, ExternalConnection> externalConnectionsOfType = item.GetExternalConnectionsOfType<StoragePointExternalConnection>(false);
			Dictionary<int, ExternalConnection> connections = base.SharePointOptions.Connections;
			foreach (KeyValuePair<int, ExternalConnection> keyValuePair in externalConnectionsOfType)
			{
				if (connections.ContainsKey(keyValuePair.Key))
				{
					continue;
				}
				item.RemoveExternalConnection(keyValuePair.Value);
				keyValuePair.Value.Delete();
			}
			foreach (KeyValuePair<int, ExternalConnection> connection in connections)
			{
				if (externalConnectionsOfType.ContainsKey(connection.Key))
				{
					continue;
				}
				connection.Value.Insert();
				item.AddExternalConnection(connection.Value);
			}
		}
	}
}