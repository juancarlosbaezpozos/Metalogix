using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Metalogix.Actions;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class CopyDefaultWebPartPageDialog : ScopableLeftNavigableTabsForm
	{
		private TCWebPartsOptions w_tcWebPartsOptions = new TCWebPartsOptions();

		private TCMappingOptions w_tcMappingOptions = new TCMappingOptions();

		private IContainer components;

		public WebPartOptions Options
		{
			get
			{
				return Action.Options as WebPartOptions;
			}
			set
			{
				Action.Options = value;
				LoadUI(Action);
			}
		}

		public CopyDefaultWebPartPageDialog()
		{
			InitializeComponent();
			Text = "Configure Default Web Part Page Copying Options";
			base.Size = new Size(550, 300);
			List<TabbableControl> tabs = new List<TabbableControl> { w_tcWebPartsOptions, w_tcMappingOptions };
			w_tcWebPartsOptions.ShowCopyWebPartsOnLandingPages = false;
			w_tcWebPartsOptions.ShowCopyWebPartsOnWebPartPages = false;
			w_tcWebPartsOptions.ShowCopyFormPageWebParts = false;
			w_tcWebPartsOptions.ShowCopyWebPartsOnViewPages = false;
			w_tcWebPartsOptions.ShowCopyWebPartsRecursive = true;
			w_tcMappingOptions.Scope = SharePointObjectScope.Permissions;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.CopyDefaultWebPartPageDialog));
			((System.ComponentModel.ISupportInitialize)base.tabControl).BeginInit();
			base.SuspendLayout();
			base.tabControl.LookAndFeel.SkinName = "Office 2013";
			base.tabControl.LookAndFeel.UseDefaultLookAndFeel = false;
			base.ActionTemplatesSupported = true;
			base.Appearance.BackColor = (System.Drawing.Color)resources.GetObject("CopyDefaultWebPartPageDialog.Appearance.BackColor");
			base.Appearance.Options.UseBackColor = true;
			resources.ApplyResources(this, "$this");
			base.LookAndFeel.SkinName = "Office 2013";
			base.Name = "CopyDefaultWebPartPageDialog";
			base.ShowIcon = true;
			((System.ComponentModel.ISupportInitialize)base.tabControl).EndInit();
			base.ResumeLayout(false);
		}

		protected override void LoadUI(Action action)
		{
			base.LoadUI(action);
			SPWebPartOptions sPWebPartOptions = new SPWebPartOptions();
			SPMappingOptions sPMappingOptions = new SPMappingOptions();
			sPWebPartOptions.SetFromOptions(action.Options);
			sPMappingOptions.SetFromOptions(action.Options);
			w_tcWebPartsOptions.Options = sPWebPartOptions;
			w_tcMappingOptions.Options = sPMappingOptions;
		}

		protected override bool SaveUI(Action action)
		{
			if (!base.SaveUI(action))
			{
				return false;
			}
			action.Options.SetFromOptions(w_tcWebPartsOptions.Options);
			action.Options.SetFromOptions(w_tcMappingOptions.Options);
			return true;
		}
	}
}
