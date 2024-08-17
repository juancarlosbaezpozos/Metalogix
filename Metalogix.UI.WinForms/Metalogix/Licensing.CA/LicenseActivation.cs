using Metalogix;
using Metalogix.Interfaces;
using Metalogix.Licensing;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Metalogix.Licensing.CA
{
	public class LicenseActivation : Form
	{
		internal const int LONG_KEY_TEXTBOX_HEIGHT = 204;

		private Label w_lblError;

		private TextBox w_tbLicenseKeyShort;

		private Label lblLicenseKey;

		private Button w_btnAddKey;

		private Button btnCancel;

		private Label w_lblInstructions;

		private Label w_lblNote;

		private CheckBox w_cbNoInternetConnection;

		private TextBox w_tbLicenseKeyLong;

		private Label w_lblOfflineInstructions;

		private Label w_lblMetalogixProduct;

		private string m_sProductTitle;

		private string m_sProductCode;

		private System.ComponentModel.Container components;

		private string m_sLicenseKey;

		public string LicenseKey
		{
			get
			{
				return this.m_sLicenseKey;
			}
		}

		private LicenseActivation()
		{
		}

		public LicenseActivation(string sProductTitle, string sProductCode)
		{
			this.InitializeComponent();
			base.CenterToScreen();
			int x = base.Location.X;
			Point location = base.Location;
			base.Location = new Point(x, location.Y - 102);
			this.m_sProductTitle = sProductTitle;
			this.m_sProductCode = sProductCode;
			this.Text = this.Text.Replace("MetalogixProduct", sProductTitle);
			this.w_lblMetalogixProduct.Text = this.w_lblMetalogixProduct.Text.Replace("MetalogixProduct", sProductTitle);
			if (Application.OpenForms.Count > 0 && Application.OpenForms[0].Visible)
			{
				base.ShowInTaskbar = false;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(LicenseActivation));
			this.btnCancel = new Button();
			this.w_lblError = new Label();
			this.w_tbLicenseKeyShort = new TextBox();
			this.lblLicenseKey = new Label();
			this.w_btnAddKey = new Button();
			this.w_lblInstructions = new Label();
			this.w_lblNote = new Label();
			this.w_cbNoInternetConnection = new CheckBox();
			this.w_tbLicenseKeyLong = new TextBox();
			this.w_lblOfflineInstructions = new Label();
			this.w_lblMetalogixProduct = new Label();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			componentResourceManager.ApplyResources(this.w_lblError, "w_lblError");
			this.w_lblError.ForeColor = Color.Crimson;
			this.w_lblError.Name = "w_lblError";
			componentResourceManager.ApplyResources(this.w_tbLicenseKeyShort, "w_tbLicenseKeyShort");
			this.w_tbLicenseKeyShort.Name = "w_tbLicenseKeyShort";
			this.w_tbLicenseKeyShort.TextChanged += new EventHandler(this.On_LicenseKey_TextChanged);
			componentResourceManager.ApplyResources(this.lblLicenseKey, "lblLicenseKey");
			this.lblLicenseKey.Name = "lblLicenseKey";
			componentResourceManager.ApplyResources(this.w_btnAddKey, "w_btnAddKey");
			this.w_btnAddKey.Name = "w_btnAddKey";
			this.w_btnAddKey.Click += new EventHandler(this.On_EnterKey_Click);
			componentResourceManager.ApplyResources(this.w_lblInstructions, "w_lblInstructions");
			this.w_lblInstructions.Name = "w_lblInstructions";
			componentResourceManager.ApplyResources(this.w_lblNote, "w_lblNote");
			this.w_lblNote.Name = "w_lblNote";
			componentResourceManager.ApplyResources(this.w_cbNoInternetConnection, "w_cbNoInternetConnection");
			this.w_cbNoInternetConnection.Name = "w_cbNoInternetConnection";
			this.w_cbNoInternetConnection.UseVisualStyleBackColor = true;
			this.w_cbNoInternetConnection.CheckedChanged += new EventHandler(this.On_cbNoInternetConnection_CheckedChanged);
			componentResourceManager.ApplyResources(this.w_tbLicenseKeyLong, "w_tbLicenseKeyLong");
			this.w_tbLicenseKeyLong.Name = "w_tbLicenseKeyLong";
			this.w_tbLicenseKeyLong.TextChanged += new EventHandler(this.On_LicenseKey_TextChanged);
			componentResourceManager.ApplyResources(this.w_lblOfflineInstructions, "w_lblOfflineInstructions");
			this.w_lblOfflineInstructions.Name = "w_lblOfflineInstructions";
			componentResourceManager.ApplyResources(this.w_lblMetalogixProduct, "w_lblMetalogixProduct");
			this.w_lblMetalogixProduct.Name = "w_lblMetalogixProduct";
			base.AcceptButton = this.w_btnAddKey;
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			base.CancelButton = this.btnCancel;
			componentResourceManager.ApplyResources(this, "$this");
			base.Controls.Add(this.w_lblMetalogixProduct);
			base.Controls.Add(this.w_lblOfflineInstructions);
			base.Controls.Add(this.w_tbLicenseKeyLong);
			base.Controls.Add(this.w_cbNoInternetConnection);
			base.Controls.Add(this.w_lblNote);
			base.Controls.Add(this.w_lblInstructions);
			base.Controls.Add(this.w_btnAddKey);
			base.Controls.Add(this.lblLicenseKey);
			base.Controls.Add(this.w_tbLicenseKeyShort);
			base.Controls.Add(this.w_lblError);
			base.Controls.Add(this.btnCancel);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "LicenseActivation";
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void On_cbNoInternetConnection_CheckedChanged(object sender, EventArgs e)
		{
			this.ToggleKeyView();
		}

		private void On_EnterKey_Click(object sender, EventArgs e)
		{
			string innerText = null;
			innerText = (!this.w_cbNoInternetConnection.Checked ? this.w_tbLicenseKeyShort.Text.Trim() : this.w_tbLicenseKeyLong.Text.Trim());
			try
			{
				try
				{
					System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
					if (innerText.Length == 32)
					{
						HttpWebRequest length = (HttpWebRequest)WebRequest.Create("http://www.metalogix.net/products/GetKey.aspx");
						length.Method = "POST";
						StringBuilder stringBuilder = new StringBuilder(1024);
						stringBuilder.Append(string.Concat("KeyId=", innerText));
						byte[] bytes = (new ASCIIEncoding()).GetBytes(stringBuilder.ToString());
						length.ContentType = "application/x-www-form-urlencoded";
						length.ContentLength = (long)stringBuilder.Length;
						Stream requestStream = length.GetRequestStream();
						requestStream.Write(bytes, 0, (int)bytes.Length);
						requestStream.Close();
						HttpWebResponse response = (HttpWebResponse)length.GetResponse();
						if (response.StatusCode == HttpStatusCode.OK)
						{
							string end = (new StreamReader(response.GetResponseStream())).ReadToEnd();
							XmlDocument xmlDocument = new XmlDocument();
							xmlDocument.LoadXml(end);
							XmlNode xmlNodes = xmlDocument.SelectSingleNode("//key");
							if (xmlNodes != null)
							{
								innerText = xmlNodes.InnerText;
							}
						}
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					GlobalServices.ErrorHandler.HandleException(string.Concat("Error requesting license from server: ", exception.Message), exception);
				}
			}
			finally
			{
				System.Windows.Forms.Cursor.Current = Cursors.Default;
			}
			try
			{
				MLLicenseCA mLLicenseCA = new MLLicenseCA(innerText);
				if (!mLLicenseCA.IsWellFormed)
				{
					this.ShowError("The specified license key is invalid.");
				}
				else
				{
					mLLicenseCA.Validate(this.m_sProductCode);
					AgreeToLicense agreeToLicense = new AgreeToLicense(mLLicenseCA.LicenseType, this.m_sProductTitle, mLLicenseCA.ExpiryDate);
					if ((agreeToLicense.ShowDialog(null) != System.Windows.Forms.DialogResult.OK ? false : agreeToLicense.Accepted))
					{
						this.m_sLicenseKey = innerText;
						try
						{
							if (mLLicenseCA.LicenseType == MLLicenseType.Partner || mLLicenseCA.LicenseType == MLLicenseType.Commercial)
							{
								Assembly entryAssembly = Assembly.GetEntryAssembly();
								string str = (entryAssembly != null ? entryAssembly.GetName().Version.ToString(4) : "");
								object[] product = new object[] { mLLicenseCA.Product, str, mLLicenseCA.Organization, mLLicenseCA.Name, mLLicenseCA.Email, null, null };
								product[5] = mLLicenseCA.ExpiryDate.ToString("yyyy-MM-dd");
								product[6] = mLLicenseCA.TotalPages;
								string str1 = string.Format("{0} - v{1} :{2}:{3}:{4}:{5}:{6}", product);
								str1 = (str1.Length > 100 ? str1.Substring(0, 100) : str1);
								LicenseServerAdapter.LogLicenseActivation(str1, mLLicenseCA.SerialNumber.ToString());
							}
						}
						catch
						{
						}
						base.DialogResult = System.Windows.Forms.DialogResult.OK;
						base.Close();
						return;
					}
				}
			}
			catch (LicenseException licenseException)
			{
				this.ShowError("The specified license key is invalid.");
			}
		}

		private void On_LicenseKey_TextChanged(object sender, EventArgs e)
		{
			this.UpdateUI();
		}

		private void ShowError(string sMessage)
		{
			this.w_lblError.Text = sMessage;
		}

		private void ToggleKeyView()
		{
			bool @checked = this.w_cbNoInternetConnection.Checked;
			this.w_tbLicenseKeyShort.Enabled = !@checked;
			this.w_tbLicenseKeyLong.Visible = @checked;
			int num = 214;
			if (!@checked)
			{
				base.Height = base.Height - num;
				int width = this.MinimumSize.Width;
				System.Drawing.Size minimumSize = this.MinimumSize;
				this.MinimumSize = new System.Drawing.Size(width, minimumSize.Height - num);
				int width1 = this.MaximumSize.Width;
				System.Drawing.Size maximumSize = this.MaximumSize;
				this.MaximumSize = new System.Drawing.Size(width1, maximumSize.Height - num);
			}
			else
			{
				this.w_tbLicenseKeyLong.Width = this.w_tbLicenseKeyShort.Width;
				this.w_tbLicenseKeyLong.Height = 204;
				int num1 = this.MaximumSize.Width;
				System.Drawing.Size size = this.MaximumSize;
				this.MaximumSize = new System.Drawing.Size(num1, size.Height + num);
				int width2 = this.MinimumSize.Width;
				System.Drawing.Size minimumSize1 = this.MinimumSize;
				this.MinimumSize = new System.Drawing.Size(width2, minimumSize1.Height + num);
				base.Height = base.Height + num;
			}
			this.UpdateUI();
		}

		private void UpdateUI()
		{
			this.w_btnAddKey.Enabled = (this.w_tbLicenseKeyLong.Text.Trim().Length <= 0 || !this.w_cbNoInternetConnection.Checked ? this.w_tbLicenseKeyShort.Text.Trim().Length > 0 : true);
			this.w_lblError.Text = "";
		}
	}
}