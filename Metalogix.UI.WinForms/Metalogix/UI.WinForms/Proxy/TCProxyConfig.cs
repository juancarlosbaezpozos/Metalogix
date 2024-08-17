using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using Metalogix.Connectivity.Proxy;
using Metalogix.Permissions;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Components.AnchoredControls;
using Metalogix.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Security.Principal;
using System.Windows.Forms;
using TooltipsTest;

namespace Metalogix.UI.WinForms.Proxy
{
    [ControlImage("Metalogix.UI.WinForms.Icons.ProxyOptions.png")]
    [ControlName("Proxy Options")]
    [UsesGroupBox(true)]
    public partial class TCProxyConfig : TabbableControl
    {
        private IProxyOptionsContainer m_options;

        private bool m_bShowEnabled = true;

        private IContainer components;

        private PanelControl w_plEnabled;

        protected CheckEdit w_cbProxy;

        protected GroupControl w_credentialGroupBox;

        protected TextEdit w_txtPassword;

        private LabelControl w_labelPwd;

        protected TextEdit w_txtDifferentUser;

        protected CheckEdit w_radioButtonNewUser;

        protected CheckEdit w_radioButtonCurrentUser;

        protected TextEdit textBoxPort;

        private LabelControl label4;

        protected TextEdit textBoxServer;

        private LabelControl lblKey;

        private HelpTipButton w_helpProxy;

        public IProxyOptionsContainer Options
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

        public MLProxy Proxy
        {
            get
            {
                MLProxy mLProxy = new MLProxy()
                {
                    Enabled = this.w_cbProxy.Checked,
                    Server = this.textBoxServer.Text,
                    Port = this.textBoxPort.Text,
                    Credentials = (this.w_radioButtonNewUser.Checked ? new Credentials(this.w_txtDifferentUser.Text, this.w_txtPassword.Text.ToSecureString(), true) : new Credentials())
                };
                return mLProxy;
            }
            set
            {
                this.UpdateProxyUI(value);
            }
        }

        public bool ShowEnabled
        {
            get
            {
                return this.m_bShowEnabled;
            }
            set
            {
                if (this.m_bShowEnabled && !value)
                {
                    base.HideControl(this.w_plEnabled);
                    base.HideControl(this.w_plEnabled);
                }
                else if (!this.m_bShowEnabled && value)
                {
                    base.ShowControl(this.w_plEnabled, this);
                }
                this.m_bShowEnabled = value;
            }
        }

        public TCProxyConfig()
        {
            this.InitializeComponent();
            this.w_cbProxy.Checked = (this.textBoxServer.Enabled = (this.textBoxPort.Enabled = (this.w_credentialGroupBox.Enabled = false)));
            System.Type type = base.GetType();
            this.w_helpProxy.SetResourceString(type.FullName + this.w_cbProxy.Name, type);
        }

        private void checkBoxProxy_CheckedChanged(object sender, EventArgs e)
        {
            bool flag;
            TextEdit textEdit = this.textBoxServer;
            TextEdit textEdit1 = this.textBoxPort;
            GroupControl wCredentialGroupBox = this.w_credentialGroupBox;
            bool @checked = ((CheckEdit)sender).Checked;
            bool flag1 = @checked;
            wCredentialGroupBox.Enabled = @checked;
            bool flag2 = flag1;
            bool flag3 = flag2;
            textEdit1.Enabled = flag2;
            textEdit.Enabled = flag3;
            TextEdit wTxtDifferentUser = this.w_txtDifferentUser;
            TextEdit wTxtPassword = this.w_txtPassword;
            flag = (!this.w_radioButtonNewUser.Checked ? false : this.w_radioButtonNewUser.Enabled);
            bool flag4 = flag;
            wTxtPassword.Enabled = flag;
            wTxtDifferentUser.Enabled = flag4;
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
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(TCProxyConfig));
            this.w_plEnabled = new PanelControl();
            this.w_helpProxy = new HelpTipButton();
            this.w_cbProxy = new CheckEdit();
            this.w_credentialGroupBox = new GroupControl();
            this.w_txtPassword = new TextEdit();
            this.w_labelPwd = new LabelControl();
            this.w_txtDifferentUser = new TextEdit();
            this.w_radioButtonNewUser = new CheckEdit();
            this.w_radioButtonCurrentUser = new CheckEdit();
            this.textBoxPort = new TextEdit();
            this.label4 = new LabelControl();
            this.textBoxServer = new TextEdit();
            this.lblKey = new LabelControl();
            ((ISupportInitialize)this.w_plEnabled).BeginInit();
            this.w_plEnabled.SuspendLayout();
            ((ISupportInitialize)this.w_helpProxy).BeginInit();
            ((ISupportInitialize)this.w_cbProxy.Properties).BeginInit();
            ((ISupportInitialize)this.w_credentialGroupBox).BeginInit();
            this.w_credentialGroupBox.SuspendLayout();
            ((ISupportInitialize)this.w_txtPassword.Properties).BeginInit();
            ((ISupportInitialize)this.w_txtDifferentUser.Properties).BeginInit();
            ((ISupportInitialize)this.w_radioButtonNewUser.Properties).BeginInit();
            ((ISupportInitialize)this.w_radioButtonCurrentUser.Properties).BeginInit();
            ((ISupportInitialize)this.textBoxPort.Properties).BeginInit();
            ((ISupportInitialize)this.textBoxServer.Properties).BeginInit();
            base.SuspendLayout();
            componentResourceManager.ApplyResources(this.w_plEnabled, "w_plEnabled");
            this.w_plEnabled.BorderStyle = BorderStyles.NoBorder;
            this.w_plEnabled.Controls.Add(this.w_helpProxy);
            this.w_plEnabled.Controls.Add(this.w_cbProxy);
            this.w_plEnabled.Name = "w_plEnabled";
            this.w_helpProxy.AnchoringControl = this.w_cbProxy;
            this.w_helpProxy.BackColor = Color.Transparent;
            this.w_helpProxy.CommonParentControl = null;
            componentResourceManager.ApplyResources(this.w_helpProxy, "w_helpProxy");
            this.w_helpProxy.Name = "w_helpProxy";
            this.w_helpProxy.TabStop = false;
            componentResourceManager.ApplyResources(this.w_cbProxy, "w_cbProxy");
            this.w_cbProxy.Name = "w_cbProxy";
            this.w_cbProxy.Properties.Appearance.BackColor = (Color)componentResourceManager.GetObject("w_cbProxy.Properties.Appearance.BackColor");
            this.w_cbProxy.Properties.Appearance.Options.UseBackColor = true;
            this.w_cbProxy.Properties.Caption = componentResourceManager.GetString("w_cbProxy.Properties.Caption");
            this.w_cbProxy.CheckedChanged += new EventHandler(this.checkBoxProxy_CheckedChanged);
            componentResourceManager.ApplyResources(this.w_credentialGroupBox, "w_credentialGroupBox");
            this.w_credentialGroupBox.Controls.Add(this.w_txtPassword);
            this.w_credentialGroupBox.Controls.Add(this.w_labelPwd);
            this.w_credentialGroupBox.Controls.Add(this.w_txtDifferentUser);
            this.w_credentialGroupBox.Controls.Add(this.w_radioButtonNewUser);
            this.w_credentialGroupBox.Controls.Add(this.w_radioButtonCurrentUser);
            this.w_credentialGroupBox.Name = "w_credentialGroupBox";
            componentResourceManager.ApplyResources(this.w_txtPassword, "w_txtPassword");
            this.w_txtPassword.Name = "w_txtPassword";
            this.w_txtPassword.Properties.Appearance.BackColor = (Color)componentResourceManager.GetObject("w_txtPassword.Properties.Appearance.BackColor");
            this.w_txtPassword.Properties.Appearance.Options.UseBackColor = true;
            this.w_txtPassword.Properties.PasswordChar = '*';
            componentResourceManager.ApplyResources(this.w_labelPwd, "w_labelPwd");
            this.w_labelPwd.Name = "w_labelPwd";
            componentResourceManager.ApplyResources(this.w_txtDifferentUser, "w_txtDifferentUser");
            this.w_txtDifferentUser.Name = "w_txtDifferentUser";
            this.w_txtDifferentUser.Properties.Appearance.BackColor = (Color)componentResourceManager.GetObject("w_txtDifferentUser.Properties.Appearance.BackColor");
            this.w_txtDifferentUser.Properties.Appearance.Options.UseBackColor = true;
            componentResourceManager.ApplyResources(this.w_radioButtonNewUser, "w_radioButtonNewUser");
            this.w_radioButtonNewUser.Name = "w_radioButtonNewUser";
            this.w_radioButtonNewUser.Properties.Caption = componentResourceManager.GetString("w_radioButtonNewUser.Properties.Caption");
            this.w_radioButtonNewUser.Properties.CheckStyle = CheckStyles.Radio;
            this.w_radioButtonNewUser.Properties.RadioGroupIndex = 1;
            this.w_radioButtonNewUser.TabStop = false;
            this.w_radioButtonNewUser.CheckedChanged += new EventHandler(this.On_DefaultUser_CheckedChanged);
            componentResourceManager.ApplyResources(this.w_radioButtonCurrentUser, "w_radioButtonCurrentUser");
            this.w_radioButtonCurrentUser.Name = "w_radioButtonCurrentUser";
            this.w_radioButtonCurrentUser.Properties.CheckStyle = CheckStyles.Radio;
            this.w_radioButtonCurrentUser.Properties.RadioGroupIndex = 1;
            this.w_radioButtonCurrentUser.CheckedChanged += new EventHandler(this.On_DefaultUser_CheckedChanged);
            componentResourceManager.ApplyResources(this.textBoxPort, "textBoxPort");
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Properties.Appearance.BackColor = (Color)componentResourceManager.GetObject("textBoxPort.Properties.Appearance.BackColor");
            this.textBoxPort.Properties.Appearance.Options.UseBackColor = true;
            componentResourceManager.ApplyResources(this.label4, "label4");
            this.label4.Appearance.BackColor = (Color)componentResourceManager.GetObject("label4.Appearance.BackColor");
            this.label4.Name = "label4";
            componentResourceManager.ApplyResources(this.textBoxServer, "textBoxServer");
            this.textBoxServer.Name = "textBoxServer";
            this.textBoxServer.Properties.Appearance.BackColor = (Color)componentResourceManager.GetObject("textBoxServer.Properties.Appearance.BackColor");
            this.textBoxServer.Properties.Appearance.Options.UseBackColor = true;
            this.lblKey.Appearance.BackColor = (Color)componentResourceManager.GetObject("lblKey.Appearance.BackColor");
            componentResourceManager.ApplyResources(this.lblKey, "lblKey");
            this.lblKey.Name = "lblKey";
            componentResourceManager.ApplyResources(this, "$this");
            base.AutoScaleMode = AutoScaleMode.Font;
            base.Controls.Add(this.w_credentialGroupBox);
            base.Controls.Add(this.w_plEnabled);
            base.Controls.Add(this.label4);
            base.Controls.Add(this.textBoxPort);
            base.Controls.Add(this.lblKey);
            base.Controls.Add(this.textBoxServer);
            this.MaximumSize = new Size(1000, 179);
            this.MinimumSize = new Size(267, 179);
            base.Name = "TCProxyConfig";
            base.Load += new EventHandler(this.On_Load);
            ((ISupportInitialize)this.w_plEnabled).EndInit();
            this.w_plEnabled.ResumeLayout(false);
            ((ISupportInitialize)this.w_helpProxy).EndInit();
            ((ISupportInitialize)this.w_cbProxy.Properties).EndInit();
            ((ISupportInitialize)this.w_credentialGroupBox).EndInit();
            this.w_credentialGroupBox.ResumeLayout(false);
            this.w_credentialGroupBox.PerformLayout();
            ((ISupportInitialize)this.w_txtPassword.Properties).EndInit();
            ((ISupportInitialize)this.w_txtDifferentUser.Properties).EndInit();
            ((ISupportInitialize)this.w_radioButtonNewUser.Properties).EndInit();
            ((ISupportInitialize)this.w_radioButtonCurrentUser.Properties).EndInit();
            ((ISupportInitialize)this.textBoxPort.Properties).EndInit();
            ((ISupportInitialize)this.textBoxServer.Properties).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        protected override void LoadUI()
        {
            if (this.Options != null)
            {
                this.Proxy = this.Options.Proxy;
            }
        }

        private void On_DefaultUser_CheckedChanged(object sender, EventArgs e)
        {
            bool flag;
            TextEdit wTxtDifferentUser = this.w_txtDifferentUser;
            TextEdit wTxtPassword = this.w_txtPassword;
            flag = (!this.w_radioButtonNewUser.Checked ? false : this.w_radioButtonNewUser.Enabled);
            bool flag1 = flag;
            wTxtPassword.Enabled = flag;
            wTxtDifferentUser.Enabled = flag1;
        }

        private void On_Load(object sender, EventArgs e)
        {
            this.w_radioButtonCurrentUser.Text = WindowsIdentity.GetCurrent().Name;
        }

        public override bool SaveUI()
        {
            string str;
            if (this.ValidateInput(out str))
            {
                this.Options.Proxy = this.Proxy;
                return true;
            }
            if (!string.IsNullOrEmpty(str))
            {
                FlatXtraMessageBox.Show(this, str, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            return false;
        }

        private void UpdateProxyUI(MLProxy proxy)
        {
            if (proxy == null)
            {
                return;
            }
            CheckEdit wCbProxy = this.w_cbProxy;
            TextEdit textEdit = this.textBoxServer;
            TextEdit textEdit1 = this.textBoxPort;
            GroupControl wCredentialGroupBox = this.w_credentialGroupBox;
            bool enabled = proxy.Enabled;
            bool flag = enabled;
            wCredentialGroupBox.Enabled = enabled;
            bool flag1 = flag;
            bool flag2 = flag1;
            textEdit1.Enabled = flag1;
            bool flag3 = flag2;
            bool flag4 = flag3;
            textEdit.Enabled = flag3;
            wCbProxy.Checked = flag4;
            this.textBoxServer.Text = proxy.Server;
            this.textBoxPort.Text = proxy.Port;
            if (proxy.Credentials == null || proxy.Credentials.IsDefault)
            {
                this.w_radioButtonCurrentUser.Checked = true;
                return;
            }
            this.w_radioButtonNewUser.Checked = true;
            this.w_txtDifferentUser.Text = proxy.Credentials.UserName;
            this.w_txtPassword.Text = proxy.Credentials.Password.ToInsecureString();
        }

        public virtual bool ValidateInput(out string sWarningMessage)
        {
            sWarningMessage = null;
            MLProxy proxy = this.Proxy;
            if (proxy != null && proxy.Enabled)
            {
                if (string.IsNullOrEmpty(proxy.Server))
                {
                    sWarningMessage = "The proxy server name must be specified.";
                    return false;
                }
                if (!string.IsNullOrEmpty(proxy.Port))
                {
                    int num = 0;
                    if (!int.TryParse(proxy.Port, out num))
                    {
                        sWarningMessage = "The proxy port must be a number.";
                        return false;
                    }
                }
            }
            return true;
        }
    }
}