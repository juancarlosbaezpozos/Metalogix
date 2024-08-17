using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms
{
	internal static class UIController
	{
		public static void CopyFormProperties(Form source, Form target)
		{
			Type type = source.GetType();
			Type type1 = target.GetType();
			PropertyInfo[] properties = typeof(UIController.IFormProperties).GetProperties();
			for (int i = 0; i < (int)properties.Length; i++)
			{
				PropertyInfo propertyInfo = properties[i];
				PropertyInfo property = type.GetProperty(propertyInfo.Name);
				PropertyInfo property1 = type1.GetProperty(propertyInfo.Name);
				object value = property.GetValue(source, null);
				if (!(value is Control.ControlCollection))
				{
					property1.SetValue(target, value, null);
				}
				else
				{
					Stack<Control> controls = new Stack<Control>();
					foreach (Control control in (Control.ControlCollection)value)
					{
						controls.Push(control);
					}
					while (controls.Count != 0)
					{
						Control control1 = controls.Pop();
						target.Controls.Add(control1);
					}
				}
			}
		}

		public static void FireFormEvent(Form source, EventArgs e)
		{
			StackFrame frame = (new StackTrace()).GetFrame(1);
			UIController.FireFormEvent(source, e, frame.GetMethod().Name);
		}

		public static void FireFormEvent(Form source, EventArgs e, string methodName)
		{
			object[] objArray;
			MethodInfo method = source.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
			Form form = source;
			if (e == null)
			{
				objArray = null;
			}
			else
			{
				objArray = new object[] { e };
			}
			method.Invoke(form, objArray);
		}

		private interface IFormProperties
		{
			IButtonControl AcceptButton
			{
				get;
				set;
			}

			bool AllowDrop
			{
				get;
				set;
			}

			AutoScaleMode AutoScaleMode
			{
				get;
				set;
			}

			bool AutoScroll
			{
				get;
				set;
			}

			bool AutoSize
			{
				get;
				set;
			}

			IButtonControl CancelButton
			{
				get;
				set;
			}

			ContextMenuStrip ContextMenuStrip
			{
				get;
				set;
			}

			Control.ControlCollection Controls
			{
				get;
			}

			bool Enabled
			{
				get;
				set;
			}

			FormBorderStyle FormBorderStyle
			{
				get;
				set;
			}

			bool HelpButton
			{
				get;
				set;
			}

			Icon Icon
			{
				get;
				set;
			}

			Point Location
			{
				get;
				set;
			}

			bool MaximizeBox
			{
				get;
				set;
			}

			Size MaximumSize
			{
				get;
				set;
			}

			bool MinimizeBox
			{
				get;
				set;
			}

			Size MinimumSize
			{
				get;
				set;
			}

			string Name
			{
				get;
				set;
			}

			double Opacity
			{
				get;
				set;
			}

			Padding Padding
			{
				get;
				set;
			}

			bool ShowIcon
			{
				get;
				set;
			}

			bool ShowInTaskbar
			{
				get;
				set;
			}

			Size Size
			{
				get;
				set;
			}

			SizeGripStyle SizeGripStyle
			{
				get;
				set;
			}

			FormStartPosition StartPosition
			{
				get;
				set;
			}

			string Text
			{
				get;
				set;
			}

			bool TopMost
			{
				get;
				set;
			}

			bool UseWaitCursor
			{
				get;
				set;
			}

			bool Visible
			{
				get;
				set;
			}

			FormWindowState WindowState
			{
				get;
				set;
			}
		}
	}
}