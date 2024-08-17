using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using Metalogix;
using Metalogix.Data.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Data.Filters
{
    public partial class FilterExpressionEditorDialog : XtraForm
    {
        private bool m_bTitleManuallySet;

        private IContainer components;

        private FilterExpressionEditorControl w_filterExpressionEditor;

        private SimpleButton w_bOkay;

        private SimpleButton w_bCancel;

        public FilterBuilderType FilterableTypes
        {
            get
            {
                return this.w_filterExpressionEditor.FilterableTypes;
            }
            set
            {
                this.w_filterExpressionEditor.FilterableTypes = value;
                if (!this.m_bTitleManuallySet)
                {
                    string str = "";
                    int num = 0;
                    foreach (Type objectType in value.ObjectTypes)
                    {
                        str = string.Concat(str, ActionUtils.GetTypePluralizedName(objectType));
                        if (num >= value.ObjectTypes.Count)
                        {
                            continue;
                        }
                        str = string.Concat(str, ", ");
                    }
                    char[] chrArray = new char[] { ',', ' ' };
                    this.Text = string.Concat("Filter ", str.TrimEnd(chrArray));
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

        public string LabelText
        {
            get
            {
                return this.w_filterExpressionEditor.LabelText;
            }
            set
            {
                this.w_filterExpressionEditor.LabelText = value;
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

        public FilterExpressionEditorDialog(bool isBasicMode = false)
        {
            this.InitializeComponent();
            if (isBasicMode)
            {
                this.ApplyBasicModeSkin();
            }
        }

        private void ApplyBasicModeSkin()
        {
            this.w_filterExpressionEditor.ApplyBasicModeSkin();
            this.w_bOkay.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
            this.w_bOkay.LookAndFeel.UseDefaultLookAndFeel = false;
            this.w_bCancel.LookAndFeel.SkinName = "Metalogix 2017 Explicit";
            this.w_bCancel.LookAndFeel.UseDefaultLookAndFeel = false;
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
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(FilterExpressionEditorDialog));
            this.w_bOkay = new SimpleButton();
            this.w_bCancel = new SimpleButton();
            this.w_filterExpressionEditor = new FilterExpressionEditorControl();
            base.SuspendLayout();
            componentResourceManager.ApplyResources(this.w_bOkay, "w_bOkay");
            this.w_bOkay.DialogResult = DialogResult.OK;
            this.w_bOkay.Name = "w_bOkay";
            componentResourceManager.ApplyResources(this.w_bCancel, "w_bCancel");
            this.w_bCancel.DialogResult = DialogResult.Cancel;
            this.w_bCancel.Name = "w_bCancel";
            componentResourceManager.ApplyResources(this.w_filterExpressionEditor, "w_filterExpressionEditor");
            this.w_filterExpressionEditor.FilterExpression = null;
            this.w_filterExpressionEditor.LabelText = "";
            this.w_filterExpressionEditor.Name = "w_filterExpressionEditor";
            base.AcceptButton = this.w_bOkay;
            componentResourceManager.ApplyResources(this, "$this");
            base.AutoScaleMode = AutoScaleMode.Font;
            base.CancelButton = this.w_bCancel;
            base.Controls.Add(this.w_bOkay);
            base.Controls.Add(this.w_bCancel);
            base.Controls.Add(this.w_filterExpressionEditor);
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "FilterExpressionEditorDialog";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            base.ResumeLayout(false);
        }
    }
}