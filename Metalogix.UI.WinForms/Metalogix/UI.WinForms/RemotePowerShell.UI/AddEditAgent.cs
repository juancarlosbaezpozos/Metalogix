using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Metalogix.Actions.Remoting;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Properties;
using Metalogix.UI.WinForms.RemotePowerShell.Actions;
using Metalogix.UI.WinForms.RemotePowerShell.Wizard.UI;

namespace Metalogix.UI.WinForms.RemotePowerShell.UI
{
    public class AddEditAgent : XtraForm
    {
        private delegate void SetTextCallback(string text);

        private const string AddAgent = "Add Agent";

        private const string EditAgent = "Edit Agent";

        private readonly string _dialogName = "Add Agent";

        private readonly bool _isEditAgent;

        private IContainer components;

        private SimpleButton btnConnect;

        private SimpleButton btnCancel;

        private AddEditAgentControl addEditAgentControl;

        private TCInstallCertificate tcInstallCertificate;

        public Agent Agent { get; set; }

        public List<Agent> ConfiguredAgents { get; set; }

        public sealed override string Text
        {
            get
		{
			return base.Text;
		}
            set
		{
			base.Text = value;
		}
        }

        public AddEditAgent(bool isEditAgent = false)
	{
		InitializeComponent();
		_isEditAgent = isEditAgent;
		if (_isEditAgent)
		{
			_dialogName = "Edit Agent";
			Bitmap editAgent16 = Resources.EditAgent16;
			base.Icon = Icon.FromHandle(editAgent16.GetHicon());
			tcInstallCertificate.HideControls();
			base.ClientSize = new Size(433, 249);
		}
		Text = _dialogName;
	}

        private void btnCancel_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
	}

        private void btnConnect_Click(object sender, EventArgs e)
	{
		try
		{
			base.DialogResult = DialogResult.None;
			if (!addEditAgentControl.ValidateControls(_dialogName) || (!_isEditAgent && !tcInstallCertificate.ValidateControls(_dialogName)))
			{
				return;
			}
			if (string.IsNullOrEmpty(addEditAgentControl.MachineName))
			{
				addEditAgentControl.MachineName = addEditAgentControl.MachineIP;
			}
			if (!AgentHelper.IsExistingAgent(ConfiguredAgents, addEditAgentControl.MachineName))
			{
				string empty = string.Empty;
				string str = string.Empty;
				if (!_isEditAgent)
				{
					List<KeyValuePair<DateTime, string>> keyValuePairs = new List<KeyValuePair<DateTime, string>>
					{
						new KeyValuePair<DateTime, string>(DateTime.Now, "Configuration Started.")
					};
					Agent = new Agent(Guid.NewGuid(), addEditAgentControl.MachineIP, addEditAgentControl.MachineName, empty, str, addEditAgentControl.UserName, addEditAgentControl.Password, AgentStatus.Configuring, keyValuePairs);
				}
				else
				{
					Agent.UserName = addEditAgentControl.UserName;
					Agent.Password = addEditAgentControl.Password;
				}
				IRemoteWorker remoteWorker = new RemoteWorker(Agent);
				if (!remoteWorker.Connect())
				{
					FlatXtraMessageBox.Show(this, "Could not connect to remote agent.", "Unable to connect to agent", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
				else if (_isEditAgent || DeployCertificate(remoteWorker))
				{
					base.DialogResult = DialogResult.OK;
					Close();
				}
			}
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException("Unable to connect to agent", "Could not connect to remote agent.", exception, ErrorIcon.Error);
		}
	}

        private bool DeployCertificate(IRemoteWorker worker)
	{
		if (!File.Exists(tcInstallCertificate.CertificatePath))
		{
			string str = $"Certificate not found at given location.\nPlease make sure certificate exists at given location.";
			FlatXtraMessageBox.Show(str, "Certificate Not Found", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return false;
		}
		string str1 = AgentHelper.DeployCertificate(worker, tcInstallCertificate.CertificatePath, tcInstallCertificate.Password);
		if (str1.Contains("already exists") || str1.Contains("added to store"))
		{
			return true;
		}
		FlatXtraMessageBox.Show("Invalid certificate credentials.", _dialogName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
		return false;
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
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.RemotePowerShell.UI.AddEditAgent));
		this.btnConnect = new DevExpress.XtraEditors.SimpleButton();
		this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
		this.tcInstallCertificate = new Metalogix.UI.WinForms.RemotePowerShell.Wizard.UI.TCInstallCertificate();
		this.addEditAgentControl = new Metalogix.UI.WinForms.Components.AddEditAgentControl();
		base.SuspendLayout();
		this.btnConnect.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnConnect.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.btnConnect.Location = new System.Drawing.Point(266, 317);
		this.btnConnect.Name = "btnConnect";
		this.btnConnect.Size = new System.Drawing.Size(75, 23);
		this.btnConnect.TabIndex = 58;
		this.btnConnect.Text = "Connect";
		this.btnConnect.Click += new System.EventHandler(btnConnect_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.Location = new System.Drawing.Point(350, 317);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 59;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(btnCancel_Click);
		this.tcInstallCertificate.Location = new System.Drawing.Point(12, 207);
		this.tcInstallCertificate.Name = "tcInstallCertificate";
		this.tcInstallCertificate.Size = new System.Drawing.Size(409, 97);
		this.tcInstallCertificate.TabIndex = 61;
		this.addEditAgentControl.Appearance.BackColor = System.Drawing.Color.White;
		this.addEditAgentControl.Appearance.Options.UseBackColor = true;
		this.addEditAgentControl.Location = new System.Drawing.Point(-8, 9);
		this.addEditAgentControl.MachineName = null;
		this.addEditAgentControl.Name = "addEditAgentControl";
		this.addEditAgentControl.Size = new System.Drawing.Size(450, 206);
		this.addEditAgentControl.TabIndex = 60;
		base.AcceptButton = this.btnConnect;
		base.Appearance.BackColor = System.Drawing.Color.White;
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(433, 351);
		base.Controls.Add(this.tcInstallCertificate);
		base.Controls.Add(this.addEditAgentControl);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnConnect);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "AddEditAgent";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Add Agent";
		base.ResumeLayout(false);
	}

        public void LoadUI()
	{
		addEditAgentControl.LoadUI(Agent);
	}
    }
}
