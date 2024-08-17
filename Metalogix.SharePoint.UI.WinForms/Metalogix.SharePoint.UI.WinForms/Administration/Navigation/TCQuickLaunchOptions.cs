using DevExpress.Utils;
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
	[ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Administration.QuickLaunch32.ico"), ControlName("Quick Launch")]
	public partial class TCQuickLaunchOptions : ScopableTabbableControl, IPropagateNavigation
	{
		private SPQuickLaunchOptions m_Options;

		private bool m_bPropagatingSettings;

		private SPQuickLaunchOptions m_SavedOptions;

		private System.ComponentModel.IContainer components;

		private CheckEdit w_cbQuickLaunchEnabled;

		private CheckEdit w_cbTreeViewEnabled;

		private PanelControl w_pnlCurrentNav;

		private LabelControl w_lblCurrentNavPanel;

		private CheckEdit w_rbCurrentNavSiblingsAndBelow;

		private LabelControl w_lblCurrentNavMaxItems;

		private CheckEdit w_cbCurrentNavShowSubsites;

		private CheckEdit w_rbCurrentNavSubsites;

		private CheckEdit w_rbCurrentNavSameAsParent;

		private CheckEdit w_cbCurrentNavShowPublishingPages;

		private LabelControl w_lblObjectModelSettings;

		private CheckEdit w_rbLeaveNavigationUnchanged;

		private SpinEdit w_txtCurrentNavMaxItems;

		public SPQuickLaunchOptions Options
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

		private bool PusingSettingsToOtherSites
		{
			get
			{
				return this.m_bPropagatingSettings;
			}
			set
			{
				this.m_bPropagatingSettings = value;
			}
		}

		public TCQuickLaunchOptions()
		{
			this.InitializeComponent();
		}

		protected override void LoadUI()
		{
			this.w_cbQuickLaunchEnabled.Checked = this.Options.QuickLaunchEnabled;
			this.w_cbTreeViewEnabled.Checked = this.Options.TreeViewEnabled;
			this.w_cbCurrentNavShowSubsites.Checked = this.Options.CurrentNavigationZone.ShowSubSites;
			this.w_cbCurrentNavShowPublishingPages.Checked = this.Options.CurrentNavigationZone.ShowPublishingPages;
			this.w_txtCurrentNavMaxItems.Text = this.Options.CurrentNavigationZone.MaxDynamicItems.ToString();
			ChangeNavigationSettingsOptions.CurrentNavigationDisplayType currentNavigationType = this.Options.CurrentNavigationType;
			if (currentNavigationType == ChangeNavigationSettingsOptions.CurrentNavigationDisplayType.CurrentSiteAndBelowPlusSiblings)
			{
				this.w_rbCurrentNavSiblingsAndBelow.Checked = true;
			}
			else if (currentNavigationType == ChangeNavigationSettingsOptions.CurrentNavigationDisplayType.OnlyBelowCurrentSite)
			{
				this.w_rbCurrentNavSubsites.Checked = true;
			}
			else if (currentNavigationType == ChangeNavigationSettingsOptions.CurrentNavigationDisplayType.SameAsParentSite)
			{
				this.w_rbCurrentNavSameAsParent.Checked = true;
			}
			else if (currentNavigationType == ChangeNavigationSettingsOptions.CurrentNavigationDisplayType.Unspecified)
			{
				this.w_rbLeaveNavigationUnchanged.Checked = true;
			}
			this.UpdateEnabledState();
			this.Options.QuickLaunchOptionsModified = new ChangeNavigationSettingsOptions.QuickLaunchModifiedFlags();
		}

		public override bool SaveUI()
		{
			this.Options.QuickLaunchEnabled = this.w_cbQuickLaunchEnabled.Checked;
			this.Options.TreeViewEnabled = this.w_cbTreeViewEnabled.Checked;
			this.Options.CurrentNavigationZone.ShowSubSites = this.w_cbCurrentNavShowSubsites.Checked;
			this.Options.CurrentNavigationZone.ShowPublishingPages = this.w_cbCurrentNavShowPublishingPages.Checked;
			this.Options.CurrentNavigationZone.MaxDynamicItems = int.Parse(this.w_txtCurrentNavMaxItems.Text);
			if (this.w_rbCurrentNavSameAsParent.Checked)
			{
				this.Options.CurrentNavigationType = ChangeNavigationSettingsOptions.CurrentNavigationDisplayType.SameAsParentSite;
			}
			else if (this.w_rbCurrentNavSiblingsAndBelow.Checked)
			{
				this.Options.CurrentNavigationType = ChangeNavigationSettingsOptions.CurrentNavigationDisplayType.CurrentSiteAndBelowPlusSiblings;
			}
			else if (this.w_rbCurrentNavSubsites.Checked)
			{
				this.Options.CurrentNavigationType = ChangeNavigationSettingsOptions.CurrentNavigationDisplayType.OnlyBelowCurrentSite;
			}
			else
			{
				this.Options.CurrentNavigationType = ChangeNavigationSettingsOptions.CurrentNavigationDisplayType.Unspecified;
			}
			return true;
		}

		protected override void UpdateEnabledState()
		{
			bool enabled = this.Options != null && this.Options.SharePointVersion.IsSharePoint2010;
			bool flag = this.Options != null && (this.Options.AdapterType == ChangeNavigationSettingsOptions.SharePointAdapterType.OM || this.Options.AdapterType == ChangeNavigationSettingsOptions.SharePointAdapterType.ExtensionsService);
			this.w_lblObjectModelSettings.Enabled = false;
			this.w_lblObjectModelSettings.Visible = !flag;
			this.w_cbQuickLaunchEnabled.Enabled = flag;
			this.w_cbTreeViewEnabled.Enabled = flag;
			this.w_lblCurrentNavMaxItems.Enabled = enabled;
			this.w_txtCurrentNavMaxItems.Enabled = enabled;
		}

	    public void StartNavigationPropagationHandler()
		{
			this.SaveUI();
			this.m_SavedOptions = (SPQuickLaunchOptions)this.Options.Clone();
			if (!this.Options.QuickLaunchOptionsModified.QuickLaunchEnabled && this.w_cbQuickLaunchEnabled.Enabled)
			{
				this.w_cbQuickLaunchEnabled.CheckState = CheckState.Indeterminate;
				this.Options.QuickLaunchOptionsModified.QuickLaunchEnabled = false;
			}
			if (!this.Options.QuickLaunchOptionsModified.TreeViewEnabled && this.w_cbTreeViewEnabled.Enabled)
			{
				this.w_cbTreeViewEnabled.CheckState = CheckState.Indeterminate;
				this.Options.QuickLaunchOptionsModified.TreeViewEnabled = false;
			}
			if (!this.Options.QuickLaunchOptionsModified.ShowSubsites)
			{
				this.w_cbCurrentNavShowSubsites.CheckState = CheckState.Indeterminate;
				this.Options.QuickLaunchOptionsModified.ShowSubsites = false;
			}
			if (!this.Options.QuickLaunchOptionsModified.ShowPublishingPages)
			{
				this.w_cbCurrentNavShowPublishingPages.CheckState = CheckState.Indeterminate;
				this.Options.QuickLaunchOptionsModified.ShowPublishingPages = false;
			}
			if (!this.Options.QuickLaunchOptionsModified.CurrentNavigationType)
			{
				this.w_rbLeaveNavigationUnchanged.Checked = true;
				this.Options.QuickLaunchOptionsModified.CurrentNavigationType = false;
			}
		}

	    public void StopNavigationPropagationHandler()
		{
			if (this.w_cbQuickLaunchEnabled.CheckState == CheckState.Indeterminate)
			{
				bool quickLaunchEnabled = this.Options.QuickLaunchOptionsModified.QuickLaunchEnabled;
				this.w_cbQuickLaunchEnabled.CheckState = (this.m_SavedOptions.QuickLaunchEnabled ? CheckState.Checked : CheckState.Unchecked);
				this.Options.QuickLaunchOptionsModified.QuickLaunchEnabled = quickLaunchEnabled;
			}
			if (this.w_cbTreeViewEnabled.CheckState == CheckState.Indeterminate)
			{
				bool treeViewEnabled = this.Options.QuickLaunchOptionsModified.TreeViewEnabled;
				this.w_cbTreeViewEnabled.CheckState = (this.m_SavedOptions.TreeViewEnabled ? CheckState.Checked : CheckState.Unchecked);
				this.Options.QuickLaunchOptionsModified.TreeViewEnabled = treeViewEnabled;
			}
			if (this.w_cbCurrentNavShowSubsites.CheckState == CheckState.Indeterminate)
			{
				bool showSubsites = this.Options.QuickLaunchOptionsModified.ShowSubsites;
				this.w_cbCurrentNavShowSubsites.CheckState = (this.m_SavedOptions.CurrentNavigationZone.ShowSubSites ? CheckState.Checked : CheckState.Unchecked);
				this.Options.QuickLaunchOptionsModified.ShowSubsites = showSubsites;
			}
			if (this.w_cbCurrentNavShowPublishingPages.CheckState == CheckState.Indeterminate)
			{
				bool showPublishingPages = this.Options.QuickLaunchOptionsModified.ShowPublishingPages;
				this.w_cbCurrentNavShowPublishingPages.CheckState = (this.m_SavedOptions.CurrentNavigationZone.ShowPublishingPages ? CheckState.Checked : CheckState.Unchecked);
				this.Options.QuickLaunchOptionsModified.ShowPublishingPages = showPublishingPages;
			}
			if (!this.Options.QuickLaunchOptionsModified.CurrentNavigationType)
			{
				switch (this.m_SavedOptions.CurrentNavigationType)
				{
				case ChangeNavigationSettingsOptions.CurrentNavigationDisplayType.Unspecified:
					this.w_rbLeaveNavigationUnchanged.Checked = true;
					break;
				case ChangeNavigationSettingsOptions.CurrentNavigationDisplayType.SameAsParentSite:
					this.w_rbCurrentNavSameAsParent.Checked = true;
					break;
				case ChangeNavigationSettingsOptions.CurrentNavigationDisplayType.CurrentSiteAndBelowPlusSiblings:
					this.w_rbCurrentNavSiblingsAndBelow.Checked = true;
					break;
				case ChangeNavigationSettingsOptions.CurrentNavigationDisplayType.OnlyBelowCurrentSite:
					this.w_rbCurrentNavSubsites.Checked = true;
					break;
				}
				this.Options.QuickLaunchOptionsModified.CurrentNavigationType = false;
			}
		}

		private void On_cbQuickLaunchEnabled_CheckedChanged(object sender, System.EventArgs e)
		{
			this.Options.QuickLaunchOptionsModified.QuickLaunchEnabled = true;
		}

		private void On_cbTreeViewEnabled_CheckedChanged(object sender, System.EventArgs e)
		{
			this.Options.QuickLaunchOptionsModified.TreeViewEnabled = true;
		}

		private void On_CurrentNavigationTypeChanged(object sender, System.EventArgs e)
		{
			this.Options.QuickLaunchOptionsModified.CurrentNavigationType = true;
		}

		private void On_cbCurrentNavShowSubsites_CheckedChanged(object sender, System.EventArgs e)
		{
			this.Options.QuickLaunchOptionsModified.ShowSubsites = true;
		}

		private void On_cbCurrentNavShowPublishingPages_CheckedChanged(object sender, System.EventArgs e)
		{
			this.Options.QuickLaunchOptionsModified.ShowPublishingPages = true;
		}

		private void On_txtCurrentNavMaxItems_TextChanged(object sender, System.EventArgs e)
		{
			this.Options.QuickLaunchOptionsModified.DynamicItems = true;
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
			System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(TCQuickLaunchOptions));
			this.w_pnlCurrentNav = new PanelControl();
			this.w_txtCurrentNavMaxItems = new SpinEdit();
			this.w_rbLeaveNavigationUnchanged = new CheckEdit();
			this.w_lblCurrentNavPanel = new LabelControl();
			this.w_rbCurrentNavSiblingsAndBelow = new CheckEdit();
			this.w_lblCurrentNavMaxItems = new LabelControl();
			this.w_cbCurrentNavShowSubsites = new CheckEdit();
			this.w_rbCurrentNavSubsites = new CheckEdit();
			this.w_rbCurrentNavSameAsParent = new CheckEdit();
			this.w_cbCurrentNavShowPublishingPages = new CheckEdit();
			this.w_cbQuickLaunchEnabled = new CheckEdit();
			this.w_cbTreeViewEnabled = new CheckEdit();
			this.w_lblObjectModelSettings = new LabelControl();
			((System.ComponentModel.ISupportInitialize)this.w_pnlCurrentNav).BeginInit();
			this.w_pnlCurrentNav.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_txtCurrentNavMaxItems.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbLeaveNavigationUnchanged.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbCurrentNavSiblingsAndBelow.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCurrentNavShowSubsites.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbCurrentNavSubsites.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbCurrentNavSameAsParent.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCurrentNavShowPublishingPages.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbQuickLaunchEnabled.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbTreeViewEnabled.Properties).BeginInit();
			base.SuspendLayout();
			this.w_pnlCurrentNav.BorderStyle = BorderStyles.NoBorder;
			this.w_pnlCurrentNav.Controls.Add(this.w_txtCurrentNavMaxItems);
			this.w_pnlCurrentNav.Controls.Add(this.w_rbLeaveNavigationUnchanged);
			this.w_pnlCurrentNav.Controls.Add(this.w_lblCurrentNavPanel);
			this.w_pnlCurrentNav.Controls.Add(this.w_rbCurrentNavSiblingsAndBelow);
			this.w_pnlCurrentNav.Controls.Add(this.w_lblCurrentNavMaxItems);
			this.w_pnlCurrentNav.Controls.Add(this.w_cbCurrentNavShowSubsites);
			this.w_pnlCurrentNav.Controls.Add(this.w_rbCurrentNavSubsites);
			this.w_pnlCurrentNav.Controls.Add(this.w_rbCurrentNavSameAsParent);
			this.w_pnlCurrentNav.Controls.Add(this.w_cbCurrentNavShowPublishingPages);
			componentResourceManager.ApplyResources(this.w_pnlCurrentNav, "w_pnlCurrentNav");
			this.w_pnlCurrentNav.Name = "w_pnlCurrentNav";
			componentResourceManager.ApplyResources(this.w_txtCurrentNavMaxItems, "w_txtCurrentNavMaxItems");
			this.w_txtCurrentNavMaxItems.Name = "w_txtCurrentNavMaxItems";
			this.w_txtCurrentNavMaxItems.Properties.Buttons.AddRange(new EditorButton[]
			{
				new EditorButton()
			});
			this.w_txtCurrentNavMaxItems.Properties.Mask.EditMask = componentResourceManager.GetString("w_txtCurrentNavMaxItems.Properties.Mask.EditMask");
			RepositoryItemSpinEdit arg_2C4_0 = this.w_txtCurrentNavMaxItems.Properties;
			int[] array = new int[4];
			array[0] = 2147483647;
			arg_2C4_0.MaxValue = new decimal(array);
			componentResourceManager.ApplyResources(this.w_rbLeaveNavigationUnchanged, "w_rbLeaveNavigationUnchanged");
			this.w_rbLeaveNavigationUnchanged.Name = "w_rbLeaveNavigationUnchanged";
			this.w_rbLeaveNavigationUnchanged.Properties.AutoWidth = true;
			this.w_rbLeaveNavigationUnchanged.Properties.Caption = componentResourceManager.GetString("w_rbLeaveNavigationUnchanged.Properties.Caption");
			this.w_rbLeaveNavigationUnchanged.Properties.CheckStyle = CheckStyles.Radio;
			this.w_rbLeaveNavigationUnchanged.Properties.RadioGroupIndex = 1;
			this.w_rbLeaveNavigationUnchanged.TabStop = false;
			this.w_lblCurrentNavPanel.Appearance.Font = (System.Drawing.Font)componentResourceManager.GetObject("w_lblCurrentNavPanel.Appearance.Font");
			componentResourceManager.ApplyResources(this.w_lblCurrentNavPanel, "w_lblCurrentNavPanel");
			this.w_lblCurrentNavPanel.Name = "w_lblCurrentNavPanel";
			componentResourceManager.ApplyResources(this.w_rbCurrentNavSiblingsAndBelow, "w_rbCurrentNavSiblingsAndBelow");
			this.w_rbCurrentNavSiblingsAndBelow.Name = "w_rbCurrentNavSiblingsAndBelow";
			this.w_rbCurrentNavSiblingsAndBelow.Properties.Appearance.Options.UseTextOptions = true;
			this.w_rbCurrentNavSiblingsAndBelow.Properties.Appearance.TextOptions.WordWrap = WordWrap.Wrap;
			this.w_rbCurrentNavSiblingsAndBelow.Properties.Caption = componentResourceManager.GetString("w_rbCurrentNavSiblingsAndBelow.Properties.Caption");
			this.w_rbCurrentNavSiblingsAndBelow.Properties.CheckStyle = CheckStyles.Radio;
			this.w_rbCurrentNavSiblingsAndBelow.Properties.RadioGroupIndex = 1;
			this.w_rbCurrentNavSiblingsAndBelow.TabStop = false;
			this.w_rbCurrentNavSiblingsAndBelow.CheckedChanged += new System.EventHandler(this.On_CurrentNavigationTypeChanged);
			componentResourceManager.ApplyResources(this.w_lblCurrentNavMaxItems, "w_lblCurrentNavMaxItems");
			this.w_lblCurrentNavMaxItems.Name = "w_lblCurrentNavMaxItems";
			componentResourceManager.ApplyResources(this.w_cbCurrentNavShowSubsites, "w_cbCurrentNavShowSubsites");
			this.w_cbCurrentNavShowSubsites.Name = "w_cbCurrentNavShowSubsites";
			this.w_cbCurrentNavShowSubsites.Properties.AutoWidth = true;
			this.w_cbCurrentNavShowSubsites.Properties.Caption = componentResourceManager.GetString("w_cbCurrentNavShowSubsites.Properties.Caption");
			this.w_cbCurrentNavShowSubsites.CheckedChanged += new System.EventHandler(this.On_cbCurrentNavShowSubsites_CheckedChanged);
			componentResourceManager.ApplyResources(this.w_rbCurrentNavSubsites, "w_rbCurrentNavSubsites");
			this.w_rbCurrentNavSubsites.Name = "w_rbCurrentNavSubsites";
			this.w_rbCurrentNavSubsites.Properties.AutoWidth = true;
			this.w_rbCurrentNavSubsites.Properties.Caption = componentResourceManager.GetString("w_rbCurrentNavSubsites.Properties.Caption");
			this.w_rbCurrentNavSubsites.Properties.CheckStyle = CheckStyles.Radio;
			this.w_rbCurrentNavSubsites.Properties.RadioGroupIndex = 1;
			this.w_rbCurrentNavSubsites.TabStop = false;
			this.w_rbCurrentNavSubsites.CheckedChanged += new System.EventHandler(this.On_CurrentNavigationTypeChanged);
			componentResourceManager.ApplyResources(this.w_rbCurrentNavSameAsParent, "w_rbCurrentNavSameAsParent");
			this.w_rbCurrentNavSameAsParent.Name = "w_rbCurrentNavSameAsParent";
			this.w_rbCurrentNavSameAsParent.Properties.AutoWidth = true;
			this.w_rbCurrentNavSameAsParent.Properties.Caption = componentResourceManager.GetString("w_rbCurrentNavSameAsParent.Properties.Caption");
			this.w_rbCurrentNavSameAsParent.Properties.CheckStyle = CheckStyles.Radio;
			this.w_rbCurrentNavSameAsParent.Properties.RadioGroupIndex = 1;
			this.w_rbCurrentNavSameAsParent.CheckedChanged += new System.EventHandler(this.On_CurrentNavigationTypeChanged);
			componentResourceManager.ApplyResources(this.w_cbCurrentNavShowPublishingPages, "w_cbCurrentNavShowPublishingPages");
			this.w_cbCurrentNavShowPublishingPages.Name = "w_cbCurrentNavShowPublishingPages";
			this.w_cbCurrentNavShowPublishingPages.Properties.AutoWidth = true;
			this.w_cbCurrentNavShowPublishingPages.Properties.Caption = componentResourceManager.GetString("w_cbCurrentNavShowPublishingPages.Properties.Caption");
			this.w_cbCurrentNavShowPublishingPages.CheckedChanged += new System.EventHandler(this.On_cbCurrentNavShowPublishingPages_CheckedChanged);
			componentResourceManager.ApplyResources(this.w_cbQuickLaunchEnabled, "w_cbQuickLaunchEnabled");
			this.w_cbQuickLaunchEnabled.Name = "w_cbQuickLaunchEnabled";
			this.w_cbQuickLaunchEnabled.Properties.AutoWidth = true;
			this.w_cbQuickLaunchEnabled.Properties.Caption = componentResourceManager.GetString("w_cbQuickLaunchEnabled.Properties.Caption");
			this.w_cbQuickLaunchEnabled.CheckedChanged += new System.EventHandler(this.On_cbQuickLaunchEnabled_CheckedChanged);
			componentResourceManager.ApplyResources(this.w_cbTreeViewEnabled, "w_cbTreeViewEnabled");
			this.w_cbTreeViewEnabled.Name = "w_cbTreeViewEnabled";
			this.w_cbTreeViewEnabled.Properties.AutoWidth = true;
			this.w_cbTreeViewEnabled.Properties.Caption = componentResourceManager.GetString("w_cbTreeViewEnabled.Properties.Caption");
			this.w_cbTreeViewEnabled.CheckedChanged += new System.EventHandler(this.On_cbTreeViewEnabled_CheckedChanged);
			this.w_lblObjectModelSettings.Appearance.Font = (System.Drawing.Font)componentResourceManager.GetObject("w_lblObjectModelSettings.Appearance.Font");
			componentResourceManager.ApplyResources(this.w_lblObjectModelSettings, "w_lblObjectModelSettings");
			this.w_lblObjectModelSettings.Name = "w_lblObjectModelSettings";
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.w_lblObjectModelSettings);
			base.Controls.Add(this.w_pnlCurrentNav);
			base.Controls.Add(this.w_cbQuickLaunchEnabled);
			base.Controls.Add(this.w_cbTreeViewEnabled);
			base.Name = "TCQuickLaunchOptions";
			((System.ComponentModel.ISupportInitialize)this.w_pnlCurrentNav).EndInit();
			this.w_pnlCurrentNav.ResumeLayout(false);
			this.w_pnlCurrentNav.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.w_txtCurrentNavMaxItems.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbLeaveNavigationUnchanged.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbCurrentNavSiblingsAndBelow.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCurrentNavShowSubsites.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbCurrentNavSubsites.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbCurrentNavSameAsParent.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCurrentNavShowPublishingPages.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbQuickLaunchEnabled.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbTreeViewEnabled.Properties).EndInit();
			base.ResumeLayout(false);
		}
	}
}
