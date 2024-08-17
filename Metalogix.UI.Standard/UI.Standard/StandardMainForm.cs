using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Metalogix.Actions;
using Metalogix.UI.Standard.Explorer;
using Metalogix.UI.WinForms;

namespace Metalogix.UI.Standard
{
	public class StandardMainForm : UIMainForm
	{
		private IContainer components;

		private STApplicationControl stApplicationControl1;

		private STApplicationControl stApplicationControl2;

		public StandardMainForm()
		{
			InitializeComponent();
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
			stApplicationControl1 = new STApplicationControl();
			stApplicationControl2 = new STApplicationControl();
			splitContainerV.Panel1.SuspendLayout();
			splitContainerV.Panel2.SuspendLayout();
			splitContainerV.SuspendLayout();
			((XtraForm)(object)this).SuspendLayout();
			splitContainerV.Panel1.Controls.Add(stApplicationControl1);
			splitContainerV.Panel2.Controls.Add(stApplicationControl2);
			splitContainerV.Size = new Size(748, 216);
			splitContainerV.SplitterDistance = 372;
			stApplicationControl1.Actions = new Action[0];
			stApplicationControl1.BackColor = Color.White;
			stApplicationControl1.ContextMenuStrip = base.ActionPaletteControl;
			stApplicationControl1.DataSource = null;
			stApplicationControl1.Dock = DockStyle.Fill;
			stApplicationControl1.ExplorerMultiSelectEnabled = false;
			stApplicationControl1.ExplorerMultiSelectLimitationMethod = null;
			stApplicationControl1.ItemsViewDataConverter = null;
			stApplicationControl1.Location = new Point(0, 0);
			stApplicationControl1.Name = "stApplicationControl1";
			stApplicationControl1.ShowExplorerCheckBoxes = false;
			stApplicationControl1.Size = new Size(372, 216);
			stApplicationControl1.TabIndex = 0;
			stApplicationControl2.Actions = new Action[0];
			stApplicationControl2.BackColor = Color.White;
			stApplicationControl2.ContextMenuStrip = base.ActionPaletteControl;
			stApplicationControl2.DataSource = null;
			stApplicationControl2.Dock = DockStyle.Fill;
			stApplicationControl2.ExplorerMultiSelectEnabled = false;
			stApplicationControl2.ExplorerMultiSelectLimitationMethod = null;
			stApplicationControl2.ItemsViewDataConverter = null;
			stApplicationControl2.Location = new Point(0, 0);
			stApplicationControl2.Name = "stApplicationControl2";
			stApplicationControl2.ShowExplorerCheckBoxes = false;
			stApplicationControl2.Size = new Size(372, 216);
			stApplicationControl2.TabIndex = 0;
			((ContainerControl)(object)this).AutoScaleDimensions = new SizeF(6f, 13f);
			((ContainerControl)(object)this).AutoScaleMode = AutoScaleMode.Font;
			((Form)(object)this).ClientSize = new Size(748, 517);
			base.LeftApplicationControl = stApplicationControl1;
			((Control)(object)this).Name = "StandardMainForm";
			base.RightApplicationControl = stApplicationControl2;
			splitContainerV.Panel1.ResumeLayout(performLayout: false);
			splitContainerV.Panel2.ResumeLayout(performLayout: false);
			splitContainerV.ResumeLayout(performLayout: false);
			((XtraForm)(object)this).ResumeLayout(performLayout: false);
		}
	}
}
