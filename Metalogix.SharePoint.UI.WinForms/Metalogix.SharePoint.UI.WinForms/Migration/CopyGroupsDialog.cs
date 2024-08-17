using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Metalogix.Actions;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class CopyGroupsDialog : ScopableLeftNavigableTabsForm
	{
		private TCGroupOptions w_tcGroupOptions = new TCGroupOptions();

		private TCGeneralOptions w_tcGeneralOptions = new TCGeneralOptions();

		private IContainer components;

		public CopyGroupsOptions Options
		{
			get
			{
				return Action.Options as CopyGroupsOptions;
			}
			set
			{
				Action.Options = value;
				LoadUI(Action);
			}
		}

		public CopyGroupsDialog()
		{
			InitializeComponent();
			Text = "Configure Group Copying Options";
			List<TabbableControl> tabs = new List<TabbableControl> { w_tcGroupOptions, w_tcGeneralOptions };
			w_tcGeneralOptions.Scope = SharePointObjectScope.Permissions;
			w_tcGroupOptions.Scope = SharePointObjectScope.Permissions;
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
			((System.ComponentModel.ISupportInitialize)base.tabControl).BeginInit();
			base.SuspendLayout();
			base.tabControl.LookAndFeel.SkinName = "Office 2013";
			base.tabControl.LookAndFeel.UseDefaultLookAndFeel = false;
			base.ActionTemplatesSupported = true;
			base.Appearance.BackColor = System.Drawing.Color.White;
			base.Appearance.Options.UseBackColor = true;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(784, 502);
			base.LookAndFeel.SkinName = "Office 2013";
			base.Name = "CopyGroupsDialog";
			this.Text = "CopyGroupsDialog";
			((System.ComponentModel.ISupportInitialize)base.tabControl).EndInit();
			base.ResumeLayout(false);
		}

		protected override void LoadUI(Action action)
		{
			base.LoadUI(action);
			SPGroupOptions sPGroupOptions = new SPGroupOptions();
			SPGeneralOptions sPGeneralOptions = new SPGeneralOptions();
			sPGroupOptions.SetFromOptions(action.Options);
			sPGeneralOptions.SetFromOptions(action.Options);
			w_tcGroupOptions.Options = sPGroupOptions;
			w_tcGeneralOptions.Options = sPGeneralOptions;
		}

		protected override bool SaveUI(Action action)
		{
			if (!base.SaveUI(action))
			{
				return false;
			}
			action.Options.SetFromOptions(w_tcGroupOptions.Options);
			action.Options.SetFromOptions(w_tcGeneralOptions.Options);
			return true;
		}
	}
}
