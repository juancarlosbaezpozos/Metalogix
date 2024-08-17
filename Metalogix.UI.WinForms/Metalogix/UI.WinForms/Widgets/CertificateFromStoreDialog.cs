using DevExpress.XtraEditors;
using Metalogix.DataStructures;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Widgets
{
    public partial class CertificateFromStoreDialog : XtraForm
    {
        private const string FIELD_FRIENDLY_NAME = "FriendlyName";

        private const string FIELD_ISSUED_TO = "IssuedTo";

        private const string FIELD_ISSUED_BY = "IssuedBy";

        private const string FIELD_EXPIRATION_DATE = "ExpirationDate";

        private string m_sStoreName;

        private List<X509CertificateWrapper> m_certificates;

        private IContainer components;

        private LabelControl w_lbExplanation;

        private SimpleButton w_bCancel;

        private SimpleButton w_bOkay;

        private SimplifiedGridView _gridControl;

        public ReadOnlyCollection<X509CertificateWrapper> Certificates
        {
            get
            {
                if (this.m_certificates == null)
                {
                    return new ReadOnlyCollection<X509CertificateWrapper>(new X509CertificateWrapper[0]);
                }
                return new ReadOnlyCollection<X509CertificateWrapper>(this.m_certificates);
            }
        }

        public CertificateFromStoreDialog()
        {
            this.InitializeComponent();
            this.InitializeColumnHeaders();
            this.PopulateCertificates();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private string GetNamePropFromCertString(string value)
        {
            int num = value.IndexOf("CN=", StringComparison.InvariantCultureIgnoreCase);
            if (num < 0)
            {
                return value;
            }
            num += 3;
            int num1 = value.IndexOf(',', num);
            if (num1 < 0)
            {
                return value.Substring(num);
            }
            return value.Substring(num, num1 - num);
        }

        private void InitializeColumnHeaders()
        {
            this._gridControl.RowBuilderMethod = new SimplifiedGridView.UpdateDataRowDelegate(this.UpdateDataRow);
            this._gridControl.CreateColumn(Resources.Header_FriendlyName, "FriendlyName", 150);
            this._gridControl.CreateColumn(Resources.Header_Issued_To, "IssuedTo", 120);
            this._gridControl.CreateColumn(Resources.Header_Issued_By, "IssuedBy", 120);
            this._gridControl.CreateColumn(Resources.Header_Expiration_Date, "ExpirationDate", 120);
        }

        private void InitializeComponent()
        {
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(CertificateFromStoreDialog));
            this.w_lbExplanation = new LabelControl();
            this.w_bCancel = new SimpleButton();
            this.w_bOkay = new SimpleButton();
            this._gridControl = new SimplifiedGridView();
            base.SuspendLayout();
            componentResourceManager.ApplyResources(this.w_lbExplanation, "w_lbExplanation");
            this.w_lbExplanation.Name = "w_lbExplanation";
            componentResourceManager.ApplyResources(this.w_bCancel, "w_bCancel");
            this.w_bCancel.DialogResult = DialogResult.Cancel;
            this.w_bCancel.Name = "w_bCancel";
            componentResourceManager.ApplyResources(this.w_bOkay, "w_bOkay");
            this.w_bOkay.Name = "w_bOkay";
            this.w_bOkay.Click += new EventHandler(this.On_OK_Clicked);
            componentResourceManager.ApplyResources(this._gridControl, "_gridControl");
            this._gridControl.ColumnAutoWidth = false;
            this._gridControl.DataSource = null;
            this._gridControl.GridContextMenu = null;
            this._gridControl.MultiSelect = true;
            this._gridControl.Name = "_gridControl";
            this._gridControl.SelectedItems = new object[0];
            this._gridControl.ShowColumnHeaders = true;
            base.AcceptButton = this.w_bOkay;
            componentResourceManager.ApplyResources(this, "$this");
            base.AutoScaleMode = AutoScaleMode.Font;
            base.CancelButton = this.w_bCancel;
            base.Controls.Add(this._gridControl);
            base.Controls.Add(this.w_bOkay);
            base.Controls.Add(this.w_bCancel);
            base.Controls.Add(this.w_lbExplanation);
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "CertificateFromStoreDialog";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void On_OK_Clicked(object sender, EventArgs e)
        {
            X509Certificate[] selectedItems = this._gridControl.GetSelectedItems<X509Certificate>();
            List<X509CertificateWrapper> x509CertificateWrappers = new List<X509CertificateWrapper>((int)selectedItems.Length);
            X509Certificate[] x509CertificateArray = selectedItems;
            for (int i = 0; i < (int)x509CertificateArray.Length; i++)
            {
                x509CertificateWrappers.Add(new X509CertificateWrapper(x509CertificateArray[i], this.m_sStoreName));
            }
            this.m_certificates = x509CertificateWrappers;
            base.DialogResult = DialogResult.OK;
            base.Close();
        }

        private void PopulateCertificates()
        {
            X509Store x509Store = new X509Store();
            this.m_sStoreName = x509Store.Name;
            BindingList<X509Certificate> x509Certificates = new BindingList<X509Certificate>();
            x509Store.Open(OpenFlags.ReadOnly);
            try
            {
                X509Certificate2Enumerator enumerator = x509Store.Certificates.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    x509Certificates.Add(enumerator.Current);
                }
            }
            finally
            {
                x509Store.Close();
            }
            this._gridControl.DataSource = x509Certificates;
            this._gridControl.RunAutoWidthOnAllColumns();
        }

        private void UpdateDataRow(DataRow row, object obj)
        {
            X509Certificate x509Certificate = obj as X509Certificate;
            if (x509Certificate == null)
            {
                return;
            }
            row["FriendlyName"] = X509CertificateWrapper.GetFriendlyName(x509Certificate);
            row["IssuedTo"] = this.GetNamePropFromCertString(x509Certificate.Subject);
            row["IssuedBy"] = this.GetNamePropFromCertString(x509Certificate.Issuer);
            row["ExpirationDate"] = x509Certificate.GetExpirationDateString();
        }
    }
}