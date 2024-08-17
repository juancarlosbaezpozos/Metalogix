using Metalogix.Commands;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Commands;
using Metalogix.Utilities;
using System;
using System.Management.Automation;
using System.Management.Automation.Provider;

namespace Metalogix.SharePoint.Commands.Database
{
	[CmdletProvider("MetalogixSharePointDatabaseProvider", ProviderCapabilities.None)]
	public class MetalogixSharePointDatabaseProvider : MetalogixSharePointProvider
	{
		public MetalogixSharePointDatabaseProvider()
		{
		}

		protected override System.Management.Automation.PSDriveInfo NewDrive(System.Management.Automation.PSDriveInfo drive)
		{
			SPConnection sPServer;
			Credentials credential;
			string str;
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
			SharePointDatabaseDriveDynamicParameters dynamicParameters = base.DynamicParameters as SharePointDatabaseDriveDynamicParameters;
			if (dynamicParameters == null)
			{
				credential = new Credentials();
			}
			else
			{
				credential = (dynamicParameters.User == null ? new Credentials() : new Credentials(dynamicParameters.User, dynamicParameters.Password.ToSecureString(), false));
			}
			Credentials credential1 = credential;
			string[] strArrays = drive.Root.Replace("\\", "/").Split(new char[] { '/' });
			if ((int)strArrays.Length > 1)
			{
				str = strArrays[1];
			}
			else
			{
				str = null;
			}
			string str1 = str;
			strArrays = strArrays[0].Split(new char[] { '.' });
			if ((int)strArrays.Length < 2)
			{
				base.WriteError(new ErrorRecord(new ArgumentException("The root does not contain a DB connection in the form of [Server.Database]", "drive.Root"), "InvalidRoot", ErrorCategory.InvalidArgument, drive));
				return null;
			}
			string str2 = strArrays[0];
			string str3 = strArrays[1];
			object[] objArray = new object[] { str2, str3, credential1 };
			SharePointAdapter dBAdapter = SharePointAdapter.GetDBAdapter(objArray);
			if (dBAdapter == null)
			{
				base.WriteError(new ErrorRecord(new Exception("Database reading access is not available"), "InvalidRoot", ErrorCategory.InvalidArgument, drive));
				return null;
			}
			if (str1 == null)
			{
				sPServer = new SPServer(dBAdapter);
			}
			else
			{
				((IDBReader)dBAdapter).HostHeader = dynamicParameters.HostHeader;
				dBAdapter.Url = string.Concat("/", str1);
				sPServer = new SPWeb(dBAdapter);
			}
			sPServer.CheckConnection();
			return new NodeDriveInfo(drive)
			{
				Connection = sPServer
			};
		}

		protected override object NewDriveDynamicParameters()
		{
			return new SharePointDatabaseDriveDynamicParameters();
		}
	}
}