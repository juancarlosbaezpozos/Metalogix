using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTab;
using Metalogix.Actions.Properties;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Administration;
using Metalogix.SharePoint.Properties;
using Metalogix.SharePoint.UI.WinForms.Administration;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
    public class CreateSiteCollectionControl : CollapsableControl
    {
        private static readonly string[] RestrictedSiteCollectionTemplates;

        private static readonly string[] RestrictedSiteCollectionTemplates2013Only;

        public static readonly Expression<Func<SPWebTemplate, bool>> RestrictedTemplates;

        public static readonly Expression<Func<SPWebTemplate, bool>> RestrictedTemplatesInSp2013;

        private SPBaseServer m_targetServer;

        private SPWebApplication m_selectedWebApplication;

        private SPLanguage m_selectedLanguage;

        private CreateSiteCollectionOptions m_settings;

        private bool m_bPrimaryIsValidated;

        private string m_sPrimaryInternal;

        private bool m_bSecondaryIsValidated;

        private string m_sSecondaryInternal;

        private bool m_bSuspendUserFieldEvents;

        private Dictionary<string, int> m_experienceLableToVersionMap = new Dictionary<string, int>();

        private IContainer components;

        private LabelControl w_lblSiteTitle;

        private LabelControl w_lblDescription;

        private LabelControl w_lblURL;

        private LabelControl w_lblLang;

        private LabelControl w_lblTemplate;

        private LabelControl w_lblWebApp;

        private LabelControl w_lblOwner;

        private LabelControl w_lblSecOwner;

        private ComboBoxEdit w_cmbWebApp;

        private TextEdit w_tbTitle;

        private MemoEdit w_tbDescription;

        private LabelControl w_lblSelWebApp;

        private ComboBoxEdit w_cmbPath;

        private TextEdit w_tbURL;

        private ComboBoxEdit w_cmbLang;

        private TextEdit w_tbOwner;

        private TextEdit w_tbSecOwner;

        private PanelControl w_gbMainProps;

        private PanelControl groupBox1;

        private PanelControl groupBox2;

        private PanelControl groupBox3;

        private ComboBoxEdit w_cmbTemplate;

        private SimpleButton w_btnCheckSecondaryAdmin;

        private SimpleButton w_btnCheckPrimaryAdmin;

        private PanelControl groupBox4;

        private ComboBoxEdit w_cmbContentDatabase;

        private LabelControl label2;

        private LabelControl w_lbExperienceVersion;

        private ComboBoxEdit w_cmbExperienceVersion;

        private PanelControl w_plExperienceVersion;

        private XtraTabPage TenantQuotaPage;

        private PanelControl w_gbTenantSiteQuota;

        private PanelControl w_plTenantQuotaOption;

        private SpinEdit w_numResourceQuota;

        private LabelControl lblUnitMB;

        private SpinEdit w_numStorageQuota;

        private LabelControl w_lblResourceQuota;

        private LabelControl w_lblStorageQuota;

        private XtraTabPage SiteQuotaPage;

        private PanelControl w_gbSiteQuota;

        private PanelControl w_plQuotaOptions;

        private LabelControl label3;

        private SpinEdit w_numMax;

        private LabelControl label1;

        private SpinEdit w_numWarning;

        private LabelControl label4;

        private LabelControl w_lblMax;

        internal CheckEdit w_rbToIndividual;

        private ComboBoxEdit w_cmbSiteQuotaTemplates;

        internal CheckEdit w_rbSetQuota;

        private CheckEdit w_cbSetSiteQuota;

        private global::DevExpress.XtraTab.XtraTabControl SiteQuotaTab;

        private CheckEdit cbHostHeader;

        public CreateSiteCollectionOptions Options
        {
            get
            {
                return m_settings;
            }
            set
            {
                if (value != null)
                {
                    try
                    {
                        m_settings = value;
                        LoadUI();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to fetch necessary server information for this operation. This may be because SharePoint Shared Services are not configured correctly in SharePoint Central Administration. \n\n" + ex.Message);
                    }
                }
            }
        }

        public SPBaseServer Target
        {
            get
            {
                return m_targetServer;
            }
            set
            {
                if (value != null)
                {
                    m_targetServer = value;
                    string text = UpdateServerData();
                    if (text.Length > 0)
                    {
                        throw new Exception(text);
                    }
                }
            }
        }

        public event CollectionChangeEventHandler LanguageChanged;

        static CreateSiteCollectionControl()
        {
            string[] restrictedSiteCollectionTemplates = new string[5] { "GLOBAL#0", "CENTRALADMIN#0", "APP#0", "OFFILE#0", "ACCSRV#0" };
            RestrictedSiteCollectionTemplates = restrictedSiteCollectionTemplates;
            RestrictedSiteCollectionTemplates2013Only = new string[1] { "SPSREPORTCENTER#0" };
            RestrictedTemplates = (SPWebTemplate template) => !template.IsSubWebOnly && RestrictedSiteCollectionTemplates.All((string item) => template.Name != item);
            RestrictedTemplatesInSp2013 = (SPWebTemplate template) => RestrictedSiteCollectionTemplates2013Only.All((string item) => template.Name != item);
        }

        public CreateSiteCollectionControl()
        {
            InitializeComponent();
            int num = w_btnCheckPrimaryAdmin.Location.X;
            int num2 = num - (w_tbOwner.Location.X + w_tbOwner.Width) + w_btnCheckPrimaryAdmin.Width;
            w_btnCheckPrimaryAdmin.Visible = false;
            w_btnCheckSecondaryAdmin.Visible = false;
            w_tbOwner.Width += num2;
            w_tbSecOwner.Width += num2;
        }

        private void cbHostHeader_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSiteURLUI();
        }

        private bool CheckPrimary()
        {
            if (!m_bPrimaryIsValidated)
            {
                m_bSuspendUserFieldEvents = true;
                m_sPrimaryInternal = w_tbOwner.Text;
                string text = ValidateUser(m_sPrimaryInternal);
                if (text != null)
                {
                    w_tbOwner.Text = text;
                    w_tbOwner.Font = new Font(w_tbOwner.Font, FontStyle.Underline);
                    m_bPrimaryIsValidated = true;
                }
                else
                {
                    m_bPrimaryIsValidated = false;
                    w_tbOwner.ForeColor = Color.Red;
                }
                m_bSuspendUserFieldEvents = false;
            }
            return m_bPrimaryIsValidated;
        }

        private bool CheckSecondary()
        {
            if (!m_bSecondaryIsValidated)
            {
                m_bSuspendUserFieldEvents = true;
                m_sSecondaryInternal = w_tbSecOwner.Text;
                string text = ValidateUser(m_sSecondaryInternal);
                if (text != null)
                {
                    w_tbSecOwner.Text = text;
                    w_tbSecOwner.Font = new Font(w_tbSecOwner.Font, FontStyle.Underline);
                    m_bSecondaryIsValidated = true;
                }
                else
                {
                    m_bSecondaryIsValidated = false;
                    w_tbSecOwner.ForeColor = Color.Red;
                }
                m_bSuspendUserFieldEvents = false;
            }
            return m_bSecondaryIsValidated;
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
            if (this.LanguageChanged != null)
            {
                this.LanguageChanged(this, new CollectionChangeEventArgs(CollectionChangeAction.Refresh, w_cmbLang.SelectedItem));
            }
        }

        private string GetExperinceVersionLabel(int iValue)
        {
            if (Target == null)
            {
                return iValue.ToString();
            }
            if (Target.Adapter.SharePointVersion.IsSharePoint2013OrLater)
            {
                switch (iValue)
                {
                    case 15:
                        return "2013";
                    case 14:
                        return "2010";
                }
            }
            return iValue.ToString();
        }

        private void InitializeComponent()
        {
            global::System.ComponentModel.ComponentResourceManager resources = new global::System.ComponentModel.ComponentResourceManager(typeof(global::Metalogix.SharePoint.UI.WinForms.Administration.CreateSiteCollectionControl));
            this.w_lblSiteTitle = new global::DevExpress.XtraEditors.LabelControl();
            this.w_lblDescription = new global::DevExpress.XtraEditors.LabelControl();
            this.w_lblURL = new global::DevExpress.XtraEditors.LabelControl();
            this.w_lblLang = new global::DevExpress.XtraEditors.LabelControl();
            this.w_lblTemplate = new global::DevExpress.XtraEditors.LabelControl();
            this.w_lblWebApp = new global::DevExpress.XtraEditors.LabelControl();
            this.w_lblOwner = new global::DevExpress.XtraEditors.LabelControl();
            this.w_lblSecOwner = new global::DevExpress.XtraEditors.LabelControl();
            this.w_cmbWebApp = new global::DevExpress.XtraEditors.ComboBoxEdit();
            this.w_tbTitle = new global::DevExpress.XtraEditors.TextEdit();
            this.w_tbDescription = new global::DevExpress.XtraEditors.MemoEdit();
            this.w_lblSelWebApp = new global::DevExpress.XtraEditors.LabelControl();
            this.w_cmbPath = new global::DevExpress.XtraEditors.ComboBoxEdit();
            this.w_tbURL = new global::DevExpress.XtraEditors.TextEdit();
            this.w_cmbLang = new global::DevExpress.XtraEditors.ComboBoxEdit();
            this.w_tbOwner = new global::DevExpress.XtraEditors.TextEdit();
            this.w_tbSecOwner = new global::DevExpress.XtraEditors.TextEdit();
            this.w_gbMainProps = new global::DevExpress.XtraEditors.PanelControl();
            this.groupBox1 = new global::DevExpress.XtraEditors.PanelControl();
            this.w_btnCheckSecondaryAdmin = new global::DevExpress.XtraEditors.SimpleButton();
            this.w_btnCheckPrimaryAdmin = new global::DevExpress.XtraEditors.SimpleButton();
            this.groupBox2 = new global::DevExpress.XtraEditors.PanelControl();
            this.w_plExperienceVersion = new global::DevExpress.XtraEditors.PanelControl();
            this.w_lbExperienceVersion = new global::DevExpress.XtraEditors.LabelControl();
            this.w_cmbExperienceVersion = new global::DevExpress.XtraEditors.ComboBoxEdit();
            this.w_cmbTemplate = new global::DevExpress.XtraEditors.ComboBoxEdit();
            this.groupBox3 = new global::DevExpress.XtraEditors.PanelControl();
            this.cbHostHeader = new global::DevExpress.XtraEditors.CheckEdit();
            this.groupBox4 = new global::DevExpress.XtraEditors.PanelControl();
            this.w_cmbContentDatabase = new global::DevExpress.XtraEditors.ComboBoxEdit();
            this.label2 = new global::DevExpress.XtraEditors.LabelControl();
            this.TenantQuotaPage = new global::DevExpress.XtraTab.XtraTabPage();
            this.w_gbTenantSiteQuota = new global::DevExpress.XtraEditors.PanelControl();
            this.w_plTenantQuotaOption = new global::DevExpress.XtraEditors.PanelControl();
            this.w_numResourceQuota = new global::DevExpress.XtraEditors.SpinEdit();
            this.lblUnitMB = new global::DevExpress.XtraEditors.LabelControl();
            this.w_numStorageQuota = new global::DevExpress.XtraEditors.SpinEdit();
            this.w_lblResourceQuota = new global::DevExpress.XtraEditors.LabelControl();
            this.w_lblStorageQuota = new global::DevExpress.XtraEditors.LabelControl();
            this.SiteQuotaPage = new global::DevExpress.XtraTab.XtraTabPage();
            this.w_gbSiteQuota = new global::DevExpress.XtraEditors.PanelControl();
            this.w_plQuotaOptions = new global::DevExpress.XtraEditors.PanelControl();
            this.label3 = new global::DevExpress.XtraEditors.LabelControl();
            this.w_numMax = new global::DevExpress.XtraEditors.SpinEdit();
            this.label1 = new global::DevExpress.XtraEditors.LabelControl();
            this.w_numWarning = new global::DevExpress.XtraEditors.SpinEdit();
            this.label4 = new global::DevExpress.XtraEditors.LabelControl();
            this.w_lblMax = new global::DevExpress.XtraEditors.LabelControl();
            this.w_rbToIndividual = new global::DevExpress.XtraEditors.CheckEdit();
            this.w_cmbSiteQuotaTemplates = new global::DevExpress.XtraEditors.ComboBoxEdit();
            this.w_rbSetQuota = new global::DevExpress.XtraEditors.CheckEdit();
            this.w_cbSetSiteQuota = new global::DevExpress.XtraEditors.CheckEdit();
            this.SiteQuotaTab = new global::DevExpress.XtraTab.XtraTabControl();
            ((global::System.ComponentModel.ISupportInitialize)this.w_cmbWebApp.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_tbTitle.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_tbDescription.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_cmbPath.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_tbURL.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_cmbLang.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_tbOwner.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_tbSecOwner.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_gbMainProps).BeginInit();
            this.w_gbMainProps.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)this.groupBox1).BeginInit();
            this.groupBox1.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)this.groupBox2).BeginInit();
            this.groupBox2.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)this.w_plExperienceVersion).BeginInit();
            this.w_plExperienceVersion.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)this.w_cmbExperienceVersion.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_cmbTemplate.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.groupBox3).BeginInit();
            this.groupBox3.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)this.cbHostHeader.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.groupBox4).BeginInit();
            this.groupBox4.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)this.w_cmbContentDatabase.Properties).BeginInit();
            this.TenantQuotaPage.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)this.w_gbTenantSiteQuota).BeginInit();
            this.w_gbTenantSiteQuota.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)this.w_plTenantQuotaOption).BeginInit();
            this.w_plTenantQuotaOption.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)this.w_numResourceQuota.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_numStorageQuota.Properties).BeginInit();
            this.SiteQuotaPage.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)this.w_gbSiteQuota).BeginInit();
            this.w_gbSiteQuota.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)this.w_plQuotaOptions).BeginInit();
            this.w_plQuotaOptions.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)this.w_numMax.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_numWarning.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_rbToIndividual.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_cmbSiteQuotaTemplates.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_rbSetQuota.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_cbSetSiteQuota.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.SiteQuotaTab).BeginInit();
            this.SiteQuotaTab.SuspendLayout();
            base.SuspendLayout();
            resources.ApplyResources(this.w_lblSiteTitle, "w_lblSiteTitle");
            this.w_lblSiteTitle.Name = "w_lblSiteTitle";
            resources.ApplyResources(this.w_lblDescription, "w_lblDescription");
            this.w_lblDescription.Name = "w_lblDescription";
            resources.ApplyResources(this.w_lblURL, "w_lblURL");
            this.w_lblURL.Name = "w_lblURL";
            resources.ApplyResources(this.w_lblLang, "w_lblLang");
            this.w_lblLang.Name = "w_lblLang";
            resources.ApplyResources(this.w_lblTemplate, "w_lblTemplate");
            this.w_lblTemplate.Name = "w_lblTemplate";
            resources.ApplyResources(this.w_lblWebApp, "w_lblWebApp");
            this.w_lblWebApp.Name = "w_lblWebApp";
            resources.ApplyResources(this.w_lblOwner, "w_lblOwner");
            this.w_lblOwner.Name = "w_lblOwner";
            resources.ApplyResources(this.w_lblSecOwner, "w_lblSecOwner");
            this.w_lblSecOwner.Name = "w_lblSecOwner";
            resources.ApplyResources(this.w_cmbWebApp, "w_cmbWebApp");
            this.w_cmbWebApp.Name = "w_cmbWebApp";
            global::DevExpress.XtraEditors.Controls.EditorButtonCollection buttons = this.w_cmbWebApp.Properties.Buttons;
            global::DevExpress.XtraEditors.Controls.EditorButton[] buttons2 = new global::DevExpress.XtraEditors.Controls.EditorButton[1]
            {
            new global::DevExpress.XtraEditors.Controls.EditorButton()
            };
            buttons.AddRange(buttons2);
            this.w_cmbWebApp.Properties.TextEditStyle = global::DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.w_cmbWebApp.SelectedIndexChanged += new global::System.EventHandler(On_WebApp_Changed);
            resources.ApplyResources(this.w_tbTitle, "w_tbTitle");
            this.w_tbTitle.Name = "w_tbTitle";
            resources.ApplyResources(this.w_tbDescription, "w_tbDescription");
            this.w_tbDescription.Name = "w_tbDescription";
            resources.ApplyResources(this.w_lblSelWebApp, "w_lblSelWebApp");
            this.w_lblSelWebApp.Name = "w_lblSelWebApp";
            resources.ApplyResources(this.w_cmbPath, "w_cmbPath");
            this.w_cmbPath.Name = "w_cmbPath";
            global::DevExpress.XtraEditors.Controls.EditorButtonCollection buttons3 = this.w_cmbPath.Properties.Buttons;
            global::DevExpress.XtraEditors.Controls.EditorButton[] buttons4 = new global::DevExpress.XtraEditors.Controls.EditorButton[1]
            {
            new global::DevExpress.XtraEditors.Controls.EditorButton()
            };
            buttons3.AddRange(buttons4);
            this.w_cmbPath.Properties.TextEditStyle = global::DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.w_cmbPath.SelectedIndexChanged += new global::System.EventHandler(On_Path_Changed);
            resources.ApplyResources(this.w_tbURL, "w_tbURL");
            this.w_tbURL.Name = "w_tbURL";
            resources.ApplyResources(this.w_cmbLang, "w_cmbLang");
            this.w_cmbLang.Name = "w_cmbLang";
            global::DevExpress.XtraEditors.Controls.EditorButtonCollection buttons5 = this.w_cmbLang.Properties.Buttons;
            global::DevExpress.XtraEditors.Controls.EditorButton[] buttons6 = new global::DevExpress.XtraEditors.Controls.EditorButton[1]
            {
            new global::DevExpress.XtraEditors.Controls.EditorButton()
            };
            buttons5.AddRange(buttons6);
            this.w_cmbLang.Properties.TextEditStyle = global::DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.w_cmbLang.SelectedIndexChanged += new global::System.EventHandler(On_Language_Changed);
            resources.ApplyResources(this.w_tbOwner, "w_tbOwner");
            this.w_tbOwner.Name = "w_tbOwner";
            this.w_tbOwner.Properties.Appearance.ForeColor = (global::System.Drawing.Color)resources.GetObject("w_tbOwner.Properties.Appearance.ForeColor");
            this.w_tbOwner.Properties.Appearance.Options.UseForeColor = true;
            this.w_tbOwner.TextChanged += new global::System.EventHandler(w_tbOwner_TextChanged);
            resources.ApplyResources(this.w_tbSecOwner, "w_tbSecOwner");
            this.w_tbSecOwner.Name = "w_tbSecOwner";
            this.w_tbSecOwner.Properties.Appearance.ForeColor = (global::System.Drawing.Color)resources.GetObject("w_tbSecOwner.Properties.Appearance.ForeColor");
            this.w_tbSecOwner.Properties.Appearance.Options.UseForeColor = true;
            this.w_tbSecOwner.TextChanged += new global::System.EventHandler(w_tbSecOwner_TextChanged);
            this.w_gbMainProps.BorderStyle = global::DevExpress.XtraEditors.Controls.BorderStyles.Flat;
            this.w_gbMainProps.Controls.Add(this.w_lblSiteTitle);
            this.w_gbMainProps.Controls.Add(this.w_lblWebApp);
            this.w_gbMainProps.Controls.Add(this.w_tbTitle);
            this.w_gbMainProps.Controls.Add(this.w_cmbWebApp);
            this.w_gbMainProps.Controls.Add(this.w_lblDescription);
            this.w_gbMainProps.Controls.Add(this.w_tbDescription);
            resources.ApplyResources(this.w_gbMainProps, "w_gbMainProps");
            this.w_gbMainProps.Name = "w_gbMainProps";
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BorderStyle = global::DevExpress.XtraEditors.Controls.BorderStyles.Flat;
            this.groupBox1.Controls.Add(this.w_btnCheckSecondaryAdmin);
            this.groupBox1.Controls.Add(this.w_btnCheckPrimaryAdmin);
            this.groupBox1.Controls.Add(this.w_lblOwner);
            this.groupBox1.Controls.Add(this.w_tbOwner);
            this.groupBox1.Controls.Add(this.w_tbSecOwner);
            this.groupBox1.Controls.Add(this.w_lblSecOwner);
            this.groupBox1.Name = "groupBox1";
            resources.ApplyResources(this.w_btnCheckSecondaryAdmin, "w_btnCheckSecondaryAdmin");
            this.w_btnCheckSecondaryAdmin.Image = (global::System.Drawing.Image)resources.GetObject("w_btnCheckSecondaryAdmin.Image");
            this.w_btnCheckSecondaryAdmin.Name = "w_btnCheckSecondaryAdmin";
            this.w_btnCheckSecondaryAdmin.Click += new global::System.EventHandler(On_btnCheckSeconday_Click);
            resources.ApplyResources(this.w_btnCheckPrimaryAdmin, "w_btnCheckPrimaryAdmin");
            this.w_btnCheckPrimaryAdmin.Image = (global::System.Drawing.Image)resources.GetObject("w_btnCheckPrimaryAdmin.Image");
            this.w_btnCheckPrimaryAdmin.Name = "w_btnCheckPrimaryAdmin";
            this.w_btnCheckPrimaryAdmin.Click += new global::System.EventHandler(On_btnCheckPrimary_Click);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BorderStyle = global::DevExpress.XtraEditors.Controls.BorderStyles.Flat;
            this.groupBox2.Controls.Add(this.w_plExperienceVersion);
            this.groupBox2.Controls.Add(this.w_cmbTemplate);
            this.groupBox2.Controls.Add(this.w_lblLang);
            this.groupBox2.Controls.Add(this.w_lblTemplate);
            this.groupBox2.Controls.Add(this.w_cmbLang);
            this.groupBox2.Name = "groupBox2";
            resources.ApplyResources(this.w_plExperienceVersion, "w_plExperienceVersion");
            this.w_plExperienceVersion.BorderStyle = global::DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.w_plExperienceVersion.Controls.Add(this.w_lbExperienceVersion);
            this.w_plExperienceVersion.Controls.Add(this.w_cmbExperienceVersion);
            this.w_plExperienceVersion.Name = "w_plExperienceVersion";
            resources.ApplyResources(this.w_lbExperienceVersion, "w_lbExperienceVersion");
            this.w_lbExperienceVersion.Name = "w_lbExperienceVersion";
            resources.ApplyResources(this.w_cmbExperienceVersion, "w_cmbExperienceVersion");
            this.w_cmbExperienceVersion.Name = "w_cmbExperienceVersion";
            global::DevExpress.XtraEditors.Controls.EditorButtonCollection buttons7 = this.w_cmbExperienceVersion.Properties.Buttons;
            global::DevExpress.XtraEditors.Controls.EditorButton[] buttons8 = new global::DevExpress.XtraEditors.Controls.EditorButton[1]
            {
            new global::DevExpress.XtraEditors.Controls.EditorButton()
            };
            buttons7.AddRange(buttons8);
            this.w_cmbExperienceVersion.Properties.TextEditStyle = global::DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.w_cmbExperienceVersion.SelectedIndexChanged += new global::System.EventHandler(w_cmbExperienceVersion_SelectedIndexChanged);
            resources.ApplyResources(this.w_cmbTemplate, "w_cmbTemplate");
            this.w_cmbTemplate.Name = "w_cmbTemplate";
            global::DevExpress.XtraEditors.Controls.EditorButtonCollection buttons9 = this.w_cmbTemplate.Properties.Buttons;
            global::DevExpress.XtraEditors.Controls.EditorButton[] buttons10 = new global::DevExpress.XtraEditors.Controls.EditorButton[1]
            {
            new global::DevExpress.XtraEditors.Controls.EditorButton((global::DevExpress.XtraEditors.Controls.ButtonPredefines)resources.GetObject("w_cmbTemplate.Properties.Buttons"))
            };
            buttons9.AddRange(buttons10);
            this.w_cmbTemplate.Properties.TextEditStyle = global::DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.BorderStyle = global::DevExpress.XtraEditors.Controls.BorderStyles.Flat;
            this.groupBox3.Controls.Add(this.cbHostHeader);
            this.groupBox3.Controls.Add(this.w_cmbPath);
            this.groupBox3.Controls.Add(this.w_lblURL);
            this.groupBox3.Controls.Add(this.w_lblSelWebApp);
            this.groupBox3.Controls.Add(this.w_tbURL);
            this.groupBox3.Name = "groupBox3";
            resources.ApplyResources(this.cbHostHeader, "cbHostHeader");
            this.cbHostHeader.Name = "cbHostHeader";
            this.cbHostHeader.Properties.Caption = resources.GetString("cbHostHeader.Properties.Caption");
            this.cbHostHeader.CheckedChanged += new global::System.EventHandler(cbHostHeader_CheckedChanged);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.BorderStyle = global::DevExpress.XtraEditors.Controls.BorderStyles.Flat;
            this.groupBox4.Controls.Add(this.w_cmbContentDatabase);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Name = "groupBox4";
            resources.ApplyResources(this.w_cmbContentDatabase, "w_cmbContentDatabase");
            this.w_cmbContentDatabase.Name = "w_cmbContentDatabase";
            global::DevExpress.XtraEditors.Controls.EditorButtonCollection buttons11 = this.w_cmbContentDatabase.Properties.Buttons;
            global::DevExpress.XtraEditors.Controls.EditorButton[] buttons12 = new global::DevExpress.XtraEditors.Controls.EditorButton[1]
            {
            new global::DevExpress.XtraEditors.Controls.EditorButton()
            };
            buttons11.AddRange(buttons12);
            this.w_cmbContentDatabase.Properties.TextEditStyle = global::DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.TenantQuotaPage.Controls.Add(this.w_gbTenantSiteQuota);
            this.TenantQuotaPage.Name = "TenantQuotaPage";
            this.TenantQuotaPage.PageVisible = false;
            resources.ApplyResources(this.TenantQuotaPage, "TenantQuotaPage");
            this.w_gbTenantSiteQuota.BorderStyle = global::DevExpress.XtraEditors.Controls.BorderStyles.Flat;
            this.w_gbTenantSiteQuota.Controls.Add(this.w_plTenantQuotaOption);
            resources.ApplyResources(this.w_gbTenantSiteQuota, "w_gbTenantSiteQuota");
            this.w_gbTenantSiteQuota.Name = "w_gbTenantSiteQuota";
            resources.ApplyResources(this.w_plTenantQuotaOption, "w_plTenantQuotaOption");
            this.w_plTenantQuotaOption.Appearance.BackColor = (global::System.Drawing.Color)resources.GetObject("w_plTenantQuotaOption.Appearance.BackColor");
            this.w_plTenantQuotaOption.Appearance.Options.UseBackColor = true;
            this.w_plTenantQuotaOption.BorderStyle = global::DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.w_plTenantQuotaOption.Controls.Add(this.w_numResourceQuota);
            this.w_plTenantQuotaOption.Controls.Add(this.lblUnitMB);
            this.w_plTenantQuotaOption.Controls.Add(this.w_numStorageQuota);
            this.w_plTenantQuotaOption.Controls.Add(this.w_lblResourceQuota);
            this.w_plTenantQuotaOption.Controls.Add(this.w_lblStorageQuota);
            this.w_plTenantQuotaOption.Name = "w_plTenantQuotaOption";
            resources.ApplyResources(this.w_numResourceQuota, "w_numResourceQuota");
            this.w_numResourceQuota.Name = "w_numResourceQuota";
            this.w_numResourceQuota.Properties.Buttons.AddRange(new global::DevExpress.XtraEditors.Controls.EditorButton[1]
            {
            new global::DevExpress.XtraEditors.Controls.EditorButton()
            });
            global::DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties = this.w_numResourceQuota.Properties;
            int[] bits = new int[4] { 10, 0, 0, 0 };
            properties.Increment = new decimal(bits);
            this.w_numResourceQuota.Properties.IsFloatValue = false;
            this.w_numResourceQuota.Properties.Mask.EditMask = resources.GetString("w_numResourceQuota.Properties.Mask.EditMask");
            global::DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties2 = this.w_numResourceQuota.Properties;
            int[] bits2 = new int[4] { 10000, 0, 0, 0 };
            properties2.MaxValue = new decimal(bits2);
            resources.ApplyResources(this.lblUnitMB, "lblUnitMB");
            this.lblUnitMB.Name = "lblUnitMB";
            resources.ApplyResources(this.w_numStorageQuota, "w_numStorageQuota");
            this.w_numStorageQuota.Name = "w_numStorageQuota";
            this.w_numStorageQuota.Properties.Buttons.AddRange(new global::DevExpress.XtraEditors.Controls.EditorButton[1]
            {
            new global::DevExpress.XtraEditors.Controls.EditorButton()
            });
            global::DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties3 = this.w_numStorageQuota.Properties;
            int[] bits3 = new int[4] { 10, 0, 0, 0 };
            properties3.Increment = new decimal(bits3);
            this.w_numStorageQuota.Properties.IsFloatValue = false;
            this.w_numStorageQuota.Properties.Mask.EditMask = resources.GetString("w_numStorageQuota.Properties.Mask.EditMask");
            global::DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties4 = this.w_numStorageQuota.Properties;
            int[] bits4 = new int[4] { 1000000, 0, 0, 0 };
            properties4.MaxValue = new decimal(bits4);
            global::DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties5 = this.w_numStorageQuota.Properties;
            int[] bits5 = new int[4] { 110, 0, 0, 0 };
            properties5.MinValue = new decimal(bits5);
            resources.ApplyResources(this.w_lblResourceQuota, "w_lblResourceQuota");
            this.w_lblResourceQuota.Name = "w_lblResourceQuota";
            resources.ApplyResources(this.w_lblStorageQuota, "w_lblStorageQuota");
            this.w_lblStorageQuota.Name = "w_lblStorageQuota";
            this.SiteQuotaPage.Controls.Add(this.w_gbSiteQuota);
            this.SiteQuotaPage.Name = "SiteQuotaPage";
            resources.ApplyResources(this.SiteQuotaPage, "SiteQuotaPage");
            resources.ApplyResources(this.w_gbSiteQuota, "w_gbSiteQuota");
            this.w_gbSiteQuota.BorderStyle = global::DevExpress.XtraEditors.Controls.BorderStyles.Flat;
            this.w_gbSiteQuota.Controls.Add(this.w_plQuotaOptions);
            this.w_gbSiteQuota.Controls.Add(this.w_cbSetSiteQuota);
            this.w_gbSiteQuota.Name = "w_gbSiteQuota";
            resources.ApplyResources(this.w_plQuotaOptions, "w_plQuotaOptions");
            this.w_plQuotaOptions.Appearance.BackColor = (global::System.Drawing.Color)resources.GetObject("w_plQuotaOptions.Appearance.BackColor");
            this.w_plQuotaOptions.Appearance.Options.UseBackColor = true;
            this.w_plQuotaOptions.BorderStyle = global::DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.w_plQuotaOptions.Controls.Add(this.label3);
            this.w_plQuotaOptions.Controls.Add(this.w_numMax);
            this.w_plQuotaOptions.Controls.Add(this.label1);
            this.w_plQuotaOptions.Controls.Add(this.w_numWarning);
            this.w_plQuotaOptions.Controls.Add(this.label4);
            this.w_plQuotaOptions.Controls.Add(this.w_lblMax);
            this.w_plQuotaOptions.Controls.Add(this.w_rbToIndividual);
            this.w_plQuotaOptions.Controls.Add(this.w_cmbSiteQuotaTemplates);
            this.w_plQuotaOptions.Controls.Add(this.w_rbSetQuota);
            this.w_plQuotaOptions.Name = "w_plQuotaOptions";
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            resources.ApplyResources(this.w_numMax, "w_numMax");
            this.w_numMax.Name = "w_numMax";
            this.w_numMax.Properties.Buttons.AddRange(new global::DevExpress.XtraEditors.Controls.EditorButton[1]
            {
            new global::DevExpress.XtraEditors.Controls.EditorButton()
            });
            global::DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties6 = this.w_numMax.Properties;
            int[] bits6 = new int[4] { 10, 0, 0, 0 };
            properties6.Increment = new decimal(bits6);
            this.w_numMax.Properties.IsFloatValue = false;
            this.w_numMax.Properties.Mask.EditMask = resources.GetString("w_numMax.Properties.Mask.EditMask");
            global::DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties7 = this.w_numMax.Properties;
            int[] bits7 = new int[4] { 10000, 0, 0, 0 };
            properties7.MaxValue = new decimal(bits7);
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            resources.ApplyResources(this.w_numWarning, "w_numWarning");
            this.w_numWarning.Name = "w_numWarning";
            this.w_numWarning.Properties.Buttons.AddRange(new global::DevExpress.XtraEditors.Controls.EditorButton[1]
            {
            new global::DevExpress.XtraEditors.Controls.EditorButton()
            });
            global::DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties8 = this.w_numWarning.Properties;
            int[] bits8 = new int[4] { 10, 0, 0, 0 };
            properties8.Increment = new decimal(bits8);
            this.w_numWarning.Properties.IsFloatValue = false;
            this.w_numWarning.Properties.Mask.EditMask = resources.GetString("w_numWarning.Properties.Mask.EditMask");
            global::DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties9 = this.w_numWarning.Properties;
            int[] bits9 = new int[4] { 100000, 0, 0, 0 };
            properties9.MaxValue = new decimal(bits9);
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            resources.ApplyResources(this.w_lblMax, "w_lblMax");
            this.w_lblMax.Name = "w_lblMax";
            resources.ApplyResources(this.w_rbToIndividual, "w_rbToIndividual");
            this.w_rbToIndividual.Name = "w_rbToIndividual";
            this.w_rbToIndividual.Properties.Caption = resources.GetString("w_rbToIndividual.Properties.Caption");
            this.w_rbToIndividual.Properties.CheckStyle = global::DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.w_rbToIndividual.Properties.RadioGroupIndex = 1;
            this.w_rbToIndividual.CheckedChanged += new global::System.EventHandler(On_SiteQuota_CheckedChanged);
            resources.ApplyResources(this.w_cmbSiteQuotaTemplates, "w_cmbSiteQuotaTemplates");
            this.w_cmbSiteQuotaTemplates.Name = "w_cmbSiteQuotaTemplates";
            global::DevExpress.XtraEditors.Controls.EditorButtonCollection buttons13 = this.w_cmbSiteQuotaTemplates.Properties.Buttons;
            global::DevExpress.XtraEditors.Controls.EditorButton[] buttons14 = new global::DevExpress.XtraEditors.Controls.EditorButton[1]
            {
            new global::DevExpress.XtraEditors.Controls.EditorButton()
            };
            buttons13.AddRange(buttons14);
            this.w_cmbSiteQuotaTemplates.Properties.TextEditStyle = global::DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            resources.ApplyResources(this.w_rbSetQuota, "w_rbSetQuota");
            this.w_rbSetQuota.Name = "w_rbSetQuota";
            this.w_rbSetQuota.Properties.Caption = resources.GetString("w_rbSetQuota.Properties.Caption");
            this.w_rbSetQuota.Properties.CheckStyle = global::DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.w_rbSetQuota.Properties.RadioGroupIndex = 1;
            this.w_rbSetQuota.TabStop = false;
            this.w_rbSetQuota.CheckedChanged += new global::System.EventHandler(On_SiteQuota_CheckedChanged);
            resources.ApplyResources(this.w_cbSetSiteQuota, "w_cbSetSiteQuota");
            this.w_cbSetSiteQuota.Name = "w_cbSetSiteQuota";
            this.w_cbSetSiteQuota.Properties.Caption = resources.GetString("w_cbSetSiteQuota.Properties.Caption");
            this.w_cbSetSiteQuota.CheckedChanged += new global::System.EventHandler(On_SetQuota_CheckedChanged);
            resources.ApplyResources(this.SiteQuotaTab, "SiteQuotaTab");
            this.SiteQuotaTab.LookAndFeel.Style = global::DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
            this.SiteQuotaTab.LookAndFeel.UseDefaultLookAndFeel = false;
            this.SiteQuotaTab.Name = "SiteQuotaTab";
            this.SiteQuotaTab.SelectedTabPage = this.SiteQuotaPage;
            global::DevExpress.XtraTab.XtraTabPageCollection tabPages = this.SiteQuotaTab.TabPages;
            global::DevExpress.XtraTab.XtraTabPage[] pages = new global::DevExpress.XtraTab.XtraTabPage[2] { this.SiteQuotaPage, this.TenantQuotaPage };
            tabPages.AddRange(pages);
            resources.ApplyResources(this, "$this");
            base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(this.SiteQuotaTab);
            base.Controls.Add(this.groupBox4);
            base.Controls.Add(this.groupBox3);
            base.Controls.Add(this.groupBox2);
            base.Controls.Add(this.groupBox1);
            base.Controls.Add(this.w_gbMainProps);
            base.Name = "CreateSiteCollectionControl";
            base.SizeChanged += new global::System.EventHandler(SPCreateSiteCollectionControl_SizeChanged);
            ((global::System.ComponentModel.ISupportInitialize)this.w_cmbWebApp.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_tbTitle.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_tbDescription.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_cmbPath.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_tbURL.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_cmbLang.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_tbOwner.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_tbSecOwner.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_gbMainProps).EndInit();
            this.w_gbMainProps.ResumeLayout(false);
            this.w_gbMainProps.PerformLayout();
            ((global::System.ComponentModel.ISupportInitialize)this.groupBox1).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((global::System.ComponentModel.ISupportInitialize)this.groupBox2).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((global::System.ComponentModel.ISupportInitialize)this.w_plExperienceVersion).EndInit();
            this.w_plExperienceVersion.ResumeLayout(false);
            this.w_plExperienceVersion.PerformLayout();
            ((global::System.ComponentModel.ISupportInitialize)this.w_cmbExperienceVersion.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_cmbTemplate.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.groupBox3).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((global::System.ComponentModel.ISupportInitialize)this.cbHostHeader.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.groupBox4).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((global::System.ComponentModel.ISupportInitialize)this.w_cmbContentDatabase.Properties).EndInit();
            this.TenantQuotaPage.ResumeLayout(false);
            ((global::System.ComponentModel.ISupportInitialize)this.w_gbTenantSiteQuota).EndInit();
            this.w_gbTenantSiteQuota.ResumeLayout(false);
            ((global::System.ComponentModel.ISupportInitialize)this.w_plTenantQuotaOption).EndInit();
            this.w_plTenantQuotaOption.ResumeLayout(false);
            this.w_plTenantQuotaOption.PerformLayout();
            ((global::System.ComponentModel.ISupportInitialize)this.w_numResourceQuota.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_numStorageQuota.Properties).EndInit();
            this.SiteQuotaPage.ResumeLayout(false);
            this.SiteQuotaPage.PerformLayout();
            ((global::System.ComponentModel.ISupportInitialize)this.w_gbSiteQuota).EndInit();
            this.w_gbSiteQuota.ResumeLayout(false);
            ((global::System.ComponentModel.ISupportInitialize)this.w_plQuotaOptions).EndInit();
            this.w_plQuotaOptions.ResumeLayout(false);
            this.w_plQuotaOptions.PerformLayout();
            ((global::System.ComponentModel.ISupportInitialize)this.w_numMax.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_numWarning.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_rbToIndividual.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_cmbSiteQuotaTemplates.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_rbSetQuota.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_cbSetSiteQuota.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.SiteQuotaTab).EndInit();
            this.SiteQuotaTab.ResumeLayout(false);
            base.ResumeLayout(false);
        }

        public void LoadUI()
        {
            if (Target is SPTenant)
            {
                groupBox4.Visible = false;
                PanelControl panelControl = groupBox3;
                int num = groupBox3.Location.X;
                panelControl.Location = new Point(num, groupBox3.Location.Y + 6);
                groupBox3.Height -= 4;
                groupBox1.Height -= 24;
                global::DevExpress.XtraTab.XtraTabControl siteQuotaTab = SiteQuotaTab;
                int num2 = SiteQuotaTab.Location.X;
                siteQuotaTab.Location = new Point(num2, SiteQuotaTab.Location.Y - 24);
                SiteQuotaPage.PageVisible = false;
                TenantQuotaPage.PageVisible = true;
                w_tbSecOwner.Visible = false;
                w_lblSecOwner.Visible = false;
                w_lblOwner.Text = "Owner:";
                w_plTenantQuotaOption.Visible = true;
                HideControl(cbHostHeader);
            }
            if (Target == null || Target.Languages.Count == 0 || !Target.Languages[0].HasMultipleExperienceVersions || Target.Adapter.SharePointVersion.IsSharePoint2016)
            {
                HideControl(w_plExperienceVersion);
            }
            else
            {
                int num3 = -1;
                object obj = null;
                m_experienceLableToVersionMap.Clear();
                foreach (int experienceVersion in Target.Languages[0].ExperienceVersions)
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
                if (Target.Languages[0].ExperienceVersions.Contains(Options.ExperienceVersion))
                {
                    w_cmbExperienceVersion.SelectedItem = GetExperinceVersionLabel(Options.ExperienceVersion);
                }
                else if (obj != null)
                {
                    w_cmbExperienceVersion.SelectedItem = obj;
                }
            }
            ComboBoxEdit comboBoxEdit = w_cmbWebApp;
            bool enabled = !Options.SelfServiceCreateMode && Target != null && Target.ShowAllSites;
            comboBoxEdit.Enabled = enabled;
            w_cmbContentDatabase.Enabled = !Options.SelfServiceCreateMode;
            w_cbSetSiteQuota.Enabled = !Options.SelfServiceCreateMode;
            w_plQuotaOptions.Enabled = !Options.SelfServiceCreateMode && Options.SetSiteQuota;
            cbHostHeader.Enabled = !Options.SelfServiceCreateMode;
            if (Options.WebApplication != null)
            {
                w_cmbWebApp.SelectedItem = Options.WebApplication;
            }
            if (Options.Language != null)
            {
                w_cmbLang.SelectedItem = Options.Language;
            }
            if (Options.Template != null)
            {
                w_cmbTemplate.SelectedItem = Options.Template;
            }
            w_tbTitle.Text = Options.Title;
            w_tbDescription.Text = Options.Description;
            if (Options.Path != null && !cbHostHeader.Checked)
            {
                w_cmbPath.SelectedItem = Options.Path;
            }
            if (w_cmbPath.SelectedIndex > 0 && w_cmbPath.Properties.Items.Count != 0)
            {
                w_cmbPath.SelectedIndex = ((w_cmbPath.Properties.Items.Count > 1) ? 1 : 0);
            }
            w_tbURL.Text = Options.URL;
            w_tbOwner.Text = Options.OwnerLogin;
            w_tbSecOwner.Text = Options.SecondaryOwnerLogin;
            bool flag = false;
            w_numMax.Value = Options.QuotaMaximum;
            w_numWarning.Value = Options.QuotaWarning;
            if (Target is SPTenant)
            {
                w_numStorageQuota.Value = Options.StorageQuota;
                w_numResourceQuota.Value = Convert.ToDecimal(Options.ResourceQuota);
            }
            if (Options.QuotaID != null)
            {
                foreach (object item in w_cmbSiteQuotaTemplates.Properties.Items)
                {
                    if (!(item is SPSiteQuota sPSiteQuota) || !(sPSiteQuota.QuotaID == Options.QuotaID))
                    {
                        continue;
                    }
                    w_cmbSiteQuotaTemplates.SelectedItem = sPSiteQuota;
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                w_rbSetQuota.Checked = true;
                w_cbSetSiteQuota.Checked = true;
            }
            else if (w_numMax.Value > 0m || w_numWarning.Value > 0m)
            {
                w_rbToIndividual.Checked = true;
                w_cbSetSiteQuota.Checked = true;
            }
            w_cbSetSiteQuota.Checked = Options.SetSiteQuota;
            if (!(Target is SPTenant))
            {
                cbHostHeader.Checked = !Options.SelfServiceCreateMode && Options.IsHostHeader;
                UpdateSiteURLUI();
            }
        }

        private void On_btnCheckPrimary_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            CheckPrimary();
            Cursor = Cursors.Default;
        }

        private void On_btnCheckSeconday_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            CheckSecondary();
            Cursor = Cursors.Default;
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

        private void On_Path_Changed(object sender, EventArgs e)
        {
            if (w_cmbPath.SelectedItem is SPPath sPPath)
            {
                w_tbURL.Enabled = sPPath.IsWildcard;
            }
        }

        private void On_SetQuota_CheckedChanged(object sender, EventArgs e)
        {
            w_plQuotaOptions.Enabled = w_cbSetSiteQuota.Checked;
            w_plTenantQuotaOption.Enabled = w_cbSetSiteQuota.Checked;
        }

        private void On_SiteQuota_CheckedChanged(object sender, EventArgs e)
        {
            w_cmbSiteQuotaTemplates.Enabled = w_rbSetQuota.Checked;
            w_numMax.Enabled = w_rbToIndividual.Checked;
            w_numWarning.Enabled = w_rbToIndividual.Checked;
        }

        private void On_WebApp_Changed(object sender, EventArgs e)
        {
            if (w_cmbWebApp.SelectedItem is SPWebApplication)
            {
                m_selectedWebApplication = (SPWebApplication)w_cmbWebApp.SelectedItem;
                UpdateWebAppData();
            }
        }

        public bool SaveUI()
        {
            Options.ValidateOwnerLogins = false;
            m_sPrimaryInternal = w_tbOwner.Text;
            m_sSecondaryInternal = w_tbSecOwner.Text;
            Options.WebApplication = (SPWebApplication)w_cmbWebApp.SelectedItem;
            CreateSiteCollectionOptions options = Options;
            string contentDatabaseName = ((!(w_cmbContentDatabase.Text == global::Metalogix.SharePoint.Properties.Resources.Auto_Detect)) ? w_cmbContentDatabase.Text : null);
            options.ContentDatabaseName = contentDatabaseName;
            Options.Language = (SPLanguage)w_cmbLang.SelectedItem;
            Options.Template = (SPWebTemplate)w_cmbTemplate.SelectedItem;
            Options.Title = w_tbTitle.Text;
            Options.Description = w_tbDescription.Text;
            if (cbHostHeader.Checked)
            {
                Options.HostHeaderURL = w_lblSelWebApp.Text + w_tbURL.Text;
            }
            else
            {
                Options.Path = w_cmbPath.SelectedItem.ToString();
            }
            Options.IsHostHeader = cbHostHeader.Checked;
            Options.URL = (w_tbURL.Enabled ? w_tbURL.Text : "");
            Options.OwnerLogin = m_sPrimaryInternal;
            CreateSiteCollectionOptions options2 = Options;
            string secondaryOwnerLogin = ((m_sSecondaryInternal != null && m_sSecondaryInternal.Length > 0) ? m_sSecondaryInternal : null);
            options2.SecondaryOwnerLogin = secondaryOwnerLogin;
            Options.SetSiteQuota = w_cbSetSiteQuota.Checked;
            CreateSiteCollectionOptions options3 = Options;
            string quotaID = ((w_cbSetSiteQuota.Checked && w_rbSetQuota.Checked) ? ((SPSiteQuota)w_cmbSiteQuotaTemplates.SelectedItem).QuotaID : null);
            options3.QuotaID = quotaID;
            Options.QuotaMaximum = ((!w_cbSetSiteQuota.Checked || !w_rbToIndividual.Checked) ? 0 : ((long)w_numMax.Value));
            Options.QuotaWarning = ((!w_cbSetSiteQuota.Checked || !w_rbToIndividual.Checked) ? 0 : ((long)w_numWarning.Value));
            string key = ((w_cmbExperienceVersion.SelectedItem == null) ? "" : w_cmbExperienceVersion.SelectedItem.ToString());
            if (!m_experienceLableToVersionMap.TryGetValue(key, out var value))
            {
                Options.ExperienceVersion = -1;
            }
            else
            {
                Options.ExperienceVersion = value;
            }
            if (Target is SPTenant)
            {
                Options.StorageQuota = (long)w_numStorageQuota.Value;
                Options.ResourceQuota = Convert.ToDouble(w_numResourceQuota.Value);
            }
            return true;
        }

        private void SPCreateSiteCollectionControl_SizeChanged(object sender, EventArgs e)
        {
        }

        private void UpdateAvaliableTemplates()
        {
            if (m_selectedLanguage == null)
            {
                return;
            }
            SuspendLayout();
            string text = ((w_cmbTemplate.SelectedItem == null) ? null : ((SPWebTemplate)w_cmbTemplate.SelectedItem).Name);
            string str = text;
            w_cmbTemplate.Properties.Items.Clear();
            string key = ((w_cmbExperienceVersion.SelectedItem == null) ? "" : w_cmbExperienceVersion.SelectedItem.ToString());
            SPWebTemplateCollection source = ((m_experienceLableToVersionMap.Count <= 1 || !m_experienceLableToVersionMap.ContainsKey(key)) ? m_selectedLanguage.Templates : m_selectedLanguage.GetTemplatesForExperienceVersion(m_experienceLableToVersionMap[key]));
            IEnumerable<SPWebTemplate> source2 = source.Where(RestrictedTemplates.Compile());
            if (Target.Adapter.SharePointVersion.IsSharePoint2013OrLater)
            {
                source2 = source2.Where(RestrictedTemplatesInSp2013.Compile());
            }
            source2.OrderBy((SPWebTemplate template) => template.Title).ToList().ForEach(delegate (SPWebTemplate template)
            {
                w_cmbTemplate.Properties.Items.Add(template);
            });
            if (str != null)
            {
                w_cmbTemplate.SelectedItem = source2.FirstOrDefault((SPWebTemplate template) => template.Name == str);
                if (w_cmbTemplate.SelectedItem == null)
                {
                    w_cmbTemplate.Text = "";
                }
            }
            ResumeLayout();
        }

        private void UpdateQuotas()
        {
            if (Target.SiteQuotaTemplates.Count <= 0)
            {
                return;
            }
            foreach (SPSiteQuota siteQuotaTemplate in Target.SiteQuotaTemplates)
            {
                w_cmbSiteQuotaTemplates.Properties.Items.Add(siteQuotaTemplate);
            }
            if (w_cmbSiteQuotaTemplates.Properties.Items.Count > 0)
            {
                w_cmbSiteQuotaTemplates.SelectedIndex = 0;
            }
        }

        private string UpdateServerData()
        {
            string text = "";
            w_cmbLang.Properties.Items.Clear();
            w_cmbWebApp.Properties.Items.Clear();
            if (Target != null)
            {
                if (Target.WebApplications.Count <= 0)
                {
                    text += "Could not enumerate target web applications \n";
                }
                else
                {
                    foreach (SPWebApplication webApplication in Target.WebApplications)
                    {
                        if (webApplication.ContentDatabases.Count > 0 || Target.Adapter.IsClientOM)
                        {
                            w_cmbWebApp.Properties.Items.Add(webApplication);
                        }
                    }
                    if (w_cmbWebApp.Properties.Items.Count <= 0)
                    {
                        text += global::Metalogix.SharePoint.Properties.Resources.MsgNoContentDatabasesInWebApps;
                    }
                    else
                    {
                        w_cmbWebApp.SelectedIndex = 0;
                    }
                }
                if (Target.Languages.Count <= 0)
                {
                    text += "Could not enumerate target language packs \n";
                }
                else
                {
                    foreach (SPLanguage language in Target.Languages)
                    {
                        w_cmbLang.Properties.Items.Add(language);
                    }
                    if (w_cmbLang.Properties.Items.Count > 0)
                    {
                        w_cmbLang.SelectedIndex = 0;
                    }
                }
            }
            UpdateQuotas();
            w_cmbSiteQuotaTemplates.Enabled = w_rbSetQuota.Checked;
            w_numMax.Enabled = w_rbToIndividual.Checked;
            w_numWarning.Enabled = w_rbToIndividual.Checked;
            w_plQuotaOptions.Enabled = w_cbSetSiteQuota.Checked;
            return text;
        }

        private void UpdateSiteURLUI()
        {
            if (!cbHostHeader.Checked)
            {
                w_cmbPath.Visible = true;
                UpdateWebAppData();
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

        private void UpdateUI()
        {
            UpdateServerData();
        }

        private void UpdateWebAppData()
        {
            SuspendLayout();
            w_lblSelWebApp.Text = m_selectedWebApplication.Url;
            int num = w_lblSelWebApp.Location.X + w_lblSelWebApp.Width + 6;
            ComboBoxEdit comboBoxEdit = w_cmbPath;
            comboBoxEdit.Location = new Point(num, w_cmbPath.Location.Y);
            w_cmbPath.Properties.Items.Clear();
            Label label = new Label();
            int num2 = 0;
            label.Text = "";
            if (label.PreferredWidth > num2)
            {
                num2 = label.PreferredWidth;
            }
            foreach (SPPath path in m_selectedWebApplication.Paths)
            {
                string sNodeName = (m_selectedWebApplication.Url + path.ToString()).Replace("/", "%2F");
                if (path.IsWildcard || Target.Sites[sNodeName] == null)
                {
                    w_cmbPath.Properties.Items.Add(path);
                    label.Text = string.Concat(path, "/");
                    if (label.PreferredWidth > num2)
                    {
                        num2 = label.PreferredWidth;
                    }
                }
            }
            if (w_cmbPath.Properties.Items.Count <= 1)
            {
                w_cmbPath.SelectedIndex = 0;
            }
            else
            {
                w_cmbPath.SelectedIndex = 1;
            }
            num2 += 20;
            int num3 = w_cmbPath.Width;
            w_cmbPath.Size = new Size(num2, w_cmbPath.Height);
            Point location = w_cmbPath.Location;
            Point location2 = w_tbURL.Location;
            int num4 = location.X + w_cmbPath.Width + 6 - location2.X;
            TextEdit textEdit = w_tbURL;
            Point location3 = w_tbURL.Location;
            Point location4 = w_tbURL.Location;
            textEdit.Location = new Point(location3.X + num4, location4.Y);
            TextEdit textEdit2 = w_tbURL;
            Size size = w_tbURL.Size;
            Size size2 = w_tbURL.Size;
            textEdit2.Size = new Size(size.Width - num4, size2.Height);
            w_cmbContentDatabase.Properties.Items.Clear();
            w_cmbContentDatabase.Properties.Items.Add(global::Metalogix.SharePoint.Properties.Resources.Auto_Detect);
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

        public bool ValidateEntries()
        {
            string text = null;
            bool flag = false;
            if (w_tbSecOwner.Text.Length != 0 && !CheckSecondary())
            {
                text = "Specified secondary site collection administrator is invalid";
                flag = true;
                w_tbSecOwner.Focus();
            }
            if (w_tbOwner.Text.Length == 0)
            {
                text = "Please enter the login name for the owner of the site collection";
                flag = false;
                w_tbOwner.Focus();
            }
            else if (!CheckPrimary())
            {
                text = "Specified primary site collection administrator is invalid";
                flag = true;
                w_tbOwner.Focus();
            }
            if (w_cmbLang.SelectedIndex < 0)
            {
                text = "Please specify the language pack for the site collection to use";
                flag = false;
                w_cmbLang.Focus();
            }
            if (w_cmbTemplate.SelectedIndex < 0)
            {
                text = "Please specify a web template for the site collection to use";
                flag = false;
                w_cmbTemplate.Focus();
            }
            if (w_cmbPath.SelectedIndex < 0)
            {
                text = "Please select a managed path for the site collection";
                flag = false;
                w_cmbPath.Focus();
            }
            if (w_tbURL.Text.Length == 0 && w_tbURL.Enabled)
            {
                text = "Please enter a url for the site collection";
                flag = false;
                w_tbURL.Focus();
            }
            if (w_tbTitle.Text.Length == 0)
            {
                text = "Please enter a title for the site collection";
                flag = false;
                w_tbTitle.Focus();
            }
            if (!Utils.IsValidSharePointURL(w_tbURL.Text, cbHostHeader.Checked))
            {
                text = global::Metalogix.Actions.Properties.Resources.ValidSiteCollectionURL + " " + (cbHostHeader.Checked ? Utils.illegalCharactersForHostHeaderSiteUrl : Utils.IllegalCharactersForSiteUrl);
                flag = false;
                w_tbURL.Focus();
            }
            if (w_cmbWebApp.SelectedIndex < 0)
            {
                text = "Please select a web application";
                flag = false;
                w_cmbWebApp.Focus();
            }
            if (w_cbSetSiteQuota.Checked && w_rbSetQuota.Checked && w_cmbSiteQuotaTemplates.SelectedItem == null)
            {
                text = "Please select the site quota template to use for the site collection";
                flag = false;
                w_cmbSiteQuotaTemplates.Focus();
            }
            if (w_cbSetSiteQuota.Checked && !w_rbSetQuota.Checked && !w_rbToIndividual.Checked)
            {
                text = "Please select the site quota template to use for the site collection";
                flag = false;
                w_rbToIndividual.Focus();
            }
            if (text == null)
            {
                return true;
            }
            if (!flag)
            {
                FlatXtraMessageBox.Show(text, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }
            if (FlatXtraMessageBox.Show(text + ". Allow?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) != DialogResult.Yes)
            {
                return false;
            }
            Options.ValidateOwnerLogins = false;
            return true;
        }

        private string ValidateUser(string sIdentifier)
        {
            return sIdentifier;
        }

        private void w_cmbExperienceVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAvaliableTemplates();
            FireTemplatesChanged();
        }

        private void w_tbOwner_TextChanged(object sender, EventArgs e)
        {
            if (!m_bSuspendUserFieldEvents)
            {
                m_bPrimaryIsValidated = false;
                m_sPrimaryInternal = null;
                w_tbOwner.Font = new Font(w_tbOwner.Font, FontStyle.Regular);
                w_tbOwner.ForeColor = Color.Black;
            }
        }

        private void w_tbSecOwner_TextChanged(object sender, EventArgs e)
        {
            if (!m_bSuspendUserFieldEvents)
            {
                m_bSecondaryIsValidated = false;
                m_sSecondaryInternal = null;
                w_tbSecOwner.Font = new Font(w_tbSecOwner.Font, FontStyle.Regular);
                w_tbSecOwner.ForeColor = Color.Black;
            }
        }
    }
}