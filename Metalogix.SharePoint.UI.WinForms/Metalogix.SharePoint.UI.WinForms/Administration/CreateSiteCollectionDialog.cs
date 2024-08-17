using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Options.Administration;
using Metalogix.SharePoint.UI.WinForms.Administration;
using TooltipsTest;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
    public class CreateSiteCollectionDialog : XtraForm
    {
        private IContainer components;

        private CreateSiteCollectionControl w_createSiteControl;

        private SimpleButton w_btnCancel;

        private SimpleButton w_btnOK;

        private HelpTipButton helpLinkHostHeader;

        public CreateSiteCollectionOptions Options
        {
            get
            {
                return w_createSiteControl.Options;
            }
            set
            {
                w_createSiteControl.Options = value;
            }
        }

        public SPBaseServer Target
        {
            get
            {
                return w_createSiteControl.Target;
            }
            set
            {
                w_createSiteControl.Target = value;
            }
        }

        public CreateSiteCollectionDialog()
        {
            InitializeComponent();
            Type type = GetType();
            helpLinkHostHeader.SetResourceString(type.FullName + helpLinkHostHeader.Name, type);
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
            global::System.ComponentModel.ComponentResourceManager resources = new global::System.ComponentModel.ComponentResourceManager(typeof(global::Metalogix.SharePoint.UI.WinForms.Administration.CreateSiteCollectionDialog));
            this.w_btnCancel = new global::DevExpress.XtraEditors.SimpleButton();
            this.w_btnOK = new global::DevExpress.XtraEditors.SimpleButton();
            this.w_createSiteControl = new global::Metalogix.SharePoint.UI.WinForms.Administration.CreateSiteCollectionControl();
            this.helpLinkHostHeader = new global::TooltipsTest.HelpTipButton();
            ((global::System.ComponentModel.ISupportInitialize)this.helpLinkHostHeader).BeginInit();
            base.SuspendLayout();
            resources.ApplyResources(this.w_btnCancel, "w_btnCancel");
            this.w_btnCancel.DialogResult = global::System.Windows.Forms.DialogResult.Cancel;
            this.w_btnCancel.Name = "w_btnCancel";
            resources.ApplyResources(this.w_btnOK, "w_btnOK");
            this.w_btnOK.DialogResult = global::System.Windows.Forms.DialogResult.OK;
            this.w_btnOK.Name = "w_btnOK";
            this.w_btnOK.Click += new global::System.EventHandler(On_btnOK_Click);
            resources.ApplyResources(this.w_createSiteControl, "w_createSiteControl");
            this.w_createSiteControl.Name = "w_createSiteControl";
            this.w_createSiteControl.Options = null;
            this.w_createSiteControl.Target = null;
            this.helpLinkHostHeader.AnchoringControl = null;
            this.helpLinkHostHeader.BackColor = global::System.Drawing.Color.Transparent;
            this.helpLinkHostHeader.CommonParentControl = null;
            resources.ApplyResources(this.helpLinkHostHeader, "helpLinkHostHeader");
            this.helpLinkHostHeader.Name = "helpLinkHostHeader";
            this.helpLinkHostHeader.RealOffset = null;
            this.helpLinkHostHeader.RelativeOffset = null;
            this.helpLinkHostHeader.TabStop = false;
            base.AcceptButton = this.w_btnOK;
            resources.ApplyResources(this, "$this");
            base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = this.w_btnCancel;
            base.Controls.Add(this.helpLinkHostHeader);
            base.Controls.Add(this.w_createSiteControl);
            base.Controls.Add(this.w_btnCancel);
            base.Controls.Add(this.w_btnOK);
            base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "CreateSiteCollectionDialog";
            base.ShowInTaskbar = false;
            ((global::System.ComponentModel.ISupportInitialize)this.helpLinkHostHeader).EndInit();
            base.ResumeLayout(false);
        }

        private void On_btnOK_Click(object sender, EventArgs e)
        {
            if (!w_createSiteControl.ValidateEntries())
            {
                base.DialogResult = DialogResult.None;
            }
            else if (w_createSiteControl.SaveUI())
            {
                base.DialogResult = DialogResult.OK;
            }
            else
            {
                base.DialogResult = DialogResult.None;
            }
        }
    }
}