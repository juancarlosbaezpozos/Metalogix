using System;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Widgets
{
	public class DisableLineRenderer : ToolStripSystemRenderer
	{
		public DisableLineRenderer()
		{
		}

		protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
		{
		}
	}
}