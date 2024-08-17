using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.UI.WinForms.Properties;
using TooltipsTest;

namespace Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls
{
    public class TCAzureOptionsBasicView : ScopableTabbableControl
    {
        private bool IsAzureUserMappingWarningMessageRepeating;

        private IContainer components;

        private ToggleSwitch tsEncryptAzureOption;

        private ToggleSwitch tsAzureOption;

        private HelpTipButton helpAzureOption;

        private HelpTipButton helpEncryptAzureOption;

        public TCAzureOptionsBasicView()
        {
            InitializeComponent();
            InitializeFormControls();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        public ArrayList GetAzureOptionsSummaryDetails()
        {
            ArrayList arrayList = new ArrayList();
            if (!SPUIUtils.IsMigrationPipelineAllowed(base.Scope, TargetNodes))
            {
                return arrayList;
            }
            arrayList.Add(new OptionsSummary($"{tsAzureOption.Properties.OnText} : {tsAzureOption.IsOn}", 2));
            if (tsEncryptAzureOption.Enabled)
            {
                arrayList.Add(new OptionsSummary($"{tsEncryptAzureOption.Properties.OnText} : {tsEncryptAzureOption.IsOn}", 3));
            }
            return arrayList;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.BasicView.UserControls.TCAzureOptionsBasicView));
            this.tsEncryptAzureOption = new DevExpress.XtraEditors.ToggleSwitch();
            this.tsAzureOption = new DevExpress.XtraEditors.ToggleSwitch();
            this.helpAzureOption = new TooltipsTest.HelpTipButton();
            this.helpEncryptAzureOption = new TooltipsTest.HelpTipButton();
            ((System.ComponentModel.ISupportInitialize)this.tsEncryptAzureOption.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.tsAzureOption.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helpAzureOption).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.helpEncryptAzureOption).BeginInit();
            base.SuspendLayout();
            this.tsEncryptAzureOption.Enabled = false;
            this.tsEncryptAzureOption.Location = new System.Drawing.Point(72, 33);
            this.tsEncryptAzureOption.Name = "tsEncryptAzureOption";
            this.tsEncryptAzureOption.Properties.Appearance.Options.UseTextOptions = true;
            this.tsEncryptAzureOption.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsEncryptAzureOption.Properties.OffText = "Encrypt Content in Azure Container";
            this.tsEncryptAzureOption.Properties.OnText = "Encrypt Content in Azure Container";
            this.tsEncryptAzureOption.Size = new System.Drawing.Size(257, 24);
            this.tsEncryptAzureOption.TabIndex = 1;
            this.tsAzureOption.Location = new System.Drawing.Point(7, 1);
            this.tsAzureOption.Name = "tsAzureOption";
            this.tsAzureOption.Properties.Appearance.Options.UseTextOptions = true;
            this.tsAzureOption.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tsAzureOption.Properties.OffText = "Use SharePoint Online Migration API";
            this.tsAzureOption.Properties.OnText = "Use SharePoint Online Migration API";
            this.tsAzureOption.Size = new System.Drawing.Size(259, 24);
            this.tsAzureOption.TabIndex = 0;
            this.tsAzureOption.Toggled += new System.EventHandler(tsAzureOption_Toggled);
            this.helpAzureOption.AnchoringControl = null;
            this.helpAzureOption.BackColor = System.Drawing.Color.Transparent;
            this.helpAzureOption.CommonParentControl = null;
            this.helpAzureOption.Image = (System.Drawing.Image)resources.GetObject("helpAzureOption.Image");
            this.helpAzureOption.Location = new System.Drawing.Point(265, 3);
            this.helpAzureOption.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpAzureOption.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpAzureOption.Name = "helpAzureOption";
            this.helpAzureOption.RealOffset = null;
            this.helpAzureOption.RelativeOffset = null;
            this.helpAzureOption.Size = new System.Drawing.Size(20, 20);
            this.helpAzureOption.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpAzureOption.TabIndex = 11;
            this.helpAzureOption.TabStop = false;
            this.helpEncryptAzureOption.AnchoringControl = null;
            this.helpEncryptAzureOption.BackColor = System.Drawing.Color.Transparent;
            this.helpEncryptAzureOption.CommonParentControl = null;
            this.helpEncryptAzureOption.Image = (System.Drawing.Image)resources.GetObject("helpEncryptAzureOption.Image");
            this.helpEncryptAzureOption.Location = new System.Drawing.Point(328, 34);
            this.helpEncryptAzureOption.MaximumSize = new System.Drawing.Size(20, 20);
            this.helpEncryptAzureOption.MinimumSize = new System.Drawing.Size(20, 20);
            this.helpEncryptAzureOption.Name = "helpEncryptAzureOption";
            this.helpEncryptAzureOption.RealOffset = null;
            this.helpEncryptAzureOption.RelativeOffset = null;
            this.helpEncryptAzureOption.Size = new System.Drawing.Size(20, 20);
            this.helpEncryptAzureOption.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.helpEncryptAzureOption.TabIndex = 12;
            this.helpEncryptAzureOption.TabStop = false;
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(this.helpEncryptAzureOption);
            base.Controls.Add(this.helpAzureOption);
            base.Controls.Add(this.tsEncryptAzureOption);
            base.Controls.Add(this.tsAzureOption);
            base.Name = "TCAzureOptionsBasicView";
            base.Size = new System.Drawing.Size(350, 57);
            this.UseTab = true;
            ((System.ComponentModel.ISupportInitialize)this.tsEncryptAzureOption.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.tsAzureOption.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helpAzureOption).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.helpEncryptAzureOption).EndInit();
            base.ResumeLayout(false);
        }

        private void InitializeFormControls()
        {
            Type type = GetType();
            helpAzureOption.SetResourceString(type.FullName + tsAzureOption.Name, type);
            helpAzureOption.IsBasicViewHelpIcon = true;
            helpEncryptAzureOption.SetResourceString(type.FullName + tsEncryptAzureOption.Name, type);
            helpEncryptAzureOption.IsBasicViewHelpIcon = true;
        }

        public void LoadFolderOptions(SPFolderContentOptions folderOptions)
        {
            tsAzureOption.IsOn = SPUIUtils.IsMigrationPipelineAllowed(base.Scope, TargetNodes) && folderOptions.UseAzureOffice365Upload;
            if (tsAzureOption.IsOn)
            {
                tsEncryptAzureOption.Enabled = true;
                tsEncryptAzureOption.IsOn = folderOptions.EncryptAzureMigrationJobs;
            }
        }

        public void LoadListContentOptions(SPListContentOptions listContentOptions)
        {
            tsAzureOption.IsOn = SPUIUtils.IsMigrationPipelineAllowed(base.Scope, TargetNodes) && listContentOptions.UseAzureOffice365Upload;
            if (tsAzureOption.IsOn)
            {
                tsEncryptAzureOption.Enabled = true;
                tsEncryptAzureOption.IsOn = listContentOptions.EncryptAzureMigrationJobs;
            }
        }

        public bool SaveFolderOptions(SPFolderContentOptions folderOptions)
        {
            folderOptions.UseAzureOffice365Upload = tsAzureOption.IsOn;
            folderOptions.EncryptAzureMigrationJobs = tsEncryptAzureOption.IsOn;
            bool flag = false;
            if (!base.IsModeSwitched && folderOptions.UseAzureOffice365Upload && SourceNodes != null && SourceNodes.Count > 0 && TargetNodes != null && TargetNodes.Count > 0)
            {
                flag = SPUIUtils.IsAzureConnectionStringEmpty((SPNode)SourceNodes[0], (SPNode)TargetNodes[0], folderOptions.EncryptAzureMigrationJobs, ref IsAzureUserMappingWarningMessageRepeating);
            }
            return !flag;
        }

        public bool SaveListContentOptions(SPListContentOptions listContentOptions)
        {
            listContentOptions.UseAzureOffice365Upload = tsAzureOption.IsOn;
            listContentOptions.EncryptAzureMigrationJobs = tsEncryptAzureOption.IsOn;
            bool flag = false;
            if (!base.IsModeSwitched && listContentOptions.UseAzureOffice365Upload && SourceNodes != null && SourceNodes.Count > 0 && TargetNodes != null && TargetNodes.Count > 0)
            {
                flag = SPUIUtils.IsAzureConnectionStringEmpty((SPNode)SourceNodes[0], (SPNode)TargetNodes[0], listContentOptions.EncryptAzureMigrationJobs, ref IsAzureUserMappingWarningMessageRepeating);
            }
            return !flag;
        }

        private void tsAzureOption_Toggled(object sender, EventArgs e)
        {
            bool isOn = tsAzureOption.IsOn;
            if (!base.CanFocus || !isOn)
            {
                tsEncryptAzureOption.IsOn = false;
                tsEncryptAzureOption.Enabled = false;
            }
            else if (MessageBox.Show(Resources.AzureUploadWarning, Resources.AzureUploadWarningCaption, MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                tsEncryptAzureOption.Enabled = true;
            }
            else
            {
                tsAzureOption.IsOn = false;
            }
        }

        public void UpdateEnabledState(bool copyItems)
        {
            if (tsAzureOption.IsOn)
            {
                tsEncryptAzureOption.Enabled = true;
            }
        }
    }
}
