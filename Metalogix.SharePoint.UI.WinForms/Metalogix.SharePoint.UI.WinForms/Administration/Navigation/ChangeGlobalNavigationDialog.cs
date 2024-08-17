using Metalogix.SharePoint.Options.Administration.Navigation;
using Metalogix.UI.WinForms.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Administration.Navigation
{
	public partial class ChangeGlobalNavigationDialog : ChangeNavigationTemplateDialog
	{
		private TCGlobalNavigationOptions w_tcGlobalNavigation = new TCGlobalNavigationOptions();

		private TCNavigationSortingOptions w_tcSorting = new TCNavigationSortingOptions();

		private TCNavigationRibbonOptions w_tcRibbon = new TCNavigationRibbonOptions();

		private ChangeNavigationSettingsOptions m_Options;

		private System.ComponentModel.IContainer components;

		public ChangeNavigationSettingsOptions Options
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

		public ChangeGlobalNavigationDialog()
		{
			this.InitializeComponent();
			this.Text = "Change Global Navigation Settings";
			System.Collections.Generic.List<TabbableControl> list = new System.Collections.Generic.List<TabbableControl>();
			list.Add(this.w_tcGlobalNavigation);
			list.Add(this.w_tcSorting);
			list.Add(this.w_tcRibbon);
			using (System.Collections.Generic.List<TabbableControl>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ScopableTabbableControl scopableTabbableControl = (ScopableTabbableControl)enumerator.Current;
					scopableTabbableControl.Scope = SharePointObjectScope.Site;
				}
			}
			base.Tabs = list;
			base.StartPropagatingNavigationEvent += new ChangeNavigationTemplateDialog.NavigationPropagationDelegate(this.w_tcRibbon.StartNavigationPropagationHandler);
			base.StopPropagatingNavigationEvent += new ChangeNavigationTemplateDialog.NavigationPropagationDelegate(this.w_tcRibbon.StopNavigationPropagationHandler);
			base.StartPropagatingNavigationEvent += new ChangeNavigationTemplateDialog.NavigationPropagationDelegate(this.w_tcSorting.StartNavigationPropagationHandler);
			base.StopPropagatingNavigationEvent += new ChangeNavigationTemplateDialog.NavigationPropagationDelegate(this.w_tcSorting.StopNavigationPropagationHandler);
			base.StartPropagatingNavigationEvent += new ChangeNavigationTemplateDialog.NavigationPropagationDelegate(this.w_tcGlobalNavigation.StartNavigationPropagationHandler);
			base.StopPropagatingNavigationEvent += new ChangeNavigationTemplateDialog.NavigationPropagationDelegate(this.w_tcGlobalNavigation.StopNavigationPropagationHandler);
		}

		private void LoadUI()
		{
			SPGlobalNavigationOptions sPGlobalNavigationOptions = new SPGlobalNavigationOptions();
			sPGlobalNavigationOptions.SetFromOptions(this.Options);
			this.w_tcGlobalNavigation.Options = sPGlobalNavigationOptions;
			SPNavigationSortingOptions sPNavigationSortingOptions = new SPNavigationSortingOptions();
			sPNavigationSortingOptions.SetFromOptions(this.Options);
			this.w_tcSorting.Options = sPNavigationSortingOptions;
			SPNavigationRibbonOptions sPNavigationRibbonOptions = new SPNavigationRibbonOptions();
			sPNavigationRibbonOptions.SetFromOptions(this.Options);
			this.w_tcRibbon.Options = sPNavigationRibbonOptions;
		}

		protected override bool SaveUI()
		{
			if (base.SaveUI())
			{
				this.Options.SetFromOptions(this.w_tcGlobalNavigation.Options);
				this.Options.SetFromOptions(this.w_tcSorting.Options);
				this.Options.SetFromOptions(this.w_tcRibbon.Options);
				this.Options.ApplyChangesToParentSites = base.PushChangesToParents;
				this.Options.ApplyChangesToSubSites = base.PushChangesToSubsites;
			}
			return true;
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
			System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(ChangeGlobalNavigationDialog));
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.w_btnCancel, "w_btnCancel");
			componentResourceManager.ApplyResources(this.w_btnOK, "w_btnOK");
			componentResourceManager.ApplyResources(this.w_btnSave, "w_btnSave");
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Name = "ChangeGlobalNavigationDialog";
			base.ShowIcon = true;
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
