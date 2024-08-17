using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Metalogix.UI.WinForms;

namespace Metalogix.SharePoint.UI.WinForms.Mapping
{
	public class CreateDomainMappingDialog : XtraForm
	{
		private string m_sSourceDomain;

		private string m_sTargetDomain;

		private IContainer components;

		private SimpleButton bOK;

		private SimpleButton bCancel;

		private TextBox tbSourceDomain;

		private Label lSource;

		private Label lTarget;

		private TextBox tbTargetDomain;

		public string SourceDomain
		{
			get
			{
				return m_sSourceDomain;
			}
			set
			{
				m_sSourceDomain = value;
			}
		}

		public string TargetDomain
		{
			get
			{
				return m_sTargetDomain;
			}
			set
			{
				m_sTargetDomain = value;
			}
		}

		public CreateDomainMappingDialog()
		{
			InitializeComponent();
		}

		private void bOK_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(tbTargetDomain.Text) || string.IsNullOrEmpty(tbSourceDomain.Text))
			{
				FlatXtraMessageBox.Show("Please enter a text value for both a source and target domain.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			SourceDomain = tbSourceDomain.Text;
			TargetDomain = tbTargetDomain.Text;
			base.DialogResult = DialogResult.OK;
			Close();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Mapping.CreateDomainMappingDialog));
			this.bOK = new DevExpress.XtraEditors.SimpleButton();
			this.bCancel = new DevExpress.XtraEditors.SimpleButton();
			this.tbSourceDomain = new System.Windows.Forms.TextBox();
			this.lSource = new System.Windows.Forms.Label();
			this.lTarget = new System.Windows.Forms.Label();
			this.tbTargetDomain = new System.Windows.Forms.TextBox();
			base.SuspendLayout();
			resources.ApplyResources(this.bOK, "bOK");
			this.bOK.Name = "bOK";
			this.bOK.Click += new System.EventHandler(bOK_Click);
			resources.ApplyResources(this.bCancel, "bCancel");
			this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bCancel.Name = "bCancel";
			resources.ApplyResources(this.tbSourceDomain, "tbSourceDomain");
			this.tbSourceDomain.Name = "tbSourceDomain";
			this.tbSourceDomain.KeyDown += new System.Windows.Forms.KeyEventHandler(tbSourceDomain_KeyDown);
			resources.ApplyResources(this.lSource, "lSource");
			this.lSource.Name = "lSource";
			resources.ApplyResources(this.lTarget, "lTarget");
			this.lTarget.Name = "lTarget";
			resources.ApplyResources(this.tbTargetDomain, "tbTargetDomain");
			this.tbTargetDomain.Name = "tbTargetDomain";
			this.tbTargetDomain.KeyDown += new System.Windows.Forms.KeyEventHandler(tbTargetDomain_KeyDown);
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.tbTargetDomain);
			base.Controls.Add(this.bCancel);
			base.Controls.Add(this.lTarget);
			base.Controls.Add(this.bOK);
			base.Controls.Add(this.lSource);
			base.Controls.Add(this.tbSourceDomain);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "CreateDomainMappingDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void tbSourceDomain_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
			{
				bOK_Click(sender, e);
			}
		}

		private void tbTargetDomain_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
			{
				bOK_Click(sender, e);
			}
		}
	}
}
