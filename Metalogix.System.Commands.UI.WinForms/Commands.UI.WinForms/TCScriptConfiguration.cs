using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix;
using Metalogix.Commands;
using Metalogix.Commands.UI.WinForms;
using Metalogix.Commands.UI.WinForms.Properties;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.Commands.UI.WinForms
{
    [ControlImage("Metalogix.Commands.UI.WinForms.Icons.windows_powershell_icon.png")]
    [ControlName("Script Options")]
    [UsesGroupBox(true)]
    public class TCScriptConfiguration : TabbableControl
    {
        public struct ScriptConfigurationOptions
        {
            public ScriptFileLocation Location;

            public string FileName;

            public string FullScript;
        }

        private ScriptConfigurationOptions _options;

        private IContainer components;

        private CheckEdit w_rbNone;

        private CheckEdit w_rbFromFile;

        private CheckEdit w_rbConfigure;

        private SimpleButton w_btnLoadScriptFromFile;

        private MemoEdit w_scriptEditor;

        private TextEdit w_textBoxScriptFileName;

        private TextEditContextMenu _textEditContextMenu;

        public override Image Image
        {
            get
            {
                return base.Image;
            }
            set
            {
                base.Image = value;
            }
        }

        public ScriptConfigurationOptions Options
        {
            get
            {
                return _options;
            }
            set
            {
                _options = value;
                LoadUI();
            }
        }

        public TCScriptConfiguration()
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
            this.w_rbNone = new global::DevExpress.XtraEditors.CheckEdit();
            this.w_rbFromFile = new global::DevExpress.XtraEditors.CheckEdit();
            this.w_rbConfigure = new global::DevExpress.XtraEditors.CheckEdit();
            this.w_btnLoadScriptFromFile = new global::DevExpress.XtraEditors.SimpleButton();
            this.w_scriptEditor = new global::DevExpress.XtraEditors.MemoEdit();
            this._textEditContextMenu = new global::Metalogix.UI.WinForms.TextEditContextMenu();
            this.w_textBoxScriptFileName = new global::DevExpress.XtraEditors.TextEdit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_rbNone.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_rbFromFile.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_rbConfigure.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_scriptEditor.Properties).BeginInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_textBoxScriptFileName.Properties).BeginInit();
            base.SuspendLayout();
            this.w_rbNone.Location = new global::System.Drawing.Point(2, 1);
            this.w_rbNone.Name = "w_rbNone";
            this.w_rbNone.Properties.AutoWidth = true;
            this.w_rbNone.Properties.Caption = "None";
            this.w_rbNone.Properties.CheckStyle = global::DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.w_rbNone.Properties.RadioGroupIndex = 1;
            this.w_rbNone.Size = new global::System.Drawing.Size(48, 19);
            this.w_rbNone.TabIndex = 0;
            this.w_rbNone.TabStop = false;
            this.w_rbNone.CheckedChanged += new global::System.EventHandler(On_None_CheckChanged);
            this.w_rbFromFile.Location = new global::System.Drawing.Point(2, 24);
            this.w_rbFromFile.Name = "w_rbFromFile";
            this.w_rbFromFile.Properties.AutoWidth = true;
            this.w_rbFromFile.Properties.Caption = "Use Script From File";
            this.w_rbFromFile.Properties.CheckStyle = global::DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.w_rbFromFile.Properties.RadioGroupIndex = 1;
            this.w_rbFromFile.Size = new global::System.Drawing.Size(117, 19);
            this.w_rbFromFile.TabIndex = 1;
            this.w_rbFromFile.TabStop = false;
            this.w_rbFromFile.CheckedChanged += new global::System.EventHandler(On_ScriptFromFile_CheckChanged);
            this.w_rbConfigure.Location = new global::System.Drawing.Point(2, 73);
            this.w_rbConfigure.Name = "w_rbConfigure";
            this.w_rbConfigure.Properties.AutoWidth = true;
            this.w_rbConfigure.Properties.Caption = "Use Configured Script";
            this.w_rbConfigure.Properties.CheckStyle = global::DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.w_rbConfigure.Properties.RadioGroupIndex = 1;
            this.w_rbConfigure.Size = new global::System.Drawing.Size(127, 19);
            this.w_rbConfigure.TabIndex = 2;
            this.w_rbConfigure.TabStop = false;
            this.w_rbConfigure.CheckedChanged += new global::System.EventHandler(On_ScriptFromConfiguration_CheckChanged);
            this.w_btnLoadScriptFromFile.Anchor = global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Right;
            this.w_btnLoadScriptFromFile.Enabled = false;
            this.w_btnLoadScriptFromFile.Image = global::Metalogix.Commands.UI.WinForms.Properties.Resources.FileSystem16;
            this.w_btnLoadScriptFromFile.Location = new global::System.Drawing.Point(339, 46);
            this.w_btnLoadScriptFromFile.Name = "w_btnLoadScriptFromFile";
            this.w_btnLoadScriptFromFile.Padding = new global::System.Windows.Forms.Padding(4);
            this.w_btnLoadScriptFromFile.Size = new global::System.Drawing.Size(24, 24);
            this.w_btnLoadScriptFromFile.TabIndex = 2;
            this.w_btnLoadScriptFromFile.Click += new global::System.EventHandler(On_SelectScriptFile_Clicked);
            this.w_scriptEditor.Anchor = global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Bottom | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right;
            this.w_scriptEditor.EditValue = "";
            this.w_scriptEditor.Enabled = false;
            this.w_scriptEditor.Location = new global::System.Drawing.Point(19, 98);
            this.w_scriptEditor.Name = "w_scriptEditor";
            this.w_scriptEditor.Properties.AcceptsTab = true;
            this.w_scriptEditor.Properties.ContextMenuStrip = this._textEditContextMenu;
            this.w_scriptEditor.Size = new global::System.Drawing.Size(351, 195);
            this.w_scriptEditor.TabIndex = 3;
            this._textEditContextMenu.Name = "TextEditContextMenu";
            this._textEditContextMenu.Size = new global::System.Drawing.Size(118, 148);
            this.w_textBoxScriptFileName.Anchor = global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right;
            this.w_textBoxScriptFileName.Enabled = false;
            this.w_textBoxScriptFileName.Location = new global::System.Drawing.Point(19, 48);
            this.w_textBoxScriptFileName.Name = "w_textBoxScriptFileName";
            this.w_textBoxScriptFileName.Properties.Appearance.BackColor = global::System.Drawing.Color.White;
            this.w_textBoxScriptFileName.Properties.Appearance.Options.UseBackColor = true;
            this.w_textBoxScriptFileName.Properties.ContextMenuStrip = this._textEditContextMenu;
            this.w_textBoxScriptFileName.Properties.ReadOnly = true;
            this.w_textBoxScriptFileName.Size = new global::System.Drawing.Size(314, 20);
            this.w_textBoxScriptFileName.TabIndex = 3;
            base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(this.w_btnLoadScriptFromFile);
            base.Controls.Add(this.w_scriptEditor);
            base.Controls.Add(this.w_textBoxScriptFileName);
            base.Controls.Add(this.w_rbConfigure);
            base.Controls.Add(this.w_rbFromFile);
            base.Controls.Add(this.w_rbNone);
            base.Name = "TCScriptConfiguration";
            base.Size = new global::System.Drawing.Size(382, 304);
            ((global::System.ComponentModel.ISupportInitialize)this.w_rbNone.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_rbFromFile.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_rbConfigure.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_scriptEditor.Properties).EndInit();
            ((global::System.ComponentModel.ISupportInitialize)this.w_textBoxScriptFileName.Properties).EndInit();
            base.ResumeLayout(false);
        }

        protected override void LoadUI()
        {
            switch (_options.Location)
            {
                default:
                    w_rbNone.Checked = true;
                    break;
                case ScriptFileLocation.File:
                    w_rbFromFile.Checked = true;
                    break;
                case ScriptFileLocation.Configuration:
                    w_rbConfigure.Checked = true;
                    break;
            }
            w_textBoxScriptFileName.Text = _options.FileName;
            w_scriptEditor.Text = _options.FullScript;
        }

        private void On_None_CheckChanged(object sender, EventArgs e)
        {
            if (w_rbNone.Checked)
            {
                w_btnLoadScriptFromFile.Enabled = false;
                w_scriptEditor.Enabled = false;
            }
        }

        private void On_ScriptFromConfiguration_CheckChanged(object sender, EventArgs e)
        {
            if (w_rbConfigure.Checked)
            {
                w_btnLoadScriptFromFile.Enabled = false;
                w_scriptEditor.Enabled = true;
            }
        }

        private void On_ScriptFromFile_CheckChanged(object sender, EventArgs e)
        {
            if (w_rbFromFile.Checked)
            {
                w_btnLoadScriptFromFile.Enabled = true;
                w_scriptEditor.Enabled = false;
            }
        }

        private void On_SelectScriptFile_Clicked(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select script file";
                openFileDialog.Filter = "PowerShell Script (*.ps1)|*.ps1";
                openFileDialog.InitialDirectory = ApplicationData.ApplicationPath;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    w_textBoxScriptFileName.Text = openFileDialog.FileName;
                }
            }
        }

        public override bool SaveUI()
        {
            string validateFileName = null;
            if (w_rbFromFile.Checked)
            {
                _options.Location = ScriptFileLocation.File;
            }
            else if (w_rbConfigure.Checked)
            {
                _options.Location = ScriptFileLocation.Configuration;
            }
            else if (w_rbNone.Checked)
            {
                _options.Location = ScriptFileLocation.None;
            }
            switch (_options.Location)
            {
                case ScriptFileLocation.None:
                    {
                        if (string.IsNullOrEmpty(validateFileName))
                        {
                            return true;
                        }
                        DialogResult dialogResult = MessageBox.Show(validateFileName, Resources.ConfigurationInvalid, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return false;
                    }
                case ScriptFileLocation.File:
                    {
                        DialogResult dialogResult;
                        if (!string.IsNullOrEmpty(w_textBoxScriptFileName.Text))
                        {
                            _options.FileName = w_textBoxScriptFileName.Text;
                            _options.FullScript = string.Empty;
                            if (string.IsNullOrEmpty(validateFileName))
                            {
                                return true;
                            }
                            dialogResult = MessageBox.Show(validateFileName, Resources.ConfigurationInvalid, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            return false;
                        }
                        validateFileName = Resources.ValidateFileName;
                        w_btnLoadScriptFromFile.Focus();
                        if (string.IsNullOrEmpty(validateFileName))
                        {
                            return true;
                        }
                        dialogResult = MessageBox.Show(validateFileName, Resources.ConfigurationInvalid, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return false;
                    }
                case ScriptFileLocation.Configuration:
                    {
                        DialogResult dialogResult;
                        if (!string.IsNullOrEmpty(w_scriptEditor.Text))
                        {
                            _options.FileName = string.Empty;
                            _options.FullScript = w_scriptEditor.Text;
                            if (string.IsNullOrEmpty(validateFileName))
                            {
                                return true;
                            }
                            dialogResult = MessageBox.Show(validateFileName, Resources.ConfigurationInvalid, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            return false;
                        }
                        validateFileName = Resources.ValidateScript;
                        w_scriptEditor.Focus();
                        if (string.IsNullOrEmpty(validateFileName))
                        {
                            return true;
                        }
                        dialogResult = MessageBox.Show(validateFileName, Resources.ConfigurationInvalid, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return false;
                    }
                default:
                    {
                        if (string.IsNullOrEmpty(validateFileName))
                        {
                            return true;
                        }
                        DialogResult dialogResult = MessageBox.Show(validateFileName, Resources.ConfigurationInvalid, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return false;
                    }
            }
        }
    }
}