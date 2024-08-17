using Metalogix.Actions;
using Metalogix.Licensing;
using Metalogix.SharePoint.Actions.Administration;
using System;
using System.Net;

namespace Metalogix.SharePoint.Actions.Database
{
	[Image("Metalogix.SharePoint.Actions.Icons.Database.ConnectToSharePointDatabaseBackupFile16x16.png")]
	[LargeImage("Metalogix.SharePoint.Actions.Icons.Database.ConnectToSharePointDatabaseBackupFile32x32.png")]
	[LicensedProducts(ProductFlags.SRM)]
	[MenuText("Add Connection {3-Connect} > Connect to SharePoint Database Backup File - Read only {0}")]
	[Name("Connect to SharePoint Database Backup File")]
	[ShowInMenus(true)]
	public class ConnectToBackup : Metalogix.SharePoint.Actions.Administration.ConnectAction
	{
		public ConnectToBackup()
		{
		}

		public string DetermineBackupFileName(string fileName, bool targetServerIsLocal)
		{
			string str = fileName;
			if (!this.IsUncPath(fileName) && !targetServerIsLocal)
			{
				string hostName = Dns.GetHostName();
				int num = fileName.IndexOf(':');
				str = string.Concat("\\\\", hostName, "\\", (num >= 0 ? string.Concat(fileName.Substring(0, num), "$", fileName.Substring(num + 1)) : fileName));
			}
			return str;
		}

		public bool IsUncPath(string path)
		{
			return path.StartsWith("\\\\");
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
		}
	}
}