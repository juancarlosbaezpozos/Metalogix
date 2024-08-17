using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Metalogix.SharePoint.Migration
{
    public class InputBox : XtraForm
    {
        private List<string> m_InvalidInputs;

        private string m_sInputValue = "";

        private IContainer components;

        private Label w_lbCaption;

        private SimpleButton w_bOkay;

        private SimpleButton w_bCancel;

        private TextBox w_txtInput;

        public string InputValue
        {
            get
            {
                return m_sInputValue;
            }
            set
            {
                m_sInputValue = value;
            }
        }

        public InputBox(string sTitle, string sCaption, bool isSimplifiedMode = false, int increasedHeight = 0, int increasedWidth = 0)
        {
            InitializeComponent();
            if (isSimplifiedMode)
            {
                ApplyBasicModeSkin();
            }
            Text = sTitle;
            w_lbCaption.Text = sCaption;
            if (isSimplifiedMode)
            {
                MaximumSize = new Size(1000, base.Size.Height + increasedHeight);
                Size size1 = base.Size;
                Size size2 = base.Size;
                base.Size = new Size(size1.Width + increasedWidth, size2.Height + increasedHeight);
                MinimumSize = new Size(base.Size.Width, base.Size.Height);
                TextBox wTxtInput = w_txtInput;
                int x = w_txtInput.Location.X;
                wTxtInput.Location = new Point(x, w_txtInput.Location.Y + (increasedHeight - 6));
            }
        }

        public InputBox(string sTitle, string sCaption, List<string> invalidInputs)
        {
            InitializeComponent();
            m_InvalidInputs = invalidInputs;
            Text = sTitle;
            w_lbCaption.Text = sCaption;
        }

        private void ApplyBasicModeSkin()
        {
            w_bOkay.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
            w_bOkay.LookAndFeel.UseDefaultLookAndFeel = false;
            w_bCancel.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
            w_bCancel.LookAndFeel.UseDefaultLookAndFeel = false;
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
            System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.Migration.InputBox));
            this.w_lbCaption = new System.Windows.Forms.Label();
            this.w_bOkay = new DevExpress.XtraEditors.SimpleButton();
            this.w_bCancel = new DevExpress.XtraEditors.SimpleButton();
            this.w_txtInput = new System.Windows.Forms.TextBox();
            base.SuspendLayout();
            componentResourceManager.ApplyResources(this.w_lbCaption, "w_lbCaption");
            this.w_lbCaption.Name = "w_lbCaption";
            componentResourceManager.ApplyResources(this.w_bOkay, "w_bOkay");
            this.w_bOkay.Name = "w_bOkay";
            this.w_bOkay.Click += new System.EventHandler(w_bOkay_Click);
            componentResourceManager.ApplyResources(this.w_bCancel, "w_bCancel");
            this.w_bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.w_bCancel.Name = "w_bCancel";
            componentResourceManager.ApplyResources(this.w_txtInput, "w_txtInput");
            this.w_txtInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.w_txtInput.Name = "w_txtInput";
            this.w_txtInput.TextChanged += new System.EventHandler(w_txtInput_TextChanged);
            base.AcceptButton = this.w_bOkay;
            base.Appearance.BackColor = (System.Drawing.Color)componentResourceManager.GetObject("InputBox.Appearance.BackColor");
            base.Appearance.Options.UseBackColor = true;
            componentResourceManager.ApplyResources(this, "$this");
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = this.w_bCancel;
            base.Controls.Add(this.w_txtInput);
            base.Controls.Add(this.w_bCancel);
            base.Controls.Add(this.w_bOkay);
            base.Controls.Add(this.w_lbCaption);
            base.LookAndFeel.SkinName = "Office 2013";
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "InputBox";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            base.Load += new System.EventHandler(InputBox_Load);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void InputBox_Load(object sender, EventArgs e)
        {
            base.ActiveControl = w_txtInput;
        }

        private void w_bCancel_Click(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void w_bOkay_Click(object sender, EventArgs e)
        {
            InputValue = w_txtInput.Text;
            base.DialogResult = DialogResult.OK;
            Close();
        }

        private void w_txtInput_TextChanged(object sender, EventArgs e)
        {
            w_bOkay.Enabled = false;
            bool flag = false;
            if (m_InvalidInputs != null)
            {
                foreach (string mInvalidInput in m_InvalidInputs)
                {
                    if (mInvalidInput.Equals((sender as TextBox).Text, StringComparison.OrdinalIgnoreCase))
                    {
                        flag = true;
                    }
                }
            }
            if (!flag)
            {
                w_bOkay.Enabled = !(sender as TextBox).Text.Equals("");
            }
        }
    }
}
