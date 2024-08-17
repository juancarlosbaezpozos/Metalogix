using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Components
{
	public class XtraWizardTabbableControl : TabbableControl
	{
		private IContainer components;

		public XtraWizardTabbableControl()
		{
			this.InitializeComponent();
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
			this.components = new System.ComponentModel.Container();
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		}

		public virtual new bool LoadUI()
		{
			return true;
		}

		public virtual bool ValidatePage()
		{
			return true;
		}
	}
}