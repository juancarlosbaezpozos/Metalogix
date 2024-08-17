using System.Windows.Forms;
using Metalogix.Actions;

namespace Metalogix.UI.WinForms.Actions
{
    public partial class HasSelectableObjects : UserControl, IHasSelectableObjects
    {
        public virtual HasSelectableObjects FocusedSelectableContainer
        {
            get
            {
                if (!base.ContainsFocus)
                {
                    return null;
                }
                return RecursiveSearch(this);
            }
        }

        public virtual IXMLAbleList SelectedObjects => null;

        private void InitializeComponent()
        {
            base.SuspendLayout();
            base.Name = "HasSelectableObjects";
            base.ResumeLayout(false);
        }

        private HasSelectableObjects RecursiveSearch(Control target)
        {
            HasSelectableObjects hasSelectableObject = null;
            foreach (Control control in target.Controls)
            {
                if (!control.ContainsFocus)
                {
                    continue;
                }
                hasSelectableObject = RecursiveSearch(control);
                break;
            }
            if (!(target is HasSelectableObjects) || hasSelectableObject != null)
            {
                return hasSelectableObject;
            }
            return (HasSelectableObjects)target;
        }
    }
}
