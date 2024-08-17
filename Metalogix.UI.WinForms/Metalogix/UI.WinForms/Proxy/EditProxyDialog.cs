using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTab;
using Metalogix.Connectivity.Proxy;
using Metalogix.Utilities;

namespace Metalogix.UI.WinForms.Proxy
{
    public class EditProxyDialog : XtraForm
    {
        private EditProxyOptions _options;

        private IContainer components;

        private SimpleButton buttonCancel;

        private SimpleButton buttonOK;

        private XtraTabControl tabControl1;

        private XtraTabPage tabPage1;

        private XtraTabPage tabPage2;

        private LabelControl label2;

        private LabelControl label1;

        private TextEdit w_textBoxPassword;

        private TextEdit w_textBoxUserName;

        private CheckEdit w_radioButtonDiffCred;

        private CheckEdit w_radioButtonDefaultCred;

        private CheckEdit w_checkBoxBypassAddress;

        private CheckEdit w_checkBoxBypassLocal;

        private EditProxyAddressControl w_editProxyAddressControl;

        private LabelControl label3;

        private BarManager _barManager;

        private Bar bar1;

        private Bar _barProxy;

        private BarStaticItem _barLabelProxyServerAddress;

        private BarEditItem _barTextEditProxyServerAddress;

        private RepositoryItemTextEdit repositoryItemTextEdit1;

        private BarDockControl barDockControlTop;

        private BarDockControl barDockControlBottom;

        private BarDockControl barDockControlLeft;

        private BarDockControl barDockControlRight;

        public EditProxyOptions Options
        {
            get
		{
			if (_options != null)
			{
				_options.ProxyServerAddress = ((_barTextEditProxyServerAddress.EditValue != null) ? _barTextEditProxyServerAddress.EditValue.ToString() : string.Empty);
				_options.UseDefaultCredentials = w_radioButtonDefaultCred.Checked;
				_options.UserName = w_textBoxUserName.Text;
				_options.Password = w_textBoxPassword.Text.ToSecureString();
				_options.SavePassword = true;
				_options.BypassProxyOnLocal = w_checkBoxBypassLocal.Checked;
				_options.BypassProxyOnAddress = w_checkBoxBypassAddress.Checked;
				_options.BypassAddresses = w_editProxyAddressControl.Options;
			}
			return _options;
		}
            set
		{
			_options = value;
			if (_options != null)
			{
				_barTextEditProxyServerAddress.EditValue = _options.ProxyServerAddress;
				w_textBoxUserName.Text = _options.UserName;
				w_textBoxPassword.Text = _options.Password.ToInsecureString();
				w_checkBoxBypassLocal.Checked = _options.BypassProxyOnLocal;
				w_checkBoxBypassAddress.Checked = _options.BypassProxyOnAddress;
				w_radioButtonDefaultCred.Checked = _options.UseDefaultCredentials;
				w_radioButtonDiffCred.Checked = !_options.UseDefaultCredentials;
				w_editProxyAddressControl.Options = _options.BypassAddresses;
			}
		}
        }

        public EditProxyDialog()
	{
		InitializeComponent();
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
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.Proxy.EditProxyDialog));
		this.buttonCancel = new DevExpress.XtraEditors.SimpleButton();
		this.buttonOK = new DevExpress.XtraEditors.SimpleButton();
		this.tabControl1 = new DevExpress.XtraTab.XtraTabControl();
		this.tabPage2 = new DevExpress.XtraTab.XtraTabPage();
		this.w_editProxyAddressControl = new Metalogix.UI.WinForms.Proxy.EditProxyAddressControl();
		this.w_checkBoxBypassAddress = new DevExpress.XtraEditors.CheckEdit();
		this.w_checkBoxBypassLocal = new DevExpress.XtraEditors.CheckEdit();
		this.tabPage1 = new DevExpress.XtraTab.XtraTabPage();
		this.label3 = new DevExpress.XtraEditors.LabelControl();
		this.w_textBoxPassword = new DevExpress.XtraEditors.TextEdit();
		this.w_radioButtonDefaultCred = new DevExpress.XtraEditors.CheckEdit();
		this.w_textBoxUserName = new DevExpress.XtraEditors.TextEdit();
		this.label2 = new DevExpress.XtraEditors.LabelControl();
		this.label1 = new DevExpress.XtraEditors.LabelControl();
		this.w_radioButtonDiffCred = new DevExpress.XtraEditors.CheckEdit();
		this._barManager = new DevExpress.XtraBars.BarManager();
		this.bar1 = new DevExpress.XtraBars.Bar();
		this._barProxy = new DevExpress.XtraBars.Bar();
		this._barLabelProxyServerAddress = new DevExpress.XtraBars.BarStaticItem();
		this._barTextEditProxyServerAddress = new DevExpress.XtraBars.BarEditItem();
		this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		((System.ComponentModel.ISupportInitialize)this.tabControl1).BeginInit();
		this.tabControl1.SuspendLayout();
		this.tabPage2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.w_checkBoxBypassAddress.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.w_checkBoxBypassLocal.Properties).BeginInit();
		this.tabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.w_textBoxPassword.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.w_radioButtonDefaultCred.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.w_textBoxUserName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.w_radioButtonDiffCred.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this._barManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).BeginInit();
		base.SuspendLayout();
		this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.buttonCancel.Location = new System.Drawing.Point(354, 271);
		this.buttonCancel.Name = "buttonCancel";
		this.buttonCancel.Size = new System.Drawing.Size(75, 23);
		this.buttonCancel.TabIndex = 2;
		this.buttonCancel.Text = "Cancel";
		this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.buttonOK.Location = new System.Drawing.Point(273, 271);
		this.buttonOK.Name = "buttonOK";
		this.buttonOK.Size = new System.Drawing.Size(75, 23);
		this.buttonOK.TabIndex = 1;
		this.buttonOK.Text = "Save";
		this.tabControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tabControl1.Location = new System.Drawing.Point(0, 28);
		this.tabControl1.Name = "tabControl1";
		this.tabControl1.SelectedTabPage = this.tabPage2;
		this.tabControl1.Size = new System.Drawing.Size(441, 237);
		this.tabControl1.TabIndex = 0;
		DevExpress.XtraTab.XtraTabPageCollection tabPages = this.tabControl1.TabPages;
		DevExpress.XtraTab.XtraTabPage[] xtraTabPageArray = new DevExpress.XtraTab.XtraTabPage[2] { this.tabPage1, this.tabPage2 };
		tabPages.AddRange(xtraTabPageArray);
		this.tabPage2.Controls.Add(this.w_editProxyAddressControl);
		this.tabPage2.Controls.Add(this.w_checkBoxBypassAddress);
		this.tabPage2.Controls.Add(this.w_checkBoxBypassLocal);
		this.tabPage2.Name = "tabPage2";
		this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage2.Size = new System.Drawing.Size(435, 209);
		this.tabPage2.Text = "Bypass Proxy Settings";
		this.w_editProxyAddressControl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.w_editProxyAddressControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.w_editProxyAddressControl.Enabled = false;
		this.w_editProxyAddressControl.Location = new System.Drawing.Point(28, 52);
		this.w_editProxyAddressControl.Name = "w_editProxyAddressControl";
		this.w_editProxyAddressControl.Options = null;
		this.w_editProxyAddressControl.Size = new System.Drawing.Size(378, 140);
		this.w_editProxyAddressControl.TabIndex = 2;
		this.w_checkBoxBypassAddress.Location = new System.Drawing.Point(8, 29);
		this.w_checkBoxBypassAddress.Name = "w_checkBoxBypassAddress";
		this.w_checkBoxBypassAddress.Properties.Caption = "Bypass proxy server based on address list";
		this.w_checkBoxBypassAddress.Size = new System.Drawing.Size(236, 19);
		this.w_checkBoxBypassAddress.TabIndex = 1;
		this.w_checkBoxBypassAddress.TabStop = false;
		this.w_checkBoxBypassAddress.CheckedChanged += new System.EventHandler(w_checkBoxBypassAddress_CheckedChanged);
		this.w_checkBoxBypassLocal.Location = new System.Drawing.Point(8, 6);
		this.w_checkBoxBypassLocal.Name = "w_checkBoxBypassLocal";
		this.w_checkBoxBypassLocal.Properties.Caption = "Bypass proxy server on local addresses";
		this.w_checkBoxBypassLocal.Size = new System.Drawing.Size(211, 19);
		this.w_checkBoxBypassLocal.TabIndex = 0;
		this.w_checkBoxBypassLocal.TabStop = false;
		this.tabPage1.Controls.Add(this.label3);
		this.tabPage1.Controls.Add(this.w_textBoxPassword);
		this.tabPage1.Controls.Add(this.w_radioButtonDefaultCred);
		this.tabPage1.Controls.Add(this.w_textBoxUserName);
		this.tabPage1.Controls.Add(this.label2);
		this.tabPage1.Controls.Add(this.label1);
		this.tabPage1.Controls.Add(this.w_radioButtonDiffCred);
		this.tabPage1.Name = "tabPage1";
		this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage1.Size = new System.Drawing.Size(435, 209);
		this.tabPage1.Text = "Credentials";
		this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.label3.Appearance.BackColor = System.Drawing.SystemColors.Info;
		this.label3.Appearance.ForeColor = System.Drawing.SystemColors.InfoText;
		this.label3.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
		this.label3.Location = new System.Drawing.Point(6, 5);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(422, 26);
		this.label3.TabIndex = 6;
		this.label3.Text = "Note: Proxy information will be stored locally on the machine.  Password will be encrypted for security.";
		this.w_textBoxPassword.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.w_textBoxPassword.Enabled = false;
		this.w_textBoxPassword.Location = new System.Drawing.Point(123, 110);
		this.w_textBoxPassword.Name = "w_textBoxPassword";
		this.w_textBoxPassword.Properties.UseSystemPasswordChar = true;
		this.w_textBoxPassword.Size = new System.Drawing.Size(263, 20);
		this.w_textBoxPassword.TabIndex = 3;
		this.w_radioButtonDefaultCred.EditValue = true;
		this.w_radioButtonDefaultCred.Location = new System.Drawing.Point(6, 38);
		this.w_radioButtonDefaultCred.Name = "w_radioButtonDefaultCred";
		this.w_radioButtonDefaultCred.Properties.Caption = "Use current user credentials";
		this.w_radioButtonDefaultCred.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
		this.w_radioButtonDefaultCred.Properties.RadioGroupIndex = 1;
		this.w_radioButtonDefaultCred.Size = new System.Drawing.Size(157, 19);
		this.w_radioButtonDefaultCred.TabIndex = 0;
		this.w_textBoxUserName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.w_textBoxUserName.Enabled = false;
		this.w_textBoxUserName.Location = new System.Drawing.Point(123, 84);
		this.w_textBoxUserName.Name = "w_textBoxUserName";
		this.w_textBoxUserName.Size = new System.Drawing.Size(263, 20);
		this.w_textBoxUserName.TabIndex = 2;
		this.label2.Location = new System.Drawing.Point(32, 113);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(50, 13);
		this.label2.TabIndex = 5;
		this.label2.Text = "Password:";
		this.label1.Location = new System.Drawing.Point(32, 87);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(52, 13);
		this.label1.TabIndex = 4;
		this.label1.Text = "Username:";
		this.w_radioButtonDiffCred.Location = new System.Drawing.Point(6, 61);
		this.w_radioButtonDiffCred.Name = "w_radioButtonDiffCred";
		this.w_radioButtonDiffCred.Properties.Caption = "Use different user credentials";
		this.w_radioButtonDiffCred.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
		this.w_radioButtonDiffCred.Properties.RadioGroupIndex = 1;
		this.w_radioButtonDiffCred.Size = new System.Drawing.Size(162, 19);
		this.w_radioButtonDiffCred.TabIndex = 1;
		this.w_radioButtonDiffCred.TabStop = false;
		this.w_radioButtonDiffCred.CheckedChanged += new System.EventHandler(w_radioButtonDiffCred_CheckedChanged);
		this._barManager.AllowCustomization = false;
		this._barManager.AllowMoveBarOnToolbar = false;
		this._barManager.AllowQuickCustomization = false;
		DevExpress.XtraBars.Bars bars = this._barManager.Bars;
		DevExpress.XtraBars.Bar[] barArray = new DevExpress.XtraBars.Bar[2] { this.bar1, this._barProxy };
		bars.AddRange(barArray);
		this._barManager.DockControls.Add(this.barDockControlTop);
		this._barManager.DockControls.Add(this.barDockControlBottom);
		this._barManager.DockControls.Add(this.barDockControlLeft);
		this._barManager.DockControls.Add(this.barDockControlRight);
		this._barManager.Form = this;
		DevExpress.XtraBars.BarItems items = this._barManager.Items;
		DevExpress.XtraBars.BarItem[] barItemArray = new DevExpress.XtraBars.BarItem[2] { this._barLabelProxyServerAddress, this._barTextEditProxyServerAddress };
		items.AddRange(barItemArray);
		this._barManager.MainMenu = this._barProxy;
		this._barManager.MaxItemId = 2;
		this._barManager.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemTextEdit1 });
		this.bar1.BarName = "Tools";
		this.bar1.DockCol = 0;
		this.bar1.DockRow = 1;
		this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
		this.bar1.Text = "Tools";
		this._barProxy.BarName = "Main menu";
		this._barProxy.DockCol = 0;
		this._barProxy.DockRow = 0;
		this._barProxy.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
		DevExpress.XtraBars.LinksInfo linksPersistInfo = this._barProxy.LinksPersistInfo;
		DevExpress.XtraBars.LinkPersistInfo[] linkPersistInfo = new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this._barLabelProxyServerAddress),
			new DevExpress.XtraBars.LinkPersistInfo(this._barTextEditProxyServerAddress)
		};
		linksPersistInfo.AddRange(linkPersistInfo);
		this._barProxy.OptionsBar.AllowQuickCustomization = false;
		this._barProxy.OptionsBar.DrawDragBorder = false;
		this._barProxy.OptionsBar.UseWholeRow = true;
		this._barProxy.Text = "Main menu";
		this._barLabelProxyServerAddress.Caption = "Proxy Server Address";
		this._barLabelProxyServerAddress.Id = 0;
		this._barLabelProxyServerAddress.Name = "_barLabelProxyServerAddress";
		this._barLabelProxyServerAddress.TextAlignment = System.Drawing.StringAlignment.Near;
		this._barTextEditProxyServerAddress.AutoFillWidth = true;
		this._barTextEditProxyServerAddress.Caption = "Proxy Server Address Editor";
		this._barTextEditProxyServerAddress.Edit = this.repositoryItemTextEdit1;
		this._barTextEditProxyServerAddress.Id = 1;
		this._barTextEditProxyServerAddress.Name = "_barTextEditProxyServerAddress";
		this._barTextEditProxyServerAddress.Width = 275;
		this.repositoryItemTextEdit1.AutoHeight = false;
		this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Size = new System.Drawing.Size(441, 51);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 306);
		this.barDockControlBottom.Size = new System.Drawing.Size(441, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 51);
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 255);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(441, 51);
		this.barDockControlRight.Size = new System.Drawing.Size(0, 255);
		base.AcceptButton = this.buttonOK;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.buttonCancel;
		base.ClientSize = new System.Drawing.Size(441, 306);
		base.Controls.Add(this.tabControl1);
		base.Controls.Add(this.buttonOK);
		base.Controls.Add(this.buttonCancel);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "EditProxyDialog";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Configure Proxy Settings";
		((System.ComponentModel.ISupportInitialize)this.tabControl1).EndInit();
		this.tabControl1.ResumeLayout(false);
		this.tabPage2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.w_checkBoxBypassAddress.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.w_checkBoxBypassLocal.Properties).EndInit();
		this.tabPage1.ResumeLayout(false);
		this.tabPage1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.w_textBoxPassword.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.w_radioButtonDefaultCred.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.w_textBoxUserName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.w_radioButtonDiffCred.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this._barManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).EndInit();
		base.ResumeLayout(false);
	}

        private void w_checkBoxBypassAddress_CheckedChanged(object sender, EventArgs e)
	{
		w_editProxyAddressControl.Enabled = w_checkBoxBypassAddress.Checked;
	}

        private void w_radioButtonDiffCred_CheckedChanged(object sender, EventArgs e)
	{
		w_textBoxPassword.Enabled = w_radioButtonDiffCred.Checked;
		w_textBoxUserName.Enabled = w_radioButtonDiffCred.Checked;
	}
    }
}
