using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Components
{
	public class CollapsableControl : XtraUserControl
	{
		private List<System.Windows.Forms.Control> m_hiddenControls = new List<System.Windows.Forms.Control>();

		public CollapsableControl()
		{
		}

		protected bool HideControl(System.Windows.Forms.Control control)
		{
			System.Windows.Forms.Control i;
			bool flag = false;
			for (i = control; i.Parent != null; i = i.Parent)
			{
				if (i.Parent == this)
				{
					flag = true;
				}
			}
			if (flag && !this.m_hiddenControls.Contains(control))
			{
				CollapsableComponent.HideControl(control, (ContainerControl)i);
				this.m_hiddenControls.Add(control);
			}
			return flag;
		}

		protected bool ShowControl(System.Windows.Forms.Control control, System.Windows.Forms.Control newParentControl)
		{
			return this.ShowControl(control, newParentControl, true);
		}

		protected bool ShowControl(System.Windows.Forms.Control control, System.Windows.Forms.Control newParentControl, bool bExpandParent)
		{
			bool flag = false;
			for (System.Windows.Forms.Control i = newParentControl; i != null; i = i.Parent)
			{
				if (i == this)
				{
					flag = true;
				}
			}
			if (flag)
			{
				CollapsableComponent.ShowControl(control, newParentControl, bExpandParent);
				this.m_hiddenControls.Remove(control);
			}
			return flag;
		}
	}
}