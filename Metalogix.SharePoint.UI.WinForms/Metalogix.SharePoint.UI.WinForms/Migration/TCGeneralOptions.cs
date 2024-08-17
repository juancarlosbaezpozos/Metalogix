using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.Actions;
using Metalogix.DataStructures;
using Metalogix.SharePoint.Actions.BCS;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Migration;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Actions;
using Metalogix.UI.WinForms.Components;
using TooltipsTest;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Migration.GeneralOptions.png")]
	[ControlName("General Options")]
	public class TCGeneralOptions : ScopableTabbableControl
	{
		private static readonly int LINK_CORRECTION_SCOPE_INDENT;

		private SPGeneralOptions m_options;

		private SendEmailOptions m_sendEmailOptions = new SendEmailOptions();

		private bool m_bCopyingNavigationStructure;

		private bool m_bCopyingPortalListings;

		private bool bInitializing;

		private IContainer components;

		internal LabelControl w_lblComparisonLevel;

		internal CheckEdit w_cbVerbose;

		internal CheckEdit w_cbCheckResults;

		internal CheckEdit w_cbForceRefresh;

		internal CheckEdit w_cbCorrectLinks;

		internal CheckEdit w_cbTextFieldLinkCorrection;

		private PanelControl w_plCheckResults;

		private PanelControl w_plLinkCorrectionScope;

		private CheckEdit w_rbCopyScope;

		private CheckEdit w_rbSiteCollectionScope;

		private LabelControl w_lblLinkCorrectionScope;

		private PanelControl w_plLinkCorrection;

		private CheckEdit w_cbLogSkippedItems;

		internal CheckEdit w_cbSendEmail;

		private SimpleButton w_btnConfigureEmailSettings;

		private CheckEdit w_cbOverrideCheckouts;

		private CheckEdit w_cbComprehensiveLinkCorrection;

		private PanelControl w_plComprehensiveLinkCorrection;

		private HelpTipButton w_helpCorrectLinks;

		private HelpTipButton w_helpComprehensiveLinkCorrection;

		private HelpTipButton w_helpVerbose;

		private HelpTipButton w_helpForceRefresh;

		private ComboBoxEdit w_cmbLevel;

		protected bool CopyingNavigationStructure
		{
			get
			{
				return m_bCopyingNavigationStructure;
			}
			set
			{
				m_bCopyingNavigationStructure = value;
			}
		}

		protected bool CopyingPortalListings
		{
			get
			{
				return m_bCopyingPortalListings;
			}
			set
			{
				m_bCopyingPortalListings = value;
			}
		}

		public bool DisplayCheckResults
		{
			get
			{
				return w_plCheckResults.Visible;
			}
			set
			{
				if (DisplayCheckResults != value)
				{
					if (value)
					{
						ShowControl(w_plCheckResults, this);
					}
					else
					{
						HideControl(w_plCheckResults);
					}
				}
			}
		}

		public bool DisplayLinkCorrectionOption
		{
			get
			{
				return w_plLinkCorrection.Visible;
			}
			set
			{
				if (value == DisplayLinkCorrectionOption)
				{
					return;
				}
				if (!value)
				{
					HideControl(w_plLinkCorrection);
					if (w_plLinkCorrectionScope.Visible)
					{
						HideControl(w_plLinkCorrectionScope);
					}
				}
				else
				{
					ShowControl(w_plLinkCorrection, this);
					if (!w_plLinkCorrectionScope.Visible)
					{
						ShowControl(w_plLinkCorrectionScope, this);
					}
				}
			}
		}

		public bool DisplayLogSkippedItems
		{
			get
			{
				return w_cbLogSkippedItems.Visible;
			}
			set
			{
				if (DisplayLogSkippedItems != value)
				{
					if (value)
					{
						ShowControl(w_cbLogSkippedItems, this);
					}
					else
					{
						HideControl(w_cbLogSkippedItems);
					}
				}
			}
		}

		public bool DisplayOverrideCheckouts
		{
			get
			{
				return w_cbOverrideCheckouts.Visible;
			}
			set
			{
				if (DisplayOverrideCheckouts != value)
				{
					if (value)
					{
						ShowControl(w_cbOverrideCheckouts, this);
					}
					else
					{
						HideControl(w_cbOverrideCheckouts);
					}
				}
			}
		}

		public bool DisplayTextFieldLinkCorrection
		{
			get
			{
				return w_cbTextFieldLinkCorrection.Visible;
			}
			set
			{
				if (DisplayTextFieldLinkCorrection != value)
				{
					if (value)
					{
						ShowControl(w_cbTextFieldLinkCorrection, w_plLinkCorrection);
					}
					else
					{
						HideControl(w_cbTextFieldLinkCorrection);
					}
				}
			}
		}

		public bool DisplayVerboseLogging
		{
			get
			{
				return w_cbVerbose.Visible;
			}
			set
			{
				if (DisplayVerboseLogging != value)
				{
					if (value)
					{
						ShowControl(w_cbVerbose, this);
						w_helpVerbose.Visible = true;
					}
					else
					{
						HideControl(w_cbVerbose);
						w_helpVerbose.Visible = false;
					}
				}
			}
		}

		public bool LinkCorrectionIsOptional
		{
			get
			{
				return w_cbCorrectLinks.Visible;
			}
			set
			{
				if (value != LinkCorrectionIsOptional)
				{
					if (!value)
					{
						HideControl(w_cbCorrectLinks);
						PanelControl panelControl = w_plLinkCorrectionScope;
						int num = w_plLinkCorrectionScope.Location.X - LINK_CORRECTION_SCOPE_INDENT;
						panelControl.Location = new Point(num, w_plLinkCorrectionScope.Location.Y);
						CheckEdit checkEdit = w_cbTextFieldLinkCorrection;
						int num2 = w_cbTextFieldLinkCorrection.Location.X - LINK_CORRECTION_SCOPE_INDENT;
						checkEdit.Location = new Point(num2, w_cbTextFieldLinkCorrection.Location.Y);
					}
					else
					{
						ShowControl(w_cbCorrectLinks, w_plLinkCorrection);
						PanelControl panelControl2 = w_plLinkCorrectionScope;
						int num3 = w_plLinkCorrectionScope.Location.X + LINK_CORRECTION_SCOPE_INDENT;
						panelControl2.Location = new Point(num3, w_plLinkCorrectionScope.Location.Y);
						CheckEdit checkEdit2 = w_cbTextFieldLinkCorrection;
						int num4 = w_cbTextFieldLinkCorrection.Location.X + LINK_CORRECTION_SCOPE_INDENT;
						checkEdit2.Location = new Point(num4, w_cbTextFieldLinkCorrection.Location.Y);
					}
				}
			}
		}

		public SPGeneralOptions Options
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

		static TCGeneralOptions()
		{
			LINK_CORRECTION_SCOPE_INDENT = 19;
		}

		public TCGeneralOptions()
		{
			InitializeComponent();
			if (!SharePointConfigurationVariables.AllowCheckResults || !DisplayCheckResults)
			{
				DisplayCheckResults = false;
			}
			w_cmbLevel.Properties.Items.Clear();
			w_cmbLevel.Properties.Items.Add(CompareLevel.Strict);
			w_cmbLevel.Properties.Items.Add(CompareLevel.Moderate);
			Type type = GetType();
			w_helpComprehensiveLinkCorrection.SetResourceString(type.FullName + w_cbComprehensiveLinkCorrection.Name, type);
			w_helpCorrectLinks.SetResourceString(type.FullName + w_cbCorrectLinks.Name, type);
			w_helpForceRefresh.SetResourceString(type.FullName + w_cbForceRefresh.Name, type);
			w_helpVerbose.SetResourceString(type.FullName + w_cbVerbose.Name, type);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		public override void HandleMessage(TabbableControl sender, string sMessage, object oValue)
		{
			if (sMessage == "CopyPortalListingsChanged")
			{
				CopyingPortalListings = (bool)oValue;
				UpdateEnabledState();
			}
			else if (sMessage == "CopyNavigationLinksAndHeadingsChanged")
			{
				CopyingNavigationStructure = (bool)oValue;
				UpdateEnabledState();
			}
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.TCGeneralOptions));
			this.w_cbForceRefresh = new DevExpress.XtraEditors.CheckEdit();
			this.w_lblComparisonLevel = new DevExpress.XtraEditors.LabelControl();
			this.w_cbVerbose = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbCheckResults = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbCorrectLinks = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbTextFieldLinkCorrection = new DevExpress.XtraEditors.CheckEdit();
			this.w_plCheckResults = new DevExpress.XtraEditors.PanelControl();
			this.w_cmbLevel = new DevExpress.XtraEditors.ComboBoxEdit();
			this.w_plLinkCorrectionScope = new DevExpress.XtraEditors.PanelControl();
			this.w_rbCopyScope = new DevExpress.XtraEditors.CheckEdit();
			this.w_rbSiteCollectionScope = new DevExpress.XtraEditors.CheckEdit();
			this.w_lblLinkCorrectionScope = new DevExpress.XtraEditors.LabelControl();
			this.w_plLinkCorrection = new DevExpress.XtraEditors.PanelControl();
			this.w_helpCorrectLinks = new TooltipsTest.HelpTipButton();
			this.w_plComprehensiveLinkCorrection = new DevExpress.XtraEditors.PanelControl();
			this.w_helpComprehensiveLinkCorrection = new TooltipsTest.HelpTipButton();
			this.w_cbComprehensiveLinkCorrection = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbLogSkippedItems = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbSendEmail = new DevExpress.XtraEditors.CheckEdit();
			this.w_btnConfigureEmailSettings = new DevExpress.XtraEditors.SimpleButton();
			this.w_cbOverrideCheckouts = new DevExpress.XtraEditors.CheckEdit();
			this.w_helpVerbose = new TooltipsTest.HelpTipButton();
			this.w_helpForceRefresh = new TooltipsTest.HelpTipButton();
			((System.ComponentModel.ISupportInitialize)this.w_cbForceRefresh.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbVerbose.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCheckResults.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCorrectLinks.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbTextFieldLinkCorrection.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_plCheckResults).BeginInit();
			this.w_plCheckResults.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_cmbLevel.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_plLinkCorrectionScope).BeginInit();
			this.w_plLinkCorrectionScope.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_rbCopyScope.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbSiteCollectionScope.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_plLinkCorrection).BeginInit();
			this.w_plLinkCorrection.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_helpCorrectLinks).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_plComprehensiveLinkCorrection).BeginInit();
			this.w_plComprehensiveLinkCorrection.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_helpComprehensiveLinkCorrection).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbComprehensiveLinkCorrection.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbLogSkippedItems.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbSendEmail.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbOverrideCheckouts.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_helpVerbose).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_helpForceRefresh).BeginInit();
			base.SuspendLayout();
			resources.ApplyResources(this.w_cbForceRefresh, "w_cbForceRefresh");
			this.w_cbForceRefresh.Name = "w_cbForceRefresh";
			this.w_cbForceRefresh.Properties.AutoWidth = true;
			this.w_cbForceRefresh.Properties.Caption = resources.GetString("w_cbForceRefresh.Properties.Caption");
			resources.ApplyResources(this.w_lblComparisonLevel, "w_lblComparisonLevel");
			this.w_lblComparisonLevel.Name = "w_lblComparisonLevel";
			resources.ApplyResources(this.w_cbVerbose, "w_cbVerbose");
			this.w_cbVerbose.Name = "w_cbVerbose";
			this.w_cbVerbose.Properties.AutoWidth = true;
			this.w_cbVerbose.Properties.Caption = resources.GetString("w_cbVerbose.Properties.Caption");
			resources.ApplyResources(this.w_cbCheckResults, "w_cbCheckResults");
			this.w_cbCheckResults.Name = "w_cbCheckResults";
			this.w_cbCheckResults.Properties.AutoWidth = true;
			this.w_cbCheckResults.Properties.Caption = resources.GetString("w_cbCheckResults.Properties.Caption");
			this.w_cbCheckResults.CheckedChanged += new System.EventHandler(On_CheckedResult_Changed);
			resources.ApplyResources(this.w_cbCorrectLinks, "w_cbCorrectLinks");
			this.w_cbCorrectLinks.Name = "w_cbCorrectLinks";
			this.w_cbCorrectLinks.Properties.AutoWidth = true;
			this.w_cbCorrectLinks.Properties.Caption = resources.GetString("w_cbCorrectLinks.Properties.Caption");
			this.w_cbCorrectLinks.CheckedChanged += new System.EventHandler(On_CheckedResult_Changed);
			resources.ApplyResources(this.w_cbTextFieldLinkCorrection, "w_cbTextFieldLinkCorrection");
			this.w_cbTextFieldLinkCorrection.Name = "w_cbTextFieldLinkCorrection";
			this.w_cbTextFieldLinkCorrection.Properties.AutoWidth = true;
			this.w_cbTextFieldLinkCorrection.Properties.Caption = resources.GetString("w_cbTextFieldLinkCorrection.Properties.Caption");
			resources.ApplyResources(this.w_plCheckResults, "w_plCheckResults");
			this.w_plCheckResults.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plCheckResults.Controls.Add(this.w_cmbLevel);
			this.w_plCheckResults.Controls.Add(this.w_cbCheckResults);
			this.w_plCheckResults.Controls.Add(this.w_lblComparisonLevel);
			this.w_plCheckResults.Name = "w_plCheckResults";
			resources.ApplyResources(this.w_cmbLevel, "w_cmbLevel");
			this.w_cmbLevel.Name = "w_cmbLevel";
			DevExpress.XtraEditors.Controls.EditorButtonCollection buttons = this.w_cmbLevel.Properties.Buttons;
			DevExpress.XtraEditors.Controls.EditorButton[] buttons2 = new DevExpress.XtraEditors.Controls.EditorButton[1]
			{
				new DevExpress.XtraEditors.Controls.EditorButton()
			};
			buttons.AddRange(buttons2);
			this.w_plLinkCorrectionScope.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plLinkCorrectionScope.Controls.Add(this.w_rbCopyScope);
			this.w_plLinkCorrectionScope.Controls.Add(this.w_rbSiteCollectionScope);
			this.w_plLinkCorrectionScope.Controls.Add(this.w_lblLinkCorrectionScope);
			resources.ApplyResources(this.w_plLinkCorrectionScope, "w_plLinkCorrectionScope");
			this.w_plLinkCorrectionScope.Name = "w_plLinkCorrectionScope";
			resources.ApplyResources(this.w_rbCopyScope, "w_rbCopyScope");
			this.w_rbCopyScope.Name = "w_rbCopyScope";
			this.w_rbCopyScope.Properties.AutoWidth = true;
			this.w_rbCopyScope.Properties.Caption = resources.GetString("w_rbCopyScope.Properties.Caption");
			this.w_rbCopyScope.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.w_rbCopyScope.Properties.RadioGroupIndex = 1;
			this.w_rbCopyScope.TabStop = false;
			resources.ApplyResources(this.w_rbSiteCollectionScope, "w_rbSiteCollectionScope");
			this.w_rbSiteCollectionScope.Name = "w_rbSiteCollectionScope";
			this.w_rbSiteCollectionScope.Properties.AutoWidth = true;
			this.w_rbSiteCollectionScope.Properties.Caption = resources.GetString("w_rbSiteCollectionScope.Properties.Caption");
			this.w_rbSiteCollectionScope.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.w_rbSiteCollectionScope.Properties.RadioGroupIndex = 1;
			resources.ApplyResources(this.w_lblLinkCorrectionScope, "w_lblLinkCorrectionScope");
			this.w_lblLinkCorrectionScope.Name = "w_lblLinkCorrectionScope";
			this.w_plLinkCorrection.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plLinkCorrection.Controls.Add(this.w_helpCorrectLinks);
			this.w_plLinkCorrection.Controls.Add(this.w_plComprehensiveLinkCorrection);
			this.w_plLinkCorrection.Controls.Add(this.w_plLinkCorrectionScope);
			this.w_plLinkCorrection.Controls.Add(this.w_cbCorrectLinks);
			this.w_plLinkCorrection.Controls.Add(this.w_cbTextFieldLinkCorrection);
			resources.ApplyResources(this.w_plLinkCorrection, "w_plLinkCorrection");
			this.w_plLinkCorrection.Name = "w_plLinkCorrection";
			this.w_helpCorrectLinks.AnchoringControl = this.w_cbCorrectLinks;
			this.w_helpCorrectLinks.BackColor = System.Drawing.Color.Transparent;
			this.w_helpCorrectLinks.CommonParentControl = null;
			resources.ApplyResources(this.w_helpCorrectLinks, "w_helpCorrectLinks");
			this.w_helpCorrectLinks.Name = "w_helpCorrectLinks";
			this.w_helpCorrectLinks.TabStop = false;
			this.w_plComprehensiveLinkCorrection.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plComprehensiveLinkCorrection.Controls.Add(this.w_helpComprehensiveLinkCorrection);
			this.w_plComprehensiveLinkCorrection.Controls.Add(this.w_cbComprehensiveLinkCorrection);
			resources.ApplyResources(this.w_plComprehensiveLinkCorrection, "w_plComprehensiveLinkCorrection");
			this.w_plComprehensiveLinkCorrection.Name = "w_plComprehensiveLinkCorrection";
			this.w_helpComprehensiveLinkCorrection.AnchoringControl = this.w_cbComprehensiveLinkCorrection;
			this.w_helpComprehensiveLinkCorrection.BackColor = System.Drawing.Color.Transparent;
			this.w_helpComprehensiveLinkCorrection.CommonParentControl = null;
			resources.ApplyResources(this.w_helpComprehensiveLinkCorrection, "w_helpComprehensiveLinkCorrection");
			this.w_helpComprehensiveLinkCorrection.Name = "w_helpComprehensiveLinkCorrection";
			this.w_helpComprehensiveLinkCorrection.TabStop = false;
			resources.ApplyResources(this.w_cbComprehensiveLinkCorrection, "w_cbComprehensiveLinkCorrection");
			this.w_cbComprehensiveLinkCorrection.Name = "w_cbComprehensiveLinkCorrection";
			this.w_cbComprehensiveLinkCorrection.Properties.AutoWidth = true;
			this.w_cbComprehensiveLinkCorrection.Properties.Caption = resources.GetString("w_cbComprehensiveLinkCorrection.Properties.Caption");
			resources.ApplyResources(this.w_cbLogSkippedItems, "w_cbLogSkippedItems");
			this.w_cbLogSkippedItems.Name = "w_cbLogSkippedItems";
			this.w_cbLogSkippedItems.Properties.AutoWidth = true;
			this.w_cbLogSkippedItems.Properties.Caption = resources.GetString("w_cbLogSkippedItems.Properties.Caption");
			resources.ApplyResources(this.w_cbSendEmail, "w_cbSendEmail");
			this.w_cbSendEmail.Name = "w_cbSendEmail";
			this.w_cbSendEmail.Properties.AutoWidth = true;
			this.w_cbSendEmail.Properties.Caption = resources.GetString("w_cbSendEmail.Properties.Caption");
			this.w_cbSendEmail.CheckedChanged += new System.EventHandler(w_cbSendEmail_CheckedChanged);
			resources.ApplyResources(this.w_btnConfigureEmailSettings, "w_btnConfigureEmailSettings");
			this.w_btnConfigureEmailSettings.Name = "w_btnConfigureEmailSettings";
			this.w_btnConfigureEmailSettings.Click += new System.EventHandler(On_btnConfigureEmailSettings_Clicked);
			resources.ApplyResources(this.w_cbOverrideCheckouts, "w_cbOverrideCheckouts");
			this.w_cbOverrideCheckouts.Name = "w_cbOverrideCheckouts";
			this.w_cbOverrideCheckouts.Properties.AutoWidth = true;
			this.w_cbOverrideCheckouts.Properties.Caption = resources.GetString("w_cbOverrideCheckouts.Properties.Caption");
			this.w_helpVerbose.AnchoringControl = this.w_cbVerbose;
			this.w_helpVerbose.BackColor = System.Drawing.Color.Transparent;
			this.w_helpVerbose.CommonParentControl = null;
			resources.ApplyResources(this.w_helpVerbose, "w_helpVerbose");
			this.w_helpVerbose.Name = "w_helpVerbose";
			this.w_helpVerbose.TabStop = false;
			this.w_helpForceRefresh.AnchoringControl = this.w_cbForceRefresh;
			this.w_helpForceRefresh.BackColor = System.Drawing.Color.Transparent;
			this.w_helpForceRefresh.CommonParentControl = null;
			resources.ApplyResources(this.w_helpForceRefresh, "w_helpForceRefresh");
			this.w_helpForceRefresh.Name = "w_helpForceRefresh";
			this.w_helpForceRefresh.TabStop = false;
			base.Controls.Add(this.w_helpForceRefresh);
			base.Controls.Add(this.w_helpVerbose);
			base.Controls.Add(this.w_cbOverrideCheckouts);
			base.Controls.Add(this.w_btnConfigureEmailSettings);
			base.Controls.Add(this.w_cbSendEmail);
			base.Controls.Add(this.w_cbLogSkippedItems);
			base.Controls.Add(this.w_plLinkCorrection);
			base.Controls.Add(this.w_plCheckResults);
			base.Controls.Add(this.w_cbForceRefresh);
			base.Controls.Add(this.w_cbVerbose);
			base.Name = "TCGeneralOptions";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)this.w_cbForceRefresh.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbVerbose.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCheckResults.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCorrectLinks.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbTextFieldLinkCorrection.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_plCheckResults).EndInit();
			this.w_plCheckResults.ResumeLayout(false);
			this.w_plCheckResults.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.w_cmbLevel.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_plLinkCorrectionScope).EndInit();
			this.w_plLinkCorrectionScope.ResumeLayout(false);
			this.w_plLinkCorrectionScope.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.w_rbCopyScope.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbSiteCollectionScope.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_plLinkCorrection).EndInit();
			this.w_plLinkCorrection.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_helpCorrectLinks).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_plComprehensiveLinkCorrection).EndInit();
			this.w_plComprehensiveLinkCorrection.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.w_helpComprehensiveLinkCorrection).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbComprehensiveLinkCorrection.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbLogSkippedItems.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbSendEmail.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbOverrideCheckouts.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_helpVerbose).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_helpForceRefresh).EndInit();
			base.ResumeLayout(false);
		}

		protected override void LoadUI()
		{
			bInitializing = true;
			w_cbSendEmail.Checked = Options.SendEmail;
			m_sendEmailOptions.SetFromOptions(Options);
			w_cbForceRefresh.Checked = Options.ForceRefresh;
			w_cbCheckResults.Checked = Options.CheckResults;
			w_cbVerbose.Checked = Options.Verbose;
			w_cbLogSkippedItems.Checked = Options.LogSkippedItems;
			w_cbOverrideCheckouts.Checked = Options.OverrideCheckouts;
			w_cbCorrectLinks.Checked = Options.CorrectingLinks;
			w_cbTextFieldLinkCorrection.Checked = Options.LinkCorrectTextFields;
			w_cbComprehensiveLinkCorrection.Checked = Options.UseComprehensiveLinkCorrection;
			if (Options.LinkCorrectionScope == LinkCorrectionScope.SiteCollection)
			{
				w_rbSiteCollectionScope.Checked = true;
			}
			else if (Options.LinkCorrectionScope == LinkCorrectionScope.MigrationOnly)
			{
				w_rbCopyScope.Checked = true;
			}
			w_cmbLevel.SelectedItem = Options.CompareOptions.Level;
			w_cmbLevel.Enabled = w_cbCheckResults.Checked;
			UpdateEnabledState();
			bInitializing = false;
		}

		private void On_btnConfigureEmailSettings_Clicked(object sender, EventArgs e)
		{
			ConfigureEmailSettingsDialog configureEmailSettingsDialog = new ConfigureEmailSettingsDialog
			{
				Options = m_sendEmailOptions
			};
			configureEmailSettingsDialog.ShowDialog();
		}

		private void On_cbCorrectLinks_CheckedChanged(object sender, EventArgs e)
		{
			UpdateEnabledState();
		}

		private void On_CheckedResult_Changed(object sender, EventArgs e)
		{
			UpdateEnabledState();
		}

		public override bool SaveUI()
		{
			Options.OverrideCheckouts = w_cbOverrideCheckouts.Checked;
			if (base.Scope != SharePointObjectScope.Permissions)
			{
				Options.CheckResults = w_cbCheckResults.Checked;
				Options.CorrectingLinks = w_cbCorrectLinks.Checked;
				Options.LinkCorrectTextFields = w_cbTextFieldLinkCorrection.Checked;
				Options.UseComprehensiveLinkCorrection = w_cbComprehensiveLinkCorrection.Checked;
				if (!w_rbCopyScope.Checked)
				{
					Options.LinkCorrectionScope = LinkCorrectionScope.SiteCollection;
				}
				else
				{
					Options.LinkCorrectionScope = LinkCorrectionScope.MigrationOnly;
				}
			}
			else
			{
				Options.CorrectingLinks = false;
				Options.LinkCorrectTextFields = false;
				Options.UseComprehensiveLinkCorrection = false;
				Options.CheckResults = false;
			}
			Options.SendEmail = w_cbSendEmail.Checked;
			Options.SetFromOptions(m_sendEmailOptions);
			Options.ForceRefresh = w_cbForceRefresh.Checked;
			Options.Verbose = w_cbVerbose.Checked;
			Options.LogSkippedItems = w_cbLogSkippedItems.Checked;
			Options.CompareOptions.Level = (CompareLevel)w_cmbLevel.SelectedItem;
			Options.EnableSslForEmail = AdapterConfigurationVariables.EnableSslForEmail;
			return true;
		}

		protected override void UpdateEnabledState()
		{
			if (base.Scope != SharePointObjectScope.Permissions)
			{
				w_lblComparisonLevel.Enabled = w_cbCheckResults.Checked;
				w_cmbLevel.Enabled = w_cbCheckResults.Checked;
				if (CopyingNavigationStructure || CopyingPortalListings)
				{
					w_cbCorrectLinks.Enabled = false;
					w_cbCorrectLinks.Checked = true;
				}
				else
				{
					w_cbCorrectLinks.Enabled = true;
				}
				w_cbTextFieldLinkCorrection.Enabled = w_cbCorrectLinks.Checked;
				w_cbComprehensiveLinkCorrection.Enabled = w_cbCorrectLinks.Checked && !CopyingPortalListings;
				w_cbComprehensiveLinkCorrection.Checked = w_cbComprehensiveLinkCorrection.Checked || CopyingPortalListings;
				w_plLinkCorrectionScope.Enabled = w_cbCorrectLinks.Checked;
				switch (base.Scope)
				{
				case SharePointObjectScope.List:
					if (BCSHelper.HasExternalListsOnly(SourceNodes))
					{
						DisplayLinkCorrectionOption = false;
						HideControl(w_cbOverrideCheckouts);
					}
					break;
				case SharePointObjectScope.Item:
					if (w_plComprehensiveLinkCorrection.Visible)
					{
						HideControl(w_plComprehensiveLinkCorrection);
					}
					break;
				default:
					if (w_plLinkCorrectionScope.Visible)
					{
						HideControl(w_plLinkCorrectionScope);
					}
					if (w_plComprehensiveLinkCorrection.Visible)
					{
						HideControl(w_plComprehensiveLinkCorrection);
					}
					break;
				case SharePointObjectScope.SiteCollection:
				case SharePointObjectScope.Site:
				case SharePointObjectScope.Folder:
					break;
				}
			}
			else
			{
				DisplayLinkCorrectionOption = false;
				DisplayCheckResults = false;
				DisplayOverrideCheckouts = false;
			}
			w_btnConfigureEmailSettings.Enabled = w_cbSendEmail.Checked && w_cbSendEmail.Enabled;
		}

		private void w_cbSendEmail_CheckedChanged(object sender, EventArgs e)
		{
			UpdateEnabledState();
		}
	}
}
