using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Metalogix.Metabase;
using Metalogix.Metabase.Adapters;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.UI.WinForms.RemotePowerShell.Wizard.UI
{
    [ControlName("Configure Metabase")]
    public class TCConfigureMetabase : AgentWizardTabbableControl
    {
        private IContainer components;

        private GroupControl gbxMetabaseConnection;

        private TCXtraDBConnection xtraDBConnection;

        private RichTextBox tbxMessage;

        public TCConfigureMetabase()
	{
		InitializeComponent();
	}

        protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

        private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.RemotePowerShell.Wizard.UI.TCConfigureMetabase));
		this.gbxMetabaseConnection = new DevExpress.XtraEditors.GroupControl();
		this.xtraDBConnection = new Metalogix.UI.WinForms.Components.TCXtraDBConnection();
		this.tbxMessage = new System.Windows.Forms.RichTextBox();
		((System.ComponentModel.ISupportInitialize)this.gbxMetabaseConnection).BeginInit();
		this.gbxMetabaseConnection.SuspendLayout();
		base.SuspendLayout();
		this.gbxMetabaseConnection.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gbxMetabaseConnection.Controls.Add(this.xtraDBConnection);
		this.gbxMetabaseConnection.Location = new System.Drawing.Point(13, 144);
		this.gbxMetabaseConnection.Name = "gbxMetabaseConnection";
		this.gbxMetabaseConnection.Size = new System.Drawing.Size(500, 262);
		this.gbxMetabaseConnection.TabIndex = 0;
		this.gbxMetabaseConnection.Text = "Configure Metabase";
		this.xtraDBConnection.AllowBrowsingDatabases = true;
		this.xtraDBConnection.AllowBrowsingNetworkServers = true;
		this.xtraDBConnection.AllowDatabaseCreation = true;
		this.xtraDBConnection.AllowDatabaseDeletion = true;
		this.xtraDBConnection.Appearance.BackColor = System.Drawing.Color.White;
		this.xtraDBConnection.Appearance.Options.UseBackColor = true;
		this.xtraDBConnection.EnableLocationEditing = true;
		this.xtraDBConnection.EnableRememberMe = false;
		this.xtraDBConnection.Location = new System.Drawing.Point(28, 22);
		this.xtraDBConnection.Margin = new System.Windows.Forms.Padding(10);
		this.xtraDBConnection.Name = "xtraDBConnection";
		this.xtraDBConnection.RememberMe = true;
		this.xtraDBConnection.ServerHistory = null;
		this.xtraDBConnection.Size = new System.Drawing.Size(440, 220);
		this.xtraDBConnection.SqlDatabaseName = "";
		this.xtraDBConnection.SqlLocalServers = (System.Collections.ArrayList)componentResourceManager.GetObject("xtraDBConnection.SqlLocalServers");
		this.xtraDBConnection.SqlNetworkServers = (System.Collections.ArrayList)componentResourceManager.GetObject("xtraDBConnection.SqlNetworkServers");
		this.xtraDBConnection.SqlServerName = "";
		this.xtraDBConnection.TabIndex = 8;
		this.tbxMessage.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tbxMessage.BackColor = System.Drawing.Color.White;
		this.tbxMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.tbxMessage.Location = new System.Drawing.Point(10, 10);
		this.tbxMessage.Name = "tbxMessage";
		this.tbxMessage.ReadOnly = true;
		this.tbxMessage.Size = new System.Drawing.Size(475, 67);
		this.tbxMessage.TabIndex = 7;
		this.tbxMessage.Text = componentResourceManager.GetString("tbxMessage.Text");
		base.Appearance.BackColor = System.Drawing.Color.White;
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.gbxMetabaseConnection);
		base.Controls.Add(this.tbxMessage);
		base.Name = "TCConfigureMetabase";
		base.Size = new System.Drawing.Size(525, 420);
		((System.ComponentModel.ISupportInitialize)this.gbxMetabaseConnection).EndInit();
		this.gbxMetabaseConnection.ResumeLayout(false);
		base.ResumeLayout(false);
	}

        public override bool ValidatePage()
	{
		bool flag = false;
		try
		{
			if (xtraDBConnection.ConnectToDatabase(ControlName))
			{
				string connectionString = xtraDBConnection.GetConnectionString();
				if (!string.IsNullOrEmpty(connectionString))
				{
					ConfigurationVariables.DefaultMetabaseAdapter = DatabaseAdapterType.SqlServer.ToString();
					ConfigurationVariables.AutoProvisionNewMetabaseFile = true;
					ConfigurationVariables.FileMetabaseContext = ApplicationData.ApplicationPath;
					ConfigurationVariables.SQLMetabaseContext = connectionString;
					flag = true;
					AgentWizardTabbableControl.AgentDetails.MetabaseDBConnectionString = connectionString;
				}
			}
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException(ControlName, "Error occurred while configuring metabase database.", exception, ErrorIcon.Error);
		}
		return flag;
	}
    }
}
