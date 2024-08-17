using Metalogix.Commands.UI.WinForms.Properties;
using Metalogix.UI.WinForms.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Metalogix.Commands.UI.WinForms
{
    public partial class PowerShellTranformerDialog : LeftNavigableTabsForm
    {
        private TCScriptConfiguration w_tcBeginTransform = new TCScriptConfiguration();

        private TCScriptConfiguration w_tcTransform = new TCScriptConfiguration();

        private TCScriptConfiguration w_tcEndTransform = new TCScriptConfiguration();

        private PowerShellTransformerOptions _options;

        private IContainer components;

        public PowerShellTransformerOptions Options
        {
            get
            {
                return this._options;
            }
            set
            {
                this._options = value;
                this.LoadUI();
            }
        }

        public PowerShellTranformerDialog()
        {
            this.InitializeComponent();
            base.ThreeStateConfiguration = false;
            base.Image = ImageCache.GetImage("Metalogix.Commands.UI.WinForms.Icons.TransformerArrowPowerShell16.png", base.GetType().Assembly);
            base.Icon = System.Drawing.Icon.FromHandle(Resources.TransformerArrowPowerShell16.GetHicon());
            this.w_tcBeginTransform.ControlName = Resources.BeginTransformation;
            this.w_tcBeginTransform.ImageName = "Metalogix.Commands.UI.WinForms.Icons.TransformerArrowPowerShellUp32.png";
            this.w_tcTransform.ControlName = Resources.Transform;
            this.w_tcTransform.ImageName = "Metalogix.Commands.UI.WinForms.Icons.TransformerArrowPowerShell32.png";
            this.w_tcEndTransform.ControlName = Resources.EndTransformation;
            this.w_tcEndTransform.ImageName = "Metalogix.Commands.UI.WinForms.Icons.TransformerArrowPowerShellDown32.png";
            base.Tabs = new System.Collections.Generic.List<TabbableControl>(3)
            {
                this.w_tcBeginTransform,
                this.w_tcTransform,
                this.w_tcEndTransform
            };
        }

        private void LoadUI()
        {
            TCScriptConfiguration.ScriptConfigurationOptions options = new TCScriptConfiguration.ScriptConfigurationOptions
            {
                Location = this.Options.BeginScriptLocation,
                FileName = this.Options.BeginTranformScriptFileName,
                FullScript = this.Options.FullBeginTransformScript
            };
            this.w_tcBeginTransform.Options = options;
            TCScriptConfiguration.ScriptConfigurationOptions options2 = new TCScriptConfiguration.ScriptConfigurationOptions
            {
                Location = this.Options.TransformScriptLocation,
                FileName = this.Options.TransformScriptFileName,
                FullScript = this.Options.FullTransformScript
            };
            this.w_tcTransform.Options = options2;
            TCScriptConfiguration.ScriptConfigurationOptions options3 = new TCScriptConfiguration.ScriptConfigurationOptions
            {
                Location = this.Options.EndScriptLocation,
                FileName = this.Options.EndTransformScriptFileName,
                FullScript = this.Options.FullEndTransformScript
            };
            this.w_tcEndTransform.Options = options3;
        }

        protected override bool SaveUI()
        {
            bool flag = base.SaveUI();
            if (flag)
            {
                this.Options.BeginScriptLocation = this.w_tcBeginTransform.Options.Location;
                this.Options.BeginTranformScriptFileName = this.w_tcBeginTransform.Options.FileName;
                this.Options.FullBeginTransformScript = this.w_tcBeginTransform.Options.FullScript;
                this.Options.TransformScriptLocation = this.w_tcTransform.Options.Location;
                this.Options.TransformScriptFileName = this.w_tcTransform.Options.FileName;
                this.Options.FullTransformScript = this.w_tcTransform.Options.FullScript;
                this.Options.EndScriptLocation = this.w_tcEndTransform.Options.Location;
                this.Options.EndTransformScriptFileName = this.w_tcEndTransform.Options.FileName;
                this.Options.FullEndTransformScript = this.w_tcEndTransform.Options.FullScript;
            }
            return flag;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(PowerShellTranformerDialog));
            ((ISupportInitialize)this.tabControl).BeginInit();
            base.SuspendLayout();
            this.tabControl.LookAndFeel.SkinName = "Office 2013";
            this.tabControl.LookAndFeel.UseDefaultLookAndFeel = false;
            componentResourceManager.ApplyResources(this, "$this");
            base.AutoScaleMode = AutoScaleMode.Font;
            base.Name = "PowerShellTranformerDialog";
            ((ISupportInitialize)this.tabControl).EndInit();
            base.ResumeLayout(false);
        }
    }
}
