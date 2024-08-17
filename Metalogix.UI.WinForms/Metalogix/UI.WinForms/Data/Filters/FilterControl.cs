using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using Metalogix.Data.Filters;
using Metalogix.DataStructures.Generic;
using Metalogix.UI.WinForms.Data;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Data.Filters
{
    public partial class FilterControl : FilterControlParent
    {
        public static OperandTranslationDictionary Translator;

        private IFilterExpression m_filters;

        private string m_sTypeHeader = "Object";

        private bool _suspendEvent;

        private IContainer components;

        private CheckEdit w_cbFilter;

        private SimpleButton w_btnEdit;

        private MemoEdit w_tbFilterDisplay;

        public IFilterExpression Filters
        {
            get
            {
                return this.m_filters;
            }
            set
            {
                this.m_filters = value;
                this.UpdateFilterUI();
            }
        }

        public bool IsFiltered
        {
            get
            {
                return this.w_cbFilter.Checked;
            }
            set
            {
                this.w_cbFilter.Checked = value;
            }
        }

        public string TypeHeader
        {
            get
            {
                return this.m_sTypeHeader;
            }
            set
            {
                this.w_cbFilter.Text = this.w_cbFilter.Text.Replace(this.m_sTypeHeader, value);
                CheckEdit wCbFilter = this.w_cbFilter;
                int width = this.w_cbFilter.PreferredSize.Width;
                Size size = this.w_cbFilter.Size;
                wCbFilter.Size = new Size(width, size.Height);
                MemoEdit wTbFilterDisplay = this.w_tbFilterDisplay;
                Point location = this.w_cbFilter.Location;
                Point point = this.w_tbFilterDisplay.Location;
                wTbFilterDisplay.Location = new Point(location.X + this.w_cbFilter.Width + 5, point.Y);
                MemoEdit memoEdit = this.w_tbFilterDisplay;
                int x = this.w_btnEdit.Location.X;
                Point location1 = this.w_tbFilterDisplay.Location;
                Size size1 = this.w_tbFilterDisplay.Size;
                memoEdit.Size = new Size(x - location1.X - 5, size1.Height);
                this.m_sTypeHeader = value;
            }
        }

        static FilterControl()
        {
            FilterControl.Translator = new OperandTranslationDictionary();
        }

        public FilterControl()
        {
            this.InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private string GetExclusionString()
        {
            if (this.Filters == null || this.Filters is FilterExpressionList && ((FilterExpressionList)this.Filters).Count == 0)
            {
                return "";
            }
            StringBuilder stringBuilder = new StringBuilder();
            base.BuildExclusionString(this.Filters, stringBuilder);
            return stringBuilder.ToString();
        }

        private void InitializeComponent()
        {
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(FilterControl));
            this.w_cbFilter = new CheckEdit();
            this.w_btnEdit = new SimpleButton();
            this.w_tbFilterDisplay = new MemoEdit();
            ((ISupportInitialize)this.w_cbFilter.Properties).BeginInit();
            ((ISupportInitialize)this.w_tbFilterDisplay.Properties).BeginInit();
            base.SuspendLayout();
            componentResourceManager.ApplyResources(this.w_cbFilter, "w_cbFilter");
            this.w_cbFilter.Name = "w_cbFilter";
            this.w_cbFilter.Properties.AutoWidth = true;
            this.w_cbFilter.Properties.Caption = componentResourceManager.GetString("w_cbFilter.Properties.Caption");
            this.w_cbFilter.CheckedChanged += new EventHandler(this.On_Filters_Checked);
            componentResourceManager.ApplyResources(this.w_btnEdit, "w_btnEdit");
            this.w_btnEdit.Appearance.Font = (Font)componentResourceManager.GetObject("w_btnEdit.Appearance.Font");
            this.w_btnEdit.Appearance.Options.UseFont = true;
            this.w_btnEdit.Name = "w_btnEdit";
            this.w_btnEdit.Click += new EventHandler(this.On_btnEditFilters_Clicked);
            componentResourceManager.ApplyResources(this.w_tbFilterDisplay, "w_tbFilterDisplay");
            this.w_tbFilterDisplay.Name = "w_tbFilterDisplay";
            this.w_tbFilterDisplay.Properties.ReadOnly = true;
            componentResourceManager.ApplyResources(this, "$this");
            base.AutoScaleMode = AutoScaleMode.Font;
            base.Controls.Add(this.w_tbFilterDisplay);
            base.Controls.Add(this.w_cbFilter);
            base.Controls.Add(this.w_btnEdit);
            base.Name = "FilterControl";
            ((ISupportInitialize)this.w_cbFilter.Properties).EndInit();
            ((ISupportInitialize)this.w_tbFilterDisplay.Properties).EndInit();
            base.ResumeLayout(false);
        }

        public virtual void LoadUI()
        {
            this.UpdateFilterUI();
        }

        private void On_btnEditFilters_Clicked(object sender, EventArgs e)
        {
            FilterExpressionEditorDialog filterExpressionEditorDialog = new FilterExpressionEditorDialog(false)
            {
                FilterableTypes = new FilterBuilderType(this.TypeFilters, this.AllowFreeFormFilterEntry)
            };
            if (this.m_filters != null)
            {
                filterExpressionEditorDialog.FilterExpression = this.m_filters;
            }
            filterExpressionEditorDialog.ShowDialog();
            if (filterExpressionEditorDialog.DialogResult == DialogResult.OK)
            {
                this.Filters = filterExpressionEditorDialog.FilterExpression;
            }
        }

        private void On_Filters_Checked(object sender, EventArgs e)
        {
            if (!this._suspendEvent)
            {
                this._suspendEvent = true;
                CancelEventArgs cancelEventArg = new CancelEventArgs();
                this.OnCheckedChanged(cancelEventArg);
                if (!cancelEventArg.Cancel)
                {
                    this.UpdateFilterUI();
                }
                else
                {
                    this.w_cbFilter.Checked = !this.w_cbFilter.Checked;
                }
                this._suspendEvent = false;
            }
        }

        protected virtual void OnCheckedChanged(CancelEventArgs e)
        {
            if (this.CheckedChanged != null)
            {
                this.CheckedChanged(this, e);
            }
        }

        public virtual void SaveUI()
        {
        }

        private void UpdateFilterUI()
        {
            this.w_btnEdit.Enabled = this.w_cbFilter.Checked;
            this.w_tbFilterDisplay.Enabled = this.w_cbFilter.Checked;
            if (this.Filters == null)
            {
                this.w_tbFilterDisplay.Text = "";
                this.w_tbFilterDisplay.Properties.ScrollBars = ScrollBars.None;
                return;
            }
            MemoEdit wTbFilterDisplay = this.w_tbFilterDisplay;
            string exclusionString = this.GetExclusionString();
            char[] chrArray = new char[] { '\n' };
            wTbFilterDisplay.Lines = exclusionString.Split(chrArray);
            Size preferredSize = this.w_tbFilterDisplay.PreferredSize;
            bool height = preferredSize.Height > this.w_tbFilterDisplay.Height;
            this.w_tbFilterDisplay.Properties.ScrollBars = (height ? ScrollBars.Vertical : ScrollBars.None);
        }

        public event CheckedChangedEventHandler CheckedChanged;
    }
}