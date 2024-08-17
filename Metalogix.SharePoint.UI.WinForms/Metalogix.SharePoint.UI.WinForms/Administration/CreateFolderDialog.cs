using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.Actions;
using Metalogix.SharePoint.Options.Administration;
using Metalogix.UI.WinForms;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
    public class CreateFolderDialog : XtraForm
    {
        private CreateFolderOptions _options;

        private IContainer components;

        private SimpleButton w_btnCancel;

        private SimpleButton w_btnOK;

        private TextEdit w_txtFolderName;

        private LabelControl label1;

        private LabelControl label2;

        private ComboBoxEdit w_comboBoxFolderType;

        private CheckEdit w_checkBoxOverwrite;

        public ActionContext Context { get; set; }

        public CreateFolderOptions Options
        {
            get
            {
                if (_options == null)
                {
                    _options = new CreateFolderOptions();
                }
                _options.FolderName = w_txtFolderName.Text;
                _options.Overwrite = w_checkBoxOverwrite.Checked;
                SPContentType sPContentType = w_comboBoxFolderType.SelectedItem as SPContentType;
                _options.ContentType = sPContentType.Name;
                _options.ContentTypeId = sPContentType.ContentTypeID;
                return _options;
            }
            set
            {
                _options = value;
                if (_options == null)
                {
                    return;
                }
                w_txtFolderName.Text = _options.FolderName;
                w_checkBoxOverwrite.Checked = _options.Overwrite;
                foreach (SPContentType item in w_comboBoxFolderType.Properties.Items)
                {
                    if (!item.ContentTypeID.Equals(_options.ContentTypeId, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    w_comboBoxFolderType.EditValue = item;
                    break;
                }
            }
        }

        public CreateFolderDialog()
        {
            InitializeComponent();
        }

        private void CreateFolderDialog_Load(object sender, EventArgs e)
        {
            if (Context == null || Context.Targets == null || Context.Targets.Count == 0 || !(Context.Targets[0] is SPFolder sPFolder))
            {
                return;
            }
            SPList parentList = sPFolder.ParentList;
            List<SPContentType> list = new List<SPContentType>();
            if (parentList.ContentTypes != null)
            {
                foreach (SPContentType contentType in parentList.ContentTypes)
                {
                    if (contentType.ContentTypeID.StartsWith("0x0120"))
                    {
                        list.Add(contentType);
                        if (list.Count == 2)
                        {
                            break;
                        }
                    }
                }
            }
            if (list.Count <= 0)
            {
                base.DialogResult = DialogResult.Cancel;
                Close();
                FlatXtraMessageBox.Show("Cannot create folders in the target item.", "Cannot Create Folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                w_comboBoxFolderType.Properties.Items.AddRange(list.ToArray());
                w_comboBoxFolderType.SelectedIndex = 0;
                w_comboBoxFolderType.Enabled = parentList.ContentTypesEnabled && list.Count > 1;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Administration.CreateFolderDialog));
            this.w_btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.w_btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.w_txtFolderName = new DevExpress.XtraEditors.TextEdit();
            this.label1 = new DevExpress.XtraEditors.LabelControl();
            this.label2 = new DevExpress.XtraEditors.LabelControl();
            this.w_comboBoxFolderType = new DevExpress.XtraEditors.ComboBoxEdit();
            this.w_checkBoxOverwrite = new DevExpress.XtraEditors.CheckEdit();
            ((System.ComponentModel.ISupportInitialize)this.w_txtFolderName.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_comboBoxFolderType.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.w_checkBoxOverwrite.Properties).BeginInit();
            base.SuspendLayout();
            resources.ApplyResources(this.w_btnCancel, "w_btnCancel");
            this.w_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.w_btnCancel.Name = "w_btnCancel";
            resources.ApplyResources(this.w_btnOK, "w_btnOK");
            this.w_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.w_btnOK.Name = "w_btnOK";
            this.w_btnOK.Click += new System.EventHandler(w_btnOK_Click);
            resources.ApplyResources(this.w_txtFolderName, "w_txtFolderName");
            this.w_txtFolderName.Name = "w_txtFolderName";
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            resources.ApplyResources(this.w_comboBoxFolderType, "w_comboBoxFolderType");
            this.w_comboBoxFolderType.Name = "w_comboBoxFolderType";
            DevExpress.XtraEditors.Controls.EditorButtonCollection buttons = this.w_comboBoxFolderType.Properties.Buttons;
            DevExpress.XtraEditors.Controls.EditorButton[] buttons2 = new DevExpress.XtraEditors.Controls.EditorButton[1]
            {
                new DevExpress.XtraEditors.Controls.EditorButton()
            };
            buttons.AddRange(buttons2);
            this.w_comboBoxFolderType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            resources.ApplyResources(this.w_checkBoxOverwrite, "w_checkBoxOverwrite");
            this.w_checkBoxOverwrite.Name = "w_checkBoxOverwrite";
            this.w_checkBoxOverwrite.Properties.AutoWidth = true;
            this.w_checkBoxOverwrite.Properties.Caption = resources.GetString("w_checkBoxOverwrite.Properties.Caption");
            base.AcceptButton = this.w_btnOK;
            resources.ApplyResources(this, "$this");
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = this.w_btnCancel;
            base.Controls.Add(this.w_checkBoxOverwrite);
            base.Controls.Add(this.w_comboBoxFolderType);
            base.Controls.Add(this.label2);
            base.Controls.Add(this.w_txtFolderName);
            base.Controls.Add(this.label1);
            base.Controls.Add(this.w_btnCancel);
            base.Controls.Add(this.w_btnOK);
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "CreateFolderDialog";
            base.ShowInTaskbar = false;
            base.Load += new System.EventHandler(CreateFolderDialog_Load);
            ((System.ComponentModel.ISupportInitialize)this.w_txtFolderName.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_comboBoxFolderType.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.w_checkBoxOverwrite.Properties).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void w_btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (w_txtFolderName.Text == "")
                {
                    w_txtFolderName.Focus();
                    throw new Exception("Folder name cannot be blank");
                }
                if (w_comboBoxFolderType.SelectedItem == null)
                {
                    w_comboBoxFolderType.Focus();
                    throw new Exception("Folder type cannot be blank");
                }
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                GlobalServices.ErrorHandler.HandleException("Error", ex2.Message, ex2, ErrorIcon.Warning);
                base.DialogResult = DialogResult.None;
            }
        }
    }
}
