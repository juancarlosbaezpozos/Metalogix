using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using Metalogix;
using Metalogix.Actions;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Actions.BasicView
{
    public partial class ConfigureEmailSettingsDialogBasicView : XtraForm
    {
        private SendEmailOptions _options;

        private IContainer components;

        protected SimpleButton btnCancel;

        protected SimpleButton btnOK;

        private LabelControl lblTo;

        private TextEdit tbxToEmailAddress;

        private TextEdit tbxFrom;

        private LabelControl lblFrom;

        private TextEdit tbxCC;

        private LabelControl lblCC;

        private TextEdit tbxBCC;

        private LabelControl lblBCC;

        private TextEdit tbxSubject;

        private LabelControl lblSubject;

        private GroupControl w_groupBox;

        private TextEdit tbxServer;

        private LabelControl lblServer;

        private GroupControl gbxTemplates;

        private TextEdit tbxFailureTemplate;

        private LabelControl lblFailure;

        private TextEdit tbxSuccessTemplate;

        private LabelControl lblSuccess;

        private SimpleButton btnSuccessTemplate;

        private SimpleButton btnFailureTemplate;

        private TextEditContextMenu _textEditContextMenu;

        private GroupControl groupControl1;

        private TextEdit tbxPassword;

        private LabelControl lblPassword;

        private TextEdit tbxDifferentUser;

        private LabelControl lblUsername;

        private TileControl tcConnectAs;

        private TileGroup tgCurrentUser;

        private TileItem tiCurrentUser;

        private TileGroup tgDifferentUser;

        private TileItem tiDifferentUser;

        public SendEmailOptions Options
        {
            get
            {
                return this._options;
            }
            set
            {
                this._options = value;
                this.LoadUI();
            }
        }

        public ConfigureEmailSettingsDialogBasicView()
        {
            this.InitializeComponent();
            this.tbxServer.Focus();
        }

        private void btnFailureTemplate_Click(object sender, EventArgs e)
        {
            string str;
            if (this.GetFileName(out str))
            {
                this.tbxFailureTemplate.Text = str;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!this.SaveUI())
            {
                base.DialogResult = DialogResult.None;
            }
        }

        private void btnSuccessTemplate_Click(object sender, EventArgs e)
        {
            string str;
            if (this.GetFileName(out str))
            {
                this.tbxSuccessTemplate.Text = str;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void EnableControls(bool isEnabled)
        {
            this.tiCurrentUser.Checked = !isEnabled;
            this.tiDifferentUser.Checked = isEnabled;
            this.lblUsername.Enabled = isEnabled;
            this.tbxDifferentUser.Enabled = isEnabled;
            this.lblPassword.Enabled = isEnabled;
            this.tbxPassword.Enabled = isEnabled;
        }

        private bool GetFileName(out string fileName)
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
            OpenFileDialog openFileDialog1 = openFileDialog;
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                fileName = null;
                return false;
            }
            fileName = openFileDialog1.FileName;
            return true;
        }

        private void InitializeComponent()
        {
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ConfigureEmailSettingsDialogBasicView));
            TileItemElement tileItemElement = new TileItemElement();
            TileItemElement differentUser = new TileItemElement();
            this.btnCancel = new SimpleButton();
            this.btnOK = new SimpleButton();
            this.lblTo = new LabelControl();
            this.lblFrom = new LabelControl();
            this.lblCC = new LabelControl();
            this.lblBCC = new LabelControl();
            this.lblSubject = new LabelControl();
            this.w_groupBox = new GroupControl();
            this.tcConnectAs = new TileControl();
            this.tgCurrentUser = new TileGroup();
            this.tiCurrentUser = new TileItem();
            this.tgDifferentUser = new TileGroup();
            this.tiDifferentUser = new TileItem();
            this.groupControl1 = new GroupControl();
            this.tbxPassword = new TextEdit();
            this._textEditContextMenu = new TextEditContextMenu();
            this.lblPassword = new LabelControl();
            this.tbxDifferentUser = new TextEdit();
            this.lblUsername = new LabelControl();
            this.lblServer = new LabelControl();
            this.gbxTemplates = new GroupControl();
            this.btnFailureTemplate = new SimpleButton();
            this.btnSuccessTemplate = new SimpleButton();
            this.tbxFailureTemplate = new TextEdit();
            this.lblFailure = new LabelControl();
            this.tbxSuccessTemplate = new TextEdit();
            this.lblSuccess = new LabelControl();
            this.tbxToEmailAddress = new TextEdit();
            this.tbxFrom = new TextEdit();
            this.tbxCC = new TextEdit();
            this.tbxBCC = new TextEdit();
            this.tbxSubject = new TextEdit();
            this.tbxServer = new TextEdit();
            ((ISupportInitialize)this.w_groupBox).BeginInit();
            this.w_groupBox.SuspendLayout();
            ((ISupportInitialize)this.groupControl1).BeginInit();
            this.groupControl1.SuspendLayout();
            ((ISupportInitialize)this.tbxPassword.Properties).BeginInit();
            ((ISupportInitialize)this.tbxDifferentUser.Properties).BeginInit();
            ((ISupportInitialize)this.gbxTemplates).BeginInit();
            this.gbxTemplates.SuspendLayout();
            ((ISupportInitialize)this.tbxFailureTemplate.Properties).BeginInit();
            ((ISupportInitialize)this.tbxSuccessTemplate.Properties).BeginInit();
            ((ISupportInitialize)this.tbxToEmailAddress.Properties).BeginInit();
            ((ISupportInitialize)this.tbxFrom.Properties).BeginInit();
            ((ISupportInitialize)this.tbxCC.Properties).BeginInit();
            ((ISupportInitialize)this.tbxBCC.Properties).BeginInit();
            ((ISupportInitialize)this.tbxSubject.Properties).BeginInit();
            ((ISupportInitialize)this.tbxServer.Properties).BeginInit();
            base.SuspendLayout();
            componentResourceManager.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.btnCancel.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
            this.btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnCancel.Name = "btnCancel";
            componentResourceManager.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.DialogResult = DialogResult.OK;
            this.btnOK.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
            this.btnOK.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnOK.Name = "btnOK";
            this.btnOK.Click += new EventHandler(this.btnOK_Click);
            componentResourceManager.ApplyResources(this.lblTo, "lblTo");
            this.lblTo.Name = "lblTo";
            componentResourceManager.ApplyResources(this.lblFrom, "lblFrom");
            this.lblFrom.Name = "lblFrom";
            componentResourceManager.ApplyResources(this.lblCC, "lblCC");
            this.lblCC.Name = "lblCC";
            componentResourceManager.ApplyResources(this.lblBCC, "lblBCC");
            this.lblBCC.Name = "lblBCC";
            componentResourceManager.ApplyResources(this.lblSubject, "lblSubject");
            this.lblSubject.Name = "lblSubject";
            componentResourceManager.ApplyResources(this.w_groupBox, "w_groupBox");
            this.w_groupBox.AppearanceCaption.Font = (Font)componentResourceManager.GetObject("w_groupBox.AppearanceCaption.Font");
            this.w_groupBox.AppearanceCaption.Options.UseFont = true;
            this.w_groupBox.Controls.Add(this.tcConnectAs);
            this.w_groupBox.Controls.Add(this.groupControl1);
            this.w_groupBox.Name = "w_groupBox";
            this.tcConnectAs.AllowDrag = false;
            this.tcConnectAs.AllowDragTilesBetweenGroups = false;
            this.tcConnectAs.AppearanceGroupHighlighting.HoveredMaskColor = Color.FromArgb(98, 181, 229);
            this.tcConnectAs.AppearanceItem.Hovered.BackColor = (Color)componentResourceManager.GetObject("tcConnectAs.AppearanceItem.Hovered.BackColor");
            this.tcConnectAs.AppearanceItem.Hovered.FontStyleDelta = (FontStyle)componentResourceManager.GetObject("tcConnectAs.AppearanceItem.Hovered.FontStyleDelta");
            this.tcConnectAs.AppearanceItem.Hovered.Options.UseBackColor = true;
            this.tcConnectAs.AppearanceItem.Hovered.Options.UseFont = true;
            this.tcConnectAs.AppearanceItem.Normal.BackColor = (Color)componentResourceManager.GetObject("tcConnectAs.AppearanceItem.Normal.BackColor");
            this.tcConnectAs.AppearanceItem.Normal.FontStyleDelta = (FontStyle)componentResourceManager.GetObject("tcConnectAs.AppearanceItem.Normal.FontStyleDelta");
            this.tcConnectAs.AppearanceItem.Normal.Options.UseBackColor = true;
            this.tcConnectAs.AppearanceItem.Normal.Options.UseFont = true;
            this.tcConnectAs.Cursor = Cursors.Default;
            componentResourceManager.ApplyResources(this.tcConnectAs, "tcConnectAs");
            this.tcConnectAs.DragSize = new Size(0, 0);
            this.tcConnectAs.Groups.Add(this.tgCurrentUser);
            this.tcConnectAs.Groups.Add(this.tgDifferentUser);
            this.tcConnectAs.IndentBetweenGroups = 40;
            this.tcConnectAs.IndentBetweenItems = 30;
            this.tcConnectAs.ItemContentAnimation = TileItemContentAnimationType.Fade;
            this.tcConnectAs.ItemSize = 50;
            this.tcConnectAs.MaxId = 4;
            this.tcConnectAs.Name = "tcConnectAs";
            this.tgCurrentUser.Items.Add(this.tiCurrentUser);
            this.tgCurrentUser.Name = "tgCurrentUser";
            this.tiCurrentUser.Checked = true;
            tileItemElement.Image = Resources.Current_User;
            tileItemElement.ImageScaleMode = TileItemImageScaleMode.ZoomInside;
            tileItemElement.ImageToTextAlignment = TileControlImageToTextAlignment.Left;
            componentResourceManager.ApplyResources(tileItemElement, "tileItemElement3");
            tileItemElement.TextAlignment = TileItemContentAlignment.BottomLeft;
            this.tiCurrentUser.Elements.Add(tileItemElement);
            this.tiCurrentUser.Id = 2;
            this.tiCurrentUser.ItemSize = TileItemSize.Wide;
            this.tiCurrentUser.Name = "tiCurrentUser";
            this.tiCurrentUser.ItemClick += new TileItemClickEventHandler(this.tiCurrentUser_ItemClick);
            this.tgDifferentUser.Items.Add(this.tiDifferentUser);
            this.tgDifferentUser.Name = "tgDifferentUser";
            differentUser.Image = Resources.Different_User;
            differentUser.ImageScaleMode = TileItemImageScaleMode.ZoomInside;
            differentUser.ImageToTextAlignment = TileControlImageToTextAlignment.Left;
            componentResourceManager.ApplyResources(differentUser, "tileItemElement4");
            differentUser.TextAlignment = TileItemContentAlignment.BottomLeft;
            this.tiDifferentUser.Elements.Add(differentUser);
            this.tiDifferentUser.Id = 3;
            this.tiDifferentUser.ItemSize = TileItemSize.Wide;
            this.tiDifferentUser.Name = "tiDifferentUser";
            this.tiDifferentUser.ItemClick += new TileItemClickEventHandler(this.tiDifferentUser_ItemClick);
            componentResourceManager.ApplyResources(this.groupControl1, "groupControl1");
            this.groupControl1.AppearanceCaption.Font = (Font)componentResourceManager.GetObject("groupControl1.AppearanceCaption.Font");
            this.groupControl1.AppearanceCaption.Options.UseFont = true;
            this.groupControl1.Controls.Add(this.tbxPassword);
            this.groupControl1.Controls.Add(this.lblPassword);
            this.groupControl1.Controls.Add(this.tbxDifferentUser);
            this.groupControl1.Controls.Add(this.lblUsername);
            this.groupControl1.Name = "groupControl1";
            componentResourceManager.ApplyResources(this.tbxPassword, "tbxPassword");
            this.tbxPassword.Name = "tbxPassword";
            this.tbxPassword.Properties.ContextMenuStrip = this._textEditContextMenu;
            this._textEditContextMenu.Name = "TextEditContextMenu";
            componentResourceManager.ApplyResources(this._textEditContextMenu, "_textEditContextMenu");
            componentResourceManager.ApplyResources(this.lblPassword, "lblPassword");
            this.lblPassword.Name = "lblPassword";
            componentResourceManager.ApplyResources(this.tbxDifferentUser, "tbxDifferentUser");
            this.tbxDifferentUser.Name = "tbxDifferentUser";
            this.tbxDifferentUser.Properties.ContextMenuStrip = this._textEditContextMenu;
            componentResourceManager.ApplyResources(this.lblUsername, "lblUsername");
            this.lblUsername.Name = "lblUsername";
            componentResourceManager.ApplyResources(this.lblServer, "lblServer");
            this.lblServer.Name = "lblServer";
            componentResourceManager.ApplyResources(this.gbxTemplates, "gbxTemplates");
            this.gbxTemplates.AppearanceCaption.Font = (Font)componentResourceManager.GetObject("gbxTemplates.AppearanceCaption.Font");
            this.gbxTemplates.AppearanceCaption.Options.UseFont = true;
            this.gbxTemplates.Controls.Add(this.btnFailureTemplate);
            this.gbxTemplates.Controls.Add(this.btnSuccessTemplate);
            this.gbxTemplates.Controls.Add(this.tbxFailureTemplate);
            this.gbxTemplates.Controls.Add(this.lblFailure);
            this.gbxTemplates.Controls.Add(this.tbxSuccessTemplate);
            this.gbxTemplates.Controls.Add(this.lblSuccess);
            this.gbxTemplates.Name = "gbxTemplates";
            componentResourceManager.ApplyResources(this.btnFailureTemplate, "btnFailureTemplate");
            this.btnFailureTemplate.Name = "btnFailureTemplate";
            this.btnFailureTemplate.Click += new EventHandler(this.btnFailureTemplate_Click);
            componentResourceManager.ApplyResources(this.btnSuccessTemplate, "btnSuccessTemplate");
            this.btnSuccessTemplate.Name = "btnSuccessTemplate";
            this.btnSuccessTemplate.Click += new EventHandler(this.btnSuccessTemplate_Click);
            componentResourceManager.ApplyResources(this.tbxFailureTemplate, "tbxFailureTemplate");
            this.tbxFailureTemplate.Name = "tbxFailureTemplate";
            this.tbxFailureTemplate.Properties.ContextMenuStrip = this._textEditContextMenu;
            componentResourceManager.ApplyResources(this.lblFailure, "lblFailure");
            this.lblFailure.Name = "lblFailure";
            componentResourceManager.ApplyResources(this.tbxSuccessTemplate, "tbxSuccessTemplate");
            this.tbxSuccessTemplate.Name = "tbxSuccessTemplate";
            this.tbxSuccessTemplate.Properties.ContextMenuStrip = this._textEditContextMenu;
            componentResourceManager.ApplyResources(this.lblSuccess, "lblSuccess");
            this.lblSuccess.Name = "lblSuccess";
            componentResourceManager.ApplyResources(this.tbxToEmailAddress, "tbxToEmailAddress");
            this.tbxToEmailAddress.Name = "tbxToEmailAddress";
            this.tbxToEmailAddress.Properties.ContextMenuStrip = this._textEditContextMenu;
            componentResourceManager.ApplyResources(this.tbxFrom, "tbxFrom");
            this.tbxFrom.Name = "tbxFrom";
            this.tbxFrom.Properties.ContextMenuStrip = this._textEditContextMenu;
            componentResourceManager.ApplyResources(this.tbxCC, "tbxCC");
            this.tbxCC.Name = "tbxCC";
            this.tbxCC.Properties.ContextMenuStrip = this._textEditContextMenu;
            componentResourceManager.ApplyResources(this.tbxBCC, "tbxBCC");
            this.tbxBCC.Name = "tbxBCC";
            this.tbxBCC.Properties.ContextMenuStrip = this._textEditContextMenu;
            componentResourceManager.ApplyResources(this.tbxSubject, "tbxSubject");
            this.tbxSubject.Name = "tbxSubject";
            this.tbxSubject.Properties.ContextMenuStrip = this._textEditContextMenu;
            componentResourceManager.ApplyResources(this.tbxServer, "tbxServer");
            this.tbxServer.Name = "tbxServer";
            this.tbxServer.Properties.ContextMenuStrip = this._textEditContextMenu;
            base.AcceptButton = this.btnOK;
            componentResourceManager.ApplyResources(this, "$this");
            base.AutoScaleMode = AutoScaleMode.Font;
            base.CancelButton = this.btnCancel;
            base.Controls.Add(this.gbxTemplates);
            base.Controls.Add(this.lblServer);
            base.Controls.Add(this.tbxServer);
            base.Controls.Add(this.w_groupBox);
            base.Controls.Add(this.tbxSubject);
            base.Controls.Add(this.lblSubject);
            base.Controls.Add(this.tbxBCC);
            base.Controls.Add(this.lblBCC);
            base.Controls.Add(this.tbxCC);
            base.Controls.Add(this.lblCC);
            base.Controls.Add(this.tbxFrom);
            base.Controls.Add(this.lblFrom);
            base.Controls.Add(this.tbxToEmailAddress);
            base.Controls.Add(this.lblTo);
            base.Controls.Add(this.btnCancel);
            base.Controls.Add(this.btnOK);
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "ConfigureEmailSettingsDialogBasicView";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            base.SizeGripStyle = SizeGripStyle.Hide;
            ((ISupportInitialize)this.w_groupBox).EndInit();
            this.w_groupBox.ResumeLayout(false);
            ((ISupportInitialize)this.groupControl1).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            ((ISupportInitialize)this.tbxPassword.Properties).EndInit();
            ((ISupportInitialize)this.tbxDifferentUser.Properties).EndInit();
            ((ISupportInitialize)this.gbxTemplates).EndInit();
            this.gbxTemplates.ResumeLayout(false);
            this.gbxTemplates.PerformLayout();
            ((ISupportInitialize)this.tbxFailureTemplate.Properties).EndInit();
            ((ISupportInitialize)this.tbxSuccessTemplate.Properties).EndInit();
            ((ISupportInitialize)this.tbxToEmailAddress.Properties).EndInit();
            ((ISupportInitialize)this.tbxFrom.Properties).EndInit();
            ((ISupportInitialize)this.tbxCC.Properties).EndInit();
            ((ISupportInitialize)this.tbxBCC.Properties).EndInit();
            ((ISupportInitialize)this.tbxSubject.Properties).EndInit();
            ((ISupportInitialize)this.tbxServer.Properties).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void LoadUI()
        {
            this.tbxServer.Text = this._options.EmailServer;
            if (!string.IsNullOrEmpty(this._options.EmailUserName))
            {
                this.tbxDifferentUser.Text = this._options.EmailUserName;
                this.lblUsername.Enabled = true;
                this.tbxDifferentUser.Enabled = true;
                this.tbxPassword.Text = this._options.EmailPassword;
                this.tbxPassword.Enabled = true;
                this.lblPassword.Enabled = true;
                this.tiDifferentUser.Checked = true;
                this.tiCurrentUser.Checked = false;
            }
            this.tbxToEmailAddress.Text = this._options.ToEmailAddress;
            this.tbxFrom.Text = this._options.FromEmailAddress;
            this.tbxCC.Text = this._options.CCEmailAddress;
            this.tbxBCC.Text = this._options.BCCEmailAddress;
            this.tbxSubject.Text = this._options.EmailSubject;
            this.tbxSuccessTemplate.Text = this._options.EmailSuccessTemplateFilePath;
            this.tbxFailureTemplate.Text = this._options.EmailFailureTemplateFilePath;
        }

        private bool SaveUI()
        {
            string str = null;
            if (string.IsNullOrEmpty(this.tbxServer.Text.Trim()))
            {
                this.tbxServer.Focus();
                str = "Please specify an email server to use";
            }
            if (this.tiDifferentUser.Checked)
            {
                if (string.IsNullOrEmpty(str) && string.IsNullOrEmpty(this.tbxDifferentUser.Text.Trim()))
                {
                    this.tbxDifferentUser.Focus();
                    str = "Please specify username";
                }
                if (string.IsNullOrEmpty(str) && string.IsNullOrEmpty(this.tbxPassword.Text))
                {
                    this.tbxPassword.Focus();
                    str = "Please specify password";
                }
            }
            if (string.IsNullOrEmpty(str) && string.IsNullOrEmpty(this.tbxFrom.Text.Trim()))
            {
                this.tbxFrom.Focus();
                str = "Please specify an email address to use as the sender";
            }
            if (string.IsNullOrEmpty(str) && string.IsNullOrEmpty(this.tbxToEmailAddress.Text.Trim()))
            {
                this.tbxToEmailAddress.Focus();
                str = "Please specify an email address to send the message to";
            }
            if (!string.IsNullOrEmpty(str))
            {
                FlatXtraMessageBox.Show(str, "Required Input Missing", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }
            this._options.EmailServer = this.tbxServer.Text;
            if (!this.tiDifferentUser.Checked)
            {
                this._options.EmailUserName = null;
                this._options.EmailPassword = null;
            }
            else
            {
                this._options.EmailUserName = this.tbxDifferentUser.Text;
                this._options.EmailPassword = this.tbxPassword.Text;
            }
            this._options.ToEmailAddress = this.tbxToEmailAddress.Text;
            this._options.FromEmailAddress = this.tbxFrom.Text;
            this._options.CCEmailAddress = this.tbxCC.Text;
            this._options.BCCEmailAddress = this.tbxBCC.Text;
            this._options.EmailSubject = this.tbxSubject.Text;
            this._options.EmailSuccessTemplateFilePath = this.tbxSuccessTemplate.Text;
            this._options.EmailFailureTemplateFilePath = this.tbxFailureTemplate.Text;
            return true;
        }

        private void tiCurrentUser_ItemClick(object sender, TileItemEventArgs e)
        {
            if (this.tiCurrentUser.Checked)
            {
                return;
            }
            this.EnableControls(false);
        }

        private void tiDifferentUser_ItemClick(object sender, TileItemEventArgs e)
        {
            if (this.tiDifferentUser.Checked)
            {
                return;
            }
            this.EnableControls(true);
        }
    }
}