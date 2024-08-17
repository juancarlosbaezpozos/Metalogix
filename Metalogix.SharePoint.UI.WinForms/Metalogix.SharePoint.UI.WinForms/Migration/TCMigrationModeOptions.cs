using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Components;
using TooltipsTest;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Migration.MigrationMode.png")]
	[ControlName("Migration Mode")]
	public class TCMigrationModeOptions : MigrationModeScopableTabbableControl
	{
		private SPMigrationModeOptions m_Options;

		private bool m_bSuppressUpdateSitesCheckedEvent;

		private bool m_bSuppressUpdateListsCheckedEvent;

		private bool m_bSuppressUpdateItemsCheckedEvent;

		private IContainer components;

		private CheckEdit w_rbFullMode;

		private CheckEdit w_rbIncrementalMode;

		private LabelControl w_lblFullCopyDescription;

		private LabelControl label2;

		private CheckEdit w_rbCustomMode;

		private GroupControl w_gbSites;

		private GroupControl w_gbLists;

		private SimpleButton w_btnUpdateSiteOptions;

		private CheckEdit w_cbUpdateSites;

		private CheckEdit w_rbPreserveSites;

		private CheckEdit w_rbOverwriteSites;

		private SimpleButton w_btnUpdateListOptions;

		private CheckEdit w_cbUpdateLists;

		private CheckEdit w_rbPreserveLists;

		private CheckEdit w_rbOverwriteLists;

		private GroupControl w_gbItems;

		private CheckEdit w_rbPreserveItems;

		private CheckEdit w_rbOverwriteItems;

		private CheckEdit w_cbPropagateItemDeletions;

		private CheckEdit w_cbUpdateItems;

		private GroupControl w_gbOtherOptions;

		private PanelControl w_plSites;

		private PanelControl w_plLists;

		private PanelControl panel2;

		private PanelControl w_plOtherOptions;

		private SimpleButton w_btnUpdateItemOptions;

		private CheckEdit w_cbCheckModifiedDatesForLists;

		private CheckEdit w_cbCheckModifiedDatesForItemsDocuments;

		private HelpTipButton w_helpCustomMode;

		private HelpTipButton w_helpUpdateLists;

		private HelpTipButton w_helpUpdateItems;

		private HelpTipButton w_helpPropagateItemDeletions;

		public SPMigrationModeOptions Options
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

		public bool PropagateDeletionsSupported
		{
			get
			{
				if (TargetNodes == null || TargetNodes.Count <= 0)
				{
					return false;
				}
				return !((SPNode)TargetNodes[0]).Adapter.IsNws && !((SPNode)TargetNodes[0]).Adapter.IsClientOM;
			}
		}

		public bool ShowUpdateItems
		{
			get
			{
				return w_cbUpdateItems.Visible;
			}
			set
			{
				w_cbUpdateItems.Visible = value;
			}
		}

		public bool ShowUpdateItemsElipseButton
		{
			get
			{
				return w_btnUpdateItemOptions.Visible;
			}
			set
			{
				w_btnUpdateItemOptions.Visible = value;
			}
		}

		public bool ShowUpdateLists
		{
			get
			{
				return w_cbUpdateLists.Visible;
			}
			set
			{
				w_cbUpdateLists.Visible = value;
			}
		}

		public bool ShowUpdateListsElipseButton
		{
			get
			{
				return w_btnUpdateListOptions.Visible;
			}
			set
			{
				w_btnUpdateListOptions.Visible = value;
			}
		}

		public bool ShowUpdateSites
		{
			get
			{
				return w_cbUpdateSites.Visible;
			}
			set
			{
				w_cbUpdateSites.Visible = value;
			}
		}

		public bool ShowUpdateSitesElipseButton
		{
			get
			{
				return w_btnUpdateSiteOptions.Visible;
			}
			set
			{
				w_btnUpdateSiteOptions.Visible = value;
			}
		}

		public TCMigrationModeOptions()
		{
			InitializeComponent();
			Type type = GetType();
			w_helpCustomMode.SetResourceString(type.FullName + w_rbCustomMode.Name, type);
			w_helpUpdateLists.SetResourceString(type.FullName + w_cbUpdateLists.Name, type);
			w_helpUpdateItems.SetResourceString(type.FullName + w_cbUpdateItems.Name, type);
			w_helpPropagateItemDeletions.SetResourceString(type.FullName + w_cbPropagateItemDeletions.Name, type);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void FireMigrationModeChangedEvent()
		{
			MigrationMode migrationMode = MigrationMode.Custom;
			if (w_rbFullMode.Checked)
			{
				migrationMode = MigrationMode.Full;
			}
			else if (w_rbIncrementalMode.Checked)
			{
				migrationMode = MigrationMode.Incremental;
			}
			MigrationModeChangedInfo migrationModeChangedInfo = default(MigrationModeChangedInfo);
			migrationModeChangedInfo.NewMigrationMode = migrationMode;
			MigrationModeChangedInfo migrationModeChangedInfo2 = migrationModeChangedInfo;
			bool overwritingOrUpdatingItems = migrationMode == MigrationMode.Custom && (w_rbOverwriteItems.Checked || w_cbUpdateItems.Checked);
			migrationModeChangedInfo2.OverwritingOrUpdatingItems = overwritingOrUpdatingItems;
			migrationModeChangedInfo2.PropagatingItemDeletions = migrationMode == MigrationMode.Custom && w_cbPropagateItemDeletions.Checked;
			SendMessage("MigrationModeChanged", migrationModeChangedInfo2);
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.TCMigrationModeOptions));
			this.label2 = new DevExpress.XtraEditors.LabelControl();
			this.w_lblFullCopyDescription = new DevExpress.XtraEditors.LabelControl();
			this.w_rbCustomMode = new DevExpress.XtraEditors.CheckEdit();
			this.w_rbIncrementalMode = new DevExpress.XtraEditors.CheckEdit();
			this.w_rbFullMode = new DevExpress.XtraEditors.CheckEdit();
			this.w_gbSites = new DevExpress.XtraEditors.GroupControl();
			this.w_btnUpdateSiteOptions = new DevExpress.XtraEditors.SimpleButton();
			this.w_cbUpdateSites = new DevExpress.XtraEditors.CheckEdit();
			this.w_rbPreserveSites = new DevExpress.XtraEditors.CheckEdit();
			this.w_rbOverwriteSites = new DevExpress.XtraEditors.CheckEdit();
			this.w_gbLists = new DevExpress.XtraEditors.GroupControl();
			this.w_helpUpdateLists = new TooltipsTest.HelpTipButton();
			this.w_btnUpdateListOptions = new DevExpress.XtraEditors.SimpleButton();
			this.w_cbCheckModifiedDatesForLists = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbUpdateLists = new DevExpress.XtraEditors.CheckEdit();
			this.w_rbPreserveLists = new DevExpress.XtraEditors.CheckEdit();
			this.w_rbOverwriteLists = new DevExpress.XtraEditors.CheckEdit();
			this.w_gbItems = new DevExpress.XtraEditors.GroupControl();
			this.w_helpUpdateItems = new TooltipsTest.HelpTipButton();
			this.w_btnUpdateItemOptions = new DevExpress.XtraEditors.SimpleButton();
			this.w_cbCheckModifiedDatesForItemsDocuments = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbUpdateItems = new DevExpress.XtraEditors.CheckEdit();
			this.w_rbPreserveItems = new DevExpress.XtraEditors.CheckEdit();
			this.w_rbOverwriteItems = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbPropagateItemDeletions = new DevExpress.XtraEditors.CheckEdit();
			this.w_gbOtherOptions = new DevExpress.XtraEditors.GroupControl();
			this.w_helpPropagateItemDeletions = new TooltipsTest.HelpTipButton();
			this.w_plSites = new DevExpress.XtraEditors.PanelControl();
			this.w_plLists = new DevExpress.XtraEditors.PanelControl();
			this.panel2 = new DevExpress.XtraEditors.PanelControl();
			this.w_plOtherOptions = new DevExpress.XtraEditors.PanelControl();
			this.w_helpCustomMode = new TooltipsTest.HelpTipButton();
			((System.ComponentModel.ISupportInitialize)this.w_rbCustomMode.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbIncrementalMode.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbFullMode.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_gbSites).BeginInit();
			this.w_gbSites.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_cbUpdateSites.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbPreserveSites.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbOverwriteSites.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_gbLists).BeginInit();
			this.w_gbLists.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_helpUpdateLists).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCheckModifiedDatesForLists.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbUpdateLists.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbPreserveLists.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbOverwriteLists.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_gbItems).BeginInit();
			this.w_gbItems.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_helpUpdateItems).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCheckModifiedDatesForItemsDocuments.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbUpdateItems.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbPreserveItems.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbOverwriteItems.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbPropagateItemDeletions.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_gbOtherOptions).BeginInit();
			this.w_gbOtherOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_helpPropagateItemDeletions).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_plSites).BeginInit();
			this.w_plSites.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_plLists).BeginInit();
			this.w_plLists.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.panel2).BeginInit();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_plOtherOptions).BeginInit();
			this.w_plOtherOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_helpCustomMode).BeginInit();
			base.SuspendLayout();
			this.label2.Location = new System.Drawing.Point(136, 31);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(121, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "(Update existing objects)";
			this.w_lblFullCopyDescription.Location = new System.Drawing.Point(136, 8);
			this.w_lblFullCopyDescription.Name = "w_lblFullCopyDescription";
			this.w_lblFullCopyDescription.Size = new System.Drawing.Size(134, 13);
			this.w_lblFullCopyDescription.TabIndex = 1;
			this.w_lblFullCopyDescription.Text = "(Overwrite existing objects)";
			this.w_rbCustomMode.Location = new System.Drawing.Point(6, 52);
			this.w_rbCustomMode.Name = "w_rbCustomMode";
			this.w_rbCustomMode.Properties.Caption = "Custom Copy";
			this.w_rbCustomMode.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.w_rbCustomMode.Properties.RadioGroupIndex = 1;
			this.w_rbCustomMode.Size = new System.Drawing.Size(86, 19);
			this.w_rbCustomMode.TabIndex = 4;
			this.w_rbCustomMode.TabStop = false;
			this.w_rbCustomMode.CheckedChanged += new System.EventHandler(On_CheckedChanged);
			this.w_rbIncrementalMode.Location = new System.Drawing.Point(6, 29);
			this.w_rbIncrementalMode.Name = "w_rbIncrementalMode";
			this.w_rbIncrementalMode.Properties.Caption = "Incremental Copy";
			this.w_rbIncrementalMode.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.w_rbIncrementalMode.Properties.RadioGroupIndex = 1;
			this.w_rbIncrementalMode.Size = new System.Drawing.Size(106, 19);
			this.w_rbIncrementalMode.TabIndex = 2;
			this.w_rbIncrementalMode.TabStop = false;
			this.w_rbIncrementalMode.CheckedChanged += new System.EventHandler(On_CheckedChanged);
			this.w_rbFullMode.Location = new System.Drawing.Point(6, 6);
			this.w_rbFullMode.Name = "w_rbFullMode";
			this.w_rbFullMode.Properties.Caption = "Full Copy";
			this.w_rbFullMode.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.w_rbFullMode.Properties.RadioGroupIndex = 1;
			this.w_rbFullMode.Size = new System.Drawing.Size(67, 19);
			this.w_rbFullMode.TabIndex = 0;
			this.w_rbFullMode.TabStop = false;
			this.w_rbFullMode.CheckedChanged += new System.EventHandler(On_CheckedChanged);
			this.w_gbSites.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.w_gbSites.Controls.Add(this.w_btnUpdateSiteOptions);
			this.w_gbSites.Controls.Add(this.w_cbUpdateSites);
			this.w_gbSites.Controls.Add(this.w_rbPreserveSites);
			this.w_gbSites.Controls.Add(this.w_rbOverwriteSites);
			this.w_gbSites.Location = new System.Drawing.Point(0, 0);
			this.w_gbSites.Name = "w_gbSites";
			this.w_gbSites.Size = new System.Drawing.Size(381, 71);
			this.w_gbSites.TabIndex = 0;
			this.w_gbSites.Text = "Existing Sites";
			this.w_gbSites.UseDisabledStatePainter = false;
			this.w_btnUpdateSiteOptions.Enabled = false;
			this.w_btnUpdateSiteOptions.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_btnUpdateSiteOptions.Location = new System.Drawing.Point(220, 42);
			this.w_btnUpdateSiteOptions.Name = "w_btnUpdateSiteOptions";
			this.w_btnUpdateSiteOptions.Size = new System.Drawing.Size(35, 21);
			this.w_btnUpdateSiteOptions.TabIndex = 3;
			this.w_btnUpdateSiteOptions.Text = "...";
			this.w_btnUpdateSiteOptions.Click += new System.EventHandler(On_btnUpdateSiteOptions_Click);
			this.w_cbUpdateSites.Enabled = false;
			this.w_cbUpdateSites.Location = new System.Drawing.Point(127, 44);
			this.w_cbUpdateSites.Name = "w_cbUpdateSites";
			this.w_cbUpdateSites.Properties.Caption = "Update Sites";
			this.w_cbUpdateSites.Size = new System.Drawing.Size(84, 19);
			this.w_cbUpdateSites.TabIndex = 2;
			this.w_cbUpdateSites.CheckedChanged += new System.EventHandler(On_UpdateSitesCheckChanged);
			this.w_rbPreserveSites.Location = new System.Drawing.Point(17, 43);
			this.w_rbPreserveSites.Name = "w_rbPreserveSites";
			this.w_rbPreserveSites.Properties.Caption = "Preserve Sites";
			this.w_rbPreserveSites.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.w_rbPreserveSites.Properties.RadioGroupIndex = 2;
			this.w_rbPreserveSites.Size = new System.Drawing.Size(92, 19);
			this.w_rbPreserveSites.TabIndex = 1;
			this.w_rbPreserveSites.TabStop = false;
			this.w_rbPreserveSites.CheckedChanged += new System.EventHandler(On_CheckedChanged);
			this.w_rbOverwriteSites.EditValue = true;
			this.w_rbOverwriteSites.Location = new System.Drawing.Point(17, 24);
			this.w_rbOverwriteSites.Name = "w_rbOverwriteSites";
			this.w_rbOverwriteSites.Properties.Caption = "Overwrite Sites";
			this.w_rbOverwriteSites.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.w_rbOverwriteSites.Properties.RadioGroupIndex = 2;
			this.w_rbOverwriteSites.Size = new System.Drawing.Size(95, 19);
			this.w_rbOverwriteSites.TabIndex = 0;
			this.w_rbOverwriteSites.CheckedChanged += new System.EventHandler(On_rbOverwriteSites_CheckedChanged);
			this.w_gbLists.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.w_gbLists.Controls.Add(this.w_helpUpdateLists);
			this.w_gbLists.Controls.Add(this.w_cbCheckModifiedDatesForLists);
			this.w_gbLists.Controls.Add(this.w_btnUpdateListOptions);
			this.w_gbLists.Controls.Add(this.w_cbUpdateLists);
			this.w_gbLists.Controls.Add(this.w_rbPreserveLists);
			this.w_gbLists.Controls.Add(this.w_rbOverwriteLists);
			this.w_gbLists.Location = new System.Drawing.Point(0, 0);
			this.w_gbLists.Name = "w_gbLists";
			this.w_gbLists.Size = new System.Drawing.Size(381, 90);
			this.w_gbLists.TabIndex = 0;
			this.w_gbLists.Text = "Existing Lists";
			this.w_gbLists.UseDisabledStatePainter = false;
			this.w_helpUpdateLists.AnchoringControl = this.w_btnUpdateListOptions;
			this.w_helpUpdateLists.BackColor = System.Drawing.Color.Transparent;
			this.w_helpUpdateLists.CommonParentControl = null;
			this.w_helpUpdateLists.Image = (System.Drawing.Image)resources.GetObject("w_helpUpdateLists.Image");
			this.w_helpUpdateLists.Location = new System.Drawing.Point(262, 41);
			this.w_helpUpdateLists.MaximumSize = new System.Drawing.Size(20, 20);
			this.w_helpUpdateLists.MinimumSize = new System.Drawing.Size(20, 20);
			this.w_helpUpdateLists.Name = "w_helpUpdateLists";
			this.w_helpUpdateLists.Size = new System.Drawing.Size(20, 20);
			this.w_helpUpdateLists.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.w_helpUpdateLists.TabIndex = 5;
			this.w_helpUpdateLists.TabStop = false;
			this.w_btnUpdateListOptions.Enabled = false;
			this.w_btnUpdateListOptions.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_btnUpdateListOptions.Location = new System.Drawing.Point(220, 41);
			this.w_btnUpdateListOptions.Name = "w_btnUpdateListOptions";
			this.w_btnUpdateListOptions.Size = new System.Drawing.Size(35, 21);
			this.w_btnUpdateListOptions.TabIndex = 3;
			this.w_btnUpdateListOptions.Text = "...";
			this.w_btnUpdateListOptions.Click += new System.EventHandler(On_btnUpdateListOptions_Click);
			this.w_cbCheckModifiedDatesForLists.Enabled = false;
			this.w_cbCheckModifiedDatesForLists.Location = new System.Drawing.Point(129, 66);
			this.w_cbCheckModifiedDatesForLists.Name = "w_cbCheckModifiedDatesForLists";
			this.w_cbCheckModifiedDatesForLists.Properties.Caption = "Check modified dates for Lists";
			this.w_cbCheckModifiedDatesForLists.Size = new System.Drawing.Size(177, 19);
			this.w_cbCheckModifiedDatesForLists.TabIndex = 4;
			this.w_cbUpdateLists.Enabled = false;
			this.w_cbUpdateLists.Location = new System.Drawing.Point(129, 43);
			this.w_cbUpdateLists.Name = "w_cbUpdateLists";
			this.w_cbUpdateLists.Properties.Caption = "Update Lists";
			this.w_cbUpdateLists.Size = new System.Drawing.Size(82, 19);
			this.w_cbUpdateLists.TabIndex = 2;
			this.w_cbUpdateLists.CheckedChanged += new System.EventHandler(On_UpdateListsCheckChanged);
			this.w_rbPreserveLists.Location = new System.Drawing.Point(17, 42);
			this.w_rbPreserveLists.Name = "w_rbPreserveLists";
			this.w_rbPreserveLists.Properties.Caption = "Preserve Lists";
			this.w_rbPreserveLists.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.w_rbPreserveLists.Properties.RadioGroupIndex = 3;
			this.w_rbPreserveLists.Size = new System.Drawing.Size(90, 19);
			this.w_rbPreserveLists.TabIndex = 1;
			this.w_rbPreserveLists.TabStop = false;
			this.w_rbPreserveLists.CheckedChanged += new System.EventHandler(On_CheckedChanged);
			this.w_rbOverwriteLists.EditValue = true;
			this.w_rbOverwriteLists.Location = new System.Drawing.Point(17, 23);
			this.w_rbOverwriteLists.Name = "w_rbOverwriteLists";
			this.w_rbOverwriteLists.Properties.Caption = "Overwrite Lists";
			this.w_rbOverwriteLists.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.w_rbOverwriteLists.Properties.RadioGroupIndex = 3;
			this.w_rbOverwriteLists.Size = new System.Drawing.Size(93, 19);
			this.w_rbOverwriteLists.TabIndex = 0;
			this.w_rbOverwriteLists.CheckedChanged += new System.EventHandler(On_rbOverwriteLists_CheckedChanged);
			this.w_gbItems.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.w_gbItems.Controls.Add(this.w_helpUpdateItems);
			this.w_gbItems.Controls.Add(this.w_cbCheckModifiedDatesForItemsDocuments);
			this.w_gbItems.Controls.Add(this.w_btnUpdateItemOptions);
			this.w_gbItems.Controls.Add(this.w_cbUpdateItems);
			this.w_gbItems.Controls.Add(this.w_rbPreserveItems);
			this.w_gbItems.Controls.Add(this.w_rbOverwriteItems);
			this.w_gbItems.Location = new System.Drawing.Point(0, 0);
			this.w_gbItems.Name = "w_gbItems";
			this.w_gbItems.Size = new System.Drawing.Size(381, 92);
			this.w_gbItems.TabIndex = 0;
			this.w_gbItems.Text = "Existing Items / Documents";
			this.w_gbItems.UseDisabledStatePainter = false;
			this.w_helpUpdateItems.AnchoringControl = this.w_btnUpdateItemOptions;
			this.w_helpUpdateItems.BackColor = System.Drawing.Color.Transparent;
			this.w_helpUpdateItems.CommonParentControl = null;
			this.w_helpUpdateItems.Image = (System.Drawing.Image)resources.GetObject("w_helpUpdateItems.Image");
			this.w_helpUpdateItems.Location = new System.Drawing.Point(266, 41);
			this.w_helpUpdateItems.MaximumSize = new System.Drawing.Size(20, 20);
			this.w_helpUpdateItems.MinimumSize = new System.Drawing.Size(20, 20);
			this.w_helpUpdateItems.Name = "w_helpUpdateItems";
			this.w_helpUpdateItems.Size = new System.Drawing.Size(20, 20);
			this.w_helpUpdateItems.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.w_helpUpdateItems.TabIndex = 5;
			this.w_helpUpdateItems.TabStop = false;
			this.w_btnUpdateItemOptions.Enabled = false;
			this.w_btnUpdateItemOptions.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_btnUpdateItemOptions.Location = new System.Drawing.Point(224, 41);
			this.w_btnUpdateItemOptions.Name = "w_btnUpdateItemOptions";
			this.w_btnUpdateItemOptions.Size = new System.Drawing.Size(35, 21);
			this.w_btnUpdateItemOptions.TabIndex = 3;
			this.w_btnUpdateItemOptions.Text = "...";
			this.w_btnUpdateItemOptions.Click += new System.EventHandler(On_btnUpdateItemOptions_Clicked);
			this.w_cbCheckModifiedDatesForItemsDocuments.Enabled = false;
			this.w_cbCheckModifiedDatesForItemsDocuments.Location = new System.Drawing.Point(129, 66);
			this.w_cbCheckModifiedDatesForItemsDocuments.Name = "w_cbCheckModifiedDatesForItemsDocuments";
			this.w_cbCheckModifiedDatesForItemsDocuments.Properties.Caption = "Check modified dates for Items/Documents";
			this.w_cbCheckModifiedDatesForItemsDocuments.Size = new System.Drawing.Size(247, 19);
			this.w_cbCheckModifiedDatesForItemsDocuments.TabIndex = 4;
			this.w_cbUpdateItems.Enabled = false;
			this.w_cbUpdateItems.Location = new System.Drawing.Point(129, 43);
			this.w_cbUpdateItems.Name = "w_cbUpdateItems";
			this.w_cbUpdateItems.Properties.Caption = "Update Items";
			this.w_cbUpdateItems.Size = new System.Drawing.Size(86, 19);
			this.w_cbUpdateItems.TabIndex = 2;
			this.w_cbUpdateItems.CheckedChanged += new System.EventHandler(On_UpdateItemsCheckChanged);
			this.w_rbPreserveItems.Location = new System.Drawing.Point(17, 42);
			this.w_rbPreserveItems.Name = "w_rbPreserveItems";
			this.w_rbPreserveItems.Properties.Caption = "Preserve Items";
			this.w_rbPreserveItems.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.w_rbPreserveItems.Properties.RadioGroupIndex = 4;
			this.w_rbPreserveItems.Size = new System.Drawing.Size(94, 19);
			this.w_rbPreserveItems.TabIndex = 1;
			this.w_rbPreserveItems.TabStop = false;
			this.w_rbPreserveItems.CheckedChanged += new System.EventHandler(On_CheckedChanged);
			this.w_rbOverwriteItems.EditValue = true;
			this.w_rbOverwriteItems.Location = new System.Drawing.Point(17, 23);
			this.w_rbOverwriteItems.Name = "w_rbOverwriteItems";
			this.w_rbOverwriteItems.Properties.Caption = "Overwrite Items";
			this.w_rbOverwriteItems.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.w_rbOverwriteItems.Properties.RadioGroupIndex = 4;
			this.w_rbOverwriteItems.Size = new System.Drawing.Size(111, 19);
			this.w_rbOverwriteItems.TabIndex = 0;
			this.w_rbOverwriteItems.CheckedChanged += new System.EventHandler(On_CheckedChanged);
			this.w_cbPropagateItemDeletions.Location = new System.Drawing.Point(17, 26);
			this.w_cbPropagateItemDeletions.Name = "w_cbPropagateItemDeletions";
			this.w_cbPropagateItemDeletions.Properties.Caption = "Migrate Item Deletions Only";
			this.w_cbPropagateItemDeletions.Size = new System.Drawing.Size(213, 19);
			this.w_cbPropagateItemDeletions.TabIndex = 0;
			this.w_cbPropagateItemDeletions.CheckedChanged += new System.EventHandler(On_CheckedChanged);
			this.w_gbOtherOptions.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.w_gbOtherOptions.Controls.Add(this.w_helpPropagateItemDeletions);
			this.w_gbOtherOptions.Controls.Add(this.w_cbPropagateItemDeletions);
			this.w_gbOtherOptions.Location = new System.Drawing.Point(0, 0);
			this.w_gbOtherOptions.Name = "w_gbOtherOptions";
			this.w_gbOtherOptions.Size = new System.Drawing.Size(381, 52);
			this.w_gbOtherOptions.TabIndex = 0;
			this.w_gbOtherOptions.Text = "Other Options";
			this.w_gbOtherOptions.UseDisabledStatePainter = false;
			this.w_helpPropagateItemDeletions.AnchoringControl = this.w_cbPropagateItemDeletions;
			this.w_helpPropagateItemDeletions.BackColor = System.Drawing.Color.Transparent;
			this.w_helpPropagateItemDeletions.CommonParentControl = null;
			this.w_helpPropagateItemDeletions.Image = (System.Drawing.Image)resources.GetObject("w_helpPropagateItemDeletions.Image");
			this.w_helpPropagateItemDeletions.Location = new System.Drawing.Point(232, 25);
			this.w_helpPropagateItemDeletions.MaximumSize = new System.Drawing.Size(20, 20);
			this.w_helpPropagateItemDeletions.MinimumSize = new System.Drawing.Size(20, 20);
			this.w_helpPropagateItemDeletions.Name = "w_helpPropagateItemDeletions";
			this.w_helpPropagateItemDeletions.Size = new System.Drawing.Size(20, 20);
			this.w_helpPropagateItemDeletions.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.w_helpPropagateItemDeletions.TabIndex = 1;
			this.w_helpPropagateItemDeletions.TabStop = false;
			this.w_plSites.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.w_plSites.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plSites.Controls.Add(this.w_gbSites);
			this.w_plSites.Location = new System.Drawing.Point(23, 75);
			this.w_plSites.Name = "w_plSites";
			this.w_plSites.Size = new System.Drawing.Size(395, 74);
			this.w_plSites.TabIndex = 75;
			this.w_plLists.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.w_plLists.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plLists.Controls.Add(this.w_gbLists);
			this.w_plLists.Location = new System.Drawing.Point(23, 146);
			this.w_plLists.Name = "w_plLists";
			this.w_plLists.Size = new System.Drawing.Size(395, 90);
			this.w_plLists.TabIndex = 76;
			this.panel2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.panel2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panel2.Controls.Add(this.w_gbItems);
			this.panel2.Location = new System.Drawing.Point(23, 235);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(395, 95);
			this.panel2.TabIndex = 77;
			this.w_plOtherOptions.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.w_plOtherOptions.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plOtherOptions.Controls.Add(this.w_gbOtherOptions);
			this.w_plOtherOptions.Location = new System.Drawing.Point(23, 328);
			this.w_plOtherOptions.Name = "w_plOtherOptions";
			this.w_plOtherOptions.Size = new System.Drawing.Size(395, 59);
			this.w_plOtherOptions.TabIndex = 78;
			this.w_helpCustomMode.AnchoringControl = this.w_rbCustomMode;
			this.w_helpCustomMode.BackColor = System.Drawing.Color.Transparent;
			this.w_helpCustomMode.CommonParentControl = null;
			this.w_helpCustomMode.Image = (System.Drawing.Image)resources.GetObject("w_helpCustomMode.Image");
			this.w_helpCustomMode.Location = new System.Drawing.Point(95, 52);
			this.w_helpCustomMode.MaximumSize = new System.Drawing.Size(20, 20);
			this.w_helpCustomMode.MinimumSize = new System.Drawing.Size(20, 20);
			this.w_helpCustomMode.Name = "w_helpCustomMode";
			this.w_helpCustomMode.Size = new System.Drawing.Size(20, 20);
			this.w_helpCustomMode.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.w_helpCustomMode.TabIndex = 79;
			this.w_helpCustomMode.TabStop = false;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.w_helpCustomMode);
			base.Controls.Add(this.w_rbCustomMode);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.w_lblFullCopyDescription);
			base.Controls.Add(this.w_rbIncrementalMode);
			base.Controls.Add(this.w_rbFullMode);
			base.Controls.Add(this.w_plSites);
			base.Controls.Add(this.w_plLists);
			base.Controls.Add(this.panel2);
			base.Controls.Add(this.w_plOtherOptions);
			base.Name = "TCMigrationModeOptions";
			base.Size = new System.Drawing.Size(421, 390);
			((System.ComponentModel.ISupportInitialize)this.w_rbCustomMode.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbIncrementalMode.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbFullMode.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_gbSites).EndInit();
			this.w_gbSites.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_cbUpdateSites.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbPreserveSites.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbOverwriteSites.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_gbLists).EndInit();
			this.w_gbLists.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_helpUpdateLists).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCheckModifiedDatesForLists.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbUpdateLists.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbPreserveLists.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbOverwriteLists.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_gbItems).EndInit();
			this.w_gbItems.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_helpUpdateItems).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCheckModifiedDatesForItemsDocuments.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbUpdateItems.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbPreserveItems.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbOverwriteItems.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbPropagateItemDeletions.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_gbOtherOptions).EndInit();
			this.w_gbOtherOptions.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_helpPropagateItemDeletions).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_plSites).EndInit();
			this.w_plSites.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_plLists).EndInit();
			this.w_plLists.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.panel2).EndInit();
			this.panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_plOtherOptions).EndInit();
			this.w_plOtherOptions.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_helpCustomMode).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		protected override void LoadUI()
		{
			LoadIncrementalMode(Options);
			w_rbFullMode.Checked = Options.MigrationMode == MigrationMode.Full;
			w_rbIncrementalMode.Checked = Options.MigrationMode == MigrationMode.Incremental;
			w_rbCustomMode.Checked = Options.MigrationMode == MigrationMode.Custom || Options.MigrationMode == MigrationMode.BackwardsCompatibility;
			w_rbOverwriteItems.Checked = Options.ItemCopyingMode == ListItemCopyMode.Overwrite;
			w_rbPreserveItems.Checked = Options.ItemCopyingMode != ListItemCopyMode.Overwrite;
			bool flag = base.Scope == SharePointObjectScope.SiteCollection || base.Scope == SharePointObjectScope.Site;
			if (!flag && base.Scope != SharePointObjectScope.List)
			{
				w_rbPreserveLists.Checked = true;
			}
			else
			{
				w_rbOverwriteLists.Checked = Options.OverwriteLists;
				w_rbPreserveLists.Checked = !Options.OverwriteLists;
				if (!flag)
				{
					w_rbPreserveSites.Checked = true;
				}
				else
				{
					w_rbOverwriteSites.Checked = Options.OverwriteSites;
					w_rbPreserveSites.Checked = !Options.OverwriteSites;
				}
			}
			m_bSuppressUpdateSitesCheckedEvent = true;
			w_cbUpdateSites.Checked = Options.UpdateSites;
			m_bSuppressUpdateSitesCheckedEvent = false;
			m_bSuppressUpdateListsCheckedEvent = true;
			w_cbUpdateLists.Checked = Options.UpdateLists;
			w_cbCheckModifiedDatesForLists.Checked = Options.CheckModifiedDatesForLists;
			m_bSuppressUpdateListsCheckedEvent = false;
			m_bSuppressUpdateItemsCheckedEvent = true;
			w_cbUpdateItems.Checked = Options.UpdateItems;
			w_cbCheckModifiedDatesForItemsDocuments.Checked = Options.CheckModifiedDatesForItemsDocuments;
			m_bSuppressUpdateItemsCheckedEvent = false;
			w_cbPropagateItemDeletions.Checked = Options.PropagateItemDeletions;
			UpdateEnabledState();
		}

		protected override void MultiSelectUISetup()
		{
		}

		private void On_btnUpdateItemOptions_Clicked(object sender, EventArgs e)
		{
			OpenUpdateItemsDialog();
		}

		private void On_btnUpdateListOptions_Click(object sender, EventArgs e)
		{
			OpenUpdateListsDialog();
		}

		private void On_btnUpdateSiteOptions_Click(object sender, EventArgs e)
		{
			OpenUpdateSitesDialog();
		}

		private void On_CheckedChanged(object sender, EventArgs e)
		{
			UpdateEnabledState();
			FireMigrationModeChangedEvent();
		}

		private void On_rbOverwriteLists_CheckedChanged(object sender, EventArgs e)
		{
			if (w_rbOverwriteLists.Checked)
			{
				w_rbOverwriteItems.Checked = true;
			}
			On_CheckedChanged(sender, e);
		}

		private void On_rbOverwriteSites_CheckedChanged(object sender, EventArgs e)
		{
			if (w_rbOverwriteSites.Checked)
			{
				w_rbOverwriteLists.Checked = true;
			}
			if (w_rbOverwriteLists.Checked)
			{
				w_rbOverwriteItems.Checked = true;
			}
			On_CheckedChanged(sender, e);
		}

		private void On_UpdateItemsCheckChanged(object sender, EventArgs e)
		{
			if (w_cbUpdateItems.Checked && !m_bSuppressUpdateItemsCheckedEvent && !OpenUpdateItemsDialog())
			{
				w_cbUpdateItems.Checked = false;
			}
			UpdateEnabledState();
		}

		private void On_UpdateListsCheckChanged(object sender, EventArgs e)
		{
			if (w_cbUpdateLists.Checked && !m_bSuppressUpdateListsCheckedEvent && !OpenUpdateListsDialog())
			{
				w_cbUpdateLists.Checked = false;
			}
			UpdateEnabledState();
		}

		private void On_UpdateSitesCheckChanged(object sender, EventArgs e)
		{
			if (w_cbUpdateSites.Checked && !m_bSuppressUpdateSitesCheckedEvent && !OpenUpdateSitesDialog())
			{
				w_cbUpdateSites.Checked = false;
			}
			UpdateEnabledState();
		}

		private bool OpenUpdateItemsDialog()
		{
			UpdateItemOptionsDialog updateItemOptionsDialog = new UpdateItemOptionsDialog(Options.UpdateItemOptionsBitField);
			DialogResult dialogResult = updateItemOptionsDialog.ShowDialog();
			if (dialogResult == DialogResult.OK)
			{
				Options.UpdateItemOptionsBitField = updateItemOptionsDialog.UpdateItemOptionsBitField;
			}
			if (Options.UpdateItemOptionsBitField == 0)
			{
				w_cbUpdateItems.Checked = false;
			}
			return dialogResult == DialogResult.OK;
		}

		private bool OpenUpdateListsDialog()
		{
			UpdateListOptionsDialog updateListOptionsDialog = new UpdateListOptionsDialog(Options.UpdateListOptionsBitField);
			DialogResult dialogResult = updateListOptionsDialog.ShowDialog();
			if (dialogResult == DialogResult.OK)
			{
				Options.UpdateListOptionsBitField = updateListOptionsDialog.UpdateListOptionsBitField;
			}
			if (Options.UpdateListOptionsBitField == 0)
			{
				w_cbUpdateLists.Checked = false;
			}
			return dialogResult == DialogResult.OK;
		}

		private bool OpenUpdateSitesDialog()
		{
			UpdateSiteOptionsDialog updateSiteOptionsDialog = new UpdateSiteOptionsDialog(Options.UpdateSiteOptionsBitField);
			DialogResult dialogResult = updateSiteOptionsDialog.ShowDialog();
			if (dialogResult == DialogResult.OK)
			{
				Options.UpdateSiteOptionsBitField = updateSiteOptionsDialog.UpdateSiteOptionsBitField;
			}
			if (Options.UpdateSiteOptionsBitField == 0)
			{
				w_cbUpdateSites.Checked = false;
			}
			return dialogResult == DialogResult.OK;
		}

		private void SaveCustomModeSettings()
		{
			Options.MigrationMode = MigrationMode.Custom;
			Options.OverwriteSites = w_rbOverwriteSites.Checked;
			Options.OverwriteLists = w_rbOverwriteLists.Checked;
			Options.ItemCopyingMode = ((!w_rbOverwriteItems.Checked) ? ListItemCopyMode.Preserve : ListItemCopyMode.Overwrite);
			Options.UpdateSites = w_cbUpdateSites.Checked;
			Options.UpdateLists = w_cbUpdateLists.Checked;
			Options.CheckModifiedDatesForLists = w_cbCheckModifiedDatesForLists.Checked;
			Options.UpdateItems = w_cbUpdateItems.Checked;
			Options.CheckModifiedDatesForItemsDocuments = w_cbCheckModifiedDatesForItemsDocuments.Checked;
			Options.PropagateItemDeletions = !Options.OverwriteLists && w_cbPropagateItemDeletions.Checked;
		}

		public override bool SaveUI()
		{
			if (w_rbFullMode.Checked)
			{
				SaveFullModeSettings(Options);
			}
			else if (w_rbIncrementalMode.Checked)
			{
				SaveIncrementalModeSettings(Options);
			}
			else if (w_rbCustomMode.Checked)
			{
				SaveCustomModeSettings();
			}
			return true;
		}

		protected override void UpdateEnabledState()
		{
			bool flag = base.Scope == SharePointObjectScope.SiteCollection || base.Scope == SharePointObjectScope.Site;
			bool flag2 = flag || base.Scope == SharePointObjectScope.List;
			w_gbSites.Enabled = w_rbCustomMode.Checked;
			GroupControl groupControl = w_gbLists;
			bool enabled = w_rbCustomMode.Checked && (!flag || !w_rbOverwriteSites.Checked);
			groupControl.Enabled = enabled;
			GroupControl groupControl2 = w_gbItems;
			bool enabled2 = w_rbCustomMode.Checked && (!flag2 || !w_rbOverwriteLists.Checked) && (!flag || !w_rbOverwriteSites.Checked);
			groupControl2.Enabled = enabled2;
			GroupControl groupControl3 = w_gbOtherOptions;
			bool enabled3 = w_rbCustomMode.Checked && !w_rbOverwriteItems.Checked && (!flag2 || !w_rbOverwriteLists.Checked) && (!flag || !w_rbOverwriteSites.Checked);
			groupControl3.Enabled = enabled3;
			w_cbUpdateSites.Enabled = w_rbPreserveSites.Checked;
			w_btnUpdateSiteOptions.Enabled = w_cbUpdateSites.Enabled && w_cbUpdateSites.Checked;
			CheckEdit checkEdit = w_cbCheckModifiedDatesForLists;
			CheckEdit checkEdit2 = w_cbUpdateLists;
			bool enabled4 = (checkEdit2.Enabled = w_rbPreserveLists.Checked);
			checkEdit.Enabled = enabled4;
			w_btnUpdateListOptions.Enabled = w_cbUpdateLists.Enabled && w_cbUpdateLists.Checked;
			CheckEdit checkEdit3 = w_cbCheckModifiedDatesForItemsDocuments;
			CheckEdit checkEdit4 = w_cbUpdateItems;
			bool enabled5 = (checkEdit4.Enabled = w_rbPreserveItems.Checked);
			checkEdit3.Enabled = enabled5;
			w_btnUpdateItemOptions.Enabled = w_cbUpdateItems.Enabled && w_cbUpdateItems.Checked;
			w_cbPropagateItemDeletions.Visible = PropagateDeletionsSupported;
			w_cbPropagateItemDeletions.Enabled = w_rbCustomMode.Checked && !w_rbOverwriteLists.Checked && PropagateDeletionsSupported;
		}

		protected override void UpdateScope()
		{
			switch (base.Scope)
			{
			case SharePointObjectScope.List:
				HideControl(w_plSites);
				break;
			case SharePointObjectScope.Folder:
				HideControl(w_plSites);
				HideControl(w_plLists);
				break;
			case SharePointObjectScope.Item:
				HideControl(w_plSites);
				HideControl(w_plLists);
				HideControl(w_plOtherOptions);
				break;
			}
		}
	}
}
