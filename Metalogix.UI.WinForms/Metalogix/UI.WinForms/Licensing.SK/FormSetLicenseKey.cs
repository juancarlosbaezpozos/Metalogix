using Metalogix;
using Metalogix.Licensing;
using Metalogix.Licensing.SK;
using Metalogix.UI.WinForms;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Licensing.SK
{
	public class FormSetLicenseKey : Form
	{
		private string pastedText = "";

		private string pastedTextBox = "";

		private bool pasted;

		private MLLicenseSKBase _license;

		private IContainer components;

		private Label lblKey;

		private TextBox textBoxLic1;

		private TextBox textBoxLic2;

		private TextBox textBoxLic3;

		private TextBox textBoxLic4;

		private TextBox textBoxLic5;

		private Button buttonSet;

		private Button buttonCancel;

		private Button buttonProxy;

		private System.Windows.Forms.ContextMenuStrip contextMenuTextbox;

		private ToolStripMenuItem StripMenuItemCopy;

		private ToolStripMenuItem StripMenuItemCut;

		private ToolStripMenuItem StripMenuItemPaste;

		private Label label1;

		public MLLicenseSKBase License
		{
			get
			{
				return this._license;
			}
		}

		public FormSetLicenseKey() : this(null)
		{
		}

		public FormSetLicenseKey(MLLicenseSKBase license)
		{
			this.InitializeComponent();
			this.label1.Text = SKLP.Get.InitData.SetLicenseText;
			string licenseKey = SKLP.Get.GetLicense().LicenseKey;
			if (!string.IsNullOrEmpty(licenseKey))
			{
				this.PasteLicenseCode(licenseKey);
			}
		}

		private void buttonProxy_Click(object sender, EventArgs e)
		{
			try
			{
				using (FormProxySetup formProxySetup = new FormProxySetup(((MLLicenseProviderSK)MLLicenseProvider.Instance).Proxy))
				{
					if (formProxySetup.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
					{
						SKLP.Get.SetProxy(formProxySetup.Proxy);
					}
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Trace.WriteLine(string.Concat("FormSetLicenseKey >> buttonProxy_Click: ", exception.ToString()));
				GlobalServices.GetErrorHandlerAs<Metalogix.UI.WinForms.ErrorHandler>().HandleException(this, this.Text, string.Concat("An error occured: ", exception.Message), exception, ErrorIcon.Error);
			}
		}

		private void buttonSet_Click(object sender, EventArgs e)
		{
			try
			{
				string[] text = new string[] { this.textBoxLic1.Text, "-", this.textBoxLic2.Text, "-", this.textBoxLic3.Text, "-", this.textBoxLic4.Text, "-", this.textBoxLic5.Text };
				string str = string.Concat(text);
				MLLicenseSKBase mLLicenseSKBase = SKLP.Get.CreateLicense(str);
				if (mLLicenseSKBase.LicenseType != MLLicenseType.Invalid)
				{
					MLLicenseSKBase license = SKLP.Get.GetLicense();
					if (!license.IsSet || string.Compare(license.LicenseKey, mLLicenseSKBase.LicenseKey, StringComparison.OrdinalIgnoreCase) != 0)
					{
						try
						{
							LicenseProxy proxy = SKLP.Get.Proxy;
							if (!SKLP.Get.SetLicense(mLLicenseSKBase, proxy))
							{
								FlatXtraMessageBox.Show("The entered license key is invalid. Please enter a valid license key.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Hand);
								return;
							}
						}
						catch (Exception exception1)
						{
							Exception exception = exception1;
							GlobalServices.GetErrorHandlerAs<Metalogix.UI.WinForms.ErrorHandler>().HandleException(this, this.Text, exception.Message, exception, ErrorIcon.Error);
							return;
						}
						this._license = mLLicenseSKBase;
					}
					else
					{
						this._license = license;
					}
					base.DialogResult = System.Windows.Forms.DialogResult.OK;
					base.Close();
				}
				else
				{
					FlatXtraMessageBox.Show(this, "Please, enter a valid key first.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
					base.DialogResult = System.Windows.Forms.DialogResult.None;
				}
			}
			catch (Exception exception3)
			{
				Exception exception2 = exception3;
				GlobalServices.GetErrorHandlerAs<Metalogix.UI.WinForms.ErrorHandler>().HandleException(this, this.Text, "The entered license key is invalid. Please enter a valid license key.", exception2, ErrorIcon.Error);
				Trace.WriteLine(string.Concat("Failed to set the license key. Error: ", exception2.ToString()));
			}
		}

		private void contextMenuTextbox_Popup(object sender, EventArgs e)
		{
			try
			{
				TextBox sourceControl = (sender as System.Windows.Forms.ContextMenuStrip).SourceControl as TextBox;
				this.StripMenuItemCopy.Enabled = sourceControl.Text != "";
				this.StripMenuItemCut.Enabled = sourceControl.Text != "";
				IDataObject dataObject = Clipboard.GetDataObject();
				if (!dataObject.GetDataPresent(DataFormats.Text))
				{
					this.StripMenuItemPaste.Enabled = false;
				}
				else
				{
					string str = dataObject.GetData(DataFormats.Text).ToString();
					this.StripMenuItemPaste.Enabled = (str.Length == 25 ? true : str.Length == 29);
				}
			}
			catch (Exception exception)
			{
				Trace.WriteLine(string.Concat("FormLicenseKey >> contextMenuTextbox_Popup:", exception.ToString()));
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

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(FormSetLicenseKey));
			this.lblKey = new Label();
			this.textBoxLic1 = new TextBox();
			this.contextMenuTextbox = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.StripMenuItemCopy = new ToolStripMenuItem();
			this.StripMenuItemCut = new ToolStripMenuItem();
			this.StripMenuItemPaste = new ToolStripMenuItem();
			this.textBoxLic2 = new TextBox();
			this.textBoxLic3 = new TextBox();
			this.textBoxLic4 = new TextBox();
			this.textBoxLic5 = new TextBox();
			this.buttonSet = new Button();
			this.buttonCancel = new Button();
			this.buttonProxy = new Button();
			this.label1 = new Label();
			this.contextMenuTextbox.SuspendLayout();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.lblKey, "lblKey");
			this.lblKey.BackColor = Color.Transparent;
			this.lblKey.Name = "lblKey";
			this.textBoxLic1.BackColor = Color.White;
			this.textBoxLic1.BorderStyle = BorderStyle.FixedSingle;
			this.textBoxLic1.ContextMenuStrip = this.contextMenuTextbox;
			componentResourceManager.ApplyResources(this.textBoxLic1, "textBoxLic1");
			this.textBoxLic1.Name = "textBoxLic1";
			this.textBoxLic1.TextChanged += new EventHandler(this.textBoxLic1_TextChanged);
			this.textBoxLic1.KeyDown += new KeyEventHandler(this.textBoxLic1_KeyDown);
			ToolStripItemCollection items = this.contextMenuTextbox.Items;
			ToolStripItem[] stripMenuItemCopy = new ToolStripItem[] { this.StripMenuItemCopy, this.StripMenuItemCut, this.StripMenuItemPaste };
			items.AddRange(stripMenuItemCopy);
			this.contextMenuTextbox.Name = "contextMenuTextbox";
			componentResourceManager.ApplyResources(this.contextMenuTextbox, "contextMenuTextbox");
			this.contextMenuTextbox.Opened += new EventHandler(this.contextMenuTextbox_Popup);
			this.StripMenuItemCopy.Name = "StripMenuItemCopy";
			componentResourceManager.ApplyResources(this.StripMenuItemCopy, "StripMenuItemCopy");
			this.StripMenuItemCopy.Click += new EventHandler(this.menuItemCopy_Click);
			this.StripMenuItemCut.Name = "StripMenuItemCut";
			componentResourceManager.ApplyResources(this.StripMenuItemCut, "StripMenuItemCut");
			this.StripMenuItemCut.Click += new EventHandler(this.menuItemCut_Click);
			this.StripMenuItemPaste.Name = "StripMenuItemPaste";
			componentResourceManager.ApplyResources(this.StripMenuItemPaste, "StripMenuItemPaste");
			this.StripMenuItemPaste.Click += new EventHandler(this.menuItemPaste_Click);
			this.textBoxLic2.BackColor = Color.White;
			this.textBoxLic2.BorderStyle = BorderStyle.FixedSingle;
			this.textBoxLic2.ContextMenuStrip = this.contextMenuTextbox;
			componentResourceManager.ApplyResources(this.textBoxLic2, "textBoxLic2");
			this.textBoxLic2.Name = "textBoxLic2";
			this.textBoxLic2.TextChanged += new EventHandler(this.textBoxLic1_TextChanged);
			this.textBoxLic2.KeyDown += new KeyEventHandler(this.textBoxLic1_KeyDown);
			this.textBoxLic3.BackColor = Color.White;
			this.textBoxLic3.BorderStyle = BorderStyle.FixedSingle;
			this.textBoxLic3.ContextMenuStrip = this.contextMenuTextbox;
			componentResourceManager.ApplyResources(this.textBoxLic3, "textBoxLic3");
			this.textBoxLic3.Name = "textBoxLic3";
			this.textBoxLic3.TextChanged += new EventHandler(this.textBoxLic1_TextChanged);
			this.textBoxLic3.KeyDown += new KeyEventHandler(this.textBoxLic1_KeyDown);
			this.textBoxLic4.BackColor = Color.White;
			this.textBoxLic4.BorderStyle = BorderStyle.FixedSingle;
			this.textBoxLic4.ContextMenuStrip = this.contextMenuTextbox;
			componentResourceManager.ApplyResources(this.textBoxLic4, "textBoxLic4");
			this.textBoxLic4.Name = "textBoxLic4";
			this.textBoxLic4.TextChanged += new EventHandler(this.textBoxLic1_TextChanged);
			this.textBoxLic4.KeyDown += new KeyEventHandler(this.textBoxLic1_KeyDown);
			this.textBoxLic5.BackColor = Color.White;
			this.textBoxLic5.BorderStyle = BorderStyle.FixedSingle;
			this.textBoxLic5.ContextMenuStrip = this.contextMenuTextbox;
			componentResourceManager.ApplyResources(this.textBoxLic5, "textBoxLic5");
			this.textBoxLic5.Name = "textBoxLic5";
			this.textBoxLic5.TextChanged += new EventHandler(this.textBoxLic1_TextChanged);
			this.textBoxLic5.KeyDown += new KeyEventHandler(this.textBoxLic1_KeyDown);
			componentResourceManager.ApplyResources(this.buttonSet, "buttonSet");
			this.buttonSet.Name = "buttonSet";
			this.buttonSet.UseVisualStyleBackColor = true;
			this.buttonSet.Click += new EventHandler(this.buttonSet_Click);
			componentResourceManager.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.buttonProxy, "buttonProxy");
			this.buttonProxy.Name = "buttonProxy";
			this.buttonProxy.UseVisualStyleBackColor = true;
			this.buttonProxy.Click += new EventHandler(this.buttonProxy_Click);
			this.label1.BackColor = Color.Transparent;
			componentResourceManager.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.label1);
			base.Controls.Add(this.buttonProxy);
			base.Controls.Add(this.buttonCancel);
			base.Controls.Add(this.buttonSet);
			base.Controls.Add(this.lblKey);
			base.Controls.Add(this.textBoxLic1);
			base.Controls.Add(this.textBoxLic2);
			base.Controls.Add(this.textBoxLic3);
			base.Controls.Add(this.textBoxLic4);
			base.Controls.Add(this.textBoxLic5);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			base.Name = "FormSetLicenseKey";
			base.KeyDown += new KeyEventHandler(this.textBoxLic1_KeyDown);
			this.contextMenuTextbox.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void menuItemCopy_Click(object sender, EventArgs e)
		{
			try
			{
				TextBox sourceControl = ((sender as ToolStripMenuItem).GetCurrentParent() as System.Windows.Forms.ContextMenuStrip).SourceControl as TextBox;
				Clipboard.SetDataObject(sourceControl.Text);
			}
			catch (Exception exception)
			{
				Trace.WriteLine(string.Concat("FormLicenseKey >> menuItemCopy_Click:", exception.ToString()));
			}
		}

		private void menuItemCut_Click(object sender, EventArgs e)
		{
			try
			{
				TextBox sourceControl = ((sender as ToolStripMenuItem).GetCurrentParent() as System.Windows.Forms.ContextMenuStrip).SourceControl as TextBox;
				Clipboard.SetDataObject(sourceControl.Text);
				sourceControl.Text = "";
			}
			catch (Exception exception)
			{
				Trace.WriteLine(string.Concat("FormLicenseKey >> menuItemCut_Click:", exception.ToString()));
			}
		}

		private void menuItemPaste_Click(object sender, EventArgs e)
		{
			try
			{
				IDataObject dataObject = Clipboard.GetDataObject();
				if (dataObject.GetDataPresent(DataFormats.Text))
				{
					string str = dataObject.GetData(DataFormats.Text).ToString();
					this.pasted = true;
					this.pastedTextBox = (((sender as ToolStripMenuItem).GetCurrentParent() as System.Windows.Forms.ContextMenuStrip).SourceControl as TextBox).Name;
					if (SKLP.Get.CreateLicense(str).LicenseType != MLLicenseType.Invalid)
					{
						this.PasteLicenseCode(str);
					}
					else
					{
						FlatXtraMessageBox.Show(this, "The key in the clipboard is not valid.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
						return;
					}
				}
			}
			catch (Exception exception)
			{
				Trace.WriteLine(string.Concat("FormLicenseKey >> menuItemPaste_Click:", exception.ToString()));
			}
		}

		private void PasteLicenseCode(string textToPaste)
		{
			try
			{
				if (!string.IsNullOrEmpty(textToPaste) && textToPaste.Length >= 5)
				{
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
			}
			catch (Exception exception)
			{
				Trace.WriteLine(string.Concat("FormLicenseKey >> PasteLicenseCode:", exception.ToString()));
			}
		}

		private void textBoxLic1_KeyDown(object sender, KeyEventArgs e)
		{
			try
			{
				if (e.KeyCode == Keys.V && (e.Modifiers & Keys.Control) != Keys.None)
				{
					IDataObject dataObject = Clipboard.GetDataObject();
					if (dataObject.GetDataPresent(DataFormats.Text))
					{
						e.Handled = true;
						string str = dataObject.GetData(DataFormats.Text).ToString().Trim();
						this.pasted = true;
						this.pastedTextBox = (sender as TextBox).Name;
						if (str.Length == 25 || str.Length == 29)
						{
							if (SKLP.Get.CreateLicense(str).LicenseType != MLLicenseType.Invalid)
							{
								this.PasteLicenseCode(str);
							}
							else
							{
								FlatXtraMessageBox.Show(this, "The key in the clipboard is not valid.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
								return;
							}
						}
					}
				}
			}
			catch (Exception exception)
			{
				Trace.WriteLine(string.Concat("FormLicenseKey >> textBoxLic1_KeyDown:", exception.ToString()));
			}
		}

		private void textBoxLic1_TextChanged(object sender, EventArgs e)
		{
			try
			{
				if (this.pasted && this.pastedTextBox == (sender as TextBox).Name)
				{
					(sender as TextBox).Text = this.pastedText;
					this.pasted = false;
				}
				else if ((sender as TextBox).Text.Length >= 5)
				{
					string name = (sender as TextBox).Name;
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
			catch (Exception exception)
			{
				Trace.WriteLine(string.Concat("FormLicenseKey >> textBoxLic1_TextChanged:", exception.ToString()));
			}
		}
	}
}