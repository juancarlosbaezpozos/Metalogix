using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Options.Administration;
using Metalogix.SharePoint.UI.WinForms.Administration;
using Metalogix.UI.WinForms;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
    public class CreateListDialog : XtraForm
    {
        private SPWeb _targetWeb;

        private SPListTemplateCollection _listTemplates;

        private CreateListOptions m_options = new CreateListOptions();

        private IContainer components;

        private SimpleButton w_btnCancel;

        private SimpleButton w_btnOK;

        private LabelControl w_lblType;

        private ComboBoxEdit w_comboListType;

        private TextEdit w_txtDescription;

        private TextEdit w_txtListName;

        private LabelControl w_lblDescription;

        private LabelControl w_lblName;

        private CheckEdit w_chkQuickLaunch;

        private CheckEdit w_chkApproval;

        private CheckEdit w_chkVersions;

        private CheckEdit w_chkMinorVersions;

        private TextEdit w_txtListTitle;

        private LabelControl w_lblTitle;

        public SPListTemplateCollection ListTemplates
        {
            get
            {
                return _listTemplates;
            }
            private set
            {
                if (value == null)
                {
                    return;
                }
                w_comboListType.Properties.Items.Clear();
                foreach (SPListTemplate item in value)
                {
                    if (!item.IsHidden && item.IsCreatableTemplateType && IsAllowedTemplate(item))
                    {
                        w_comboListType.Properties.Items.Add(item);
                    }
                }
                _listTemplates = value;
            }
        }

        public CreateListOptions Options
        {
            get
            {
                return m_options;
            }
            set
            {
                PropertyInfo[] properties = typeof(CreateListOptions).GetProperties();
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

        public SPWeb TargetWeb => _targetWeb;

        public CreateListDialog(SPWeb targetWeb)
        {
            InitializeComponent();
            _targetWeb = targetWeb;
            ListTemplates = targetWeb.ListTemplates;
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
            global::System.ComponentModel.ComponentResourceManager resources = new global::System.ComponentModel.ComponentResourceManager(typeof(global::Metalogix.SharePoint.UI.WinForms.Administration.CreateListDialog));
            this.w_btnCancel = new global::DevExpress.XtraEditors.SimpleButton();
            this.w_btnOK = new global::DevExpress.XtraEditors.SimpleButton();
            this.w_lblType = new global::DevExpress.XtraEditors.LabelControl();
            this.w_comboListType = new global::DevExpress.XtraEditors.ComboBoxEdit();
            this.w_txtDescription = new global::DevExpress.XtraEditors.TextEdit();
            this.w_txtListName = new global::DevExpress.XtraEditors.TextEdit();
            this.w_lblDescription = new global::DevExpress.XtraEditors.LabelControl();
            this.w_lblName = new global::DevExpress.XtraEditors.LabelControl();
            this.w_chkQuickLaunch = new global::DevExpress.XtraEditors.CheckEdit();
            this.w_chkApproval = new global::DevExpress.XtraEditors.CheckEdit();
            this.w_chkVersions = new global::DevExpress.XtraEditors.CheckEdit();
            this.w_chkMinorVersions = new global::DevExpress.XtraEditors.CheckEdit();
            this.w_txtListTitle = new global::DevExpress.XtraEditors.TextEdit();
            this.w_lblTitle = new global::DevExpress.XtraEditors.LabelControl();
            ((global::System.ComponentModel.ISupportInitialize)this.w_comboListType.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_txtDescription.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_txtListName.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_chkQuickLaunch.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_chkApproval.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_chkVersions.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_chkMinorVersions.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_txtListTitle.Properties).BeginInit();
            base.SuspendLayout();
            resources.ApplyResources(this.w_btnCancel, "w_btnCancel");
            this.w_btnCancel.DialogResult = global::System.Windows.Forms.DialogResult.Cancel;
            this.w_btnCancel.Name = "w_btnCancel";
            resources.ApplyResources(this.w_btnOK, "w_btnOK");
            this.w_btnOK.DialogResult = global::System.Windows.Forms.DialogResult.OK;
            this.w_btnOK.Name = "w_btnOK";
            resources.ApplyResources(this.w_lblType, "w_lblType");
            this.w_lblType.Name = "w_lblType";
            resources.ApplyResources(this.w_comboListType, "w_comboListType");
            this.w_comboListType.Name = "w_comboListType";
            global::DevExpress.XtraEditors.Controls.EditorButtonCollection buttons = this.w_comboListType.Properties.Buttons;
            global::DevExpress.XtraEditors.Controls.EditorButton[] buttons2 = new global::DevExpress.XtraEditors.Controls.EditorButton[1]
            {
            new global::DevExpress.XtraEditors.Controls.EditorButton()
            };
            buttons.AddRange(buttons2);
            this.w_comboListType.Properties.TextEditStyle = global::DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.w_comboListType.SelectedIndexChanged += new global::System.EventHandler(On_ListType_SelectedIndexChanged);
            resources.ApplyResources(this.w_txtDescription, "w_txtDescription");
            this.w_txtDescription.Name = "w_txtDescription";
            resources.ApplyResources(this.w_txtListName, "w_txtListName");
            this.w_txtListName.Name = "w_txtListName";
            resources.ApplyResources(this.w_lblDescription, "w_lblDescription");
            this.w_lblDescription.Name = "w_lblDescription";
            resources.ApplyResources(this.w_lblName, "w_lblName");
            this.w_lblName.Name = "w_lblName";
            resources.ApplyResources(this.w_chkQuickLaunch, "w_chkQuickLaunch");
            this.w_chkQuickLaunch.Name = "w_chkQuickLaunch";
            this.w_chkQuickLaunch.Properties.Caption = resources.GetString("w_chkQuickLaunch.Properties.Caption");
            resources.ApplyResources(this.w_chkApproval, "w_chkApproval");
            this.w_chkApproval.Name = "w_chkApproval";
            this.w_chkApproval.Properties.Caption = resources.GetString("w_chkApproval.Properties.Caption");
            resources.ApplyResources(this.w_chkVersions, "w_chkVersions");
            this.w_chkVersions.Name = "w_chkVersions";
            this.w_chkVersions.Properties.Caption = resources.GetString("w_chkVersions.Properties.Caption");
            this.w_chkVersions.CheckedChanged += new global::System.EventHandler(On_Versions_CheckedChanged);
            resources.ApplyResources(this.w_chkMinorVersions, "w_chkMinorVersions");
            this.w_chkMinorVersions.Name = "w_chkMinorVersions";
            this.w_chkMinorVersions.Properties.Caption = resources.GetString("w_chkMinorVersions.Properties.Caption");
            resources.ApplyResources(this.w_txtListTitle, "w_txtListTitle");
            this.w_txtListTitle.Name = "w_txtListTitle";
            this.w_txtListTitle.Leave += new global::System.EventHandler(On_tbListTitle_Leave);
            resources.ApplyResources(this.w_lblTitle, "w_lblTitle");
            this.w_lblTitle.Name = "w_lblTitle";
            base.AcceptButton = this.w_btnOK;
            resources.ApplyResources(this, "$this");
            base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = this.w_btnCancel;
            base.Controls.Add(this.w_txtListTitle);
            base.Controls.Add(this.w_lblTitle);
            base.Controls.Add(this.w_chkMinorVersions);
            base.Controls.Add(this.w_chkVersions);
            base.Controls.Add(this.w_chkApproval);
            base.Controls.Add(this.w_chkQuickLaunch);
            base.Controls.Add(this.w_btnCancel);
            base.Controls.Add(this.w_btnOK);
            base.Controls.Add(this.w_lblType);
            base.Controls.Add(this.w_comboListType);
            base.Controls.Add(this.w_txtDescription);
            base.Controls.Add(this.w_txtListName);
            base.Controls.Add(this.w_lblDescription);
            base.Controls.Add(this.w_lblName);
            base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "CreateListDialog";
            base.ShowInTaskbar = false;
            base.FormClosing += new global::System.Windows.Forms.FormClosingEventHandler(On_Form_Closing);
            ((global::System.ComponentModel.ISupportInitialize)this.w_comboListType.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_txtDescription.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_txtListName.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_chkQuickLaunch.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_chkApproval.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_chkVersions.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_chkMinorVersions.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_txtListTitle.Properties).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private bool IsAllowedTemplate(SPListTemplate template)
        {
            if (TargetWeb.Template.Name.Equals("BLOG#0") && (template.TemplateType == ListTemplateType.BlogPosts || template.TemplateType == ListTemplateType.BlogComments))
            {
                return false;
            }
            return true;
        }

        private void LoadUI()
        {
            w_chkApproval.Checked = Options.RequiresContentApproval;
            w_chkVersions.Checked = Options.HasVersions;
            w_chkMinorVersions.Checked = Options.HasMinorVersions && Options.HasVersions;
            w_chkQuickLaunch.Checked = Options.IsOnQuickLaunch;
            w_txtListTitle.Text = Options.Title;
            w_txtListName.Text = Options.Name;
            w_txtDescription.Text = Options.Description;
            w_comboListType.SelectedItem = ListTemplates.GetByType(Options.Template);
        }

        private void On_Form_Closing(object sender, FormClosingEventArgs e)
        {
            if (base.DialogResult == DialogResult.OK)
            {
                if (w_txtListTitle.IsEditorActive)
                {
                    On_tbListTitle_Leave(null, null);
                }
                if (w_txtListName.Text == "")
                {
                    FlatXtraMessageBox.Show("List name cannot be blank", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    base.DialogResult = DialogResult.None;
                    w_txtListName.Focus();
                    e.Cancel = true;
                }
                else if (w_comboListType.SelectedItem == null)
                {
                    FlatXtraMessageBox.Show("A list template must be selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    base.DialogResult = DialogResult.None;
                    w_comboListType.Focus();
                    e.Cancel = true;
                }
                else
                {
                    SaveUI();
                }
            }
        }

        private void On_ListType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void On_tbListTitle_Leave(object sender, EventArgs e)
        {
            string text = w_txtListTitle.Text.Trim();
            if (text != "" && w_txtListName.Text.Trim() == "")
            {
                w_txtListName.Text = text.Replace(" ", "");
            }
        }

        private void On_Versions_CheckedChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void SaveUI()
        {
            Options.RequiresContentApproval = w_chkApproval.Checked;
            Options.IsOnQuickLaunch = w_chkQuickLaunch.Checked;
            Options.HasVersions = w_chkVersions.Checked;
            Options.HasMinorVersions = w_chkMinorVersions.Checked && w_chkMinorVersions.Enabled;
            Options.Title = w_txtListTitle.Text;
            Options.Name = w_txtListName.Text;
            Options.Description = w_txtDescription.Text;
            Options.Template = ((SPListTemplate)w_comboListType.SelectedItem).TemplateType;
            Options.FeatureId = ((SPListTemplate)w_comboListType.SelectedItem).FeatureId.ToString();
        }

        private void UpdateUI()
        {
            SuspendLayout();
            w_chkMinorVersions.Enabled = w_chkVersions.Checked;
        }
    }
}