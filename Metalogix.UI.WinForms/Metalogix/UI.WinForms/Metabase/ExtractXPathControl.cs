using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.Metabase.Options;

namespace Metalogix.UI.WinForms.Metabase
{
    public class ExtractXPathControl : UserControl
    {
        private IContainer components;

        private GroupControl w_groupBoxExtract;

        private LabelControl w_lblSeparator;

        private CheckEdit w_radioReturnCounts;

        private CheckEdit w_radioReturnFirstMatch;

        private CheckEdit w_radioReturnAllMatches;

        private GroupControl w_groupBoxFormat;

        private CheckEdit w_radioFormatOuterXml;

        private CheckEdit w_radioFormatText;

        private CheckEdit w_radioFormatInnerXml;

        private ComboBoxEdit w_comboSep;

        public bool FormatEnabled
        {
            get
		{
			return w_groupBoxFormat.Enabled;
		}
            set
		{
			w_groupBoxFormat.Enabled = value;
		}
        }

        public ExtractContentOptions.ReturnDetail ReturnDetail
        {
            get
		{
			if (w_radioReturnFirstMatch.Checked)
			{
				return ExtractContentOptions.ReturnDetail.FirstMatch;
			}
			if (w_radioReturnAllMatches.Checked)
			{
				return ExtractContentOptions.ReturnDetail.AllMatches;
			}
			return ExtractContentOptions.ReturnDetail.CountsOnly;
		}
            set
		{
			switch (value)
			{
			case ExtractContentOptions.ReturnDetail.FirstMatch:
				w_radioReturnFirstMatch.Checked = true;
				break;
			case ExtractContentOptions.ReturnDetail.AllMatches:
				w_radioReturnAllMatches.Checked = true;
				break;
			case ExtractContentOptions.ReturnDetail.CountsOnly:
				w_radioReturnCounts.Checked = true;
				break;
			}
		}
        }

        public ExtractContentOptions.ReturnFormat ReturnFormat
        {
            get
		{
			if (w_radioFormatText.Checked)
			{
				return ExtractContentOptions.ReturnFormat.Text;
			}
			if (w_radioFormatInnerXml.Checked)
			{
				return ExtractContentOptions.ReturnFormat.InnerXML;
			}
			return ExtractContentOptions.ReturnFormat.OuterXML;
		}
            set
		{
			switch (value)
			{
			case ExtractContentOptions.ReturnFormat.Text:
				w_radioFormatText.Checked = true;
				break;
			case ExtractContentOptions.ReturnFormat.InnerXML:
				w_radioFormatInnerXml.Checked = true;
				break;
			case ExtractContentOptions.ReturnFormat.OuterXML:
				w_radioFormatOuterXml.Checked = true;
				break;
			}
		}
        }

        public string Separator
        {
            get
		{
			string text = w_comboSep.Text;
			string str = text;
			if (text != null)
			{
				switch (str)
				{
				case "\\t":
					return "\t";
				case "\\r\\n":
					return "\r\n";
				case "\\n":
					return "\n";
				}
			}
			return w_comboSep.Text;
		}
            set
		{
			w_comboSep.Text = value;
			if (value != null)
			{
				if (value == "\t")
				{
					w_comboSep.Text = "\\t";
				}
				else if (value == "\r\n")
				{
					w_comboSep.Text = "\\r\\n";
				}
				else if (!(value != "\n"))
				{
					w_comboSep.Text = "\\n";
				}
			}
		}
        }

        public ExtractXPathControl()
	{
		InitializeComponent();
		UpdateSeparator();
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
		this.w_groupBoxExtract = new DevExpress.XtraEditors.GroupControl();
		this.w_comboSep = new DevExpress.XtraEditors.ComboBoxEdit();
		this.w_lblSeparator = new DevExpress.XtraEditors.LabelControl();
		this.w_radioReturnCounts = new DevExpress.XtraEditors.CheckEdit();
		this.w_radioReturnFirstMatch = new DevExpress.XtraEditors.CheckEdit();
		this.w_radioReturnAllMatches = new DevExpress.XtraEditors.CheckEdit();
		this.w_groupBoxFormat = new DevExpress.XtraEditors.GroupControl();
		this.w_radioFormatOuterXml = new DevExpress.XtraEditors.CheckEdit();
		this.w_radioFormatText = new DevExpress.XtraEditors.CheckEdit();
		this.w_radioFormatInnerXml = new DevExpress.XtraEditors.CheckEdit();
		((System.ComponentModel.ISupportInitialize)this.w_groupBoxExtract).BeginInit();
		this.w_groupBoxExtract.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.w_comboSep.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.w_radioReturnCounts.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.w_radioReturnFirstMatch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.w_radioReturnAllMatches.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.w_groupBoxFormat).BeginInit();
		this.w_groupBoxFormat.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.w_radioFormatOuterXml.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.w_radioFormatText.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.w_radioFormatInnerXml.Properties).BeginInit();
		base.SuspendLayout();
		this.w_groupBoxExtract.Controls.Add(this.w_comboSep);
		this.w_groupBoxExtract.Controls.Add(this.w_lblSeparator);
		this.w_groupBoxExtract.Controls.Add(this.w_radioReturnCounts);
		this.w_groupBoxExtract.Controls.Add(this.w_radioReturnFirstMatch);
		this.w_groupBoxExtract.Controls.Add(this.w_radioReturnAllMatches);
		this.w_groupBoxExtract.Location = new System.Drawing.Point(3, 1);
		this.w_groupBoxExtract.Name = "w_groupBoxExtract";
		this.w_groupBoxExtract.Size = new System.Drawing.Size(134, 112);
		this.w_groupBoxExtract.TabIndex = 33;
		this.w_groupBoxExtract.Text = "Extract";
		this.w_comboSep.Location = new System.Drawing.Point(78, 87);
		this.w_comboSep.Name = "w_comboSep";
		this.w_comboSep.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.w_comboSep.Size = new System.Drawing.Size(51, 20);
		this.w_comboSep.TabIndex = 4;
		this.w_lblSeparator.ImeMode = System.Windows.Forms.ImeMode.NoControl;
		this.w_lblSeparator.Location = new System.Drawing.Point(20, 90);
		this.w_lblSeparator.Name = "w_lblSeparator";
		this.w_lblSeparator.Size = new System.Drawing.Size(48, 13);
		this.w_lblSeparator.TabIndex = 3;
		this.w_lblSeparator.Text = "Separator";
		this.w_radioReturnCounts.ImeMode = System.Windows.Forms.ImeMode.NoControl;
		this.w_radioReturnCounts.Location = new System.Drawing.Point(6, 23);
		this.w_radioReturnCounts.Name = "w_radioReturnCounts";
		this.w_radioReturnCounts.Properties.AutoWidth = true;
		this.w_radioReturnCounts.Properties.Caption = "Counts only";
		this.w_radioReturnCounts.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
		this.w_radioReturnCounts.Properties.RadioGroupIndex = 1;
		this.w_radioReturnCounts.Size = new System.Drawing.Size(80, 19);
		this.w_radioReturnCounts.TabIndex = 0;
		this.w_radioReturnCounts.TabStop = false;
		this.w_radioReturnCounts.CheckedChanged += new System.EventHandler(On_ReturnDetail_CheckedChanged);
		this.w_radioReturnFirstMatch.EditValue = true;
		this.w_radioReturnFirstMatch.ImeMode = System.Windows.Forms.ImeMode.NoControl;
		this.w_radioReturnFirstMatch.Location = new System.Drawing.Point(6, 43);
		this.w_radioReturnFirstMatch.Name = "w_radioReturnFirstMatch";
		this.w_radioReturnFirstMatch.Properties.AutoWidth = true;
		this.w_radioReturnFirstMatch.Properties.Caption = "First match only";
		this.w_radioReturnFirstMatch.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
		this.w_radioReturnFirstMatch.Properties.RadioGroupIndex = 1;
		this.w_radioReturnFirstMatch.Size = new System.Drawing.Size(99, 19);
		this.w_radioReturnFirstMatch.TabIndex = 1;
		this.w_radioReturnFirstMatch.CheckedChanged += new System.EventHandler(On_ReturnDetail_CheckedChanged);
		this.w_radioReturnAllMatches.ImeMode = System.Windows.Forms.ImeMode.NoControl;
		this.w_radioReturnAllMatches.Location = new System.Drawing.Point(6, 65);
		this.w_radioReturnAllMatches.Name = "w_radioReturnAllMatches";
		this.w_radioReturnAllMatches.Properties.AutoWidth = true;
		this.w_radioReturnAllMatches.Properties.Caption = "All matching results";
		this.w_radioReturnAllMatches.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
		this.w_radioReturnAllMatches.Properties.RadioGroupIndex = 1;
		this.w_radioReturnAllMatches.Size = new System.Drawing.Size(115, 19);
		this.w_radioReturnAllMatches.TabIndex = 2;
		this.w_radioReturnAllMatches.TabStop = false;
		this.w_radioReturnAllMatches.CheckedChanged += new System.EventHandler(On_ReturnDetail_CheckedChanged);
		this.w_groupBoxFormat.Controls.Add(this.w_radioFormatOuterXml);
		this.w_groupBoxFormat.Controls.Add(this.w_radioFormatText);
		this.w_groupBoxFormat.Controls.Add(this.w_radioFormatInnerXml);
		this.w_groupBoxFormat.Location = new System.Drawing.Point(143, 1);
		this.w_groupBoxFormat.Name = "w_groupBoxFormat";
		this.w_groupBoxFormat.Size = new System.Drawing.Size(96, 112);
		this.w_groupBoxFormat.TabIndex = 34;
		this.w_groupBoxFormat.Text = "Format";
		this.w_radioFormatOuterXml.ImeMode = System.Windows.Forms.ImeMode.NoControl;
		this.w_radioFormatOuterXml.Location = new System.Drawing.Point(7, 66);
		this.w_radioFormatOuterXml.Name = "w_radioFormatOuterXml";
		this.w_radioFormatOuterXml.Properties.AutoWidth = true;
		this.w_radioFormatOuterXml.Properties.Caption = "Outer XML";
		this.w_radioFormatOuterXml.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
		this.w_radioFormatOuterXml.Properties.RadioGroupIndex = 2;
		this.w_radioFormatOuterXml.Size = new System.Drawing.Size(73, 19);
		this.w_radioFormatOuterXml.TabIndex = 34;
		this.w_radioFormatOuterXml.TabStop = false;
		this.w_radioFormatText.EditValue = true;
		this.w_radioFormatText.ImeMode = System.Windows.Forms.ImeMode.NoControl;
		this.w_radioFormatText.Location = new System.Drawing.Point(7, 24);
		this.w_radioFormatText.Name = "w_radioFormatText";
		this.w_radioFormatText.Properties.AutoWidth = true;
		this.w_radioFormatText.Properties.Caption = "Text";
		this.w_radioFormatText.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
		this.w_radioFormatText.Properties.RadioGroupIndex = 2;
		this.w_radioFormatText.Size = new System.Drawing.Size(45, 19);
		this.w_radioFormatText.TabIndex = 32;
		this.w_radioFormatInnerXml.ImeMode = System.Windows.Forms.ImeMode.NoControl;
		this.w_radioFormatInnerXml.Location = new System.Drawing.Point(7, 44);
		this.w_radioFormatInnerXml.Name = "w_radioFormatInnerXml";
		this.w_radioFormatInnerXml.Properties.AutoWidth = true;
		this.w_radioFormatInnerXml.Properties.Caption = "Inner XML";
		this.w_radioFormatInnerXml.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
		this.w_radioFormatInnerXml.Properties.RadioGroupIndex = 2;
		this.w_radioFormatInnerXml.Size = new System.Drawing.Size(71, 19);
		this.w_radioFormatInnerXml.TabIndex = 33;
		this.w_radioFormatInnerXml.TabStop = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.w_groupBoxFormat);
		base.Controls.Add(this.w_groupBoxExtract);
		base.Name = "ExtractXPathControl";
		base.Size = new System.Drawing.Size(245, 117);
		((System.ComponentModel.ISupportInitialize)this.w_groupBoxExtract).EndInit();
		this.w_groupBoxExtract.ResumeLayout(false);
		this.w_groupBoxExtract.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.w_comboSep.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.w_radioReturnCounts.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.w_radioReturnFirstMatch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.w_radioReturnAllMatches.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.w_groupBoxFormat).EndInit();
		this.w_groupBoxFormat.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.w_radioFormatOuterXml.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.w_radioFormatText.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.w_radioFormatInnerXml.Properties).EndInit();
		base.ResumeLayout(false);
	}

        private void On_ReturnDetail_CheckedChanged(object sender, EventArgs e)
	{
		UpdateSeparator();
	}

        private void UpdateSeparator()
	{
		w_comboSep.Enabled = w_radioReturnAllMatches.Checked;
	}
    }
}
