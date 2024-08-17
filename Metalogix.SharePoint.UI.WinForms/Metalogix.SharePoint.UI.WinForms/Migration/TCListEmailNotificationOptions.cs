using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.DataStructures;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ControlName("Copying Options")]
	public class TCListEmailNotificationOptions : ScopableTabbableControl
	{
		private TransformationTaskCollection m_RenameTransformations;

		private SPListEmailNotificationOptions m_Options;

		private IContainer components;

		private CheckEdit w_cbRecursive;

		private PanelControl w_plWebLevel;

		private CheckEdit w_cbSitesAndListsRenamed;

		private SimpleButton w_btnEditNameMappings;

		public SPListEmailNotificationOptions Options
		{
			get
			{
				return m_Options;
			}
			set
			{
				m_Options = value;
				LoadUI();
			}
		}

		private TransformationTaskCollection RenameTransformations
		{
			get
			{
				if (m_RenameTransformations == null)
				{
					m_RenameTransformations = new TransformationTaskCollection();
				}
				return m_RenameTransformations;
			}
			set
			{
				m_RenameTransformations = value;
			}
		}

		public TCListEmailNotificationOptions()
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
			this.w_cbRecursive = new DevExpress.XtraEditors.CheckEdit();
			this.w_plWebLevel = new DevExpress.XtraEditors.PanelControl();
			this.w_btnEditNameMappings = new DevExpress.XtraEditors.SimpleButton();
			this.w_cbSitesAndListsRenamed = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)this.w_cbRecursive.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_plWebLevel).BeginInit();
			this.w_plWebLevel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_cbSitesAndListsRenamed.Properties).BeginInit();
			base.SuspendLayout();
			this.w_cbRecursive.Location = new System.Drawing.Point(6, 0);
			this.w_cbRecursive.Name = "w_cbRecursive";
			this.w_cbRecursive.Properties.AutoWidth = true;
			this.w_cbRecursive.Properties.Caption = "Copy notification settings recursively in subsites";
			this.w_cbRecursive.Size = new System.Drawing.Size(253, 19);
			this.w_cbRecursive.TabIndex = 0;
			this.w_plWebLevel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plWebLevel.Controls.Add(this.w_btnEditNameMappings);
			this.w_plWebLevel.Controls.Add(this.w_cbSitesAndListsRenamed);
			this.w_plWebLevel.Controls.Add(this.w_cbRecursive);
			this.w_plWebLevel.Location = new System.Drawing.Point(0, 9);
			this.w_plWebLevel.Name = "w_plWebLevel";
			this.w_plWebLevel.Size = new System.Drawing.Size(345, 46);
			this.w_plWebLevel.TabIndex = 1;
			this.w_btnEditNameMappings.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			this.w_btnEditNameMappings.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_btnEditNameMappings.Location = new System.Drawing.Point(216, 22);
			this.w_btnEditNameMappings.Name = "w_btnEditNameMappings";
			this.w_btnEditNameMappings.Size = new System.Drawing.Size(35, 21);
			this.w_btnEditNameMappings.TabIndex = 3;
			this.w_btnEditNameMappings.Text = "...";
			this.w_btnEditNameMappings.Click += new System.EventHandler(On_btnEditNameMappings_Click);
			this.w_cbSitesAndListsRenamed.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_cbSitesAndListsRenamed.Location = new System.Drawing.Point(6, 23);
			this.w_cbSitesAndListsRenamed.Name = "w_cbSitesAndListsRenamed";
			this.w_cbSitesAndListsRenamed.Properties.AutoWidth = true;
			this.w_cbSitesAndListsRenamed.Properties.Caption = "Map sites and lists renamed on target";
			this.w_cbSitesAndListsRenamed.Size = new System.Drawing.Size(203, 19);
			this.w_cbSitesAndListsRenamed.TabIndex = 2;
			this.w_cbSitesAndListsRenamed.CheckedChanged += new System.EventHandler(w_cbSitesAndListsRenamed_CheckedChanged);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.w_plWebLevel);
			base.Name = "TCListEmailNotificationOptions";
			base.Size = new System.Drawing.Size(348, 116);
			((System.ComponentModel.ISupportInitialize)this.w_cbRecursive.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_plWebLevel).EndInit();
			this.w_plWebLevel.ResumeLayout(false);
			this.w_plWebLevel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.w_cbSitesAndListsRenamed.Properties).EndInit();
			base.ResumeLayout(false);
		}

		protected override void LoadUI()
		{
			RenameTransformations = ((Options.RenamingTransformations != null) ? ((TransformationTaskCollection)Options.RenamingTransformations.Clone()) : new TransformationTaskCollection());
			w_cbRecursive.Checked = Options.Recursive;
			w_cbSitesAndListsRenamed.Checked = Options.RenameSpecificNodes;
			UpdateEnabledState();
		}

		private void On_btnEditNameMappings_Click(object sender, EventArgs e)
		{
			PropertyTransformationDialog propertyTransformationDialog = new PropertyTransformationDialog(SourceNodes, "Specify rename mappings of sites and lists");
			propertyTransformationDialog.Tasks = RenameTransformations.Clone() as TransformationTaskCollection;
			PropertyTransformationDialog propertyTransformationDialog2 = propertyTransformationDialog;
			if (propertyTransformationDialog2.ShowDialog() == DialogResult.OK)
			{
				RenameTransformations = propertyTransformationDialog2.Tasks;
			}
		}

		public override bool SaveUI()
		{
			Options.Recursive = w_cbRecursive.Checked;
			Options.RenameSpecificNodes = w_cbSitesAndListsRenamed.Checked;
			if (w_cbSitesAndListsRenamed.Checked)
			{
				Options.RenamingTransformations = RenameTransformations;
			}
			return true;
		}

		protected override void UpdateEnabledState()
		{
			w_btnEditNameMappings.Enabled = w_cbSitesAndListsRenamed.Checked;
		}

		private void w_cbSitesAndListsRenamed_CheckedChanged(object sender, EventArgs e)
		{
			UpdateEnabledState();
		}
	}
}
