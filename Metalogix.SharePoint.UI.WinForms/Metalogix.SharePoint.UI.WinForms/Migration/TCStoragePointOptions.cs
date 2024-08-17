using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Components;
using TooltipsTest;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Migration.StoragePointOptions.png")]
	[ControlName("StoragePoint Options")]
	[UsesGroupBox(false)]
	public class TCStoragePointOptions : TabbableControl
	{
		private enum StoragePointPresent
		{
			NotPresent,
			Present,
			PresentOnSome
		}

		private StoragePointPresent _storagePointPresent;

		private IContainer components;

		private LabelControl w_lIntroduction;

		private LabelControl w_lStoragePointDetectedLabel;

		private LabelControl w_lStoragePointFoundValue;

		protected GroupControl w_gbStoragePoint;

		private CheckEdit w_cbSideloadDocs;

		private HelpTipButton w_helpSideloadDocs;

		public StoragePointOptions Options
		{
			get
			{
				return new StoragePointOptions
				{
					SideLoadDocuments = w_cbSideloadDocs.Checked
				};
			}
			set
			{
				if (value != null)
				{
					w_cbSideloadDocs.Checked = value.SideLoadDocuments;
				}
			}
		}

		public TCStoragePointOptions()
		{
			InitializeComponent();
			Type type = GetType();
			w_helpSideloadDocs.SetResourceString(type.FullName + w_cbSideloadDocs.Name, type);
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.TCStoragePointOptions));
			this.w_lIntroduction = new DevExpress.XtraEditors.LabelControl();
			this.w_lStoragePointDetectedLabel = new DevExpress.XtraEditors.LabelControl();
			this.w_lStoragePointFoundValue = new DevExpress.XtraEditors.LabelControl();
			this.w_gbStoragePoint = new DevExpress.XtraEditors.GroupControl();
			this.w_helpSideloadDocs = new TooltipsTest.HelpTipButton();
			this.w_cbSideloadDocs = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)this.w_gbStoragePoint).BeginInit();
			this.w_gbStoragePoint.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_helpSideloadDocs).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbSideloadDocs.Properties).BeginInit();
			base.SuspendLayout();
			this.w_lIntroduction.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.w_lIntroduction.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
			this.w_lIntroduction.Location = new System.Drawing.Point(4, 1);
			this.w_lIntroduction.Name = "w_lIntroduction";
			this.w_lIntroduction.Size = new System.Drawing.Size(602, 26);
			this.w_lIntroduction.TabIndex = 0;
			this.w_lIntroduction.Text = "If Metalogix StoragePoint is installed on the target server, you can use the following settings to enhance the speed of your migration.";
			this.w_lStoragePointDetectedLabel.Location = new System.Drawing.Point(4, 30);
			this.w_lStoragePointDetectedLabel.Name = "w_lStoragePointDetectedLabel";
			this.w_lStoragePointDetectedLabel.Size = new System.Drawing.Size(145, 13);
			this.w_lStoragePointDetectedLabel.TabIndex = 1;
			this.w_lStoragePointDetectedLabel.Text = "StoragePoint found on target:";
			this.w_lStoragePointFoundValue.Location = new System.Drawing.Point(156, 30);
			this.w_lStoragePointFoundValue.Name = "w_lStoragePointFoundValue";
			this.w_lStoragePointFoundValue.Size = new System.Drawing.Size(13, 13);
			this.w_lStoragePointFoundValue.TabIndex = 2;
			this.w_lStoragePointFoundValue.Text = "No";
			this.w_gbStoragePoint.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.w_gbStoragePoint.Controls.Add(this.w_helpSideloadDocs);
			this.w_gbStoragePoint.Controls.Add(this.w_cbSideloadDocs);
			this.w_gbStoragePoint.Location = new System.Drawing.Point(3, 46);
			this.w_gbStoragePoint.Name = "w_gbStoragePoint";
			this.w_gbStoragePoint.Size = new System.Drawing.Size(603, 298);
			this.w_gbStoragePoint.TabIndex = 3;
			this.w_gbStoragePoint.Text = "StoragePoint";
			this.w_helpSideloadDocs.AnchoringControl = this.w_cbSideloadDocs;
			this.w_helpSideloadDocs.BackColor = System.Drawing.Color.Transparent;
			this.w_helpSideloadDocs.CommonParentControl = null;
			this.w_helpSideloadDocs.Image = (System.Drawing.Image)resources.GetObject("w_helpSideloadDocs.Image");
			this.w_helpSideloadDocs.Location = new System.Drawing.Point(361, 21);
			this.w_helpSideloadDocs.MaximumSize = new System.Drawing.Size(20, 20);
			this.w_helpSideloadDocs.MinimumSize = new System.Drawing.Size(20, 20);
			this.w_helpSideloadDocs.Name = "w_helpSideloadDocs";
			this.w_helpSideloadDocs.Size = new System.Drawing.Size(20, 20);
			this.w_helpSideloadDocs.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.w_helpSideloadDocs.TabIndex = 1;
			this.w_helpSideloadDocs.TabStop = false;
			this.w_cbSideloadDocs.Location = new System.Drawing.Point(6, 22);
			this.w_cbSideloadDocs.Name = "w_cbSideloadDocs";
			this.w_cbSideloadDocs.Properties.AutoWidth = true;
			this.w_cbSideloadDocs.Properties.Caption = "Add documents directly to StoragePoint if an endpoint is configured.";
			this.w_cbSideloadDocs.Size = new System.Drawing.Size(350, 19);
			this.w_cbSideloadDocs.TabIndex = 0;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.w_gbStoragePoint);
			base.Controls.Add(this.w_lStoragePointFoundValue);
			base.Controls.Add(this.w_lStoragePointDetectedLabel);
			base.Controls.Add(this.w_lIntroduction);
			base.Name = "TCStoragePointOptions";
			base.Size = new System.Drawing.Size(609, 347);
			base.Load += new System.EventHandler(TCPasteFileToStoragePoint_Load);
			((System.ComponentModel.ISupportInitialize)this.w_gbStoragePoint).EndInit();
			this.w_gbStoragePoint.ResumeLayout(false);
			this.w_gbStoragePoint.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.w_helpSideloadDocs).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbSideloadDocs.Properties).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void TCPasteFileToStoragePoint_Load(object sender, EventArgs e)
		{
			UpdateStoragePointStatus();
			UpdateUI();
		}

		private StoragePointPresent UpdateStoragePointStatus()
		{
			List<StoragePointPresent> list = new List<StoragePointPresent>();
			if (Context == null)
			{
				_storagePointPresent = StoragePointPresent.NotPresent;
				return _storagePointPresent;
			}
			foreach (object target in Context.Targets)
			{
				SPWeb sPWeb = target as SPWeb;
				if (sPWeb == null)
				{
					if (!(target is SPFolder))
					{
						continue;
					}
					sPWeb = ((SPFolder)target).ParentList.ParentWeb;
				}
				if (sPWeb != null)
				{
					if (!sPWeb.StoragePointPresent)
					{
						list.Add(StoragePointPresent.NotPresent);
					}
					else
					{
						list.Add(StoragePointPresent.Present);
					}
				}
			}
			if (list.Count == 0)
			{
				_storagePointPresent = StoragePointPresent.NotPresent;
			}
			else if (list.Contains(StoragePointPresent.NotPresent) && list.Contains(StoragePointPresent.Present))
			{
				_storagePointPresent = StoragePointPresent.PresentOnSome;
			}
			else if (!list.Contains(StoragePointPresent.Present))
			{
				_storagePointPresent = StoragePointPresent.NotPresent;
			}
			else
			{
				_storagePointPresent = StoragePointPresent.Present;
			}
			return _storagePointPresent;
		}

		private void UpdateUI()
		{
			switch (_storagePointPresent)
			{
			case StoragePointPresent.NotPresent:
				w_gbStoragePoint.Enabled = false;
				w_cbSideloadDocs.Checked = false;
				w_lStoragePointFoundValue.Text = "No";
				break;
			case StoragePointPresent.Present:
				w_gbStoragePoint.Enabled = true;
				w_lStoragePointFoundValue.Text = "Yes";
				break;
			case StoragePointPresent.PresentOnSome:
				w_gbStoragePoint.Enabled = false;
				w_cbSideloadDocs.Checked = false;
				w_lStoragePointFoundValue.Text = "Not found on some targets.";
				break;
			default:
				w_gbStoragePoint.Enabled = false;
				w_cbSideloadDocs.Checked = false;
				w_lStoragePointFoundValue.Text = "No";
				break;
			}
		}
	}
}
