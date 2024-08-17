using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using Metalogix;
using Metalogix.Interfaces;
using Metalogix.Licensing.Common;
using Metalogix.MLLicensing.Properties;
using Metalogix.Telemetry;
using Metalogix.UI.WinForms.Properties;
using Metalogix.Utilities;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms
{
    public partial class UIAboutForm : XtraForm
    {
        private UIApplication m_application;

        private IContainer components;

        private SimpleButton btnOK;

        private SimpleButton btnRefresh;

        private PanelControl pnlMetalogixLogo;

        private PanelControl panelLicenseInfo;

        private PanelControl pnlRefreshandCloseButtons;

        private PanelControl panelTelemetry;

        private CheckEdit checkBoxEnableTelemetry;

        private HyperLinkEdit labelLink;

        private TextEditContextMenu _textEditContextMenu;

        private LabelControl lblLicenseKey;

        private LabelControl lblDaysUntilReregistrationValue;

        private LabelControl lblLicenseUsageValue;

        private LabelControl lblMaintenanceDateValue;

        private LabelControl lblLicenseExpirationDateValue;

        private LabelControl lblLicenseKeyValue;

        private LabelControl lblMetalogixCopywrites;

        private LabelControl lblDaysUntilReregistration;

        private LabelControl lblLicenseUsage;

        private LabelControl lblMaintenanceDate;

        private LabelControl lblLicenseExpirationDate;

        private LabelControl lblMaintenanceExpirationWarning;

        private LabelControl lblLicenseUsageWarning;

        private PictureBox pbxMetalogixLogo;

        private LabelControl lblLicenseExpiredWarning;

        public UIApplication Application
        {
            get
            {
                return this.m_application;
            }
            set
            {
                this.m_application = value;
                if (this.Application != null)
                {
                    this.DisplayLicense(false);
                }
                this.checkBoxEnableTelemetry.Checked =Telemetry.Client.OptIn;
            }
        }

        public UIAboutForm()
        {
            this.InitializeComponent();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            this.DisplayLicense(true);
        }

        private void checkBoxEnableTelemetry_CheckedChanged(object sender, EventArgs e)
        {
            Telemetry.Client.OptIn = this.checkBoxEnableTelemetry.Checked;
        }

        private void DisplayLicense(bool refresh)
        {
            try
            {
                this.Text =Metalogix.MLLicensing.Properties.Resources.Metalogix_License;
                MLLicenseCommon license = this.Application.GetLicense() as MLLicenseCommon;
                if (refresh)
                {
                    license.SaveUsageData();
                    try
                    {
                        license.Provider.Refresh();
                        license = this.Application.GetLicense() as MLLicenseCommon;
                    }
                    catch (Exception exception1)
                    {
                        Exception exception = exception1;
                        string str = string.Concat("Failed to refresh the license: ", exception.Message);
                        GlobalServices.GetErrorHandlerAs<ErrorHandler>().HandleException(this, "Error", str, exception, ErrorIcon.Error);
                    }
                }
                if (license != null)
                {
                    string str1 = "dd-MMM-yyyy";
                    DateTime utcNow = DateTime.UtcNow;
                    DateTime dateTime = DateTime.ParseExact(utcNow.ToString(str1, CultureInfo.InvariantCulture), str1, CultureInfo.InvariantCulture, DateTimeStyles.None);
                    DateTime expirationDate = license.ExpirationDate;
                    DateTime dateTime1 = DateTime.ParseExact(expirationDate.ToString(str1, CultureInfo.InvariantCulture), str1, CultureInfo.InvariantCulture, DateTimeStyles.None);
                    DateTime maintenanceExpirationDate = license.MaintenanceExpirationDate;
                    DateTime dateTime2 = DateTime.ParseExact(maintenanceExpirationDate.ToString(str1, CultureInfo.InvariantCulture), str1, CultureInfo.InvariantCulture, DateTimeStyles.None);
                    this.lblLicenseKeyValue.Text = license.LicenseKey.ToString();
                    this.lblLicenseExpirationDateValue.Text = license.ExpirationDate.ToString(str1);
                    this.lblMaintenanceDateValue.Text = license.MaintenanceExpirationDate.ToString(str1);
                    this.lblLicenseUsageValue.Text = string.Concat((license.IsLegacyProduct || license.IsContentMatrixExpress ? Format.FormatSize(new long?(license.UsedDataFull)) : Format.FormatSize(new long?(license.UsedLicensedData))), " of ", (license.LicensedData.ToString() == "-1" ? "Unlimited" : Format.FormatSize(new long?(license.LicensedData))));
                    this.lblDaysUntilReregistrationValue.Text = string.Format("{0} Days", license.OfflineValidityDays);
                    int days = dateTime1.Subtract(dateTime).Days;
                    int num = dateTime2.Subtract(dateTime).Days;
                    if (days >= 0 && (double)days <= UIConfigurationVariables.DaysToShowLicenseExpiryWarning)
                    {
                        this.lblLicenseExpiredWarning.Visible = true;
                        this.lblLicenseExpiredWarning.Text = string.Format(Metalogix.MLLicensing.Properties.Resources.License_About_To_Expire, days);
                    }
                    else if (days >= 0)
                    {
                        this.lblLicenseExpiredWarning.Visible = false;
                    }
                    else
                    {
                        this.lblLicenseExpiredWarning.Visible = true;
                    }
                    if (num >= 0 && (double)num <= UIConfigurationVariables.DaysToShowLicenseExpiryWarning)
                    {
                        this.lblMaintenanceExpirationWarning.Visible = true;
                        this.lblMaintenanceExpirationWarning.Text = string.Format(Metalogix.MLLicensing.Properties.Resources.Maintenance_About_To_Expire, num).Replace(Environment.NewLine, " ");
                    }
                    else if (num >= 0)
                    {
                        this.lblMaintenanceExpirationWarning.Visible = false;
                    }
                    else
                    {
                        this.lblMaintenanceExpirationWarning.Visible = true;
                    }
                    if (license.UsagePercentage <= 80)
                    {
                        this.lblLicenseUsageWarning.Visible = false;
                    }
                    else
                    {
                        this.lblLicenseUsageWarning.Visible = true;
                        if (license.UsagePercentage >= 100)
                        {
                            this.lblLicenseUsageWarning.Text = Metalogix.MLLicensing.Properties.Resources.License_Limit_Exceeded.Replace(Environment.NewLine, " ");
                        }
                        else
                        {
                            LabelControl labelControl = this.lblLicenseUsageWarning;
                            string licenseUsageWarning = Metalogix.MLLicensing.Properties.Resources.License_Usage_Warning;
                            double usagePercentage = license.UsagePercentage;
                            labelControl.Text = string.Format(licenseUsageWarning, usagePercentage.ToString("n2")).Replace(Environment.NewLine, " ");
                        }
                    }
                }
            }
            catch (Exception exception2)
            {
            }
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
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UIAboutForm));
            this.btnOK = new SimpleButton();
            this.btnRefresh = new SimpleButton();
            this.pnlMetalogixLogo = new PanelControl();
            this.pbxMetalogixLogo = new PictureBox();
            this.panelLicenseInfo = new PanelControl();
            this.lblLicenseExpiredWarning = new LabelControl();
            this.lblLicenseUsageWarning = new LabelControl();
            this.lblMaintenanceExpirationWarning = new LabelControl();
            this.lblDaysUntilReregistrationValue = new LabelControl();
            this.lblLicenseUsageValue = new LabelControl();
            this.lblMaintenanceDateValue = new LabelControl();
            this.lblLicenseExpirationDateValue = new LabelControl();
            this.lblLicenseKeyValue = new LabelControl();
            this.lblMetalogixCopywrites = new LabelControl();
            this.lblDaysUntilReregistration = new LabelControl();
            this.lblLicenseUsage = new LabelControl();
            this.lblMaintenanceDate = new LabelControl();
            this.lblLicenseExpirationDate = new LabelControl();
            this.lblLicenseKey = new LabelControl();
            this.pnlRefreshandCloseButtons = new PanelControl();
            this.panelTelemetry = new PanelControl();
            this.labelLink = new HyperLinkEdit();
            this.checkBoxEnableTelemetry = new CheckEdit();
            this._textEditContextMenu = new TextEditContextMenu();
            ((ISupportInitialize)this.pnlMetalogixLogo).BeginInit();
            this.pnlMetalogixLogo.SuspendLayout();
            ((ISupportInitialize)this.pbxMetalogixLogo).BeginInit();
            ((ISupportInitialize)this.panelLicenseInfo).BeginInit();
            this.panelLicenseInfo.SuspendLayout();
            ((ISupportInitialize)this.pnlRefreshandCloseButtons).BeginInit();
            this.pnlRefreshandCloseButtons.SuspendLayout();
            ((ISupportInitialize)this.panelTelemetry).BeginInit();
            this.panelTelemetry.SuspendLayout();
            ((ISupportInitialize)this.labelLink.Properties).BeginInit();
            ((ISupportInitialize)this.checkBoxEnableTelemetry.Properties).BeginInit();
            base.SuspendLayout();
            componentResourceManager.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.DialogResult = DialogResult.OK;
            this.btnOK.Name = "btnOK";
            componentResourceManager.ApplyResources(this.btnRefresh, "btnRefresh");
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Click += new EventHandler(this.btnRefresh_Click);
            this.pnlMetalogixLogo.BorderStyle = BorderStyles.NoBorder;
            this.pnlMetalogixLogo.Controls.Add(this.pbxMetalogixLogo);
            componentResourceManager.ApplyResources(this.pnlMetalogixLogo, "pnlMetalogixLogo");
            this.pnlMetalogixLogo.Name = "pnlMetalogixLogo";
            this.pbxMetalogixLogo.Image =Metalogix.UI.WinForms.Properties.Resources.MetalogixImage;
            componentResourceManager.ApplyResources(this.pbxMetalogixLogo, "pbxMetalogixLogo");
            this.pbxMetalogixLogo.Name = "pbxMetalogixLogo";
            this.pbxMetalogixLogo.TabStop = false;
            this.panelLicenseInfo.BorderStyle = BorderStyles.NoBorder;
            this.panelLicenseInfo.Controls.Add(this.lblLicenseExpiredWarning);
            this.panelLicenseInfo.Controls.Add(this.lblLicenseUsageWarning);
            this.panelLicenseInfo.Controls.Add(this.lblMaintenanceExpirationWarning);
            this.panelLicenseInfo.Controls.Add(this.lblDaysUntilReregistrationValue);
            this.panelLicenseInfo.Controls.Add(this.lblLicenseUsageValue);
            this.panelLicenseInfo.Controls.Add(this.lblMaintenanceDateValue);
            this.panelLicenseInfo.Controls.Add(this.lblLicenseExpirationDateValue);
            this.panelLicenseInfo.Controls.Add(this.lblLicenseKeyValue);
            this.panelLicenseInfo.Controls.Add(this.lblMetalogixCopywrites);
            this.panelLicenseInfo.Controls.Add(this.lblDaysUntilReregistration);
            this.panelLicenseInfo.Controls.Add(this.lblLicenseUsage);
            this.panelLicenseInfo.Controls.Add(this.lblMaintenanceDate);
            this.panelLicenseInfo.Controls.Add(this.lblLicenseExpirationDate);
            this.panelLicenseInfo.Controls.Add(this.lblLicenseKey);
            componentResourceManager.ApplyResources(this.panelLicenseInfo, "panelLicenseInfo");
            this.panelLicenseInfo.Name = "panelLicenseInfo";
            this.lblLicenseExpiredWarning.Appearance.Font = (Font)componentResourceManager.GetObject("lblLicenseExpiredWarning.Appearance.Font");
            this.lblLicenseExpiredWarning.Appearance.ForeColor = (Color)componentResourceManager.GetObject("lblLicenseExpiredWarning.Appearance.ForeColor");
            componentResourceManager.ApplyResources(this.lblLicenseExpiredWarning, "lblLicenseExpiredWarning");
            this.lblLicenseExpiredWarning.Name = "lblLicenseExpiredWarning";
            this.lblLicenseUsageWarning.Appearance.Font = (Font)componentResourceManager.GetObject("lblLicenseUsageWarning.Appearance.Font");
            this.lblLicenseUsageWarning.Appearance.ForeColor = (Color)componentResourceManager.GetObject("lblLicenseUsageWarning.Appearance.ForeColor");
            componentResourceManager.ApplyResources(this.lblLicenseUsageWarning, "lblLicenseUsageWarning");
            this.lblLicenseUsageWarning.Name = "lblLicenseUsageWarning";
            this.lblMaintenanceExpirationWarning.Appearance.Font = (Font)componentResourceManager.GetObject("lblMaintenanceExpirationWarning.Appearance.Font");
            this.lblMaintenanceExpirationWarning.Appearance.ForeColor = (Color)componentResourceManager.GetObject("lblMaintenanceExpirationWarning.Appearance.ForeColor");
            componentResourceManager.ApplyResources(this.lblMaintenanceExpirationWarning, "lblMaintenanceExpirationWarning");
            this.lblMaintenanceExpirationWarning.Name = "lblMaintenanceExpirationWarning";
            componentResourceManager.ApplyResources(this.lblDaysUntilReregistrationValue, "lblDaysUntilReregistrationValue");
            this.lblDaysUntilReregistrationValue.Name = "lblDaysUntilReregistrationValue";
            componentResourceManager.ApplyResources(this.lblLicenseUsageValue, "lblLicenseUsageValue");
            this.lblLicenseUsageValue.Name = "lblLicenseUsageValue";
            componentResourceManager.ApplyResources(this.lblMaintenanceDateValue, "lblMaintenanceDateValue");
            this.lblMaintenanceDateValue.Name = "lblMaintenanceDateValue";
            componentResourceManager.ApplyResources(this.lblLicenseExpirationDateValue, "lblLicenseExpirationDateValue");
            this.lblLicenseExpirationDateValue.Name = "lblLicenseExpirationDateValue";
            componentResourceManager.ApplyResources(this.lblLicenseKeyValue, "lblLicenseKeyValue");
            this.lblLicenseKeyValue.Name = "lblLicenseKeyValue";
            componentResourceManager.ApplyResources(this.lblMetalogixCopywrites, "lblMetalogixCopywrites");
            this.lblMetalogixCopywrites.Name = "lblMetalogixCopywrites";
            this.lblDaysUntilReregistration.Appearance.Font = (Font)componentResourceManager.GetObject("lblDaysUntilReregistration.Appearance.Font");
            componentResourceManager.ApplyResources(this.lblDaysUntilReregistration, "lblDaysUntilReregistration");
            this.lblDaysUntilReregistration.Name = "lblDaysUntilReregistration";
            this.lblLicenseUsage.Appearance.Font = (Font)componentResourceManager.GetObject("lblLicenseUsage.Appearance.Font");
            componentResourceManager.ApplyResources(this.lblLicenseUsage, "lblLicenseUsage");
            this.lblLicenseUsage.Name = "lblLicenseUsage";
            this.lblMaintenanceDate.Appearance.Font = (Font)componentResourceManager.GetObject("lblMaintenanceDate.Appearance.Font");
            componentResourceManager.ApplyResources(this.lblMaintenanceDate, "lblMaintenanceDate");
            this.lblMaintenanceDate.Name = "lblMaintenanceDate";
            this.lblLicenseExpirationDate.Appearance.Font = (Font)componentResourceManager.GetObject("lblLicenseExpirationDate.Appearance.Font");
            componentResourceManager.ApplyResources(this.lblLicenseExpirationDate, "lblLicenseExpirationDate");
            this.lblLicenseExpirationDate.Name = "lblLicenseExpirationDate";
            this.lblLicenseKey.Appearance.Font = (Font)componentResourceManager.GetObject("lblLicenseKey.Appearance.Font");
            componentResourceManager.ApplyResources(this.lblLicenseKey, "lblLicenseKey");
            this.lblLicenseKey.Name = "lblLicenseKey";
            this.pnlRefreshandCloseButtons.BorderStyle = BorderStyles.NoBorder;
            this.pnlRefreshandCloseButtons.Controls.Add(this.btnRefresh);
            this.pnlRefreshandCloseButtons.Controls.Add(this.btnOK);
            componentResourceManager.ApplyResources(this.pnlRefreshandCloseButtons, "pnlRefreshandCloseButtons");
            this.pnlRefreshandCloseButtons.Name = "pnlRefreshandCloseButtons";
            this.panelTelemetry.BorderStyle = BorderStyles.NoBorder;
            this.panelTelemetry.Controls.Add(this.labelLink);
            this.panelTelemetry.Controls.Add(this.checkBoxEnableTelemetry);
            componentResourceManager.ApplyResources(this.panelTelemetry, "panelTelemetry");
            this.panelTelemetry.Name = "panelTelemetry";
            this.labelLink.Cursor = Cursors.Hand;
            componentResourceManager.ApplyResources(this.labelLink, "labelLink");
            this.labelLink.Name = "labelLink";
            this.labelLink.Properties.Appearance.Font = (Font)componentResourceManager.GetObject("labelLink.Properties.Appearance.Font");
            this.labelLink.Properties.Appearance.ForeColor = (Color)componentResourceManager.GetObject("labelLink.Properties.Appearance.ForeColor");
            this.labelLink.Properties.Appearance.Options.UseFont = true;
            this.labelLink.Properties.Appearance.Options.UseForeColor = true;
            this.labelLink.Properties.BorderStyle = BorderStyles.NoBorder;
            this.labelLink.Properties.UseParentBackground = true;
            this.labelLink.OpenLink += new OpenLinkEventHandler(this.labelLink_OpenLink);
            componentResourceManager.ApplyResources(this.checkBoxEnableTelemetry, "checkBoxEnableTelemetry");
            this.checkBoxEnableTelemetry.Name = "checkBoxEnableTelemetry";
            this.checkBoxEnableTelemetry.Properties.AutoHeight = (bool)componentResourceManager.GetObject("checkBoxEnableTelemetry.Properties.AutoHeight");
            this.checkBoxEnableTelemetry.Properties.Caption = componentResourceManager.GetString("checkBoxEnableTelemetry.Properties.Caption");
            this.checkBoxEnableTelemetry.CheckedChanged += new EventHandler(this.checkBoxEnableTelemetry_CheckedChanged);
            this._textEditContextMenu.Name = "TextEditContextMenu";
            componentResourceManager.ApplyResources(this._textEditContextMenu, "_textEditContextMenu");
            base.AcceptButton = this.btnOK;
            componentResourceManager.ApplyResources(this, "$this");
            base.CancelButton = this.btnOK;
            base.Controls.Add(this.panelLicenseInfo);
            base.Controls.Add(this.pnlMetalogixLogo);
            base.Controls.Add(this.panelTelemetry);
            base.Controls.Add(this.pnlRefreshandCloseButtons);
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "UIAboutForm";
            base.ShowInTaskbar = false;
            base.SizeGripStyle = SizeGripStyle.Show;
            base.Shown += new EventHandler(this.On_Shown);
            ((ISupportInitialize)this.pnlMetalogixLogo).EndInit();
            this.pnlMetalogixLogo.ResumeLayout(false);
            ((ISupportInitialize)this.pbxMetalogixLogo).EndInit();
            ((ISupportInitialize)this.panelLicenseInfo).EndInit();
            this.panelLicenseInfo.ResumeLayout(false);
            this.panelLicenseInfo.PerformLayout();
            ((ISupportInitialize)this.pnlRefreshandCloseButtons).EndInit();
            this.pnlRefreshandCloseButtons.ResumeLayout(false);
            ((ISupportInitialize)this.panelTelemetry).EndInit();
            this.panelTelemetry.ResumeLayout(false);
            ((ISupportInitialize)this.labelLink.Properties).EndInit();
            ((ISupportInitialize)this.checkBoxEnableTelemetry.Properties).EndInit();
            base.ResumeLayout(false);
        }

        private void labelLink_OpenLink(object sender, OpenLinkEventArgs e)
        {
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "http://www.metalogix.com/ceip";
                    process.StartInfo.UseShellExecute = true;
                    process.Start();
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                GlobalServices.ErrorHandler.HandleException("Open URL Error", string.Format("Error opening URL \"http://www.metalogix.com/ceip\" : {0}", exception.Message), exception, ErrorIcon.Warning);
            }
        }

        private void On_Shown(object sender, EventArgs e)
        {
            this.btnOK.Select();
        }
    }
}