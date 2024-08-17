using DevExpress.Utils;
using DevExpress.XtraEditors;
using Metalogix.DataStructures;
using Metalogix.UI.WinForms.Components.AnchoredControls;
using Metalogix.UI.WinForms.Properties;
using Metalogix.UI.WinForms.Widgets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TooltipsTest;

namespace Metalogix.UI.WinForms.Components
{
    [ControlImage("Metalogix.UI.WinForms.Icons.IncludedCertificates.png")]
    [ControlName("Included Certificates")]
    [UsesGroupBox(true)]
    public partial class TCCertificateInclusionConfig : TabbableControl
    {
        private const string FIELD_FRIENDLY_NAME = "FriendlyName";

        private const string FIELD_ISSUED_TO = "IssuedTo";

        private const string FIELD_ISSUED_BY = "IssuedBy";

        private const string FIELD_EXPIRATION_DATE = "ExpirationDate";

        private const string FIELD_SOURCE_FILE = "SourceFile";

        private BindingList<X509CertificateWrapper> _dataSource = new BindingList<X509CertificateWrapper>();

        private ICertificateInclusionOptionsContainer m_options;

        private IContainer components;

        private SimpleButton w_bAddFromStore;

        private SimpleButton w_bAddFromFile;

        private SimpleButton w_bRemove;

        private HelpTipButton w_helpCertificateInclusionConfig;

        private SimplifiedGridView _gridControl;

        public IEnumerable<X509CertificateWrapper> Certificates
        {
            get
            {
                return this._dataSource.ToArray<X509CertificateWrapper>();
            }
            set
            {
                this.PopulateItemsView(value);
            }
        }

        public ICertificateInclusionOptionsContainer Options
        {
            get
            {
                return this.m_options;
            }
            set
            {
                this.m_options = value;
                this.LoadUI();
            }
        }

        public TCCertificateInclusionConfig()
        {
            this.InitializeComponent();
            this.InitializeColumnHeaders();
        }

        private void AddCertificateToItemsView(X509CertificateWrapper cert)
        {
            if (cert == null)
            {
                return;
            }
            foreach (X509CertificateWrapper x509CertificateWrapper in this._dataSource)
            {
                if (!x509CertificateWrapper.Equals(cert))
                {
                    continue;
                }
                return;
            }
            this._dataSource.Add(cert);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeColumnHeaders()
        {
            this._gridControl.CreateColumn(Resources.Header_FriendlyName, "FriendlyName", 150);
            this._gridControl.CreateColumn(Resources.Header_Issued_To, "IssuedTo", 120);
            this._gridControl.CreateColumn(Resources.Header_Issued_By, "IssuedBy", 120);
            this._gridControl.CreateColumn(Resources.Header_Expiration_Date, "ExpirationDate", 120);
            this._gridControl.CreateColumn(Resources.Header_Source_File, "SourceFile", 200);
            Type type = base.GetType();
            this.w_helpCertificateInclusionConfig.SetResourceString(type.FullName, type);
            this._gridControl.RowBuilderMethod = new SimplifiedGridView.UpdateDataRowDelegate(this.UpdateDataRow);
            this._gridControl.DataSource = this._dataSource;
        }

        private void InitializeComponent()
        {
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(TCCertificateInclusionConfig));
            this.w_bAddFromStore = new SimpleButton();
            this.w_bAddFromFile = new SimpleButton();
            this.w_bRemove = new SimpleButton();
            this.w_helpCertificateInclusionConfig = new HelpTipButton();
            this._gridControl = new SimplifiedGridView();
            ((ISupportInitialize)this.w_helpCertificateInclusionConfig).BeginInit();
            base.SuspendLayout();
            componentResourceManager.ApplyResources(this.w_bAddFromStore, "w_bAddFromStore");
            this.w_bAddFromStore.Appearance.Options.UseTextOptions = true;
            this.w_bAddFromStore.Appearance.TextOptions.WordWrap = WordWrap.Wrap;
            this.w_bAddFromStore.Name = "w_bAddFromStore";
            this.w_bAddFromStore.Click += new EventHandler(this.On_AddFromStore_Clicked);
            componentResourceManager.ApplyResources(this.w_bAddFromFile, "w_bAddFromFile");
            this.w_bAddFromFile.Appearance.Options.UseTextOptions = true;
            this.w_bAddFromFile.Appearance.TextOptions.WordWrap = WordWrap.Wrap;
            this.w_bAddFromFile.Name = "w_bAddFromFile";
            this.w_bAddFromFile.Click += new EventHandler(this.On_AddFromFile_Clicked);
            componentResourceManager.ApplyResources(this.w_bRemove, "w_bRemove");
            this.w_bRemove.Name = "w_bRemove";
            this.w_bRemove.Click += new EventHandler(this.On_Remove_Clicked);
            componentResourceManager.ApplyResources(this.w_helpCertificateInclusionConfig, "w_helpCertificateInclusionConfig");
            this.w_helpCertificateInclusionConfig.AnchoringControl = null;
            this.w_helpCertificateInclusionConfig.BackColor = Color.Transparent;
            this.w_helpCertificateInclusionConfig.CommonParentControl = null;
            this.w_helpCertificateInclusionConfig.Name = "w_helpCertificateInclusionConfig";
            this.w_helpCertificateInclusionConfig.RealOffset = null;
            this.w_helpCertificateInclusionConfig.RelativeOffset = null;
            this.w_helpCertificateInclusionConfig.TabStop = false;
            componentResourceManager.ApplyResources(this._gridControl, "_gridControl");
            this._gridControl.ColumnAutoWidth = false;
            this._gridControl.DataSource = null;
            this._gridControl.GridContextMenu = null;
            this._gridControl.MultiSelect = true;
            this._gridControl.Name = "_gridControl";
            this._gridControl.SelectedItems = new object[0];
            this._gridControl.ShowColumnHeaders = true;
            this._gridControl.ShowGridLines = DefaultBoolean.False;
            componentResourceManager.ApplyResources(this, "$this");
            base.AutoScaleMode = AutoScaleMode.Font;
            base.Controls.Add(this._gridControl);
            base.Controls.Add(this.w_helpCertificateInclusionConfig);
            base.Controls.Add(this.w_bRemove);
            base.Controls.Add(this.w_bAddFromFile);
            base.Controls.Add(this.w_bAddFromStore);
            base.Name = "TCCertificateInclusionConfig";
            ((ISupportInitialize)this.w_helpCertificateInclusionConfig).EndInit();
            base.ResumeLayout(false);
        }

        protected override void LoadUI()
        {
            if (this.Options == null)
            {
                return;
            }
            this.Certificates = this.Options.Certificates;
        }

        private void On_AddFromFile_Clicked(object sender, EventArgs e)
        {
            CertificateFromFileDialog certificateFromFileDialog = new CertificateFromFileDialog();
            if (certificateFromFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.AddCertificateToItemsView(certificateFromFileDialog.Certificate);
            }
        }

        private void On_AddFromStore_Clicked(object sender, EventArgs e)
        {
            CertificateFromStoreDialog certificateFromStoreDialog = new CertificateFromStoreDialog();
            if (certificateFromStoreDialog.ShowDialog() == DialogResult.OK)
            {
                this._gridControl.BeginUpdate();
                try
                {
                    foreach (X509CertificateWrapper certificate in certificateFromStoreDialog.Certificates)
                    {
                        this.AddCertificateToItemsView(certificate);
                    }
                }
                finally
                {
                    this._gridControl.EndUpdate();
                }
            }
        }

        private void On_Remove_Clicked(object sender, EventArgs e)
        {
            this._gridControl.BeginUpdate();
            try
            {
                X509CertificateWrapper[] selectedItems = this._gridControl.GetSelectedItems<X509CertificateWrapper>();
                for (int i = 0; i < (int)selectedItems.Length; i++)
                {
                    X509CertificateWrapper x509CertificateWrapper = selectedItems[i];
                    this._dataSource.Remove(x509CertificateWrapper);
                }
            }
            finally
            {
                this._gridControl.EndUpdate();
            }
        }

        private void PopulateItemsView(IEnumerable<X509CertificateWrapper> certs)
        {
            this._gridControl.BeginUpdate();
            try
            {
                this._dataSource.Clear();
                if (certs != null)
                {
                    foreach (X509CertificateWrapper cert in certs)
                    {
                        this._dataSource.Add(cert);
                    }
                }
            }
            finally
            {
                this._gridControl.EndUpdate();
            }
        }

        public override bool SaveUI()
        {
            if (this.Options != null)
            {
                this.Options.Certificates = this.Certificates;
            }
            return true;
        }

        private void UpdateDataRow(DataRow row, object obj)
        {
            X509CertificateWrapper x509CertificateWrapper = obj as X509CertificateWrapper;
            if (x509CertificateWrapper == null)
            {
                return;
            }
            row["FriendlyName"] = x509CertificateWrapper.FriendlyName;
            row["IssuedTo"] = x509CertificateWrapper.IssuedTo;
            row["IssuedBy"] = x509CertificateWrapper.IssuedBy;
            row["ExpirationDate"] = x509CertificateWrapper.ExpirationDateString;
            row["SourceFile"] = (x509CertificateWrapper.IsFromStore ? "" : x509CertificateWrapper.FilePath);
        }
    }
}