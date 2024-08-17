using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using Metalogix;
using Metalogix.Actions;
using Metalogix.UI.WinForms;
using System;
using System.ComponentModel;
using System.Resources;
using System.Security.Principal;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Actions
{
    public partial class ConfigureEmailSettingsDialog : XtraForm
    {
        private SendEmailOptions m_options;

        private IContainer components;

        protected SimpleButton w_btnCancel;

        protected SimpleButton w_btnOK;

        private LabelControl w_lblTo;

        private TextEdit w_tbToEmailAddress;

        private TextEdit w_tbFrom;

        private LabelControl w_lblFrom;

        private TextEdit w_tbCC;

        private LabelControl w_lblCC;

        private TextEdit w_tbBCC;

        private LabelControl w_lblBCC;

        private TextEdit w_tbSubject;

        private LabelControl w_lblSubject;

        private GroupControl w_groupBox;

        private TextEdit w_tbServer;

        private LabelControl w_lblServer;

        private TextEdit w_tbPassword;

        private LabelControl w_labelPwd;

        private TextEdit w_tbDifferentUser;

        private CheckEdit w_radioButtonNewUser;

        private CheckEdit w_radioButtonCurrentUser;

        private GroupControl w_gbTemplates;

        private TextEdit w_tbFailureTemplate;

        private LabelControl label2;

        private TextEdit w_tbSuccessTemplate;

        private LabelControl label1;

        private SimpleButton w_btnSuccessTemplate;

        private SimpleButton w_btnFailureTemplate;

        private TextEditContextMenu _textEditContextMenu;

        public SendEmailOptions Options
        {
            get
            {
                return this.m_options;
            }
            set
            {
                this.m_options = value;
                this.LoadUI();
            }
        }

        public ConfigureEmailSettingsDialog()
        {
            this.InitializeComponent();
            this.w_radioButtonCurrentUser.Text = WindowsIdentity.GetCurrent().Name;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GetFileName(out string sFileName)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                AddExtension = true,
                AutoUpgradeEnabled = true,
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "*.htm;*.html",
                Filter = "HTML files (*.htm;*.html)|*.htm;*.html|All files (*.*)|*.*",
                FilterIndex = 0,
                Multiselect = false,
                RestoreDirectory = true,
                Title = "Select template file",
                InitialDirectory = ApplicationData.ApplicationPath
            };
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                sFileName = null;
                return false;
            }
            sFileName = openFileDialog.FileName;
            return true;
        }

        private void InitializeComponent()
        {
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ConfigureEmailSettingsDialog));
            this.w_btnCancel = new SimpleButton();
            this.w_btnOK = new SimpleButton();
            this.w_lblTo = new LabelControl();
            this.w_tbToEmailAddress = new TextEdit();
            this._textEditContextMenu = new TextEditContextMenu();
            this.w_tbFrom = new TextEdit();
            this.w_lblFrom = new LabelControl();
            this.w_tbCC = new TextEdit();
            this.w_lblCC = new LabelControl();
            this.w_tbBCC = new TextEdit();
            this.w_lblBCC = new LabelControl();
            this.w_tbSubject = new TextEdit();
            this.w_lblSubject = new LabelControl();
            this.w_groupBox = new GroupControl();
            this.w_tbPassword = new TextEdit();
            this.w_labelPwd = new LabelControl();
            this.w_tbDifferentUser = new TextEdit();
            this.w_radioButtonNewUser = new CheckEdit();
            this.w_radioButtonCurrentUser = new CheckEdit();
            this.w_tbServer = new TextEdit();
            this.w_lblServer = new LabelControl();
            this.w_gbTemplates = new GroupControl();
            this.w_btnFailureTemplate = new SimpleButton();
            this.w_btnSuccessTemplate = new SimpleButton();
            this.w_tbFailureTemplate = new TextEdit();
            this.label2 = new LabelControl();
            this.w_tbSuccessTemplate = new TextEdit();
            this.label1 = new LabelControl();
            ((ISupportInitialize)this.w_tbToEmailAddress.Properties).BeginInit();
            ((ISupportInitialize)this.w_tbFrom.Properties).BeginInit();
            ((ISupportInitialize)this.w_tbCC.Properties).BeginInit();
            ((ISupportInitialize)this.w_tbBCC.Properties).BeginInit();
            ((ISupportInitialize)this.w_tbSubject.Properties).BeginInit();
            ((ISupportInitialize)this.w_groupBox).BeginInit();
            this.w_groupBox.SuspendLayout();
            ((ISupportInitialize)this.w_tbPassword.Properties).BeginInit();
            ((ISupportInitialize)this.w_tbDifferentUser.Properties).BeginInit();
            ((ISupportInitialize)this.w_radioButtonNewUser.Properties).BeginInit();
            ((ISupportInitialize)this.w_radioButtonCurrentUser.Properties).BeginInit();
            ((ISupportInitialize)this.w_tbServer.Properties).BeginInit();
            ((ISupportInitialize)this.w_gbTemplates).BeginInit();
            this.w_gbTemplates.SuspendLayout();
            ((ISupportInitialize)this.w_tbFailureTemplate.Properties).BeginInit();
            ((ISupportInitialize)this.w_tbSuccessTemplate.Properties).BeginInit();
            base.SuspendLayout();
            componentResourceManager.ApplyResources(this.w_btnCancel, "w_btnCancel");
            this.w_btnCancel.DialogResult = DialogResult.Cancel;
            this.w_btnCancel.Name = "w_btnCancel";
            componentResourceManager.ApplyResources(this.w_btnOK, "w_btnOK");
            this.w_btnOK.DialogResult = DialogResult.OK;
            this.w_btnOK.Name = "w_btnOK";
            this.w_btnOK.Click += new EventHandler(this.On_Ok_Clicked);
            componentResourceManager.ApplyResources(this.w_lblTo, "w_lblTo");
            this.w_lblTo.Name = "w_lblTo";
            componentResourceManager.ApplyResources(this.w_tbToEmailAddress, "w_tbToEmailAddress");
            this.w_tbToEmailAddress.Name = "w_tbToEmailAddress";
            this.w_tbToEmailAddress.Properties.ContextMenuStrip = this._textEditContextMenu;
            this._textEditContextMenu.Name = "TextEditContextMenu";
            componentResourceManager.ApplyResources(this._textEditContextMenu, "_textEditContextMenu");
            componentResourceManager.ApplyResources(this.w_tbFrom, "w_tbFrom");
            this.w_tbFrom.Name = "w_tbFrom";
            this.w_tbFrom.Properties.ContextMenuStrip = this._textEditContextMenu;
            componentResourceManager.ApplyResources(this.w_lblFrom, "w_lblFrom");
            this.w_lblFrom.Name = "w_lblFrom";
            componentResourceManager.ApplyResources(this.w_tbCC, "w_tbCC");
            this.w_tbCC.Name = "w_tbCC";
            this.w_tbCC.Properties.ContextMenuStrip = this._textEditContextMenu;
            componentResourceManager.ApplyResources(this.w_lblCC, "w_lblCC");
            this.w_lblCC.Name = "w_lblCC";
            componentResourceManager.ApplyResources(this.w_tbBCC, "w_tbBCC");
            this.w_tbBCC.Name = "w_tbBCC";
            this.w_tbBCC.Properties.ContextMenuStrip = this._textEditContextMenu;
            componentResourceManager.ApplyResources(this.w_lblBCC, "w_lblBCC");
            this.w_lblBCC.Name = "w_lblBCC";
            componentResourceManager.ApplyResources(this.w_tbSubject, "w_tbSubject");
            this.w_tbSubject.Name = "w_tbSubject";
            this.w_tbSubject.Properties.ContextMenuStrip = this._textEditContextMenu;
            componentResourceManager.ApplyResources(this.w_lblSubject, "w_lblSubject");
            this.w_lblSubject.Name = "w_lblSubject";
            componentResourceManager.ApplyResources(this.w_groupBox, "w_groupBox");
            this.w_groupBox.Controls.Add(this.w_tbPassword);
            this.w_groupBox.Controls.Add(this.w_labelPwd);
            this.w_groupBox.Controls.Add(this.w_tbDifferentUser);
            this.w_groupBox.Controls.Add(this.w_radioButtonNewUser);
            this.w_groupBox.Controls.Add(this.w_radioButtonCurrentUser);
            this.w_groupBox.Name = "w_groupBox";
            componentResourceManager.ApplyResources(this.w_tbPassword, "w_tbPassword");
            this.w_tbPassword.Name = "w_tbPassword";
            this.w_tbPassword.Properties.ContextMenuStrip = this._textEditContextMenu;
            componentResourceManager.ApplyResources(this.w_labelPwd, "w_labelPwd");
            this.w_labelPwd.Name = "w_labelPwd";
            componentResourceManager.ApplyResources(this.w_tbDifferentUser, "w_tbDifferentUser");
            this.w_tbDifferentUser.Name = "w_tbDifferentUser";
            this.w_tbDifferentUser.Properties.ContextMenuStrip = this._textEditContextMenu;
            componentResourceManager.ApplyResources(this.w_radioButtonNewUser, "w_radioButtonNewUser");
            this.w_radioButtonNewUser.Name = "w_radioButtonNewUser";
            this.w_radioButtonNewUser.Properties.Caption = componentResourceManager.GetString("w_radioButtonNewUser.Properties.Caption");
            this.w_radioButtonNewUser.Properties.CheckStyle = CheckStyles.Radio;
            this.w_radioButtonNewUser.Properties.RadioGroupIndex = 1;
            this.w_radioButtonNewUser.TabStop = false;
            this.w_radioButtonNewUser.CheckedChanged += new EventHandler(this.On_radioButtonNewUser_CheckedChanged);
            componentResourceManager.ApplyResources(this.w_radioButtonCurrentUser, "w_radioButtonCurrentUser");
            this.w_radioButtonCurrentUser.Name = "w_radioButtonCurrentUser";
            this.w_radioButtonCurrentUser.Properties.CheckStyle = CheckStyles.Radio;
            this.w_radioButtonCurrentUser.Properties.RadioGroupIndex = 1;
            this.w_radioButtonCurrentUser.CheckedChanged += new EventHandler(this.On_radioButtonNewUser_CheckedChanged);
            componentResourceManager.ApplyResources(this.w_tbServer, "w_tbServer");
            this.w_tbServer.Name = "w_tbServer";
            this.w_tbServer.Properties.ContextMenuStrip = this._textEditContextMenu;
            componentResourceManager.ApplyResources(this.w_lblServer, "w_lblServer");
            this.w_lblServer.Name = "w_lblServer";
            componentResourceManager.ApplyResources(this.w_gbTemplates, "w_gbTemplates");
            this.w_gbTemplates.Controls.Add(this.w_btnFailureTemplate);
            this.w_gbTemplates.Controls.Add(this.w_btnSuccessTemplate);
            this.w_gbTemplates.Controls.Add(this.w_tbFailureTemplate);
            this.w_gbTemplates.Controls.Add(this.label2);
            this.w_gbTemplates.Controls.Add(this.w_tbSuccessTemplate);
            this.w_gbTemplates.Controls.Add(this.label1);
            this.w_gbTemplates.Name = "w_gbTemplates";
            componentResourceManager.ApplyResources(this.w_btnFailureTemplate, "w_btnFailureTemplate");
            this.w_btnFailureTemplate.Name = "w_btnFailureTemplate";
            this.w_btnFailureTemplate.Click += new EventHandler(this.On_FailureTemplateChange_Clicked);
            componentResourceManager.ApplyResources(this.w_btnSuccessTemplate, "w_btnSuccessTemplate");
            this.w_btnSuccessTemplate.Name = "w_btnSuccessTemplate";
            this.w_btnSuccessTemplate.Click += new EventHandler(this.On_SuccessTemplateChange_Clicked);
            componentResourceManager.ApplyResources(this.w_tbFailureTemplate, "w_tbFailureTemplate");
            this.w_tbFailureTemplate.Name = "w_tbFailureTemplate";
            this.w_tbFailureTemplate.Properties.ContextMenuStrip = this._textEditContextMenu;
            componentResourceManager.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            componentResourceManager.ApplyResources(this.w_tbSuccessTemplate, "w_tbSuccessTemplate");
            this.w_tbSuccessTemplate.Name = "w_tbSuccessTemplate";
            this.w_tbSuccessTemplate.Properties.ContextMenuStrip = this._textEditContextMenu;
            componentResourceManager.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            base.AcceptButton = this.w_btnOK;
            componentResourceManager.ApplyResources(this, "$this");
            base.AutoScaleMode = AutoScaleMode.Font;
            base.CancelButton = this.w_btnCancel;
            base.Controls.Add(this.w_gbTemplates);
            base.Controls.Add(this.w_lblServer);
            base.Controls.Add(this.w_tbServer);
            base.Controls.Add(this.w_groupBox);
            base.Controls.Add(this.w_tbSubject);
            base.Controls.Add(this.w_lblSubject);
            base.Controls.Add(this.w_tbBCC);
            base.Controls.Add(this.w_lblBCC);
            base.Controls.Add(this.w_tbCC);
            base.Controls.Add(this.w_lblCC);
            base.Controls.Add(this.w_tbFrom);
            base.Controls.Add(this.w_lblFrom);
            base.Controls.Add(this.w_tbToEmailAddress);
            base.Controls.Add(this.w_lblTo);
            base.Controls.Add(this.w_btnCancel);
            base.Controls.Add(this.w_btnOK);
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "ConfigureEmailSettingsDialog";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            ((ISupportInitialize)this.w_tbToEmailAddress.Properties).EndInit();
            ((ISupportInitialize)this.w_tbFrom.Properties).EndInit();
            ((ISupportInitialize)this.w_tbCC.Properties).EndInit();
            ((ISupportInitialize)this.w_tbBCC.Properties).EndInit();
            ((ISupportInitialize)this.w_tbSubject.Properties).EndInit();
            ((ISupportInitialize)this.w_groupBox).EndInit();
            this.w_groupBox.ResumeLayout(false);
            this.w_groupBox.PerformLayout();
            ((ISupportInitialize)this.w_tbPassword.Properties).EndInit();
            ((ISupportInitialize)this.w_tbDifferentUser.Properties).EndInit();
            ((ISupportInitialize)this.w_radioButtonNewUser.Properties).EndInit();
            ((ISupportInitialize)this.w_radioButtonCurrentUser.Properties).EndInit();
            ((ISupportInitialize)this.w_tbServer.Properties).EndInit();
            ((ISupportInitialize)this.w_gbTemplates).EndInit();
            this.w_gbTemplates.ResumeLayout(false);
            this.w_gbTemplates.PerformLayout();
            ((ISupportInitialize)this.w_tbFailureTemplate.Properties).EndInit();
            ((ISupportInitialize)this.w_tbSuccessTemplate.Properties).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void LoadUI()
        {
            this.w_tbServer.Text = this.m_options.EmailServer;
            if (!string.IsNullOrEmpty(this.m_options.EmailUserName))
            {
                this.w_tbDifferentUser.Text = this.m_options.EmailUserName;
                this.w_tbPassword.Text = this.m_options.EmailPassword;
                this.w_radioButtonNewUser.Checked = true;
            }
            this.w_tbToEmailAddress.Text = this.m_options.ToEmailAddress;
            this.w_tbFrom.Text = this.m_options.FromEmailAddress;
            this.w_tbCC.Text = this.m_options.CCEmailAddress;
            this.w_tbBCC.Text = this.m_options.BCCEmailAddress;
            this.w_tbSubject.Text = this.m_options.EmailSubject;
            this.w_tbSuccessTemplate.Text = this.m_options.EmailSuccessTemplateFilePath;
            this.w_tbFailureTemplate.Text = this.m_options.EmailFailureTemplateFilePath;
        }

        private void On_FailureTemplateChange_Clicked(object sender, EventArgs e)
        {
            string str = null;
            if (this.GetFileName(out str))
            {
                this.w_tbFailureTemplate.Text = str;
            }
        }

        private void On_Ok_Clicked(object sender, EventArgs e)
        {
            if (!this.SaveUI())
            {
                base.DialogResult = DialogResult.None;
            }
        }

        private void On_radioButtonNewUser_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateUI();
        }

        private void On_SuccessTemplateChange_Clicked(object sender, EventArgs e)
        {
            string str = null;
            if (this.GetFileName(out str))
            {
                this.w_tbSuccessTemplate.Text = str;
            }
        }

        private bool SaveUI()
        {
            string str = null;
            if (string.IsNullOrEmpty(this.w_tbServer.Text))
            {
                this.w_tbServer.Focus();
                str = "Please specify an email server to use";
            }
            if (string.IsNullOrEmpty(this.w_tbFrom.Text))
            {
                this.w_tbFrom.Focus();
                str = "Please specify an email address to use as the sender";
            }
            if (string.IsNullOrEmpty(this.w_tbToEmailAddress.Text))
            {
                this.w_tbToEmailAddress.Focus();
                str = "Please specify an email address to send the message to";
            }
            if (str != null)
            {
                FlatXtraMessageBox.Show(str, "Required Input Missing", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }
            this.m_options.EmailServer = this.w_tbServer.Text;
            if (!this.w_radioButtonNewUser.Checked)
            {
                this.m_options.EmailUserName = null;
                this.m_options.EmailPassword = null;
            }
            else
            {
                this.m_options.EmailUserName = this.w_tbDifferentUser.Text;
                this.m_options.EmailPassword = this.w_tbPassword.Text;
            }
            this.m_options.ToEmailAddress = this.w_tbToEmailAddress.Text;
            this.m_options.FromEmailAddress = this.w_tbFrom.Text;
            this.m_options.CCEmailAddress = this.w_tbCC.Text;
            this.m_options.BCCEmailAddress = this.w_tbBCC.Text;
            this.m_options.EmailSubject = this.w_tbSubject.Text;
            this.m_options.EmailSuccessTemplateFilePath = this.w_tbSuccessTemplate.Text;
            this.m_options.EmailFailureTemplateFilePath = this.w_tbFailureTemplate.Text;
            return true;
        }

        private void UpdateUI()
        {
            this.w_tbDifferentUser.Enabled = this.w_radioButtonNewUser.Checked;
            this.w_tbPassword.Enabled = this.w_radioButtonNewUser.Checked;
        }
    }
}