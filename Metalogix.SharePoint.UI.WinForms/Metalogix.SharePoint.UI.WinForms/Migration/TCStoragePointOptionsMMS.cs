using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTab;
using Metalogix.Explorer;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.ExternalConnections;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Properties;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using TooltipsTest;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Migration.StoragePointOptions.png")]
	[ControlName("StoragePoint Options")]
	[UsesGroupBox(false)]
	public class TCStoragePointOptionsMMS : ScopableTabbableControl
	{
		private IContainer components;

		private LabelControl w_lIntroduction;

		private LabelControl w_lStoragePointDetectedLabel;

		private LabelControl w_lStoragePointFoundValue;

		protected GroupControl w_gbStoragePoint;

		private CheckEdit w_rbNone;

		private CheckEdit w_rbSideloadDocs;

		private HelpTipButton w_helpSideloadDocs;

		public ExternalizationOptions Options
		{
			get
			{
				return new ExternalizationOptions
				{
					SideLoadDocuments = w_rbSideloadDocs.Checked
				};
			}
			set
			{
				if (value == null)
				{
					return;
				}
				w_rbSideloadDocs.Checked = value.SideLoadDocuments;
				if (TargetNodes[0] is SPNode sPNode && sPNode.Adapter.IsClientOM)
				{
					Form parentForm = base.ParentForm;
					if (parentForm != null)
					{
						DevExpress.XtraTab.XtraTabControl xtraTabControl = (DevExpress.XtraTab.XtraTabControl)parentForm.Controls["tabControl"];
						if (xtraTabControl != null)
						{
							IEnumerable<XtraTabPage> source = xtraTabControl.TabPages.Where((XtraTabPage tabPage) => tabPage.Text.Equals("StoragePoint Options", StringComparison.InvariantCultureIgnoreCase));
							if (source.Count() > 0)
							{
								xtraTabControl.TabPages.Remove(source.First());
							}
						}
					}
				}
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				foreach (Node targetNode in TargetNodes)
				{
					bool storagePointPresent;
					if (targetNode is SPWeb)
					{
						SPWeb sPWeb = targetNode as SPWeb;
						flag3 = flag3 || sPWeb.Adapter.SharePointVersion.IsSharePoint2013OrLater;
						storagePointPresent = sPWeb.StoragePointPresent;
					}
					else if (!(targetNode is SPFolder))
					{
						if (!(targetNode is SPServer))
						{
							continue;
						}
						SPServer sPServer = targetNode as SPServer;
						flag3 = flag3 || sPServer.Adapter.SharePointVersion.IsSharePoint2013OrLater;
						storagePointPresent = sPServer.StoragePointPresent;
					}
					else
					{
						SPWeb parentWeb = (targetNode as SPFolder).ParentList.ParentWeb;
						flag3 = flag3 || parentWeb.Adapter.SharePointVersion.IsSharePoint2013OrLater;
						storagePointPresent = parentWeb.StoragePointPresent;
					}
					if (!storagePointPresent)
					{
						flag2 = true;
					}
					else
					{
						flag = true;
					}
				}
				if (flag && flag2)
				{
					w_rbSideloadDocs.Enabled = false;
					w_rbSideloadDocs.Checked = false;
					w_lStoragePointFoundValue.Text = "Not found on some targets.";
				}
				else if (!flag)
				{
					w_rbSideloadDocs.Enabled = false;
					w_rbSideloadDocs.Checked = false;
					w_lStoragePointFoundValue.Text = "No";
				}
				else
				{
					w_rbSideloadDocs.Enabled = true;
					w_lStoragePointFoundValue.Text = "Yes";
				}
				if (SourceNodes != null && SourceNodes.Count > 0 && TargetNodes != null && TargetNodes.Count > 0)
				{
					ExternalizationSupport externalizationSupport = ((SPNode)SourceNodes[0]).Adapter.ExternalizationSupport;
					ExternalizationSupport externalizationSupport2 = ((SPNode)TargetNodes[0]).Adapter.ExternalizationSupport;
					bool flag4 = ((SPNode)SourceNodes[0]).GetExternalConnectionsOfType<StoragePointExternalConnection>(recurseUp: true).Count > 0;
					if ((externalizationSupport != ExternalizationSupport.Supported || externalizationSupport2 != ExternalizationSupport.Supported || !SharePointConfigurationVariables.AllowDBWriting) && flag4)
					{
						w_rbSideloadDocs.Enabled = false;
					}
				}
				if (!w_rbSideloadDocs.Checked)
				{
					w_rbNone.Checked = true;
				}
			}
		}

		public TCStoragePointOptionsMMS()
		{
			InitializeComponent();
			Type type = GetType();
			w_helpSideloadDocs.SetResourceString(type.FullName + w_rbSideloadDocs.Name, type);
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.TCStoragePointOptionsMMS));
			this.w_lIntroduction = new DevExpress.XtraEditors.LabelControl();
			this.w_lStoragePointDetectedLabel = new DevExpress.XtraEditors.LabelControl();
			this.w_lStoragePointFoundValue = new DevExpress.XtraEditors.LabelControl();
			this.w_gbStoragePoint = new DevExpress.XtraEditors.GroupControl();
			this.w_helpSideloadDocs = new TooltipsTest.HelpTipButton();
			this.w_rbSideloadDocs = new DevExpress.XtraEditors.CheckEdit();
			this.w_rbNone = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)this.w_gbStoragePoint).BeginInit();
			this.w_gbStoragePoint.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_helpSideloadDocs).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbSideloadDocs.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbNone.Properties).BeginInit();
			base.SuspendLayout();
			this.w_lIntroduction.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.w_lIntroduction.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
			this.w_lIntroduction.Location = new System.Drawing.Point(4, 3);
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
			this.w_gbStoragePoint.Controls.Add(this.w_rbNone);
			this.w_gbStoragePoint.Controls.Add(this.w_rbSideloadDocs);
			this.w_gbStoragePoint.Location = new System.Drawing.Point(3, 46);
			this.w_gbStoragePoint.Name = "w_gbStoragePoint";
			this.w_gbStoragePoint.Size = new System.Drawing.Size(603, 298);
			this.w_gbStoragePoint.TabIndex = 3;
			this.w_gbStoragePoint.Text = "StoragePoint";
			this.w_helpSideloadDocs.AnchoringControl = this.w_rbSideloadDocs;
			this.w_helpSideloadDocs.BackColor = System.Drawing.Color.Transparent;
			this.w_helpSideloadDocs.CommonParentControl = null;
			this.w_helpSideloadDocs.Image = (System.Drawing.Image)resources.GetObject("w_helpSideloadDocs.Image");
			this.w_helpSideloadDocs.Location = new System.Drawing.Point(363, 20);
			this.w_helpSideloadDocs.MaximumSize = new System.Drawing.Size(20, 20);
			this.w_helpSideloadDocs.MinimumSize = new System.Drawing.Size(20, 20);
			this.w_helpSideloadDocs.Name = "w_helpSideloadDocs";
			this.w_helpSideloadDocs.Size = new System.Drawing.Size(20, 20);
			this.w_helpSideloadDocs.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.w_helpSideloadDocs.TabIndex = 119;
			this.w_helpSideloadDocs.TabStop = false;
			this.w_rbSideloadDocs.Location = new System.Drawing.Point(6, 21);
			this.w_rbSideloadDocs.Name = "w_rbSideloadDocs";
			this.w_rbSideloadDocs.Properties.AutoWidth = true;
			this.w_rbSideloadDocs.Properties.Caption = "Add documents directly to StoragePoint if an endpoint is configured.";
			this.w_rbSideloadDocs.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.w_rbSideloadDocs.Properties.RadioGroupIndex = 1;
			this.w_rbSideloadDocs.Size = new System.Drawing.Size(350, 19);
			this.w_rbSideloadDocs.TabIndex = 116;
			this.w_rbSideloadDocs.TabStop = false;
			this.w_rbSideloadDocs.CheckedChanged += new System.EventHandler(w_rbSideloadDocs_CheckedChanged);
			this.w_rbNone.EditValue = true;
			this.w_rbNone.Location = new System.Drawing.Point(6, 43);
			this.w_rbNone.Name = "w_rbNone";
			this.w_rbNone.Properties.AutoWidth = true;
			this.w_rbNone.Properties.Caption = "None";
			this.w_rbNone.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.w_rbNone.Properties.RadioGroupIndex = 1;
			this.w_rbNone.Size = new System.Drawing.Size(48, 19);
			this.w_rbNone.TabIndex = 117;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.w_gbStoragePoint);
			base.Controls.Add(this.w_lStoragePointFoundValue);
			base.Controls.Add(this.w_lStoragePointDetectedLabel);
			base.Controls.Add(this.w_lIntroduction);
			base.Name = "TCStoragePointOptionsMMS";
			base.Size = new System.Drawing.Size(609, 347);
			((System.ComponentModel.ISupportInitialize)this.w_gbStoragePoint).EndInit();
			this.w_gbStoragePoint.ResumeLayout(false);
			this.w_gbStoragePoint.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.w_helpSideloadDocs).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbSideloadDocs.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbNone.Properties).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void w_rbSideloadDocs_CheckedChanged(object sender, EventArgs e)
		{
			if (w_rbSideloadDocs.Checked && base.ContainsFocus && FlatXtraMessageBox.Show(Resources.Feature_StoragePoint_Warning, Resources.WarningString, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) != DialogResult.OK)
			{
				w_rbNone.Checked = true;
			}
		}
	}
}
