using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Metalogix.Actions;
using Metalogix.SharePoint.Options.Migration;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class CopyRolesDialog : XtraForm
	{
		private PasteRolesOptions m_options;

		private ConfigurationResult m_result = ConfigurationResult.Cancel;

		private IContainer components;

		private CheckEdit w_cbRecursive;

		protected SimpleButton w_btnRun;

		protected SimpleButton w_btnSave;

		protected SimpleButton w_btnCancel;

		public ConfigurationResult ConfigurationResult
		{
			get
			{
				return m_result;
			}
			set
			{
				m_result = value;
			}
		}

		public PasteRolesOptions Options
		{
			get
			{
				return m_options;
			}
			set
			{
				m_options = value;
				if (value != null)
				{
					LoadUI();
				}
			}
		}

		public CopyRolesDialog()
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.CopyRolesDialog));
			this.w_cbRecursive = new DevExpress.XtraEditors.CheckEdit();
			this.w_btnRun = new DevExpress.XtraEditors.SimpleButton();
			this.w_btnSave = new DevExpress.XtraEditors.SimpleButton();
			this.w_btnCancel = new DevExpress.XtraEditors.SimpleButton();
			base.SuspendLayout();
			resources.ApplyResources(this.w_cbRecursive, "w_cbRecursive");
			this.w_cbRecursive.Name = "w_cbRecursive";
			resources.ApplyResources(this.w_btnRun, "w_btnRun");
			this.w_btnRun.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.w_btnRun.Name = "w_btnRun";
			this.w_btnRun.Click += new System.EventHandler(On_Run_Clicked);
			resources.ApplyResources(this.w_btnSave, "w_btnSave");
			this.w_btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.w_btnSave.Name = "w_btnSave";
			this.w_btnSave.Click += new System.EventHandler(On_Save_Clicked);
			resources.ApplyResources(this.w_btnCancel, "w_btnCancel");
			this.w_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_btnCancel.Name = "w_btnCancel";
			this.w_btnCancel.Click += new System.EventHandler(On_Cancel_Clicked);
			base.AcceptButton = this.w_btnRun;
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.w_btnRun);
			base.Controls.Add(this.w_btnSave);
			base.Controls.Add(this.w_btnCancel);
			base.Controls.Add(this.w_cbRecursive);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "CopyRolesDialog";
			base.ShowInTaskbar = false;
			base.Shown += new System.EventHandler(On_Shown);
			base.ResumeLayout(false);
		}

		private void LoadUI()
		{
			w_cbRecursive.Checked = Options.RecursivelyCopyPermissionLevels;
		}

		private void On_Cancel_Clicked(object sender, EventArgs e)
		{
			m_result = ConfigurationResult.Cancel;
		}

		private void On_Run_Clicked(object sender, EventArgs e)
		{
			SaveUI();
			m_result = ConfigurationResult.Run;
		}

		private void On_Save_Clicked(object sender, EventArgs e)
		{
			SaveUI();
			m_result = ConfigurationResult.Save;
		}

		private void On_Shown(object sender, EventArgs e)
		{
			w_btnRun.Focus();
		}

		private void SaveUI()
		{
			Options.RecursivelyCopyPermissionLevels = w_cbRecursive.Checked;
		}
	}
}
