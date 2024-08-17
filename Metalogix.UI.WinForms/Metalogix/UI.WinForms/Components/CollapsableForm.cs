using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Metalogix.UI.WinForms.Components
{
    public class CollapsableForm : XtraForm
    {
        protected bool HideControl(Control control)
        {
            bool flag = false;
            Control i = control;
            while (i.Parent != null)
            {
                if (i.Parent == this)
                {
                    flag = true;
                }
                i = i.Parent;
            }
            if (flag)
            {
                CollapsableComponent.HideControl(control, this);
            }
            return flag;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.Components.CollapsableForm));
            base.SuspendLayout();
            componentResourceManager.ApplyResources(this, "$this");
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "CollapsableForm";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            base.ResumeLayout(false);
        }

        protected bool ShowControl(Control control, Control newParentControl)
        {
            return ShowControl(control, newParentControl, bExpandParent: true);
        }

        protected bool ShowControl(Control control, Control newParentControl, bool bExpandParent)
        {
            CollapsableComponent.ShowControl(control, newParentControl, bExpandParent);
            return true;
        }
    }
}
