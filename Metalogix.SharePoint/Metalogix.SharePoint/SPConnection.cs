using Metalogix;
using Metalogix.Core;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.Licensing.Common;
using Metalogix.Metabase;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.Authentication;
using Metalogix.SharePoint.Adapters.Properties;
using Metalogix.SharePoint.Properties;
using Metalogix.Telemetry.Accumulators;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Xml;

namespace Metalogix.SharePoint
{
	public abstract class SPConnection : SPNode, Metalogix.Explorer.Connection, IBugReportable
	{
		protected bool m_bShowAllSites = true;

		private string m_SharePointVersion;

		private string m_UnderlyingAdapterType;

		protected string m_sUrl;

		private string m_sUserName;

		private bool _isLimitedSiteCollectionConnection = false;

		private List<string> _limitedSiteCollections = new List<string>();

		private bool bConnectionCheckedAtLeastOnce = false;

		private int _telemetrySent;

		private bool m_bBackupConnection = false;

		private SPConnection.BackupConnectionType m_BackupType = SPConnection.BackupConnectionType.None;

		public string AdapterName
		{
			get
			{
				return this.m_UnderlyingAdapterType;
			}
		}

		public SPConnection.BackupConnectionType BackupType
		{
			get
			{
				return this.m_BackupType;
			}
			set
			{
				this.m_BackupType = value;
				if (value != SPConnection.BackupConnectionType.None)
				{
					this.m_bBackupConnection = true;
				}
			}
		}

		public string ConnectionString
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder(1024);
				StringWriter stringWriter = new StringWriter(stringBuilder);
				try
				{
					XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
					try
					{
						xmlTextWriter.Formatting = Formatting.Indented;
						xmlTextWriter.WriteStartElement("Connection");
						xmlTextWriter.WriteAttributeString("NodeType", base.GetType().AssemblyQualifiedName);
						if (this.IsBackupConnection)
						{
							xmlTextWriter.WriteAttributeString("BackupType", Enum.GetName(typeof(SPConnection.BackupConnectionType), this.BackupType));
						}
						xmlTextWriter.WriteAttributeString("SharePointVersion", this.m_SharePointVersion);
						xmlTextWriter.WriteAttributeString("UnderlyingAdapterType", this.m_UnderlyingAdapterType);
						xmlTextWriter.WriteAttributeString("ShowAllSites", this.m_bShowAllSites.ToString());
						if (base.MetabaseConnection != null)
						{
							xmlTextWriter.WriteAttributeString("MetabaseType", base.MetabaseConnection.MetabaseType);
							xmlTextWriter.WriteAttributeString("MetabaseContext", Cryptography.EncryptText(base.MetabaseConnection.MetabaseContext.ToSecureString(), Cryptography.ProtectionScope.CurrentUser, null));
						}
						if (this.IsLimitedSiteCollectionConnection)
						{
							xmlTextWriter.WriteAttributeString("IsLimitedSiteCollectionConnection", this.IsLimitedSiteCollectionConnection.ToString());
						}
						if (base.Adapter != null)
						{
							base.Adapter.ToXML(xmlTextWriter);
						}
						if (this.IsLimitedSiteCollectionConnection)
						{
							this.AddLimitedSiteCollectionsInXml(xmlTextWriter);
						}
						xmlTextWriter.WriteEndElement();
					}
					finally
					{
						if (xmlTextWriter != null)
						{
							((IDisposable)xmlTextWriter).Dispose();
						}
					}
				}
				finally
				{
					if (stringWriter != null)
					{
						((IDisposable)stringWriter).Dispose();
					}
				}
				return stringBuilder.ToString();
			}
		}

		public override string DisplayUrl
		{
			get
			{
				return ((base.Status == ConnectionStatus.Valid ? false : base.Parent == null) ? base.Adapter.ServerDisplayName : base.DisplayUrl);
			}
		}

		public bool IsBackupConnection
		{
			get
			{
				return this.m_bBackupConnection;
			}
		}

		public bool IsLimitedSiteCollectionConnection
		{
			get
			{
				return this._isLimitedSiteCollectionConnection;
			}
			set
			{
				this._isLimitedSiteCollectionConnection = value;
			}
		}

		public bool? IsSharePointOnline
		{
			get
			{
				return new bool?(base.Adapter.SharePointVersion.IsSharePointOnline);
			}
		}

		public List<string> LimitedSiteCollections
		{
			get
			{
				return this._limitedSiteCollections;
			}
			set
			{
				this._limitedSiteCollections = value;
			}
		}

		public override string LinkableUrl
		{
			get
			{
				string str;
				if ((base.Adapter == null || base.Adapter.ServerLinkName == null ? false : !(base.Adapter.ServerLinkName.Trim() == "")))
				{
					str = (base.Status != ConnectionStatus.Valid ? string.Concat(base.Adapter.ServerLinkName, base.Adapter.ServerRelativeUrl) : base.LinkableUrl);
				}
				else
				{
					str = null;
				}
				return str;
			}
		}

		public Metalogix.Explorer.Node Node
		{
			get
			{
				return this;
			}
		}

		public override string Url
		{
			get
			{
				string mSUrl;
				if (base.Adapter == null)
				{
					mSUrl = this.m_sUrl;
				}
				else
				{
					mSUrl = (!base.Adapter.Url.StartsWith(base.Adapter.Server) ? string.Concat(base.Adapter.Server, base.Adapter.Url) : base.Adapter.Url);
				}
				return mSUrl;
			}
		}

		public string UserName
		{
			get
			{
				return this.m_sUserName;
			}
		}

		public string VersionNumber
		{
			get
			{
				return this.m_SharePointVersion;
			}
		}

		static SPConnection()
		{
			SPConnection.SetupLicensing();
			if (SharePointConfigurationVariables.EnableAdapterLogging)
			{
				SharePointAdapter.EnableAdapterLogging = SharePointConfigurationVariables.EnableAdapterLogging;
				SharePointAdapter.AdapterLogFileLocation = SharePointConfigurationVariables.AppLogFilePath;
			}
		}

		protected SPConnection(SharePointAdapter adapter, SPNode parent) : base(adapter, parent)
		{
		}

		protected SPConnection(XmlNode connectionNode) : base(null, null)
		{
			this.m_adapter = SharePointAdapter.Create(connectionNode);
			if (connectionNode.Attributes["Url"] != null)
			{
				this.m_sUrl = connectionNode.Attributes["Url"].Value;
			}
			if (connectionNode.Attributes["UserName"] != null)
			{
				this.m_sUserName = connectionNode.Attributes["UserName"].Value;
			}
			if (connectionNode.Attributes["BackupType"] != null)
			{
				try
				{
					this.m_BackupType = (SPConnection.BackupConnectionType)Enum.Parse(typeof(SPConnection.BackupConnectionType), connectionNode.Attributes["BackupType"].Value);
					this.m_bBackupConnection = true;
				}
				catch
				{
					this.m_bBackupConnection = false;
					this.m_BackupType = SPConnection.BackupConnectionType.None;
				}
			}
			if (connectionNode.Attributes["ShowAllSites"] != null)
			{
				this.m_bShowAllSites = bool.Parse(connectionNode.Attributes["ShowAllSites"].Value);
			}
			if ((connectionNode.Attributes["MetabaseType"] == null ? false : connectionNode.Attributes["MetabaseContext"] != null))
			{
				base.MetabaseConnection = MetabaseFactory.ConnectToExistingMetabase(connectionNode.Attributes["MetabaseType"].Value, Cryptography.DecryptText(connectionNode.Attributes["MetabaseContext"].Value).ToInsecureString());
			}
			this.IsLimitedSiteCollectionConnection = connectionNode.GetAttributeValueAsBoolean("IsLimitedSiteCollectionConnection");
			if (!this.IsLimitedSiteCollectionConnection)
			{
				this.LimitedSiteCollections = null;
			}
			else
			{
				foreach (XmlNode childNode in connectionNode.ChildNodes)
				{
					if (childNode.Name.Equals("SiteCollections", StringComparison.InvariantCultureIgnoreCase))
					{
						string empty = string.Empty;
						Dictionary<string, string> strs = new Dictionary<string, string>();
						this.LimitedSiteCollections = SPUtils.GetSiteCollectionsFromXml(childNode.OuterXml, out empty, out strs, null, null);
						if (!string.IsNullOrEmpty(empty))
						{
							throw new Exception(empty);
						}
						break;
					}
				}
			}
		}

		private void AddLimitedSiteCollectionsInXml(XmlWriter xmlWriter)
		{
			if (this.LimitedSiteCollections.Count > 0)
			{
				xmlWriter.WriteStartElement("SiteCollections");
				foreach (string limitedSiteCollection in this.LimitedSiteCollections)
				{
					xmlWriter.WriteElementString("Url", limitedSiteCollection);
				}
				xmlWriter.WriteEndElement();
			}
		}

		private void CalculateSPOTenantSize()
		{
			string str;
			Guid guid;
			bool flag = false;
			string empty = string.Empty;
			string empty1 = string.Empty;
			string str1 = string.Format("{0};{1}", this.GetServerUrl(), this.GetConnectedAs());
			try
			{
				try
				{
					if (this.GetTenantStorageDetailsUsingO365ReportingService(out empty1, out str, out empty))
					{
						if ((string.IsNullOrEmpty(empty1) ? true : !empty1.TryParseGuid(out guid)))
						{
							empty = string.Format("Failed to get tenant server id from Office 365 reporting service for tenant: {0}.", str1);
							Logger.Debug.Write("LicenseUpdate error >> CalculateSPOTenantSize >> Failed to get tenant server id from Office 365 reporting service");
						}
						else
						{
							long num = (long)0;
							if ((string.IsNullOrEmpty(str) ? false : LicensingUtils.IsContentMatrixContentUnderMgmt()))
							{
								num = Format.ParseFormattedSize(string.Concat(str, " MB"));
							}
							if (!MLLicenseProvider.Instance.UpdateLicense(num, empty1, true, str1))
							{
								empty = string.Format("Failed to update tenant size {0} on license server for tenant {1};{2}", num, empty1, str1);
								Logger.Debug.Write(string.Format("LicenseUpdate error >> CalculateSPOTenantSize >> Failed to update tenant size on license server for tenant {0}.", empty1));
							}
							else
							{
								flag = true;
							}
						}
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					Metalogix.Utilities.ExceptionDetail exceptionMessageAndDetail = ExceptionUtils.GetExceptionMessageAndDetail(exception);
					object[] message = new object[] { empty1, str1, exceptionMessageAndDetail.Message, exceptionMessageAndDetail.Detail };
					empty = string.Format("Error occurred while processing SPO license data calculation for tenant: {0};{1}. Message: {2}. Details: {3}.", message);
					Logger.Debug.Write(empty);
					Metalogix.Core.Logging.LogExceptionToTextFileWithEventLogBackup(exception, "CalculateSPOTenantSize", true);
				}
			}
			finally
			{
				if (!flag)
				{
					this.LogTelemetry("SPOLicensingDataUpdateFailed", empty, "CalculateSPOTenantSize");
				}
			}
		}

		public void CheckConnection()
		{
			this.CheckConnection(false);
		}

		public void CheckConnection(bool bBypassManualConnectBlock)
		{
			this.CheckConnection(bBypassManualConnectBlock, false);
		}

		internal void CheckConnection(bool bBypassManualConnectBlock, bool byPassLicenseCheck)
		{
			string str = null;
			if (base.Adapter != null)
			{
				if ((!bBypassManualConnectBlock && !this.bConnectionCheckedAtLeastOnce && base.Adapter.AuthenticationInitializerType != null && !AuthenticationInitializer.GetInitializerAllowsAutomaticLogin(base.Adapter.AuthenticationInitializerType)))
				{
					throw new Exception(Metalogix.SharePoint.Properties.Resources.Manual_Reconnection_Required);
				}
				base.Adapter.CheckConnection();
				if ((byPassLicenseCheck ? false : !SPConnection.LicenseAllowsConnection(base.Adapter.SharePointVersion, out str)))
				{
					throw new MLLicenseException(str);
				}
				this.bConnectionCheckedAtLeastOnce = true;
				if (!byPassLicenseCheck)
				{
					base.Adapter.IsDataLimitExceededForContentUnderMgmt = LicensingUtils.IsDataLimitExceededForContentUnderMgmt();
				}
				if (base.Adapter.SharePointVersion.IsSharePointOnline)
				{
					this.CalculateSPOTenantSize();
				}
				if (Interlocked.CompareExchange(ref this._telemetrySent, 1, 0) == 0)
				{
					object[] displayedShortName = new object[] { base.Adapter.DisplayedShortName, base.Adapter.SharePointVersion, base.Adapter.SharePointVersion.IsSharePointOnline, this.GetServerUrl(), this.GetConnectedAs() };
					string str1 = string.Format("Adapter: {0}, SP Version: {1}, IsSharePointOnline: {2}, ServerUrl: {3}, ConnectedAs: {4}.", displayedShortName);
					LongAccumulator.Message.Send(str1, (long)1, null);
				}
			}
		}

		public Metalogix.Explorer.Connection Clone()
		{
			return ConnectionFactory.CreateConnection(this.ConnectionString);
		}

		public override void Close()
		{
			if (this.m_adapter != null)
			{
				this.m_adapter.Dispose();
			}
		}

		public bool ConnectionEquals(XmlNode connXML)
		{
			bool flag;
			try
			{
				if (!(TypeUtils.UpdateType(connXML.Attributes["NodeType"].Value) != base.GetType().AssemblyQualifiedName))
				{
					XmlAttribute itemOf = connXML.Attributes["ShowAllSites"];
					if ((itemOf == null ? true : !(itemOf.Value != this.m_bShowAllSites.ToString())))
					{
						if (itemOf == null)
						{
							if (!this.m_bShowAllSites)
							{
								flag = false;
								return flag;
							}
						}
						flag = base.Adapter.AdapterEquals(connXML);
					}
					else
					{
						flag = false;
					}
				}
				else
				{
					flag = false;
				}
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		private void ExtractTenantGuid(WebException wex, out string spoTenantGuid)
		{
			string str = "WWW-Authenticate";
			string str1 = "Bearer realm";
			spoTenantGuid = string.Empty;
			string str2 = wex.Response.Headers.Get(str);
			if (!string.IsNullOrEmpty(str2))
			{
				char[] chrArray = new char[] { ',' };
				string[] strArrays = str2.Split(chrArray, StringSplitOptions.RemoveEmptyEntries);
				if ((strArrays == null ? false : (int)strArrays.Length > 0))
				{
					string[] strArrays1 = strArrays;
					for (int i = 0; i < (int)strArrays1.Length; i++)
					{
						string str3 = strArrays1[i];
						if (str3.Trim().StartsWith(str1, StringComparison.OrdinalIgnoreCase))
						{
							chrArray = new char[] { '=' };
							string[] strArrays2 = str3.Split(chrArray, StringSplitOptions.RemoveEmptyEntries);
							if ((strArrays2 == null || (int)strArrays2.Length <= 1 ? false : strArrays2[0].Equals(str1, StringComparison.OrdinalIgnoreCase)))
							{
								string str4 = strArrays2[1];
								chrArray = new char[] { '\"' };
								spoTenantGuid = str4.Trim(chrArray);
								break;
							}
						}
					}
				}
			}
		}

	    ~SPConnection()
	    {
	        this.Close();
	    }

        private string GetConnectedAs()
		{
			string str;
			if (base.Adapter.CredentialsAreDefault)
			{
				string userDomainName = Environment.UserDomainName;
				str = (string.IsNullOrEmpty(userDomainName) ? Environment.UserName : string.Concat(userDomainName, "\\", Environment.UserName));
			}
			else if (base.Adapter.Credentials != null)
			{
				str = (string.IsNullOrEmpty(base.Adapter.Credentials.UserName) ? string.Empty : base.Adapter.Credentials.UserName);
			}
			else
			{
				str = "(null)";
			}
			return str;
		}

		private string GetServerUrl()
		{
			string serverUrl = base.Adapter.ServerUrl;
			if (string.IsNullOrEmpty(serverUrl))
			{
				serverUrl = "Undefined";
			}
			return serverUrl;
		}

		private bool GetTenantStorageDetailsUsingO365ReportingService(out string spoTenantGuid, out string spoTenantUsedData, out string errorMessage)
		{
			SPOTenantStorageMetricReport sPOTenantStorageMetricReport;
			spoTenantGuid = string.Empty;
			spoTenantUsedData = string.Empty;
			errorMessage = string.Empty;
			bool flag = false;
			bool tenantStorageDetailsUsingO365WebService = false;
			try
			{
				try
				{
					string str = "https://reports.office365.com/ecp/ReportingWebService/Reporting.svc/SPOTeamSiteStorageWeekly?$format=json&$orderby=Date%20desc&$top=1";
					WebClient webClient = new WebClient()
					{
						Credentials = base.Adapter.Credentials.NetworkCredentials
					};
					string str1 = webClient.DownloadString(str);
					DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(SPOTenantStorageMetricReport));
					MemoryStream memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(str1));
					try
					{
						sPOTenantStorageMetricReport = (SPOTenantStorageMetricReport)dataContractJsonSerializer.ReadObject(memoryStream);
					}
					finally
					{
						if (memoryStream != null)
						{
							((IDisposable)memoryStream).Dispose();
						}
					}
					if ((sPOTenantStorageMetricReport == null || sPOTenantStorageMetricReport.SPOTenantStorageDetails == null ? false : sPOTenantStorageMetricReport.SPOTenantStorageDetails.Count > 0))
					{
						SPOTenantReporting sPOTenantReporting = sPOTenantStorageMetricReport.SPOTenantStorageDetails.FirstOrDefault<SPOTenantReporting>();
						if (sPOTenantReporting != null)
						{
							spoTenantGuid = sPOTenantReporting.TenantGuid;
							spoTenantUsedData = sPOTenantReporting.Used;
							flag = true;
							tenantStorageDetailsUsingO365WebService = true;
						}
					}
				}
				catch (WebException webException1)
				{
					WebException webException = webException1;
					string str2 = string.Format("{0};{1}", this.GetServerUrl(), this.GetConnectedAs());
					errorMessage = string.Format("Failed to get response from Office 365 reporting web service for server url : {0}. Error : {1}.", str2, webException.Message);
					tenantStorageDetailsUsingO365WebService = this.GetTenantStorageDetailsUsingO365WebService(ref spoTenantGuid, ref errorMessage, webException);
					flag = true;
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					Metalogix.Utilities.ExceptionDetail exceptionMessageAndDetail = ExceptionUtils.GetExceptionMessageAndDetail(exception);
					Logger.Debug.Write(string.Format("LicenseUpdate error >> GetTenantStorageDetailsUsingO365ReportingService >> Something went wrong while fetching tenant storage details. Message: {0}. Details: {1}", exceptionMessageAndDetail.Message, exceptionMessageAndDetail.Detail));
					Metalogix.Core.Logging.LogExceptionToTextFileWithEventLogBackup(exception, "GetTenantStorageDetailsUsingO365ReportingService", true);
				}
			}
			finally
			{
				if (!flag)
				{
					tenantStorageDetailsUsingO365WebService = this.GetTenantStorageDetailsUsingO365WebService(ref spoTenantGuid, ref errorMessage, null);
				}
			}
			return tenantStorageDetailsUsingO365WebService;
		}

		private bool GetTenantStorageDetailsUsingO365WebService(ref string spoTenantGuid, ref string errorMessage, WebException ex = null)
		{
			Guid guid;
			bool flag;
			string str = string.Format("{0};{1}", this.GetServerUrl(), this.GetConnectedAs());
			try
			{
				if ((ex == null ? false : ex.Status != WebExceptionStatus.ProtocolError))
				{
					Logger.Debug.Write(string.Format("LicenseUpdate error >> GetTenantStorageDetailsUsingO365WebService >> Failed to get response from Office 365 reporting service. Error : {0}.", ex.Message));
					errorMessage = string.Format("Failed to get response from Office 365 reporting service for tenant : {0}. Error {1}.", str, ex.Message);
				}
				else
				{
					string[] url = new string[] { this.Url, "_vti_bin/client.svc" };
					string str1 = UrlUtils.ConcatUrls(url);
					WebClient webClient = new WebClient()
					{
						Credentials = base.Adapter.Credentials.NetworkCredentials
					};
					WebClient webClient1 = webClient;
					try
					{
						webClient1.Headers.Add(HttpRequestHeader.Authorization, "Bearer");
						webClient1.DownloadString(str1);
					}
					finally
					{
						if (webClient1 != null)
						{
							((IDisposable)webClient1).Dispose();
						}
					}
				}
			}
			catch (WebException webException1)
			{
				WebException webException = webException1;
				try
				{
					if (webException.Response != null)
					{
						if ((webException.Response.Headers == null ? false : webException.Response.Headers.Count > 0))
						{
							this.ExtractTenantGuid(webException, out spoTenantGuid);
							if ((string.IsNullOrEmpty(spoTenantGuid) ? true : !spoTenantGuid.TryParseGuid(out guid)))
							{
								errorMessage = string.Format("Failed to get response from Office 365 web service for server url : {0}. Error : {1}.", str, webException.Message);
							}
							else
							{
								flag = !MLLicenseProvider.Instance.IsSPOServerIdExist(spoTenantGuid);
								return flag;
							}
						}
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					errorMessage = string.Format("Failed to get response from Office 365 web service for tenant : {0};{1}. Error : {2}.", spoTenantGuid, str, exception.Message);
				}
			}
			flag = false;
			return flag;
		}

		public bool IsConnectionValid()
		{
			bool flag = false;
			try
			{
				switch (base.Status)
				{
					case ConnectionStatus.NotChecked:
					case ConnectionStatus.Checking:
					{
						this.CheckConnection();
						flag = true;
						break;
					}
					case ConnectionStatus.Valid:
					{
						flag = true;
						break;
					}
				}
			}
			catch (Exception exception)
			{
				flag = false;
			}
			return flag;
		}

		private static bool LicenseAllowsConnection(SharePointVersion version)
		{
			string str;
			return SPConnection.LicenseAllowsConnection(version, out str);
		}

		private static bool LicenseAllowsConnection(SharePointVersion version, out string message)
		{
			bool flag;
			if ((!version.IsSharePoint2013OrLater ? true : LicensingUtils.GetLevel() == CompatibilityLevel.Current))
			{
				LicensedSharePointVersions licensedSharePointVersions = LicensingUtils.GetLicensedSharePointVersions();
				if (!(!version.IsSharePoint2003 ? true : (licensedSharePointVersions & LicensedSharePointVersions.SP2003) == LicensedSharePointVersions.SP2003))
				{
					message = string.Format(Metalogix.SharePoint.Properties.Resources.FS_SharePointVersionUnlicensed, Metalogix.SharePoint.Properties.Resources.SP2003);
					flag = false;
				}
				else if (!(!version.IsSharePoint2007 ? true : (licensedSharePointVersions & LicensedSharePointVersions.SP2007) == LicensedSharePointVersions.SP2007))
				{
					message = string.Format(Metalogix.SharePoint.Properties.Resources.FS_SharePointVersionUnlicensed, Metalogix.SharePoint.Properties.Resources.SP2007);
					flag = false;
				}
				else if (!(!version.IsSharePoint2010 ? true : (licensedSharePointVersions & LicensedSharePointVersions.SP2010) == LicensedSharePointVersions.SP2010))
				{
					message = string.Format(Metalogix.SharePoint.Properties.Resources.FS_SharePointVersionUnlicensed, Metalogix.SharePoint.Properties.Resources.SP2010);
					flag = false;
				}
				else if (!(!version.IsSharePointOnline ? true : (licensedSharePointVersions & LicensedSharePointVersions.SPOnline) == LicensedSharePointVersions.SPOnline))
				{
					message = string.Format(Metalogix.SharePoint.Properties.Resources.FS_SharePointVersionUnlicensed, Metalogix.SharePoint.Properties.Resources.SPOnline);
					flag = false;
				}
				else if (!(!version.IsSharePoint2013 ? true : (licensedSharePointVersions & LicensedSharePointVersions.SP2013) == LicensedSharePointVersions.SP2013))
				{
					message = string.Format(Metalogix.SharePoint.Properties.Resources.FS_SharePointVersionUnlicensed, Metalogix.SharePoint.Properties.Resources.SP2013);
					flag = false;
				}
				else if ((!version.IsSharePoint2016 ? true : (licensedSharePointVersions & LicensedSharePointVersions.SP2016) == LicensedSharePointVersions.SP2016))
				{
					message = null;
					flag = true;
				}
				else
				{
					message = string.Format(Metalogix.SharePoint.Properties.Resources.FS_SharePointVersionUnlicensed, Metalogix.SharePoint.Properties.Resources.SP2016);
					flag = false;
				}
			}
			else
			{
				message = Metalogix.SharePoint.Adapters.Properties.Resources.SharePoint_Not_Supported_By_License;
				flag = false;
			}
			return flag;
		}

		private void LogTelemetry(string infoKey, string infoValue, string methodName)
		{
			try
			{
				StringAccumulator.Message.Send(infoKey, infoValue, false, null);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Logger.Debug.Write(infoValue);
				Metalogix.Core.Logging.LogExceptionToTextFileWithEventLogBackup(exception, methodName, true);
			}
		}

		private static void OnActiveConnectionsChanged(NodeCollectionChangeType changeType, Metalogix.Explorer.Node node)
		{
			if (changeType == NodeCollectionChangeType.FullReset)
			{
				SPConnection.UpdateAdapterLicense();
			}
		}

		private static void OnLicenseUpdated(object sender, EventArgs eventArgs)
		{
			SPConnection.UpdateAdapterLicense();
		}

		public void Reconnect()
		{
			this.ReconnectCallback(null);
		}

		public void ReconnectAsync()
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.ReconnectCallback));
		}

		private void ReconnectCallback(object obj)
		{
			this.ClearExcessNodeData();
			base.ReleaseChildren();
			base.SetStatus(ConnectionStatus.NotChecked);
			this.FetchChildren();
			this.UpdateCurrentNode();
		}

		public void SetServerAndAdapterDetails()
		{
			if (base.Adapter != null)
			{
				if (string.IsNullOrEmpty(this.m_SharePointVersion))
				{
					this.m_SharePointVersion = base.Adapter.SharePointVersion.ToString();
				}
				if (string.IsNullOrEmpty(this.m_UnderlyingAdapterType))
				{
					IAutoDetectAdapter adapter = base.Adapter as IAutoDetectAdapter;
					this.m_UnderlyingAdapterType = (adapter != null ? adapter.InternalAdapterShortName : base.Adapter.DisplayedShortName);
				}
			}
		}

		private static void SetupLicensing()
		{
			SPConnection.UpdateAdapterLicense();
			MLLicenseProvider.LicenseUpdated += new EventHandler(SPConnection.OnLicenseUpdated);
			Metalogix.Explorer.Settings.ActiveConnectionsChanged += new NodeCollectionChangedHandler(SPConnection.OnActiveConnectionsChanged);
		}

		private static void UpdateAdapterLicense()
		{
			MLLicense license = MLLicenseProvider.Instance.GetLicense(new LicenseContext(), typeof(SPConnection), null, false) as MLLicense;
			bool isWeb = ApplicationData.IsWeb;
			if ((license == null ? false : license is MLLicenseCommon))
			{
				MLLicenseCommon mLLicenseCommon = license as MLLicenseCommon;
				isWeb = ((mLLicenseCommon.IsLegacyProduct ? false : !mLLicenseCommon.IsContentMatrixExpress) ? mLLicenseCommon.SkipWebChecking : true);
			}
			AdapterLicensing.UpdateLicenseData(new Dictionary<string, object>()
			{
				{ "skipWebChecking", isWeb }
			});
		}

		public void UpdateLicensedStatus()
		{
			if (!(base.Status != ConnectionStatus.Invalid ? true : !(base.ConnectionError is MLLicenseException)))
			{
				if (LicensingUtils.GetLevel() == CompatibilityLevel.Current)
				{
					this.ReconnectAsync();
				}
			}
			else if (base.Status == ConnectionStatus.Valid)
			{
				if (!SPConnection.LicenseAllowsConnection(base.Adapter.SharePointVersion))
				{
					this.ReconnectAsync();
				}
			}
			base.FireStatusChanged();
		}

		public enum BackupConnectionType
		{
			None,
			Bak,
			Mdf
		}
	}
}