using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Metalogix.SharePoint.UI.WinForms.Mapping
{
	public class CreateUserMappingDialog : XtraForm
	{
		private string m_sLoginName;

		private string m_sEmail;

		private string m_sDisplayName;

		private string m_sNotes;

		private IContainer components;

		private GroupBox groupBox1;

		private SimpleButton bCancel;

		private SimpleButton bOK;

		private Label lLoginName;

		private TextBox tbLoginName;

		private Label lDisplayName;

		private Label lEmail;

		private TextBox tbNotes;

		private TextBox tbEmail;

		private TextBox tbDisplayName;

		private Label lNotes;

		public string DisplayName
		{
			get
			{
				return m_sDisplayName;
			}
			set
			{
				m_sDisplayName = value;
			}
		}

		public string Email
		{
			get
			{
				return m_sEmail;
			}
			set
			{
				m_sEmail = value;
			}
		}

		public string LoginName
		{
			get
			{
				return m_sLoginName;
			}
			set
			{
				m_sLoginName = value;
			}
		}

		public string Notes
		{
			get
			{
				return m_sNotes;
			}
			set
			{
				m_sNotes = value;
			}
		}

		public CreateUserMappingDialog(bool isBasicMode = false)
		{
			InitializeComponent();
			if (isBasicMode)
			{
				ApplyBasicModeSkin();
			}
		}

		private void ApplyBasicModeSkin()
		{
			bOK.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
			bOK.LookAndFeel.UseDefaultLookAndFeel = false;
			bCancel.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
			bCancel.LookAndFeel.UseDefaultLookAndFeel = false;
		}

		private void bOK_Click(object sender, EventArgs e)
		{
			LoginName = tbLoginName.Text;
			DisplayName = tbDisplayName.Text;
			Email = tbEmail.Text;
			Notes = tbNotes.Text;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Mapping.CreateUserMappingDialog));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.tbNotes = new System.Windows.Forms.TextBox();
			this.tbEmail = new System.Windows.Forms.TextBox();
			this.tbDisplayName = new System.Windows.Forms.TextBox();
			this.lNotes = new System.Windows.Forms.Label();
			this.lDisplayName = new System.Windows.Forms.Label();
			this.lEmail = new System.Windows.Forms.Label();
			this.lLoginName = new System.Windows.Forms.Label();
			this.tbLoginName = new System.Windows.Forms.TextBox();
			this.bCancel = new DevExpress.XtraEditors.SimpleButton();
			this.bOK = new DevExpress.XtraEditors.SimpleButton();
			this.groupBox1.SuspendLayout();
			base.SuspendLayout();
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Controls.Add(this.tbNotes);
			this.groupBox1.Controls.Add(this.tbEmail);
			this.groupBox1.Controls.Add(this.tbDisplayName);
			this.groupBox1.Controls.Add(this.lNotes);
			this.groupBox1.Controls.Add(this.lDisplayName);
			this.groupBox1.Controls.Add(this.lEmail);
			this.groupBox1.Controls.Add(this.lLoginName);
			this.groupBox1.Controls.Add(this.tbLoginName);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			resources.ApplyResources(this.tbNotes, "tbNotes");
			this.tbNotes.Name = "tbNotes";
			this.tbNotes.KeyDown += new System.Windows.Forms.KeyEventHandler(tbTextBox_KeyUp);
			resources.ApplyResources(this.tbEmail, "tbEmail");
			this.tbEmail.Name = "tbEmail";
			this.tbEmail.KeyDown += new System.Windows.Forms.KeyEventHandler(tbTextBox_KeyUp);
			resources.ApplyResources(this.tbDisplayName, "tbDisplayName");
			this.tbDisplayName.Name = "tbDisplayName";
			this.tbDisplayName.KeyDown += new System.Windows.Forms.KeyEventHandler(tbTextBox_KeyUp);
			resources.ApplyResources(this.lNotes, "lNotes");
			this.lNotes.Name = "lNotes";
			resources.ApplyResources(this.lDisplayName, "lDisplayName");
			this.lDisplayName.Name = "lDisplayName";
			resources.ApplyResources(this.lEmail, "lEmail");
			this.lEmail.Name = "lEmail";
			resources.ApplyResources(this.lLoginName, "lLoginName");
			this.lLoginName.Name = "lLoginName";
			resources.ApplyResources(this.tbLoginName, "tbLoginName");
			this.tbLoginName.Name = "tbLoginName";
			this.tbLoginName.TextChanged += new System.EventHandler(tbLoginName_TextChanged);
			this.tbLoginName.KeyDown += new System.Windows.Forms.KeyEventHandler(tbTextBox_KeyUp);
			resources.ApplyResources(this.bCancel, "bCancel");
			this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bCancel.Name = "bCancel";
			resources.ApplyResources(this.bOK, "bOK");
			this.bOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.bOK.Name = "bOK";
			this.bOK.Click += new System.EventHandler(bOK_Click);
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.bCancel);
			base.Controls.Add(this.bOK);
			base.Controls.Add(this.groupBox1);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "CreateUserMappingDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			base.ResumeLayout(false);
		}

		private void tbLoginName_TextChanged(object sender, EventArgs e)
		{
			if (tbLoginName.Text.Length < 1)
			{
				bOK.Enabled = false;
			}
			else
			{
				bOK.Enabled = true;
			}
		}

		private void tbTextBox_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Return)
			{
				bOK_Click(sender, e);
			}
		}
	}
}
