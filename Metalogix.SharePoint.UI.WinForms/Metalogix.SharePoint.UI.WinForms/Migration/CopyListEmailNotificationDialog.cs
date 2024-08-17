using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class CopyListEmailNotificationDialog : ScopableLeftNavigableTabsForm
	{
		private TCListEmailNotificationOptions w_tcEmailNotificationOptions = new TCListEmailNotificationOptions();

		private ListEmailNotificationOptions m_options;

		private IContainer components;

		public ListEmailNotificationOptions Options
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

		public CopyListEmailNotificationDialog()
		{
			InitializeComponent();
			Text = "Configure List E-mail Notification Copying Options";
			base.Size = new Size(550, 150);
			base.Tabs = new List<TabbableControl> { w_tcEmailNotificationOptions };
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
			base.SuspendLayout();
			base.Appearance.BackColor = System.Drawing.Color.White;
			base.Appearance.Options.UseBackColor = true;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(784, 502);
			base.LookAndFeel.SkinName = "Office 2013";
			base.Name = "CopyListEmailNotificationDialog";
			this.Text = "CopyListEmailNotificationDialog";
			base.ResumeLayout(false);
		}

		private void LoadUI()
		{
			SPListEmailNotificationOptions sPListEmailNotificationOptions = new SPListEmailNotificationOptions();
			sPListEmailNotificationOptions.SetFromOptions(Options);
			w_tcEmailNotificationOptions.Options = sPListEmailNotificationOptions;
		}

		protected override bool SaveUI()
		{
			if (!base.SaveUI())
			{
				return false;
			}
			Options.SetFromOptions(w_tcEmailNotificationOptions.Options);
			return true;
		}
	}
}
