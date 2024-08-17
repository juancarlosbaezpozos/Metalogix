using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Metalogix.Actions;
using Metalogix.SharePoint.Migration;
using Metalogix.UI.WinForms.Components;
using Metalogix.UI.WinForms.Interfaces;

namespace Metalogix.UI.WinForms.Actions
{
    public partial class TemplatableTabsForm : LeftNavigableTabsForm
    {
        private const int TEMPLATE_WIDTH_OFFSET = 365;

        private bool _suppressBasicModeChanges;

        private ActionOptionsProvider _templateProvider = ActionOptionsProvider.TemplatesProvider;

        private bool _actionTemplatesSupported;

        private IContainer components;

        protected SimpleButton _loadActionTemplateButton;

        protected SimpleButton _saveActionTemplateButton;

        private SimpleButton btnBasicMode;

        protected override int AbsoluteMinimumWidth
        {
            get
            {
                int absoluteMinimumWidth = base.AbsoluteMinimumWidth;
                if (ActionTemplatesSupported)
                {
                    absoluteMinimumWidth += 365;
                }
                return absoluteMinimumWidth;
            }
        }

        public override Metalogix.Actions.Action Action
        {
            get
            {
                return base.Action;
            }
            set
            {
                base.Action = value;
                LoadUI(value);
            }
        }

        protected bool ActionTemplatesEnabled
        {
            get
            {
                if (!(base.ActionType != null) || TemplateProvider == null)
                {
                    return false;
                }
                return ActionTemplatesSupported;
            }
        }

        public bool ActionTemplatesSupported
        {
            get
            {
                return _actionTemplatesSupported;
            }
            set
            {
                _actionTemplatesSupported = value;
                UpdateActionTemplatesButtonVisibility();
            }
        }

        public virtual bool CanShowBasicModeButton => false;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ActionOptionsProvider TemplateProvider
        {
            get
            {
                return _templateProvider;
            }
            set
            {
                _templateProvider = value;
                UpdateActionTemplatesButtonVisibility();
            }
        }

        public TemplatableTabsForm()
        {
            InitializeComponent();
            UpdateActionTemplatesButtonVisibility();
        }

        private void _loadActionTemplateButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (TemplateProvider != null && ActionTemplatesEnabled)
                {
                    ActionTemplateSelector actionTemplateSelector = new ActionTemplateSelector(TemplateProvider, base.ActionType, Action.IsBasicModeAllowed && !CanShowBasicModeButton);
                    if (actionTemplateSelector.ShowDialog() == DialogResult.OK)
                    {
                        Metalogix.Actions.Action action = Metalogix.Actions.Action.CreateActionFromTypeName(base.ActionType.FullName);
                        actionTemplateSelector.SelectedTemplate.SetOptions(action);
                        _suppressBasicModeChanges = true;
                        LoadUI(action);
                    }
                }
            }
            catch (Exception exception)
            {
                GlobalServices.ErrorHandler.HandleException(exception);
            }
            finally
            {
                _suppressBasicModeChanges = false;
            }
        }

        private void _saveActionTemplateButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (TemplateProvider == null || !ActionTemplatesEnabled)
                {
                    return;
                }
                Metalogix.Actions.Action action = Metalogix.Actions.Action.CreateActionFromTypeName(base.ActionType.FullName);
                if (!SaveUI(action))
                {
                    return;
                }
                ActionOptionsTemplate[] optionsTemplatesForAction = TemplateProvider.GetOptionsTemplatesForAction(base.ActionType);
                IEnumerable<string> templateName = optionsTemplatesForAction.Select((ActionOptionsTemplate t) => t.TemplateName);
                List<string> strs = new List<string>(templateName);
                bool flag1 = false;
                string inputValue;
                bool flag;
                do
                {
                    InputBox inputBox = OpenSaveJobConfigurationDialog();
                    if (inputBox.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                    inputValue = inputBox.InputValue;
                    if (Action.IsBasicModeAllowed && !CanShowBasicModeButton)
                    {
                        inputValue = $"{inputValue}{UIUtils.SimplifiedMode}";
                    }
                    flag = !string.IsNullOrEmpty(inputValue) && !strs.Contains(inputValue);
                    if (!flag)
                    {
                        string str = $"A job configuration named \"{inputValue}\" already exists. Would you like to overwrite it?";
                        switch (FlatXtraMessageBox.Show(str, "Template Already Exists", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation))
                        {
                            case DialogResult.Yes:
                                flag1 = true;
                                break;
                            case DialogResult.Cancel:
                                return;
                        }
                    }
                }
                while (!flag && !flag1);
                if (flag1)
                {
                    ActionOptionsTemplate[] actionOptionsTemplateArray = optionsTemplatesForAction;
                    foreach (ActionOptionsTemplate actionOptionsTemplate in actionOptionsTemplateArray)
                    {
                        if (actionOptionsTemplate.TemplateName == inputValue)
                        {
                            actionOptionsTemplate.Delete();
                        }
                    }
                }
                TemplateProvider.CreateNewTemplate(inputValue, base.ActionType, action.Options.ToXML());
            }
            catch (Exception exception)
            {
                GlobalServices.ErrorHandler.HandleException(exception);
            }
        }

        private void btnBasicMode_Click(object sender, EventArgs e)
        {
            SwitchView(Action, isFromBasicView: true);
            HideBasicModeButton();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void HideBasicModeButton()
        {
            if (btnBasicMode.Visible)
            {
                btnBasicMode.Visible = false;
                SimpleButton point = _loadActionTemplateButton;
                Point location = _loadActionTemplateButton.Location;
                Point location1 = _loadActionTemplateButton.Location;
                point.Location = new Point(location.X - 90, location1.Y);
                SimpleButton simpleButton = _saveActionTemplateButton;
                Point point1 = _saveActionTemplateButton.Location;
                Point location2 = _saveActionTemplateButton.Location;
                simpleButton.Location = new Point(point1.X - 90, location2.Y);
                _loadActionTemplateButton.TabIndex--;
                _saveActionTemplateButton.TabIndex--;
            }
        }

        private void InitializeComponent()
        {
            this._loadActionTemplateButton = new DevExpress.XtraEditors.SimpleButton();
            this._saveActionTemplateButton = new DevExpress.XtraEditors.SimpleButton();
            this.btnBasicMode = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)base.tabControl).BeginInit();
            base.SuspendLayout();
            base.tabControl.LookAndFeel.SkinName = "Office 2013";
            base.tabControl.LookAndFeel.UseDefaultLookAndFeel = false;
            this._loadActionTemplateButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this._loadActionTemplateButton.Location = new System.Drawing.Point(12, 467);
            this._loadActionTemplateButton.Name = "_loadActionTemplateButton";
            this._loadActionTemplateButton.Size = new System.Drawing.Size(195, 23);
            this._loadActionTemplateButton.TabIndex = 1;
            this._loadActionTemplateButton.Text = "Open Existing Job Configuration";
            this._loadActionTemplateButton.Click += new System.EventHandler(_loadActionTemplateButton_Click);
            this._saveActionTemplateButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this._saveActionTemplateButton.Location = new System.Drawing.Point(222, 467);
            this._saveActionTemplateButton.Name = "_saveActionTemplateButton";
            this._saveActionTemplateButton.Size = new System.Drawing.Size(155, 23);
            this._saveActionTemplateButton.TabIndex = 2;
            this._saveActionTemplateButton.Text = "Save Job Configuration";
            this._saveActionTemplateButton.Click += new System.EventHandler(_saveActionTemplateButton_Click);
            this.btnBasicMode.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.btnBasicMode.Location = new System.Drawing.Point(392, 467);
            this.btnBasicMode.Name = "btnBasicMode";
            this.btnBasicMode.Size = new System.Drawing.Size(101, 23);
            this.btnBasicMode.TabIndex = 35;
            this.btnBasicMode.Text = "Simplified &Mode";
            this.btnBasicMode.Visible = false;
            this.btnBasicMode.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnBasicMode.Click += new System.EventHandler(btnBasicMode_Click);
            base.Appearance.BackColor = System.Drawing.Color.White;
            base.Appearance.Options.UseBackColor = true;
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.ClientSize = new System.Drawing.Size(784, 502);
            base.Controls.Add(this.btnBasicMode);
            base.Controls.Add(this._saveActionTemplateButton);
            base.Controls.Add(this._loadActionTemplateButton);
            base.LookAndFeel.SkinName = "Office 2013";
            base.Name = "TemplatableTabsForm";
            this.Text = "TemplatableTabsForm";
            base.Controls.SetChildIndex(base.w_btnOK, 0);
            base.Controls.SetChildIndex(base.w_btnCancel, 0);
            base.Controls.SetChildIndex(base.tabControl, 0);
            base.Controls.SetChildIndex(this._loadActionTemplateButton, 0);
            base.Controls.SetChildIndex(this._saveActionTemplateButton, 0);
            base.Controls.SetChildIndex(this.btnBasicMode, 0);
            ((System.ComponentModel.ISupportInitialize)base.tabControl).EndInit();
            base.ResumeLayout(false);
        }

        protected virtual void LoadUI(Metalogix.Actions.Action action)
        {
            if (base.TransformerTab != null)
            {
                foreach (TabbableControl tab in base.Tabs)
                {
                    if (tab is ITransformerTabConfig currentTransformerCollection)
                    {
                        currentTransformerCollection.Transformers = base.TransformerTab.CurrentTransformerCollection;
                    }
                }
            }
            if (!_suppressBasicModeChanges)
            {
                ShowBasicModeButton();
            }
        }

        private InputBox OpenSaveJobConfigurationDialog()
        {
            string str = "Enter a name for the configuration";
            if (Action.IsBasicModeAllowed && !CanShowBasicModeButton)
            {
                object[] newLine = new object[6]
                {
                    str,
                    Environment.NewLine,
                    Environment.NewLine,
                    "Simplified Mode",
                    Environment.NewLine,
                    UIUtils.SimplifiedMode
                };
                str = string.Format("{0}{1}{2}Note: You are saving a job configuration from {3}. {4}The configuration will have \"{5}\" appended to the end of its name.", newLine);
            }
            return new InputBox("Save Job Configuration", str, Action.IsBasicModeAllowed && !CanShowBasicModeButton, 50, 90);
        }

        protected override bool SaveUI()
        {
            return SaveUI(Action);
        }

        protected virtual bool SaveUI(Metalogix.Actions.Action action)
        {
            return AttemptSaveTabs();
        }

        private void ShowBasicModeButton()
        {
            if (Action != null && Action.IsBasicModeAllowed && CanShowBasicModeButton)
            {
                btnBasicMode.Visible = true;
                SimpleButton point = btnBasicMode;
                point.Location = new Point(12, btnBasicMode.Location.Y);
                SimpleButton simpleButton = _loadActionTemplateButton;
                Point location1 = _loadActionTemplateButton.Location;
                Point point1 = _loadActionTemplateButton.Location;
                simpleButton.Location = new Point(location1.X + 116, point1.Y);
                SimpleButton simpleButton1 = _saveActionTemplateButton;
                Point location2 = _saveActionTemplateButton.Location;
                Point point2 = _saveActionTemplateButton.Location;
                simpleButton1.Location = new Point(location2.X + 116, point2.Y);
                btnBasicMode.TabIndex = 1;
                _loadActionTemplateButton.TabIndex++;
                _saveActionTemplateButton.TabIndex++;
            }
        }

        protected virtual void SwitchView(Metalogix.Actions.Action action, bool isFromBasicView)
        {
        }

        private void UpdateActionTemplatesButtonVisibility()
        {
            if (!base.DesignMode)
            {
                _loadActionTemplateButton.Visible = ActionTemplatesEnabled;
                _saveActionTemplateButton.Visible = ActionTemplatesEnabled;
            }
        }
    }
}
