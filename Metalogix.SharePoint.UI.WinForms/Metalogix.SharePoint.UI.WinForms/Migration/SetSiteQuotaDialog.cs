using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class SetSiteQuotaDialog : XtraForm
	{
		private string m_sQuotaID;

		private long m_lQuotaMax;

		private long m_lQuotaWarning;

		private SPSiteQuotaCollection m_siteQuotaTemplates;

		private IContainer components;

		private SimpleButton w_bOkay;

		private SimpleButton w_bCancel;

		internal PanelControl w_plSiteQuotas;

		private SpinEdit w_numWarning;

		internal CheckEdit w_rbToIndividual;

		private DevExpress.XtraEditors.ComboBox w_cmbSiteQuotaTemplates;

		internal CheckEdit w_rbSetQuota;

		internal CheckEdit w_rbSourceQuota;

		private LabelControl label2;

		private LabelControl w_lblMax;

		private LabelControl w_lblSetQuota;

		private LabelControl label3;

		private SpinEdit w_numMax;

		private LabelControl label1;

		public string QuotaID
		{
			get
			{
				return m_sQuotaID;
			}
			set
			{
				m_sQuotaID = value;
			}
		}

		public long SiteQuotaMaximum
		{
			get
			{
				return m_lQuotaMax;
			}
			set
			{
				m_lQuotaMax = value;
			}
		}

		public SPSiteQuotaCollection SiteQuotaTemplates
		{
			get
			{
				return m_siteQuotaTemplates;
			}
			set
			{
				m_siteQuotaTemplates = value;
				UpdateQuotas();
			}
		}

		public long SiteQuotaWarning
		{
			get
			{
				return m_lQuotaWarning;
			}
			set
			{
				m_lQuotaWarning = value;
			}
		}

		public SetSiteQuotaDialog()
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
			this.w_bOkay = new DevExpress.XtraEditors.SimpleButton();
			this.w_bCancel = new DevExpress.XtraEditors.SimpleButton();
			this.w_plSiteQuotas = new DevExpress.XtraEditors.PanelControl();
			this.label3 = new DevExpress.XtraEditors.LabelControl();
			this.w_numMax = new DevExpress.XtraEditors.SpinEdit();
			this.label1 = new DevExpress.XtraEditors.LabelControl();
			this.w_numWarning = new DevExpress.XtraEditors.SpinEdit();
			this.label2 = new DevExpress.XtraEditors.LabelControl();
			this.w_lblMax = new DevExpress.XtraEditors.LabelControl();
			this.w_rbToIndividual = new DevExpress.XtraEditors.CheckEdit();
			this.w_cmbSiteQuotaTemplates = new DevExpress.XtraEditors.ComboBox();
			this.w_rbSetQuota = new DevExpress.XtraEditors.CheckEdit();
			this.w_rbSourceQuota = new DevExpress.XtraEditors.CheckEdit();
			this.w_lblSetQuota = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)this.w_plSiteQuotas).BeginInit();
			this.w_plSiteQuotas.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_numMax.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_numWarning.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbToIndividual.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_cmbSiteQuotaTemplates.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbSetQuota.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbSourceQuota.Properties).BeginInit();
			base.SuspendLayout();
			this.w_bOkay.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.w_bOkay.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_bOkay.Location = new System.Drawing.Point(234, 151);
			this.w_bOkay.Name = "w_bOkay";
			this.w_bOkay.Size = new System.Drawing.Size(75, 23);
			this.w_bOkay.TabIndex = 2;
			this.w_bOkay.Text = "OK";
			this.w_bOkay.Click += new System.EventHandler(On_OK_Clicked);
			this.w_bCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.w_bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_bCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_bCancel.Location = new System.Drawing.Point(315, 151);
			this.w_bCancel.Name = "w_bCancel";
			this.w_bCancel.Size = new System.Drawing.Size(75, 23);
			this.w_bCancel.TabIndex = 3;
			this.w_bCancel.Text = "Cancel";
			this.w_plSiteQuotas.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.w_plSiteQuotas.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plSiteQuotas.Controls.Add(this.label3);
			this.w_plSiteQuotas.Controls.Add(this.w_numMax);
			this.w_plSiteQuotas.Controls.Add(this.label1);
			this.w_plSiteQuotas.Controls.Add(this.w_numWarning);
			this.w_plSiteQuotas.Controls.Add(this.label2);
			this.w_plSiteQuotas.Controls.Add(this.w_lblMax);
			this.w_plSiteQuotas.Controls.Add(this.w_rbToIndividual);
			this.w_plSiteQuotas.Controls.Add(this.w_cmbSiteQuotaTemplates);
			this.w_plSiteQuotas.Controls.Add(this.w_rbSetQuota);
			this.w_plSiteQuotas.Controls.Add(this.w_rbSourceQuota);
			this.w_plSiteQuotas.Location = new System.Drawing.Point(6, 32);
			this.w_plSiteQuotas.Name = "w_plSiteQuotas";
			this.w_plSiteQuotas.Size = new System.Drawing.Size(391, 110);
			this.w_plSiteQuotas.TabIndex = 1;
			this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label3.Location = new System.Drawing.Point(370, 47);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(14, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "MB";
			this.w_numMax.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			this.w_numMax.EditValue = new decimal(new int[4]);
			this.w_numMax.Location = new System.Drawing.Point(296, 44);
			this.w_numMax.Name = "w_numMax";
			this.w_numMax.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
			{
				new DevExpress.XtraEditors.Controls.EditorButton()
			});
			this.w_numMax.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties = this.w_numMax.Properties;
			int[] bits = new int[4] { 10, 0, 0, 0 };
			properties.Increment = new decimal(bits);
			this.w_numMax.Properties.IsFloatValue = false;
			this.w_numMax.Properties.Mask.EditMask = "N00";
			DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties2 = this.w_numMax.Properties;
			int[] bits2 = new int[4] { 1000000, 0, 0, 0 };
			properties2.MaxValue = new decimal(bits2);
			this.w_numMax.Size = new System.Drawing.Size(71, 20);
			this.w_numMax.TabIndex = 3;
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label1.Location = new System.Drawing.Point(370, 69);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(14, 13);
			this.label1.TabIndex = 7;
			this.label1.Text = "MB";
			this.w_numWarning.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			this.w_numWarning.EditValue = new decimal(new int[4]);
			this.w_numWarning.Location = new System.Drawing.Point(296, 66);
			this.w_numWarning.Name = "w_numWarning";
			this.w_numWarning.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
			{
				new DevExpress.XtraEditors.Controls.EditorButton()
			});
			this.w_numWarning.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties3 = this.w_numWarning.Properties;
			int[] bits3 = new int[4] { 10, 0, 0, 0 };
			properties3.Increment = new decimal(bits3);
			this.w_numWarning.Properties.IsFloatValue = false;
			this.w_numWarning.Properties.Mask.EditMask = "N00";
			DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit properties4 = this.w_numWarning.Properties;
			int[] bits4 = new int[4] { 1000000, 0, 0, 0 };
			properties4.MaxValue = new decimal(bits4);
			this.w_numWarning.Size = new System.Drawing.Size(71, 20);
			this.w_numWarning.TabIndex = 6;
			this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label2.Location = new System.Drawing.Point(58, 69);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(233, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Send warning e-mail when site storage reaches: ";
			this.w_lblMax.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_lblMax.Location = new System.Drawing.Point(58, 47);
			this.w_lblMax.Name = "w_lblMax";
			this.w_lblMax.Size = new System.Drawing.Size(170, 13);
			this.w_lblMax.TabIndex = 2;
			this.w_lblMax.Text = "Limit site storage to a maximum of: ";
			this.w_rbToIndividual.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_rbToIndividual.Location = new System.Drawing.Point(18, 22);
			this.w_rbToIndividual.Name = "w_rbToIndividual";
			this.w_rbToIndividual.Properties.Caption = "To Individual Values";
			this.w_rbToIndividual.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.w_rbToIndividual.Properties.RadioGroupIndex = 1;
			this.w_rbToIndividual.Size = new System.Drawing.Size(130, 19);
			this.w_rbToIndividual.TabIndex = 1;
			this.w_rbToIndividual.TabStop = false;
			this.w_rbToIndividual.CheckedChanged += new System.EventHandler(On_Radio_CheckedChanged);
			this.w_cmbSiteQuotaTemplates.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.w_cmbSiteQuotaTemplates.Location = new System.Drawing.Point(126, 88);
			this.w_cmbSiteQuotaTemplates.Name = "w_cmbSiteQuotaTemplates";
			this.w_cmbSiteQuotaTemplates.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
			{
				new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.DropDown)
			});
			this.w_cmbSiteQuotaTemplates.Size = new System.Drawing.Size(265, 20);
			this.w_cmbSiteQuotaTemplates.TabIndex = 9;
			this.w_rbSetQuota.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_rbSetQuota.Location = new System.Drawing.Point(18, 88);
			this.w_rbSetQuota.Name = "w_rbSetQuota";
			this.w_rbSetQuota.Properties.Caption = "To Template";
			this.w_rbSetQuota.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.w_rbSetQuota.Properties.RadioGroupIndex = 1;
			this.w_rbSetQuota.Size = new System.Drawing.Size(94, 19);
			this.w_rbSetQuota.TabIndex = 8;
			this.w_rbSetQuota.TabStop = false;
			this.w_rbSetQuota.CheckedChanged += new System.EventHandler(On_Radio_CheckedChanged);
			this.w_rbSourceQuota.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_rbSourceQuota.Location = new System.Drawing.Point(18, 0);
			this.w_rbSourceQuota.Name = "w_rbSourceQuota";
			this.w_rbSourceQuota.Properties.Caption = "To Source Value";
			this.w_rbSourceQuota.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.w_rbSourceQuota.Properties.RadioGroupIndex = 1;
			this.w_rbSourceQuota.Size = new System.Drawing.Size(245, 19);
			this.w_rbSourceQuota.TabIndex = 0;
			this.w_rbSourceQuota.TabStop = false;
			this.w_rbSourceQuota.CheckedChanged += new System.EventHandler(On_Radio_CheckedChanged);
			this.w_lblSetQuota.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.w_lblSetQuota.Location = new System.Drawing.Point(6, 6);
			this.w_lblSetQuota.Name = "w_lblSetQuota";
			this.w_lblSetQuota.Size = new System.Drawing.Size(74, 13);
			this.w_lblSetQuota.TabIndex = 0;
			this.w_lblSetQuota.Text = "Set Site Quota:";
			base.AcceptButton = this.w_bOkay;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.w_bCancel;
			base.ClientSize = new System.Drawing.Size(405, 186);
			base.Controls.Add(this.w_lblSetQuota);
			base.Controls.Add(this.w_plSiteQuotas);
			base.Controls.Add(this.w_bOkay);
			base.Controls.Add(this.w_bCancel);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "SetSiteQuotaDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Set Site Quota";
			base.Load += new System.EventHandler(On_Load);
			((System.ComponentModel.ISupportInitialize)this.w_plSiteQuotas).EndInit();
			this.w_plSiteQuotas.ResumeLayout(false);
			this.w_plSiteQuotas.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.w_numMax.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_numWarning.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbToIndividual.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_cmbSiteQuotaTemplates.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbSetQuota.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_rbSourceQuota.Properties).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void LoadUI()
		{
			bool flag = false;
			w_numMax.Value = SiteQuotaMaximum;
			w_numWarning.Value = SiteQuotaWarning;
			if (QuotaID != null)
			{
				foreach (object item in w_cmbSiteQuotaTemplates.Properties.Items)
				{
					if (!(item is SPSiteQuota sPSiteQuota) || !(sPSiteQuota.QuotaID == QuotaID))
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
			else if (!(w_numMax.Value > 0m) && !(w_numWarning.Value > 0m))
			{
				w_rbSourceQuota.Checked = true;
			}
			else
			{
				w_rbToIndividual.Checked = true;
			}
		}

		private void On_Load(object sender, EventArgs e)
		{
			LoadUI();
		}

		private void On_OK_Clicked(object sender, EventArgs e)
		{
			SaveUI();
			base.DialogResult = DialogResult.OK;
		}

		private void On_Radio_CheckedChanged(object sender, EventArgs e)
		{
			w_cmbSiteQuotaTemplates.Enabled = w_rbSetQuota.Checked;
			w_numMax.Enabled = w_rbToIndividual.Checked;
			w_numWarning.Enabled = w_rbToIndividual.Checked;
		}

		private void SaveUI()
		{
			string sQuotaID = ((!w_rbSetQuota.Checked) ? null : ((SPSiteQuota)w_cmbSiteQuotaTemplates.SelectedItem).QuotaID);
			m_sQuotaID = sQuotaID;
			m_lQuotaMax = (w_rbToIndividual.Checked ? ((long)w_numMax.Value) : 0);
			m_lQuotaWarning = (w_rbToIndividual.Checked ? ((long)w_numWarning.Value) : 0);
		}

		private void UpdateQuotas()
		{
			if (SiteQuotaTemplates.Count <= 0)
			{
				return;
			}
			foreach (SPSiteQuota siteQuotaTemplate in SiteQuotaTemplates)
			{
				w_cmbSiteQuotaTemplates.Properties.Items.Add(siteQuotaTemplate);
			}
			if (w_cmbSiteQuotaTemplates.Properties.Items.Count > 0)
			{
				w_cmbSiteQuotaTemplates.SelectedIndex = 0;
			}
		}
	}
}
