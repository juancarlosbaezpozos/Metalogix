using Metalogix.UI.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Components.AnchoredControls
{
	public static class AnchoringUtils
	{
		public static void Anchor(IAnchoredControl anchoredControl, Control anchoringControl)
		{
			if (anchoringControl == null)
			{
				return;
			}
			if (anchoringControl.InvokeRequired)
			{
				AnchoringUtils.AnchoringDelegate anchoringDelegate = new AnchoringUtils.AnchoringDelegate(AnchoringUtils.Anchor);
				object[] objArray = new object[] { anchoredControl, anchoringControl };
				anchoringControl.Invoke(anchoringDelegate, objArray);
				return;
			}
			Point location = anchoringControl.Location;
			int x = location.X + AnchoringUtils.GetContentWidth(anchoringControl) + anchoredControl.RelativeOffset.X;
			Point point = anchoringControl.Location;
			Point point1 = new Point(x, point.Y + anchoredControl.RelativeOffset.Y);
			if (point1.X != anchoredControl.Location.X || point1.Y != anchoredControl.Location.Y)
			{
				anchoredControl.Location = point1;
			}
		}

		public static void AnchorVisibilityChanged(IAnchoredControl anchoredControl, Control anchoringControl)
		{
			if (anchoringControl == null)
			{
				return;
			}
			if (!anchoringControl.InvokeRequired)
			{
				anchoredControl.Visible = anchoringControl.Visible;
				return;
			}
			AnchoringUtils.AnchoringDelegate anchoringDelegate = new AnchoringUtils.AnchoringDelegate(AnchoringUtils.AnchorVisibilityChanged);
			object[] objArray = new object[] { anchoredControl, anchoringControl };
			anchoringControl.Invoke(anchoringDelegate, objArray);
		}

		public static void Bind(IAnchoredControl anchoredControl, Control anchoringControl)
		{
			if (anchoringControl != null)
			{
				IAnchoredControl anchoredControl1 = anchoredControl;
				anchoringControl.LocationChanged += new EventHandler(anchoredControl1.OnAnchorPointChanged);
				IAnchoredControl anchoredControl2 = anchoredControl;
				anchoringControl.SizeChanged += new EventHandler(anchoredControl2.OnAnchorPointChanged);
				AnchoringUtils.BindParenting(anchoredControl, anchoringControl);
				IAnchoredControl anchoredControl3 = anchoredControl;
				anchoringControl.VisibleChanged += new EventHandler(anchoredControl3.OnAnchorVisibleChanged);
				anchoredControl.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			}
		}

		private static void BindParenting(IAnchoredControl anchoredControl, Control anchoringControl)
		{
			Control parent = anchoringControl.Parent;
			Control control = anchoredControl.Parent;
			if (parent == null || control == null)
			{
				return;
			}
			Stack<Control> controls = new Stack<Control>();
			Stack<Control> controls1 = new Stack<Control>();
			Control control1 = null;
			while (parent != null)
			{
				if (parent != control)
				{
					controls.Push(parent);
					parent = parent.Parent;
				}
				else
				{
					control1 = control;
					break;
				}
			}
			if (control1 == null)
			{
				while (control != null)
				{
					controls1.Push(control);
					control = control.Parent;
				}
				while (controls.Count > 0 && controls1.Count > 0 && controls.Pop() == controls1.Peek())
				{
					control1 = controls1.Pop();
				}
			}
			anchoredControl.CommonParentControl = control1;
			while (controls1.Count > 0)
			{
				Control control2 = controls1.Pop();
				IAnchoredControl anchoredControl1 = anchoredControl;
				control2.LocationChanged += new EventHandler(anchoredControl1.OnAnchorContextChanged);
			}
			while (controls.Count > 0)
			{
				Control control3 = controls.Pop();
				IAnchoredControl anchoredControl2 = anchoredControl;
				control3.LocationChanged += new EventHandler(anchoredControl2.OnAnchorContextChanged);
			}
		}

		public static Coordinates CalculateRealOffset(IAnchoredControl anchoredControl, Control anchoringControl)
		{
			Control i;
			Coordinates coordinate = new Coordinates(anchoredControl.Location.X, anchoredControl.Location.Y);
			Point location = anchoringControl.Location;
			Coordinates coordinate1 = new Coordinates(location.X + AnchoringUtils.GetContentWidth(anchoringControl), anchoringControl.Location.Y);
			for (i = anchoredControl.Parent; i != anchoredControl.CommonParentControl; i = i.Parent)
			{
				coordinate.Add(i.Location);
			}
			for (i = anchoringControl.Parent; i != anchoredControl.CommonParentControl; i = i.Parent)
			{
				coordinate1.Add(i.Location);
			}
			coordinate.Subtract(coordinate1);
			return coordinate;
		}

		public static void ConfigureOffsets(IAnchoredControl anchoredControl, Control anchoringControl)
		{
			if (anchoringControl == null)
			{
				return;
			}
			if (anchoredControl.RealOffset != null)
			{
				AnchoringUtils.RecalculateOffsets(anchoredControl, anchoringControl);
				return;
			}
			anchoredControl.RealOffset = AnchoringUtils.CalculateRealOffset(anchoredControl, anchoringControl);
			int x = anchoredControl.Location.X;
			Point location = anchoringControl.Location;
			int num = x - (location.X + AnchoringUtils.GetContentWidth(anchoringControl));
			int y = anchoredControl.Location.Y;
			Point point = anchoringControl.Location;
			anchoredControl.RelativeOffset = new Coordinates(num, y - point.Y);
		}

		private static int GetContentWidth(Control control)
		{
			if (!control.AutoSize)
			{
				Label label = control as Label;
				if (label != null)
				{
					if (label.BorderStyle != BorderStyle.None)
					{
						return control.Width;
					}
					return Math.Min(label.Width, label.PreferredWidth);
				}
			}
			return control.Width;
		}

		public static void RecalculateOffsets(IAnchoredControl anchoredControl, Control anchoringControl)
		{
			if (anchoredControl.CommonParentControl.InvokeRequired)
			{
				Control commonParentControl = anchoredControl.CommonParentControl;
				AnchoringUtils.AnchoringDelegate anchoringDelegate = new AnchoringUtils.AnchoringDelegate(AnchoringUtils.RecalculateOffsets);
				object[] objArray = new object[] { anchoredControl, anchoringControl };
				commonParentControl.Invoke(anchoringDelegate, objArray);
				return;
			}
			Coordinates coordinate = AnchoringUtils.CalculateRealOffset(anchoredControl, anchoringControl);
			int x = anchoredControl.RealOffset.X - coordinate.X;
			int y = anchoredControl.RealOffset.Y - coordinate.Y;
			Point location = anchoredControl.Location;
			Point point = anchoredControl.Location;
			Point point1 = new Point(location.X + x, point.Y + y);
			anchoredControl.Location = point1;
			int num = anchoredControl.Location.X;
			Point location1 = anchoringControl.Location;
			int x1 = num - (location1.X + AnchoringUtils.GetContentWidth(anchoringControl));
			int y1 = anchoredControl.Location.Y;
			Point location2 = anchoringControl.Location;
			anchoredControl.RelativeOffset = new Coordinates(x1, y1 - location2.Y);
		}

		public delegate void AnchoringDelegate(IAnchoredControl anchoredControl, Control anchoringControl);
	}
}