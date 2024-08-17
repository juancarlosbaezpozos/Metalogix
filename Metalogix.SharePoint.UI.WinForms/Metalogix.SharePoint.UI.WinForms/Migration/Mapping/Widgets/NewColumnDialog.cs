using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Migration.Mapping.Widgets
{
	public class NewColumnDialog : Form
	{
		private IContainer components;

		private Button w_bCancel;

		private Button w_bOkay;

		private TextBox w_tbColumnName;

		private Label w_lbColumnName;

		private CheckBox w_cbCreateAsSiteColumn;

		private CheckBox w_cbAddToContentType;

		private Label w_lbGroup;

		private ComboBox w_cbGroup;

		private ComboBox w_cbContentType;

		private Label w_lbType;

		public bool AddToContentType
		{
			get
			{
				return this.w_cbAddToContentType.Checked;
			}
			set
			{
				this.w_cbAddToContentType.Checked = value;
			}
		}

		public string ColumnName
		{
			get
			{
				return this.w_tbColumnName.Text;
			}
			set
			{
				this.w_tbColumnName.Text = value;
			}
		}

		public string ContentType
		{
			get
			{
				return this.w_cbContentType.Text;
			}
			set
			{
				this.w_cbContentType.Text = value;
			}
		}

		public string[] ContentTypeOptions
		{
			get
			{
				if (this.w_cbContentType.Items.Count == 0)
				{
					return new string[0];
				}
				string[] item = new string[this.w_cbContentType.Items.Count];
				for (int i = 0; i < this.w_cbContentType.Items.Count; i++)
				{
					item[i] = (string)this.w_cbContentType.Items[i];
				}
				return item;
			}
			set
			{
				this.w_cbContentType.Items.Clear();
				if (value != null && (int)value.Length > 0)
				{
					this.w_cbContentType.Items.AddRange(value);
					this.w_cbContentType.SelectedIndex = 0;
				}
				this.UpdateUI();
			}
		}

		public bool CreateAsSiteColumn
		{
			get
			{
				return this.w_cbCreateAsSiteColumn.Checked;
			}
			set
			{
				this.w_cbCreateAsSiteColumn.Checked = value;
			}
		}

		public string SiteColumnGroup
		{
			get
			{
				return this.w_cbGroup.Text;
			}
			set
			{
				this.w_cbGroup.Text = value;
			}
		}

		public string[] SiteColumnGroupOptions
		{
			get
			{
				if (this.w_cbGroup.Items.Count == 0)
				{
					return new string[0];
				}
				string[] item = new string[this.w_cbGroup.Items.Count];
				for (int i = 0; i < this.w_cbGroup.Items.Count; i++)
				{
					item[i] = (string)this.w_cbGroup.Items[i];
				}
				return item;
			}
			set
			{
				this.w_cbGroup.Items.Clear();
				if (value != null && (int)value.Length > 0)
				{
					this.w_cbGroup.Items.AddRange(value);
					this.w_cbGroup.SelectedIndex = 0;
				}
				this.UpdateUI();
			}
		}

		public NewColumnDialog()
		{
			this.InitializeComponent();
			this.UpdateUI();
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(NewColumnDialog));
			this.w_bCancel = new Button();
			this.w_bOkay = new Button();
			this.w_tbColumnName = new TextBox();
			this.w_lbColumnName = new Label();
			this.w_cbCreateAsSiteColumn = new CheckBox();
			this.w_cbAddToContentType = new CheckBox();
			this.w_lbGroup = new Label();
			this.w_cbGroup = new ComboBox();
			this.w_cbContentType = new ComboBox();
			this.w_lbType = new Label();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.w_bCancel, "w_bCancel");
			this.w_bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_bCancel.Name = "w_bCancel";
			this.w_bCancel.UseVisualStyleBackColor = true;
			this.w_bCancel.Click += new EventHandler(this.w_bCancel_Click);
			componentResourceManager.ApplyResources(this.w_bOkay, "w_bOkay");
			this.w_bOkay.Name = "w_bOkay";
			this.w_bOkay.UseVisualStyleBackColor = true;
			this.w_bOkay.Click += new EventHandler(this.w_bOkay_Click);
			componentResourceManager.ApplyResources(this.w_tbColumnName, "w_tbColumnName");
			this.w_tbColumnName.Name = "w_tbColumnName";
			this.w_tbColumnName.TextChanged += new EventHandler(this.w_tbColumnName_TextChanged);
			componentResourceManager.ApplyResources(this.w_lbColumnName, "w_lbColumnName");
			this.w_lbColumnName.Name = "w_lbColumnName";
			componentResourceManager.ApplyResources(this.w_cbCreateAsSiteColumn, "w_cbCreateAsSiteColumn");
			this.w_cbCreateAsSiteColumn.Name = "w_cbCreateAsSiteColumn";
			this.w_cbCreateAsSiteColumn.UseVisualStyleBackColor = true;
			this.w_cbCreateAsSiteColumn.CheckedChanged += new EventHandler(this.w_cbCreateAsSiteColumn_CheckedChanged);
			componentResourceManager.ApplyResources(this.w_cbAddToContentType, "w_cbAddToContentType");
			this.w_cbAddToContentType.Name = "w_cbAddToContentType";
			this.w_cbAddToContentType.UseVisualStyleBackColor = true;
			this.w_cbAddToContentType.CheckedChanged += new EventHandler(this.w_cbAddToContentType_CheckedChanged);
			componentResourceManager.ApplyResources(this.w_lbGroup, "w_lbGroup");
			this.w_lbGroup.Name = "w_lbGroup";
			componentResourceManager.ApplyResources(this.w_cbGroup, "w_cbGroup");
			this.w_cbGroup.DropDownStyle = ComboBoxStyle.DropDownList;
			this.w_cbGroup.FormattingEnabled = true;
			this.w_cbGroup.Name = "w_cbGroup";
			componentResourceManager.ApplyResources(this.w_cbContentType, "w_cbContentType");
			this.w_cbContentType.DropDownStyle = ComboBoxStyle.DropDownList;
			this.w_cbContentType.FormattingEnabled = true;
			this.w_cbContentType.Name = "w_cbContentType";
			componentResourceManager.ApplyResources(this.w_lbType, "w_lbType");
			this.w_lbType.Name = "w_lbType";
			base.AcceptButton = this.w_bOkay;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.w_bCancel;
			base.Controls.Add(this.w_lbType);
			base.Controls.Add(this.w_cbContentType);
			base.Controls.Add(this.w_cbGroup);
			base.Controls.Add(this.w_lbGroup);
			base.Controls.Add(this.w_cbAddToContentType);
			base.Controls.Add(this.w_cbCreateAsSiteColumn);
			base.Controls.Add(this.w_lbColumnName);
			base.Controls.Add(this.w_tbColumnName);
			base.Controls.Add(this.w_bOkay);
			base.Controls.Add(this.w_bCancel);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "NewColumnDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void UpdateUI()
		{
			this.w_bOkay.Enabled = !string.IsNullOrEmpty(this.w_tbColumnName.Text);
			this.w_cbCreateAsSiteColumn.Enabled = this.w_cbGroup.Items.Count > 0;
			this.w_cbCreateAsSiteColumn.Checked = (!this.w_cbCreateAsSiteColumn.Checked ? false : this.w_cbCreateAsSiteColumn.Enabled);
			this.w_cbGroup.Enabled = (!this.w_cbCreateAsSiteColumn.Enabled ? false : this.w_cbCreateAsSiteColumn.Checked);
			this.w_cbAddToContentType.Enabled = (this.w_cbContentType.Items.Count <= 0 ? false : this.w_cbCreateAsSiteColumn.Checked);
			this.w_cbAddToContentType.Checked = (!this.w_cbAddToContentType.Checked ? false : this.w_cbAddToContentType.Enabled);
			this.w_cbContentType.Enabled = (!this.w_cbAddToContentType.Enabled ? false : this.w_cbAddToContentType.Checked);
		}

		private void w_bCancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			base.Close();
		}

		private void w_bOkay_Click(object sender, EventArgs e)
		{
			if (!this.w_bOkay.Enabled)
			{
				return;
			}
			base.DialogResult = System.Windows.Forms.DialogResult.OK;
			base.Close();
		}

		private void w_cbAddToContentType_CheckedChanged(object sender, EventArgs e)
		{
			this.UpdateUI();
		}

		private void w_cbCreateAsSiteColumn_CheckedChanged(object sender, EventArgs e)
		{
			this.UpdateUI();
		}

		private void w_tbColumnName_TextChanged(object sender, EventArgs e)
		{
			this.UpdateUI();
		}
	}
}