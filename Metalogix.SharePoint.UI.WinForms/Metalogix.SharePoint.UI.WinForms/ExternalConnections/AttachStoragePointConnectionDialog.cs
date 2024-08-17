using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Metalogix.Actions;
using Metalogix.Connectivity.Proxy;
using Metalogix.DataStructures;
using Metalogix.Explorer;
using Metalogix.ExternalConnections;
using Metalogix.SharePoint.ExternalConnections;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Proxy;

namespace Metalogix.SharePoint.UI.WinForms.ExternalConnections
{
	public class AttachStoragePointConnectionDialog : LeftNavigableTabsForm
	{
		private class StoragePointConnectionConfiguration : IProxyOptionsContainer, ICertificateInclusionOptionsContainer
		{
			public IEnumerable<X509CertificateWrapper> Certificates { get; set; }

			public MLProxy Proxy { get; set; }
		}

		private StoragePointConnectionConfiguration m_configuration = new StoragePointConnectionConfiguration();

		private TCStoragePointConnectionConfig m_generalConfigTab = new TCStoragePointConnectionConfig();

		private TCProxyConfig m_proxyConfigTab = new TCProxyConfig();

		private TCCertificateInclusionConfig m_certificateConfigTab = new TCCertificateInclusionConfig();

		private IContainer components;

		public override ActionContext Context
		{
			set
			{
				m_generalConfigTab.Context = value;
				Node node = (Node)value.Targets[0];
				Dictionary<int, ExternalConnection> externalConnectionsOfType = node.GetExternalConnectionsOfType<StoragePointExternalConnection>(recurseUp: true);
				if (externalConnectionsOfType.Count > 0)
				{
					StoragePointExternalConnection storagePointExternalConnection = (StoragePointExternalConnection)externalConnectionsOfType.First().Value;
					m_generalConfigTab.Connection = storagePointExternalConnection;
					m_configuration.Proxy = storagePointExternalConnection.Proxy;
					m_configuration.Certificates = storagePointExternalConnection.IncludedCertificates;
				}
				m_proxyConfigTab.Options = m_configuration;
				m_certificateConfigTab.Options = m_configuration;
			}
		}

		public AttachStoragePointConnectionDialog()
		{
			InitializeComponent();
			List<TabbableControl> tabs = new List<TabbableControl> { m_generalConfigTab, m_proxyConfigTab, m_certificateConfigTab };
			base.Tabs = tabs;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		public Dictionary<int, ExternalConnection> GetSelectedConnections()
		{
			Dictionary<int, ExternalConnection> dictionary = new Dictionary<int, ExternalConnection>();
			if (base.Tag is ExternalConnection externalConnection)
			{
				dictionary.Add(externalConnection.ExternalConnectionID, externalConnection);
			}
			return dictionary;
		}

		private void InitializeComponent()
		{
			base.SuspendLayout();
			base.w_btnCancel.Location = new System.Drawing.Point(537, 347);
			base.w_btnOK.Location = new System.Drawing.Point(447, 347);
			base.Appearance.BackColor = System.Drawing.Color.White;
			base.Appearance.Options.UseBackColor = true;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(624, 382);
			this.MinimumSize = new System.Drawing.Size(640, 420);
			base.Name = "AttachStoragePointConnectionDialog";
			this.Text = "StoragePoint Connection";
			base.ResumeLayout(false);
		}

		protected override bool SaveUI()
		{
			if (!base.SaveUI())
			{
				return false;
			}
			if (!string.IsNullOrEmpty(m_generalConfigTab.Url))
			{
				StoragePointExternalConnection storagePointExternalConnection = new StoragePointExternalConnection
				{
					Url = m_generalConfigTab.Url,
					Credentials = m_generalConfigTab.Credentials,
					Proxy = m_proxyConfigTab.Proxy,
					IncludedCertificates = new X509CertificateWrapperCollection(m_certificateConfigTab.Certificates)
				};
				StoragePointExternalConnection storagePointExternalConnection2 = storagePointExternalConnection;
				storagePointExternalConnection2.CheckConnection();
				if (storagePointExternalConnection2.Status == ConnectionStatus.Invalid)
				{
					return false;
				}
				base.Tag = storagePointExternalConnection2;
			}
			else
			{
				base.Tag = null;
			}
			return true;
		}
	}
}
