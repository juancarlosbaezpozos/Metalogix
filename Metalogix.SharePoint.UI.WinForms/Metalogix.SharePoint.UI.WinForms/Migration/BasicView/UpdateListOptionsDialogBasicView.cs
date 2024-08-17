using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;

namespace Metalogix.SharePoint.UI.WinForms.Migration.BasicView
{
	public class UpdateListOptionsDialogBasicView : XtraForm
	{
		private int _updateListOptions;

		private IContainer components;

		private ToggleSwitch tsCoreMetadata;

		private ToggleSwitch tsPermissions;

		private ToggleSwitch tsContentTypes;

		private ToggleSwitch tsViews;

		private ToggleSwitch tsFields;

		private SimpleButton btnCancel;

		private SimpleButton btnOk;

		public int UpdateListOptions => _updateListOptions;

		public UpdateListOptionsDialogBasicView(int iUpdateListBitField)
		{
			InitializeComponent();
			_updateListOptions = iUpdateListBitField;
			LoadUI();
		}

		private void btnOk_Click(object sender, EventArgs e)
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
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.tsPermissions = new DevExpress.XtraEditors.ToggleSwitch();
			this.tsContentTypes = new DevExpress.XtraEditors.ToggleSwitch();
			this.tsViews = new DevExpress.XtraEditors.ToggleSwitch();
			this.tsFields = new DevExpress.XtraEditors.ToggleSwitch();
			this.tsCoreMetadata = new DevExpress.XtraEditors.ToggleSwitch();
			((System.ComponentModel.ISupportInitialize)this.tsPermissions.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.tsContentTypes.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.tsViews.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.tsFields.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.tsCoreMetadata.Properties).BeginInit();
			base.SuspendLayout();
			this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(116, 166);
			this.btnCancel.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
			this.btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "Cancel";
			this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point(27, 166);
			this.btnOk.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
			this.btnOk.LookAndFeel.UseDefaultLookAndFeel = false;
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 5;
			this.btnOk.Text = "OK";
			this.btnOk.Click += new System.EventHandler(btnOk_Click);
			this.tsPermissions.Location = new System.Drawing.Point(12, 132);
			this.tsPermissions.Name = "tsPermissions";
			this.tsPermissions.Properties.Appearance.Options.UseTextOptions = true;
			this.tsPermissions.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.tsPermissions.Properties.OffText = "Update Permissions";
			this.tsPermissions.Properties.OnText = "Update Permissions";
			this.tsPermissions.Size = new System.Drawing.Size(173, 24);
			this.tsPermissions.TabIndex = 4;
			this.tsContentTypes.Location = new System.Drawing.Point(12, 102);
			this.tsContentTypes.Name = "tsContentTypes";
			this.tsContentTypes.Properties.Appearance.Options.UseTextOptions = true;
			this.tsContentTypes.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.tsContentTypes.Properties.OffText = "Update Content Types";
			this.tsContentTypes.Properties.OnText = "Update Content Types";
			this.tsContentTypes.Size = new System.Drawing.Size(189, 24);
			this.tsContentTypes.TabIndex = 3;
			this.tsViews.Location = new System.Drawing.Point(12, 72);
			this.tsViews.Name = "tsViews";
			this.tsViews.Properties.Appearance.Options.UseTextOptions = true;
			this.tsViews.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.tsViews.Properties.OffText = "Update Views";
			this.tsViews.Properties.OnText = "Update Views";
			this.tsViews.Size = new System.Drawing.Size(145, 24);
			this.tsViews.TabIndex = 2;
			this.tsFields.Location = new System.Drawing.Point(12, 42);
			this.tsFields.Name = "tsFields";
			this.tsFields.Properties.Appearance.Options.UseTextOptions = true;
			this.tsFields.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.tsFields.Properties.OffText = "Update Fields";
			this.tsFields.Properties.OnText = "Update Fields";
			this.tsFields.Size = new System.Drawing.Size(145, 24);
			this.tsFields.TabIndex = 1;
			this.tsCoreMetadata.Location = new System.Drawing.Point(12, 12);
			this.tsCoreMetadata.Name = "tsCoreMetadata";
			this.tsCoreMetadata.Properties.Appearance.Options.UseTextOptions = true;
			this.tsCoreMetadata.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.tsCoreMetadata.Properties.OffText = "Update Core Metadata";
			this.tsCoreMetadata.Properties.OnText = "Update Core Metadata";
			this.tsCoreMetadata.Size = new System.Drawing.Size(190, 24);
			this.tsCoreMetadata.TabIndex = 0;
			base.AcceptButton = this.btnOk;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.btnCancel;
			base.ClientSize = new System.Drawing.Size(217, 205);
			base.ControlBox = false;
			base.Controls.Add(this.btnCancel);
			base.Controls.Add(this.btnOk);
			base.Controls.Add(this.tsPermissions);
			base.Controls.Add(this.tsContentTypes);
			base.Controls.Add(this.tsViews);
			base.Controls.Add(this.tsFields);
			base.Controls.Add(this.tsCoreMetadata);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "UpdateListOptionsDialogBasicView";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Update List Options";
			((System.ComponentModel.ISupportInitialize)this.tsPermissions.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.tsContentTypes.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.tsViews.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.tsFields.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.tsCoreMetadata.Properties).EndInit();
			base.ResumeLayout(false);
		}

		private void LoadUI()
		{
			tsCoreMetadata.IsOn = (1 & _updateListOptions) > 0;
			tsContentTypes.IsOn = (0x10 & _updateListOptions) > 0;
			tsFields.IsOn = (2 & _updateListOptions) > 0;
			tsPermissions.IsOn = (8 & _updateListOptions) > 0;
			tsViews.IsOn = (4 & _updateListOptions) > 0;
		}

		private void SaveUI()
		{
			_updateListOptions = 0;
			_updateListOptions |= (tsCoreMetadata.IsOn ? 1 : 0);
			_updateListOptions |= (tsContentTypes.IsOn ? 16 : 0);
			_updateListOptions |= (tsFields.IsOn ? 2 : 0);
			_updateListOptions |= (tsPermissions.IsOn ? 8 : 0);
			_updateListOptions |= (tsViews.IsOn ? 4 : 0);
		}
	}
}
