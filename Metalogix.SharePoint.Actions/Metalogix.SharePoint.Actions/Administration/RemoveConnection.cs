using Metalogix;
using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.Interfaces;
using Metalogix.Licensing;
using Metalogix.Metabase.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Database;
using System;
using System.Collections;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Actions.Administration
{
	[ActionConfigRequired(true)]
	[BasicModeViewAllowed(true)]
	[IsConnectivityAction(true)]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[MenuText("Disconnect {3-Connect}")]
	[Name("Remove Connection")]
	[RunAsync(true)]
	[SourceCardinality(Cardinality.Zero)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(SPConnection))]
	public class RemoveConnection : Metalogix.Actions.Action
	{
		public bool DeleteTempDBs
		{
			get;
			set;
		}

		public RemoveConnection()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			bool flag;
			if (!base.AppliesTo(sourceSelections, targetSelections))
			{
				return false;
			}
			IEnumerator enumerator = targetSelections.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (((Node)enumerator.Current).Parent == null)
					{
						continue;
					}
					flag = false;
					return flag;
				}
				return true;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return flag;
		}

		public void RemoveBakFileConnection(SPConnection spConn, bool bDeleteTempDB)
		{
			if (spConn.Adapter == null || !(spConn.Adapter is IDBReader) || spConn.Status != ConnectionStatus.Valid)
			{
				return;
			}
			if (bDeleteTempDB)
			{
				try
				{
					IDBReader adapter = (IDBReader)spConn.Adapter;
					using (SqlConnection sqlConnection = new SqlConnection(adapter.ConnectionString))
					{
						sqlConnection.Open();
						(new SqlCommand("USE Master", sqlConnection)).ExecuteNonQuery();
						string str = string.Format("IF EXISTS (SELECT * FROM sys.databases where name = '{0}') DROP DATABASE [{0}]", adapter.Database);
						(new SqlCommand(str, sqlConnection)).ExecuteNonQuery();
						sqlConnection.Close();
					}
					this.RemoveOpenBackupsEntry(spConn);
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					GlobalServices.ErrorHandler.HandleException("Unable to remove temporary database", string.Concat("The connection will be removed from Selective Restore Manager, however the database will still exist in SQL Server. The reason the database could not be deleted is: ", exception.Message), exception, ErrorIcon.Information);
				}
			}
		}

		public void RemoveMdfFileConnection(SPConnection spConn)
		{
			if (spConn.Adapter != null && spConn.Adapter is IDBReader)
			{
				ConnectionStatus status = spConn.Status;
			}
		}

		public void RemoveOpenBackupsEntry(SPConnection spConn)
		{
			DatabaseSettings.OpenedBackups.Remove(spConn.DisplayUrl.ToUpper());
			DatabaseSettings.SaveOpenedBackups();
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			DisconnectAction disconnectAction = new DisconnectAction();
			if (disconnectAction.AppliesTo(source, target) && disconnectAction.Configure(ref source, ref target) == ConfigurationResult.Run)
			{
				disconnectAction.Run(source, target);
			}
			foreach (SPConnection sPConnection in target)
			{
				if (sPConnection.IsBackupConnection)
				{
					if (sPConnection.BackupType == SPConnection.BackupConnectionType.Bak)
					{
						this.RemoveBakFileConnection(sPConnection, this.DeleteTempDBs);
					}
					else if (sPConnection.BackupType == SPConnection.BackupConnectionType.Mdf)
					{
						this.RemoveMdfFileConnection(sPConnection);
					}
				}
				Metalogix.Explorer.Settings.ActiveConnections.Remove(sPConnection);
				sPConnection.Close();
			}
		}

		protected override void SetServerAndAdapterDetailsForBugReportingTool(IXMLAbleList target)
		{
		}
	}
}