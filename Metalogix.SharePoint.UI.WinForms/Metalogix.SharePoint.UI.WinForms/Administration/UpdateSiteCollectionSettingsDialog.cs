using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Options.Administration;
using Metalogix.UI.WinForms;
using Metalogix.Utilities;

namespace Metalogix.SharePoint.UI.WinForms.Administration
{
    public class UpdateSiteCollectionSettingsDialog : XtraForm
    {
        private SPWeb m_target;

        private UpdateSiteCollectionSettingsOptions m_options;

        private IContainer components;

        private SimpleButton w_bOkay;

        private SimpleButton w_bCancel;

        private PanelControl w_gbSiteQuota;

        private PanelControl w_plQuotaOptions;

        private LabelControl label3;

        private SpinEdit w_numMax;

        private LabelControl label1;

        private SpinEdit w_numWarning;

        private LabelControl label4;

        private LabelControl w_lblMax;

        internal CheckEdit w_rbToIndividual;

        private global::DevExpress.XtraEditors.ComboBox w_cmbSiteQuotaTemplates;

        internal CheckEdit w_rbSetQuota;

        private CheckEdit w_cbSetSiteQuota;

        private PanelControl w_gbSiteAdmins;

        private CheckEdit w_cbSetSiteCollectionAdmins;

        private MemoEdit w_tbSiteCollectionAdmins;

        public UpdateSiteCollectionSettingsOptions Options
        {
            get
            {
                return m_options;
            }
            set
            {
                m_options = value;
                LoadUI();
            }
        }

        public SPWeb Target
        {
            get
            {
                return m_target;
            }
            set
            {
                m_target = value;
                UpdateQuotas();
            }
        }

        public UpdateSiteCollectionSettingsDialog()
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
            this.w_bOkay = new global::DevExpress.XtraEditors.SimpleButton();
            this.w_bCancel = new global::DevExpress.XtraEditors.SimpleButton();
            this.w_gbSiteQuota = new global::DevExpress.XtraEditors.PanelControl();
            this.w_plQuotaOptions = new global::DevExpress.XtraEditors.PanelControl();
            this.label3 = new global::DevExpress.XtraEditors.LabelControl();
            this.w_numMax = new global::DevExpress.XtraEditors.SpinEdit();
            this.label1 = new global::DevExpress.XtraEditors.LabelControl();
            this.w_numWarning = new global::DevExpress.XtraEditors.SpinEdit();
            this.label4 = new global::DevExpress.XtraEditors.LabelControl();
            this.w_lblMax = new global::DevExpress.XtraEditors.LabelControl();
            this.w_rbToIndividual = new global::DevExpress.XtraEditors.CheckEdit();
            this.w_cmbSiteQuotaTemplates = new global::DevExpress.XtraEditors.ComboBox();
            this.w_rbSetQuota = new global::DevExpress.XtraEditors.CheckEdit();
            this.w_cbSetSiteQuota = new global::DevExpress.XtraEditors.CheckEdit();
            this.w_gbSiteAdmins = new global::DevExpress.XtraEditors.PanelControl();
            this.w_tbSiteCollectionAdmins = new global::DevExpress.XtraEditors.MemoEdit();
            this.w_cbSetSiteCollectionAdmins = new global::DevExpress.XtraEditors.CheckEdit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_gbSiteQuota).BeginInit();
            this.w_gbSiteQuota.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)this.w_plQuotaOptions).BeginInit();
            this.w_plQuotaOptions.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)this.w_numMax.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_numWarning.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_rbToIndividual.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_cmbSiteQuotaTemplates.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_rbSetQuota.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_cbSetSiteQuota.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_gbSiteAdmins).BeginInit();
            this.w_gbSiteAdmins.SuspendLayout();
            ((global::System.ComponentModel.ISupportInitialize)this.w_tbSiteCollectionAdmins.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_cbSetSiteCollectionAdmins.Properties).BeginInit();
            base.SuspendLayout();
            this.w_bOkay.Anchor = global::System.Windows.Forms.AnchorStyles.Bottom | global::System.Windows.Forms.AnchorStyles.Right;
            this.w_bOkay.DialogResult = global::System.Windows.Forms.DialogResult.OK;
            this.w_bOkay.ImeMode = global::System.Windows.Forms.ImeMode.NoControl;
            this.w_bOkay.Location = new global::System.Drawing.Point(300, 253);
            this.w_bOkay.Name = "w_bOkay";
            this.w_bOkay.Size = new global::System.Drawing.Size(75, 23);
            this.w_bOkay.TabIndex = 2;
            this.w_bOkay.Text = "OK";
            this.w_bOkay.Click += new global::System.EventHandler(On_Okay);
            this.w_bCancel.Anchor = global::System.Windows.Forms.AnchorStyles.Bottom | global::System.Windows.Forms.AnchorStyles.Right;
            this.w_bCancel.DialogResult = global::System.Windows.Forms.DialogResult.Cancel;
            this.w_bCancel.ImeMode = global::System.Windows.Forms.ImeMode.NoControl;
            this.w_bCancel.Location = new global::System.Drawing.Point(381, 253);
            this.w_bCancel.Name = "w_bCancel";
            this.w_bCancel.Size = new global::System.Drawing.Size(75, 23);
            this.w_bCancel.TabIndex = 3;
            this.w_bCancel.Text = "Cancel";
            this.w_gbSiteQuota.BorderStyle = global::DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.w_gbSiteQuota.Controls.Add(this.w_plQuotaOptions);
            this.w_gbSiteQuota.Controls.Add(this.w_cbSetSiteQuota);
            this.w_gbSiteQuota.Location = new global::System.Drawing.Point(6, 120);
            this.w_gbSiteQuota.Name = "w_gbSiteQuota";
            this.w_gbSiteQuota.Size = new global::System.Drawing.Size(450, 128);
            this.w_gbSiteQuota.TabIndex = 1;
            this.w_plQuotaOptions.Anchor = global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right;
            this.w_plQuotaOptions.Controls.Add(this.label3);
            this.w_plQuotaOptions.Controls.Add(this.w_numMax);
            this.w_plQuotaOptions.Controls.Add(this.label1);
            this.w_plQuotaOptions.Controls.Add(this.w_numWarning);
            this.w_plQuotaOptions.Controls.Add(this.label4);
            this.w_plQuotaOptions.Controls.Add(this.w_lblMax);
            this.w_plQuotaOptions.Controls.Add(this.w_rbToIndividual);
            this.w_plQuotaOptions.Controls.Add(this.w_cmbSiteQuotaTemplates);
            this.w_plQuotaOptions.Controls.Add(this.w_rbSetQuota);
            this.w_plQuotaOptions.Location = new global::System.Drawing.Point(4, 29);
            this.w_plQuotaOptions.Name = "w_plQuotaOptions";
            this.w_plQuotaOptions.Size = new global::System.Drawing.Size(444, 96);
            this.w_plQuotaOptions.TabIndex = 1;
            this.label3.Anchor = global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Right;
            this.label3.ImeMode = global::System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new global::System.Drawing.Point(422, 25);
            this.label3.Name = "label3";
            this.label3.Size = new global::System.Drawing.Size(14, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "MB";
            this.w_numMax.Anchor = global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Right;
            this.w_numMax.EditValue = new decimal(new int[4]);
            this.w_numMax.Location = new global::System.Drawing.Point(292, 22);
            this.w_numMax.Name = "w_numMax";
            this.w_numMax.Properties.Buttons.AddRange(new global::DevExpress.XtraEditors.Controls.EditorButton[1]
            {
            new global::DevExpress.XtraEditors.Controls.EditorButton()
            });
            global::DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties = this.w_numMax.Properties;
            int[] bits = new int[4] { 10, 0, 0, 0 };
            properties.Increment = new decimal(bits);
            this.w_numMax.Properties.IsFloatValue = false;
            this.w_numMax.Properties.Mask.EditMask = "N00";
            global::DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties2 = this.w_numMax.Properties;
            int[] bits2 = new int[4] { 1000000, 0, 0, 0 };
            properties2.MaxValue = new decimal(bits2);
            this.w_numMax.Size = new global::System.Drawing.Size(126, 20);
            this.w_numMax.TabIndex = 2;
            this.label1.Anchor = global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Right;
            this.label1.ImeMode = global::System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new global::System.Drawing.Point(422, 47);
            this.label1.Name = "label1";
            this.label1.Size = new global::System.Drawing.Size(14, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "MB";
            this.w_numWarning.Anchor = global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Right;
            this.w_numWarning.EditValue = new decimal(new int[4]);
            this.w_numWarning.Location = new global::System.Drawing.Point(292, 44);
            this.w_numWarning.Name = "w_numWarning";
            this.w_numWarning.Properties.Buttons.AddRange(new global::DevExpress.XtraEditors.Controls.EditorButton[1]
            {
            new global::DevExpress.XtraEditors.Controls.EditorButton()
            });
            global::DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties3 = this.w_numWarning.Properties;
            int[] bits3 = new int[4] { 10, 0, 0, 0 };
            properties3.Increment = new decimal(bits3);
            this.w_numWarning.Properties.IsFloatValue = false;
            this.w_numWarning.Properties.Mask.EditMask = "N00";
            global::DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties4 = this.w_numWarning.Properties;
            int[] bits4 = new int[4] { 1000000, 0, 0, 0 };
            properties4.MaxValue = new decimal(bits4);
            this.w_numWarning.Size = new global::System.Drawing.Size(126, 20);
            this.w_numWarning.TabIndex = 5;
            this.label4.ImeMode = global::System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new global::System.Drawing.Point(44, 47);
            this.label4.Name = "label4";
            this.label4.Size = new global::System.Drawing.Size(233, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Send warning e-mail when site storage reaches: ";
            this.w_lblMax.ImeMode = global::System.Windows.Forms.ImeMode.NoControl;
            this.w_lblMax.Location = new global::System.Drawing.Point(44, 25);
            this.w_lblMax.Name = "w_lblMax";
            this.w_lblMax.Size = new global::System.Drawing.Size(170, 13);
            this.w_lblMax.TabIndex = 1;
            this.w_lblMax.Text = "Limit site storage to a maximum of: ";
            this.w_rbToIndividual.EditValue = true;
            this.w_rbToIndividual.ImeMode = global::System.Windows.Forms.ImeMode.NoControl;
            this.w_rbToIndividual.Location = new global::System.Drawing.Point(20, 0);
            this.w_rbToIndividual.Name = "w_rbToIndividual";
            this.w_rbToIndividual.Properties.Caption = "To Individual Values";
            this.w_rbToIndividual.Properties.CheckStyle = global::DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.w_rbToIndividual.Properties.RadioGroupIndex = 1;
            this.w_rbToIndividual.Size = new global::System.Drawing.Size(130, 19);
            this.w_rbToIndividual.TabIndex = 0;
            this.w_rbToIndividual.CheckedChanged += new global::System.EventHandler(On_SiteQuota_CheckedChanged);
            this.w_cmbSiteQuotaTemplates.Anchor = global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right;
            this.w_cmbSiteQuotaTemplates.Location = new global::System.Drawing.Point(120, 66);
            this.w_cmbSiteQuotaTemplates.Name = "w_cmbSiteQuotaTemplates";
            this.w_cmbSiteQuotaTemplates.Properties.Buttons.AddRange(new global::DevExpress.XtraEditors.Controls.EditorButton[1]
            {
            new global::DevExpress.XtraEditors.Controls.EditorButton(global::DevExpress.XtraEditors.Controls.ButtonPredefines.DropDown)
            });
            this.w_cmbSiteQuotaTemplates.Size = new global::System.Drawing.Size(318, 20);
            this.w_cmbSiteQuotaTemplates.TabIndex = 8;
            this.w_rbSetQuota.ImeMode = global::System.Windows.Forms.ImeMode.NoControl;
            this.w_rbSetQuota.Location = new global::System.Drawing.Point(20, 66);
            this.w_rbSetQuota.Name = "w_rbSetQuota";
            this.w_rbSetQuota.Properties.Caption = "To Template";
            this.w_rbSetQuota.Properties.CheckStyle = global::DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.w_rbSetQuota.Properties.RadioGroupIndex = 1;
            this.w_rbSetQuota.Size = new global::System.Drawing.Size(94, 19);
            this.w_rbSetQuota.TabIndex = 1;
            this.w_rbSetQuota.TabStop = false;
            this.w_rbSetQuota.CheckedChanged += new global::System.EventHandler(On_SiteQuota_CheckedChanged);
            this.w_cbSetSiteQuota.ImeMode = global::System.Windows.Forms.ImeMode.NoControl;
            this.w_cbSetSiteQuota.Location = new global::System.Drawing.Point(6, 10);
            this.w_cbSetSiteQuota.Name = "w_cbSetSiteQuota";
            this.w_cbSetSiteQuota.Properties.Caption = "Set Site Quota";
            this.w_cbSetSiteQuota.Size = new global::System.Drawing.Size(95, 19);
            this.w_cbSetSiteQuota.TabIndex = 0;
            this.w_cbSetSiteQuota.CheckedChanged += new global::System.EventHandler(On_SetQuota_CheckedChanged);
            this.w_gbSiteAdmins.BorderStyle = global::DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.w_gbSiteAdmins.Controls.Add(this.w_tbSiteCollectionAdmins);
            this.w_gbSiteAdmins.Controls.Add(this.w_cbSetSiteCollectionAdmins);
            this.w_gbSiteAdmins.Location = new global::System.Drawing.Point(6, 6);
            this.w_gbSiteAdmins.Name = "w_gbSiteAdmins";
            this.w_gbSiteAdmins.Size = new global::System.Drawing.Size(450, 112);
            this.w_gbSiteAdmins.TabIndex = 0;
            this.w_tbSiteCollectionAdmins.Location = new global::System.Drawing.Point(26, 33);
            this.w_tbSiteCollectionAdmins.Name = "w_tbSiteCollectionAdmins";
            this.w_tbSiteCollectionAdmins.Size = new global::System.Drawing.Size(418, 72);
            this.w_tbSiteCollectionAdmins.TabIndex = 1;
            this.w_cbSetSiteCollectionAdmins.Location = new global::System.Drawing.Point(6, 10);
            this.w_cbSetSiteCollectionAdmins.Name = "w_cbSetSiteCollectionAdmins";
            this.w_cbSetSiteCollectionAdmins.Properties.Caption = "Set Site Collection Admins";
            this.w_cbSetSiteCollectionAdmins.Size = new global::System.Drawing.Size(149, 19);
            this.w_cbSetSiteCollectionAdmins.TabIndex = 0;
            this.w_cbSetSiteCollectionAdmins.CheckedChanged += new global::System.EventHandler(On_SetSiteCollectionAdmins_CheckedChanged);
            base.AcceptButton = this.w_bOkay;
            base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = this.w_bCancel;
            base.ClientSize = new global::System.Drawing.Size(461, 282);
            base.Controls.Add(this.w_gbSiteAdmins);
            base.Controls.Add(this.w_gbSiteQuota);
            base.Controls.Add(this.w_bOkay);
            base.Controls.Add(this.w_bCancel);
            base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            base.Name = "UpdateSiteCollectionSettingsDialog";
            base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Update Site Collection Settings";
            ((global::System.ComponentModel.ISupportInitialize)this.w_gbSiteQuota).EndInit();
            this.w_gbSiteQuota.ResumeLayout(false);
            ((global::System.ComponentModel.ISupportInitialize)this.w_plQuotaOptions).EndInit();
            this.w_plQuotaOptions.ResumeLayout(false);
            this.w_plQuotaOptions.PerformLayout();
            ((global::System.ComponentModel.ISupportInitialize)this.w_numMax.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_numWarning.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_rbToIndividual.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_cmbSiteQuotaTemplates.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_rbSetQuota.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_cbSetSiteQuota.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_gbSiteAdmins).EndInit();
            this.w_gbSiteAdmins.ResumeLayout(false);
            ((global::System.ComponentModel.ISupportInitialize)this.w_tbSiteCollectionAdmins.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_cbSetSiteCollectionAdmins.Properties).EndInit();
            base.ResumeLayout(false);
        }

        private void LoadUI()
        {
            w_cbSetSiteCollectionAdmins.Checked = Options.SetSiteCollectionAdmins;
            w_tbSiteCollectionAdmins.Text = Options.SiteCollectionAdmins;
            UpdateSiteCollectionsEnabled();
            w_cbSetSiteQuota.Checked = Options.SetSiteQuota;
            w_numWarning.Value = Options.QuotaWarning;
            w_numMax.Value = Options.QuotaMaximum;
            bool flag = false;
            if (Options.QuotaID != null)
            {
                foreach (object item in w_cmbSiteQuotaTemplates.Properties.Items)
                {
                    if (!(item is SPSiteQuota sPSiteQuota) || !(sPSiteQuota.QuotaID == Options.QuotaID))
                    {
                        continue;
                    }
                    w_cmbSiteQuotaTemplates.SelectedItem = sPSiteQuota;
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                w_rbSetQuota.Checked = true;
            }
            else if (w_numMax.Value > 0m || w_numWarning.Value > 0m)
            {
                w_rbToIndividual.Checked = true;
            }
            UpdateQuotasEnabled();
        }

        private void On_Okay(object sender, EventArgs e)
        {
            if (!SaveUI())
            {
                base.DialogResult = DialogResult.None;
            }
        }

        private void On_SetQuota_CheckedChanged(object sender, EventArgs e)
        {
            UpdateQuotasEnabled();
        }

        private void On_SetSiteCollectionAdmins_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSiteCollectionsEnabled();
        }

        private void On_SiteQuota_CheckedChanged(object sender, EventArgs e)
        {
            UpdateQuotasEnabled();
        }

        private bool SaveUI()
        {
            string text = null;
            if (w_cbSetSiteQuota.Checked && w_rbSetQuota.Checked && w_cmbSiteQuotaTemplates.SelectedItem == null)
            {
                text = "Please select the site quota template to use for the site collection";
                w_cmbSiteQuotaTemplates.Focus();
            }
            if (w_cbSetSiteQuota.Checked && !w_rbSetQuota.Checked && !w_rbToIndividual.Checked)
            {
                text = "Please select the site quota template to use for the site collection";
                w_rbToIndividual.Focus();
            }
            if (w_cbSetSiteCollectionAdmins.Checked && string.IsNullOrEmpty(w_tbSiteCollectionAdmins.Text))
            {
                text = "Please enter the site collection admins you wish to set.";
                w_tbSiteCollectionAdmins.Focus();
            }
            if (text != null)
            {
                FlatXtraMessageBox.Show(text, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }
            Options.SetSiteQuota = w_cbSetSiteQuota.Checked;
            if (w_cbSetSiteQuota.Checked)
            {
                UpdateSiteCollectionSettingsOptions options = Options;
                string quotaID = ((!w_rbSetQuota.Checked) ? null : ((SPSiteQuota)w_cmbSiteQuotaTemplates.SelectedItem).QuotaID);
                options.QuotaID = quotaID;
                Options.QuotaMaximum = (w_rbToIndividual.Checked ? ((long)w_numMax.Value) : 0);
                Options.QuotaWarning = (w_rbToIndividual.Checked ? ((long)w_numWarning.Value) : 0);
            }
            Options.SetSiteCollectionAdmins = w_cbSetSiteCollectionAdmins.Checked;
            Options.SiteCollectionAdmins = w_tbSiteCollectionAdmins.Text;
            return true;
        }

        private void UpdateQuotas()
        {
            string siteQuotaTemplates = Target.Adapter.Reader.GetSiteQuotaTemplates();
            SPSiteQuotaCollection sPSiteQuotaCollection = new SPSiteQuotaCollection(null, XmlUtility.StringToXmlNode(siteQuotaTemplates));
            if (sPSiteQuotaCollection.Count <= 0)
            {
                return;
            }
            foreach (SPSiteQuota item in sPSiteQuotaCollection)
            {
                w_cmbSiteQuotaTemplates.Properties.Items.Add(item);
            }
            if (w_cmbSiteQuotaTemplates.Properties.Items.Count > 0)
            {
                w_cmbSiteQuotaTemplates.SelectedIndex = 0;
            }
        }

        private void UpdateQuotasEnabled()
        {
            w_cmbSiteQuotaTemplates.Enabled = w_rbSetQuota.Checked;
            w_numMax.Enabled = w_rbToIndividual.Checked;
            w_numWarning.Enabled = w_rbToIndividual.Checked;
            w_plQuotaOptions.Enabled = w_cbSetSiteQuota.Checked;
        }

        private void UpdateSiteCollectionsEnabled()
        {
            w_tbSiteCollectionAdmins.Enabled = w_cbSetSiteCollectionAdmins.Checked;
        }
    }
}