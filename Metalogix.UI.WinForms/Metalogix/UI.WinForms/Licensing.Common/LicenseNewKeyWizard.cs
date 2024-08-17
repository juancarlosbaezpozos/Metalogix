using DevExpress.XtraEditors;
using Metalogix;
using Metalogix.Licensing.Common;
using Metalogix.Licensing.LicenseServer;
using Metalogix.MLLicensing.Properties;
using Metalogix.UI.WinForms;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Licensing.Common
{
	public class LicenseNewKeyWizard : Form
	{
		private readonly ILicensingDialogServiceProvider _server;

		private bool _useOldKeyActivation;

		private string pastedText = "";

		private string pastedTextBox = "";

		private bool pasted;

		private IContainer components;

		private Panel panel1;

		private Button roundedButtonNext;

		private Button roundedButtonClose;

		private Button roundedButtonBack;

		private Label lblKey;

		private TextBox textBoxLic1;

		private TextBox textBoxLic2;

		private TextBox textBoxLic3;

		private TextBox textBoxLic4;

		private TextBox textBoxLic5;

		private LicenseNewKeyWizard.WizardControl tabControl1;

		private TabPage tabPageLicKey;

		private TabPage tabPage2;

		private Label label2;

		private Label label4;

		private RadioButton radioButtonOnline;

		private RadioButton radioButtonOffline;

		private Panel gradientLine;

		private Label label1;

		private TextBox textBoxOfflineRequest;

		private Label label6;

		private LinkLabel linkLabelSaveReqToFile;

		private TabPage tabPage1;

		private TabPage tabPage3;

		private Label label7;

		private Label label8;

		private Label label10;

		private LinkLabel linkLabelLoadFromFile;

		private Label label11;

		private TextBox textBoxOfflineResponse;

		private Label label9;

		private Label labelLicenseExpirationText;

		private Label label5;

		private Label labelLicensedDataText;

		private Label label16;

		private Label labelUsedDataText;

		private Label labelLicUsedData;

		private Label labelLicLicData;

		private Label labelLicMaintenanceExp;

		private Label labelLicExpiration;

		private System.Windows.Forms.ContextMenuStrip contextMenuTextbox;

		private ToolStripMenuItem StripMenuItemCopy;

		private ToolStripMenuItem StripMenuItemCut;

		private ToolStripMenuItem StripMenuItemPaste;

		private Label label3;

		private Label label12;

		private PictureBox pictureBox1;

		private PictureBox pictureBox2;

		private PictureBox pictureBox3;

		private PictureBox pictureBox4;

		private TextBox textBoxActivationLink;

		private Label label13;

		private Label label14;

		private LinkLabel linkLabelNavigateLicActivation;

		private LinkLabel linkLabelSetProxy;

		private Panel panelNewKeyInputs;

		private TextBox textBoxOldKey;

		private RadioButton radioButtonNewKey;

		private RadioButton radioButtonOldKey;

		private Panel panel2;

		private Label labelValidationInfo;

		private Panel activationPanel;

		private LicenseNewKeyWizard.Page CurrentPage
		{
			get
			{
				return (LicenseNewKeyWizard.Page)this.tabControl1.SelectedIndex;
			}
			set
			{
				int num = (int)value;
				if (num == 0)
				{
					this.textBoxLic1.Select();
				}
				this.tabControl1.SelectedIndex = num;
			}
		}

		private bool UseOldKeyActivation
		{
			get
			{
				return this._useOldKeyActivation;
			}
			set
			{
				this._useOldKeyActivation = value;
				this.textBoxOldKey.Enabled = this._useOldKeyActivation;
				this.panelNewKeyInputs.Enabled = !this._useOldKeyActivation;
			}
		}

	    public LicenseNewKeyWizard(ILicensingDialogServiceProvider server, string licenseKey)
	    {
	        if (server == null)
	        {
	            throw new System.ArgumentNullException("server");
	        }
	        this._server = server;
	        base.TopMost = true;
	        this.InitializeComponent();
	        base.BringToFront();
	        this.textBoxActivationLink.Text = this._server.LicenseOfflineActivationURL;
	        this.SetupPage();
	        if (!string.IsNullOrEmpty(licenseKey))
	        {
	            this.UseOldKeyActivation = (licenseKey.Length != 25 && licenseKey.Length != 29);
	            this.radioButtonOldKey.Checked = this.UseOldKeyActivation;
	            this.PasteLicenseCode(licenseKey, false);
	        }
	        else
	        {
	            this.UseOldKeyActivation = false;
	        }
	        try
	        {
	            server.GetLicenseInformation();
	            this.labelValidationInfo.Text = string.Empty;
	        }
	        catch (LicenseDoesntExistException)
	        {
	            this.labelValidationInfo.Text = string.Empty;
	        }
	        catch (System.Exception ex)
	        {
	            this.labelValidationInfo.Text = string.Format("Invalid license: {0}", ex.Message);
	            this.labelValidationInfo.ForeColor = System.Drawing.Color.Red;
	        }
	        int num = (int)System.Math.Ceiling((double)this.labelValidationInfo.PreferredWidth / (double)this.labelValidationInfo.Width);
	        int num2 = (this.labelValidationInfo.PreferredHeight - 6) * num + 6 - this.labelValidationInfo.Height;
	        this.labelValidationInfo.Height = this.labelValidationInfo.Height + num2;
	        base.Height += num2;
	        this.activationPanel.Location = new System.Drawing.Point(this.activationPanel.Location.X, this.activationPanel.Location.Y + num2);
	        if (server.Product == Product.MMEPF)
	        {
	            this.radioButtonOldKey.Visible = (this.textBoxOldKey.Visible = false);
	        }
	    }
        private bool ActivateLicenseOffline()
		{
			bool flag;
			try
			{
				if (this.textBoxOfflineResponse.Text.Length >= 100)
				{
					using (AutoWaitCursor autoWaitCursor = AutoWaitCursor.Create(this))
					{
						this._server.ActivateLicenseOffline(this.textBoxOfflineResponse.Text);
					}
					flag = true;
				}
				else
				{
					FlatXtraMessageBox.Show(this, "Please enter valid offline activation data first.", this._server.DialogTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
					flag = false;
				}
			}
			catch (LicenseExpiredException licenseExpiredException1)
			{
				LicenseExpiredException licenseExpiredException = licenseExpiredException1;
				if (licenseExpiredException.Message.Equals(Resources.License_Expired, StringComparison.InvariantCulture))
				{
					LicenseNewKeyWizard.DisplayLicenseExceededAlert(this, Resources.License_Alert_Title, Resources.Alert_Title, string.Format("Error: {0}", Resources.License_Has_Expired), Resources.License_Caption, Resources.Contact_Sales_For_Limit_Exceeded, null);
				}
				else if (!licenseExpiredException.Message.Equals(Resources.Licensed_Data_Exceeded, StringComparison.InvariantCulture))
				{
					string licenseCaption = Resources.License_Caption;
					string str = string.Concat("Failed to activate the license: ", (LicenseNewKeyWizard.IsLicenseException(licenseExpiredException) ? licenseExpiredException.Message : "Unspecified error."));
					GlobalServices.GetErrorHandlerAs<Metalogix.UI.WinForms.ErrorHandler>().HandleException(this, licenseCaption, str, null, ErrorIcon.Warning);
				}
				else
				{
					LicenseNewKeyWizard.DisplayLicenseExceededAlert(this, Resources.License_Alert_Title, Resources.Alert_Title, string.Format("Error: {0}", Resources.License_Limit_Exceeded), Resources.License_Limit_Exceeded_Message, Resources.Contact_Sales_For_Limit_Exceeded, null);
				}
				Trace.WriteLine(string.Concat("LicenseNewKeyWizard >> Failed to get activation data: ", licenseExpiredException));
				flag = false;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				string str1 = string.Concat("Failed to activate the license: ", (LicenseNewKeyWizard.IsLicenseException(exception) ? exception.Message : "Unspecified error."));
				GlobalServices.GetErrorHandlerAs<Metalogix.UI.WinForms.ErrorHandler>().HandleException(this, this._server.DialogTitle, str1, exception, ErrorIcon.Error);
				Trace.WriteLine(string.Concat("LicenseNewKeyWizard >> Failed to get activation data: ", exception));
				flag = false;
			}
			return flag;
		}

		private bool ActivateLicenseOnline()
		{
			bool flag;
			try
			{
				LicenseNewKeyWizard.LicenseKey licenseKey = this.GetLicenseKey(true);
				if (licenseKey.IsValid)
				{
					using (AutoWaitCursor autoWaitCursor = AutoWaitCursor.Create(this))
					{
						this._server.UpdateLicenseKey(licenseKey.Value, this.UseOldKeyActivation);
					}
					if (this._server.GetLicenseInformation().DataLimitExceeded)
					{
						throw new LicenseExpiredException(Resources.Licensed_Data_Exceeded);
					}
					flag = true;
				}
				else
				{
					flag = false;
				}
			}
			catch (LicenseExpiredException licenseExpiredException1)
			{
				LicenseExpiredException licenseExpiredException = licenseExpiredException1;
				if (licenseExpiredException.Message.Equals(Resources.License_Expired, StringComparison.InvariantCulture))
				{
					LicenseNewKeyWizard.DisplayLicenseExceededAlert(this, Resources.License_Alert_Title, Resources.Alert_Title, string.Format("Error: {0}", Resources.License_Has_Expired), Resources.License_Caption, Resources.Contact_Sales_For_Limit_Exceeded, null);
				}
				else if (!licenseExpiredException.Message.Equals(Resources.Licensed_Data_Exceeded, StringComparison.InvariantCulture))
				{
					string licenseCaption = Resources.License_Caption;
					string str = string.Concat("Failed to activate the license", (LicenseNewKeyWizard.IsLicenseException(licenseExpiredException) ? string.Concat(": ", licenseExpiredException.Message) : string.Concat(". ", Environment.NewLine, "In case you do not have Internet connection available, please use the Offline Activation for registering the software.")));
					GlobalServices.GetErrorHandlerAs<Metalogix.UI.WinForms.ErrorHandler>().HandleException(this, licenseCaption, str, null, ErrorIcon.Warning);
				}
				else
				{
					LicenseNewKeyWizard.DisplayLicenseExceededAlert(this, Resources.License_Alert_Title, Resources.Alert_Title, string.Format("Error: {0}", Resources.License_Limit_Exceeded), Resources.License_Limit_Exceeded_Message, Resources.Contact_Sales_For_Limit_Exceeded, null);
				}
				Trace.WriteLine(string.Concat("LicenseNewKeyWizard >> Failed to get activation data: ", licenseExpiredException));
				flag = false;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				string str1 = string.Concat("Failed to activate the license", (LicenseNewKeyWizard.IsLicenseException(exception) ? string.Concat(": ", exception.Message) : string.Concat(". ", Environment.NewLine, "In case you do not have Internet connection available, please use the Offline Activation for registering the software.")));
				GlobalServices.GetErrorHandlerAs<Metalogix.UI.WinForms.ErrorHandler>().HandleException(this, this._server.DialogTitle, str1, exception, ErrorIcon.Error);
				Trace.WriteLine(string.Concat("LicenseNewKeyWizard >> Failed to get activation data: ", exception));
				flag = false;
			}
			return flag;
		}

		private void checkBoxOldStyleLic_CheckedChanged(object sender, EventArgs e)
		{
			this.UseOldKeyActivation = this.radioButtonOldKey.Checked;
		}

	    // Metalogix.UI.WinForms.Licensing.Common.LicenseNewKeyWizard
	    private void contextMenuTextbox_Opened(object sender, System.EventArgs e)
	    {
	        try
	        {
	            TextBox sourceTextBox = LicenseNewKeyWizard.GetSourceTextBox(sender as ContextMenuStrip);
	            if (sourceTextBox == null)
	            {
	                this.StripMenuItemCopy.Enabled = (this.StripMenuItemCut.Enabled = (this.StripMenuItemPaste.Enabled = false));
	            }
	            else
	            {
	                this.StripMenuItemCopy.Enabled = (sourceTextBox.Text != string.Empty);
	                this.StripMenuItemCut.Enabled = (sourceTextBox.Text != string.Empty && !sourceTextBox.ReadOnly);
	                IDataObject dataObject = Clipboard.GetDataObject();
	                if (dataObject != null && dataObject.GetDataPresent(DataFormats.Text))
	                {
	                    string text = dataObject.GetData(DataFormats.Text).ToString().Trim();
	                    if (sourceTextBox == this.textBoxOfflineRequest || sourceTextBox == this.textBoxOfflineResponse)
	                    {
	                        this.StripMenuItemPaste.Enabled = !sourceTextBox.ReadOnly;
	                    }
	                    else if (sourceTextBox == this.textBoxOldKey)
	                    {
	                        this.StripMenuItemPaste.Enabled = (text.Length > 200);
	                    }
	                    else
	                    {
	                        this.StripMenuItemPaste.Enabled = (text.Length == 25 || text.Length == 29);
	                    }
	                }
	                else
	                {
	                    this.StripMenuItemPaste.Enabled = false;
	                }
	            }
	        }
	        catch (System.Exception ex)
	        {
	            Logger.Error.Write("Failed to toggle context menu functions.", ex);
	            this.StripMenuItemCopy.Enabled = (this.StripMenuItemCut.Enabled = (this.StripMenuItemPaste.Enabled = false));
	        }
	    }

        public static void DisplayLicenseExceededAlert(LicenseNewKeyWizard lic, string licenseAlertTitle, string alertTitle, string licenseUsageWarning, string licenseDataUsage, string contactInfo, string suppressMessage)
		{
			AdvancedAlertDialogBox advancedAlertDialogBox = new AdvancedAlertDialogBox(licenseAlertTitle, alertTitle, licenseUsageWarning, licenseDataUsage, contactInfo, suppressMessage);
			advancedAlertDialogBox.ShowDialog(lic);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private bool GenerateOfflineRequest()
		{
			bool flag;
			try
			{
				LicenseNewKeyWizard.LicenseKey licenseKey = this.GetLicenseKey(true);
				if (licenseKey.IsValid)
				{
					using (AutoWaitCursor autoWaitCursor = AutoWaitCursor.Create(this))
					{
						this.textBoxOfflineRequest.Text = this._server.GetOfflineLicenseActivationData(licenseKey.Value, this.UseOldKeyActivation);
					}
					flag = true;
				}
				else
				{
					flag = false;
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				string str = string.Concat("Failed to generate offline activation data: ", (LicenseNewKeyWizard.IsLicenseException(exception) ? exception.Message : "Unspecified error."));
				GlobalServices.GetErrorHandlerAs<Metalogix.UI.WinForms.ErrorHandler>().HandleException(this, this._server.DialogTitle, str, exception, ErrorIcon.Error);
				Trace.WriteLine(string.Concat("LicenseNewKeyWizard >> Failed to get activation data: ", exception));
				flag = false;
			}
			return flag;
		}

		private LicenseNewKeyWizard.LicenseKey GetLicenseKey(bool validate)
		{
			string str;
			LicenseNewKeyWizard.LicenseKey licenseKey = new LicenseNewKeyWizard.LicenseKey();
			if (this.UseOldKeyActivation)
			{
				str = this.textBoxOldKey.Text.Trim();
			}
			else
			{
				object[] objArray = new object[] { this.textBoxLic1.Text.Trim(), this.textBoxLic2.Text.Trim(), this.textBoxLic3.Text.Trim(), this.textBoxLic4.Text.Trim(), this.textBoxLic5.Text.Trim() };
				str = string.Format("{0}-{1}-{2}-{3}-{4}", objArray);
			}
			licenseKey.Value = str;
			licenseKey.IsValid = true;
			LicenseNewKeyWizard.LicenseKey licenseKey1 = licenseKey;
			if (validate)
			{
				licenseKey1.IsValid = this.IsValidLicenseKey(licenseKey1.Value, this.UseOldKeyActivation);
				if (!licenseKey1.IsValid)
				{
					FlatXtraMessageBox.Show(this, "The key entered is invalid.\nPlease ensure you have entered the key correctly and try again.", this._server.DialogTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				}
			}
			return licenseKey1;
		}

		private static TextBox GetSourceTextBox(ToolStripMenuItem mi)
		{
			if (mi == null)
			{
				return null;
			}
			return LicenseNewKeyWizard.GetSourceTextBox(mi.GetCurrentParent() as System.Windows.Forms.ContextMenuStrip);
		}

		private static TextBox GetSourceTextBox(System.Windows.Forms.ContextMenuStrip ms)
		{
			if (ms == null)
			{
				return null;
			}
			return ms.SourceControl as TextBox;
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(LicenseNewKeyWizard));
			this.panel1 = new Panel();
			this.roundedButtonBack = new Button();
			this.roundedButtonNext = new Button();
			this.gradientLine = new Panel();
			this.roundedButtonClose = new Button();
			this.lblKey = new Label();
			this.textBoxLic1 = new TextBox();
			this.contextMenuTextbox = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.StripMenuItemCopy = new ToolStripMenuItem();
			this.StripMenuItemCut = new ToolStripMenuItem();
			this.StripMenuItemPaste = new ToolStripMenuItem();
			this.textBoxLic3 = new TextBox();
			this.textBoxLic2 = new TextBox();
			this.textBoxLic4 = new TextBox();
			this.textBoxLic5 = new TextBox();
			this.tabControl1 = new LicenseNewKeyWizard.WizardControl();
			this.tabPageLicKey = new TabPage();
			this.panel2 = new Panel();
			this.radioButtonOldKey = new RadioButton();
			this.panelNewKeyInputs = new Panel();
			this.radioButtonNewKey = new RadioButton();
			this.textBoxOldKey = new TextBox();
			this.labelValidationInfo = new Label();
			this.linkLabelSetProxy = new LinkLabel();
			this.pictureBox1 = new PictureBox();
			this.label3 = new Label();
			this.label2 = new Label();
			this.label4 = new Label();
			this.radioButtonOnline = new RadioButton();
			this.radioButtonOffline = new RadioButton();
			this.tabPage2 = new TabPage();
			this.pictureBox3 = new PictureBox();
			this.label13 = new Label();
			this.label8 = new Label();
			this.label1 = new Label();
			this.label7 = new Label();
			this.label6 = new Label();
			this.linkLabelNavigateLicActivation = new LinkLabel();
			this.linkLabelSaveReqToFile = new LinkLabel();
			this.textBoxOfflineRequest = new TextBox();
			this.textBoxActivationLink = new TextBox();
			this.tabPage1 = new TabPage();
			this.label14 = new Label();
			this.pictureBox4 = new PictureBox();
			this.label10 = new Label();
			this.linkLabelLoadFromFile = new LinkLabel();
			this.label11 = new Label();
			this.textBoxOfflineResponse = new TextBox();
			this.tabPage3 = new TabPage();
			this.pictureBox2 = new PictureBox();
			this.label12 = new Label();
			this.labelLicUsedData = new Label();
			this.label9 = new Label();
			this.labelLicLicData = new Label();
			this.labelLicMaintenanceExp = new Label();
			this.label5 = new Label();
			this.labelLicExpiration = new Label();
			this.labelUsedDataText = new Label();
			this.labelLicenseExpirationText = new Label();
			this.labelLicensedDataText = new Label();
			this.label16 = new Label();
			this.activationPanel = new Panel();
			this.panel1.SuspendLayout();
			this.contextMenuTextbox.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPageLicKey.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panelNewKeyInputs.SuspendLayout();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			this.tabPage2.SuspendLayout();
			((ISupportInitialize)this.pictureBox3).BeginInit();
			this.tabPage1.SuspendLayout();
			((ISupportInitialize)this.pictureBox4).BeginInit();
			this.tabPage3.SuspendLayout();
			((ISupportInitialize)this.pictureBox2).BeginInit();
			this.activationPanel.SuspendLayout();
			base.SuspendLayout();
			this.panel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.panel1.BackColor = SystemColors.Control;
			this.panel1.Controls.Add(this.roundedButtonBack);
			this.panel1.Controls.Add(this.roundedButtonNext);
			this.panel1.Controls.Add(this.gradientLine);
			this.panel1.Controls.Add(this.roundedButtonClose);
			this.panel1.Location = new Point(0, 354);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(441, 32);
			this.panel1.TabIndex = 1;
			this.roundedButtonBack.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.roundedButtonBack.BackColor = Color.Transparent;
			this.roundedButtonBack.Location = new Point(179, 7);
			this.roundedButtonBack.Name = "roundedButtonBack";
			this.roundedButtonBack.Size = new System.Drawing.Size(75, 20);
			this.roundedButtonBack.TabIndex = 0;
			this.roundedButtonBack.Text = "< Back";
			this.roundedButtonBack.UseVisualStyleBackColor = false;
			this.roundedButtonBack.Click += new EventHandler(this.roundedButtonBack_Click);
			this.roundedButtonNext.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.roundedButtonNext.BackColor = Color.Transparent;
			this.roundedButtonNext.Location = new Point(260, 7);
			this.roundedButtonNext.Name = "roundedButtonNext";
			this.roundedButtonNext.Size = new System.Drawing.Size(75, 20);
			this.roundedButtonNext.TabIndex = 1;
			this.roundedButtonNext.Text = "Next >";
			this.roundedButtonNext.UseVisualStyleBackColor = false;
			this.roundedButtonNext.Click += new EventHandler(this.roundedButtonNext_Click);
			this.gradientLine.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.gradientLine.BackColor = Color.LightGray;
			this.gradientLine.ForeColor = Color.Gray;
			this.gradientLine.Location = new Point(0, 0);
			this.gradientLine.Name = "gradientLine";
			this.gradientLine.Size = new System.Drawing.Size(441, 1);
			this.gradientLine.TabIndex = 0;
			this.roundedButtonClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.roundedButtonClose.BackColor = Color.Transparent;
			this.roundedButtonClose.Location = new Point(354, 7);
			this.roundedButtonClose.Name = "roundedButtonClose";
			this.roundedButtonClose.Size = new System.Drawing.Size(75, 20);
			this.roundedButtonClose.TabIndex = 2;
			this.roundedButtonClose.Text = "Cancel";
			this.roundedButtonClose.UseVisualStyleBackColor = false;
			this.roundedButtonClose.Click += new EventHandler(this.roundedButtonClose_Click);
			this.lblKey.BackColor = Color.Transparent;
			this.lblKey.Location = new Point(3, 6);
			this.lblKey.Name = "lblKey";
			this.lblKey.Size = new System.Drawing.Size(104, 17);
			this.lblKey.TabIndex = 0;
			this.lblKey.Text = "Enter license key:";
			this.lblKey.TextAlign = ContentAlignment.MiddleLeft;
			this.textBoxLic1.BackColor = Color.White;
			this.textBoxLic1.BorderStyle = BorderStyle.FixedSingle;
			this.textBoxLic1.ContextMenuStrip = this.contextMenuTextbox;
			this.textBoxLic1.Location = new Point(1, 1);
			this.textBoxLic1.MaxLength = 5;
			this.textBoxLic1.Name = "textBoxLic1";
			this.textBoxLic1.Size = new System.Drawing.Size(52, 21);
			this.textBoxLic1.TabIndex = 0;
			this.textBoxLic1.TextChanged += new EventHandler(this.textBoxLic1_TextChanged);
			this.textBoxLic1.KeyDown += new KeyEventHandler(this.textBoxLic1_KeyDown);
			ToolStripItemCollection items = this.contextMenuTextbox.Items;
			ToolStripItem[] stripMenuItemCopy = new ToolStripItem[] { this.StripMenuItemCopy, this.StripMenuItemCut, this.StripMenuItemPaste };
			items.AddRange(stripMenuItemCopy);
			this.contextMenuTextbox.Name = "contextMenuTextbox";
			this.contextMenuTextbox.Size = new System.Drawing.Size(102, 70);
			this.contextMenuTextbox.Opened += new EventHandler(this.contextMenuTextbox_Opened);
			this.StripMenuItemCopy.Name = "StripMenuItemCopy";
			this.StripMenuItemCopy.Size = new System.Drawing.Size(101, 22);
			this.StripMenuItemCopy.Text = "Copy";
			this.StripMenuItemCopy.Click += new EventHandler(this.StripMenuItemCopy_Click);
			this.StripMenuItemCut.Name = "StripMenuItemCut";
			this.StripMenuItemCut.Size = new System.Drawing.Size(101, 22);
			this.StripMenuItemCut.Text = "Cut";
			this.StripMenuItemCut.Click += new EventHandler(this.StripMenuItemCut_Click);
			this.StripMenuItemPaste.Name = "StripMenuItemPaste";
			this.StripMenuItemPaste.Size = new System.Drawing.Size(101, 22);
			this.StripMenuItemPaste.Text = "Paste";
			this.StripMenuItemPaste.Click += new EventHandler(this.StripMenuItemPaste_Click);
			this.textBoxLic3.BackColor = Color.White;
			this.textBoxLic3.BorderStyle = BorderStyle.FixedSingle;
			this.textBoxLic3.ContextMenuStrip = this.contextMenuTextbox;
			this.textBoxLic3.Location = new Point(117, 1);
			this.textBoxLic3.MaxLength = 5;
			this.textBoxLic3.Name = "textBoxLic3";
			this.textBoxLic3.Size = new System.Drawing.Size(52, 21);
			this.textBoxLic3.TabIndex = 2;
			this.textBoxLic3.TextChanged += new EventHandler(this.textBoxLic1_TextChanged);
			this.textBoxLic3.KeyDown += new KeyEventHandler(this.textBoxLic1_KeyDown);
			this.textBoxLic2.BackColor = Color.White;
			this.textBoxLic2.BorderStyle = BorderStyle.FixedSingle;
			this.textBoxLic2.ContextMenuStrip = this.contextMenuTextbox;
			this.textBoxLic2.Location = new Point(59, 1);
			this.textBoxLic2.MaxLength = 5;
			this.textBoxLic2.Name = "textBoxLic2";
			this.textBoxLic2.Size = new System.Drawing.Size(52, 21);
			this.textBoxLic2.TabIndex = 1;
			this.textBoxLic2.TextChanged += new EventHandler(this.textBoxLic1_TextChanged);
			this.textBoxLic2.KeyDown += new KeyEventHandler(this.textBoxLic1_KeyDown);
			this.textBoxLic4.BackColor = Color.White;
			this.textBoxLic4.BorderStyle = BorderStyle.FixedSingle;
			this.textBoxLic4.ContextMenuStrip = this.contextMenuTextbox;
			this.textBoxLic4.Location = new Point(175, 1);
			this.textBoxLic4.MaxLength = 5;
			this.textBoxLic4.Name = "textBoxLic4";
			this.textBoxLic4.Size = new System.Drawing.Size(52, 21);
			this.textBoxLic4.TabIndex = 3;
			this.textBoxLic4.TextChanged += new EventHandler(this.textBoxLic1_TextChanged);
			this.textBoxLic4.KeyDown += new KeyEventHandler(this.textBoxLic1_KeyDown);
			this.textBoxLic5.BackColor = Color.White;
			this.textBoxLic5.BorderStyle = BorderStyle.FixedSingle;
			this.textBoxLic5.ContextMenuStrip = this.contextMenuTextbox;
			this.textBoxLic5.Location = new Point(233, 1);
			this.textBoxLic5.MaxLength = 5;
			this.textBoxLic5.Name = "textBoxLic5";
			this.textBoxLic5.Size = new System.Drawing.Size(52, 21);
			this.textBoxLic5.TabIndex = 4;
			this.textBoxLic5.TextChanged += new EventHandler(this.textBoxLic1_TextChanged);
			this.textBoxLic5.KeyDown += new KeyEventHandler(this.textBoxLic1_KeyDown);
			this.tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.tabControl1.Appearance = TabAppearance.FlatButtons;
			this.tabControl1.Controls.Add(this.tabPageLicKey);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Location = new Point(3, 1);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(435, 356);
			this.tabControl1.TabIndex = 0;
			this.tabControl1.Tag = "";
			this.tabControl1.KeyDown += new KeyEventHandler(this.tabControl1_KeyDown);
			this.tabPageLicKey.BackColor = SystemColors.Control;
			this.tabPageLicKey.Controls.Add(this.labelValidationInfo);
			this.tabPageLicKey.Controls.Add(this.pictureBox1);
			this.tabPageLicKey.Controls.Add(this.label3);
			this.tabPageLicKey.Controls.Add(this.label4);
			this.tabPageLicKey.Controls.Add(this.activationPanel);
			this.tabPageLicKey.Location = new Point(4, 25);
			this.tabPageLicKey.Name = "tabPageLicKey";
			this.tabPageLicKey.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageLicKey.Size = new System.Drawing.Size(427, 327);
			this.tabPageLicKey.TabIndex = 0;
			this.tabPageLicKey.Text = "EnterLicKey";
			this.tabPageLicKey.ToolTipText = "Enter you license key";
			this.panel2.Controls.Add(this.lblKey);
			this.panel2.Controls.Add(this.radioButtonOldKey);
			this.panel2.Controls.Add(this.panelNewKeyInputs);
			this.panel2.Controls.Add(this.radioButtonNewKey);
			this.panel2.Controls.Add(this.textBoxOldKey);
			this.panel2.Location = new Point(50, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(358, 149);
			this.panel2.TabIndex = 0;
			this.radioButtonOldKey.AutoSize = true;
			this.radioButtonOldKey.Location = new Point(6, 84);
			this.radioButtonOldKey.Name = "radioButtonOldKey";
			this.radioButtonOldKey.Size = new System.Drawing.Size(323, 17);
			this.radioButtonOldKey.TabIndex = 2;
			this.radioButtonOldKey.TabStop = true;
			this.radioButtonOldKey.Text = "I am updating from an older version and have an old-style key";
			this.radioButtonOldKey.UseVisualStyleBackColor = true;
			this.radioButtonOldKey.CheckedChanged += new EventHandler(this.radioButtonOldKey_CheckedChanged);
			this.panelNewKeyInputs.Controls.Add(this.textBoxLic1);
			this.panelNewKeyInputs.Controls.Add(this.textBoxLic4);
			this.panelNewKeyInputs.Controls.Add(this.textBoxLic5);
			this.panelNewKeyInputs.Controls.Add(this.textBoxLic3);
			this.panelNewKeyInputs.Controls.Add(this.textBoxLic2);
			this.panelNewKeyInputs.Location = new Point(25, 49);
			this.panelNewKeyInputs.Name = "panelNewKeyInputs";
			this.panelNewKeyInputs.Size = new System.Drawing.Size(315, 29);
			this.panelNewKeyInputs.TabIndex = 24;
			this.radioButtonNewKey.AutoSize = true;
			this.radioButtonNewKey.Checked = true;
			this.radioButtonNewKey.Location = new Point(6, 26);
			this.radioButtonNewKey.Name = "radioButtonNewKey";
			this.radioButtonNewKey.Size = new System.Drawing.Size(120, 17);
			this.radioButtonNewKey.TabIndex = 1;
			this.radioButtonNewKey.TabStop = true;
			this.radioButtonNewKey.Text = "I have a license key";
			this.radioButtonNewKey.UseVisualStyleBackColor = true;
			this.radioButtonNewKey.CheckedChanged += new EventHandler(this.radioButtonNewKey_CheckedChanged);
			this.textBoxOldKey.BackColor = Color.White;
			this.textBoxOldKey.BorderStyle = BorderStyle.FixedSingle;
			this.textBoxOldKey.ContextMenuStrip = this.contextMenuTextbox;
			this.textBoxOldKey.Location = new Point(25, 107);
			this.textBoxOldKey.MaxLength = 2000;
			this.textBoxOldKey.Multiline = true;
			this.textBoxOldKey.Name = "textBoxOldKey";
			this.textBoxOldKey.Size = new System.Drawing.Size(285, 37);
			this.textBoxOldKey.TabIndex = 3;
			this.labelValidationInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			this.labelValidationInfo.BackColor = Color.Transparent;
			this.labelValidationInfo.Location = new Point(53, 38);
			this.labelValidationInfo.Name = "labelValidationInfo";
			this.labelValidationInfo.Size = new System.Drawing.Size(351, 15);
			this.labelValidationInfo.TabIndex = 1;
			this.labelValidationInfo.Text = "[License validation info comes here]\r\n[]";
			this.linkLabelSetProxy.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			this.linkLabelSetProxy.AutoSize = true;
			this.linkLabelSetProxy.Location = new Point(344, 186);
			this.linkLabelSetProxy.Name = "linkLabelSetProxy";
			this.linkLabelSetProxy.Size = new System.Drawing.Size(69, 13);
			this.linkLabelSetProxy.TabIndex = 7;
			this.linkLabelSetProxy.TabStop = true;
			this.linkLabelSetProxy.Text = "Set proxy ...";
			this.linkLabelSetProxy.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabelSetProxy_LinkClicked);
			this.pictureBox1.Image = (Image)componentResourceManager.GetObject("pictureBox1.Image");
			this.pictureBox1.Location = new Point(15, 6);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(32, 32);
			this.pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 21;
			this.pictureBox1.TabStop = false;
			this.label3.BackColor = Color.Transparent;
			this.label3.Font = new System.Drawing.Font("Tahoma", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.label3.ForeColor = Color.DimGray;
			this.label3.Location = new Point(53, 6);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(354, 32);
			this.label3.TabIndex = 0;
			this.label3.Text = "Enter license key and select the activation method";
			this.label3.TextAlign = ContentAlignment.MiddleLeft;
			this.label2.BackColor = Color.Transparent;
			this.label2.Location = new Point(53, 164);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(136, 17);
			this.label2.TabIndex = 3;
			this.label2.Text = "Choose activation mode:";
			this.label2.TextAlign = ContentAlignment.MiddleLeft;
			this.label4.AutoSize = true;
			this.label4.Location = new Point(53, 271);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(0, 13);
			this.label4.TabIndex = 5;
			this.radioButtonOnline.BackColor = Color.Transparent;
			this.radioButtonOnline.Checked = true;
			this.radioButtonOnline.Location = new Point(56, 184);
			this.radioButtonOnline.Name = "radioButtonOnline";
			this.radioButtonOnline.Size = new System.Drawing.Size(323, 17);
			this.radioButtonOnline.TabIndex = 4;
			this.radioButtonOnline.TabStop = true;
			this.radioButtonOnline.Text = "Online activation (requires online internet connection)";
			this.radioButtonOnline.UseVisualStyleBackColor = false;
			this.radioButtonOnline.CheckedChanged += new EventHandler(this.licenseActivationModeChanged);
			this.radioButtonOffline.BackColor = Color.Transparent;
			this.radioButtonOffline.Location = new Point(56, 207);
			this.radioButtonOffline.Name = "radioButtonOffline";
			this.radioButtonOffline.Size = new System.Drawing.Size(323, 17);
			this.radioButtonOffline.TabIndex = 6;
			this.radioButtonOffline.Text = "Offline activation";
			this.radioButtonOffline.UseVisualStyleBackColor = false;
			this.radioButtonOffline.CheckedChanged += new EventHandler(this.licenseActivationModeChanged);
			this.tabPage2.BackColor = SystemColors.Control;
			this.tabPage2.Controls.Add(this.pictureBox3);
			this.tabPage2.Controls.Add(this.label13);
			this.tabPage2.Controls.Add(this.label8);
			this.tabPage2.Controls.Add(this.label1);
			this.tabPage2.Controls.Add(this.label7);
			this.tabPage2.Controls.Add(this.label6);
			this.tabPage2.Controls.Add(this.linkLabelNavigateLicActivation);
			this.tabPage2.Controls.Add(this.linkLabelSaveReqToFile);
			this.tabPage2.Controls.Add(this.textBoxOfflineRequest);
			this.tabPage2.Controls.Add(this.textBoxActivationLink);
			this.tabPage2.Location = new Point(4, 25);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(427, 358);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "OfflineRequest";
			this.pictureBox3.Image = (Image)componentResourceManager.GetObject("pictureBox3.Image");
			this.pictureBox3.Location = new Point(15, 6);
			this.pictureBox3.Name = "pictureBox3";
			this.pictureBox3.Size = new System.Drawing.Size(32, 32);
			this.pictureBox3.SizeMode = PictureBoxSizeMode.AutoSize;
			this.pictureBox3.TabIndex = 25;
			this.pictureBox3.TabStop = false;
			this.label13.AllowDrop = true;
			this.label13.BackColor = Color.Transparent;
			this.label13.Location = new Point(53, 126);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(109, 13);
			this.label13.TabIndex = 2;
			this.label13.Text = "Activation Request:";
			this.label8.AllowDrop = true;
			this.label8.BackColor = Color.Transparent;
			this.label8.Location = new Point(53, 233);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(115, 13);
			this.label8.TabIndex = 6;
			this.label8.Text = "Activation URL:";
			this.label1.AllowDrop = true;
			this.label1.BackColor = Color.Transparent;
			this.label1.Font = new System.Drawing.Font("Tahoma", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.label1.ForeColor = Color.DimGray;
			this.label1.Location = new Point(53, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(345, 32);
			this.label1.TabIndex = 0;
			this.label1.Text = "Generate Activation Request";
			this.label1.TextAlign = ContentAlignment.MiddleLeft;
			this.label7.BackColor = Color.Transparent;
			this.label7.Location = new Point(53, 38);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(351, 55);
			this.label7.TabIndex = 1;
			this.label7.Text = componentResourceManager.GetString("label7.Text");
			this.label6.AutoSize = true;
			this.label6.Location = new Point(171, 113);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(0, 13);
			this.label6.TabIndex = 3;
			this.linkLabelNavigateLicActivation.BackColor = Color.Transparent;
			this.linkLabelNavigateLicActivation.Location = new Point(361, 249);
			this.linkLabelNavigateLicActivation.Name = "linkLabelNavigateLicActivation";
			this.linkLabelNavigateLicActivation.Size = new System.Drawing.Size(37, 13);
			this.linkLabelNavigateLicActivation.TabIndex = 8;
			this.linkLabelNavigateLicActivation.TabStop = true;
			this.linkLabelNavigateLicActivation.Text = "Go...";
			this.linkLabelNavigateLicActivation.TextAlign = ContentAlignment.MiddleRight;
			this.linkLabelNavigateLicActivation.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabelActivationLink_LinkClicked);
			this.linkLabelSaveReqToFile.BackColor = Color.Transparent;
			this.linkLabelSaveReqToFile.Location = new Point(313, 126);
			this.linkLabelSaveReqToFile.Name = "linkLabelSaveReqToFile";
			this.linkLabelSaveReqToFile.Size = new System.Drawing.Size(85, 13);
			this.linkLabelSaveReqToFile.TabIndex = 4;
			this.linkLabelSaveReqToFile.TabStop = true;
			this.linkLabelSaveReqToFile.Text = "Save to file...";
			this.linkLabelSaveReqToFile.TextAlign = ContentAlignment.MiddleRight;
			this.linkLabelSaveReqToFile.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabelSaveReqToFile_LinkClicked);
			this.textBoxOfflineRequest.BackColor = Color.White;
			this.textBoxOfflineRequest.BorderStyle = BorderStyle.FixedSingle;
			this.textBoxOfflineRequest.ContextMenuStrip = this.contextMenuTextbox;
			this.textBoxOfflineRequest.Location = new Point(56, 142);
			this.textBoxOfflineRequest.MaxLength = 1000;
			this.textBoxOfflineRequest.Multiline = true;
			this.textBoxOfflineRequest.Name = "textBoxOfflineRequest";
			this.textBoxOfflineRequest.ReadOnly = true;
			this.textBoxOfflineRequest.Size = new System.Drawing.Size(342, 80);
			this.textBoxOfflineRequest.TabIndex = 5;
			this.textBoxOfflineRequest.KeyDown += new KeyEventHandler(this.On_OfflineRequest_KeyDown);
			this.textBoxActivationLink.BackColor = Color.LightGray;
			this.textBoxActivationLink.BorderStyle = BorderStyle.None;
			this.textBoxActivationLink.Font = new System.Drawing.Font("Tahoma", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.textBoxActivationLink.Location = new Point(56, 249);
			this.textBoxActivationLink.Name = "textBoxActivationLink";
			this.textBoxActivationLink.ReadOnly = true;
			this.textBoxActivationLink.Size = new System.Drawing.Size(299, 14);
			this.textBoxActivationLink.TabIndex = 7;
			this.textBoxActivationLink.Text = "http://www.metalogix.com/LicenseActivation";
			this.textBoxActivationLink.KeyDown += new KeyEventHandler(this.On_OfflineLink_KeyDown);
			this.tabPage1.BackColor = SystemColors.Control;
			this.tabPage1.Controls.Add(this.label14);
			this.tabPage1.Controls.Add(this.pictureBox4);
			this.tabPage1.Controls.Add(this.label10);
			this.tabPage1.Controls.Add(this.linkLabelLoadFromFile);
			this.tabPage1.Controls.Add(this.label11);
			this.tabPage1.Controls.Add(this.textBoxOfflineResponse);
			this.tabPage1.Location = new Point(4, 25);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(427, 358);
			this.tabPage1.TabIndex = 2;
			this.tabPage1.Text = "OfflineResponse";
			this.label14.AllowDrop = true;
			this.label14.BackColor = Color.Transparent;
			this.label14.Location = new Point(53, 126);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(141, 13);
			this.label14.TabIndex = 2;
			this.label14.Text = "Activation Response:";
			this.pictureBox4.Image = (Image)componentResourceManager.GetObject("pictureBox4.Image");
			this.pictureBox4.Location = new Point(15, 6);
			this.pictureBox4.Name = "pictureBox4";
			this.pictureBox4.Size = new System.Drawing.Size(32, 32);
			this.pictureBox4.SizeMode = PictureBoxSizeMode.AutoSize;
			this.pictureBox4.TabIndex = 23;
			this.pictureBox4.TabStop = false;
			this.label10.BackColor = Color.Transparent;
			this.label10.Location = new Point(53, 38);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(345, 72);
			this.label10.TabIndex = 1;
			this.label10.Text = componentResourceManager.GetString("label10.Text");
			this.linkLabelLoadFromFile.BackColor = Color.Transparent;
			this.linkLabelLoadFromFile.Location = new Point(301, 126);
			this.linkLabelLoadFromFile.Name = "linkLabelLoadFromFile";
			this.linkLabelLoadFromFile.Size = new System.Drawing.Size(97, 13);
			this.linkLabelLoadFromFile.TabIndex = 3;
			this.linkLabelLoadFromFile.TabStop = true;
			this.linkLabelLoadFromFile.Text = "Load from file...";
			this.linkLabelLoadFromFile.TextAlign = ContentAlignment.MiddleRight;
			this.linkLabelLoadFromFile.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabelLoadFromFile_LinkClicked);
			this.label11.AllowDrop = true;
			this.label11.BackColor = Color.Transparent;
			this.label11.Font = new System.Drawing.Font("Tahoma", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.label11.ForeColor = Color.DimGray;
			this.label11.Location = new Point(53, 6);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(254, 32);
			this.label11.TabIndex = 0;
			this.label11.Text = "Enter your Activation Response";
			this.label11.TextAlign = ContentAlignment.MiddleLeft;
			this.textBoxOfflineResponse.BackColor = Color.White;
			this.textBoxOfflineResponse.BorderStyle = BorderStyle.FixedSingle;
			this.textBoxOfflineResponse.ContextMenuStrip = this.contextMenuTextbox;
			this.textBoxOfflineResponse.Location = new Point(56, 142);
			this.textBoxOfflineResponse.MaxLength = 3000;
			this.textBoxOfflineResponse.Multiline = true;
			this.textBoxOfflineResponse.Name = "textBoxOfflineResponse";
			this.textBoxOfflineResponse.Size = new System.Drawing.Size(342, 80);
			this.textBoxOfflineResponse.TabIndex = 4;
			this.tabPage3.BackColor = SystemColors.Control;
			this.tabPage3.Controls.Add(this.pictureBox2);
			this.tabPage3.Controls.Add(this.label12);
			this.tabPage3.Controls.Add(this.labelLicUsedData);
			this.tabPage3.Controls.Add(this.label9);
			this.tabPage3.Controls.Add(this.labelLicLicData);
			this.tabPage3.Controls.Add(this.labelLicMaintenanceExp);
			this.tabPage3.Controls.Add(this.label5);
			this.tabPage3.Controls.Add(this.labelLicExpiration);
			this.tabPage3.Controls.Add(this.labelUsedDataText);
			this.tabPage3.Controls.Add(this.labelLicenseExpirationText);
			this.tabPage3.Controls.Add(this.labelLicensedDataText);
			this.tabPage3.Controls.Add(this.label16);
			this.tabPage3.Location = new Point(4, 25);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage3.Size = new System.Drawing.Size(427, 358);
			this.tabPage3.TabIndex = 3;
			this.tabPage3.Text = "Finish";
			this.pictureBox2.Image = (Image)componentResourceManager.GetObject("pictureBox2.Image");
			this.pictureBox2.Location = new Point(15, 6);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(32, 32);
			this.pictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;
			this.pictureBox2.TabIndex = 22;
			this.pictureBox2.TabStop = false;
			this.label12.BackColor = Color.Transparent;
			this.label12.Font = new System.Drawing.Font("Tahoma", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.label12.ForeColor = Color.DimGray;
			this.label12.Location = new Point(53, 6);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(185, 32);
			this.label12.TabIndex = 0;
			this.label12.Text = "Congratulations!";
			this.label12.TextAlign = ContentAlignment.MiddleLeft;
			this.labelLicUsedData.AutoSize = true;
			this.labelLicUsedData.BackColor = Color.Transparent;
			this.labelLicUsedData.Font = new System.Drawing.Font("Tahoma", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.labelLicUsedData.Location = new Point(226, 187);
			this.labelLicUsedData.Name = "labelLicUsedData";
			this.labelLicUsedData.Size = new System.Drawing.Size(32, 13);
			this.labelLicUsedData.TabIndex = 10;
			this.labelLicUsedData.Text = "num";
			this.label9.AutoSize = true;
			this.label9.BackColor = Color.Transparent;
			this.label9.Location = new Point(53, 58);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(214, 13);
			this.label9.TabIndex = 1;
			this.label9.Text = "The license key was successfully activated.";
			this.label9.TextAlign = ContentAlignment.MiddleCenter;
			this.labelLicLicData.AutoSize = true;
			this.labelLicLicData.BackColor = Color.Transparent;
			this.labelLicLicData.Font = new System.Drawing.Font("Tahoma", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.labelLicLicData.Location = new Point(226, 165);
			this.labelLicLicData.Name = "labelLicLicData";
			this.labelLicLicData.Size = new System.Drawing.Size(32, 13);
			this.labelLicLicData.TabIndex = 8;
			this.labelLicLicData.Text = "num";
			this.labelLicMaintenanceExp.AutoSize = true;
			this.labelLicMaintenanceExp.BackColor = Color.Transparent;
			this.labelLicMaintenanceExp.Font = new System.Drawing.Font("Tahoma", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.labelLicMaintenanceExp.Location = new Point(226, 143);
			this.labelLicMaintenanceExp.Name = "labelLicMaintenanceExp";
			this.labelLicMaintenanceExp.Size = new System.Drawing.Size(33, 13);
			this.labelLicMaintenanceExp.TabIndex = 6;
			this.labelLicMaintenanceExp.Text = "date";
			this.label5.AutoSize = true;
			this.label5.BackColor = Color.Transparent;
			this.label5.Location = new Point(53, 98);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(80, 13);
			this.label5.TabIndex = 2;
			this.label5.Text = "License details:";
			this.labelLicExpiration.AutoSize = true;
			this.labelLicExpiration.BackColor = Color.Transparent;
			this.labelLicExpiration.Font = new System.Drawing.Font("Tahoma", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.labelLicExpiration.Location = new Point(226, 121);
			this.labelLicExpiration.Name = "labelLicExpiration";
			this.labelLicExpiration.Size = new System.Drawing.Size(33, 13);
			this.labelLicExpiration.TabIndex = 4;
			this.labelLicExpiration.Text = "date";
			this.labelUsedDataText.AutoSize = true;
			this.labelUsedDataText.BackColor = Color.Transparent;
			this.labelUsedDataText.Location = new Point(68, 187);
			this.labelUsedDataText.Name = "labelUsedDataText";
			this.labelUsedDataText.Size = new System.Drawing.Size(109, 13);
			this.labelUsedDataText.TabIndex = 9;
			this.labelUsedDataText.Text = "Size of used storage:";
			this.labelLicenseExpirationText.AutoSize = true;
			this.labelLicenseExpirationText.BackColor = Color.Transparent;
			this.labelLicenseExpirationText.Location = new Point(68, 121);
			this.labelLicenseExpirationText.Name = "labelLicenseExpirationText";
			this.labelLicenseExpirationText.Size = new System.Drawing.Size(95, 13);
			this.labelLicenseExpirationText.TabIndex = 3;
			this.labelLicenseExpirationText.Text = "License expires on";
			this.labelLicensedDataText.AutoSize = true;
			this.labelLicensedDataText.BackColor = Color.Transparent;
			this.labelLicensedDataText.Location = new Point(68, 165);
			this.labelLicensedDataText.Name = "labelLicensedDataText";
			this.labelLicensedDataText.Size = new System.Drawing.Size(124, 13);
			this.labelLicensedDataText.TabIndex = 7;
			this.labelLicensedDataText.Text = "Size of licensed storage:";
			this.label16.AutoSize = true;
			this.label16.BackColor = Color.Transparent;
			this.label16.Location = new Point(68, 143);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(125, 13);
			this.label16.TabIndex = 5;
			this.label16.Text = "Maintenance expires on:";
			this.activationPanel.Controls.Add(this.linkLabelSetProxy);
			this.activationPanel.Controls.Add(this.panel2);
			this.activationPanel.Controls.Add(this.label2);
			this.activationPanel.Controls.Add(this.radioButtonOnline);
			this.activationPanel.Controls.Add(this.radioButtonOffline);
			this.activationPanel.Location = new Point(0, 56);
			this.activationPanel.Name = "activationPanel";
			this.activationPanel.Size = new System.Drawing.Size(427, 273);
			this.activationPanel.TabIndex = 22;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = Color.LightGray;
			base.ClientSize = new System.Drawing.Size(441, 386);
			base.Controls.Add(this.tabControl1);
			base.Controls.Add(this.panel1);
			this.Font = new System.Drawing.Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "LicenseNewKeyWizard";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Activate License Key";
			base.FormClosing += new FormClosingEventHandler(this.LicenseNewKeyWizard_FormClosing);
			base.Shown += new EventHandler(this.On_Shown);
			base.KeyDown += new KeyEventHandler(this.On_OfflineRequest_KeyDown);
			this.panel1.ResumeLayout(false);
			this.contextMenuTextbox.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabPageLicKey.ResumeLayout(false);
			this.tabPageLicKey.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.panelNewKeyInputs.ResumeLayout(false);
			this.panelNewKeyInputs.PerformLayout();
			((ISupportInitialize)this.pictureBox1).EndInit();
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			((ISupportInitialize)this.pictureBox3).EndInit();
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			((ISupportInitialize)this.pictureBox4).EndInit();
			this.tabPage3.ResumeLayout(false);
			this.tabPage3.PerformLayout();
			((ISupportInitialize)this.pictureBox2).EndInit();
			this.activationPanel.ResumeLayout(false);
			this.activationPanel.PerformLayout();
			base.ResumeLayout(false);
		}

		private static bool IsLicenseException(Exception ex)
		{
			return ex is BaseLicenseException;
		}

		private bool IsValidLicenseKey(string text, bool isOldStyleLicense)
		{
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			if (isOldStyleLicense)
			{
				return text.Length > 200;
			}
			Metalogix.Licensing.LicenseServer.LicenseKey licenseKey = new Metalogix.Licensing.LicenseServer.LicenseKey(text);
			if (!licenseKey.IsValid)
			{
				return false;
			}
			ILicensingDialogServiceProvider licensingDialogServiceProvider = this._server;
			int productCode = licenseKey.ProductCode;
			return licensingDialogServiceProvider.IsKeyLegitimate(productCode.ToString(), licenseKey.Value);
		}

		private void licenseActivationModeChanged(object sender, EventArgs e)
		{
			this.linkLabelSetProxy.Enabled = this.radioButtonOnline.Checked;
		}

		private void LicenseNewKeyWizard_FormClosing(object sender, FormClosingEventArgs e)
		{
			base.DialogResult = (this.CurrentPage == LicenseNewKeyWizard.Page.Finish ? System.Windows.Forms.DialogResult.OK : System.Windows.Forms.DialogResult.Cancel);
		}

		private void linkLabelActivationLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				Process.Start(this.textBoxActivationLink.Text);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				GlobalServices.GetErrorHandlerAs<Metalogix.UI.WinForms.ErrorHandler>().HandleException(this, this._server.DialogTitle, "Failed to open website.", exception, ErrorIcon.Error);
				Trace.WriteLine(string.Concat("LicenseNewKeyWizard >> Failed to open website: ", exception));
			}
		}

		private void linkLabelLoadFromFile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				OpenFileDialog openFileDialog = new OpenFileDialog()
				{
					DefaultExt = "dat",
					Filter = "Activation response file (*.dat)|*.dat|All files (*.*)|*.*",
					FileName = "LicenseActivationResponse.dat"
				};
				OpenFileDialog openFileDialog1 = openFileDialog;
				if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					using (StreamReader streamReader = new StreamReader(openFileDialog1.FileName))
					{
						this.textBoxOfflineResponse.Text = streamReader.ReadToEnd();
					}
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				GlobalServices.GetErrorHandlerAs<Metalogix.UI.WinForms.ErrorHandler>().HandleException(this, this._server.DialogTitle, "Failed to load activation response", exception, ErrorIcon.Error);
				Trace.WriteLine(string.Concat("LicenseNewKeyWizard >> Failed to get activation data: ", exception));
			}
		}

		private void linkLabelSaveReqToFile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog()
				{
					DefaultExt = "dat",
					Filter = "Activation request file (*.dat)|*.dat|All files (*.*)|*.*",
					FileName = "LicenseActivationRequest.dat",
					AddExtension = true
				};
				SaveFileDialog saveFileDialog1 = saveFileDialog;
				if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					using (StreamWriter streamWriter = new StreamWriter(saveFileDialog1.FileName, false))
					{
						streamWriter.Write(this.textBoxOfflineRequest.Text);
					}
					FlatXtraMessageBox.Show(this, "Activation request successfully saved to the file.", this._server.DialogTitle, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				GlobalServices.GetErrorHandlerAs<Metalogix.UI.WinForms.ErrorHandler>().HandleException(this, this._server.DialogTitle, "Failed to load activation response.", exception, ErrorIcon.Error);
				Trace.WriteLine(string.Concat("LicenseNewKeyWizard >> Failed to get activation data: ", exception));
			}
		}

		private void linkLabelSetProxy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				(new LicenseProxyDlg(this._server)).ShowDialog();
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				FlatXtraMessageBox.Show(this, "Failed to set proxy.", this._server.DialogTitle, MessageBoxButtons.OK, MessageBoxIcon.Hand);
				Trace.WriteLine(string.Concat("LicenseNewKeyWizard >> Failed to set proxy: ", exception));
			}
		}

		private void On_OfflineLink_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Modifiers == Keys.Control && e.KeyCode == Keys.A)
			{
				this.textBoxActivationLink.SelectAll();
			}
		}

		private void On_OfflineRequest_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Modifiers == Keys.Control && e.KeyCode == Keys.A)
			{
				this.textBoxOfflineRequest.SelectAll();
			}
		}

		private void On_Shown(object sender, EventArgs e)
		{
			base.BringToFront();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.radioButtonNewKey.Select();
			this.linkLabelSetProxy.Visible = this._server.AllowSettingProxy;
			this.licenseActivationModeChanged(this, EventArgs.Empty);
		}

		private void PasteLicenseCode(string textToPaste, bool validateKey)
		{
			textToPaste = (textToPaste != null ? textToPaste.Trim() : string.Empty);
			if (!this.UseOldKeyActivation && validateKey && (textToPaste.Length < 25 || !this._server.IsKeyLegitimate(textToPaste.Substring(0, 3), textToPaste)))
			{
				FlatXtraMessageBox.Show(this, "The key in the clipboard is not valid.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				return;
			}
			if (this.UseOldKeyActivation && validateKey && textToPaste.Length < 200)
			{
				FlatXtraMessageBox.Show(this, "The key in the clipboard is not valid.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				return;
			}
			if (this.UseOldKeyActivation)
			{
				this.textBoxOldKey.Text = textToPaste;
				return;
			}
			string str = this.pastedTextBox;
			string str1 = str;
			if (str != null)
			{
				if (str1 == "textBoxLic1")
				{
					this.pastedText = textToPaste.Substring(0, 5);
				}
				else if (str1 == "textBoxLic2")
				{
					this.pastedText = (textToPaste.Length == 25 ? textToPaste.Substring(5, 5) : textToPaste.Substring(6, 5));
				}
				else if (str1 == "textBoxLic3")
				{
					this.pastedText = (textToPaste.Length == 25 ? textToPaste.Substring(10, 5) : textToPaste.Substring(12, 5));
				}
				else if (str1 == "textBoxLic4")
				{
					this.pastedText = (textToPaste.Length == 25 ? textToPaste.Substring(15, 5) : textToPaste.Substring(18, 5));
				}
				else if (str1 == "textBoxLic5")
				{
					this.pastedText = (textToPaste.Length == 25 ? textToPaste.Substring(20, 5) : textToPaste.Substring(24, 5));
				}
			}
			this.textBoxLic1.Text = textToPaste.Substring(0, 5);
			this.textBoxLic2.Text = (textToPaste.Length == 25 ? textToPaste.Substring(5, 5) : textToPaste.Substring(6, 5));
			this.textBoxLic3.Text = (textToPaste.Length == 25 ? textToPaste.Substring(10, 5) : textToPaste.Substring(12, 5));
			this.textBoxLic4.Text = (textToPaste.Length == 25 ? textToPaste.Substring(15, 5) : textToPaste.Substring(18, 5));
			this.textBoxLic5.Text = (textToPaste.Length == 25 ? textToPaste.Substring(20, 5) : textToPaste.Substring(24, 5));
		}

		private void radioButtonNewKey_CheckedChanged(object sender, EventArgs e)
		{
			this.UseOldKeyActivation = false;
		}

		private void radioButtonOldKey_CheckedChanged(object sender, EventArgs e)
		{
			this.UseOldKeyActivation = true;
		}

		private void roundedButtonBack_Click(object sender, EventArgs e)
		{
			switch (this.CurrentPage)
			{
				case LicenseNewKeyWizard.Page.EnterLicense:
				{
					this.SetupPage();
					return;
				}
				case LicenseNewKeyWizard.Page.ActivationRequest:
				{
					this.CurrentPage = LicenseNewKeyWizard.Page.EnterLicense;
					this.SetupPage();
					return;
				}
				case LicenseNewKeyWizard.Page.ActivationResponse:
				{
					this.CurrentPage = LicenseNewKeyWizard.Page.ActivationRequest;
					this.SetupPage();
					return;
				}
				case LicenseNewKeyWizard.Page.Finish:
				{
					this.CurrentPage = (this.radioButtonOnline.Checked ? LicenseNewKeyWizard.Page.EnterLicense : LicenseNewKeyWizard.Page.ActivationRequest);
					this.SetupPage();
					return;
				}
			}
			throw new NotImplementedException("The given page functionality was not implemented");
		}

		private void roundedButtonClose_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void roundedButtonNext_Click(object sender, EventArgs e)
		{
			if (!this.ValidatePage())
			{
				return;
			}
			switch (this.CurrentPage)
			{
				case LicenseNewKeyWizard.Page.EnterLicense:
				{
					this.CurrentPage = (this.radioButtonOnline.Checked ? LicenseNewKeyWizard.Page.Finish : LicenseNewKeyWizard.Page.ActivationRequest);
					break;
				}
				case LicenseNewKeyWizard.Page.ActivationRequest:
				{
					this.CurrentPage = LicenseNewKeyWizard.Page.ActivationResponse;
					break;
				}
				case LicenseNewKeyWizard.Page.ActivationResponse:
				{
					this.CurrentPage = LicenseNewKeyWizard.Page.Finish;
					break;
				}
				case LicenseNewKeyWizard.Page.Finish:
				{
					base.DialogResult = System.Windows.Forms.DialogResult.OK;
					base.Close();
					break;
				}
				default:
				{
					throw new NotImplementedException("The given page functionality was not implemented");
				}
			}
			this.SetupPage();
		}

		private void SetupPage()
		{
			bool currentPage = this.CurrentPage == LicenseNewKeyWizard.Page.EnterLicense;
			bool flag = this.CurrentPage == LicenseNewKeyWizard.Page.Finish;
			this.roundedButtonBack.Visible = (currentPage ? false : !flag);
			this.roundedButtonNext.Visible = !flag;
			this.roundedButtonClose.Text = (flag ? "Finish" : "Close");
		}

		private void ShowLicenseInfo()
		{
			MLLicenseCommon licenseInformation;
			try
			{
				using (AutoWaitCursor autoWaitCursor = AutoWaitCursor.Create(this))
				{
					licenseInformation = this._server.GetLicenseInformation();
				}
				Label str = this.labelLicExpiration;
				DateTime expirationDate = licenseInformation.ExpirationDate;
				str.Text = expirationDate.ToString("dddd, dd MMMM yyyy", CultureInfo.InvariantCulture);
				Label label = this.labelLicMaintenanceExp;
				DateTime maintenanceExpirationDate = licenseInformation.MaintenanceExpirationDate;
				label.Text = maintenanceExpirationDate.ToString("dddd, dd MMMM yyyy", CultureInfo.InvariantCulture);
				this.labelLicLicData.Text = (licenseInformation.LicensedData >= (long)0 ? licenseInformation.FormatUsageData(licenseInformation.LicensedData) : "Unlimited");
				this.labelLicUsedData.Text = licenseInformation.FormatUsageData(licenseInformation.UsedDataFull);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Trace.Write(string.Concat("LicenseNewKeyWizard >> ShowLicenseInfo: ", exception));
				GlobalServices.GetErrorHandlerAs<Metalogix.UI.WinForms.ErrorHandler>().HandleException(this, this._server.DialogTitle, "Failed to show license informations.", exception, ErrorIcon.Error);
			}
		}

		private void StripMenuItemCopy_Click(object sender, EventArgs e)
		{
			try
			{
				TextBox sourceTextBox = LicenseNewKeyWizard.GetSourceTextBox(sender as ToolStripMenuItem);
				if (sourceTextBox != null)
				{
					Clipboard.SetDataObject(sourceTextBox.Text);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				GlobalServices.GetErrorHandlerAs<Metalogix.UI.WinForms.ErrorHandler>().HandleException(this, this._server.DialogTitle, "Failed to copy to clipboard.", exception, ErrorIcon.Error);
				Logger.Error.Write("Failed to copy to clipboard.", exception);
			}
		}

		private void StripMenuItemCut_Click(object sender, EventArgs e)
		{
			try
			{
				TextBox sourceTextBox = LicenseNewKeyWizard.GetSourceTextBox(sender as ToolStripMenuItem);
				if (sourceTextBox != null)
				{
					Clipboard.SetDataObject(sourceTextBox.Text);
					sourceTextBox.Text = "";
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				GlobalServices.GetErrorHandlerAs<Metalogix.UI.WinForms.ErrorHandler>().HandleException(this, this._server.DialogTitle, "Failed to cut to clipboard.", exception, ErrorIcon.Error);
				Logger.Error.Write("Failed to cut to clipboard.", exception);
			}
		}

		private void StripMenuItemPaste_Click(object sender, EventArgs e)
		{
			try
			{
				IDataObject dataObject = Clipboard.GetDataObject();
				if (dataObject != null && dataObject.GetDataPresent(DataFormats.Text))
				{
					string str = dataObject.GetData(DataFormats.Text).ToString();
					TextBox sourceTextBox = LicenseNewKeyWizard.GetSourceTextBox(sender as ToolStripMenuItem);
					if (sourceTextBox == null)
					{
						return;
					}
					else if (sourceTextBox == this.textBoxOfflineRequest || sourceTextBox == this.textBoxOfflineResponse)
					{
						sourceTextBox.Text = str;
					}
					else
					{
						this.pastedTextBox = sourceTextBox.Name;
						this.PasteLicenseCode(str, true);
					}
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				GlobalServices.GetErrorHandlerAs<Metalogix.UI.WinForms.ErrorHandler>().HandleException(this, this._server.DialogTitle, "Failed to paste from clipboard.", exception, ErrorIcon.Error);
				Logger.Error.Write("Failed to paste from clipboard.", exception);
			}
		}

		private void tabControl1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
			{
				try
				{
					string[] text = new string[] { this.textBoxLic1.Text, this.textBoxLic2.Text, this.textBoxLic3.Text, this.textBoxLic4.Text, this.textBoxLic5.Text };
					string str = string.Concat(text);
					switch (this.CurrentPage)
					{
						case LicenseNewKeyWizard.Page.EnterLicense:
						{
							if (str != string.Empty)
							{
								this.roundedButtonNext_Click("enter", new EventArgs());
								break;
							}
							else if (!this.radioButtonOnline.Checked)
							{
								this.roundedButtonNext_Click("enter", new EventArgs());
								break;
							}
							else
							{
								base.Close();
								break;
							}
						}
						case LicenseNewKeyWizard.Page.ActivationRequest:
						{
							if (this.textBoxOfflineRequest.Text != string.Empty)
							{
								this.roundedButtonNext_Click("enter", new EventArgs());
								break;
							}
							else
							{
								base.Close();
								break;
							}
						}
						case LicenseNewKeyWizard.Page.ActivationResponse:
						{
							if (this.textBoxOfflineResponse.Text != string.Empty)
							{
								this.roundedButtonNext_Click("enter", new EventArgs());
								break;
							}
							else
							{
								base.Close();
								break;
							}
						}
						case LicenseNewKeyWizard.Page.Finish:
						{
							base.DialogResult = System.Windows.Forms.DialogResult.OK;
							base.Close();
							break;
						}
						default:
						{
							throw new NotImplementedException("The given page functionality was not implemented");
						}
					}
				}
				catch (Exception exception)
				{
					GlobalServices.GetErrorHandlerAs<Metalogix.UI.WinForms.ErrorHandler>().HandleException(this, "The given page functionality was not implemented", exception);
				}
			}
			if (e.KeyCode == Keys.Escape && FlatXtraMessageBox.Show("Are you sure to quit License Key Wizard?", "Do you want to do quit?", MessageBoxButtons.YesNoCancel) == System.Windows.Forms.DialogResult.Yes)
			{
				base.Close();
			}
		}

		private void textBoxLic1_KeyDown(object sender, KeyEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.V && (e.Modifiers & Keys.Control) != Keys.None)
				{
					IDataObject dataObject = Clipboard.GetDataObject();
					if (dataObject != null && dataObject.GetDataPresent(DataFormats.Text))
					{
						e.Handled = true;
						string str = dataObject.GetData(DataFormats.Text).ToString().Trim();
						this.pasted = true;
						TextBox textBox = sender as TextBox;
						if (textBox != null && (str.Length == 25 || str.Length == 29))
						{
							this.pastedTextBox = textBox.Name;
							this.PasteLicenseCode(str, true);
						}
					}
				}
			}
			catch (Exception exception)
			{
				Logger.Error.Write("Failed to handle keydown on key textbox.", exception);
			}
		}

		private void textBoxLic1_TextChanged(object sender, EventArgs e)
		{
			try
			{
				TextBox textBox = sender as TextBox;
				if (textBox != null)
				{
					if (this.pasted && this.pastedTextBox == textBox.Name)
					{
						textBox.Text = this.pastedText;
						this.pasted = false;
					}
					else if (textBox.Text.Length >= 5)
					{
						string name = textBox.Name;
						string str = name;
						if (name != null)
						{
							if (str == "textBoxLic1")
							{
								this.textBoxLic2.Focus();
							}
							else if (str == "textBoxLic2")
							{
								this.textBoxLic3.Focus();
							}
							else if (str == "textBoxLic3")
							{
								this.textBoxLic4.Focus();
							}
							else if (str == "textBoxLic4")
							{
								this.textBoxLic5.Focus();
							}
						}
					}
				}
			}
			catch (Exception exception)
			{
				Logger.Error.Write("Failed to handle text changed on key textbox.", exception);
			}
		}

		private bool ValidatePage()
		{
			switch (this.CurrentPage)
			{
				case LicenseNewKeyWizard.Page.EnterLicense:
				{
					if (this.radioButtonOffline.Checked)
					{
						return this.GenerateOfflineRequest();
					}
					if (!this.ActivateLicenseOnline())
					{
						return false;
					}
					this.ShowLicenseInfo();
					return true;
				}
				case LicenseNewKeyWizard.Page.ActivationRequest:
				{
					return true;
				}
				case LicenseNewKeyWizard.Page.ActivationResponse:
				{
					if (!this.ActivateLicenseOffline())
					{
						return false;
					}
					this.ShowLicenseInfo();
					return true;
				}
				case LicenseNewKeyWizard.Page.Finish:
				{
					return true;
				}
			}
			throw new NotImplementedException("The given page functionality was not implemented");
		}

		private struct LicenseKey
		{
			public bool IsValid;

			public string Value;
		}

		private enum Page
		{
			EnterLicense,
			ActivationRequest,
			ActivationResponse,
			Finish
		}

		public class WizardControl : TabControl
		{
			public WizardControl()
			{
			}

			protected override void OnKeyDown(KeyEventArgs e)
			{
				if (e.KeyData == (Keys.LButton | Keys.Back | Keys.Tab | Keys.Control) || e.KeyData == (Keys.RButton | Keys.Space | Keys.Next | Keys.PageDown | Keys.Control) || e.KeyData == (Keys.LButton | Keys.Space | Keys.Prior | Keys.PageUp | Keys.Control))
				{
					return;
				}
				base.OnKeyDown(e);
			}

			protected override void WndProc(ref Message m)
			{
				if (m.Msg != 4904 || base.DesignMode)
				{
					base.WndProc(ref m);
					return;
				}
				m.Result = (IntPtr)1;
			}
		}
	}
}