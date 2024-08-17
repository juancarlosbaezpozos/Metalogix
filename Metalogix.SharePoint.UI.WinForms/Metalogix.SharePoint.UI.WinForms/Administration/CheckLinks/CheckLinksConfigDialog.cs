using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Metalogix.Actions;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Options.Administration.CheckLinks;
using Metalogix.SharePoint.UI.WinForms;
using Metalogix.SharePoint.UI.WinForms.Administration.CheckLinks;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Administration.CheckLinks
{
    public class CheckLinksConfigDialog : ScopableLeftNavigableTabsForm
    {
        private TCCheckLinksOptions w_tcCheckLinksOptions = new TCCheckLinksOptions();

        private TCFlaggingOptions w_tcFlaggingOptions = new TCFlaggingOptions();

        private IContainer components;

        public CheckLinksOptions Options
        {
            get
            {
                return Action.Options as CheckLinksOptions;
            }
            set
            {
                Action.Options = value;
                LoadUI(Action);
            }
        }

        public CheckLinksConfigDialog(SharePointObjectScope dialogScope)
        {
            InitializeComponent();
            Text = "Check Links";
            List<TabbableControl> list = new List<TabbableControl> { w_tcCheckLinksOptions, w_tcFlaggingOptions };
            foreach (ScopableTabbableControl item in list)
            {
                item.Scope = dialogScope;
            }
            base.Tabs = list;
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
            global::System.ComponentModel.ComponentResourceManager resources = new global::System.ComponentModel.ComponentResourceManager(typeof(global::Metalogix.SharePoint.UI.WinForms.Administration.CheckLinks.CheckLinksConfigDialog));
            base.SuspendLayout();
            base.w_btnCancel.Location = new global::System.Drawing.Point(537, 347);
            base.w_btnOK.Location = new global::System.Drawing.Point(447, 347);
            base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
            base.ClientSize = new global::System.Drawing.Size(624, 382);
            base.Icon = (global::System.Drawing.Icon)resources.GetObject("$this.Icon");
            this.MinimumSize = new global::System.Drawing.Size(640, 420);
            base.Name = "CheckLinksConfigDialog";
            base.ShowIcon = true;
            this.Text = "CheckLinksConfigDialog";
            base.ActionTemplatesSupported = true;
            base.ResumeLayout(false);
        }

        protected override void LoadUI(Action action)
        {
            base.LoadUI(action);
            SPCheckLinksOptions sPCheckLinksOptions = new SPCheckLinksOptions();
            sPCheckLinksOptions.SetFromOptions(action.Options);
            w_tcCheckLinksOptions.Options = sPCheckLinksOptions;
            SPFlaggingOptions sPFlaggingOptions = new SPFlaggingOptions();
            sPFlaggingOptions.SetFromOptions(action.Options);
            w_tcFlaggingOptions.Options = sPFlaggingOptions;
        }

        protected override bool SaveUI(Action action)
        {
            if (base.SaveUI(action))
            {
                action.Options.SetFromOptions(w_tcCheckLinksOptions.Options);
                action.Options.SetFromOptions(w_tcFlaggingOptions.Options);
            }
            return true;
        }
    }
}