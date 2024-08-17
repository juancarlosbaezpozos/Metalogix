using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.Data.Filters;
using Metalogix.Explorer;
using Metalogix.SharePoint.Options.Migration.Mapping;
using Metalogix.UI.WinForms.Data;
using Metalogix.UI.WinForms.Data.Filters;

namespace Metalogix.SharePoint.UI.WinForms.Migration.BasicView
{
	public class FilterControlBasicView : FilterControlParent
	{
		private string _typeHeader = "Object";

		private IFilterExpression _filters;

		private IFilterExpression _siteFieldsFilter;

		private IContainer components;

		private ToggleSwitch tsApplyFilter;

		private SimpleButton btnFilter;

		private RichTextBox rtbFilter;

		private PanelControl pnlFilter;

		public ColumnMappings CurrentColumnMappings { get; set; }

		public IFilterExpression CurrentFilter { get; set; }

		public IFilterExpression Filters
		{
			get
			{
				return _filters;
			}
			set
			{
				_filters = value;
				UpdateFilterUi();
			}
		}

		public bool IsEventSuppresed { get; set; }

		public bool IsFiltered
		{
			get
			{
				return tsApplyFilter.IsOn;
			}
			set
			{
				tsApplyFilter.IsOn = value;
			}
		}

		public bool IsFilterExpressionEditorDialog { get; set; }

		public IFilterExpression SiteFieldsFilter
		{
			get
			{
				return _siteFieldsFilter;
			}
			set
			{
				_siteFieldsFilter = value;
				if (_siteFieldsFilter != null)
				{
					Filters = _siteFieldsFilter;
					UpdateFilterUi();
				}
			}
		}

		public SPList SourceList { get; set; }

		public NodeCollection SourceNodes { get; set; }

		public SPWeb SourceWeb { get; set; }

		public string TypeHeader
		{
			get
			{
				return _typeHeader;
			}
			set
			{
				tsApplyFilter.Properties.OffText = tsApplyFilter.Properties.OffText.Replace(_typeHeader, value);
				tsApplyFilter.Properties.OnText = tsApplyFilter.Properties.OnText.Replace(_typeHeader, value);
				_typeHeader = value;
			}
		}

		public event ToggleSwitchedEventHandler ToggleSwitched;

		public FilterControlBasicView()
		{
			InitializeComponent();
		}

		private void btnFilter_Click(object sender, EventArgs e)
		{
			if (IsFilterExpressionEditorDialog)
			{
				OpenFilterExpressionEditorDialog();
				return;
			}
			if (TypeHeader.Contains("List"))
			{
				OpenListColumnMappingDialog();
			}
			if (TypeHeader.Contains("Site"))
			{
				OpenSiteColumnMappingDialog();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private string GetExclusionString()
		{
			if (Filters == null || (Filters is FilterExpressionList && ((FilterExpressionList)Filters).Count == 0))
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			BuildExclusionString(Filters, stringBuilder);
			return stringBuilder.ToString();
		}

		private void InitializeComponent()
		{
			this.tsApplyFilter = new DevExpress.XtraEditors.ToggleSwitch();
			this.btnFilter = new DevExpress.XtraEditors.SimpleButton();
			this.rtbFilter = new System.Windows.Forms.RichTextBox();
			this.pnlFilter = new DevExpress.XtraEditors.PanelControl();
			((System.ComponentModel.ISupportInitialize)this.tsApplyFilter.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.pnlFilter).BeginInit();
			this.pnlFilter.SuspendLayout();
			base.SuspendLayout();
			this.tsApplyFilter.Location = new System.Drawing.Point(2, 11);
			this.tsApplyFilter.Name = "tsApplyFilter";
			this.tsApplyFilter.Properties.Appearance.Options.UseTextOptions = true;
			this.tsApplyFilter.Properties.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			this.tsApplyFilter.Properties.OffText = "   Apply filter on Object";
			this.tsApplyFilter.Properties.OnText = "   Apply filter on Object";
			this.tsApplyFilter.Size = new System.Drawing.Size(254, 24);
			this.tsApplyFilter.TabIndex = 1;
			this.tsApplyFilter.Toggled += new System.EventHandler(tsApplyFilter_Toggled);
			this.btnFilter.AppearanceDisabled.BackColor = System.Drawing.SystemColors.ControlDark;
			this.btnFilter.AppearanceDisabled.BackColor2 = System.Drawing.SystemColors.ControlDark;
			this.btnFilter.AppearanceDisabled.BorderColor = System.Drawing.SystemColors.ControlDark;
			this.btnFilter.AppearanceDisabled.Options.UseBackColor = true;
			this.btnFilter.AppearanceDisabled.Options.UseBorderColor = true;
			this.btnFilter.Enabled = false;
			this.btnFilter.Location = new System.Drawing.Point(573, 12);
			this.btnFilter.Name = "btnFilter";
			this.btnFilter.Size = new System.Drawing.Size(23, 23);
			this.btnFilter.TabIndex = 2;
			this.btnFilter.Text = "...";
			this.btnFilter.Click += new System.EventHandler(btnFilter_Click);
			this.rtbFilter.BackColor = System.Drawing.SystemColors.Window;
			this.rtbFilter.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.rtbFilter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbFilter.Enabled = false;
			this.rtbFilter.Location = new System.Drawing.Point(2, 2);
			this.rtbFilter.Name = "rtbFilter";
			this.rtbFilter.ReadOnly = true;
			this.rtbFilter.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.rtbFilter.Size = new System.Drawing.Size(290, 36);
			this.rtbFilter.TabIndex = 0;
			this.rtbFilter.TabStop = false;
			this.rtbFilter.Text = "";
			this.rtbFilter.WordWrap = false;
			this.pnlFilter.Appearance.BackColor = System.Drawing.Color.FromArgb(171, 171, 171);
			this.pnlFilter.Appearance.BackColor2 = System.Drawing.Color.FromArgb(171, 171, 171);
			this.pnlFilter.Appearance.BorderColor = System.Drawing.Color.FromArgb(171, 171, 171);
			this.pnlFilter.Appearance.Options.UseBackColor = true;
			this.pnlFilter.Appearance.Options.UseBorderColor = true;
			this.pnlFilter.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
			this.pnlFilter.Controls.Add(this.rtbFilter);
			this.pnlFilter.Location = new System.Drawing.Point(267, 4);
			this.pnlFilter.Name = "pnlFilter";
			this.pnlFilter.Size = new System.Drawing.Size(294, 40);
			this.pnlFilter.TabIndex = 4;
			this.pnlFilter.TabStop = true;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.pnlFilter);
			base.Controls.Add(this.btnFilter);
			base.Controls.Add(this.tsApplyFilter);
			base.Name = "FilterControlBasicView";
			base.Size = new System.Drawing.Size(613, 50);
			((System.ComponentModel.ISupportInitialize)this.tsApplyFilter.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.pnlFilter).EndInit();
			this.pnlFilter.ResumeLayout(false);
			base.ResumeLayout(false);
		}

		private void OnToggleSwitched(CancelEventArgs e)
		{
			if (this.ToggleSwitched != null)
			{
				this.ToggleSwitched(this, e);
			}
		}

		private void OpenFilterExpressionEditorDialog()
		{
			FilterExpressionEditorDialog filterExpressionEditorDialog = new FilterExpressionEditorDialog(isBasicMode: true)
			{
				FilterableTypes = new FilterBuilderType(TypeFilters, AllowFreeFormFilterEntry)
			};
			if (_filters != null)
			{
				filterExpressionEditorDialog.FilterExpression = _filters;
			}
			filterExpressionEditorDialog.ShowDialog();
			if (filterExpressionEditorDialog.DialogResult == DialogResult.OK)
			{
				Filters = filterExpressionEditorDialog.FilterExpression;
			}
		}

		private void OpenListColumnMappingDialog()
		{
			if (SourceList == null)
			{
				return;
			}
			ColumnFilteringConfigDialog columnFilteringConfigDialog = new ColumnFilteringConfigDialog(SourceList, isBasicMode: true);
			if (CurrentColumnMappings != null && CurrentColumnMappings.FieldsFilter != null)
			{
				columnFilteringConfigDialog.FilterExpression = CurrentColumnMappings.FieldsFilter;
			}
			if (columnFilteringConfigDialog.ShowDialog() == DialogResult.OK)
			{
				if (CurrentColumnMappings == null)
				{
					CurrentColumnMappings = new ColumnMappings();
				}
				CurrentColumnMappings.FieldsFilter = columnFilteringConfigDialog.FilterExpression;
				CurrentFilter = CurrentColumnMappings.FieldsFilter;
				Filters = columnFilteringConfigDialog.FilterExpression;
				UpdateFilterUi();
			}
		}

		private void OpenSiteColumnMappingDialog()
		{
			if (SourceWeb != null)
			{
				ColumnFilteringConfigDialog columnFilteringConfigDialog = new ColumnFilteringConfigDialog(SourceNodes, isBasicMode: true)
				{
					FilterExpression = SiteFieldsFilter
				};
				if (columnFilteringConfigDialog.ShowDialog() == DialogResult.OK)
				{
					SiteFieldsFilter = columnFilteringConfigDialog.FilterExpression;
					Filters = SiteFieldsFilter;
					UpdateFilterUi();
				}
			}
		}

		private void tsApplyFilter_Toggled(object sender, EventArgs e)
		{
			if (IsEventSuppresed)
			{
				return;
			}
			UpdateFilterUi();
			if (IsFilterExpressionEditorDialog)
			{
				CancelEventArgs cancelEventArgs = new CancelEventArgs();
				OnToggleSwitched(cancelEventArgs);
				if (cancelEventArgs.Cancel)
				{
					tsApplyFilter.IsOn = !tsApplyFilter.IsOn;
				}
			}
		}

		private void UpdateFilterUi()
		{
			btnFilter.Enabled = tsApplyFilter.IsOn;
			rtbFilter.Enabled = tsApplyFilter.IsOn;
			if (Filters == null)
			{
				rtbFilter.Text = string.Empty;
				return;
			}
			RichTextBox richTextBox = rtbFilter;
			string exclusionString = GetExclusionString();
			char[] separator = new char[1] { '\n' };
			richTextBox.Lines = exclusionString.Split(separator);
		}
	}
}
