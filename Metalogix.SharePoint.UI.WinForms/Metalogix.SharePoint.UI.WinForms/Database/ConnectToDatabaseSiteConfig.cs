using Metalogix;
using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.Interfaces;
using Metalogix.Permissions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Database;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.Exceptions;
using Metalogix.SharePoint.Properties;
using Metalogix.SharePoint.UI.WinForms.Administration;
using Metalogix.SharePoint.UI.WinForms.Properties;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Actions;
using Metalogix.UI.WinForms.Database;
using Metalogix.UI.WinForms.Widgets;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Xml;

namespace Metalogix.SharePoint.UI.WinForms.Database
{
	[ActionConfig(new Type[] { typeof(ConnectToDatabaseSite) })]
	public class ConnectToDatabaseSiteConfig : IActionConfig
	{
		public ConnectToDatabaseSiteConfig()
		{
		}

	    // Metalogix.SharePoint.UI.WinForms.Database.ConnectToDatabaseSiteConfig
	    public ConfigurationResult Configure(ActionConfigContext context)
	    {
	        ConfigurationResult result;
	        try
	        {
	            DatabaseConnectDialog dlgConnect = new DatabaseConnectDialog();
	            dlgConnect.AllowBrowsingNetworkServers = true;
	            dlgConnect.AllowBrowsingDatabases = false;
	            DialogResult dialogResult = dlgConnect.ShowDialog();
	            if (dialogResult != DialogResult.OK)
	            {
	                result = ConfigurationResult.Cancel;
	            }
	            else
	            {
	                Credentials credentials = dlgConnect.Credentials;
	                Credentials credentials2 = credentials.IsDefault ? Credentials.DefaultCredentials : new Credentials(credentials.UserName, credentials.Password, credentials.SavePassword);
	                IDBReader reader = (IDBReader)SharePointAdapter.GetDBAdapter(new object[]
	                {
	                    dlgConnect.SqlServerName,
	                    "master",
	                    credentials2
	                });
	                reader.CheckConnection(false);
	                Cursor.Current = Cursors.WaitCursor;
	                ConnectionCollection connections = null;
	                bool workloadCompleted = false;
	                try
	                {
	                    string databaseCollectionXml;
	                    PleaseWaitDialog.ShowWaitDialog(string.Format(Metalogix.SharePoint.UI.WinForms.Properties.Resources.FS_SearchingForSharePointDatabases, dlgConnect.SqlServerName), delegate (System.ComponentModel.BackgroundWorker worker)
	                    {
	                        databaseCollectionXml = reader.GetSQLDatabaseList();
	                        System.Xml.XmlElement xmlConnectionCollection = this.CreateConnectionCollectionXml(databaseCollectionXml, dlgConnect.Credentials);
	                        connections = new ConnectionCollection(xmlConnectionCollection);
	                        this.RemoveNonSharePointDatabases(connections);
	                        connections.Sort(delegate (Node x, Node y)
	                        {
	                            if (x.DisplayName != null && y.DisplayName != null)
	                            {
	                                return x.DisplayName.CompareTo(y.DisplayName);
	                            }
	                            if (x.DisplayName != null || y.DisplayName != null)
	                            {
	                                return -1;
	                            }
	                            return 0;
	                        });
	                        workloadCompleted = true;
	                    }, true);
	                }
	                catch (System.Exception ex)
	                {
	                    throw new System.Exception("Could not fetch database list: " + ex.Message, ex);
	                }
	                finally
	                {
	                    Cursor.Current = Cursors.Default;
	                }
	                if (!workloadCompleted)
	                {
	                    result = ConfigurationResult.Cancel;
	                }
	                else
	                {
	                    NodeChooserDialog nodeChooserDialog = new NodeChooserDialog();
	                    nodeChooserDialog.DataSource = connections;
	                    nodeChooserDialog.Text = "Connect to SharePoint SQL Server";
	                    nodeChooserDialog.Message = "Please select a SharePoint Database, or a SharePoint Site";
	                    nodeChooserDialog.Icon = dlgConnect.Icon;
	                    nodeChooserDialog.ShowIcon = true;
	                    dialogResult = nodeChooserDialog.ShowDialog();
	                    if (dialogResult != DialogResult.OK)
	                    {
	                        result = ConfigurationResult.Cancel;
	                    }
	                    else
	                    {
	                        SPConnection sPConnection = (SPConnection)((SPConnection)nodeChooserDialog.SelectedNode).Clone();
	                        sPConnection.Adapter.Credentials = credentials2;
	                        Metalogix.Explorer.Settings.ActiveConnections.Add(sPConnection);
	                        HostNameDialog hostNameDialog = new HostNameDialog();
	                        hostNameDialog.CancelSkipButtonText = "Skip";
	                        hostNameDialog.ShowDialog();
	                        if (hostNameDialog.DialogResult == DialogResult.Cancel)
	                        {
	                            result = ConfigurationResult.Cancel;
	                        }
	                        else
	                        {
	                            string hostName = hostNameDialog.HostName;
	                            NodePropertiesDialog nodePropertiesDialog = new NodePropertiesDialog();
	                            if (!nodePropertiesDialog.SetLinkName(sPConnection, hostName))
	                            {
	                                FlatXtraMessageBox.Show(Metalogix.SharePoint.Properties.Resources.Host_Name_Invalid_Warning, "Could not apply host name", MessageBoxButtons.OK, MessageBoxIcon.Hand);
	                            }
	                            Metalogix.Explorer.Settings.SaveActiveConnections();
	                            hostNameDialog.Dispose();
	                            nodePropertiesDialog.Dispose();
	                            result = ConfigurationResult.Run;
	                        }
	                    }
	                }
	            }
	        }
	        catch (System.Exception exc)
	        {
	            GlobalServices.ErrorHandler.HandleException(exc);
	            result = ConfigurationResult.Cancel;
	        }
	        return result;
	    }


        private XmlElement CreateConnectionCollectionXml(string databaseCollectionXml, Credentials credentials)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(databaseCollectionXml);
			XmlDocument xmlDocument1 = new XmlDocument();
			XmlElement xmlElement = xmlDocument1.CreateElement("ConnectionCollection");
			foreach (XmlNode xmlNodes in xmlDocument.SelectNodes("//Database"))
			{
				XmlElement xmlElement1 = xmlDocument1.CreateElement("Connection");
				XmlAttribute assemblyQualifiedName = xmlDocument1.CreateAttribute("NodeType");
				assemblyQualifiedName.Value = typeof(SPServer).AssemblyQualifiedName;
				xmlElement1.Attributes.Append(assemblyQualifiedName);
				foreach (XmlAttribute attribute in xmlNodes.Attributes)
				{
					XmlAttribute value = xmlDocument1.CreateAttribute(attribute.Name);
					value.Value = attribute.Value;
					xmlElement1.Attributes.Append(value);
				}
				if (!credentials.IsDefault)
				{
					XmlAttribute insecureString = xmlDocument1.CreateAttribute("Password");
					if (!credentials.SavePassword)
					{
						insecureString.Value = credentials.Password.ToInsecureString();
					}
					else
					{
						insecureString.Value = Cryptography.EncryptText(credentials.Password, Cryptography.ProtectionScope.CurrentUser, null);
					}
					xmlElement1.Attributes.Append(insecureString);
				}
				xmlElement.AppendChild(xmlElement1);
			}
			xmlDocument1.AppendChild(xmlElement);
			return xmlElement;
		}

		private void RemoveNonSharePointDatabases(ConnectionCollection connections)
		{
			List<Node> nodes = new List<Node>();
			foreach (Node connection in connections)
			{
				try
				{
					connection.Connection.CheckConnection();
				}
				catch (Exception exception)
				{
					if (exception is NotSharePointDatabaseException)
					{
						nodes.Add(connection);
					}
				}
			}
			connections.RemoveRange(nodes.ToArray());
		}
	}
}