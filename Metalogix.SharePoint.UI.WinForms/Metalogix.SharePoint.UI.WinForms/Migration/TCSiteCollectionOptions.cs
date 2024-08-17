using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using Metalogix.Actions.Properties;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Properties;
using Metalogix.SharePoint.UI.WinForms.Administration;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Utilities;
using TooltipsTest;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Migration.SiteCollectionOptions.png")]
	[ControlName("Site Collection Options")]
	public class TCSiteCollectionOptions : SiteCollectionOptionsScopableTabbableControl
	{
		private SPWebApplication m_selectedWebApplication;

		private SPLanguage m_selectedLanguage;

		private string m_sQuotaId;

		private long m_lQuotaMax;

		private long m_lQuotaWarning;

		private SPSiteCollectionOptions m_options;

		private SPWebApplicationCollection m_webApps;

		private string m_sSourceUrl = "";

		private string m_sWebAppName;

		private string m_sTemplateName;

		private bool m_bCopyPageLayouts = true;

		private bool m_bCopyMasterPages = true;

		private bool m_bCopyOtherResources = true;

		private bool m_bCorrectMasterPageLinks;

		private Dictionary<string, int> m_experienceLableToVersionMap = new Dictionary<string, int>();

		private IContainer components;

		private LabelControl w_lblSelWebApp;

		private TextEdit w_tbURL;

		private LabelControl w_lblUrl;

		private LabelControl label1;

		private LabelControl w_lblOwner;

		private TextEdit w_tbOwner;

		private LabelControl w_lblWebApp;

		private TextEdit w_tbSecOwner;

		private LabelControl w_lblSecOwner;

		private LabelControl w_lblTemplate;

		private LabelControl lContentDatabase;

		private CheckEdit w_cbCopyMasterPageGallery;

		private CheckEdit w_cbCopyListTemplateGallery;

		private SimpleButton w_btnMPGalleryOptions;

		private CheckEdit w_cbSetQuota;

		private SimpleButton w_btnSiteQuota;

		private CheckEdit w_cbCopyAuditSettings;

		private LabelControl w_lbExperienceVersion;

		private PanelControl w_plExperienceVersion;

		private HelpTipButton w_helpCopyMasterPageGallery;

		private HelpTipButton w_helpCopyAuditSettings;

		private ComboBoxEdit w_cmbContentDatabase;

		private ComboBoxEdit w_cmbWebApp;

		private ComboBoxEdit w_cmbExperienceVersion;

		private ComboBoxEdit w_cmbLang;

		private ComboBoxEdit w_cmbTemplateName;

		private ComboBoxEdit w_cmbPath;

		private SpinEdit w_numResourceQuota;

		private LabelControl lblUnitMB;

		private SpinEdit w_numStorageQuota;

		private LabelControl w_lblResourceQuota;

		private LabelControl w_lblStorageQuota;

		private SimpleButton w_btnCopyAdmins;

		private CheckEdit w_cbCopySiteAdmins;

		private CheckEdit cbHostHeader;

		private HelpTipButton helpLinkHostHeader;

		public SPWebApplicationCollection AvailableWebApplications
		{
			get
			{
				return m_webApps;
			}
			set
			{
				m_webApps = value;
				if (value != null)
				{
					UpdateWebAppUI();
				}
			}
		}

		public SPSiteCollectionOptions Options
		{
			get
			{
				return m_options;
			}
			set
			{
				m_options = value;
				if (value != null)
				{
					LoadUI();
				}
			}
		}

		public string SourceUrl
		{
			get
			{
				return m_sSourceUrl;
			}
			set
			{
				m_sSourceUrl = value;
				if (m_sSourceUrl != null)
				{
					w_tbURL.Text = m_sSourceUrl;
				}
			}
		}

		public TCSiteCollectionOptions()
		{
			InitializeComponent();
			Type type = GetType();
			w_helpCopyMasterPageGallery.SetResourceString(type.FullName + w_cbCopyMasterPageGallery.Name, type);
			w_helpCopyAuditSettings.SetResourceString(type.FullName + w_cbCopyAuditSettings.Name, type);
			helpLinkHostHeader.SetResourceString(type.FullName + helpLinkHostHeader.Name, type);
		}

		private void cbHostHeader_CheckedChanged(object sender, EventArgs e)
		{
			UpdateSiteURLUI();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void FireTemplatesChanged()
		{
			SendMessage("AvailableTemplatesChanged", base.TargetWebTemplates);
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.TCSiteCollectionOptions));
			this.w_lblSelWebApp = new DevExpress.XtraEditors.LabelControl();
			this.w_tbURL = new DevExpress.XtraEditors.TextEdit();
			this.w_lblUrl = new DevExpress.XtraEditors.LabelControl();
			this.label1 = new DevExpress.XtraEditors.LabelControl();
			this.w_lblOwner = new DevExpress.XtraEditors.LabelControl();
			this.w_tbOwner = new DevExpress.XtraEditors.TextEdit();
			this.w_lblWebApp = new DevExpress.XtraEditors.LabelControl();
			this.w_tbSecOwner = new DevExpress.XtraEditors.TextEdit();
			this.w_lblSecOwner = new DevExpress.XtraEditors.LabelControl();
			this.w_lblTemplate = new DevExpress.XtraEditors.LabelControl();
			this.lContentDatabase = new DevExpress.XtraEditors.LabelControl();
			this.w_cbCopyMasterPageGallery = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbCopyListTemplateGallery = new DevExpress.XtraEditors.CheckEdit();
			this.w_btnMPGalleryOptions = new DevExpress.XtraEditors.SimpleButton();
			this.w_cbSetQuota = new DevExpress.XtraEditors.CheckEdit();
			this.w_btnSiteQuota = new DevExpress.XtraEditors.SimpleButton();
			this.w_cbCopyAuditSettings = new DevExpress.XtraEditors.CheckEdit();
			this.w_lbExperienceVersion = new DevExpress.XtraEditors.LabelControl();
			this.w_plExperienceVersion = new DevExpress.XtraEditors.PanelControl();
			this.w_cmbExperienceVersion = new DevExpress.XtraEditors.ComboBoxEdit();
			this.w_helpCopyMasterPageGallery = new TooltipsTest.HelpTipButton();
			this.w_helpCopyAuditSettings = new TooltipsTest.HelpTipButton();
			this.w_cmbContentDatabase = new DevExpress.XtraEditors.ComboBoxEdit();
			this.w_cmbWebApp = new DevExpress.XtraEditors.ComboBoxEdit();
			this.w_cmbLang = new DevExpress.XtraEditors.ComboBoxEdit();
			this.w_cmbTemplateName = new DevExpress.XtraEditors.ComboBoxEdit();
			this.w_cmbPath = new DevExpress.XtraEditors.ComboBoxEdit();
			this.w_numResourceQuota = new DevExpress.XtraEditors.SpinEdit();
			this.lblUnitMB = new DevExpress.XtraEditors.LabelControl();
			this.w_numStorageQuota = new DevExpress.XtraEditors.SpinEdit();
			this.w_lblResourceQuota = new DevExpress.XtraEditors.LabelControl();
			this.w_lblStorageQuota = new DevExpress.XtraEditors.LabelControl();
			this.w_btnCopyAdmins = new DevExpress.XtraEditors.SimpleButton();
			this.w_cbCopySiteAdmins = new DevExpress.XtraEditors.CheckEdit();
			this.cbHostHeader = new DevExpress.XtraEditors.CheckEdit();
			this.helpLinkHostHeader = new TooltipsTest.HelpTipButton();
			((System.ComponentModel.ISupportInitialize)this.w_tbURL.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_tbOwner.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_tbSecOwner.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyMasterPageGallery.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyListTemplateGallery.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbSetQuota.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyAuditSettings.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_plExperienceVersion).BeginInit();
			this.w_plExperienceVersion.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_cmbExperienceVersion.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_helpCopyMasterPageGallery).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_helpCopyAuditSettings).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cmbContentDatabase.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cmbWebApp.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cmbLang.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cmbTemplateName.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cmbPath.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_numResourceQuota.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_numStorageQuota.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopySiteAdmins.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.cbHostHeader.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.helpLinkHostHeader).BeginInit();
			base.SuspendLayout();
			resources.ApplyResources(this.w_lblSelWebApp, "w_lblSelWebApp");
			this.w_lblSelWebApp.Name = "w_lblSelWebApp";
			resources.ApplyResources(this.w_tbURL, "w_tbURL");
			this.w_tbURL.Name = "w_tbURL";
			resources.ApplyResources(this.w_lblUrl, "w_lblUrl");
			this.w_lblUrl.Name = "w_lblUrl";
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			resources.ApplyResources(this.w_lblOwner, "w_lblOwner");
			this.w_lblOwner.Name = "w_lblOwner";
			resources.ApplyResources(this.w_tbOwner, "w_tbOwner");
			this.w_tbOwner.Name = "w_tbOwner";
			resources.ApplyResources(this.w_lblWebApp, "w_lblWebApp");
			this.w_lblWebApp.Name = "w_lblWebApp";
			resources.ApplyResources(this.w_tbSecOwner, "w_tbSecOwner");
			this.w_tbSecOwner.Name = "w_tbSecOwner";
			resources.ApplyResources(this.w_lblSecOwner, "w_lblSecOwner");
			this.w_lblSecOwner.Name = "w_lblSecOwner";
			resources.ApplyResources(this.w_lblTemplate, "w_lblTemplate");
			this.w_lblTemplate.Name = "w_lblTemplate";
			resources.ApplyResources(this.lContentDatabase, "lContentDatabase");
			this.lContentDatabase.Name = "lContentDatabase";
			resources.ApplyResources(this.w_cbCopyMasterPageGallery, "w_cbCopyMasterPageGallery");
			this.w_cbCopyMasterPageGallery.Name = "w_cbCopyMasterPageGallery";
			this.w_cbCopyMasterPageGallery.Properties.AutoWidth = true;
			this.w_cbCopyMasterPageGallery.Properties.Caption = resources.GetString("w_cbCopyMasterPageGallery.Properties.Caption");
			this.w_cbCopyMasterPageGallery.CheckedChanged += new System.EventHandler(On_CopyMPGallery_Checked);
			resources.ApplyResources(this.w_cbCopyListTemplateGallery, "w_cbCopyListTemplateGallery");
			this.w_cbCopyListTemplateGallery.Name = "w_cbCopyListTemplateGallery";
			this.w_cbCopyListTemplateGallery.Properties.AutoWidth = true;
			this.w_cbCopyListTemplateGallery.Properties.Caption = resources.GetString("w_cbCopyListTemplateGallery.Properties.Caption");
			resources.ApplyResources(this.w_btnMPGalleryOptions, "w_btnMPGalleryOptions");
			this.w_btnMPGalleryOptions.Name = "w_btnMPGalleryOptions";
			this.w_btnMPGalleryOptions.Click += new System.EventHandler(On_MPGalleryOptions_Clicked);
			resources.ApplyResources(this.w_cbSetQuota, "w_cbSetQuota");
			this.w_cbSetQuota.Name = "w_cbSetQuota";
			this.w_cbSetQuota.Properties.AutoWidth = true;
			this.w_cbSetQuota.Properties.Caption = resources.GetString("w_cbSetQuota.Properties.Caption");
			this.w_cbSetQuota.CheckedChanged += new System.EventHandler(On_SetSiteQuote_Checked);
			resources.ApplyResources(this.w_btnSiteQuota, "w_btnSiteQuota");
			this.w_btnSiteQuota.Name = "w_btnSiteQuota";
			this.w_btnSiteQuota.Click += new System.EventHandler(On_SetQuota_Clicked);
			resources.ApplyResources(this.w_cbCopyAuditSettings, "w_cbCopyAuditSettings");
			this.w_cbCopyAuditSettings.Name = "w_cbCopyAuditSettings";
			this.w_cbCopyAuditSettings.Properties.AutoWidth = true;
			this.w_cbCopyAuditSettings.Properties.Caption = resources.GetString("w_cbCopyAuditSettings.Properties.Caption");
			resources.ApplyResources(this.w_lbExperienceVersion, "w_lbExperienceVersion");
			this.w_lbExperienceVersion.Name = "w_lbExperienceVersion";
			resources.ApplyResources(this.w_plExperienceVersion, "w_plExperienceVersion");
			this.w_plExperienceVersion.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plExperienceVersion.Controls.Add(this.w_cmbExperienceVersion);
			this.w_plExperienceVersion.Controls.Add(this.w_lbExperienceVersion);
			this.w_plExperienceVersion.Name = "w_plExperienceVersion";
			resources.ApplyResources(this.w_cmbExperienceVersion, "w_cmbExperienceVersion");
			this.w_cmbExperienceVersion.Name = "w_cmbExperienceVersion";
			DevExpress.XtraEditors.Controls.EditorButtonCollection buttons = this.w_cmbExperienceVersion.Properties.Buttons;
			DevExpress.XtraEditors.Controls.EditorButton[] buttons2 = new DevExpress.XtraEditors.Controls.EditorButton[1]
			{
				new DevExpress.XtraEditors.Controls.EditorButton()
			};
			buttons.AddRange(buttons2);
			this.w_cmbExperienceVersion.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.w_cmbExperienceVersion.SelectedIndexChanged += new System.EventHandler(w_cmbExperienceVersion_SelectedIndexChanged);
			this.w_helpCopyMasterPageGallery.AnchoringControl = this.w_btnMPGalleryOptions;
			this.w_helpCopyMasterPageGallery.BackColor = System.Drawing.Color.Transparent;
			this.w_helpCopyMasterPageGallery.CommonParentControl = null;
			resources.ApplyResources(this.w_helpCopyMasterPageGallery, "w_helpCopyMasterPageGallery");
			this.w_helpCopyMasterPageGallery.Name = "w_helpCopyMasterPageGallery";
			this.w_helpCopyMasterPageGallery.TabStop = false;
			this.w_helpCopyAuditSettings.AnchoringControl = this.w_cbCopyAuditSettings;
			this.w_helpCopyAuditSettings.BackColor = System.Drawing.Color.Transparent;
			this.w_helpCopyAuditSettings.CommonParentControl = null;
			resources.ApplyResources(this.w_helpCopyAuditSettings, "w_helpCopyAuditSettings");
			this.w_helpCopyAuditSettings.Name = "w_helpCopyAuditSettings";
			this.w_helpCopyAuditSettings.TabStop = false;
			resources.ApplyResources(this.w_cmbContentDatabase, "w_cmbContentDatabase");
			this.w_cmbContentDatabase.Name = "w_cmbContentDatabase";
			DevExpress.XtraEditors.Controls.EditorButtonCollection buttons3 = this.w_cmbContentDatabase.Properties.Buttons;
			DevExpress.XtraEditors.Controls.EditorButton[] buttons4 = new DevExpress.XtraEditors.Controls.EditorButton[1]
			{
				new DevExpress.XtraEditors.Controls.EditorButton()
			};
			buttons3.AddRange(buttons4);
			this.w_cmbContentDatabase.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			resources.ApplyResources(this.w_cmbWebApp, "w_cmbWebApp");
			this.w_cmbWebApp.Name = "w_cmbWebApp";
			DevExpress.XtraEditors.Controls.EditorButtonCollection buttons5 = this.w_cmbWebApp.Properties.Buttons;
			DevExpress.XtraEditors.Controls.EditorButton[] buttons6 = new DevExpress.XtraEditors.Controls.EditorButton[1]
			{
				new DevExpress.XtraEditors.Controls.EditorButton()
			};
			buttons5.AddRange(buttons6);
			this.w_cmbWebApp.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.w_cmbWebApp.SelectedIndexChanged += new System.EventHandler(On_WebApp_Changed);
			resources.ApplyResources(this.w_cmbLang, "w_cmbLang");
			this.w_cmbLang.Name = "w_cmbLang";
			DevExpress.XtraEditors.Controls.EditorButtonCollection buttons7 = this.w_cmbLang.Properties.Buttons;
			DevExpress.XtraEditors.Controls.EditorButton[] buttons8 = new DevExpress.XtraEditors.Controls.EditorButton[1]
			{
				new DevExpress.XtraEditors.Controls.EditorButton()
			};
			buttons7.AddRange(buttons8);
			this.w_cmbLang.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.w_cmbLang.SelectedIndexChanged += new System.EventHandler(On_Language_Changed);
			this.w_cmbLang.Enabled = false;
			resources.ApplyResources(this.w_cmbTemplateName, "w_cmbTemplateName");
			this.w_cmbTemplateName.Name = "w_cmbTemplateName";
			DevExpress.XtraEditors.Controls.EditorButtonCollection buttons9 = this.w_cmbTemplateName.Properties.Buttons;
			DevExpress.XtraEditors.Controls.EditorButton[] buttons10 = new DevExpress.XtraEditors.Controls.EditorButton[1]
			{
				new DevExpress.XtraEditors.Controls.EditorButton()
			};
			buttons9.AddRange(buttons10);
			this.w_cmbTemplateName.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.w_cmbTemplateName.SelectedIndexChanged += new System.EventHandler(On_WebTemplate_Changed);
			resources.ApplyResources(this.w_cmbPath, "w_cmbPath");
			this.w_cmbPath.Name = "w_cmbPath";
			DevExpress.XtraEditors.Controls.EditorButtonCollection buttons11 = this.w_cmbPath.Properties.Buttons;
			DevExpress.XtraEditors.Controls.EditorButton[] buttons12 = new DevExpress.XtraEditors.Controls.EditorButton[1]
			{
				new DevExpress.XtraEditors.Controls.EditorButton()
			};
			buttons11.AddRange(buttons12);
			this.w_cmbPath.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.w_cmbPath.SelectedIndexChanged += new System.EventHandler(On_Path_Changed);
			resources.ApplyResources(this.w_numResourceQuota, "w_numResourceQuota");
			this.w_numResourceQuota.Name = "w_numResourceQuota";
			this.w_numResourceQuota.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
			{
				new DevExpress.XtraEditors.Controls.EditorButton()
			});
			DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties = this.w_numResourceQuota.Properties;
			int[] bits = new int[4] { 10, 0, 0, 0 };
			properties.Increment = new decimal(bits);
			this.w_numResourceQuota.Properties.IsFloatValue = false;
			this.w_numResourceQuota.Properties.Mask.EditMask = resources.GetString("w_numResourceQuota.Properties.Mask.EditMask");
			DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties2 = this.w_numResourceQuota.Properties;
			int[] bits2 = new int[4] { 10000, 0, 0, 0 };
			properties2.MaxValue = new decimal(bits2);
			resources.ApplyResources(this.lblUnitMB, "lblUnitMB");
			this.lblUnitMB.Name = "lblUnitMB";
			resources.ApplyResources(this.w_numStorageQuota, "w_numStorageQuota");
			this.w_numStorageQuota.Name = "w_numStorageQuota";
			this.w_numStorageQuota.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
			{
				new DevExpress.XtraEditors.Controls.EditorButton()
			});
			DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties3 = this.w_numStorageQuota.Properties;
			int[] bits3 = new int[4] { 10, 0, 0, 0 };
			properties3.Increment = new decimal(bits3);
			this.w_numStorageQuota.Properties.IsFloatValue = false;
			this.w_numStorageQuota.Properties.Mask.EditMask = resources.GetString("w_numStorageQuota.Properties.Mask.EditMask");
			DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties4 = this.w_numStorageQuota.Properties;
			int[] bits4 = new int[4] { 1000000, 0, 0, 0 };
			properties4.MaxValue = new decimal(bits4);
			DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties5 = this.w_numStorageQuota.Properties;
			int[] bits5 = new int[4] { 110, 0, 0, 0 };
			properties5.MinValue = new decimal(bits5);
			resources.ApplyResources(this.w_lblResourceQuota, "w_lblResourceQuota");
			this.w_lblResourceQuota.Name = "w_lblResourceQuota";
			resources.ApplyResources(this.w_lblStorageQuota, "w_lblStorageQuota");
			this.w_lblStorageQuota.Name = "w_lblStorageQuota";
			resources.ApplyResources(this.w_btnCopyAdmins, "w_btnCopyAdmins");
			this.w_btnCopyAdmins.Name = "w_btnCopyAdmins";
			this.w_btnCopyAdmins.Click += new System.EventHandler(w_btnCopyAdmins_Click);
			resources.ApplyResources(this.w_cbCopySiteAdmins, "w_cbCopySiteAdmins");
			this.w_cbCopySiteAdmins.Name = "w_cbCopySiteAdmins";
			this.w_cbCopySiteAdmins.Properties.AutoWidth = true;
			this.w_cbCopySiteAdmins.Properties.Caption = resources.GetString("w_cbCopySiteAdmins.Properties.Caption");
			this.w_cbCopySiteAdmins.CheckedChanged += new System.EventHandler(w_cbCopySiteAdmins_CheckedChanged);
			resources.ApplyResources(this.cbHostHeader, "cbHostHeader");
			this.cbHostHeader.Name = "cbHostHeader";
			this.cbHostHeader.Properties.Caption = resources.GetString("cbHostHeader.Properties.Caption");
			this.cbHostHeader.CheckedChanged += new System.EventHandler(cbHostHeader_CheckedChanged);
			this.helpLinkHostHeader.AnchoringControl = null;
			this.helpLinkHostHeader.BackColor = System.Drawing.Color.Transparent;
			this.helpLinkHostHeader.CommonParentControl = null;
			resources.ApplyResources(this.helpLinkHostHeader, "helpLinkHostHeader");
			this.helpLinkHostHeader.Name = "helpLinkHostHeader";
			this.helpLinkHostHeader.RealOffset = null;
			this.helpLinkHostHeader.RelativeOffset = null;
			this.helpLinkHostHeader.TabStop = false;
			base.Controls.Add(this.helpLinkHostHeader);
			base.Controls.Add(this.cbHostHeader);
			base.Controls.Add(this.w_btnCopyAdmins);
			base.Controls.Add(this.w_cbCopySiteAdmins);
			base.Controls.Add(this.w_numResourceQuota);
			base.Controls.Add(this.lblUnitMB);
			base.Controls.Add(this.w_numStorageQuota);
			base.Controls.Add(this.w_lblResourceQuota);
			base.Controls.Add(this.w_lblStorageQuota);
			base.Controls.Add(this.w_cmbTemplateName);
			base.Controls.Add(this.w_cmbLang);
			base.Controls.Add(this.w_cmbWebApp);
			base.Controls.Add(this.w_cmbPath);
			base.Controls.Add(this.w_cmbContentDatabase);
			base.Controls.Add(this.w_helpCopyAuditSettings);
			base.Controls.Add(this.w_helpCopyMasterPageGallery);
			base.Controls.Add(this.w_plExperienceVersion);
			base.Controls.Add(this.w_cbCopyAuditSettings);
			base.Controls.Add(this.w_btnSiteQuota);
			base.Controls.Add(this.w_cbSetQuota);
			base.Controls.Add(this.w_btnMPGalleryOptions);
			base.Controls.Add(this.w_cbCopyMasterPageGallery);
			base.Controls.Add(this.w_cbCopyListTemplateGallery);
			base.Controls.Add(this.lContentDatabase);
			base.Controls.Add(this.w_lblTemplate);
			base.Controls.Add(this.w_lblSelWebApp);
			base.Controls.Add(this.w_tbURL);
			base.Controls.Add(this.w_lblUrl);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.w_lblOwner);
			base.Controls.Add(this.w_tbOwner);
			base.Controls.Add(this.w_lblWebApp);
			base.Controls.Add(this.w_tbSecOwner);
			base.Controls.Add(this.w_lblSecOwner);
			base.Name = "TCSiteCollectionOptions";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)this.w_tbURL.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_tbOwner.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_tbSecOwner.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyMasterPageGallery.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyListTemplateGallery.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbSetQuota.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopyAuditSettings.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_plExperienceVersion).EndInit();
			this.w_plExperienceVersion.ResumeLayout(false);
			this.w_plExperienceVersion.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.w_cmbExperienceVersion.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_helpCopyMasterPageGallery).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_helpCopyAuditSettings).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cmbContentDatabase.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cmbWebApp.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cmbLang.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cmbTemplateName.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cmbPath.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_numResourceQuota.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_numStorageQuota.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopySiteAdmins.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.cbHostHeader.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.helpLinkHostHeader).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		public bool IsTargetSameAsSource()
		{
			if (!AdapterConfigurationVariables.AllowDuplicateSiteCollection && SourceNodes[0] is SPWeb sPWeb)
			{
				string value = $"{w_lblSelWebApp.Text.Trim()}{m_options.URL.Trim()}";
				if (sPWeb.Url.Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					FlatXtraMessageBox.Show(Metalogix.Actions.Properties.Resources.CannotRunTargetIsSameAsSource);
					return true;
				}
			}
			return false;
		}

		private bool IsValidateUI()
		{
			if (string.IsNullOrEmpty(w_tbURL.Text) && (((SPPath)w_cmbPath.SelectedItem).IsWildcard || cbHostHeader.Checked))
			{
				FlatXtraMessageBox.Show(Metalogix.SharePoint.Properties.Resources.MsgSiteCollectionUrl, Metalogix.SharePoint.Properties.Resources.WarningString, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return true;
			}
			if (!Utils.IsValidSharePointURL(w_tbURL.Text, cbHostHeader.Checked))
			{
				FlatXtraMessageBox.Show(Metalogix.Actions.Properties.Resources.ValidSiteCollectionURL + " " + (cbHostHeader.Checked ? Utils.illegalCharactersForHostHeaderSiteUrl : Utils.IllegalCharactersForSiteUrl), Metalogix.SharePoint.Properties.Resources.WarningString, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return true;
			}
			if (string.IsNullOrEmpty(w_tbOwner.Text))
			{
				FlatXtraMessageBox.Show(Metalogix.SharePoint.Properties.Resources.MsgSiteCollectionOwner, Metalogix.SharePoint.Properties.Resources.WarningString, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return true;
			}
			if (m_options.IsSameServer && ((SPWebApplication)w_cmbWebApp.SelectedItem).Name == m_options.SourceWebApplication)
			{
				string text = w_cmbPath.Text.Trim('/');
				string sourcePath = m_options.SourcePath;
				char[] trimChars = new char[1] { '/' };
				if (text == sourcePath.Trim(trimChars))
				{
					if ((w_cmbPath.SelectedItem as SPPath).IsWildcard)
					{
						string text2 = Utils.JoinUrl(w_cmbPath.Text, w_tbURL.Text);
						char[] trimChars2 = new char[1] { '/' };
						if (text2.Trim(trimChars2) != m_options.SourceUrlName.Trim('/'))
						{
							return false;
						}
					}
					FlatXtraMessageBox.Show(Metalogix.SharePoint.Properties.Resources.MsgInvalidSiteCollectionSelection, Metalogix.SharePoint.Properties.Resources.WarningString, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return true;
				}
			}
			return false;
		}

		protected override void LoadUI()
		{
			w_tbURL.Text = string.Empty;
			ComboBoxEdit comboBoxEdit = w_cmbWebApp;
			bool enabled = Options.WebApplicationName == null || (!Options.SelfServiceCreateMode && base.Target != null && base.Target.ShowAllSites);
			comboBoxEdit.Enabled = enabled;
			w_cmbContentDatabase.Enabled = !Options.SelfServiceCreateMode;
			w_cbSetQuota.Enabled = !Options.SelfServiceCreateMode;
			w_btnSiteQuota.Enabled = !Options.SelfServiceCreateMode;
			cbHostHeader.Enabled = !Options.SelfServiceCreateMode;
			if (!string.IsNullOrEmpty(m_options.OwnerLogin))
			{
				w_tbOwner.Text = m_options.OwnerLogin;
				if (base.Target is SPTenant || base.SourceAdapter.SharePointVersion.IsSharePointOnline)
				{
					w_tbOwner.Text = string.Empty;
				}
			}
			if (!string.IsNullOrEmpty(m_options.SecondaryOwnerLogin))
			{
				w_tbSecOwner.Text = m_options.SecondaryOwnerLogin;
			}
			w_cbCopyMasterPageGallery.Checked = Options.CopyMasterPageGallery;
			w_cbCopyListTemplateGallery.Checked = Options.CopyListTemplateGallery;
			if (base.Target != null && base.SourceAdapter.SharePointVersion.MajorVersion != base.Target.Adapter.SharePointVersion.MajorVersion)
			{
				w_cbCopyListTemplateGallery.Checked = false;
				w_cbCopyListTemplateGallery.Enabled = false;
			}
			m_bCopyMasterPages = Options.CopyMasterPages;
			m_bCopyPageLayouts = Options.CopyPageLayouts;
			m_bCopyOtherResources = Options.CopyOtherResources;
			m_bCorrectMasterPageLinks = Options.CorrectMasterPageLinks;
			if (!(base.Target is SPTenant))
			{
				HideControl(w_lblStorageQuota);
				HideControl(w_numStorageQuota);
				HideControl(lblUnitMB);
			}
			else
			{
				HideControl(w_cmbContentDatabase);
				HideControl(lContentDatabase);
				HideControl(w_lblSecOwner);
				HideControl(w_tbSecOwner);
				HideControl(cbHostHeader);
				w_lblOwner.Text = "Owner:";
				w_cbSetQuota.Visible = false;
				w_btnSiteQuota.Visible = false;
				w_lblStorageQuota.Visible = true;
				w_numStorageQuota.Visible = true;
				lblUnitMB.Visible = true;
				w_lblResourceQuota.Visible = true;
				w_numResourceQuota.Visible = true;
				CheckEdit checkEdit = w_cbCopyAuditSettings;
				int num = w_cbCopyAuditSettings.Location.X;
				checkEdit.Location = new Point(num, w_cbSetQuota.Location.Y);
				HelpTipButton helpTipButton = w_helpCopyAuditSettings;
				int num2 = w_helpCopyAuditSettings.Location.X;
				helpTipButton.Location = new Point(num2, w_cbCopyAuditSettings.Location.Y);
			}
			if (base.Target == null || base.Target.Languages.Count == 0 || !base.Target.Languages[0].HasMultipleExperienceVersions || base.Target.Adapter.SharePointVersion.IsSharePoint2016)
			{
				HideControl(w_plExperienceVersion);
			}
			else
			{
				int num3 = -1;
				object obj = null;
				m_experienceLableToVersionMap.Clear();
				foreach (int experienceVersion in base.Target.Languages[0].ExperienceVersions)
				{
					string experinceVersionLabel = GetExperinceVersionLabel(experienceVersion);
					w_cmbExperienceVersion.Properties.Items.Add(experinceVersionLabel);
					m_experienceLableToVersionMap.Add(experinceVersionLabel, experienceVersion);
					if (experienceVersion > num3)
					{
						obj = experinceVersionLabel;
						num3 = experienceVersion;
					}
				}
				if (base.Target.Languages[0].ExperienceVersions.Contains(Options.ExperienceVersion))
				{
					w_cmbExperienceVersion.SelectedItem = GetExperinceVersionLabel(Options.ExperienceVersion);
				}
				else if (obj != null)
				{
					w_cmbExperienceVersion.SelectedItem = obj;
				}
			}
			m_sWebAppName = m_options.WebApplicationName;
			if (w_cmbWebApp.Properties.Items.Count > 0)
			{
				foreach (object item in w_cmbWebApp.Properties.Items)
				{
					SPWebApplication sPWebApplication = item as SPWebApplication;
					if (!(sPWebApplication != null) || !(sPWebApplication.Name == m_sWebAppName))
					{
						continue;
					}
					w_cmbWebApp.SelectedItem = item;
					break;
				}
			}
			string languageCode = m_options.LanguageCode;
			if (w_cmbLang.Properties.Items.Count > 0)
			{
				foreach (object item2 in w_cmbLang.Properties.Items)
				{
					if (!(item2 is SPLanguage sPLanguage) || !(sPLanguage.LCID.ToString() == languageCode))
					{
						continue;
					}
					w_cmbLang.SelectedItem = item2;
					m_selectedLanguage = sPLanguage;
					UpdateAvaliableTemplates();
					FireTemplatesChanged();
					break;
				}
			}
			m_sTemplateName = m_options.WebTemplateName;
			if (w_cmbTemplateName.Properties.Items.Count > 0)
			{
				foreach (object item3 in w_cmbTemplateName.Properties.Items)
				{
					if (!(item3 is SPWebTemplate sPWebTemplate) || !(sPWebTemplate.Name == m_sTemplateName))
					{
						continue;
					}
					w_cmbTemplateName.SelectedItem = item3;
					break;
				}
			}
			foreach (object item4 in w_cmbPath.Properties.Items)
			{
				if (item4.ToString() != m_options.Path)
				{
					continue;
				}
				w_cmbPath.SelectedItem = item4;
				break;
			}
			w_cbCopySiteAdmins.Checked = m_options.CopySiteAdmins;
			w_cbSetQuota.Checked = m_options.CopySiteQuota;
			m_lQuotaWarning = m_options.QuotaWarning;
			m_lQuotaMax = m_options.QuotaMaximum;
			m_sQuotaId = m_options.QuotaID;
			if (base.Target is SPTenant)
			{
				w_numStorageQuota.Value = m_options.StorageQuota;
				w_numResourceQuota.Value = Convert.ToDecimal(m_options.ResourceQuota);
			}
			UpdateQuotaEnabled();
			UpdateSiteAdminsEnabled();
			w_cbCopyAuditSettings.Checked = m_options.CopyAuditSettings;
			if (!(base.Target is SPTenant))
			{
				cbHostHeader.CheckedChanged -= cbHostHeader_CheckedChanged;
				cbHostHeader.Checked = !m_options.SelfServiceCreateMode && m_options.IsHostHeader;
				cbHostHeader.CheckedChanged += cbHostHeader_CheckedChanged;
				UpdateSiteURLUI(isUILoading: true);
			}
			SetPathURL();
			if (string.IsNullOrEmpty(m_options.ContentDatabaseName))
			{
				w_cmbContentDatabase.SelectedItem = w_cmbContentDatabase.Properties.Items[0];
				return;
			}
			foreach (object item5 in w_cmbContentDatabase.Properties.Items)
			{
				if (item5.ToString() != m_options.ContentDatabaseName)
				{
					continue;
				}
				w_cmbContentDatabase.SelectedItem = item5;
				break;
			}
		}

		private void On_CopyMPGallery_Checked(object sender, EventArgs e)
		{
			w_btnMPGalleryOptions.Enabled = w_cbCopyMasterPageGallery.Checked;
		}

		private void On_Language_Changed(object sender, EventArgs e)
		{
			if (w_cmbLang.SelectedItem is SPLanguage)
			{
				m_selectedLanguage = (SPLanguage)w_cmbLang.SelectedItem;
				UpdateAvaliableTemplates();
				FireTemplatesChanged();
			}
		}

		private void On_MPGalleryOptions_Clicked(object sender, EventArgs e)
		{
			CopyMasterPageGalleryOptionsDialog copyMasterPageGalleryOptionsDialog = new CopyMasterPageGalleryOptionsDialog
			{
				CopyMasterPages = m_bCopyMasterPages,
				CopyOtherResources = m_bCopyOtherResources,
				CopyPageLayouts = m_bCopyPageLayouts,
				CorrectMasterPageLinks = m_bCorrectMasterPageLinks
			};
			copyMasterPageGalleryOptionsDialog.ShowDialog();
			if (copyMasterPageGalleryOptionsDialog.DialogResult == DialogResult.OK)
			{
				m_bCopyMasterPages = copyMasterPageGalleryOptionsDialog.CopyMasterPages;
				m_bCopyOtherResources = copyMasterPageGalleryOptionsDialog.CopyOtherResources;
				m_bCopyPageLayouts = copyMasterPageGalleryOptionsDialog.CopyPageLayouts;
				m_bCorrectMasterPageLinks = copyMasterPageGalleryOptionsDialog.CorrectMasterPageLinks;
			}
		}

		private void On_Path_Changed(object sender, EventArgs e)
		{
			SetPathURL();
		}

		private void On_SetQuota_Clicked(object sender, EventArgs e)
		{
			SetSiteQuotaDialog setSiteQuotaDialog = new SetSiteQuotaDialog
			{
				SiteQuotaTemplates = base.Target.SiteQuotaTemplates,
				QuotaID = m_sQuotaId,
				SiteQuotaMaximum = m_lQuotaMax,
				SiteQuotaWarning = m_lQuotaWarning
			};
			if (setSiteQuotaDialog.ShowDialog() == DialogResult.OK)
			{
				m_sQuotaId = setSiteQuotaDialog.QuotaID;
				m_lQuotaMax = setSiteQuotaDialog.SiteQuotaMaximum;
				m_lQuotaWarning = setSiteQuotaDialog.SiteQuotaWarning;
			}
		}

		private void On_SetSiteQuote_Checked(object sender, EventArgs e)
		{
			UpdateQuotaEnabled();
		}

		private void On_WebApp_Changed(object sender, EventArgs e)
		{
			m_sWebAppName = ((SPWebApplication)w_cmbWebApp.SelectedItem).Name;
			if (w_cmbWebApp.SelectedItem is SPWebApplication)
			{
				m_selectedWebApplication = (SPWebApplication)w_cmbWebApp.SelectedItem;
				UpdateWebAppData();
			}
		}

		private void On_WebTemplate_Changed(object sender, EventArgs e)
		{
			if (w_cmbTemplateName.SelectedItem is SPWebTemplate sPWebTemplate)
			{
				m_sTemplateName = sPWebTemplate.Name;
			}
		}

		public override bool SaveUI()
		{
			if (!base.IsModeSwitched && IsValidateUI())
			{
				return false;
			}
			m_options.LanguageCode = m_selectedLanguage.LCID.ToString();
			m_options.Name = w_tbURL.Text;
			m_options.WebTemplateName = m_sTemplateName;
			if (cbHostHeader.Checked)
			{
				Options.HostHeaderURL = w_lblSelWebApp.Text + w_tbURL.Text;
				m_options.URL = (Uri.IsWellFormedUriString(Options.HostHeaderURL, UriKind.Absolute) ? new Uri(Options.HostHeaderURL).LocalPath : string.Empty);
			}
			else
			{
				m_options.Path = w_cmbPath.Text;
				m_options.URL = w_cmbPath.Text + w_tbURL.Text;
			}
			Options.IsHostHeader = cbHostHeader.Checked;
			m_options.OwnerLogin = w_tbOwner.Text;
			m_options.SecondaryOwnerLogin = w_tbSecOwner.Text;
			m_sWebAppName = ((SPWebApplication)w_cmbWebApp.SelectedItem).Name;
			m_options.WebApplicationName = m_sWebAppName;
			m_options.CopyMasterPageGallery = w_cbCopyMasterPageGallery.Checked;
			m_options.CopyListTemplateGallery = w_cbCopyListTemplateGallery.Checked;
			m_options.CopyMasterPages = m_bCopyMasterPages;
			m_options.CopyPageLayouts = m_bCopyPageLayouts;
			m_options.CopyOtherResources = m_bCopyOtherResources;
			m_options.CorrectMasterPageLinks = m_bCorrectMasterPageLinks;
			m_options.CopyAuditSettings = w_cbCopyAuditSettings.Checked;
			m_options.CopySiteAdmins = w_cbCopySiteAdmins.Checked;
			m_options.CopySiteQuota = w_cbSetQuota.Checked;
			m_options.QuotaID = m_sQuotaId;
			m_options.QuotaMaximum = m_lQuotaMax;
			m_options.QuotaWarning = m_lQuotaWarning;
			if (base.Target is SPTenant)
			{
				m_options.StorageQuota = Convert.ToInt64(w_numStorageQuota.Value);
				m_options.ResourceQuota = Convert.ToDouble(w_numResourceQuota.Value);
			}
			SPSiteCollectionOptions options = m_options;
			string contentDatabaseName = ((!(w_cmbContentDatabase.Text == Metalogix.SharePoint.Properties.Resources.Auto_Detect)) ? w_cmbContentDatabase.Text : null);
			options.ContentDatabaseName = contentDatabaseName;
			string key = ((w_cmbExperienceVersion.SelectedItem == null) ? "" : w_cmbExperienceVersion.SelectedItem.ToString());
			if (!m_experienceLableToVersionMap.TryGetValue(key, out var value))
			{
				m_options.ExperienceVersion = -1;
			}
			else
			{
				m_options.ExperienceVersion = value;
			}
			return true;
		}

		private void SetPathURL()
		{
			if (w_cmbPath.SelectedItem is SPPath sPPath)
			{
				w_tbURL.Enabled = cbHostHeader.Checked || sPPath.IsWildcard;
				if (!sPPath.IsWildcard && !cbHostHeader.Checked)
				{
					w_tbURL.Text = string.Empty;
				}
				else if (string.IsNullOrEmpty(w_tbURL.Text) && Options != null)
				{
					w_tbURL.Text = ((Options.Name != null) ? Options.Name : SourceUrl);
				}
			}
		}

		private void UpdateAvaliableTemplates()
		{
			string key = ((w_cmbExperienceVersion.SelectedItem == null) ? "" : w_cmbExperienceVersion.SelectedItem.ToString());
			if (m_experienceLableToVersionMap.Count <= 1 || !m_experienceLableToVersionMap.ContainsKey(key))
			{
				base.TargetWebTemplates = m_selectedLanguage.Templates;
			}
			else
			{
				base.TargetWebTemplates = m_selectedLanguage.GetTemplatesForExperienceVersion(m_experienceLableToVersionMap[key]);
			}
		}

		private void UpdateQuotaEnabled()
		{
			w_btnSiteQuota.Enabled = w_cbSetQuota.Checked && w_cbSetQuota.Enabled;
		}

		protected override string UpdateServerData()
		{
			string text = "";
			w_cmbLang.Properties.Items.Clear();
			w_cmbWebApp.Properties.Items.Clear();
			if (base.Target != null)
			{
				if (base.Target.WebApplications.Count <= 0)
				{
					text += "Could not enumerate target web applications \n";
				}
				else
				{
					foreach (SPWebApplication webApplication in base.Target.WebApplications)
					{
						w_cmbWebApp.Properties.Items.Add(webApplication);
					}
					if (w_cmbWebApp.Properties.Items.Count > 0)
					{
						w_cmbWebApp.SelectedIndex = 0;
					}
				}
				if (base.Target.Languages.Count <= 0)
				{
					text += "Could not enumerate target language packs \n";
				}
				else
				{
					foreach (SPLanguage language in base.Target.Languages)
					{
						w_cmbLang.Properties.Items.Add(language);
					}
					if (w_cmbLang.Properties.Items.Count > 0)
					{
						w_cmbLang.SelectedIndex = 0;
					}
				}
			}
			return text;
		}

		private void UpdateSiteAdminsEnabled()
		{
			w_btnCopyAdmins.Enabled = w_cbCopySiteAdmins.Checked && w_cbCopySiteAdmins.Enabled;
		}

		private void UpdateSiteURLUI(bool isUILoading = false)
		{
			if (!cbHostHeader.Checked)
			{
				w_cmbPath.Visible = true;
				UpdateWebAppData(isUILoading);
			}
			else if (w_cmbWebApp.SelectedItem != null)
			{
				char c = ':';
				string[] array = w_lblSelWebApp.Text.Split(c);
				w_lblSelWebApp.Text = array[0] + "://";
				w_cmbPath.Visible = false;
				TextEdit textEdit = w_tbURL;
				Point location = w_lblSelWebApp.Location;
				Point location2 = w_tbURL.Location;
				textEdit.Location = new Point(location.X + 38, location2.Y);
				TextEdit textEdit2 = w_tbURL;
				textEdit2.Width = base.ClientRectangle.Right - 8 - w_tbURL.Left;
				w_tbURL.Enabled = true;
			}
		}

		protected override void UpdateTemplateUI()
		{
			SuspendLayout();
			bool flag = false;
			w_cmbTemplateName.Properties.Items.Clear();
			IEnumerable<SPWebTemplate> source = base.TargetWebTemplates.Where(CreateSiteCollectionControl.RestrictedTemplates.Compile());
			if (base.Target.Adapter.SharePointVersion.IsSharePoint2013OrLater)
			{
				source = source.Where(CreateSiteCollectionControl.RestrictedTemplatesInSp2013.Compile());
			}
			source.OrderBy((SPWebTemplate template) => template.Title).ToList().ForEach(delegate(SPWebTemplate template)
			{
				w_cmbTemplateName.Properties.Items.Add(template);
				if (template.Name == m_sTemplateName)
				{
					w_cmbTemplateName.SelectedItem = template;
					flag = true;
				}
			});
			if (!flag)
			{
				int num = w_cmbTemplateName.FindString("Blank Site");
				w_cmbTemplateName.SelectedIndex = ((num != -1) ? num : 0);
			}
			ResumeLayout();
		}

		private void UpdateUI()
		{
			UpdateServerData();
		}

		private void UpdateWebAppData(bool isUILoading = false)
		{
			SuspendLayout();
			w_lblSelWebApp.Text = m_selectedWebApplication.Url;
			w_cmbPath.Properties.Items.Clear();
			m_selectedWebApplication.Paths.ForEach(delegate(SPPath path)
			{
				w_cmbPath.Properties.Items.Add(path);
			});
			w_cmbPath.SetIdealComboBoxEditSize();
			w_cmbPath.Left = w_lblSelWebApp.Right + 6;
			w_tbURL.Left = w_cmbPath.Right + 6;
			TextEdit textEdit = w_tbURL;
			textEdit.Width = base.ClientRectangle.Right - 8 - w_tbURL.Left;
			SPPath sPPath = m_selectedWebApplication.Paths.FirstOrDefault((SPPath path) => path.IsWildcard);
			if (!isUILoading)
			{
				if (sPPath == null)
				{
					w_cmbPath.SelectedIndex = 0;
				}
				else
				{
					w_cmbPath.SelectedItem = sPPath;
				}
			}
			w_cmbContentDatabase.Properties.Items.Clear();
			w_cmbContentDatabase.Properties.Items.Add(Metalogix.SharePoint.Properties.Resources.Auto_Detect);
			foreach (SPContentDatabase contentDatabase in m_selectedWebApplication.ContentDatabases)
			{
				w_cmbContentDatabase.Properties.Items.Add(contentDatabase.Name);
			}
			if (w_cmbContentDatabase.Properties.Items.Count >= 1)
			{
				w_cmbContentDatabase.SelectedIndex = 0;
			}
			if (cbHostHeader.Checked)
			{
				UpdateSiteURLUI();
			}
			ResumeLayout();
		}

		private void UpdateWebAppUI()
		{
			bool flag = false;
			w_cmbWebApp.Properties.Items.Clear();
			foreach (SPWebApplication availableWebApplication in AvailableWebApplications)
			{
				if (availableWebApplication.ContentDatabases.Count > 0)
				{
					w_cmbWebApp.Properties.Items.Add(availableWebApplication);
					if (!(availableWebApplication.Name != m_sWebAppName))
					{
						w_cmbWebApp.SelectedItem = availableWebApplication;
						flag = true;
					}
				}
			}
			if (!flag && w_cmbWebApp.Properties.Items.Count > 0)
			{
				w_cmbWebApp.SelectedIndex = 0;
			}
			if (w_cmbWebApp.Properties.Items.Count <= 0)
			{
				throw new Exception(Metalogix.SharePoint.Properties.Resources.MsgNoContentDatabasesInWebApps);
			}
		}

		private void w_btnCopyAdmins_Click(object sender, EventArgs e)
		{
			CopySiteAdminsDialog copySiteAdminsDialog = new CopySiteAdminsDialog();
			copySiteAdminsDialog.w_tbSiteCollectionAdmins.Text = m_options.SiteCollectionAdministrators;
			if (copySiteAdminsDialog.ShowDialog() == DialogResult.OK)
			{
				m_options.SiteCollectionAdministrators = copySiteAdminsDialog.w_tbSiteCollectionAdmins.Text;
			}
		}

		private void w_cbCopySiteAdmins_CheckedChanged(object sender, EventArgs e)
		{
			UpdateSiteAdminsEnabled();
		}

		private void w_cmbExperienceVersion_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateAvaliableTemplates();
		}
	}
}
