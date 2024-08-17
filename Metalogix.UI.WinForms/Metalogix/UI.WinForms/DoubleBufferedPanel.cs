using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms
{
	public class DoubleBufferedPanel : Panel
	{
		private IContainer components;

		public DoubleBufferedPanel()
		{
			base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
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
		}
	}
}