using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Metalogix.SharePoint.UI.WinForms.Properties;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Properties;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
    public class LimitSiteCollectionsDialog : CollapsableForm
    {
        private bool _isLimitedConnection;

        private List<string> _limitedSiteCollections;

        private string _connectionUrl;

        private string _connectionScope;

        private IContainer components;

        private SimpleButton btnNo;

        private SimpleButton btnYes;

        private PictureEdit w_warning;

        private Label lblMessage;

        private CheckBox chkTenantMySiteHost;

        private LinkLabel lnkSampleXml;

        private PanelControl gbMain;

        private SimpleButton btnContinue;

        private PanelControl gbUrlGrid;

        private GridControl grdCtrlUrlList;

        private GridView grdViewUrlList;

        private SimpleButton btnCancel;

        public string ConnectionScope
        {
            get
            {
                return _connectionScope;
            }
            set
            {
                _connectionScope = value;
            }
        }

        public string ConnectionUrl
        {
            get
            {
                return _connectionUrl;
            }
            set
            {
                _connectionUrl = value;
            }
        }

        public bool IncludeTenantMySiteHostConnection => chkTenantMySiteHost.Checked;

        public bool IsLimitedConnection
        {
            get
            {
                return _isLimitedConnection;
            }
            set
            {
                _isLimitedConnection = value;
            }
        }

        public List<string> LimitedSiteCollections
        {
            get
            {
                return _limitedSiteCollections;
            }
            set
            {
                _limitedSiteCollections = value;
            }
        }

        public LimitSiteCollectionsDialog()
        {
            InitializeComponent();
            HideControl(gbUrlGrid);
        }

        public LimitSiteCollectionsDialog(bool isTenantConnection)
        {
            InitializeComponent();
            if (!isTenantConnection)
            {
                HideControl(chkTenantMySiteHost);
            }
            HideControl(gbUrlGrid);
            gbMain.Location = new Point(0, 10);
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select xml file to import",
                Filter = "Xml files (*.xml)|*.xml",
                InitialDirectory = ApplicationData.ApplicationPath
            };
            openFileDialog.FileOk += delegate(object s, CancelEventArgs ev)
            {
                string validationMessage = string.Empty;
                Dictionary<string, string> siteCollection = new Dictionary<string, string>();
                LimitedSiteCollections = SPUtils.GetSiteCollectionsFromXml(File.ReadAllText(openFileDialog.FileName), out validationMessage, out siteCollection, ConnectionUrl, ConnectionScope);
                if (string.IsNullOrEmpty(validationMessage))
                {
                    showGrid(siteCollection);
                }
                else
                {
                    MessageBox.Show(validationMessage, "Invalid Site Collections");
                    ev.Cancel = true;
                }
            };
            openFileDialog.ShowDialog();
        }

        private DataTable ConvertListToDataTable(Dictionary<string, string> urlDictionary)
        {
            Image item_Status_Completed = Metalogix.UI.WinForms.Properties.Resources.Item_Status_Completed;
            Image item_Status_Failed = Metalogix.UI.WinForms.Properties.Resources.Item_Status_Failed;
            DataTable dataTable = new DataTable();
            DataColumn column = new DataColumn("urlStatus", typeof(Image))
            {
                Caption = string.Empty
            };
            DataColumn column2 = new DataColumn("Url", typeof(string));
            DataColumn column3 = new DataColumn("Message", typeof(string));
            dataTable.Columns.Add(column);
            dataTable.Columns.Add(column2);
            dataTable.Columns.Add(column3);
            foreach (KeyValuePair<string, string> item in urlDictionary)
            {
                DataRow dataRow = dataTable.NewRow();
                if (!item.Value.Equals(string.Empty))
                {
                    dataRow[0] = item_Status_Failed;
                }
                else
                {
                    dataRow[0] = item_Status_Completed;
                }
                dataRow[1] = item.Key;
                dataRow[2] = item.Value;
                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Administration.LimitSiteCollectionsDialog));
            this.lnkSampleXml = new System.Windows.Forms.LinkLabel();
            this.chkTenantMySiteHost = new System.Windows.Forms.CheckBox();
            this.btnNo = new DevExpress.XtraEditors.SimpleButton();
            this.btnYes = new DevExpress.XtraEditors.SimpleButton();
            this.w_warning = new DevExpress.XtraEditors.PictureEdit();
            this.lblMessage = new System.Windows.Forms.Label();
            this.gbMain = new DevExpress.XtraEditors.PanelControl();
            this.btnContinue = new DevExpress.XtraEditors.SimpleButton();
            this.gbUrlGrid = new DevExpress.XtraEditors.PanelControl();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.grdCtrlUrlList = new DevExpress.XtraGrid.GridControl();
            this.grdViewUrlList = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)this.w_warning.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.gbMain).BeginInit();
            this.gbMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.gbUrlGrid).BeginInit();
            this.gbUrlGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.grdCtrlUrlList).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.grdViewUrlList).BeginInit();
            base.SuspendLayout();
            this.lnkSampleXml.AutoSize = true;
            this.lnkSampleXml.Location = new System.Drawing.Point(53, 92);
            this.lnkSampleXml.Name = "lnkSampleXml";
            this.lnkSampleXml.Size = new System.Drawing.Size(85, 13);
            this.lnkSampleXml.TabIndex = 6;
            this.lnkSampleXml.TabStop = true;
            this.lnkSampleXml.Text = "View Sample Xml";
            this.lnkSampleXml.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(lnkSampleXml_LinkClicked);
            this.chkTenantMySiteHost.AutoSize = true;
            this.chkTenantMySiteHost.Checked = true;
            this.chkTenantMySiteHost.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTenantMySiteHost.Location = new System.Drawing.Point(56, 33);
            this.chkTenantMySiteHost.Name = "chkTenantMySiteHost";
            this.chkTenantMySiteHost.Size = new System.Drawing.Size(265, 17);
            this.chkTenantMySiteHost.TabIndex = 3;
            this.chkTenantMySiteHost.Text = "Include Office 365 tenant my site host connection";
            this.chkTenantMySiteHost.UseVisualStyleBackColor = true;
            this.btnNo.DialogResult = System.Windows.Forms.DialogResult.No;
            this.btnNo.Location = new System.Drawing.Point(229, 57);
            this.btnNo.Name = "btnNo";
            this.btnNo.Size = new System.Drawing.Size(165, 23);
            this.btnNo.TabIndex = 5;
            this.btnNo.Text = "No - Continue with Connection";
            this.btnYes.Location = new System.Drawing.Point(56, 57);
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new System.Drawing.Size(165, 23);
            this.btnYes.TabIndex = 4;
            this.btnYes.Text = "Yes - Import from XML";
            this.btnYes.Click += new System.EventHandler(btnYes_Click);
            this.w_warning.EditValue = Metalogix.SharePoint.UI.WinForms.Properties.Resources.Question32;
            this.w_warning.Location = new System.Drawing.Point(6, 14);
            this.w_warning.Name = "w_warning";
            this.w_warning.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.w_warning.Properties.ErrorImage = (System.Drawing.Image)resources.GetObject("w_warning.Properties.ErrorImage");
            this.w_warning.Properties.InitialImage = (System.Drawing.Image)resources.GetObject("w_warning.Properties.InitialImage");
            this.w_warning.Properties.ShowMenu = false;
            this.w_warning.Size = new System.Drawing.Size(36, 36);
            this.w_warning.TabIndex = 1;
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(52, 13);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(331, 13);
            this.lblMessage.TabIndex = 2;
            this.lblMessage.Text = "Would you like to limit the Site Collections shown in this Connection?";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.gbMain.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.gbMain.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.gbMain.Appearance.Options.UseBackColor = true;
            this.gbMain.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.gbMain.Controls.Add(this.w_warning);
            this.gbMain.Controls.Add(this.lblMessage);
            this.gbMain.Controls.Add(this.lnkSampleXml);
            this.gbMain.Controls.Add(this.btnYes);
            this.gbMain.Controls.Add(this.chkTenantMySiteHost);
            this.gbMain.Controls.Add(this.btnNo);
            this.gbMain.Location = new System.Drawing.Point(8, -2);
            this.gbMain.Name = "gbMain";
            this.gbMain.Padding = new System.Windows.Forms.Padding(3);
            this.gbMain.Size = new System.Drawing.Size(417, 114);
            this.gbMain.TabIndex = 7;
            this.btnContinue.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.btnContinue.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.btnContinue.Location = new System.Drawing.Point(87, 185);
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new System.Drawing.Size(167, 23);
            this.btnContinue.TabIndex = 8;
            this.btnContinue.Text = "Connect valid site collections";
            this.gbUrlGrid.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.gbUrlGrid.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.gbUrlGrid.Appearance.Options.UseBackColor = true;
            this.gbUrlGrid.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.gbUrlGrid.Controls.Add(this.btnCancel);
            this.gbUrlGrid.Controls.Add(this.grdCtrlUrlList);
            this.gbUrlGrid.Controls.Add(this.btnContinue);
            this.gbUrlGrid.Location = new System.Drawing.Point(0, 118);
            this.gbUrlGrid.Name = "gbUrlGrid";
            this.gbUrlGrid.Padding = new System.Windows.Forms.Padding(3);
            this.gbUrlGrid.Size = new System.Drawing.Size(431, 218);
            this.gbUrlGrid.TabIndex = 9;
            this.gbUrlGrid.Visible = false;
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.btnCancel.Location = new System.Drawing.Point(267, 185);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(63, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.grdCtrlUrlList.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.grdCtrlUrlList.Location = new System.Drawing.Point(8, 6);
            this.grdCtrlUrlList.MainView = this.grdViewUrlList;
            this.grdCtrlUrlList.Name = "grdCtrlUrlList";
            this.grdCtrlUrlList.Size = new System.Drawing.Size(417, 169);
            this.grdCtrlUrlList.TabIndex = 9;
            this.grdCtrlUrlList.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.grdViewUrlList });
            this.grdViewUrlList.GridControl = this.grdCtrlUrlList;
            this.grdViewUrlList.Name = "grdViewUrlList";
            this.grdViewUrlList.OptionsBehavior.Editable = false;
            this.grdViewUrlList.OptionsCustomization.AllowColumnMoving = false;
            this.grdViewUrlList.OptionsCustomization.AllowFilter = false;
            this.grdViewUrlList.OptionsCustomization.AllowGroup = false;
            this.grdViewUrlList.OptionsMenu.EnableColumnMenu = false;
            this.grdViewUrlList.OptionsView.ColumnAutoWidth = false;
            this.grdViewUrlList.OptionsView.ShowGroupPanel = false;
            this.grdViewUrlList.OptionsView.ShowIndicator = false;
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.ClientSize = new System.Drawing.Size(431, 342);
            base.Controls.Add(this.gbUrlGrid);
            base.Controls.Add(this.gbMain);
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "LimitSiteCollectionsDialog";
            base.ShowIcon = false;
            base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Limit Site Collections";
            ((System.ComponentModel.ISupportInitialize)this.w_warning.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.gbMain).EndInit();
            this.gbMain.ResumeLayout(false);
            this.gbMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.gbUrlGrid).EndInit();
            this.gbUrlGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.grdCtrlUrlList).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.grdViewUrlList).EndInit();
            base.ResumeLayout(false);
        }

        private void LimitSiteCollectionsDialog_Validated(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void lnkSampleXml_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<SiteCollections>");
            for (int i = 0; i < 5; i++)
            {
                stringBuilder.AppendLine("\t<Url>http://FarmName:PortNumber/ManagePath/SiteName</Url>");
            }
            stringBuilder.AppendLine("</SiteCollections>");
            string text = string.Format("{0}{1}", ApplicationData.CommonApplicationDataPath, "\\LimitSiteCollectionSampleXML.txt");
            if (!File.Exists(text))
            {
                File.WriteAllText(text, stringBuilder.ToString());
            }
            Process.Start(text);
        }

        public new DialogResult Show()
        {
            base.Validating += LimitSiteCollectionsDialog_Validated;
            DialogResult dialogResult = ShowDialog();
            if (dialogResult != DialogResult.Yes)
            {
                IsLimitedConnection = false;
            }
            else
            {
                IsLimitedConnection = true;
            }
            return dialogResult;
        }

        private void showGrid(Dictionary<string, string> limitedSiteCollectionDictionary)
        {
            DataTable dataTable = null;
            dataTable = ConvertListToDataTable(limitedSiteCollectionDictionary);
            grdViewUrlList.GridControl.DataSource = dataTable;
            grdViewUrlList.Columns[0].MaxWidth = 25;
            grdViewUrlList.Columns[0].ColumnEdit = new RepositoryItemPictureEdit();
            grdViewUrlList.Columns[0].Caption = string.Empty;
            grdViewUrlList.Columns[1].Width = 300;
            grdViewUrlList.Columns[2].Width = 100;
            SuspendLayout();
            base.Size = new Size(500, 350);
            gbUrlGrid.Location = new Point(10, 10);
            gbUrlGrid.Size = new Size(460, 300);
            ShowControl(gbUrlGrid, this);
            HideControl(gbMain);
            CenterToScreen();
            ResumeLayout();
            if (LimitedSiteCollections.Count < 1)
            {
                btnContinue.Enabled = false;
            }
        }
    }
}
