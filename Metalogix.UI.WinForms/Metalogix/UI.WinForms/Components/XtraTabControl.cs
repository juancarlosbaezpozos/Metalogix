using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTab;

namespace Metalogix.UI.WinForms.Components
{
    public class XtraTabControl : XtraUserControl
    {
        private IContainer components;

        private DevExpress.XtraTab.XtraTabControl tabControl;

        public int SelectedTabPageIndex
        {
            get
		{
			return tabControl.SelectedTabPageIndex;
		}
            set
		{
			tabControl.SelectedTabPageIndex = value;
		}
        }

        public XtraTabPageCollection TabPages => tabControl.TabPages;

        public XtraTabControl()
	{
		InitializeComponent();
		tabControl.ShowTabHeader = DefaultBoolean.False;
	}

        public void AddTab(TabbableControl control)
	{
		XtraTabPage xtraTabPage = tabControl.TabPages.Add();
		control.Dock = DockStyle.Fill;
		xtraTabPage.Controls.Add(control);
	}

        protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

        public XtraWizardTabbableControl GetSelectedPageControl()
	{
		if (tabControl.SelectedTabPage == null || tabControl.SelectedTabPage.Controls.Count <= 0)
		{
			return null;
		}
		return tabControl.SelectedTabPage.Controls[0] as XtraWizardTabbableControl;
	}

        private void InitializeComponent()
	{
		this.tabControl = new DevExpress.XtraTab.XtraTabControl();
		((System.ComponentModel.ISupportInitialize)this.tabControl).BeginInit();
		base.SuspendLayout();
		this.tabControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.tabControl.BorderStylePage = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tabControl.Location = new System.Drawing.Point(0, 0);
		this.tabControl.LookAndFeel.SkinName = "Office 2013";
		this.tabControl.LookAndFeel.UseDefaultLookAndFeel = false;
		this.tabControl.Name = "tabControl";
		this.tabControl.Size = new System.Drawing.Size(334, 318);
		this.tabControl.TabIndex = 0;
		base.Appearance.BackColor = System.Drawing.Color.White;
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.tabControl);
		base.Name = "XtraTabControl";
		base.Size = new System.Drawing.Size(334, 318);
		((System.ComponentModel.ISupportInitialize)this.tabControl).EndInit();
		base.ResumeLayout(false);
	}
    }
}
