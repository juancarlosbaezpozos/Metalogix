using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Components
{
	public class CollapsableComponent
	{
		public CollapsableComponent()
		{
		}

		public static void HideControl(Control control, ContainerControl root)
		{
			if (control.Parent != null)
			{
				Control parent = control;
				do
				{
					Control control1 = parent;
					parent = parent.Parent;
					foreach (Control point in parent.Controls)
					{
						if (point.Location.Y <= control1.Location.Y || (point.Anchor & AnchorStyles.Bottom) == AnchorStyles.Bottom)
						{
							continue;
						}
						DockStyle dock = point.Dock;
						point.Dock = DockStyle.None;
						int x = point.Location.X;
						int y = point.Location.Y;
						Size size = control.Size;
						point.Location = new Point(x, y - size.Height);
						point.Dock = dock;
					}
					if (parent != root && (parent.Anchor & AnchorStyles.Bottom) == AnchorStyles.Bottom)
					{
						continue;
					}
					parent.Height = parent.Height - control.Height;
				}
				while (parent != root);
				control.Visible = false;
			}
		}

		public static void ShowControl(Control control, Control newParentControl, bool bExpandParent)
		{
			Control control1;
			if (newParentControl != null && control.Parent != newParentControl)
			{
				if (control.Parent != null)
				{
					control.Parent.Controls.Remove(control);
				}
				newParentControl.Controls.Add(control);
			}
			if (control.Parent == null)
			{
				return;
			}
			List<Control> controls = new List<Control>();
			Control parent = newParentControl;
			if (!bExpandParent)
			{
				controls.Add(newParentControl);
			}
			else
			{
				do
				{
					controls.Insert(0, parent);
					parent = parent.Parent;
				}
				while (parent != null);
			}
			foreach (Control height in controls)
			{
				control1 = (height != newParentControl ? controls[controls.IndexOf(height) + 1] : control);
				height.Height = height.Height + control.Height;
				foreach (Control point in height.Controls)
				{
					if (point.Location.Y <= control1.Location.Y || (point.Anchor & AnchorStyles.Bottom) == AnchorStyles.Bottom)
					{
						continue;
					}
					DockStyle dock = point.Dock;
					point.Dock = DockStyle.None;
					int x = point.Location.X;
					int y = point.Location.Y;
					Size size = control.Size;
					point.Location = new Point(x, y + size.Height);
					point.Dock = dock;
				}
			}
			control.Visible = true;
		}
	}
}