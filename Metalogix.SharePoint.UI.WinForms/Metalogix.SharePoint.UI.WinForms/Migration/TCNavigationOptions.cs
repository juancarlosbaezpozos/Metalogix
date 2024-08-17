using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Components;
using TooltipsTest;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	[ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Migration.Navigation32.ico")]
	[ControlName("Navigation Options")]
	public class TCNavigationOptions : ScopableTabbableControl
	{
		private SPNavigationOptions m_options;

		private IContainer components;

		private CheckEdit w_rbCopyOnlyQuickLaunch;

		private CheckEdit w_rbCopyOnlyTopNavBar;

		private CheckEdit w_rbCopyBoth;

		private LabelControl w_lbWhichNavElements;

		private CheckEdit w_cbCopySubSiteNavigation;

		private CheckEdit w_cbComprehensiveLinkCorrection;

		private HelpTipButton helpTipCopyOnlyQuickLaunch;

		public SPNavigationOptions Options
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

		protected bool SourceIs2003
		{
			get
			{
				if (SourceNodes == null || SourceNodes.Count == 0 || !(SourceNodes[0] is SPWeb))
				{
					return false;
				}
				return ((SPWeb)SourceNodes[0]).Adapter.SharePointVersion.IsSharePoint2003;
			}
		}

		public TCNavigationOptions()
		{
			InitializeComponent();
			helpTipCopyOnlyQuickLaunch.SetResourceString(GetType().FullName + w_rbCopyOnlyQuickLaunch.Name, GetType());
			helpTipCopyOnlyQuickLaunch.VisibleChanged += helpTipCopyOnlyQuickLaunch_VisibleChanged;
		}

		private void DisableOnlyQuickLaunchAndOnlyTopLinkOption()
		{
			if (SPUIUtils.IsOptionsCopyingOnlyTopLinkAndQuickLaunchToBeDisabled(SourceNodes, TargetNodes))
			{
				w_rbCopyOnlyQuickLaunch.Enabled = false;
				w_rbCopyOnlyTopNavBar.Enabled = false;
				w_rbCopyBoth.Checked = true;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void helpTipCopyOnlyQuickLaunch_VisibleChanged(object sender, EventArgs e)
		{
			helpTipCopyOnlyQuickLaunch.Visible = SPUIUtils.IsOptionsCopyingOnlyTopLinkAndQuickLaunchToBeDisabled(SourceNodes, TargetNodes);
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.TCNavigationOptions));
			this.w_rbCopyOnlyQuickLaunch = new DevExpress.XtraEditors.CheckEdit();
			this.w_rbCopyOnlyTopNavBar = new DevExpress.XtraEditors.CheckEdit();
			this.w_rbCopyBoth = new DevExpress.XtraEditors.CheckEdit();
			this.w_lbWhichNavElements = new DevExpress.XtraEditors.LabelControl();
			this.w_cbCopySubSiteNavigation = new DevExpress.XtraEditors.CheckEdit();
			this.w_cbComprehensiveLinkCorrection = new DevExpress.XtraEditors.CheckEdit();
			this.helpTipCopyOnlyQuickLaunch = new TooltipsTest.HelpTipButton();
			((System.ComponentModel.ISupportInitialize)this.w_rbCopyOnlyQuickLaunch.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbCopyOnlyTopNavBar.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbCopyBoth.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopySubSiteNavigation.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbComprehensiveLinkCorrection.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.helpTipCopyOnlyQuickLaunch).BeginInit();
			base.SuspendLayout();
			resources.ApplyResources(this.w_rbCopyOnlyQuickLaunch, "w_rbCopyOnlyQuickLaunch");
			this.w_rbCopyOnlyQuickLaunch.Name = "w_rbCopyOnlyQuickLaunch";
			this.w_rbCopyOnlyQuickLaunch.Properties.AutoWidth = true;
			this.w_rbCopyOnlyQuickLaunch.Properties.Caption = resources.GetString("w_rbCopyOnlyQuickLaunch.Properties.Caption");
			this.w_rbCopyOnlyQuickLaunch.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.w_rbCopyOnlyQuickLaunch.Properties.RadioGroupIndex = 1;
			this.w_rbCopyOnlyQuickLaunch.TabStop = false;
			this.helpTipCopyOnlyQuickLaunch.AnchoringControl = this.w_rbCopyOnlyQuickLaunch;
			this.helpTipCopyOnlyQuickLaunch.BackColor = System.Drawing.Color.Transparent;
			this.helpTipCopyOnlyQuickLaunch.CommonParentControl = null;
			resources.ApplyResources(this.helpTipCopyOnlyQuickLaunch, "helpTipCopyOnlyQuickLaunch");
			this.helpTipCopyOnlyQuickLaunch.Name = "helpTipCopyOnlyQuickLaunch";
			this.helpTipCopyOnlyQuickLaunch.TabStop = false;
			resources.ApplyResources(this.w_rbCopyOnlyTopNavBar, "w_rbCopyOnlyTopNavBar");
			this.w_rbCopyOnlyTopNavBar.Name = "w_rbCopyOnlyTopNavBar";
			this.w_rbCopyOnlyTopNavBar.Properties.AutoWidth = true;
			this.w_rbCopyOnlyTopNavBar.Properties.Caption = resources.GetString("w_rbCopyOnlyTopNavBar.Properties.Caption");
			this.w_rbCopyOnlyTopNavBar.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.w_rbCopyOnlyTopNavBar.Properties.RadioGroupIndex = 1;
			this.w_rbCopyOnlyTopNavBar.TabStop = false;
			resources.ApplyResources(this.w_rbCopyBoth, "w_rbCopyBoth");
			this.w_rbCopyBoth.Name = "w_rbCopyBoth";
			this.w_rbCopyBoth.Properties.AutoWidth = true;
			this.w_rbCopyBoth.Properties.Caption = resources.GetString("w_rbCopyBoth.Properties.Caption");
			this.w_rbCopyBoth.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.w_rbCopyBoth.Properties.RadioGroupIndex = 1;
			this.w_rbCopyBoth.TabStop = false;
			resources.ApplyResources(this.w_lbWhichNavElements, "w_lbWhichNavElements");
			this.w_lbWhichNavElements.Name = "w_lbWhichNavElements";
			resources.ApplyResources(this.w_cbCopySubSiteNavigation, "w_cbCopySubSiteNavigation");
			this.w_cbCopySubSiteNavigation.Name = "w_cbCopySubSiteNavigation";
			this.w_cbCopySubSiteNavigation.Properties.AutoWidth = true;
			this.w_cbCopySubSiteNavigation.Properties.Caption = resources.GetString("w_cbCopySubSiteNavigation.Properties.Caption");
			resources.ApplyResources(this.w_cbComprehensiveLinkCorrection, "w_cbComprehensiveLinkCorrection");
			this.w_cbComprehensiveLinkCorrection.Name = "w_cbComprehensiveLinkCorrection";
			this.w_cbComprehensiveLinkCorrection.Properties.AutoWidth = true;
			this.w_cbComprehensiveLinkCorrection.Properties.Caption = resources.GetString("w_cbComprehensiveLinkCorrection.Properties.Caption");
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.w_cbComprehensiveLinkCorrection);
			base.Controls.Add(this.w_cbCopySubSiteNavigation);
			base.Controls.Add(this.w_lbWhichNavElements);
			base.Controls.Add(this.w_rbCopyBoth);
			base.Controls.Add(this.helpTipCopyOnlyQuickLaunch);
			base.Controls.Add(this.w_rbCopyOnlyTopNavBar);
			base.Controls.Add(this.w_rbCopyOnlyQuickLaunch);
			base.Name = "TCNavigationOptions";
			((System.ComponentModel.ISupportInitialize)this.w_rbCopyOnlyQuickLaunch.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbCopyOnlyTopNavBar.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.helpTipCopyOnlyQuickLaunch).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbCopyBoth.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbCopySubSiteNavigation.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cbComprehensiveLinkCorrection.Properties).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		protected override void LoadUI()
		{
			if (SourceIs2003)
			{
				w_rbCopyOnlyQuickLaunch.Checked = true;
			}
			else if (!Options.CopyCurrentNavigation)
			{
				w_rbCopyOnlyTopNavBar.Checked = true;
			}
			else if (Options.CopyGlobalNavigation)
			{
				w_rbCopyBoth.Checked = true;
			}
			else
			{
				w_rbCopyOnlyQuickLaunch.Checked = true;
			}
			DisableOnlyQuickLaunchAndOnlyTopLinkOption();
			w_cbCopySubSiteNavigation.Checked = Options.Recursive;
			w_cbComprehensiveLinkCorrection.Checked = Options.UsingComprehensiveLinkCorrection;
		}

		public override bool SaveUI()
		{
			Options.CopyCurrentNavigation = !w_rbCopyOnlyTopNavBar.Checked;
			Options.CopyGlobalNavigation = !w_rbCopyOnlyQuickLaunch.Checked;
			Options.Recursive = w_cbCopySubSiteNavigation.Checked;
			Options.UsingComprehensiveLinkCorrection = w_cbComprehensiveLinkCorrection.Checked;
			return true;
		}

		protected override void UpdateEnabledState()
		{
			w_rbCopyBoth.Enabled = !SourceIs2003;
			w_rbCopyOnlyQuickLaunch.Enabled = w_rbCopyBoth.Enabled;
			w_rbCopyOnlyTopNavBar.Enabled = w_rbCopyBoth.Enabled;
			DisableOnlyQuickLaunchAndOnlyTopLinkOption();
		}
	}
}
