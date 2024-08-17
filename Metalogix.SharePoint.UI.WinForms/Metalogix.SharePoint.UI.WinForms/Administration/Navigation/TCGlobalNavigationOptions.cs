using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using Metalogix.SharePoint.Administration;
using Metalogix.SharePoint.Options.Administration.Navigation;
using Metalogix.UI.WinForms.Components;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Administration.Navigation
{
	[ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Administration.GlobalNavigation32.ico"), ControlName("Global Navigation")]
	public partial class TCGlobalNavigationOptions : ScopableTabbableControl, IPropagateNavigation
	{
		private SPGlobalNavigationOptions m_Options;

		private SPGlobalNavigationOptions m_SavedOptions;

		private System.ComponentModel.IContainer components;

		private LabelControl w_lblGlobalNavigationInheritance;

		private CheckEdit w_cbGlobalNavShowPublishingPages;

		private CheckEdit w_rbGlobalNavSameAsParent;

		private CheckEdit w_rbGlobalNavSubsites;

		private CheckEdit w_cbGlobalNavShowSubsites;

		private LabelControl w_lblGlobalNavMaxItems;

		private CheckEdit w_rbLeaveUnchanged;

		private SpinEdit w_txtGlobalNavMaxItems;

		public SPGlobalNavigationOptions Options
		{
			get
			{
				return this.m_Options;
			}
			set
			{
				this.m_Options = value;
				this.LoadUI();
			}
		}

		public TCGlobalNavigationOptions()
		{
			this.InitializeComponent();
		}

		protected override void LoadUI()
		{
			this.w_cbGlobalNavShowSubsites.Checked = this.Options.GlobalNavigationZone.ShowSubSites;
			this.w_cbGlobalNavShowPublishingPages.Checked = this.Options.GlobalNavigationZone.ShowPublishingPages;
			this.w_txtGlobalNavMaxItems.Text = this.Options.GlobalNavigationZone.MaxDynamicItems.ToString();
			ChangeNavigationSettingsOptions.GlobalNavigationDisplayType globalNavigationType = this.Options.GlobalNavigationType;
			if (globalNavigationType == ChangeNavigationSettingsOptions.GlobalNavigationDisplayType.SameAsParentSite)
			{
				this.w_rbGlobalNavSameAsParent.Checked = true;
			}
			else if (globalNavigationType == ChangeNavigationSettingsOptions.GlobalNavigationDisplayType.OnlyBelowCurrentSite)
			{
				this.w_rbGlobalNavSubsites.Checked = true;
			}
			else if (globalNavigationType == ChangeNavigationSettingsOptions.GlobalNavigationDisplayType.Unspecified)
			{
				this.w_rbLeaveUnchanged.Checked = true;
			}
			this.UpdateEnabledState();
			this.Options.GlobalNavigationOptionsModified = new ChangeNavigationSettingsOptions.GlobalNavigationModifiedFlags();
		}

		public override bool SaveUI()
		{
			this.Options.GlobalNavigationZone.ShowSubSites = this.w_cbGlobalNavShowSubsites.Checked;
			this.Options.GlobalNavigationZone.ShowPublishingPages = this.w_cbGlobalNavShowPublishingPages.Checked;
			this.Options.GlobalNavigationZone.MaxDynamicItems = int.Parse(this.w_txtGlobalNavMaxItems.Text);
			if (this.w_rbGlobalNavSameAsParent.Checked)
			{
				this.Options.GlobalNavigationType = ChangeNavigationSettingsOptions.GlobalNavigationDisplayType.SameAsParentSite;
			}
			else if (this.w_rbGlobalNavSubsites.Checked)
			{
				this.Options.GlobalNavigationType = ChangeNavigationSettingsOptions.GlobalNavigationDisplayType.OnlyBelowCurrentSite;
			}
			else
			{
				this.Options.GlobalNavigationType = ChangeNavigationSettingsOptions.GlobalNavigationDisplayType.Unspecified;
			}
			return true;
		}

		protected override void UpdateEnabledState()
		{
			bool enabled = this.Options != null && this.Options.SharePointVersion.IsSharePoint2010;
			bool enabled2 = this.Options != null && (this.Options.AdapterType == ChangeNavigationSettingsOptions.SharePointAdapterType.OM || this.Options.AdapterType == ChangeNavigationSettingsOptions.SharePointAdapterType.ExtensionsService);
			this.w_cbGlobalNavShowPublishingPages.Enabled = enabled;
			this.w_cbGlobalNavShowSubsites.Enabled = enabled;
			this.w_txtGlobalNavMaxItems.Enabled = enabled;
			this.w_lblGlobalNavMaxItems.Enabled = enabled;
			this.w_lblGlobalNavigationInheritance.Enabled = enabled2;
			this.w_rbGlobalNavSameAsParent.Enabled = enabled2;
			this.w_rbGlobalNavSubsites.Enabled = enabled2;
			this.w_rbLeaveUnchanged.Enabled = enabled2;
		}

	    public void StartNavigationPropagationHandler()
		{
			this.SaveUI();
			this.m_SavedOptions = (SPGlobalNavigationOptions)this.Options.Clone();
			if (!this.Options.GlobalNavigationOptionsModified.ShowSubsites && this.w_cbGlobalNavShowSubsites.Enabled)
			{
				this.w_cbGlobalNavShowSubsites.CheckState = CheckState.Indeterminate;
				this.Options.GlobalNavigationOptionsModified.ShowSubsites = false;
			}
			if (!this.Options.GlobalNavigationOptionsModified.ShowPublishingPages && this.w_cbGlobalNavShowPublishingPages.Enabled)
			{
				this.w_cbGlobalNavShowPublishingPages.CheckState = CheckState.Indeterminate;
				this.Options.GlobalNavigationOptionsModified.ShowPublishingPages = false;
			}
			if (!this.Options.GlobalNavigationOptionsModified.GlobalNavigationType && this.w_rbLeaveUnchanged.Enabled)
			{
				this.w_rbLeaveUnchanged.Checked = true;
				this.Options.GlobalNavigationOptionsModified.GlobalNavigationType = false;
			}
		}

	    public void StopNavigationPropagationHandler()
		{
			if (this.w_cbGlobalNavShowSubsites.CheckState == CheckState.Indeterminate)
			{
				bool showSubsites = this.Options.GlobalNavigationOptionsModified.ShowSubsites;
				this.w_cbGlobalNavShowSubsites.CheckState = (this.m_SavedOptions.GlobalNavigationZone.ShowSubSites ? CheckState.Checked : CheckState.Unchecked);
				this.Options.GlobalNavigationOptionsModified.ShowSubsites = showSubsites;
			}
			if (this.w_cbGlobalNavShowPublishingPages.CheckState == CheckState.Indeterminate)
			{
				bool showPublishingPages = this.Options.GlobalNavigationOptionsModified.ShowPublishingPages;
				this.w_cbGlobalNavShowPublishingPages.CheckState = (this.m_SavedOptions.GlobalNavigationZone.ShowPublishingPages ? CheckState.Checked : CheckState.Unchecked);
				this.Options.GlobalNavigationOptionsModified.ShowPublishingPages = showPublishingPages;
			}
			if (!this.Options.GlobalNavigationOptionsModified.GlobalNavigationType)
			{
				switch (this.m_SavedOptions.GlobalNavigationType)
				{
				case ChangeNavigationSettingsOptions.GlobalNavigationDisplayType.Unspecified:
					this.w_rbLeaveUnchanged.Checked = true;
					break;
				case ChangeNavigationSettingsOptions.GlobalNavigationDisplayType.SameAsParentSite:
					this.w_rbGlobalNavSameAsParent.Checked = true;
					break;
				case ChangeNavigationSettingsOptions.GlobalNavigationDisplayType.OnlyBelowCurrentSite:
					this.w_rbGlobalNavSubsites.Checked = true;
					break;
				}
				this.Options.GlobalNavigationOptionsModified.GlobalNavigationType = false;
			}
		}

		private void On_GlobalNavType_CheckedChanged(object sender, System.EventArgs e)
		{
			this.Options.GlobalNavigationOptionsModified.GlobalNavigationType = true;
		}

		private void On_cbGlobalNavShowSubsites_CheckedChanged(object sender, System.EventArgs e)
		{
			this.Options.GlobalNavigationOptionsModified.ShowSubsites = true;
		}

		private void On_cbGlobalNavShowPublishingPages_CheckedChanged(object sender, System.EventArgs e)
		{
			this.Options.GlobalNavigationOptionsModified.ShowPublishingPages = true;
		}

		private void On_txtGlobalNavMaxItems_TextChanged(object sender, System.EventArgs e)
		{
			this.Options.GlobalNavigationOptionsModified.DynamicItems = true;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(TCGlobalNavigationOptions));
			this.w_rbLeaveUnchanged = new CheckEdit();
			this.w_lblGlobalNavigationInheritance = new LabelControl();
			this.w_lblGlobalNavMaxItems = new LabelControl();
			this.w_cbGlobalNavShowSubsites = new CheckEdit();
			this.w_rbGlobalNavSubsites = new CheckEdit();
			this.w_rbGlobalNavSameAsParent = new CheckEdit();
			this.w_cbGlobalNavShowPublishingPages = new CheckEdit();
			this.w_txtGlobalNavMaxItems = new SpinEdit();
			((System.ComponentModel.ISupportInitialize)this.w_rbLeaveUnchanged.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbGlobalNavShowSubsites.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbGlobalNavSubsites.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbGlobalNavSameAsParent.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbGlobalNavShowPublishingPages.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_txtGlobalNavMaxItems.Properties).BeginInit();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.w_rbLeaveUnchanged, "w_rbLeaveUnchanged");
			this.w_rbLeaveUnchanged.Name = "w_rbLeaveUnchanged";
			this.w_rbLeaveUnchanged.Properties.AutoWidth = true;
			this.w_rbLeaveUnchanged.Properties.Caption = componentResourceManager.GetString("w_rbLeaveUnchanged.Properties.Caption");
			this.w_lblGlobalNavigationInheritance.Appearance.Font = (System.Drawing.Font)componentResourceManager.GetObject("w_lblGlobalNavigationInheritance.Appearance.Font");
			componentResourceManager.ApplyResources(this.w_lblGlobalNavigationInheritance, "w_lblGlobalNavigationInheritance");
			this.w_lblGlobalNavigationInheritance.Name = "w_lblGlobalNavigationInheritance";
			componentResourceManager.ApplyResources(this.w_lblGlobalNavMaxItems, "w_lblGlobalNavMaxItems");
			this.w_lblGlobalNavMaxItems.Name = "w_lblGlobalNavMaxItems";
			componentResourceManager.ApplyResources(this.w_cbGlobalNavShowSubsites, "w_cbGlobalNavShowSubsites");
			this.w_cbGlobalNavShowSubsites.Name = "w_cbGlobalNavShowSubsites";
			this.w_cbGlobalNavShowSubsites.Properties.AutoWidth = true;
			this.w_cbGlobalNavShowSubsites.Properties.Caption = componentResourceManager.GetString("w_cbGlobalNavShowSubsites.Properties.Caption");
			this.w_cbGlobalNavShowSubsites.CheckedChanged += new System.EventHandler(this.On_cbGlobalNavShowSubsites_CheckedChanged);
			componentResourceManager.ApplyResources(this.w_rbGlobalNavSubsites, "w_rbGlobalNavSubsites");
			this.w_rbGlobalNavSubsites.Name = "w_rbGlobalNavSubsites";
			this.w_rbGlobalNavSubsites.Properties.AutoWidth = true;
			this.w_rbGlobalNavSubsites.Properties.Caption = componentResourceManager.GetString("w_rbGlobalNavSubsites.Properties.Caption");
			this.w_rbGlobalNavSubsites.CheckedChanged += new System.EventHandler(this.On_GlobalNavType_CheckedChanged);
			componentResourceManager.ApplyResources(this.w_rbGlobalNavSameAsParent, "w_rbGlobalNavSameAsParent");
			this.w_rbGlobalNavSameAsParent.Name = "w_rbGlobalNavSameAsParent";
			this.w_rbGlobalNavSameAsParent.Properties.AutoWidth = true;
			this.w_rbGlobalNavSameAsParent.Properties.Caption = componentResourceManager.GetString("w_rbGlobalNavSameAsParent.Properties.Caption");
			this.w_rbGlobalNavSameAsParent.CheckedChanged += new System.EventHandler(this.On_GlobalNavType_CheckedChanged);
			componentResourceManager.ApplyResources(this.w_cbGlobalNavShowPublishingPages, "w_cbGlobalNavShowPublishingPages");
			this.w_cbGlobalNavShowPublishingPages.Name = "w_cbGlobalNavShowPublishingPages";
			this.w_cbGlobalNavShowPublishingPages.Properties.AutoWidth = true;
			this.w_cbGlobalNavShowPublishingPages.Properties.Caption = componentResourceManager.GetString("w_cbGlobalNavShowPublishingPages.Properties.Caption");
			this.w_cbGlobalNavShowPublishingPages.CheckedChanged += new System.EventHandler(this.On_cbGlobalNavShowPublishingPages_CheckedChanged);
			componentResourceManager.ApplyResources(this.w_txtGlobalNavMaxItems, "w_txtGlobalNavMaxItems");
			this.w_txtGlobalNavMaxItems.Name = "w_txtGlobalNavMaxItems";
			this.w_txtGlobalNavMaxItems.Properties.Buttons.AddRange(new EditorButton[]
			{
				new EditorButton()
			});
			this.w_txtGlobalNavMaxItems.Properties.Mask.EditMask = componentResourceManager.GetString("w_txtGlobalNavMaxItems.Properties.Mask.EditMask");
			RepositoryItemSpinEdit arg_393_0 = this.w_txtGlobalNavMaxItems.Properties;
			int[] array = new int[4];
			array[0] = 2147483647;
			arg_393_0.MaxValue = new decimal(array);
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.w_txtGlobalNavMaxItems);
			base.Controls.Add(this.w_rbLeaveUnchanged);
			base.Controls.Add(this.w_lblGlobalNavigationInheritance);
			base.Controls.Add(this.w_lblGlobalNavMaxItems);
			base.Controls.Add(this.w_cbGlobalNavShowSubsites);
			base.Controls.Add(this.w_rbGlobalNavSubsites);
			base.Controls.Add(this.w_rbGlobalNavSameAsParent);
			base.Controls.Add(this.w_cbGlobalNavShowPublishingPages);
			base.Name = "TCGlobalNavigationOptions";
			((System.ComponentModel.ISupportInitialize)this.w_rbLeaveUnchanged.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbGlobalNavShowSubsites.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbGlobalNavSubsites.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbGlobalNavSameAsParent.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbGlobalNavShowPublishingPages.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_txtGlobalNavMaxItems.Properties).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
