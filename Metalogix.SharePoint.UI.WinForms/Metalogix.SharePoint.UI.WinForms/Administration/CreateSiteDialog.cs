using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Options.Administration;
using Metalogix.UI.WinForms;
using Metalogix.Utilities;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
    public class CreateSiteDialog : XtraForm
    {
        private LabelControl label1;

        private LabelControl label2;

        private TextEdit w_txtSiteURL;

        private TextEdit w_txtDescription;

        private ComboBoxEdit w_comboTemplate;

        private SimpleButton w_btnOK;

        private SimpleButton w_btnCancel;

        private LabelControl w_lblTemplate;

        private Container components;

        private TextEdit w_tbSiteTitle;

        private LabelControl label3;

        private static readonly string[] restrictedSiteTemplates;

        public static readonly Expression<Func<SPWebTemplate, bool>> RestrictedSiteTemplates;

        private SPWebTemplate m_BlankTemplate;

        private CreateSiteOptions m_options = new CreateSiteOptions();

        public CreateSiteOptions Options
        {
            get
            {
                return m_options;
            }
            set
            {
                PropertyInfo[] properties = typeof(CreateSiteOptions).GetProperties();
                foreach (PropertyInfo propertyInfo in properties)
                {
                    if (propertyInfo.CanWrite && propertyInfo.CanRead)
                    {
                        propertyInfo.SetValue(m_options, propertyInfo.GetValue(value, null), null);
                    }
                }
                LoadUI();
            }
        }

        public SPWebTemplateCollection WebTemplates { get; set; }

        static CreateSiteDialog()
        {
            restrictedSiteTemplates = new string[3] { "ACCSRV#1", "DOCMARKETPLACESITE#0", "EDISC#1" };
            RestrictedSiteTemplates = (SPWebTemplate template) => !template.IsRootWebOnly && !template.IsHidden && restrictedSiteTemplates.All((string item) => template.Name != item);
        }

        public CreateSiteDialog(SPWebTemplateCollection templates)
        {
            WebTemplates = null;
            InitializeComponent();
            IOrderedEnumerable<SPWebTemplate> orderedEnumerable = from template in templates.Where(RestrictedSiteTemplates.Compile())
                orderby template.Title
                select template;
            foreach (SPWebTemplate item in orderedEnumerable)
            {
                w_comboTemplate.Properties.Items.Add(item);
                if (!(item.Name != "STS#1"))
                {
                    m_BlankTemplate = item;
                }
            }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Administration.CreateSiteDialog));
            this.label1 = new DevExpress.XtraEditors.LabelControl();
            this.label2 = new DevExpress.XtraEditors.LabelControl();
            this.w_txtSiteURL = new DevExpress.XtraEditors.TextEdit();
            this.w_txtDescription = new DevExpress.XtraEditors.TextEdit();
            this.w_comboTemplate = new DevExpress.XtraEditors.ComboBoxEdit();
            this.w_lblTemplate = new DevExpress.XtraEditors.LabelControl();
            this.w_btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.w_btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.w_tbSiteTitle = new DevExpress.XtraEditors.TextEdit();
            this.label3 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)this.w_txtSiteURL.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_txtDescription.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_comboTemplate.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_tbSiteTitle.Properties).BeginInit();
            base.SuspendLayout();
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            resources.ApplyResources(this.w_txtSiteURL, "w_txtSiteURL");
            this.w_txtSiteURL.Name = "w_txtSiteURL";
            resources.ApplyResources(this.w_txtDescription, "w_txtDescription");
            this.w_txtDescription.Name = "w_txtDescription";
            resources.ApplyResources(this.w_comboTemplate, "w_comboTemplate");
            this.w_comboTemplate.Name = "w_comboTemplate";
            DevExpress.XtraEditors.Controls.EditorButtonCollection buttons = this.w_comboTemplate.Properties.Buttons;
            DevExpress.XtraEditors.Controls.EditorButton[] buttons2 = new DevExpress.XtraEditors.Controls.EditorButton[1]
            {
                new DevExpress.XtraEditors.Controls.EditorButton()
            };
            buttons.AddRange(buttons2);
            this.w_comboTemplate.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            resources.ApplyResources(this.w_lblTemplate, "w_lblTemplate");
            this.w_lblTemplate.Name = "w_lblTemplate";
            resources.ApplyResources(this.w_btnOK, "w_btnOK");
            this.w_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.w_btnOK.Name = "w_btnOK";
            this.w_btnOK.Click += new System.EventHandler(w_btnOK_Click);
            resources.ApplyResources(this.w_btnCancel, "w_btnCancel");
            this.w_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.w_btnCancel.Name = "w_btnCancel";
            resources.ApplyResources(this.w_tbSiteTitle, "w_tbSiteTitle");
            this.w_tbSiteTitle.Name = "w_tbSiteTitle";
            this.w_tbSiteTitle.Leave += new System.EventHandler(On_tbSiteTitle_Leave);
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            base.AcceptButton = this.w_btnOK;
            resources.ApplyResources(this, "$this");
            base.CancelButton = this.w_btnCancel;
            base.Controls.Add(this.w_tbSiteTitle);
            base.Controls.Add(this.label3);
            base.Controls.Add(this.w_btnCancel);
            base.Controls.Add(this.w_btnOK);
            base.Controls.Add(this.w_lblTemplate);
            base.Controls.Add(this.w_comboTemplate);
            base.Controls.Add(this.w_txtDescription);
            base.Controls.Add(this.w_txtSiteURL);
            base.Controls.Add(this.label2);
            base.Controls.Add(this.label1);
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "CreateSiteDialog";
            base.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)this.w_txtSiteURL.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_txtDescription.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_comboTemplate.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_tbSiteTitle.Properties).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void LoadUI()
        {
            if (Options.Template == null && m_BlankTemplate != null)
            {
                w_comboTemplate.SelectedItem = m_BlankTemplate;
            }
            else
            {
                w_comboTemplate.SelectedItem = w_comboTemplate.Properties.Items[0];
            }
        }

        private void On_tbSiteTitle_Leave(object sender, EventArgs e)
        {
            if (!w_tbSiteTitle.Text.IsNullOrWhiteSpace())
            {
                w_txtSiteURL.Text = UrlUtils.GetValidSiteUrl(w_tbSiteTitle.Text);
            }
        }

        private void SaveUI()
        {
        }

        private void UpdateUI()
        {
            SuspendLayout();
        }

        private void w_btnOK_Click(object sender, EventArgs e)
        {
            if (w_tbSiteTitle.IsEditorActive)
            {
                On_tbSiteTitle_Leave(null, null);
            }
            if (w_txtSiteURL.Text == "")
            {
                FlatXtraMessageBox.Show("Site name cannot be blank", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                base.DialogResult = DialogResult.None;
                w_txtSiteURL.Focus();
            }
            else
            {
                Options.Title = w_tbSiteTitle.Text;
                Options.URL = w_txtSiteURL.Text;
                Options.Description = w_txtDescription.Text;
                Options.Template = (SPWebTemplate)w_comboTemplate.SelectedItem;
            }
        }
    }
}
