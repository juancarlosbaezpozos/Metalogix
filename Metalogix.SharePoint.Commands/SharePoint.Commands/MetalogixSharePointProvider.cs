using Metalogix.Commands;
using Metalogix.Explorer;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Provider;

namespace Metalogix.SharePoint.Commands
{
	[CmdletProvider("MetalogixSharePointProvider", ProviderCapabilities.None)]
	public class MetalogixSharePointProvider : MetalogixNodeProvider
	{
		public MetalogixSharePointProvider()
		{
		}

		protected override void GetChildItems(string path, bool recurse)
		{
			Node node = base.FindNode(path);
			foreach (Node child in node.Children)
			{
				string str = string.Concat(path, (path.EndsWith("/") ? "" : "/"), child.Name);
				base.WriteItemObject(child, str, true);
				if (!recurse)
				{
					continue;
				}
				this.GetChildItems(str, recurse);
			}
			if (typeof(SPFolder).IsAssignableFrom(node.GetType()))
			{
				foreach (ListItem item in ((SPFolder)node).GetItems(false, ListItemQueryType.ListItem, null))
				{
					string str1 = string.Concat(path, (path.EndsWith("/") ? "" : "/"), item.Name);
					base.WriteItemObject(item, str1, true);
				}
			}
		}

		protected override void GetChildNames(string path, ReturnContainers returnContainers)
		{
			Node node = base.FindNode(path);
			foreach (Node child in node.Children)
			{
				base.WriteItemObject(child.Name, string.Concat(path, (path.EndsWith("/") ? "" : "/"), child.Name), true);
			}
			if (typeof(SPFolder).IsAssignableFrom(node.GetType()))
			{
				foreach (ListItem item in ((SPFolder)node).GetItems(false, ListItemQueryType.ListItem, null))
				{
					base.WriteItemObject(item.Name, string.Concat(path, (path.EndsWith("/") ? "" : "/"), item.Name), true);
				}
			}
		}

		protected override bool IsItemContainer(string path)
		{
			if (base.FindNode(path) == null)
			{
				base.WriteError(new ErrorRecord(new ArgumentException("The argument specified is not valid"), "InvalidArgument", ErrorCategory.InvalidArgument, path));
			}
			return true;
		}

		protected override System.Management.Automation.PSDriveInfo NewDrive(System.Management.Automation.PSDriveInfo drive)
		{
			SPConnection sPWeb;
			Credentials credential;
			if (drive == null)
			{
				base.WriteError(new ErrorRecord(new ArgumentNullException("drive"), "NullDrive", ErrorCategory.InvalidArgument, null));
				return null;
			}
			if (string.IsNullOrEmpty(drive.Root))
			{
				base.WriteError(new ErrorRecord(new ArgumentException("drive.Root"), "NoRoot", ErrorCategory.InvalidArgument, drive));
				return null;
			}
			SharePointDriveDynamicParameters dynamicParameters = base.DynamicParameters as SharePointDriveDynamicParameters;
			if (dynamicParameters == null)
			{
				credential = new Credentials();
			}
			else
			{
				credential = (dynamicParameters.User == null ? new Credentials() : new Credentials(dynamicParameters.User, dynamicParameters.Password.ToSecureString(), false));
			}
			Credentials credential1 = credential;
			SharePointAdapter adapterFromShortname = CommandLineParsingUtils.GetAdapterFromShortname(dynamicParameters.AdapterType);
			if (dynamicParameters.Type == null || !(dynamicParameters.Type.ToUpper() == "SERVER"))
			{
				sPWeb = new SPWeb(adapterFromShortname);
			}
			else
			{
				sPWeb = new SPServer(adapterFromShortname);
			}
			sPWeb.Adapter.Credentials = credential1;
			sPWeb.Adapter.Url = drive.Root;
			sPWeb.FetchChildren();
			return new NodeDriveInfo(drive)
			{
				Connection = sPWeb
			};
		}

		protected override object NewDriveDynamicParameters()
		{
			return new SharePointDriveDynamicParameters();
		}
	}
}