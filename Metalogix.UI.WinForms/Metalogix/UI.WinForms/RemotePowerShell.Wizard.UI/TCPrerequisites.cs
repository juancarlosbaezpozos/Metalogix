using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Metalogix.Actions.Remoting;
using Metalogix.UI.WinForms.Components;
using Metalogix.Widgets;

namespace Metalogix.UI.WinForms.RemotePowerShell.Wizard.UI
{
    [ControlName("Prerequisites")]
    public class TCPrerequisites : AgentWizardTabbableControl
    {
        private delegate void SetNodeStatusCallback(string nodeName, int imageIndex);

        private delegate void ShowButtonCallback(bool isEnabled);

        private const string OperationSystem = "OperationSystem";

        private const string PSScript = "PSScript";

        private const string DotNetFrameWork = ".NET 4.5.2";

        private const string PowershellVersion = "3.0";

        private IRemoteWorker _worker;

        private readonly Dictionary<string, string> _prerequisites;

        private IContainer components;

        private ImageList imgServices;

        private LabelControl lblMessage;

        private EnhancedTreeView tvServices;

        private SimpleButton btnEnableMissingServices;

        public TCPrerequisites()
	{
		InitializeComponent();
		Dictionary<string, string> strs = new Dictionary<string, string>
		{
			{ "RemoteRegistry", "Remote Registry service is running" },
			{ "LanmanWorkstation", "Workstation service is running" },
			{ "PSScript", "PowerShell script can be executed" },
			{ "OperationSystem", "Windows Server 2008 R2 or later" },
			{ ".NET 4.5.2", "Microsoft .NET Framework 4.5.2 or later" },
			{ "3.0", "Windows PowerShell 3.0 or later" }
		};
		_prerequisites = strs;
	}

        private void btnEnableMissingServices_Click(object sender, EventArgs e)
	{
		SetNextButtonState(isEnabled: false);
		SetBackButtonState(isEnabled: false);
		new Thread(StartMissingServices).Start();
	}

        private void CheckServiceStatus()
	{
		new Thread(DisplayCurrentServiceStatus).Start();
	}

        private void DisplayCurrentServiceStatus()
	{
		try
		{
			bool flag = true;
			bool dotNetFrameWork = true;
			_worker = new RemoteWorker(AgentWizardTabbableControl.AgentDetails);
			foreach (string key in _prerequisites.Keys)
			{
				string str = key;
				string str1 = str;
				if (str != null)
				{
					switch (str1)
					{
					case "PSScript":
						SetNodeStatus(key, 1);
						continue;
					case "OperationSystem":
					{
						string str2 = _worker.RunCommand("(Get-WmiObject -class Win32_OperatingSystem).Version");
						string str3 = _worker.RunCommand("(Get-WmiObject -class Win32_OperatingSystem).ProductType");
						if (string.IsNullOrEmpty(str2) || string.IsNullOrEmpty(str3))
						{
							SetNodeStatus(key, 3);
							continue;
						}
						Version version = new Version(str2);
						if (version.Major < 6 || version.Minor < 1 || !str3.Equals("3"))
						{
							SetNodeStatus(key, 3);
						}
						else
						{
							SetNodeStatus(key, 1);
						}
						continue;
					}
					case ".NET 4.5.2":
						dotNetFrameWork = GetDotNetFrameWork(key);
						continue;
					case "3.0":
						if (!IsPowerShell3OrLater(key))
						{
							dotNetFrameWork = false;
						}
						continue;
					}
				}
				if (!_worker.RunCommand($"(Get-Service -ServiceName \"{key}\").Status").Equals("Running", StringComparison.InvariantCultureIgnoreCase))
				{
					SetNodeStatus(key, 2);
					flag = false;
				}
				else
				{
					SetNodeStatus(key, 1);
				}
			}
			if (!flag)
			{
				ShowEnableMissingServicesButton(isEnabled: true);
				SetNextButtonState(isEnabled: false);
			}
			else if (dotNetFrameWork)
			{
				SetNextButtonState(isEnabled: true);
			}
			else
			{
				SetNextButtonState(isEnabled: false);
			}
			SetBackButtonState(isEnabled: true);
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException(ControlName, "Error occurred while checking status of services.", exception, ErrorIcon.Error);
			ShowEnableMissingServicesButton(isEnabled: true);
		}
	}

        protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

        private bool GetDotNetFrameWork(string key)
	{
		if (Utils.IsDotNetFramework452(_worker))
		{
			SetNodeStatus(key, 1);
			return true;
		}
		SetNodeStatus(key, 2);
		return false;
	}

        private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.RemotePowerShell.Wizard.UI.TCPrerequisites));
		this.imgServices = new System.Windows.Forms.ImageList(this.components);
		this.lblMessage = new DevExpress.XtraEditors.LabelControl();
		this.tvServices = new Metalogix.Widgets.EnhancedTreeView();
		this.btnEnableMissingServices = new DevExpress.XtraEditors.SimpleButton();
		base.SuspendLayout();
		this.imgServices.ImageStream = (System.Windows.Forms.ImageListStreamer)componentResourceManager.GetObject("imgServices.ImageStream");
		this.imgServices.TransparentColor = System.Drawing.Color.Transparent;
		this.imgServices.Images.SetKeyName(0, "CheckWait.gif");
		this.imgServices.Images.SetKeyName(1, "CheckOk.gif");
		this.imgServices.Images.SetKeyName(2, "CheckFail.gif");
		this.imgServices.Images.SetKeyName(3, "warning.gif");
		this.lblMessage.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.lblMessage.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Horizontal;
		this.lblMessage.Location = new System.Drawing.Point(10, 10);
		this.lblMessage.Name = "lblMessage";
		this.lblMessage.Size = new System.Drawing.Size(197, 13);
		this.lblMessage.TabIndex = 12;
		this.lblMessage.Text = "Checking the Prerequisites on the Agent.";
		this.tvServices.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.tvServices.CheckBoxesEnhanced = false;
		this.tvServices.ImageIndex = 0;
		this.tvServices.ImageList = this.imgServices;
		this.tvServices.ItemHeight = 25;
		this.tvServices.Location = new System.Drawing.Point(10, 45);
		this.tvServices.MultiSelectEnabled = false;
		this.tvServices.MultiSelectLimitationMethod = null;
		this.tvServices.Name = "tvServices";
		this.tvServices.SelectedImageIndex = 0;
		this.tvServices.SelectedNodes = (System.Collections.ObjectModel.ReadOnlyCollection<System.Windows.Forms.TreeNode>)componentResourceManager.GetObject("tvServices.SelectedNodes");
		this.tvServices.ShowLines = false;
		this.tvServices.ShowNodeToolTips = true;
		this.tvServices.ShowPlusMinus = false;
		this.tvServices.ShowRootLines = false;
		this.tvServices.Size = new System.Drawing.Size(382, 154);
		this.tvServices.SmudgeProtection = false;
		this.tvServices.TabIndex = 11;
		this.tvServices.UseDoubleBuffering = true;
		this.btnEnableMissingServices.Enabled = false;
		this.btnEnableMissingServices.Location = new System.Drawing.Point(36, 210);
		this.btnEnableMissingServices.Name = "btnEnableMissingServices";
		this.btnEnableMissingServices.Size = new System.Drawing.Size(160, 30);
		this.btnEnableMissingServices.TabIndex = 13;
		this.btnEnableMissingServices.Text = "Enable Missing Services";
		this.btnEnableMissingServices.Click += new System.EventHandler(btnEnableMissingServices_Click);
		base.Appearance.BackColor = System.Drawing.Color.White;
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.lblMessage);
		base.Controls.Add(this.tvServices);
		base.Controls.Add(this.btnEnableMissingServices);
		base.Name = "TCPrerequisites";
		base.Size = new System.Drawing.Size(525, 277);
		base.ResumeLayout(false);
		base.PerformLayout();
	}

        private bool IsPowerShell3OrLater(string key)
	{
		string str = _worker.RunCommand("$PSVersionTable.PSVersion.Major");
		if (!string.IsNullOrEmpty(str) && int.TryParse(str, out var num) && num >= 3)
		{
			SetNodeStatus(key, 1);
			return true;
		}
		SetNodeStatus(key, 2);
		return false;
	}

        private void LoadTreeView()
	{
		tvServices.Nodes.Clear();
		foreach (string key in _prerequisites.Keys)
		{
			tvServices.Nodes.Add(key, Convert.ToString(_prerequisites[key]), 0);
		}
	}

        public override bool LoadUI()
	{
		LoadTreeView();
		SetNextButtonState(isEnabled: false);
		SetBackButtonState(isEnabled: false);
		btnEnableMissingServices.Visible = true;
		btnEnableMissingServices.Enabled = false;
		CheckServiceStatus();
		return true;
	}

        private void SetNodeStatus(string nodeName, int imageIndex)
	{
		if (!tvServices.InvokeRequired)
		{
			TreeNode item = tvServices.Nodes[nodeName];
			if (item != null)
			{
				item.ImageIndex = imageIndex;
			}
		}
		else
		{
			SetNodeStatusCallback setNodeStatusCallback = SetNodeStatus;
			object[] objArray = new object[2] { nodeName, imageIndex };
			Invoke(setNodeStatusCallback, objArray);
		}
	}

        private void ShowEnableMissingServicesButton(bool isEnabled)
	{
		if (!btnEnableMissingServices.InvokeRequired)
		{
			btnEnableMissingServices.Enabled = isEnabled;
			return;
		}
		ShowButtonCallback showButtonCallback = ShowEnableMissingServicesButton;
		object[] objArray = new object[1] { isEnabled };
		Invoke(showButtonCallback, objArray);
	}

        private void StartMissingServices()
	{
		try
		{
			bool flag = false;
			bool flag1 = false;
			bool flag2 = false;
			foreach (string key in _prerequisites.Keys)
			{
				if (!tvServices.Nodes[key].ImageIndex.Equals(2))
				{
					continue;
				}
				string str = _worker.RunCommand($"(Get-Service -ServiceName \"{key}\") | start-service");
				if (key.Equals(".NET 4.5.2", StringComparison.InvariantCultureIgnoreCase) || key.Equals("3.0", StringComparison.InvariantCultureIgnoreCase))
				{
					flag2 = true;
					continue;
				}
				if (string.IsNullOrEmpty(str))
				{
					SetNodeStatus(key, 1);
					continue;
				}
				if (str.IndexOf("cannot be started", StringComparison.InvariantCultureIgnoreCase) >= 0 || str.IndexOf("Failed to start service", StringComparison.InvariantCultureIgnoreCase) >= 0)
				{
					SetNodeStatus(key, 2);
					flag = true;
					continue;
				}
				if (str.IndexOf("Couldn't access", StringComparison.InvariantCultureIgnoreCase) < 0)
				{
					SetNodeStatus(key, 2);
					continue;
				}
				string str1 = "Could not start the services. Please make sure agent is up and running.";
				Invoke((Action)delegate
				{
					FlatXtraMessageBox.Show(str1, "Agent Stopped", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				});
				flag1 = true;
				break;
			}
			if (flag)
			{
				string str2 = "Services marked as error cannot be started automatically. Please start these services manually.";
				Invoke((Action)delegate
				{
					FlatXtraMessageBox.Show(base.TopLevelControl, str2, "Disabled Services", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				});
			}
			else if (!flag1)
			{
				SetNextButtonState(!flag2);
				ShowEnableMissingServicesButton(isEnabled: false);
			}
			else
			{
				ShowEnableMissingServicesButton(isEnabled: true);
			}
			SetBackButtonState(isEnabled: true);
		}
		catch (Exception exception)
		{
			GlobalServices.ErrorHandler.HandleException(ControlName, "Error occurred while starting the services.", exception, ErrorIcon.Error);
			ShowEnableMissingServicesButton(isEnabled: true);
		}
	}
    }
}
