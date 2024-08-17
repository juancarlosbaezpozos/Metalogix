using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.SharePoint.Administration;
using Metalogix.SharePoint.Options.Administration.Navigation;
using Metalogix.UI.WinForms.Components;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Administration.Navigation
{
	[ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Administration.Ribbon32.ico"), ControlName("SharePoint Ribbon")]
	public partial class TCNavigationRibbonOptions : ScopableTabbableControl, IPropagateNavigation
	{
		private SPNavigationRibbonOptions m_Options;

		private System.ComponentModel.IContainer components;

		private CheckEdit w_rbDoNotChangeRibbon;

		private CheckEdit w_rbShowRibbon;

		private CheckEdit w_rbHideRibbon;

		private LabelControl w_lblDescription;

		private PanelControl w_pnlRibbon;

		private LabelControl w_lblSharePoint2010Only;

		public SPNavigationRibbonOptions Options
		{
			get
			{
				return this.m_Options;
			}
			set
			{
				this.m_Options = value;
				this.LoadUI();
			}
		}

		public TCNavigationRibbonOptions()
		{
			this.InitializeComponent();
		}

		protected override void LoadUI()
		{
			if (this.Options.ShowRibbon == ChangeNavigationSettingsOptions.RibbonOptions.LeaveUnchanged)
			{
				this.w_rbDoNotChangeRibbon.Checked = true;
			}
			else if (this.Options.ShowRibbon == ChangeNavigationSettingsOptions.RibbonOptions.ShowRibbon)
			{
				this.w_rbShowRibbon.Checked = true;
			}
			else if (this.Options.ShowRibbon == ChangeNavigationSettingsOptions.RibbonOptions.HideRibbon)
			{
				this.w_rbHideRibbon.Checked = true;
			}
			if (this.Options.SharePointVersion.IsSharePoint2010OrLater)
			{
				this.w_pnlRibbon.Location = this.w_lblSharePoint2010Only.Location;
				this.w_pnlRibbon.Enabled = true;
				this.w_lblSharePoint2010Only.Visible = false;
			}
			else
			{
				this.w_lblSharePoint2010Only.Visible = true;
				this.w_pnlRibbon.Enabled = false;
			}
			this.Options.RibbonOptionsModified = new ChangeNavigationSettingsOptions.RibbonModifiedFlags();
		}

		public override bool SaveUI()
		{
			if (this.w_rbDoNotChangeRibbon.Checked)
			{
				this.Options.ShowRibbon = ChangeNavigationSettingsOptions.RibbonOptions.LeaveUnchanged;
			}
			else if (this.w_rbShowRibbon.Checked)
			{
				this.Options.ShowRibbon = ChangeNavigationSettingsOptions.RibbonOptions.ShowRibbon;
			}
			else if (this.w_rbHideRibbon.Checked)
			{
				this.Options.ShowRibbon = ChangeNavigationSettingsOptions.RibbonOptions.HideRibbon;
			}
			return true;
		}

		public void StartNavigationPropagationHandler()
		{
		}

	    public void StopNavigationPropagationHandler()
		{
		}

		private void On_CheckedChanged(object sender, System.EventArgs e)
		{
			this.Options.RibbonOptionsModified.ShowRibbon = true;
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
			System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(TCNavigationRibbonOptions));
			this.w_rbDoNotChangeRibbon = new CheckEdit();
			this.w_rbShowRibbon = new CheckEdit();
			this.w_rbHideRibbon = new CheckEdit();
			this.w_lblDescription = new LabelControl();
			this.w_pnlRibbon = new PanelControl();
			this.w_lblSharePoint2010Only = new LabelControl();
			((System.ComponentModel.ISupportInitialize)this.w_rbDoNotChangeRibbon.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbShowRibbon.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbHideRibbon.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_pnlRibbon).BeginInit();
			this.w_pnlRibbon.SuspendLayout();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.w_rbDoNotChangeRibbon, "w_rbDoNotChangeRibbon");
			this.w_rbDoNotChangeRibbon.Name = "w_rbDoNotChangeRibbon";
			this.w_rbDoNotChangeRibbon.Properties.AutoWidth = true;
			this.w_rbDoNotChangeRibbon.Properties.Caption = componentResourceManager.GetString("w_rbDoNotChangeRibbon.Properties.Caption");
			this.w_rbDoNotChangeRibbon.CheckedChanged += new System.EventHandler(this.On_CheckedChanged);
			this.w_rbDoNotChangeRibbon.Properties.CheckStyle = CheckStyles.Radio;
			this.w_rbDoNotChangeRibbon.Properties.RadioGroupIndex = 1;
			componentResourceManager.ApplyResources(this.w_rbShowRibbon, "w_rbShowRibbon");
			this.w_rbShowRibbon.Name = "w_rbShowRibbon";
			this.w_rbShowRibbon.Properties.AutoWidth = true;
			this.w_rbShowRibbon.Properties.Caption = componentResourceManager.GetString("w_rbShowRibbon.Properties.Caption");
			this.w_rbShowRibbon.CheckedChanged += new System.EventHandler(this.On_CheckedChanged);
			this.w_rbShowRibbon.Properties.CheckStyle = CheckStyles.Radio;
			this.w_rbShowRibbon.Properties.RadioGroupIndex = 1;
			componentResourceManager.ApplyResources(this.w_rbHideRibbon, "w_rbHideRibbon");
			this.w_rbHideRibbon.Name = "w_rbHideRibbon";
			this.w_rbHideRibbon.Properties.AutoWidth = true;
			this.w_rbHideRibbon.Properties.Caption = componentResourceManager.GetString("w_rbHideRibbon.Properties.Caption");
			this.w_rbHideRibbon.CheckedChanged += new System.EventHandler(this.On_CheckedChanged);
			this.w_rbHideRibbon.Properties.CheckStyle = CheckStyles.Radio;
			this.w_rbHideRibbon.Properties.RadioGroupIndex = 1;
			this.w_lblDescription.Appearance.Font = (System.Drawing.Font)componentResourceManager.GetObject("w_lblDescription.Appearance.Font");
			componentResourceManager.ApplyResources(this.w_lblDescription, "w_lblDescription");
			this.w_lblDescription.Name = "w_lblDescription";
			this.w_pnlRibbon.BorderStyle = BorderStyles.NoBorder;
			this.w_pnlRibbon.Controls.Add(this.w_rbHideRibbon);
			this.w_pnlRibbon.Controls.Add(this.w_rbShowRibbon);
			this.w_pnlRibbon.Controls.Add(this.w_rbDoNotChangeRibbon);
			this.w_pnlRibbon.Controls.Add(this.w_lblDescription);
			componentResourceManager.ApplyResources(this.w_pnlRibbon, "w_pnlRibbon");
			this.w_pnlRibbon.Name = "w_pnlRibbon";
			this.w_lblSharePoint2010Only.Appearance.Font = (System.Drawing.Font)componentResourceManager.GetObject("w_lblSharePoint2010Only.Appearance.Font");
			componentResourceManager.ApplyResources(this.w_lblSharePoint2010Only, "w_lblSharePoint2010Only");
			this.w_lblSharePoint2010Only.Name = "w_lblSharePoint2010Only";
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.w_lblSharePoint2010Only);
			base.Controls.Add(this.w_pnlRibbon);
			base.Name = "TCNavigationRibbonOptions";
			((System.ComponentModel.ISupportInitialize)this.w_rbDoNotChangeRibbon.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbShowRibbon.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbHideRibbon.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_pnlRibbon).EndInit();
			this.w_pnlRibbon.ResumeLayout(false);
			this.w_pnlRibbon.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
