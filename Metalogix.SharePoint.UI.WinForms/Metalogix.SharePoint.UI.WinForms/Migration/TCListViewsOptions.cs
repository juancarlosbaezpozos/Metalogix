using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Migration.ListViewOptions.png")]
	[ControlName("List Views Options")]
	public class TCListViewsOptions : ScopableTabbableControl
	{
		private SPListViewsOptions m_options;

		private IContainer components;

		private CheckEdit w_cbOverwriteExistingViews;

		public SPListViewsOptions Options
		{
			get
			{
				return m_options;
			}
			set
			{
				m_options = value;
				LoadUI();
			}
		}

		public TCListViewsOptions()
		{
			InitializeComponent();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.TCListViewsOptions));
			this.w_cbOverwriteExistingViews = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)this.w_cbOverwriteExistingViews.Properties).BeginInit();
			base.SuspendLayout();
			resources.ApplyResources(this.w_cbOverwriteExistingViews, "w_cbOverwriteExistingViews");
			this.w_cbOverwriteExistingViews.Name = "w_cbOverwriteExistingViews";
			this.w_cbOverwriteExistingViews.Properties.Appearance.BackColor = (System.Drawing.Color)resources.GetObject("w_cbOverwriteExistingViews.Properties.Appearance.BackColor");
			this.w_cbOverwriteExistingViews.Properties.Appearance.Options.UseBackColor = true;
			this.w_cbOverwriteExistingViews.Properties.AutoWidth = true;
			this.w_cbOverwriteExistingViews.Properties.Caption = resources.GetString("w_cbOverwriteExistingViews.Properties.Caption");
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.w_cbOverwriteExistingViews);
			base.Name = "TCListViewsOptions";
			((System.ComponentModel.ISupportInitialize)this.w_cbOverwriteExistingViews.Properties).EndInit();
			base.ResumeLayout(false);
		}

		protected override void LoadUI()
		{
			w_cbOverwriteExistingViews.Checked = m_options.OverwriteExistingViews;
		}

		public override bool SaveUI()
		{
			m_options.OverwriteExistingViews = w_cbOverwriteExistingViews.Checked;
			return true;
		}
	}
}
