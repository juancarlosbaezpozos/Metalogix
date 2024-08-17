using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Metalogix.SharePoint.UI.WinForms;
using Metalogix.SharePoint.UI.WinForms.Administration.Navigation;

namespace Metalogix.SharePoint.UI.WinForms.Administration.Navigation
{
    public class ChangeNavigationTemplateDialog : ScopableLeftNavigableTabsForm
    {
        public delegate void NavigationPropagationDelegate();

        private bool m_bPushChangesToParents;

        private bool m_bPushChangesToSubsites;

        private IContainer components;

        protected CheckBox w_cbPushToParentSites;

        protected CheckBox w_cbPushToSubsites;

        public bool PushChangesToParents
        {
            get
            {
                return m_bPushChangesToParents;
            }
            set
            {
                m_bPushChangesToParents = value;
            }
        }

        public bool PushChangesToSubsites
        {
            get
            {
                return m_bPushChangesToSubsites;
            }
            set
            {
                m_bPushChangesToSubsites = value;
            }
        }

        public event NavigationPropagationDelegate StartPropagatingNavigationEvent;

        public event NavigationPropagationDelegate StopPropagatingNavigationEvent;

        public ChangeNavigationTemplateDialog()
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

        private void FirePropagationStartEvent()
        {
            if (this.StartPropagatingNavigationEvent != null)
            {
                this.StartPropagatingNavigationEvent();
            }
        }

        private void FirePropagationStopEvent()
        {
            if (this.StopPropagatingNavigationEvent != null)
            {
                this.StopPropagatingNavigationEvent();
            }
        }

        private void InitializeComponent()
        {
            this.w_cbPushToParentSites = new global::System.Windows.Forms.CheckBox();
            this.w_cbPushToSubsites = new global::System.Windows.Forms.CheckBox();
            base.SuspendLayout();
            base.w_btnCancel.Location = new global::System.Drawing.Point(537, 347);
            base.w_btnOK.Location = new global::System.Drawing.Point(447, 347);
            this.w_cbPushToParentSites.Anchor = global::System.Windows.Forms.AnchorStyles.Bottom | global::System.Windows.Forms.AnchorStyles.Left;
            this.w_cbPushToParentSites.AutoSize = true;
            this.w_cbPushToParentSites.FlatStyle = global::System.Windows.Forms.FlatStyle.Flat;
            this.w_cbPushToParentSites.Location = new global::System.Drawing.Point(12, 335);
            this.w_cbPushToParentSites.Name = "w_cbPushToParentSites";
            this.w_cbPushToParentSites.Size = new global::System.Drawing.Size(162, 17);
            this.w_cbPushToParentSites.TabIndex = 1;
            this.w_cbPushToParentSites.Text = "Push changes to parent sites";
            this.w_cbPushToParentSites.UseVisualStyleBackColor = true;
            this.w_cbPushToParentSites.CheckedChanged += new global::System.EventHandler(On_cbPushToParentSites_CheckedChanged);
            this.w_cbPushToSubsites.Anchor = global::System.Windows.Forms.AnchorStyles.Bottom | global::System.Windows.Forms.AnchorStyles.Left;
            this.w_cbPushToSubsites.AutoSize = true;
            this.w_cbPushToSubsites.FlatStyle = global::System.Windows.Forms.FlatStyle.Flat;
            this.w_cbPushToSubsites.Location = new global::System.Drawing.Point(12, 353);
            this.w_cbPushToSubsites.Name = "w_cbPushToSubsites";
            this.w_cbPushToSubsites.Size = new global::System.Drawing.Size(144, 17);
            this.w_cbPushToSubsites.TabIndex = 2;
            this.w_cbPushToSubsites.Text = "Push changes to subsites";
            this.w_cbPushToSubsites.UseVisualStyleBackColor = true;
            this.w_cbPushToSubsites.CheckedChanged += new global::System.EventHandler(On_cbPushToSubsites_CheckedChanged);
            base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
            base.ClientSize = new global::System.Drawing.Size(624, 382);
            base.Controls.Add(this.w_cbPushToSubsites);
            base.Controls.Add(this.w_cbPushToParentSites);
            this.MinimumSize = new global::System.Drawing.Size(640, 420);
            base.Name = "ChangeNavigationTemplateDialog";
            this.Text = "ChangeNavigationTemplateDialog";
            base.Controls.SetChildIndex(this.w_cbPushToParentSites, 0);
            base.Controls.SetChildIndex(base.w_btnOK, 0);
            base.Controls.SetChildIndex(base.w_btnCancel, 0);
            base.Controls.SetChildIndex(this.w_cbPushToSubsites, 0);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void On_cbPushToParentSites_CheckedChanged(object sender, EventArgs e)
        {
            m_bPushChangesToParents = w_cbPushToParentSites.Checked;
            if (!PushChangesToParents && !PushChangesToSubsites)
            {
                FirePropagationStopEvent();
            }
            else if (PushChangesToParents && !PushChangesToSubsites)
            {
                FirePropagationStartEvent();
            }
        }

        private void On_cbPushToSubsites_CheckedChanged(object sender, EventArgs e)
        {
            m_bPushChangesToSubsites = w_cbPushToSubsites.Checked;
            if (!PushChangesToParents && !PushChangesToSubsites)
            {
                FirePropagationStopEvent();
            }
            else if (!PushChangesToParents && PushChangesToSubsites)
            {
                FirePropagationStartEvent();
            }
        }
    }
}