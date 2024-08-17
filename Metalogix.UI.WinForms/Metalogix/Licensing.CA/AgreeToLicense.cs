using Metalogix.Licensing;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Metalogix.Licensing.CA
{
	internal class AgreeToLicense : Form
	{
		private RichTextBox richTextLicenseAgreement;

		private Label lblAcceptance;

		private RadioButton rbAgree;

		private RadioButton rbNotAgree;

		private Button btnOK;

		private Button btnCancel;

		private Label lblExpiry;

		private DateTime m_dateExpired = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));

		private System.ComponentModel.Container components;

		public bool Accepted
		{
			get
			{
				return this.rbAgree.Checked;
			}
		}

		private AgreeToLicense()
		{
		}

		public AgreeToLicense(MLLicenseType licType, string sProductTitle, DateTime dateExpired)
		{
			this.InitializeComponent();
			this.Text = string.Concat(sProductTitle, " License Agreement");
			this.m_dateExpired = dateExpired;
			this.lblExpiry.Text = string.Concat("This license expires on: ", dateExpired.ToString("dd-MMM-yyyy"));
			this.LoadAgreement(licType, sProductTitle);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(AgreeToLicense));
			this.richTextLicenseAgreement = new RichTextBox();
			this.rbNotAgree = new RadioButton();
			this.rbAgree = new RadioButton();
			this.lblAcceptance = new Label();
			this.btnOK = new Button();
			this.btnCancel = new Button();
			this.lblExpiry = new Label();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.richTextLicenseAgreement, "richTextLicenseAgreement");
			this.richTextLicenseAgreement.Name = "richTextLicenseAgreement";
			this.richTextLicenseAgreement.ReadOnly = true;
			componentResourceManager.ApplyResources(this.rbNotAgree, "rbNotAgree");
			this.rbNotAgree.Checked = true;
			this.rbNotAgree.Name = "rbNotAgree";
			this.rbNotAgree.TabStop = true;
			componentResourceManager.ApplyResources(this.rbAgree, "rbAgree");
			this.rbAgree.Name = "rbAgree";
			componentResourceManager.ApplyResources(this.lblAcceptance, "lblAcceptance");
			this.lblAcceptance.Name = "lblAcceptance";
			componentResourceManager.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Name = "btnOK";
			componentResourceManager.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			componentResourceManager.ApplyResources(this.lblExpiry, "lblExpiry");
			this.lblExpiry.Name = "lblExpiry";
			base.AcceptButton = this.btnOK;
			componentResourceManager.ApplyResources(this, "$this");
			base.CancelButton = this.btnCancel;
			base.Controls.Add(this.lblExpiry);
			base.Controls.Add(this.btnCancel);
			base.Controls.Add(this.btnOK);
			base.Controls.Add(this.lblAcceptance);
			base.Controls.Add(this.rbAgree);
			base.Controls.Add(this.rbNotAgree);
			base.Controls.Add(this.richTextLicenseAgreement);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "AgreeToLicense";
			base.ShowInTaskbar = false;
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			base.TopMost = true;
			base.ResumeLayout(false);
		}

		private bool LoadAgreement(MLLicenseType licType, string sProductTitle)
		{
			if (licType == MLLicenseType.Invalid)
			{
				return false;
			}
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			Stream manifestResourceStream = null;
			if (licType == MLLicenseType.Evaluation)
			{
				manifestResourceStream = executingAssembly.GetManifestResourceStream("Metalogix.Licensing.Docs.Evaluation_License.rtf");
			}
			else if (licType == MLLicenseType.Partner)
			{
				manifestResourceStream = executingAssembly.GetManifestResourceStream("Metalogix.Licensing.Docs.Partner_License.rtf");
			}
			else if (licType == MLLicenseType.Commercial)
			{
				manifestResourceStream = executingAssembly.GetManifestResourceStream("Metalogix.Licensing.Docs.Commercial_License.rtf");
			}
			if (manifestResourceStream == null)
			{
				return false;
			}
			StreamReader streamReader = new StreamReader(manifestResourceStream, true);
			StringWriter stringWriter = new StringWriter();
			StringBuilder stringBuilder = new StringBuilder(streamReader.ReadToEnd());
			stringBuilder.Replace("MetalogixProduct", sProductTitle);
			stringWriter.Write(stringBuilder.ToString());
			stringWriter.Flush();
			stringWriter.Close();
			this.richTextLicenseAgreement.Rtf = stringWriter.ToString();
			return true;
		}
	}
}