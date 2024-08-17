using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using Metalogix.Data.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Data.Filters
{
    public partial class FilterExpressionEditorAndTypeSelectorDialog : XtraForm
    {
        private readonly Dictionary<Type, string> _availableTypes;

        private List<CheckEdit> _appliesList = new List<CheckEdit>();

        private bool m_bTitleManuallySet;

        private IContainer components;

        private FilterExpressionEditorControl w_filterExpressionEditor;

        private SimpleButton w_bOkay;

        private SimpleButton w_bCancel;

        private LabelControl w_lblApplyToTypes;

        private LayoutControlGroup _appliesToLayoutGroup;

        private LayoutControl _appliesToLayout;

        public FilterBuilderType FilterableTypes
        {
            get
            {
                return this.w_filterExpressionEditor.FilterableTypes;
            }
            set
            {
                this.w_filterExpressionEditor.FilterableTypes = value;
                string str = "";
                int num = 0;
                foreach (Type objectType in value.ObjectTypes)
                {
                    num++;
                    str = string.Concat(str, this._availableTypes[objectType]);
                    if (num >= value.ObjectTypes.Count)
                    {
                        continue;
                    }
                    str = string.Concat(str, ", ");
                }
                this.w_filterExpressionEditor.LabelText = string.Concat("Filter ", str);
                if (!this.m_bTitleManuallySet)
                {
                    this.Text = string.Concat("Filter ", str);
                }
            }
        }

        public IFilterExpression FilterExpression
        {
            get
            {
                return this.w_filterExpressionEditor.FilterExpression;
            }
            set
            {
                this.w_filterExpressionEditor.FilterExpression = value;
            }
        }

        public string Title
        {
            get
            {
                return this.Text;
            }
            set
            {
                this.Text = value;
                this.m_bTitleManuallySet = true;
            }
        }

        public FilterExpressionEditorAndTypeSelectorDialog(Dictionary<Type, string> applyToTypes, List<string> checkedTypes = null)
        {
            this.InitializeComponent();
            this._availableTypes = applyToTypes;
            this.UpdateApplyToTypes(checkedTypes);
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
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(FilterExpressionEditorAndTypeSelectorDialog));
            this.w_bOkay = new SimpleButton();
            this.w_bCancel = new SimpleButton();
            this.w_lblApplyToTypes = new LabelControl();
            this.w_filterExpressionEditor = new FilterExpressionEditorControl();
            this._appliesToLayoutGroup = new LayoutControlGroup();
            this._appliesToLayout = new LayoutControl();
            ((ISupportInitialize)this._appliesToLayoutGroup).BeginInit();
            ((ISupportInitialize)this._appliesToLayout).BeginInit();
            base.SuspendLayout();
            componentResourceManager.ApplyResources(this.w_bOkay, "w_bOkay");
            this.w_bOkay.DialogResult = DialogResult.OK;
            this.w_bOkay.Name = "w_bOkay";
            componentResourceManager.ApplyResources(this.w_bCancel, "w_bCancel");
            this.w_bCancel.DialogResult = DialogResult.Cancel;
            this.w_bCancel.Name = "w_bCancel";
            componentResourceManager.ApplyResources(this.w_lblApplyToTypes, "w_lblApplyToTypes");
            this.w_lblApplyToTypes.Name = "w_lblApplyToTypes";
            componentResourceManager.ApplyResources(this.w_filterExpressionEditor, "w_filterExpressionEditor");
            this.w_filterExpressionEditor.FilterExpression = null;
            this.w_filterExpressionEditor.LabelText = "";
            this.w_filterExpressionEditor.Name = "w_filterExpressionEditor";
            componentResourceManager.ApplyResources(this._appliesToLayoutGroup, "_appliesToLayoutGroup");
            this._appliesToLayoutGroup.EnableIndentsWithoutBorders = DefaultBoolean.True;
            this._appliesToLayoutGroup.Location = new Point(0, 0);
            this._appliesToLayoutGroup.Name = "_appliesToLayoutGroup";
            this._appliesToLayoutGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this._appliesToLayoutGroup.Size = new Size(173, 369);
            this._appliesToLayoutGroup.TextVisible = false;
            componentResourceManager.ApplyResources(this._appliesToLayout, "_appliesToLayout");
            this._appliesToLayout.Name = "_appliesToLayout";
            this._appliesToLayout.Root = this._appliesToLayoutGroup;
            base.AcceptButton = this.w_bOkay;
            componentResourceManager.ApplyResources(this, "$this");
            base.AutoScaleMode = AutoScaleMode.Font;
            base.CancelButton = this.w_bCancel;
            base.Controls.Add(this._appliesToLayout);
            base.Controls.Add(this.w_lblApplyToTypes);
            base.Controls.Add(this.w_bOkay);
            base.Controls.Add(this.w_bCancel);
            base.Controls.Add(this.w_filterExpressionEditor);
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "FilterExpressionEditorAndTypeSelectorDialog";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            ((ISupportInitialize)this._appliesToLayoutGroup).EndInit();
            ((ISupportInitialize)this._appliesToLayout).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void OnCheckChanged(object sender, EventArgs e)
        {
            this.FilterableTypes = new FilterBuilderType((
                from c in this._appliesList
                where c.Checked
                select (Type)c.Tag).ToList<Type>());
        }

        private void UpdateApplyToTypes(List<string> checkedTypes)
        {
            if (this._availableTypes != null)
            {
                this._appliesList = this._availableTypes.Select<KeyValuePair<Type, string>, CheckEdit>((KeyValuePair<Type, string> kvp) => {
                    CheckEdit checkEdit1 = new CheckEdit()
                    {
                        Text = kvp.Value,
                        Tag = kvp.Key,
                        Checked = (checkedTypes == null ? true : checkedTypes.Contains(kvp.Key.FullName))
                    };
                    checkEdit1.CheckedChanged += new EventHandler(this.OnCheckChanged);
                    return checkEdit1;
                }).ToList<CheckEdit>();
                this._appliesList.ForEach((CheckEdit checkEdit) => this._appliesToLayout.AddItem().Control = checkEdit);
                this.FilterableTypes = new FilterBuilderType((
                    from c in this._appliesList
                    where c.Checked
                    select (Type)c.Tag).ToList<Type>());
            }
        }
    }
}