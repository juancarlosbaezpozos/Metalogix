using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;

namespace Metalogix.SharePoint.UI.WinForms.Migration.BasicView
{
	public class UpdateItemOptionsDialogBasicView : XtraForm
	{
		private int _updateItemOptionsBitField;

		private IContainer components;

		private ToggleSwitch tsUpdateCoreMetadata;

		private SimpleButton btnOK;

		private SimpleButton btnCancel;

		private ToggleSwitch tsUpdatePermissions;

		public int UpdateItemOptionsBitField => _updateItemOptionsBitField;

		public UpdateItemOptionsDialogBasicView(int updateItemBitField)
		{
			InitializeComponent();
			_updateItemOptionsBitField = updateItemBitField;
			LoadUI();
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			SaveUI();
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
			this.tsUpdateCoreMetadata = new DevExpress.XtraEditors.ToggleSwitch();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.tsUpdatePermissions = new DevExpress.XtraEditors.ToggleSwitch();
			((System.ComponentModel.ISupportInitialize)this.tsUpdateCoreMetadata.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.tsUpdatePermissions.Properties).BeginInit();
			base.SuspendLayout();
			this.tsUpdateCoreMetadata.Location = new System.Drawing.Point(12, 12);
			this.tsUpdateCoreMetadata.Name = "tsUpdateCoreMetadata";
			this.tsUpdateCoreMetadata.Properties.Appearance.Options.UseTextOptions = true;
			this.tsUpdateCoreMetadata.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.tsUpdateCoreMetadata.Properties.OffText = "Update Core Metadata";
			this.tsUpdateCoreMetadata.Properties.OnText = "Update Core Metadata";
			this.tsUpdateCoreMetadata.Size = new System.Drawing.Size(190, 24);
			this.tsUpdateCoreMetadata.TabIndex = 0;
			this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(35, 76);
			this.btnOK.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
			this.btnOK.LookAndFeel.UseDefaultLookAndFeel = false;
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(btnOK_Click);
			this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(124, 76);
			this.btnCancel.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
			this.btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "Cancel";
			this.tsUpdatePermissions.Location = new System.Drawing.Point(13, 42);
			this.tsUpdatePermissions.Name = "tsUpdatePermissions";
			this.tsUpdatePermissions.Properties.Appearance.Options.UseTextOptions = true;
			this.tsUpdatePermissions.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.tsUpdatePermissions.Properties.OffText = "Update Permissions";
			this.tsUpdatePermissions.Properties.OnText = "Update Permissions";
			this.tsUpdatePermissions.Size = new System.Drawing.Size(171, 24);
			this.tsUpdatePermissions.TabIndex = 1;
			base.AcceptButton = this.btnOK;
			base.Appearance.ForeColor = System.Drawing.Color.FromArgb(32, 31, 53);
			base.Appearance.Options.UseForeColor = true;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.btnCancel;
			base.ClientSize = new System.Drawing.Size(234, 114);
			base.ControlBox = false;
			base.Controls.Add(this.tsUpdatePermissions);
			base.Controls.Add(this.btnCancel);
			base.Controls.Add(this.btnOK);
			base.Controls.Add(this.tsUpdateCoreMetadata);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "UpdateItemOptionsDialogBasicView";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Update Items / Documents Options";
			((System.ComponentModel.ISupportInitialize)this.tsUpdateCoreMetadata.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.tsUpdatePermissions.Properties).EndInit();
			base.ResumeLayout(false);
		}

		private void LoadUI()
		{
			tsUpdateCoreMetadata.IsOn = (1 & _updateItemOptionsBitField) > 0;
			tsUpdatePermissions.IsOn = (2 & _updateItemOptionsBitField) > 0;
		}

		private void SaveUI()
		{
			_updateItemOptionsBitField = 0;
			_updateItemOptionsBitField |= (tsUpdateCoreMetadata.IsOn ? 1 : 0);
			_updateItemOptionsBitField |= (tsUpdatePermissions.IsOn ? 2 : 0);
		}
	}
}
