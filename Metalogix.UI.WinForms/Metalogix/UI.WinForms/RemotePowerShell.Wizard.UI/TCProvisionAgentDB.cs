using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Metalogix.Jobs;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Jobs;
using Metalogix.Utilities;

namespace Metalogix.UI.WinForms.RemotePowerShell.Wizard.UI
{
    [ControlName("Provision Agent Database")]
    public class TCProvisionAgentDB : AgentWizardTabbableControl
    {
        private IContainer components;

        private GroupControl gbxConnectAgentDb;

        private TCXtraDBConnection xtraDBConnection;

        private RichTextBox tbxMessage;

        public JobListFullControl JobListFullControl { get; set; }

        public string PreviousAgentDbConnectionString { get; private set; }

        public TCProvisionAgentDB()
	{
		InitializeComponent();
		PreviousAgentDbConnectionString = JobsSettings.AdapterContext.ToInsecureString();
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
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.RemotePowerShell.Wizard.UI.TCProvisionAgentDB));
		this.tbxMessage = new System.Windows.Forms.RichTextBox();
		this.gbxConnectAgentDb = new DevExpress.XtraEditors.GroupControl();
		this.xtraDBConnection = new Metalogix.UI.WinForms.Components.TCXtraDBConnection();
		((System.ComponentModel.ISupportInitialize)this.gbxConnectAgentDb).BeginInit();
		this.gbxConnectAgentDb.SuspendLayout();
		base.SuspendLayout();
		this.tbxMessage.BackColor = System.Drawing.Color.White;
		this.tbxMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.tbxMessage.Location = new System.Drawing.Point(10, 10);
		this.tbxMessage.Name = "tbxMessage";
		this.tbxMessage.ReadOnly = true;
		this.tbxMessage.Size = new System.Drawing.Size(500, 112);
		this.tbxMessage.TabIndex = 9;
		this.tbxMessage.Text = componentResourceManager.GetString("tbxMessage.Text");
		this.gbxConnectAgentDb.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gbxConnectAgentDb.Appearance.BackColor = System.Drawing.Color.White;
		this.gbxConnectAgentDb.Appearance.Options.UseBackColor = true;
		this.gbxConnectAgentDb.Controls.Add(this.xtraDBConnection);
		this.gbxConnectAgentDb.Location = new System.Drawing.Point(13, 144);
		this.gbxConnectAgentDb.Name = "gbxConnectAgentDb";
		this.gbxConnectAgentDb.Size = new System.Drawing.Size(500, 262);
		this.gbxConnectAgentDb.TabIndex = 0;
		this.gbxConnectAgentDb.Text = "Connect to Agent Database";
		this.xtraDBConnection.AllowBrowsingDatabases = true;
		this.xtraDBConnection.AllowBrowsingNetworkServers = true;
		this.xtraDBConnection.AllowDatabaseCreation = true;
		this.xtraDBConnection.AllowDatabaseDeletion = true;
		this.xtraDBConnection.Appearance.BackColor = System.Drawing.Color.White;
		this.xtraDBConnection.Appearance.Options.UseBackColor = true;
		this.xtraDBConnection.EnableLocationEditing = true;
		this.xtraDBConnection.EnableRememberMe = false;
		this.xtraDBConnection.Location = new System.Drawing.Point(28, 22);
		this.xtraDBConnection.Name = "xtraDBConnection";
		this.xtraDBConnection.RememberMe = true;
		this.xtraDBConnection.ServerHistory = null;
		this.xtraDBConnection.Size = new System.Drawing.Size(440, 220);
		this.xtraDBConnection.SqlDatabaseName = "";
		this.xtraDBConnection.SqlLocalServers = (System.Collections.ArrayList)componentResourceManager.GetObject("xtraDBConnection.SqlLocalServers");
		this.xtraDBConnection.SqlNetworkServers = (System.Collections.ArrayList)componentResourceManager.GetObject("xtraDBConnection.SqlNetworkServers");
		this.xtraDBConnection.SqlServerName = "";
		this.xtraDBConnection.TabIndex = 9;
		base.Appearance.BackColor = System.Drawing.Color.White;
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.tbxMessage);
		base.Controls.Add(this.gbxConnectAgentDb);
		base.Name = "TCProvisionAgentDB";
		base.Size = new System.Drawing.Size(525, 420);
		((System.ComponentModel.ISupportInitialize)this.gbxConnectAgentDb).EndInit();
		this.gbxConnectAgentDb.ResumeLayout(false);
		base.ResumeLayout(false);
	}

        public override bool ValidatePage()
	{
		bool flag = false;
		try
		{
			if (xtraDBConnection.ConnectToDatabase("Agent Database"))
			{
				string connectionString = xtraDBConnection.GetConnectionString();
				if (JobListFullControl != null && !string.IsNullOrEmpty(connectionString) && JobListFullControl.ConnectToAgentDb(connectionString))
				{
					flag = true;
					Reset();
					AgentWizardTabbableControl.AgentDetails.AgentDBConnectionString = connectionString;
				}
			}
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException(ControlName, "Error occurred while configuring agent database.", exception, ErrorIcon.Error);
		}
		return flag;
	}
    }
}
