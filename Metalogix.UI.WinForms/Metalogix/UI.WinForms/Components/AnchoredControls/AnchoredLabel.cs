using Metalogix.UI.WinForms;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Components.AnchoredControls
{
    public class AnchoredLabel : Label, IAnchoredControl, ISupportInitialize
    {
        event EventHandler IAnchoredControl.ParentChanged
        {
            add
            {
                base.ParentChanged += value;
            }
            remove
            {
                base.ParentChanged -= value;
            }
        }

        public void BeginInit()
        {
        }

        public void EndInit()
        {
            AnchoringUtils.Bind(this, this.AnchoringControl);
            AnchoringUtils.ConfigureOffsets(this, this.AnchoringControl);
        }

        public void OnAnchorContextChanged(object sender, EventArgs e)
        {
            AnchoringUtils.RecalculateOffsets(this, this.AnchoringControl);
        }

        public void OnAnchorPointChanged(object sender, EventArgs e)
        {
            AnchoringUtils.Anchor(this, this.AnchoringControl);
        }

        public void OnAnchorVisibleChanged(object sender, EventArgs e)
        {
            AnchoringUtils.AnchorVisibilityChanged(this, this.AnchoringControl);
        }

        public Control AnchoringControl { get; set; }

        public Control CommonParentControl { get; set; }

        Point IAnchoredControl.Location
        {
            get
            {
                return base.Location;
            }
            set
            {
                base.Location = value;
            }
        }

        Control IAnchoredControl.Parent
        {
            get
            {
                return base.Parent;
            }
            set
            {
                base.Parent = value;
            }
        }

        bool IAnchoredControl.Visible
        {
            get
            {
                return base.Visible;
            }
            set
            {
                base.Visible = value;
            }
        }

        public Coordinates RealOffset { get; set; }

        public Coordinates RelativeOffset { get; set; }
    }
}