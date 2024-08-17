using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Options.Administration.CheckLinks;
using Metalogix.SharePoint.UI.WinForms;
using Metalogix.SharePoint.UI.WinForms.Administration.CheckLinks;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Administration.CheckLinks
{
    [ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Administration.GeneralOptions.png")]
    [ControlName("General Options")]
    public class TCCheckLinksOptions : ScopableTabbableControl
    {
        private SPCheckLinksOptions m_Options;

        private IContainer components;

        private CheckEdit w_cbRecursive;

        private CheckEdit w_cbWebParts;

        private CheckEdit w_cbTextFields;

        private CheckEdit w_cbShowSuccesses;

        private LabelControl w_lblResponseTimeout;

        private SpinEdit w_txtResponseTimeout;

        public SPCheckLinksOptions Options
        {
            get
            {
                return m_Options;
            }
            set
            {
                m_Options = value;
                LoadUI();
            }
        }

        public TCCheckLinksOptions()
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

        private void InitializeComponent()
        {
            global::System.ComponentModel.ComponentResourceManager resources = new global::System.ComponentModel.ComponentResourceManager(typeof(global::Metalogix.SharePoint.UI.WinForms.Administration.CheckLinks.TCCheckLinksOptions));
            global::DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject = new global::DevExpress.Utils.SerializableAppearanceObject();
            this.w_cbRecursive = new global::DevExpress.XtraEditors.CheckEdit();
            this.w_cbWebParts = new global::DevExpress.XtraEditors.CheckEdit();
            this.w_cbTextFields = new global::DevExpress.XtraEditors.CheckEdit();
            this.w_cbShowSuccesses = new global::DevExpress.XtraEditors.CheckEdit();
            this.w_lblResponseTimeout = new global::DevExpress.XtraEditors.LabelControl();
            this.w_txtResponseTimeout = new global::DevExpress.XtraEditors.SpinEdit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_cbRecursive.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_cbWebParts.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_cbTextFields.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_cbShowSuccesses.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_txtResponseTimeout.Properties).BeginInit();
            base.SuspendLayout();
            resources.ApplyResources(this.w_cbRecursive, "w_cbRecursive");
            this.w_cbRecursive.Name = "w_cbRecursive";
            this.w_cbRecursive.Properties.AutoWidth = true;
            this.w_cbRecursive.Properties.Caption = resources.GetString("w_cbRecursive.Properties.Caption");
            resources.ApplyResources(this.w_cbWebParts, "w_cbWebParts");
            this.w_cbWebParts.Name = "w_cbWebParts";
            this.w_cbWebParts.Properties.AutoWidth = true;
            this.w_cbWebParts.Properties.Caption = resources.GetString("w_cbWebParts.Properties.Caption");
            resources.ApplyResources(this.w_cbTextFields, "w_cbTextFields");
            this.w_cbTextFields.Name = "w_cbTextFields";
            this.w_cbTextFields.Properties.AutoWidth = true;
            this.w_cbTextFields.Properties.Caption = resources.GetString("w_cbTextFields.Properties.Caption");
            resources.ApplyResources(this.w_cbShowSuccesses, "w_cbShowSuccesses");
            this.w_cbShowSuccesses.Name = "w_cbShowSuccesses";
            this.w_cbShowSuccesses.Properties.AutoWidth = true;
            this.w_cbShowSuccesses.Properties.Caption = resources.GetString("w_cbShowSuccesses.Properties.Caption");
            resources.ApplyResources(this.w_lblResponseTimeout, "w_lblResponseTimeout");
            this.w_lblResponseTimeout.Name = "w_lblResponseTimeout";
            resources.ApplyResources(this.w_txtResponseTimeout, "w_txtResponseTimeout");
            this.w_txtResponseTimeout.Name = "w_txtResponseTimeout";
            global::DevExpress.XtraEditors.Controls.EditorButtonCollection buttons = this.w_txtResponseTimeout.Properties.Buttons;
            global::DevExpress.XtraEditors.Controls.EditorButton[] buttons2 = new global::DevExpress.XtraEditors.Controls.EditorButton[1]
            {
            new global::DevExpress.XtraEditors.Controls.EditorButton()
            };
            buttons.AddRange(buttons2);
            this.w_txtResponseTimeout.Properties.Mask.EditMask = resources.GetString("w_txtResponseTimeout.Properties.Mask.EditMask");
            global::DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties = this.w_txtResponseTimeout.Properties;
            int[] bits = new int[4] { 2147483647, 0, 0, 0 };
            properties.MaxValue = new decimal(bits);
            resources.ApplyResources(this, "$this");
            base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = global::System.Drawing.Color.White;
            base.Controls.Add(this.w_txtResponseTimeout);
            base.Controls.Add(this.w_lblResponseTimeout);
            base.Controls.Add(this.w_cbShowSuccesses);
            base.Controls.Add(this.w_cbTextFields);
            base.Controls.Add(this.w_cbWebParts);
            base.Controls.Add(this.w_cbRecursive);
            base.Name = "TCCheckLinksOptions";
            ((global::System.ComponentModel.ISupportInitialize)this.w_cbRecursive.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_cbWebParts.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_cbTextFields.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_cbShowSuccesses.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_txtResponseTimeout.Properties).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        protected override void LoadUI()
        {
            if (w_cbRecursive.Visible && base.Scope != SharePointObjectScope.Site && base.Scope != SharePointObjectScope.SiteCollection)
            {
                HideControl(w_cbRecursive);
            }
            w_cbRecursive.Checked = Options.CheckSubsites;
            w_cbWebParts.Checked = Options.CheckWebparts;
            w_cbTextFields.Checked = Options.CheckTextFields;
            w_cbShowSuccesses.Checked = Options.ShowSuccesses;
            w_txtResponseTimeout.Text = Options.PageResponseTimeout.ToString();
        }

        public override bool SaveUI()
        {
            Options.CheckSubsites = w_cbRecursive.Checked;
            Options.CheckWebparts = w_cbWebParts.Checked;
            Options.CheckTextFields = w_cbTextFields.Checked;
            Options.ShowSuccesses = w_cbShowSuccesses.Checked;
            Options.PageResponseTimeout = int.Parse(w_txtResponseTimeout.Text);
            return true;
        }
    }
}