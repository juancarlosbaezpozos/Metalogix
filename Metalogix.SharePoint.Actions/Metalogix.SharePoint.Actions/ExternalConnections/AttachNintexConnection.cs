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
	[BasicModeViewAllowed(true)]
	[Image("Metalogix.SharePoint.Actions.Icons.ExternalConnections.Nintex.ico")]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.CMCSharePoint | ProductFlags.SRM | ProductFlags.CMWebComponents)]
	[MenuText("Attach External Connections {2-ExternalConnections} > Nintex Workflow Database...")]
	[Name("Attach Nintex Workflow Database")]
	[RunAsync(false)]
	[TargetCardinality(Cardinality.One)]
	[TargetType(typeof(SPConnection), true)]
	public class AttachNintexConnection : SharePointAction<AttachNintexConnectionsOptions>
	{
		public AttachNintexConnection()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			if (targetSelections.Count > 0 && (targetSelections[0] is SPTenant || ((SPNode)targetSelections[0]).Parent is SPTenant))
			{
				return false;
			}
			return base.AppliesTo(sourceSelections, targetSelections);
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			Node item = (Node)target[0];
			Dictionary<int, ExternalConnection> externalConnectionsOfType = item.GetExternalConnectionsOfType<NintexExternalConnection>(false);
			Dictionary<int, ExternalConnection> connections = base.SharePointOptions.Connections;
			foreach (KeyValuePair<int, ExternalConnection> keyValuePair in externalConnectionsOfType)
			{
				if (connections.ContainsKey(keyValuePair.Key))
				{
					continue;
				}
				item.RemoveExternalConnection(keyValuePair.Value);
			}
			foreach (KeyValuePair<int, ExternalConnection> connection in connections)
			{
				if (externalConnectionsOfType.ContainsKey(connection.Key))
				{
					continue;
				}
				item.AddExternalConnection(connection.Value);
			}
		}
	}
}