using Metalogix.SharePoint.Options.Administration.Navigation;
using Metalogix.UI.WinForms.Components;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Administration.Navigation
{
    public class ChangeQuickLaunchDialog : ChangeNavigationTemplateDialog
    {
        private TCQuickLaunchOptions w_tcQuickLaunch = new TCQuickLaunchOptions();
        private TCNavigationSortingOptions w_tcSorting = new TCNavigationSortingOptions();
        private ChangeNavigationSettingsOptions m_Options;
        private IContainer components;

        public ChangeNavigationSettingsOptions Options
        {
            get => this.m_Options;
            set
            {
                this.m_Options = value;
                this.LoadUI();
            }
        }

        public ChangeQuickLaunchDialog()
        {
            this.InitializeComponent();
            this.Text = "Change Quick Launch Settings";
            List<TabbableControl> tabbableControlList = new List<TabbableControl>()
      {
        (TabbableControl) this.w_tcQuickLaunch,
        (TabbableControl) this.w_tcSorting
      };
            foreach (ScopableTabbableControl scopableTabbableControl in tabbableControlList)
                scopableTabbableControl.Scope = SharePointObjectScope.Site;
            this.Tabs = tabbableControlList;
            // ISSUE: variable of a compiler-generated type
            TCQuickLaunchOptions wTcQuickLaunch1 = this.w_tcQuickLaunch;
            // ISSUE: reference to a compiler-generated method
            this.StartPropagatingNavigationEvent += new ChangeNavigationTemplateDialog.NavigationPropagationDelegate(wTcQuickLaunch1.StartNavigationPropagationHandler);
            // ISSUE: variable of a compiler-generated type
            TCQuickLaunchOptions wTcQuickLaunch2 = this.w_tcQuickLaunch;
            // ISSUE: reference to a compiler-generated method
            this.StopPropagatingNavigationEvent += new ChangeNavigationTemplateDialog.NavigationPropagationDelegate(wTcQuickLaunch2.StopNavigationPropagationHandler);
            // ISSUE: variable of a compiler-generated type
            TCNavigationSortingOptions wTcSorting1 = this.w_tcSorting;
            // ISSUE: reference to a compiler-generated method
            this.StartPropagatingNavigationEvent += new ChangeNavigationTemplateDialog.NavigationPropagationDelegate(wTcSorting1.StartNavigationPropagationHandler);
            // ISSUE: variable of a compiler-generated type
            TCNavigationSortingOptions wTcSorting2 = this.w_tcSorting;
            // ISSUE: reference to a compiler-generated method
            this.StopPropagatingNavigationEvent += new ChangeNavigationTemplateDialog.NavigationPropagationDelegate(wTcSorting2.StopNavigationPropagationHandler);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ChangeQuickLaunchDialog));
            this.SuspendLayout();
            componentResourceManager.ApplyResources((object)this.w_btnSave, "w_btnSave");
            componentResourceManager.ApplyResources((object)this, "$this");
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Name = nameof(ChangeQuickLaunchDialog);
            this.ShowIcon = true;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void LoadUI()
        {
            SPQuickLaunchOptions quickLaunchOptions = new SPQuickLaunchOptions();
            quickLaunchOptions.SetFromOptions((OptionsBase)this.Options);
            this.w_tcQuickLaunch.Options = quickLaunchOptions;
            SPNavigationSortingOptions navigationSortingOptions = new SPNavigationSortingOptions();
            navigationSortingOptions.SetFromOptions((OptionsBase)this.Options);
            this.w_tcSorting.Options = navigationSortingOptions;
        }

        protected override bool SaveUI()
        {
            if (base.SaveUI())
            {
                this.Options.SetFromOptions((OptionsBase)this.w_tcQuickLaunch.Options);
                this.Options.SetFromOptions((OptionsBase)this.w_tcSorting.Options);
                this.Options.ApplyChangesToParentSites = this.PushChangesToParents;
                this.Options.ApplyChangesToSubSites = this.PushChangesToSubsites;
            }
            return true;
        }
    }
}
