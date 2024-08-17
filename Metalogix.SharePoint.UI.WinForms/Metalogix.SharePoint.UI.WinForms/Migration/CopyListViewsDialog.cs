using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Metalogix.Actions;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class CopyListViewsDialog : ScopableLeftNavigableTabsForm
	{
		private TCListViewsOptions w_tcListViewsOptions = new TCListViewsOptions();

		private TCWebPartsOptions w_tcWebPartOptions = new TCWebPartsOptions();

		private TCGeneralOptions w_tcGeneralOptions = new TCGeneralOptions();

		private IContainer components;

		public PasteListViewsOptions Options
		{
			get
			{
				return Action.Options as PasteListViewsOptions;
			}
			set
			{
				Action.Options = value;
				LoadUI(Action);
			}
		}

		public CopyListViewsDialog()
		{
			InitializeComponent();
			Text = "Configure List Views Copying Options";
			List<TabbableControl> tabs = new List<TabbableControl> { w_tcListViewsOptions, w_tcWebPartOptions, w_tcGeneralOptions };
			w_tcWebPartOptions.ShowCopyWebPartsOnLandingPages = false;
			w_tcWebPartOptions.ShowCopyWebPartsOnWebPartPages = false;
			w_tcWebPartOptions.ShowCopyFormPageWebParts = false;
			w_tcGeneralOptions.Scope = SharePointObjectScope.List;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.CopyListViewsDialog));
			base.SuspendLayout();
			base.Appearance.BackColor = System.Drawing.Color.White;
			base.Appearance.Options.UseBackColor = true;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.ClientSize = new System.Drawing.Size(784, 502);
			base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			base.LookAndFeel.SkinName = "Office 2013";
			base.Name = "CopyListViewsDialog";
			base.ShowIcon = true;
			base.ActionTemplatesSupported = true;
			base.ResumeLayout(false);
		}

		protected override void LoadUI(Action action)
		{
			base.LoadUI(action);
			SPListViewsOptions sPListViewsOptions = new SPListViewsOptions();
			SPWebPartOptions sPWebPartOptions = new SPWebPartOptions();
			SPGeneralOptions sPGeneralOptions = new SPGeneralOptions();
			sPListViewsOptions.SetFromOptions(action.Options);
			sPWebPartOptions.SetFromOptions(action.Options);
			sPGeneralOptions.SetFromOptions(action.Options);
			w_tcListViewsOptions.Options = sPListViewsOptions;
			w_tcWebPartOptions.Options = sPWebPartOptions;
			w_tcGeneralOptions.Options = sPGeneralOptions;
		}

		protected override bool SaveUI(Action action)
		{
			if (!base.SaveUI(action))
			{
				return false;
			}
			action.Options.SetFromOptions(w_tcListViewsOptions.Options);
			action.Options.SetFromOptions(w_tcWebPartOptions.Options);
			action.Options.SetFromOptions(w_tcGeneralOptions.Options);
			return true;
		}
	}
}
