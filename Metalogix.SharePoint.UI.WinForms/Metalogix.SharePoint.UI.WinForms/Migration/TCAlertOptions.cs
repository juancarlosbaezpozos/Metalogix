using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ControlName("Alert Options")]
	public class TCAlertOptions : ScopableTabbableControl
	{
		private SPAlertOptions m_options;

		private bool m_bIndividual;

		private bool _isTargetSPO;

		private IContainer components;

		private CheckEdit w_cbCopyChildSiteAlerts;

		private CheckEdit w_cbCopyItemAlerts;

		internal CheckEdit cbxEncryptAzureMigrationJobs;

		internal CheckEdit cbxUseAzureUpload;

		public bool IndividuallyScoped
		{
			get
			{
				return m_bIndividual;
			}
			set
			{
				m_bIndividual = value;
				UpdateScope();
			}
		}

		public bool IsTargetSPO
		{
			get
			{
				return _isTargetSPO;
			}
			set
			{
				_isTargetSPO = value;
				HideAzureRelatedControls();
			}
		}

		public SPAlertOptions Options
		{
			get
			{
				return m_options;
			}
			set
			{
				m_options = value;
				LoadUI();
			}
		}

		public TCAlertOptions()
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

		private void HideAzureRelatedControls()
		{
			if (!IsTargetSPO)
			{
				HideControl(cbxUseAzureUpload);
				HideControl(cbxEncryptAzureMigrationJobs);
			}
		}

		private void InitializeComponent()
		{
			this.w_cbCopyChildSiteAlerts = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbCopyItemAlerts = new DevExpress.XtraEditors.CheckEdit();
			this.cbxEncryptAzureMigrationJobs = new DevExpress.XtraEditors.CheckEdit();
			this.cbxUseAzureUpload = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyChildSiteAlerts.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyItemAlerts.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.cbxEncryptAzureMigrationJobs.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.cbxUseAzureUpload.Properties).BeginInit();
			base.SuspendLayout();
			this.w_cbCopyChildSiteAlerts.Location = new System.Drawing.Point(6, 9);
			this.w_cbCopyChildSiteAlerts.Name = "w_cbCopyChildSiteAlerts";
			this.w_cbCopyChildSiteAlerts.Properties.AutoWidth = true;
			this.w_cbCopyChildSiteAlerts.Properties.Caption = "Recursively Copy Alerts for all Child Sites";
			this.w_cbCopyChildSiteAlerts.Size = new System.Drawing.Size(219, 19);
			this.w_cbCopyChildSiteAlerts.TabIndex = 0;
			this.w_cbCopyItemAlerts.Location = new System.Drawing.Point(6, 34);
			this.w_cbCopyItemAlerts.Name = "w_cbCopyItemAlerts";
			this.w_cbCopyItemAlerts.Properties.AutoWidth = true;
			this.w_cbCopyItemAlerts.Properties.Caption = "Recursively Copy All Item Alerts";
			this.w_cbCopyItemAlerts.Size = new System.Drawing.Size(176, 19);
			this.w_cbCopyItemAlerts.TabIndex = 2;
			this.cbxEncryptAzureMigrationJobs.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.cbxEncryptAzureMigrationJobs.Location = new System.Drawing.Point(24, 82);
			this.cbxEncryptAzureMigrationJobs.Name = "cbxEncryptAzureMigrationJobs";
			this.cbxEncryptAzureMigrationJobs.Properties.AutoWidth = true;
			this.cbxEncryptAzureMigrationJobs.Properties.Caption = "Encrypt Azure/SPO Container Jobs";
			this.cbxEncryptAzureMigrationJobs.Size = new System.Drawing.Size(163, 19);
			this.cbxEncryptAzureMigrationJobs.TabIndex = 18;
			this.cbxUseAzureUpload.EditValue = true;
			this.cbxUseAzureUpload.Enabled = false;
			this.cbxUseAzureUpload.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.cbxUseAzureUpload.Location = new System.Drawing.Point(6, 59);
			this.cbxUseAzureUpload.Name = "cbxUseAzureUpload";
			this.cbxUseAzureUpload.Properties.AutoWidth = true;
			this.cbxUseAzureUpload.Properties.Caption = "Use Azure/SPO Container Office 365 Upload";
			this.cbxUseAzureUpload.Size = new System.Drawing.Size(161, 19);
			this.cbxUseAzureUpload.TabIndex = 19;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.cbxUseAzureUpload);
			base.Controls.Add(this.cbxEncryptAzureMigrationJobs);
			base.Controls.Add(this.w_cbCopyItemAlerts);
			base.Controls.Add(this.w_cbCopyChildSiteAlerts);
			base.Name = "TCAlertOptions";
			base.Size = new System.Drawing.Size(344, 145);
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyChildSiteAlerts.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyItemAlerts.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.cbxEncryptAzureMigrationJobs.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.cbxUseAzureUpload.Properties).EndInit();
			base.ResumeLayout(false);
		}

		protected override void LoadUI()
		{
			w_cbCopyChildSiteAlerts.Checked = Options.CopyChildSiteAlerts;
			w_cbCopyItemAlerts.Checked = Options.CopyItemAlerts;
			cbxEncryptAzureMigrationJobs.Checked = Options.UseEncryptedAzureMigration;
			cbxUseAzureUpload.Checked = true;
		}

		public override bool SaveUI()
		{
			Options.CopyChildSiteAlerts = w_cbCopyChildSiteAlerts.Checked;
			Options.CopyItemAlerts = w_cbCopyItemAlerts.Checked;
			Options.UseEncryptedAzureMigration = cbxEncryptAzureMigrationJobs.Checked;
			Options.UseAzureUpload = true;
			return true;
		}

		protected override void UpdateScope()
		{
			if (IndividuallyScoped)
			{
				HideControl(w_cbCopyChildSiteAlerts);
			}
		}
	}
}
