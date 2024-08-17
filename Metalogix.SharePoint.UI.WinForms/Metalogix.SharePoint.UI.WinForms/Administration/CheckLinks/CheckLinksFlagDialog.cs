using Metalogix.Data.Filters;
using System;
using System.ComponentModel;
using System.Resources;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Administration.CheckLinks
{
	public class CheckLinksFlagDialog : Form
	{
		private ExpressionLogic m_FlagType = ExpressionLogic.Or;

		private StringFilterExpression m_Filter;

		private IContainer components;

		private Button w_btnOK;

		private Button w_btnCancel;

		private Label label1;

		private RadioButton w_rbOr;

		private RadioButton w_rbAnd;

		private Label label2;

		private Label label3;

		private ComboBox w_cbCondition;

		private TextBox w_txtValue;

		private Label label4;

		public StringFilterExpression Filter
		{
			get
			{
				return this.m_Filter;
			}
			set
			{
				this.m_Filter = value;
				this.w_txtValue.Text = this.m_Filter.Pattern;
				this.w_cbCondition.Text = TCFlaggingOptions.ConvertFilterOperandToFlagConditionOperator(this.m_Filter.Operand);
			}
		}

		public ExpressionLogic FlagType
		{
			get
			{
				return this.m_FlagType;
			}
			set
			{
				this.m_FlagType = value;
				if (this.m_FlagType == ExpressionLogic.And)
				{
					this.w_rbAnd.Checked = true;
					return;
				}
				this.w_rbOr.Checked = true;
			}
		}

		public CheckLinksFlagDialog()
		{
			this.InitializeComponent();
			this.UpdateEnabledState();
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(CheckLinksFlagDialog));
			this.w_btnOK = new Button();
			this.w_btnCancel = new Button();
			this.label1 = new Label();
			this.w_rbOr = new RadioButton();
			this.w_rbAnd = new RadioButton();
			this.label2 = new Label();
			this.label3 = new Label();
			this.w_cbCondition = new ComboBox();
			this.w_txtValue = new TextBox();
			this.label4 = new Label();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.w_btnOK, "w_btnOK");
			this.w_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.w_btnOK.Name = "w_btnOK";
			this.w_btnOK.UseVisualStyleBackColor = true;
			this.w_btnOK.Click += new EventHandler(this.On_btnOK_Click);
			componentResourceManager.ApplyResources(this.w_btnCancel, "w_btnCancel");
			this.w_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_btnCancel.Name = "w_btnCancel";
			this.w_btnCancel.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			componentResourceManager.ApplyResources(this.w_rbOr, "w_rbOr");
			this.w_rbOr.Checked = true;
			this.w_rbOr.Name = "w_rbOr";
			this.w_rbOr.TabStop = true;
			this.w_rbOr.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.w_rbAnd, "w_rbAnd");
			this.w_rbAnd.Name = "w_rbAnd";
			this.w_rbAnd.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			componentResourceManager.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			componentResourceManager.ApplyResources(this.w_cbCondition, "w_cbCondition");
			this.w_cbCondition.DropDownStyle = ComboBoxStyle.DropDownList;
			this.w_cbCondition.FormattingEnabled = true;
			ComboBox.ObjectCollection items = this.w_cbCondition.Items;
			object[] str = new object[] { componentResourceManager.GetString("w_cbCondition.Items"), componentResourceManager.GetString("w_cbCondition.Items1"), componentResourceManager.GetString("w_cbCondition.Items2"), componentResourceManager.GetString("w_cbCondition.Items3"), componentResourceManager.GetString("w_cbCondition.Items4"), componentResourceManager.GetString("w_cbCondition.Items5") };
			items.AddRange(str);
			this.w_cbCondition.Name = "w_cbCondition";
			this.w_cbCondition.TextChanged += new EventHandler(this.On_InputChanged);
			componentResourceManager.ApplyResources(this.w_txtValue, "w_txtValue");
			this.w_txtValue.Name = "w_txtValue";
			this.w_txtValue.TextChanged += new EventHandler(this.On_InputChanged);
			componentResourceManager.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			base.AcceptButton = this.w_btnOK;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.w_btnCancel;
			base.Controls.Add(this.label4);
			base.Controls.Add(this.w_txtValue);
			base.Controls.Add(this.w_cbCondition);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.w_rbAnd);
			base.Controls.Add(this.w_rbOr);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.w_btnCancel);
			base.Controls.Add(this.w_btnOK);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "CheckLinksFlagDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void On_btnOK_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(this.w_cbCondition.Text) || string.IsNullOrEmpty(this.w_txtValue.Text))
			{
				base.DialogResult = System.Windows.Forms.DialogResult.Cancel;
				return;
			}
			this.FlagType = (this.w_rbAnd.Checked ? ExpressionLogic.And : ExpressionLogic.Or);
			this.Filter = new StringFilterExpression(TCFlaggingOptions.ConvertFlagConditionOperatorToFilterOperand(this.w_cbCondition.Text), this.w_txtValue.Text, false);
		}

		private void On_InputChanged(object sender, EventArgs e)
		{
			this.UpdateEnabledState();
		}

		private void UpdateEnabledState()
		{
			this.w_btnOK.Enabled = (string.IsNullOrEmpty(this.w_cbCondition.Text) ? false : !string.IsNullOrEmpty(this.w_txtValue.Text));
		}
	}
}