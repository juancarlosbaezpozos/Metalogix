using Metalogix.Explorer;
using Metalogix.ExternalConnections;
using Metalogix.Permissions;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Security;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint.ExternalConnections
{
	public class NintexExternalConnection : ExternalConnection
	{
		private string m_sServer = null;

		private string m_sDatabase = null;

		private SecureString m_sConnectionString = new SecureString();

		private bool m_bIsConfigDB;

		public SecureString ConnectionString
		{
			get
			{
				SecureString mSConnectionString;
				if (this.m_sConnectionString.Length == 0)
				{
					if ((string.IsNullOrEmpty(this.m_sServer) ? true : string.IsNullOrEmpty(this.m_sDatabase)))
					{
						mSConnectionString = null;
						return mSConnectionString;
					}
					if ((!this.m_sServer.ToUpper().Contains("MICROSOFT##SSEE") ? false : this.m_sServer != "\\\\.\\pipe\\MSSQL$MICROSOFT##SSEE\\sql\\query"))
					{
						this.m_sServer = "\\\\.\\pipe\\MSSQL$MICROSOFT##SSEE\\sql\\query";
					}
					string str = null;
					if (!base.Credentials.IsDefault)
					{
						object[] userName = new object[] { base.Credentials.UserName, base.Credentials.Password.ToInsecureString(), this.m_sServer, this.Database, Metalogix.ExternalConnections.Utils.SQLQueryTimeoutTime };
						str = string.Format("user id={0};password={1};server={2};database={3};connection timeout={4}", userName);
					}
					else
					{
						str = string.Format("Integrated Security=true;server={0};database={1};connection timeout={2}", this.m_sServer, this.Database, Metalogix.ExternalConnections.Utils.SQLQueryTimeoutTime);
					}
					char[] charArray = str.ToCharArray();
					for (int i = 0; i < (int)charArray.Length; i++)
					{
						char chr = charArray[i];
						this.m_sConnectionString.AppendChar(chr);
					}
				}
				mSConnectionString = this.m_sConnectionString;
				return mSConnectionString;
			}
		}

		public string Database
		{
			get
			{
				return this.m_sDatabase;
			}
			set
			{
				this.m_sDatabase = value;
				this.m_sConnectionString.Clear();
				this.Status = ConnectionStatus.NotChecked;
			}
		}

		public bool IsConfigDB
		{
			get
			{
				return this.m_bIsConfigDB;
			}
		}

		public string Server
		{
			get
			{
				return this.m_sServer;
			}
			set
			{
				this.m_sServer = value;
				this.m_sConnectionString.Clear();
				this.Status = ConnectionStatus.NotChecked;
			}
		}

		public NintexExternalConnection()
		{
			base.Credentials = Metalogix.Permissions.Credentials.DefaultCredentials;
		}

		public NintexExternalConnection(XmlNode ndNintexExternalConnection)
		{
			this.FromXml(ndNintexExternalConnection);
		}

		public NintexExternalConnection(string sServer, string sDatabase, Metalogix.Permissions.Credentials credentials)
		{
			this.m_sServer = sServer;
			this.m_sDatabase = sDatabase;
			base.Credentials = credentials;
			this.Status = ConnectionStatus.Valid;
		}

		public override void CheckConnection()
		{
			this.CheckConnection(this.ConnectionString.ToInsecureString());
		}

		public void CheckConnection(string connectionString)
		{
			string str;
			try
			{
				if (connectionString == null)
				{
					this.Status = ConnectionStatus.Invalid;
					throw new Exception("Connection string is null, this connection cannot be created.");
				}
				SqlConnection sqlConnection = new SqlConnection(connectionString);
				try
				{
					if (!Metalogix.ExternalConnections.Utils.IsNintexDatabase(sqlConnection, out this.m_bIsConfigDB, out str))
					{
						throw new Exception(str);
					}
					this.Status = ConnectionStatus.Valid;
				}
				finally
				{
					if (sqlConnection != null)
					{
						((IDisposable)sqlConnection).Dispose();
					}
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				this.Status = ConnectionStatus.Invalid;
				throw new Exception(exception.Message);
			}
		}

		private string ConstructNintexSQLQueryFromList(List<string> WorkflowInstanceIDList)
		{
			string str = "select * from WorkflowInstance wi where wi.WorkflowInstanceID in (";
			int num = 0;
			foreach (string workflowInstanceIDList in WorkflowInstanceIDList)
			{
				if (num > 0)
				{
					str = string.Concat(str, ",");
				}
				str = string.Concat(str, "'", workflowInstanceIDList.ToUpper(), "'");
				num++;
			}
			return string.Concat(str, ")");
		}

		public override void FromXml(XmlNode ndExternalConnection)
		{
			base.FromXml(ndExternalConnection);
			this.m_sServer = ndExternalConnection.Attributes["Server"].Value;
			this.m_sDatabase = ndExternalConnection.Attributes["Database"].Value;
			this.Status = ConnectionStatus.NotChecked;
		}

		private long GetNextIDForTable(SqlConnection conn, SqlTransaction transaction, string sTableName, string sColumnName)
		{
			string[] strArrays = new string[] { "select TOP 1 ", sColumnName, " from ", sTableName, " order by ", sColumnName, " desc" };
			SqlCommand sqlCommand = new SqlCommand(string.Concat(strArrays), conn, transaction);
			long num = Convert.ToInt64(sqlCommand.ExecuteScalar());
			return (num < (long)0 ? (long)1 : num);
		}

		public string GetNintexWorkflowAssociationData(string sWorkflowAssociationID)
		{
			string str;
			SqlConnection sqlConnection = null;
			try
			{
				try
				{
					string str1 = "select * from PublishedWorkflows w where WorkflowId = @WorkflowAssociationID";
					DataTable dataTable = new DataTable("WorkflowAssociation");
					SqlConnection sqlConnection1 = new SqlConnection(this.ConnectionString.ToInsecureString());
					sqlConnection = sqlConnection1;
					SqlConnection sqlConnection2 = sqlConnection1;
					try
					{
						sqlConnection.Open();
						SqlCommand sqlCommand = Metalogix.ExternalConnections.Utils.GetSqlCommand(str1, sqlConnection);
						sqlCommand.Parameters.Add(new SqlParameter("WorkflowAssociationID", sWorkflowAssociationID));
						(new SqlDataAdapter(sqlCommand)).Fill(dataTable);
					}
					finally
					{
						if (sqlConnection2 != null)
						{
							((IDisposable)sqlConnection2).Dispose();
						}
					}
					if (dataTable.Rows.Count != 0)
					{
						StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
						XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
						xmlTextWriter.WriteStartElement("WorkflowAssociations");
						foreach (DataRow row in dataTable.Rows)
						{
							this.WriteNintexWorkflowAssociationXML(row, xmlTextWriter);
						}
						xmlTextWriter.WriteEndElement();
						str = stringWriter.ToString();
					}
					else
					{
						str = null;
					}
				}
				catch (Exception exception)
				{
					throw exception;
				}
			}
			finally
			{
				if (sqlConnection != null)
				{
					sqlConnection.Close();
				}
			}
			return str;
		}

		public string GetNintexWorkflowInstanceData(List<string> WorkflowInstanceIDList, bool bIs2010)
		{
			string str;
			SqlConnection sqlConnection = null;
			try
			{
				try
				{
					string str1 = this.ConstructNintexSQLQueryFromList(WorkflowInstanceIDList);
					DataTable dataTable = new DataTable("WorkflowInstance");
					SqlConnection sqlConnection1 = new SqlConnection(this.ConnectionString.ToInsecureString());
					sqlConnection = sqlConnection1;
					SqlConnection sqlConnection2 = sqlConnection1;
					try
					{
						sqlConnection.Open();
						SqlCommand sqlCommand = Metalogix.ExternalConnections.Utils.GetSqlCommand(str1, sqlConnection);
						(new SqlDataAdapter(sqlCommand)).Fill(dataTable);
					}
					finally
					{
						if (sqlConnection2 != null)
						{
							((IDisposable)sqlConnection2).Dispose();
						}
					}
					if (dataTable.Rows.Count != 0)
					{
						StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
						XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
						xmlTextWriter.WriteStartElement("WorkflowInstanceCollection");
						foreach (DataRow row in dataTable.Rows)
						{
							this.WriteXmlNintexWorkflowInstance(row, xmlTextWriter, bIs2010);
						}
						xmlTextWriter.WriteEndElement();
						str = stringWriter.ToString();
					}
					else
					{
						str = null;
					}
				}
				catch (Exception exception)
				{
					throw exception;
				}
			}
			finally
			{
				if (sqlConnection != null)
				{
					sqlConnection.Close();
				}
			}
			return str;
		}

		public void SetNintexStorageData(string sSiteCollectionID)
		{
			SqlConnection sqlConnection = null;
			try
			{
				string str = "select * from Storage where SiteID = @SiteCollectionID";
				DataTable dataTable = new DataTable("StorageEntries");
				SqlConnection sqlConnection1 = new SqlConnection(this.ConnectionString.ToInsecureString());
				sqlConnection = sqlConnection1;
				SqlConnection sqlConnection2 = sqlConnection1;
				try
				{
					sqlConnection.Open();
					SqlCommand sqlCommand = Metalogix.ExternalConnections.Utils.GetSqlCommand(str, sqlConnection);
					sqlCommand.Parameters.Add(new SqlParameter("SiteCollectionID", (object)(new Guid(sSiteCollectionID))));
					(new SqlDataAdapter(sqlCommand)).Fill(dataTable);
					if (dataTable.Rows.Count == 0)
					{
						SqlCommand sqlCommand1 = new SqlCommand("Insert Storage (DatabaseID, SiteID)\nVALUES(1, @SiteCollectionID)", sqlConnection);
						sqlCommand1.Parameters.Add(new SqlParameter("SiteCollectionID", (object)(new Guid(sSiteCollectionID))));
						sqlCommand1.ExecuteNonQuery();
					}
				}
				finally
				{
					if (sqlConnection2 != null)
					{
						((IDisposable)sqlConnection2).Dispose();
					}
				}
			}
			finally
			{
				if (sqlConnection != null)
				{
					sqlConnection.Close();
				}
			}
		}

		public string SetNintexWorkflowAssociationData(string sWorkflowAssociationXml)
		{
			SqlConnection sqlConnection = null;
			try
			{
				try
				{
					XmlNode firstChild = XmlUtility.StringToXmlNode(sWorkflowAssociationXml).FirstChild;
					SqlConnection sqlConnection1 = new SqlConnection(this.ConnectionString.ToInsecureString());
					sqlConnection = sqlConnection1;
					SqlConnection sqlConnection2 = sqlConnection1;
					try
					{
						sqlConnection.Open();
						SqlCommand sqlCommand = new SqlCommand("Insert PublishedWorkflows (WebApplicationId, SiteId, WebId, ListId, Version, WorkflowId, WorkflowName, WorkflowType, PublishTime, Author)\nVALUES(@WebApplicationId, @SiteId, @WebId,@ListId, @Version, @WorkflowId, @WorkflowName, @WorkflowType, @PublishTime, @Author)", sqlConnection);
						sqlCommand.Parameters.Add(new SqlParameter("WebApplicationId", firstChild.Attributes["WebApplicationId"].Value));
						sqlCommand.Parameters.Add(new SqlParameter("SiteId", firstChild.Attributes["SiteId"].Value));
						sqlCommand.Parameters.Add(new SqlParameter("WebId", firstChild.Attributes["WebId"].Value));
						sqlCommand.Parameters.Add(new SqlParameter("ListId", firstChild.Attributes["ListId"].Value));
						sqlCommand.Parameters.Add(new SqlParameter("Version", firstChild.Attributes["Version"].Value));
						sqlCommand.Parameters.Add(new SqlParameter("WorkflowId", firstChild.Attributes["WorkflowId"].Value));
						sqlCommand.Parameters.Add(new SqlParameter("WorkflowName", firstChild.Attributes["WorkflowName"].Value));
						sqlCommand.Parameters.Add(new SqlParameter("WorkflowType", firstChild.Attributes["WorkflowType"].Value));
						sqlCommand.Parameters.Add(new SqlParameter("Author", firstChild.Attributes["Author"].Value));
						if (string.IsNullOrEmpty(firstChild.Attributes["PublishTime"].Value))
						{
							sqlCommand.Parameters.Add(new SqlParameter("PublishTime", DBNull.Value));
						}
						else
						{
							sqlCommand.Parameters.Add(new SqlParameter("PublishTime", (object)Convert.ToDateTime(firstChild.Attributes["PublishTime"].Value)));
						}
						sqlCommand.ExecuteNonQuery();
					}
					finally
					{
						if (sqlConnection2 != null)
						{
							((IDisposable)sqlConnection2).Dispose();
						}
					}
				}
				catch (Exception exception)
				{
					throw exception;
				}
			}
			finally
			{
				if (sqlConnection != null)
				{
					sqlConnection.Close();
				}
			}
			return null;
		}

		public string SetNintexWorkflowInstanceData(string sNintexWorkflowDatabaseXML, bool bIs2010)
		{
			SqlCommand sqlCommand;
			SqlConnection sqlConnection = null;
			SqlTransaction sqlTransaction = null;
			try
			{
				XmlNode xmlNode = XmlUtility.StringToXmlNode(sNintexWorkflowDatabaseXML);
				SqlConnection sqlConnection1 = new SqlConnection(this.ConnectionString.ToInsecureString());
				sqlConnection = sqlConnection1;
				SqlConnection sqlConnection2 = sqlConnection1;
				try
				{
					try
					{
						sqlConnection.Open();
						sqlTransaction = sqlConnection.BeginTransaction();
						string[] strArrays = new string[] { "Insert WorkflowInstance (XOML, SiteID, ListID, ItemID, WebID, StartTime, WorkflowInstanceID, WorkflowData, WorkflowInitiator, WorkflowName, WorkflowID, State, Rules, ConfigXml, ExpectedDuration, TaskListID, WebApplicationID, HistoryListID, ItemDeleted", null, null, null, null };
						strArrays[1] = (bIs2010 ? ", Category, ContextData" : "");
						strArrays[2] = ")\nVALUES(@XOML, @SiteID, @ListID,@ItemID, @WebID, @StartTime, @WorkflowInstanceID, @WorkflowData, @WorkflowInitiator, @WorkflowName, @WorkflowID, @State, @Rules, @ConfigXml, @ExpectedDuration, @TaskListID, @WebApplicationID, @HistoryListID, @ItemDeleted";
						strArrays[3] = (bIs2010 ? ", @Category, @ContextData" : "");
						strArrays[4] = ")";
						SqlCommand value = new SqlCommand(string.Concat(strArrays), sqlConnection, sqlTransaction);
						value.Parameters.Add(new SqlParameter("XOML", xmlNode.Attributes["XOML"].Value));
						value.Parameters.Add(new SqlParameter("SiteID", xmlNode.Attributes["SiteID"].Value));
						value.Parameters.Add(new SqlParameter("ListID", xmlNode.Attributes["ListID"].Value));
						value.Parameters.Add(new SqlParameter("ItemID", xmlNode.Attributes["ItemID"].Value));
						value.Parameters.Add(new SqlParameter("WebID", xmlNode.Attributes["WebID"].Value));
						value.Parameters.Add(new SqlParameter("WorkflowInstanceID", xmlNode.Attributes["WorkflowInstanceID"].Value));
						if (string.IsNullOrEmpty(xmlNode.Attributes["StartTime"].Value))
						{
							value.Parameters.Add(new SqlParameter("StartTime", DBNull.Value));
						}
						else
						{
							value.Parameters.Add(new SqlParameter("StartTime", (object)Convert.ToDateTime(xmlNode.Attributes["StartTime"].Value)));
						}
						byte[] numArray = null;
						if ((xmlNode.Attributes["WorkflowData"] == null ? false : !string.IsNullOrEmpty(xmlNode.Attributes["WorkflowData"].Value)))
						{
							numArray = Convert.FromBase64String(xmlNode.Attributes["WorkflowData"].Value);
						}
						if (numArray == null)
						{
							value.Parameters.Add(new SqlParameter("WorkflowData", SqlDbType.Image));
							value.Parameters["WorkflowData"].Value = DBNull.Value;
						}
						else
						{
							value.Parameters.Add(new SqlParameter("WorkflowData", numArray));
						}
						value.Parameters.Add(new SqlParameter("WorkflowInitiator", xmlNode.Attributes["WorkflowInitiator"].Value));
						value.Parameters.Add(new SqlParameter("WorkflowName", xmlNode.Attributes["WorkflowName"].Value));
						value.Parameters.Add(new SqlParameter("WorkflowID", xmlNode.Attributes["WorkflowID"].Value));
						value.Parameters.Add(new SqlParameter("State", xmlNode.Attributes["State"].Value));
						value.Parameters.Add(new SqlParameter("Rules", xmlNode.Attributes["Rules"].Value));
						value.Parameters.Add(new SqlParameter("ConfigXml", xmlNode.Attributes["ConfigXml"].Value));
						value.Parameters.Add(new SqlParameter("ExpectedDuration", xmlNode.Attributes["ExpectedDuration"].Value));
						value.Parameters.Add(new SqlParameter("TaskListID", xmlNode.Attributes["TaskListID"].Value));
						value.Parameters.Add(new SqlParameter("WebApplicationID", xmlNode.Attributes["WebApplicationID"].Value));
						value.Parameters.Add(new SqlParameter("HistoryListID", xmlNode.Attributes["HistoryListID"].Value));
						value.Parameters.Add(new SqlParameter("ItemDeleted", xmlNode.Attributes["ItemDeleted"].Value));
						if (bIs2010)
						{
							if (xmlNode.Attributes["Category"] == null)
							{
								value.Parameters.Add(new SqlParameter("Category", "List"));
							}
							else
							{
								value.Parameters.Add(new SqlParameter("Category", xmlNode.Attributes["Category"].Value));
							}
							if (xmlNode.Attributes["ContextData"] == null)
							{
								value.Parameters.Add(new SqlParameter("ContextData", DBNull.Value));
							}
							else
							{
								value.Parameters.Add(new SqlParameter("ContextData", xmlNode.Attributes["ContextData"].Value));
							}
						}
						value.ExecuteNonQuery();
						long nextIDForTable = this.GetNextIDForTable(sqlConnection, sqlTransaction, "WorkflowInstance", "InstanceID");
						if (bIs2010)
						{
							Dictionary<string, string> strs = new Dictionary<string, string>();
							foreach (XmlNode xmlNodes in xmlNode.SelectNodes(".//DataStoreKey"))
							{
								sqlCommand = new SqlCommand("Insert DataStoreKeys (KeyName)\nVALUES(@KeyName); Select SCOPE_IDENTITY() as Id", sqlConnection, sqlTransaction);
								sqlCommand.Parameters.Add(new SqlParameter("KeyName", xmlNodes.Attributes["KeyName"].Value));
								string str = sqlCommand.ExecuteScalar().ToString();
								if (!string.IsNullOrEmpty(str))
								{
									strs.Add(xmlNodes.Attributes["KeyId"].Value, str);
								}
							}
							foreach (XmlNode xmlNodes1 in xmlNode.SelectNodes(".//DataStore"))
							{
								sqlCommand = new SqlCommand("Insert DataStore (InstanceID, KeyId, Data)\nVALUES(@InstanceID, @KeyId, @Data)", sqlConnection, sqlTransaction);
								sqlCommand.Parameters.Add(new SqlParameter("InstanceId", (object)nextIDForTable));
								if (!strs.ContainsKey(xmlNodes1.Attributes["KeyId"].Value))
								{
									sqlCommand.Parameters.Add(new SqlParameter("KeyId", xmlNodes1.Attributes["KeyId"].Value));
								}
								else
								{
									sqlCommand.Parameters.Add(new SqlParameter("KeyId", strs[xmlNodes1.Attributes["KeyId"].Value]));
								}
								byte[] numArray1 = null;
								if ((xmlNodes1.Attributes["Data"] == null ? false : !string.IsNullOrEmpty(xmlNodes1.Attributes["Data"].Value)))
								{
									numArray1 = Convert.FromBase64String(xmlNodes1.Attributes["Data"].Value);
								}
								if (numArray1 == null)
								{
									sqlCommand.Parameters.Add(new SqlParameter("Data", SqlDbType.Image));
									sqlCommand.Parameters["Data"].Value = DBNull.Value;
								}
								else
								{
									sqlCommand.Parameters.Add(new SqlParameter("Data", numArray1));
								}
								sqlCommand.ExecuteNonQuery();
							}
						}
						Dictionary<string, string> strs1 = new Dictionary<string, string>();
						foreach (XmlNode xmlNodes2 in xmlNode.SelectNodes(".//ConfiguredOutcome"))
						{
							sqlCommand = new SqlCommand("if not exists (select * from ConfiguredOutcomes where Name = @Name)\nInsert ConfiguredOutcomes (Name)\nVALUES(@Name);Select Id from ConfiguredOutcomes where Name = @Name", sqlConnection, sqlTransaction);
							sqlCommand.Parameters.Add(new SqlParameter("Name", xmlNodes2.Attributes["Name"].Value));
							string str1 = sqlCommand.ExecuteScalar().ToString();
							strs1.Add(xmlNodes2.Attributes["Id"].Value, str1);
						}
						foreach (XmlNode xmlNodes3 in xmlNode.SelectNodes(".//WorkflowProgress"))
						{
							SqlCommand sqlCommand1 = new SqlCommand("Insert WorkflowProgress (InstanceID, ActivityID, WorkflowData, ActivityComplete, TimeStamp, SequenceID, CurrentActivityTitle, ExpectedDuration)\nVALUES(@InstanceID, @ActivityID, @WorkflowData,@ActivityComplete, @TimeStamp, @SequenceID, @CurrentActivityTitle, @ExpectedDuration)", sqlConnection, sqlTransaction);
							sqlCommand1.Parameters.Add(new SqlParameter("InstanceID", (object)nextIDForTable));
							sqlCommand1.Parameters.Add(new SqlParameter("ActivityID", xmlNodes3.Attributes["ActivityID"].Value));
							sqlCommand1.Parameters.Add(new SqlParameter("ActivityComplete", xmlNodes3.Attributes["ActivityComplete"].Value));
							if (numArray == null)
							{
								sqlCommand1.Parameters.Add(new SqlParameter("WorkflowData", SqlDbType.Image));
								sqlCommand1.Parameters["WorkflowData"].Value = DBNull.Value;
							}
							else
							{
								sqlCommand1.Parameters.Add(new SqlParameter("WorkflowData", numArray));
							}
							if (string.IsNullOrEmpty(xmlNodes3.Attributes["TimeStamp"].Value))
							{
								sqlCommand1.Parameters.Add(new SqlParameter("TimeStamp", DBNull.Value));
							}
							else
							{
								sqlCommand1.Parameters.Add(new SqlParameter("TimeStamp", (object)Convert.ToDateTime(xmlNodes3.Attributes["TimeStamp"].Value)));
							}
							sqlCommand1.Parameters.Add(new SqlParameter("SequenceID", xmlNodes3.Attributes["SequenceID"].Value));
							sqlCommand1.Parameters.Add(new SqlParameter("CurrentActivityTitle", xmlNodes3.Attributes["CurrentActivityTitle"].Value));
							sqlCommand1.Parameters.Add(new SqlParameter("ExpectedDuration", xmlNodes3.Attributes["ExpectedDuration"].Value));
							sqlCommand1.ExecuteNonQuery();
							foreach (XmlNode xmlNodes4 in xmlNodes3.SelectNodes("./HumanWorkflow"))
							{
								string str2 = string.Format("INSERT HumanWorkflow (ApprovalType, EntryTime, ExitTime, Outcome, WorkflowTaskID, WorkflowProgressID, ApprovalData, TaskType, CustomOutcome, OutcomeAchieved{0}) VALUES (@ApprovalType, @EntryTime, @ExitTime, @Outcome, @WorkflowTaskID, @WorkflowProgressID, @ApprovalData, @TaskType, @CustomOutcome, @OutcomeAchieved{1})", (bIs2010 ? ", HistoricId" : string.Empty), (bIs2010 ? ", @HistoricId" : string.Empty));
								SqlCommand sqlCommand2 = new SqlCommand(str2, sqlConnection, sqlTransaction);
								sqlCommand2.Parameters.Add(new SqlParameter("ApprovalType", xmlNodes4.Attributes["ApprovalType"].Value));
								sqlCommand2.Parameters.Add(new SqlParameter("Outcome", xmlNodes4.Attributes["Outcome"].Value));
								sqlCommand2.Parameters.Add(new SqlParameter("WorkflowTaskID", xmlNodes4.Attributes["WorkflowTaskID"].Value));
								sqlCommand2.Parameters.Add(new SqlParameter("WorkflowProgressID", (object)this.GetNextIDForTable(sqlConnection, sqlTransaction, "WorkflowProgress", "WorkflowProgressID")));
								sqlCommand2.Parameters.Add(new SqlParameter("ApprovalData", xmlNodes4.Attributes["ApprovalData"].Value));
								sqlCommand2.Parameters.Add(new SqlParameter("TaskType", xmlNodes4.Attributes["TaskType"].Value));
								if (!strs1.ContainsKey(xmlNodes4.Attributes["CustomOutcome"].Value))
								{
									sqlCommand2.Parameters.Add(new SqlParameter("CustomOutcome", xmlNodes4.Attributes["CustomOutcome"].Value));
								}
								else
								{
									sqlCommand2.Parameters.Add(new SqlParameter("CustomOutcome", strs1[xmlNodes4.Attributes["CustomOutcome"].Value]));
								}
								sqlCommand2.Parameters.Add(new SqlParameter("OutcomeAchieved", xmlNodes4.Attributes["OutcomeAchieved"].Value));
								if (bIs2010)
								{
									sqlCommand2.Parameters.Add(new SqlParameter("HistoricId", (xmlNodes4.Attributes["HistoricId"] != null ? xmlNodes4.Attributes["HistoricId"].Value : DBNull.Value.ToString())));
								}
								if (string.IsNullOrEmpty(xmlNodes4.Attributes["EntryTime"].Value))
								{
									sqlCommand2.Parameters.Add(new SqlParameter("EntryTime", DBNull.Value));
								}
								else
								{
									sqlCommand2.Parameters.Add(new SqlParameter("EntryTime", (object)Convert.ToDateTime(xmlNodes4.Attributes["EntryTime"].Value)));
								}
								if (string.IsNullOrEmpty(xmlNodes4.Attributes["ExitTime"].Value))
								{
									sqlCommand2.Parameters.Add(new SqlParameter("ExitTime", DBNull.Value));
								}
								else
								{
									sqlCommand2.Parameters.Add(new SqlParameter("ExitTime", (object)Convert.ToDateTime(xmlNodes4.Attributes["ExitTime"].Value)));
								}
								sqlCommand2.ExecuteNonQuery();
								foreach (XmlNode xmlNodes5 in xmlNodes4.SelectNodes("./HumanWorkflowApprover"))
								{
									str2 = string.Format("INSERT HumanWorkflowApprovers (HumanWorkflowID, Username, EmailAddress, EntryTime, ExitTime, Comments, Outcome, AllowDelegation, IsDomainGroup, IsSPGroup, SPTaskID, TaskDeleted, NeedsProcessing, NoLongerRequiredVariablesReplaced, NoLongerRequiredMessageTemplate, ApprovalRequiredMessageTemplate, TaskLocked, CustomOutcome{0}) VALUES (@HumanWorkflowID, @Username, @EmailAddress, @EntryTime, @ExitTime, @Comments, @Outcome, @AllowDelegation, @IsDomainGroup, @IsSPGroup, @SPTaskID, @TaskDeleted, @NeedsProcessing, @NoLongerRequiredVariablesReplaced, @NoLongerRequiredMessageTemplate, @ApprovalRequiredMessageTemplate, @TaskLocked, @CustomOutcome{1}); SELECT SCOPE_IDENTITY()", (bIs2010 ? ", HistoricId" : string.Empty), (bIs2010 ? ", @HistoricId" : string.Empty));
									SqlCommand sqlCommand3 = new SqlCommand(str2, sqlConnection, sqlTransaction);
									sqlCommand3.Parameters.Add(new SqlParameter("HumanWorkflowID", (object)this.GetNextIDForTable(sqlConnection, sqlTransaction, "HumanWorkflow", "HumanWorkflowID")));
									sqlCommand3.Parameters.Add(new SqlParameter("Username", xmlNodes5.Attributes["Username"].Value));
									sqlCommand3.Parameters.Add(new SqlParameter("EmailAddress", xmlNodes5.Attributes["EmailAddress"].Value));
									sqlCommand3.Parameters.Add(new SqlParameter("Comments", xmlNodes5.Attributes["Comments"].Value));
									sqlCommand3.Parameters.Add(new SqlParameter("Outcome", xmlNodes5.Attributes["Outcome"].Value));
									sqlCommand3.Parameters.Add(new SqlParameter("AllowDelegation", xmlNodes5.Attributes["AllowDelegation"].Value));
									sqlCommand3.Parameters.Add(new SqlParameter("IsDomainGroup", xmlNodes5.Attributes["IsDomainGroup"].Value));
									sqlCommand3.Parameters.Add(new SqlParameter("IsSPGroup", xmlNodes5.Attributes["IsSPGroup"].Value));
									sqlCommand3.Parameters.Add(new SqlParameter("SPTaskID", xmlNodes5.Attributes["SPTaskID"].Value));
									sqlCommand3.Parameters.Add(new SqlParameter("TaskDeleted", xmlNodes5.Attributes["TaskDeleted"].Value));
									sqlCommand3.Parameters.Add(new SqlParameter("NeedsProcessing", xmlNodes5.Attributes["NeedsProcessing"].Value));
									sqlCommand3.Parameters.Add(new SqlParameter("NoLongerRequiredVariablesReplaced", xmlNodes5.Attributes["NoLongerRequiredVariablesReplaced"].Value));
									sqlCommand3.Parameters.Add(new SqlParameter("NoLongerRequiredMessageTemplate", xmlNodes5.Attributes["NoLongerRequiredMessageTemplate"].Value));
									sqlCommand3.Parameters.Add(new SqlParameter("ApprovalRequiredMessageTemplate", xmlNodes5.Attributes["ApprovalRequiredMessageTemplate"].Value));
									if (bIs2010)
									{
										sqlCommand3.Parameters.Add(new SqlParameter("HistoricId", (xmlNodes4.Attributes["HistoricId"] != null ? xmlNodes4.Attributes["HistoricId"].Value : DBNull.Value.ToString())));
									}
									if (!strs1.ContainsKey(xmlNodes5.Attributes["CustomOutcome"].Value))
									{
										sqlCommand3.Parameters.Add(new SqlParameter("CustomOutcome", xmlNodes5.Attributes["CustomOutcome"].Value));
									}
									else
									{
										sqlCommand3.Parameters.Add(new SqlParameter("CustomOutcome", strs1[xmlNodes5.Attributes["CustomOutcome"].Value]));
									}
									if (string.IsNullOrEmpty(xmlNodes5.Attributes["EntryTime"].Value))
									{
										sqlCommand3.Parameters.Add(new SqlParameter("EntryTime", DBNull.Value));
									}
									else
									{
										sqlCommand3.Parameters.Add(new SqlParameter("EntryTime", (object)Convert.ToDateTime(xmlNodes5.Attributes["EntryTime"].Value)));
									}
									if (string.IsNullOrEmpty(xmlNodes5.Attributes["ExitTime"].Value))
									{
										sqlCommand3.Parameters.Add(new SqlParameter("ExitTime", DBNull.Value));
									}
									else
									{
										sqlCommand3.Parameters.Add(new SqlParameter("ExitTime", (object)Convert.ToDateTime(xmlNodes5.Attributes["ExitTime"].Value)));
									}
									if (string.IsNullOrEmpty(xmlNodes5.Attributes["TaskLocked"].Value))
									{
										sqlCommand3.Parameters.Add(new SqlParameter("TaskLocked", DBNull.Value));
									}
									else
									{
										sqlCommand3.Parameters.Add(new SqlParameter("TaskLocked", (object)Convert.ToDateTime(xmlNodes5.Attributes["TaskLocked"].Value)));
									}
									string str3 = sqlCommand3.ExecuteScalar().ToString();
									foreach (XmlNode xmlNodes6 in xmlNodes5.SelectNodes("./DelegationHistory"))
									{
										SqlCommand sqlCommand4 = new SqlCommand("Insert DelegationHistory (Username, Delegate, ApproverID, DatabaseID, DelegationDate, Comment)\nVALUES(@Username, @Delegate, @ApproverID, @DatabaseID, @DelegationDate, @Comment)", sqlConnection, sqlTransaction);
										sqlCommand4.Parameters.Add(new SqlParameter("Username", xmlNodes6.Attributes["Username"].Value));
										sqlCommand4.Parameters.Add(new SqlParameter("Delegate", xmlNodes6.Attributes["Delegate"].Value));
										sqlCommand4.Parameters.Add(new SqlParameter("ApproverID", str3));
										sqlCommand4.Parameters.Add(new SqlParameter("DatabaseID", xmlNodes6.Attributes["DatabaseID"].Value));
										if (string.IsNullOrEmpty(xmlNodes6.Attributes["DelegationDate"].Value))
										{
											sqlCommand4.Parameters.Add(new SqlParameter("DelegationDate", DBNull.Value));
										}
										else
										{
											sqlCommand4.Parameters.Add(new SqlParameter("DelegationDate", (object)Convert.ToDateTime(xmlNodes6.Attributes["DelegationDate"].Value)));
										}
										sqlCommand4.Parameters.Add(new SqlParameter("Comment", xmlNodes6.Attributes["Comment"].Value));
										sqlCommand4.ExecuteNonQuery();
									}
								}
							}
						}
						sqlTransaction.Commit();
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						sqlTransaction.Rollback();
						throw exception;
					}
				}
				finally
				{
					if (sqlConnection2 != null)
					{
						((IDisposable)sqlConnection2).Dispose();
					}
				}
			}
			finally
			{
				if (sqlConnection != null)
				{
					sqlConnection.Close();
				}
			}
			return null;
		}

		private void WriteApproverData(DataRow dr, XmlWriter xmlWriter)
		{
			SqlConnection sqlConnection = null;
			try
			{
				try
				{
					string str = "select * from HumanWorkflowApprovers where HumanWorkflowID = @HumanWorkflowID";
					DataTable dataTable = new DataTable("WorkflowInstance");
					SqlConnection sqlConnection1 = new SqlConnection(this.ConnectionString.ToInsecureString());
					sqlConnection = sqlConnection1;
					SqlConnection sqlConnection2 = sqlConnection1;
					try
					{
						sqlConnection.Open();
						SqlCommand sqlCommand = Metalogix.ExternalConnections.Utils.GetSqlCommand(str, sqlConnection);
						sqlCommand.Parameters.Add(new SqlParameter("HumanWorkflowID", dr["HumanWorkflowID"].ToString()));
						(new SqlDataAdapter(sqlCommand)).Fill(dataTable);
					}
					finally
					{
						if (sqlConnection2 != null)
						{
							((IDisposable)sqlConnection2).Dispose();
						}
					}
					if (dataTable.Rows.Count != 0)
					{
						foreach (DataRow row in dataTable.Rows)
						{
							this.WriteHumanWorkflowApproverData(row, xmlWriter);
						}
					}
					else
					{
						return;
					}
				}
				catch
				{
					return;
				}
			}
			finally
			{
				if (sqlConnection != null)
				{
					sqlConnection.Close();
				}
			}
		}

		private void WriteConfiguredOutcomesData(DataRow dr, XmlWriter xmlWriter)
		{
			SqlConnection sqlConnection = null;
			try
			{
				try
				{
					string str = "select * from ConfiguredOutcomes where Id = @Id";
					DataTable dataTable = new DataTable("ConfiguredOutcomes");
					SqlConnection sqlConnection1 = new SqlConnection(this.ConnectionString.ToInsecureString());
					sqlConnection = sqlConnection1;
					SqlConnection sqlConnection2 = sqlConnection1;
					try
					{
						sqlConnection.Open();
						SqlCommand sqlCommand = Metalogix.ExternalConnections.Utils.GetSqlCommand(str, sqlConnection);
						sqlCommand.Parameters.Add(new SqlParameter("Id", dr["CustomOutcome"].ToString()));
						(new SqlDataAdapter(sqlCommand)).Fill(dataTable);
					}
					finally
					{
						if (sqlConnection2 != null)
						{
							((IDisposable)sqlConnection2).Dispose();
						}
					}
					if (dataTable.Rows.Count != 0)
					{
						foreach (DataRow row in dataTable.Rows)
						{
							this.WriteXmlConfiguredOutcomesData(row, xmlWriter);
						}
					}
					else
					{
						return;
					}
				}
				catch
				{
					return;
				}
			}
			finally
			{
				if (sqlConnection != null)
				{
					sqlConnection.Close();
				}
			}
		}

		protected override void WriteConnectionXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteAttributeString("Server", this.Server);
			xmlWriter.WriteAttributeString("Database", this.Database);
		}

		private void WriteDataStoreData(DataRow dr, XmlWriter xmlWriter)
		{
			SqlConnection sqlConnection = null;
			try
			{
				try
				{
					string str = "select * from DataStore where InstanceID = @InstanceID";
					DataTable dataTable = new DataTable("WorkflowInstanceDataStores");
					SqlConnection sqlConnection1 = new SqlConnection(this.ConnectionString.ToInsecureString());
					sqlConnection = sqlConnection1;
					SqlConnection sqlConnection2 = sqlConnection1;
					try
					{
						sqlConnection.Open();
						SqlCommand sqlCommand = Metalogix.ExternalConnections.Utils.GetSqlCommand(str, sqlConnection);
						sqlCommand.Parameters.Add(new SqlParameter("InstanceID", dr["InstanceID"].ToString()));
						(new SqlDataAdapter(sqlCommand)).Fill(dataTable);
					}
					finally
					{
						if (sqlConnection2 != null)
						{
							((IDisposable)sqlConnection2).Dispose();
						}
					}
					if (dataTable.Rows.Count != 0)
					{
						foreach (DataRow row in dataTable.Rows)
						{
							this.WriteXmlDataStoreData(row, xmlWriter);
						}
					}
					else
					{
						return;
					}
				}
				catch
				{
					return;
				}
			}
			finally
			{
				if (sqlConnection != null)
				{
					sqlConnection.Close();
				}
			}
		}

		private void WriteDataStoreKeyData(DataRow dr, XmlWriter xmlWriter)
		{
			SqlConnection sqlConnection = null;
			try
			{
				try
				{
					string str = "select * from DataStoreKeys where KeyId = @KeyId";
					DataTable dataTable = new DataTable("WorkflowInstanceDataStoreKeys");
					SqlConnection sqlConnection1 = new SqlConnection(this.ConnectionString.ToInsecureString());
					sqlConnection = sqlConnection1;
					SqlConnection sqlConnection2 = sqlConnection1;
					try
					{
						sqlConnection.Open();
						SqlCommand sqlCommand = Metalogix.ExternalConnections.Utils.GetSqlCommand(str, sqlConnection);
						sqlCommand.Parameters.Add(new SqlParameter("KeyId", dr["KeyId"].ToString()));
						(new SqlDataAdapter(sqlCommand)).Fill(dataTable);
					}
					finally
					{
						if (sqlConnection2 != null)
						{
							((IDisposable)sqlConnection2).Dispose();
						}
					}
					if (dataTable.Rows.Count != 0)
					{
						foreach (DataRow row in dataTable.Rows)
						{
							this.WriteXmlDataStoreKeyData(row, xmlWriter);
						}
					}
					else
					{
						return;
					}
				}
				catch
				{
					return;
				}
			}
			finally
			{
				if (sqlConnection != null)
				{
					sqlConnection.Close();
				}
			}
		}

		private void WriteDelegationHistoryData(DataRow dr, XmlWriter xmlWriter)
		{
			SqlConnection sqlConnection = null;
			try
			{
				try
				{
					string str = "select * from DelegationHistory where ApproverID = @ApproverID";
					DataTable dataTable = new DataTable("WorkflowInstance");
					SqlConnection sqlConnection1 = new SqlConnection(this.ConnectionString.ToInsecureString());
					sqlConnection = sqlConnection1;
					SqlConnection sqlConnection2 = sqlConnection1;
					try
					{
						sqlConnection.Open();
						SqlCommand sqlCommand = Metalogix.ExternalConnections.Utils.GetSqlCommand(str, sqlConnection);
						sqlCommand.Parameters.Add(new SqlParameter("ApproverID", dr["ApproverID"].ToString()));
						(new SqlDataAdapter(sqlCommand)).Fill(dataTable);
					}
					finally
					{
						if (sqlConnection2 != null)
						{
							((IDisposable)sqlConnection2).Dispose();
						}
					}
					if (dataTable.Rows.Count != 0)
					{
						foreach (DataRow row in dataTable.Rows)
						{
							this.WriteXmlDelegationHistoryData(row, xmlWriter);
						}
					}
					else
					{
						return;
					}
				}
				catch
				{
					return;
				}
			}
			finally
			{
				if (sqlConnection != null)
				{
					sqlConnection.Close();
				}
			}
		}

		private void WriteHumanWorkflowApproverData(DataRow dr, XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("HumanWorkflowApprover");
			xmlWriter.WriteAttributeString("HumanWorkflowID", dr["HumanWorkflowID"].ToString());
			xmlWriter.WriteAttributeString("Username", dr["Username"].ToString());
			xmlWriter.WriteAttributeString("EmailAddress", dr["EmailAddress"].ToString());
			xmlWriter.WriteAttributeString("EntryTime", dr["EntryTime"].ToString());
			xmlWriter.WriteAttributeString("ApproverID", dr["ApproverID"].ToString());
			xmlWriter.WriteAttributeString("ExitTime", dr["ExitTime"].ToString());
			xmlWriter.WriteAttributeString("Comments", dr["Comments"].ToString());
			xmlWriter.WriteAttributeString("Outcome", dr["Outcome"].ToString());
			xmlWriter.WriteAttributeString("AllowDelegation", dr["AllowDelegation"].ToString());
			xmlWriter.WriteAttributeString("IsDomainGroup", dr["IsDomainGroup"].ToString());
			xmlWriter.WriteAttributeString("IsSPGroup", dr["IsSPGroup"].ToString());
			xmlWriter.WriteAttributeString("SPTaskID", dr["SPTaskID"].ToString());
			xmlWriter.WriteAttributeString("TaskDeleted", dr["TaskDeleted"].ToString());
			xmlWriter.WriteAttributeString("NeedsProcessing", dr["NeedsProcessing"].ToString());
			xmlWriter.WriteAttributeString("NoLongerRequiredVariablesReplaced", dr["NoLongerRequiredVariablesReplaced"].ToString());
			xmlWriter.WriteAttributeString("NoLongerRequiredMessageTemplate", dr["NoLongerRequiredMessageTemplate"].ToString());
			xmlWriter.WriteAttributeString("ApprovalRequiredMessageTemplate", dr["ApprovalRequiredMessageTemplate"].ToString());
			xmlWriter.WriteAttributeString("TaskLocked", dr["TaskLocked"].ToString());
			xmlWriter.WriteAttributeString("CustomOutcome", dr["CustomOutcome"].ToString());
			if (dr.Table.Columns.Contains("HistoricId"))
			{
				xmlWriter.WriteAttributeString("HistoricId", dr["HistoricId"].ToString());
			}
			this.WriteDelegationHistoryData(dr, xmlWriter);
			xmlWriter.WriteEndElement();
		}

		private void WriteHumanWorkflowData(DataRow dr, XmlWriter xmlWriter)
		{
			SqlConnection sqlConnection = null;
			try
			{
				try
				{
					string str = "select * from HumanWorkflow where WorkflowProgressID = @WorkflowProgressID";
					DataTable dataTable = new DataTable("WorkflowInstance");
					SqlConnection sqlConnection1 = new SqlConnection(this.ConnectionString.ToInsecureString());
					sqlConnection = sqlConnection1;
					SqlConnection sqlConnection2 = sqlConnection1;
					try
					{
						sqlConnection.Open();
						SqlCommand sqlCommand = Metalogix.ExternalConnections.Utils.GetSqlCommand(str, sqlConnection);
						sqlCommand.Parameters.Add(new SqlParameter("WorkflowProgressID", dr["WorkflowProgressID"].ToString()));
						(new SqlDataAdapter(sqlCommand)).Fill(dataTable);
					}
					finally
					{
						if (sqlConnection2 != null)
						{
							((IDisposable)sqlConnection2).Dispose();
						}
					}
					if (dataTable.Rows.Count != 0)
					{
						foreach (DataRow row in dataTable.Rows)
						{
							this.WriteXmlHumanWorkflowData(row, xmlWriter);
						}
					}
					else
					{
						return;
					}
				}
				catch
				{
					return;
				}
			}
			finally
			{
				if (sqlConnection != null)
				{
					sqlConnection.Close();
				}
			}
		}

		private void WriteNintexWorkflowAssociationXML(DataRow dr, XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("WorkflowAssociation");
			xmlWriter.WriteAttributeString("ID", dr["Id"].ToString());
			xmlWriter.WriteAttributeString("SiteId", dr["SiteId"].ToString());
			xmlWriter.WriteAttributeString("WebId", dr["WebId"].ToString());
			xmlWriter.WriteAttributeString("WebApplicationId", dr["WebApplicationId"].ToString());
			xmlWriter.WriteAttributeString("ListId", dr["ListId"].ToString());
			xmlWriter.WriteAttributeString("Version", dr["Version"].ToString());
			xmlWriter.WriteAttributeString("WorkflowId", dr["WorkflowId"].ToString());
			xmlWriter.WriteAttributeString("WorkflowName", dr["WorkflowName"].ToString());
			xmlWriter.WriteAttributeString("WorkflowType", dr["WorkflowType"].ToString());
			xmlWriter.WriteAttributeString("PublishTime", dr["PublishTime"].ToString());
			xmlWriter.WriteAttributeString("Author", dr["Author"].ToString());
			xmlWriter.WriteEndElement();
		}

		private void WriteProgressData(DataRow dr, XmlWriter xmlWriter)
		{
			SqlConnection sqlConnection = null;
			try
			{
				try
				{
					string str = "select * from WorkflowProgress where InstanceID = @InstanceID";
					DataTable dataTable = new DataTable("WorkflowInstance");
					SqlConnection sqlConnection1 = new SqlConnection(this.ConnectionString.ToInsecureString());
					sqlConnection = sqlConnection1;
					SqlConnection sqlConnection2 = sqlConnection1;
					try
					{
						sqlConnection.Open();
						SqlCommand sqlCommand = Metalogix.ExternalConnections.Utils.GetSqlCommand(str, sqlConnection);
						sqlCommand.Parameters.Add(new SqlParameter("InstanceID", dr["InstanceID"].ToString()));
						(new SqlDataAdapter(sqlCommand)).Fill(dataTable);
					}
					finally
					{
						if (sqlConnection2 != null)
						{
							((IDisposable)sqlConnection2).Dispose();
						}
					}
					if (dataTable.Rows.Count != 0)
					{
						foreach (DataRow row in dataTable.Rows)
						{
							this.WriteXmlProgressData(row, xmlWriter);
						}
					}
					else
					{
						return;
					}
				}
				catch
				{
					return;
				}
			}
			finally
			{
				if (sqlConnection != null)
				{
					sqlConnection.Close();
				}
			}
		}

		private void WriteXmlConfiguredOutcomesData(DataRow dr, XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("ConfiguredOutcome");
			xmlWriter.WriteAttributeString("Id", dr["Id"].ToString());
			xmlWriter.WriteAttributeString("Name", dr["Name"].ToString());
			xmlWriter.WriteEndElement();
		}

		private void WriteXmlDataStoreData(DataRow dr, XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("DataStore");
			xmlWriter.WriteAttributeString("InstanceId", dr["InstanceID"].ToString());
			xmlWriter.WriteAttributeString("KeyId", dr["KeyId"].ToString());
			xmlWriter.WriteAttributeString("Data", (dr["Data"] is DBNull ? DBNull.Value.ToString() : Convert.ToBase64String((byte[])dr["Data"], Base64FormattingOptions.None)));
			this.WriteDataStoreKeyData(dr, xmlWriter);
			xmlWriter.WriteEndElement();
		}

		private void WriteXmlDataStoreKeyData(DataRow dr, XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("DataStoreKey");
			xmlWriter.WriteAttributeString("KeyId", dr["KeyId"].ToString());
			xmlWriter.WriteAttributeString("KeyName", dr["KeyName"].ToString());
			xmlWriter.WriteEndElement();
		}

		private void WriteXmlDelegationHistoryData(DataRow dr, XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("DelegationHistory");
			xmlWriter.WriteAttributeString("DelegationHistoryID", dr["DelegationHistoryID"].ToString());
			xmlWriter.WriteAttributeString("Username", dr["Username"].ToString());
			xmlWriter.WriteAttributeString("Delegate", dr["Delegate"].ToString());
			xmlWriter.WriteAttributeString("ApproverID", dr["ApproverID"].ToString());
			xmlWriter.WriteAttributeString("DatabaseID", "1");
			xmlWriter.WriteAttributeString("DelegationDate", dr["DelegationDate"].ToString());
			xmlWriter.WriteAttributeString("Comment", dr["Comment"].ToString());
			xmlWriter.WriteEndElement();
		}

		private void WriteXmlHumanWorkflowData(DataRow dr, XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("HumanWorkflow");
			xmlWriter.WriteAttributeString("HumanWorkflowID", dr["HumanWorkflowID"].ToString());
			xmlWriter.WriteAttributeString("ApprovalType", dr["ApprovalType"].ToString());
			xmlWriter.WriteAttributeString("EntryTime", dr["EntryTime"].ToString());
			xmlWriter.WriteAttributeString("ExitTime", dr["ExitTime"].ToString());
			xmlWriter.WriteAttributeString("Outcome", dr["Outcome"].ToString());
			xmlWriter.WriteAttributeString("WorkflowTaskID", dr["WorkflowTaskID"].ToString());
			xmlWriter.WriteAttributeString("ApprovalData", dr["ApprovalData"].ToString());
			xmlWriter.WriteAttributeString("TaskType", dr["TaskType"].ToString());
			xmlWriter.WriteAttributeString("CustomOutcome", dr["CustomOutcome"].ToString());
			xmlWriter.WriteAttributeString("OutcomeAchieved", dr["OutcomeAchieved"].ToString());
			if (dr.Table.Columns.Contains("HistoricId"))
			{
				xmlWriter.WriteAttributeString("HistoricId", dr["HistoricId"].ToString());
			}
			if (!(dr["CustomOutcome"] is DBNull))
			{
				this.WriteConfiguredOutcomesData(dr, xmlWriter);
			}
			this.WriteApproverData(dr, xmlWriter);
			xmlWriter.WriteEndElement();
		}

		private void WriteXmlNintexWorkflowInstance(DataRow dr, XmlWriter xmlWriter, bool bIs2010)
		{
			xmlWriter.WriteStartElement("WorkflowInstance");
			xmlWriter.WriteAttributeString("InstanceID", dr["InstanceID"].ToString());
			xmlWriter.WriteAttributeString("XOML", dr["XOML"].ToString());
			xmlWriter.WriteAttributeString("SiteID", dr["SiteID"].ToString());
			xmlWriter.WriteAttributeString("ListID", dr["ListID"].ToString());
			xmlWriter.WriteAttributeString("ItemID", dr["ItemID"].ToString());
			xmlWriter.WriteAttributeString("WebID", dr["WebID"].ToString());
			xmlWriter.WriteAttributeString("StartTime", dr["StartTime"].ToString());
			xmlWriter.WriteAttributeString("WorkflowInstanceID", dr["WorkflowInstanceID"].ToString());
			xmlWriter.WriteAttributeString("WorkflowInitiator", dr["WorkflowInitiator"].ToString());
			xmlWriter.WriteAttributeString("WorkflowName", dr["WorkflowName"].ToString());
			xmlWriter.WriteAttributeString("WorkflowID", dr["WorkflowID"].ToString());
			xmlWriter.WriteAttributeString("State", dr["State"].ToString());
			xmlWriter.WriteAttributeString("Rules", dr["Rules"].ToString());
			xmlWriter.WriteAttributeString("ConfigXml", dr["ConfigXml"].ToString());
			xmlWriter.WriteAttributeString("ExpectedDuration", dr["ExpectedDuration"].ToString());
			xmlWriter.WriteAttributeString("TaskListID", dr["TaskListID"].ToString());
			xmlWriter.WriteAttributeString("WebApplicationID", dr["WebApplicationID"].ToString());
			xmlWriter.WriteAttributeString("HistoryListID", dr["HistoryListID"].ToString());
			xmlWriter.WriteAttributeString("ItemDeleted", dr["ItemDeleted"].ToString());
			xmlWriter.WriteAttributeString("WorkflowData", (dr["WorkflowData"] is DBNull ? DBNull.Value.ToString() : Convert.ToBase64String((byte[])dr["WorkflowData"], Base64FormattingOptions.None)));
			if (dr.Table.Columns.Contains("Category"))
			{
				xmlWriter.WriteAttributeString("Category", dr["Category"].ToString());
			}
			if (dr.Table.Columns.Contains("ContextData"))
			{
				xmlWriter.WriteAttributeString("ContextData", dr["ContextData"].ToString());
			}
			if (bIs2010)
			{
				this.WriteDataStoreData(dr, xmlWriter);
			}
			this.WriteProgressData(dr, xmlWriter);
			xmlWriter.WriteEndElement();
		}

		private void WriteXmlProgressData(DataRow dr, XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("WorkflowProgress");
			xmlWriter.WriteAttributeString("WorkflowProgressID", dr["WorkflowProgressID"].ToString());
			xmlWriter.WriteAttributeString("ActivityID", dr["ActivityID"].ToString());
			xmlWriter.WriteAttributeString("ActivityComplete", dr["ActivityComplete"].ToString());
			xmlWriter.WriteAttributeString("TimeStamp", dr["TimeStamp"].ToString());
			xmlWriter.WriteAttributeString("ExpectedDuration", dr["ExpectedDuration"].ToString());
			xmlWriter.WriteAttributeString("SequenceID", dr["SequenceID"].ToString());
			xmlWriter.WriteAttributeString("CurrentActivityTitle", dr["CurrentActivityTitle"].ToString());
			xmlWriter.WriteAttributeString("WorkflowData", (dr["WorkflowData"] is DBNull ? DBNull.Value.ToString() : Convert.ToBase64String((byte[])dr["WorkflowData"], Base64FormattingOptions.None)));
			this.WriteHumanWorkflowData(dr, xmlWriter);
			xmlWriter.WriteEndElement();
		}
	}
}