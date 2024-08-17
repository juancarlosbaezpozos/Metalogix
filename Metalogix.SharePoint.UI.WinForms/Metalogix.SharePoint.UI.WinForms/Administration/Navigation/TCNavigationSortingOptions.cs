using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.SharePoint.Administration;
using Metalogix.SharePoint.Options.Administration.Navigation;
using Metalogix.UI.WinForms.Components;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Administration.Navigation
{
	[ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Administration.Sorting32.ico"), ControlName("Sorting")]
	public partial class TCNavigationSortingOptions : ScopableTabbableControl, IPropagateNavigation
	{
		private SPNavigationSortingOptions m_Options;

		private SPNavigationSortingOptions m_SavedOptions;

		private System.ComponentModel.IContainer components;

		private CheckEdit w_cbSortPublishingAutomatically;

		private PanelControl w_pnlSortingMethod;

		private CheckEdit w_rbSortManually;

		private CheckEdit w_rbSortAutomatically;

		private CheckEdit w_rbSortDescending;

		private LabelControl w_lblSortBy;

		private CheckEdit w_rbSortAscending;

		private GroupControl w_gbAutomaticSorting;

		private LabelControl label1;

		private LabelControl label2;

		private CheckEdit w_rbExistingOrdering;

		private CheckEdit w_rbSortingUnchanged;

		private ComboBoxEdit w_comboSortBy;

		public SPNavigationSortingOptions Options
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

		public TCNavigationSortingOptions()
		{
			this.InitializeComponent();
		}

		protected override void LoadUI()
		{
			if (this.Options.OverallSortingType == ChangeNavigationSettingsOptions.SortingType.Automatic)
			{
				this.w_rbSortAutomatically.Checked = true;
			}
			else if (this.Options.OverallSortingType == ChangeNavigationSettingsOptions.SortingType.Manual)
			{
				this.w_rbSortManually.Checked = true;
			}
			else if (this.Options.OverallSortingType == ChangeNavigationSettingsOptions.SortingType.Unspecified)
			{
				this.w_rbSortingUnchanged.Checked = true;
			}
			this.w_cbSortPublishingAutomatically.Checked = (this.Options.PublishingPageSortingType == ChangeNavigationSettingsOptions.SortingType.Automatic);
			if (this.Options.SortAscending)
			{
				this.w_rbSortAscending.Checked = true;
			}
			else
			{
				this.w_rbSortDescending.Checked = true;
			}
			this.w_comboSortBy.Properties.Items.Clear();
			this.w_comboSortBy.Properties.Items.Add(ChangeNavigationSettingsOptions.SortByColumn.Title);
			this.w_comboSortBy.Properties.Items.Add(ChangeNavigationSettingsOptions.SortByColumn.CreatedDate);
			this.w_comboSortBy.Properties.Items.Add(ChangeNavigationSettingsOptions.SortByColumn.LastModifiedDate);
			this.w_comboSortBy.SelectedItem = this.Options.ColumnToSortBy;
			this.UpdateEnabledState();
			this.Options.SortingOptionsModified = new ChangeNavigationSettingsOptions.SortingModifiedFlags();
		}

		public override bool SaveUI()
		{
			this.Options.PublishingPageSortingType = (this.w_cbSortPublishingAutomatically.Checked ? ChangeNavigationSettingsOptions.SortingType.Automatic : ChangeNavigationSettingsOptions.SortingType.Manual);
			if (!this.w_rbSortManually.Checked)
			{
				this.Options.SortingOptionsModified.PublishingPageSorting = false;
			}
			if (this.w_rbSortAutomatically.Checked)
			{
				this.Options.OverallSortingType = ChangeNavigationSettingsOptions.SortingType.Automatic;
			}
			else if (this.w_rbSortManually.Checked)
			{
				this.Options.OverallSortingType = ChangeNavigationSettingsOptions.SortingType.Manual;
			}
			else
			{
				this.Options.OverallSortingType = ChangeNavigationSettingsOptions.SortingType.Unspecified;
			}
			try
			{
				this.Options.ColumnToSortBy = (ChangeNavigationSettingsOptions.SortByColumn)this.w_comboSortBy.SelectedItem;
			}
			catch
			{
				this.Options.ColumnToSortBy = ChangeNavigationSettingsOptions.SortByColumn.Unspecified;
			}
			this.Options.SortAscending = this.w_rbSortAscending.Checked;
			if (this.w_rbExistingOrdering.Checked)
			{
				this.Options.SortingOptionsModified.SortDirection = false;
			}
			return true;
		}

		protected override void UpdateEnabledState()
		{
			this.w_cbSortPublishingAutomatically.Enabled = this.w_rbSortManually.Checked;
			this.w_gbAutomaticSorting.Enabled = (!this.w_rbSortingUnchanged.Checked && (this.w_rbSortAutomatically.Checked || this.w_cbSortPublishingAutomatically.Checked));
			this.w_comboSortBy.Enabled = (this.w_rbSortAutomatically.Checked || (this.w_cbSortPublishingAutomatically.Checked && this.w_cbSortPublishingAutomatically.CheckState != CheckState.Indeterminate));
			this.w_rbSortAscending.Enabled = (this.w_rbSortAutomatically.Checked || (this.w_cbSortPublishingAutomatically.Checked && this.w_cbSortPublishingAutomatically.CheckState != CheckState.Indeterminate));
			this.w_rbSortDescending.Enabled = (this.w_rbSortAutomatically.Checked || (this.w_cbSortPublishingAutomatically.Checked && this.w_cbSortPublishingAutomatically.CheckState != CheckState.Indeterminate));
			this.w_rbExistingOrdering.Enabled = (this.w_rbSortAutomatically.Checked || (this.w_cbSortPublishingAutomatically.Checked && this.w_cbSortPublishingAutomatically.CheckState != CheckState.Indeterminate));
		}

	    public void StartNavigationPropagationHandler()
		{
			this.SaveUI();
			this.m_SavedOptions = (SPNavigationSortingOptions)this.Options.Clone();
			if (!this.Options.SortingOptionsModified.PublishingPageSorting)
			{
				this.w_cbSortPublishingAutomatically.CheckState = CheckState.Indeterminate;
				this.Options.SortingOptionsModified.PublishingPageSorting = false;
			}
			if (!this.Options.SortingOptionsModified.OverallSorting)
			{
				this.w_rbSortingUnchanged.Checked = true;
				this.Options.SortingOptionsModified.OverallSorting = false;
			}
			if (!this.Options.SortingOptionsModified.SortDirection)
			{
				this.w_rbExistingOrdering.Checked = true;
				this.Options.SortingOptionsModified.SortDirection = false;
			}
			if (!this.Options.SortingOptionsModified.SortingColumn)
			{
				this.w_comboSortBy.Text = "";
				this.w_comboSortBy.SelectedItem = null;
				this.Options.SortingOptionsModified.SortingColumn = false;
			}
			this.UpdateEnabledState();
		}

	    public void StopNavigationPropagationHandler()
		{
			if (this.w_cbSortPublishingAutomatically.CheckState == CheckState.Indeterminate)
			{
				bool publishingPageSorting = this.Options.SortingOptionsModified.PublishingPageSorting;
				this.w_cbSortPublishingAutomatically.CheckState = ((this.m_SavedOptions.PublishingPageSortingType == ChangeNavigationSettingsOptions.SortingType.Automatic) ? CheckState.Checked : CheckState.Unchecked);
				this.Options.SortingOptionsModified.PublishingPageSorting = publishingPageSorting;
			}
			if (!this.Options.SortingOptionsModified.OverallSorting)
			{
				switch (this.m_SavedOptions.OverallSortingType)
				{
				case ChangeNavigationSettingsOptions.SortingType.Unspecified:
					this.w_rbSortingUnchanged.Checked = true;
					break;
				case ChangeNavigationSettingsOptions.SortingType.Automatic:
					this.w_rbSortAutomatically.Checked = true;
					break;
				case ChangeNavigationSettingsOptions.SortingType.Manual:
					this.w_rbSortManually.Checked = true;
					break;
				}
				this.Options.SortingOptionsModified.OverallSorting = false;
			}
			if (!this.Options.SortingOptionsModified.SortDirection)
			{
				if (this.m_SavedOptions.SortAscending)
				{
					this.w_rbSortAscending.Checked = true;
				}
				else
				{
					this.w_rbSortDescending.Checked = true;
				}
				this.Options.SortingOptionsModified.SortDirection = false;
			}
			if (this.w_comboSortBy.SelectedItem == null)
			{
				bool sortingColumn = this.Options.SortingOptionsModified.SortingColumn;
				this.w_comboSortBy.SelectedItem = this.m_SavedOptions.ColumnToSortBy;
				this.Options.SortingOptionsModified.SortingColumn = sortingColumn;
			}
		}

		private void On_OverallSort_CheckedChanged(object sender, System.EventArgs e)
		{
			this.Options.SortingOptionsModified.OverallSorting = true;
			this.UpdateEnabledState();
		}

		private void On_cbSortPublishingAutomatically_CheckedChanged(object sender, System.EventArgs e)
		{
			this.Options.SortingOptionsModified.PublishingPageSorting = true;
			this.UpdateEnabledState();
		}

		private void On_comboSortBy_TextChanged(object sender, System.EventArgs e)
		{
			this.Options.SortingOptionsModified.SortingColumn = true;
			this.UpdateEnabledState();
		}

		private void On_SortingDirection_CheckedChanged(object sender, System.EventArgs e)
		{
			this.Options.SortingOptionsModified.SortDirection = true;
			this.UpdateEnabledState();
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
			System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(TCNavigationSortingOptions));
			this.label2 = new LabelControl();
			this.label1 = new LabelControl();
			this.w_gbAutomaticSorting = new GroupControl();
			this.w_comboSortBy = new ComboBoxEdit();
			this.w_rbExistingOrdering = new CheckEdit();
			this.w_lblSortBy = new LabelControl();
			this.w_rbSortAscending = new CheckEdit();
			this.w_rbSortDescending = new CheckEdit();
			this.w_cbSortPublishingAutomatically = new CheckEdit();
			this.w_pnlSortingMethod = new PanelControl();
			this.w_rbSortingUnchanged = new CheckEdit();
			this.w_rbSortManually = new CheckEdit();
			this.w_rbSortAutomatically = new CheckEdit();
			((System.ComponentModel.ISupportInitialize)this.w_gbAutomaticSorting).BeginInit();
			this.w_gbAutomaticSorting.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_comboSortBy.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbExistingOrdering.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbSortAscending.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbSortDescending.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbSortPublishingAutomatically.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_pnlSortingMethod).BeginInit();
			this.w_pnlSortingMethod.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_rbSortingUnchanged.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbSortManually.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbSortAutomatically.Properties).BeginInit();
			base.SuspendLayout();
			this.label2.Appearance.Font = (System.Drawing.Font)componentResourceManager.GetObject("label2.Appearance.Font");
			componentResourceManager.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			this.label1.Appearance.Font = (System.Drawing.Font)componentResourceManager.GetObject("label1.Appearance.Font");
			componentResourceManager.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			componentResourceManager.ApplyResources(this.w_gbAutomaticSorting, "w_gbAutomaticSorting");
			this.w_gbAutomaticSorting.Controls.Add(this.w_comboSortBy);
			this.w_gbAutomaticSorting.Controls.Add(this.w_rbExistingOrdering);
			this.w_gbAutomaticSorting.Controls.Add(this.w_lblSortBy);
			this.w_gbAutomaticSorting.Controls.Add(this.w_rbSortAscending);
			this.w_gbAutomaticSorting.Controls.Add(this.w_rbSortDescending);
			this.w_gbAutomaticSorting.Name = "w_gbAutomaticSorting";
			componentResourceManager.ApplyResources(this.w_comboSortBy, "w_comboSortBy");
			this.w_comboSortBy.Name = "w_comboSortBy";
			this.w_comboSortBy.Properties.Buttons.AddRange(new EditorButton[]
			{
				new EditorButton((ButtonPredefines)componentResourceManager.GetObject("w_comboSortBy.Properties.Buttons"))
			});
			componentResourceManager.ApplyResources(this.w_rbExistingOrdering, "w_rbExistingOrdering");
			this.w_rbExistingOrdering.Name = "w_rbExistingOrdering";
			this.w_rbExistingOrdering.Properties.AutoWidth = true;
			this.w_rbExistingOrdering.Properties.Caption = componentResourceManager.GetString("w_rbExistingOrdering.Properties.Caption");
			this.w_rbExistingOrdering.Properties.CheckStyle = CheckStyles.Radio;
			this.w_rbExistingOrdering.Properties.RadioGroupIndex = 1;
			this.w_rbExistingOrdering.TabStop = false;
			componentResourceManager.ApplyResources(this.w_lblSortBy, "w_lblSortBy");
			this.w_lblSortBy.Name = "w_lblSortBy";
			componentResourceManager.ApplyResources(this.w_rbSortAscending, "w_rbSortAscending");
			this.w_rbSortAscending.Name = "w_rbSortAscending";
			this.w_rbSortAscending.Properties.AutoWidth = true;
			this.w_rbSortAscending.Properties.Caption = componentResourceManager.GetString("w_rbSortAscending.Properties.Caption");
			this.w_rbSortAscending.Properties.CheckStyle = CheckStyles.Radio;
			this.w_rbSortAscending.Properties.RadioGroupIndex = 1;
			this.w_rbSortAscending.CheckedChanged += new System.EventHandler(this.On_SortingDirection_CheckedChanged);
			componentResourceManager.ApplyResources(this.w_rbSortDescending, "w_rbSortDescending");
			this.w_rbSortDescending.Name = "w_rbSortDescending";
			this.w_rbSortDescending.Properties.AutoWidth = true;
			this.w_rbSortDescending.Properties.Caption = componentResourceManager.GetString("w_rbSortDescending.Properties.Caption");
			this.w_rbSortDescending.Properties.CheckStyle = CheckStyles.Radio;
			this.w_rbSortDescending.Properties.RadioGroupIndex = 1;
			this.w_rbSortDescending.TabStop = false;
			this.w_rbSortDescending.CheckedChanged += new System.EventHandler(this.On_SortingDirection_CheckedChanged);
			componentResourceManager.ApplyResources(this.w_cbSortPublishingAutomatically, "w_cbSortPublishingAutomatically");
			this.w_cbSortPublishingAutomatically.Name = "w_cbSortPublishingAutomatically";
			this.w_cbSortPublishingAutomatically.Properties.AutoWidth = true;
			this.w_cbSortPublishingAutomatically.Properties.Caption = componentResourceManager.GetString("w_cbSortPublishingAutomatically.Properties.Caption");
			this.w_cbSortPublishingAutomatically.CheckedChanged += new System.EventHandler(this.On_cbSortPublishingAutomatically_CheckedChanged);
			this.w_pnlSortingMethod.BorderStyle = BorderStyles.NoBorder;
			this.w_pnlSortingMethod.Controls.Add(this.w_rbSortingUnchanged);
			this.w_pnlSortingMethod.Controls.Add(this.w_rbSortManually);
			this.w_pnlSortingMethod.Controls.Add(this.w_rbSortAutomatically);
			componentResourceManager.ApplyResources(this.w_pnlSortingMethod, "w_pnlSortingMethod");
			this.w_pnlSortingMethod.Name = "w_pnlSortingMethod";
			componentResourceManager.ApplyResources(this.w_rbSortingUnchanged, "w_rbSortingUnchanged");
			this.w_rbSortingUnchanged.Name = "w_rbSortingUnchanged";
			this.w_rbSortingUnchanged.Properties.AutoWidth = true;
			this.w_rbSortingUnchanged.Properties.Caption = componentResourceManager.GetString("w_rbSortingUnchanged.Properties.Caption");
			this.w_rbSortingUnchanged.Properties.CheckStyle = CheckStyles.Radio;
			this.w_rbSortingUnchanged.Properties.RadioGroupIndex = 2;
			this.w_rbSortingUnchanged.TabStop = false;
			componentResourceManager.ApplyResources(this.w_rbSortManually, "w_rbSortManually");
			this.w_rbSortManually.Name = "w_rbSortManually";
			this.w_rbSortManually.Properties.AutoWidth = true;
			this.w_rbSortManually.Properties.Caption = componentResourceManager.GetString("w_rbSortManually.Properties.Caption");
			this.w_rbSortManually.Properties.CheckStyle = CheckStyles.Radio;
			this.w_rbSortManually.Properties.RadioGroupIndex = 2;
			this.w_rbSortManually.TabStop = false;
			this.w_rbSortManually.CheckedChanged += new System.EventHandler(this.On_OverallSort_CheckedChanged);
			componentResourceManager.ApplyResources(this.w_rbSortAutomatically, "w_rbSortAutomatically");
			this.w_rbSortAutomatically.Name = "w_rbSortAutomatically";
			this.w_rbSortAutomatically.Properties.AutoWidth = true;
			this.w_rbSortAutomatically.Properties.Caption = componentResourceManager.GetString("w_rbSortAutomatically.Properties.Caption");
			this.w_rbSortAutomatically.Properties.CheckStyle = CheckStyles.Radio;
			this.w_rbSortAutomatically.Properties.RadioGroupIndex = 2;
			this.w_rbSortAutomatically.CheckedChanged += new System.EventHandler(this.On_OverallSort_CheckedChanged);
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.w_gbAutomaticSorting);
			base.Controls.Add(this.w_cbSortPublishingAutomatically);
			base.Controls.Add(this.w_pnlSortingMethod);
			base.Name = "TCNavigationSortingOptions";
			((System.ComponentModel.ISupportInitialize)this.w_gbAutomaticSorting).EndInit();
			this.w_gbAutomaticSorting.ResumeLayout(false);
			this.w_gbAutomaticSorting.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.w_comboSortBy.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbExistingOrdering.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbSortAscending.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbSortDescending.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbSortPublishingAutomatically.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_pnlSortingMethod).EndInit();
			this.w_pnlSortingMethod.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_rbSortingUnchanged.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbSortManually.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbSortAutomatically.Properties).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
