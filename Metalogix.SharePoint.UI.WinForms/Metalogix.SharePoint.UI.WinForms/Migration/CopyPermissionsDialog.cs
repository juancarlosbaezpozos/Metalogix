using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Metalogix.Actions;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class CopyPermissionsDialog : ScopableLeftNavigableTabsForm
	{
		private TCPermissionsOptions w_tcPermissionsOptions = new TCPermissionsOptions();

		private TCMappingOptions w_tcMappingsOptions = new TCMappingOptions();

		private TCGeneralOptions w_tcGeneralOptions = new TCGeneralOptions();

		private IContainer components;

		public CopyPermissionsOptions Options
		{
			get
			{
				return Action.Options as CopyPermissionsOptions;
			}
			set
			{
				Action.Options = value;
				LoadUI(Action);
			}
		}

		public CopyPermissionsDialog(SharePointObjectScope scope)
		{
			InitializeComponent();
			Text = "Configure Permissions Copying Options";
			List<TabbableControl> tabs = new List<TabbableControl> { w_tcPermissionsOptions, w_tcMappingsOptions, w_tcGeneralOptions };
			w_tcPermissionsOptions.IndependantlyScoped = true;
			w_tcPermissionsOptions.Scope = scope;
			w_tcGeneralOptions.Scope = SharePointObjectScope.Permissions;
			w_tcMappingsOptions.Scope = SharePointObjectScope.Permissions;
			base.Tabs = tabs;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.CopyPermissionsDialog));
			base.SuspendLayout();
			base.Appearance.BackColor = (System.Drawing.Color)resources.GetObject("CopyPermissionsDialog.Appearance.BackColor");
			base.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this, "$this");
			base.LookAndFeel.SkinName = "Office 2013";
			base.Name = "CopyPermissionsDialog";
			base.ShowIcon = true;
			base.ActionTemplatesSupported = true;
			base.ResumeLayout(false);
		}

		protected override void LoadUI(Action action)
		{
			base.LoadUI(action);
			SPPermissionsOptions sPPermissionsOptions = new SPPermissionsOptions();
			SPMappingOptions sPMappingOptions = new SPMappingOptions();
			SPGeneralOptions sPGeneralOptions = new SPGeneralOptions();
			sPPermissionsOptions.SetFromOptions(action.Options);
			sPMappingOptions.SetFromOptions(action.Options);
			sPGeneralOptions.SetFromOptions(action.Options);
			w_tcPermissionsOptions.Options = sPPermissionsOptions;
			w_tcMappingsOptions.Options = sPMappingOptions;
			w_tcGeneralOptions.Options = sPGeneralOptions;
		}

		protected override bool SaveUI(Action action)
		{
			if (!base.SaveUI(action))
			{
				return false;
			}
			action.Options.SetFromOptions(w_tcPermissionsOptions.Options);
			action.Options.SetFromOptions(w_tcMappingsOptions.Options);
			action.Options.SetFromOptions(w_tcGeneralOptions.Options);
			return true;
		}
	}
}
