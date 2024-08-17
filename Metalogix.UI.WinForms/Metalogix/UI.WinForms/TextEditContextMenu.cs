using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Metalogix.UI.WinForms
{
    public partial class TextEditContextMenu : ContextMenuStrip
    {
        private TextEdit _hostTextEdit;

        private bool _menuClosed = true;

        private IContainer components;

        private ToolStripMenuItem _undoToolStripItem;

        private ToolStripSeparator _separatorUpper;

        private ToolStripMenuItem _cutToolStripItem;

        private ToolStripMenuItem _copyToolStripMenuItem;

        private ToolStripMenuItem _pasteToolStripMenuItem;

        private ToolStripMenuItem _deleteToolStripMenuItem;

        private ToolStripSeparator _separatorLower;

        private ToolStripMenuItem _selectAllToolStripMenuItem;

        public TextEditContextMenu()
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

        private TextEdit GetTextEdit()
        {
            if (base.SourceControl is TextBoxMaskBox sourceControl)
            {
                return sourceControl.OwnerEdit;
            }
            return base.SourceControl as TextEdit;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.TextEditContextMenu));
            this._undoToolStripItem = new System.Windows.Forms.ToolStripMenuItem();
            this._separatorUpper = new System.Windows.Forms.ToolStripSeparator();
            this._cutToolStripItem = new System.Windows.Forms.ToolStripMenuItem();
            this._copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._separatorLower = new System.Windows.Forms.ToolStripSeparator();
            this._selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            base.SuspendLayout();
            this._undoToolStripItem.Name = "_undoToolStripItem";
            componentResourceManager.ApplyResources(this._undoToolStripItem, "_undoToolStripItem");
            this._undoToolStripItem.Click += new System.EventHandler(On_Undo_Clicked);
            this._undoToolStripItem.Image = Metalogix.ImageCache.GetImage("Metalogix.UI.WinForms.Resources.Undo16.png", System.Reflection.Assembly.GetExecutingAssembly());
            this._cutToolStripItem.Name = "_cutToolStripItem";
            componentResourceManager.ApplyResources(this._cutToolStripItem, "_cutToolStripItem");
            this._cutToolStripItem.Click += new System.EventHandler(On_Cut_Clicked);
            this._cutToolStripItem.Image = Metalogix.ImageCache.GetImage("Metalogix.UI.WinForms.Resources.Cut16.png", System.Reflection.Assembly.GetExecutingAssembly());
            this._copyToolStripMenuItem.Name = "_copyToolStripMenuItem";
            componentResourceManager.ApplyResources(this._copyToolStripMenuItem, "_copyToolStripMenuItem");
            this._copyToolStripMenuItem.Click += new System.EventHandler(On_Copy_Clicked);
            this._copyToolStripMenuItem.Image = Metalogix.ImageCache.GetImage("Metalogix.UI.WinForms.Resources.CopySelectedJobsToClipboard16.png", System.Reflection.Assembly.GetExecutingAssembly());
            this._pasteToolStripMenuItem.Name = "_pasteToolStripMenuItem";
            componentResourceManager.ApplyResources(this._pasteToolStripMenuItem, "_pasteToolStripMenuItem");
            this._pasteToolStripMenuItem.Click += new System.EventHandler(On_Paste_Clicked);
            this._pasteToolStripMenuItem.Image = Metalogix.ImageCache.GetImage("Metalogix.UI.WinForms.Resources.Paste16.png", System.Reflection.Assembly.GetExecutingAssembly());
            this._deleteToolStripMenuItem.Name = "_deleteToolStripMenuItem";
            componentResourceManager.ApplyResources(this._deleteToolStripMenuItem, "_deleteToolStripMenuItem");
            this._deleteToolStripMenuItem.Click += new System.EventHandler(On_Delete_Clicked);
            this._deleteToolStripMenuItem.Image = Metalogix.ImageCache.GetImage("Metalogix.UI.WinForms.Resources.Delete16.png", System.Reflection.Assembly.GetExecutingAssembly());
            this._selectAllToolStripMenuItem.Name = "_selectAllToolStripMenuItem";
            componentResourceManager.ApplyResources(this._selectAllToolStripMenuItem, "_selectAllToolStripMenuItem");
            this._selectAllToolStripMenuItem.Click += new System.EventHandler(On_SelectAll_Clicked);
            base.Name = "TextEditContextMenu";
            System.Windows.Forms.ToolStripItemCollection items = this.Items;
            System.Windows.Forms.ToolStripItem[] toolStripItemArray = new System.Windows.Forms.ToolStripItem[8] { this._undoToolStripItem, this._separatorUpper, this._cutToolStripItem, this._copyToolStripMenuItem, this._pasteToolStripMenuItem, this._deleteToolStripMenuItem, this._separatorLower, this._selectAllToolStripMenuItem };
            items.AddRange(toolStripItemArray);
            base.Opening += new System.ComponentModel.CancelEventHandler(On_Menu_Opening);
            base.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(On_Menu_Closed);
            base.ResumeLayout(false);
        }

        private void On_Copy_Clicked(object sender, EventArgs e)
        {
            _hostTextEdit.Copy();
            if (_menuClosed)
            {
                _hostTextEdit = null;
            }
        }

        private void On_Cut_Clicked(object sender, EventArgs e)
        {
            _hostTextEdit.Cut();
            if (_menuClosed)
            {
                _hostTextEdit = null;
            }
        }

        private void On_Delete_Clicked(object sender, EventArgs e)
        {
            string text = _hostTextEdit.Text;
            text = _hostTextEdit.Text.Substring(0, _hostTextEdit.SelectionStart) + _hostTextEdit.Text.Substring(_hostTextEdit.SelectionStart + _hostTextEdit.SelectionLength);
            _hostTextEdit.Text = text;
            if (_menuClosed)
            {
                _hostTextEdit = null;
            }
        }

        private void On_Menu_Closed(object sender, ToolStripDropDownClosedEventArgs dropDownClosedArgs)
        {
            _menuClosed = true;
            if (dropDownClosedArgs.CloseReason != ToolStripDropDownCloseReason.ItemClicked)
            {
                _hostTextEdit = null;
            }
        }

        private void On_Menu_Opening(object sender, CancelEventArgs cancelEventArgs)
        {
            _hostTextEdit = GetTextEdit();
            _hostTextEdit.Focus();
            if (_hostTextEdit == null)
            {
                cancelEventArgs.Cancel = true;
                return;
            }
            _menuClosed = false;
            bool readOnly = !_hostTextEdit.Properties.ReadOnly;
            bool flag = !string.IsNullOrEmpty(_hostTextEdit.SelectedText);
            _undoToolStripItem.Visible = readOnly;
            _undoToolStripItem.Enabled = _hostTextEdit.CanUndo;
            _separatorUpper.Visible = readOnly;
            _cutToolStripItem.Visible = readOnly;
            _cutToolStripItem.Enabled = flag;
            _copyToolStripMenuItem.Enabled = flag;
            _pasteToolStripMenuItem.Visible = readOnly;
            _deleteToolStripMenuItem.Visible = readOnly;
            _deleteToolStripMenuItem.Enabled = flag;
            _separatorLower.Visible = readOnly;
            _selectAllToolStripMenuItem.Enabled = _hostTextEdit.SelectedText != _hostTextEdit.Text;
        }

        private void On_Paste_Clicked(object sender, EventArgs e)
        {
            _hostTextEdit.Paste();
            if (_menuClosed)
            {
                _hostTextEdit = null;
            }
        }

        private void On_SelectAll_Clicked(object sender, EventArgs e)
        {
            _hostTextEdit.SelectAll();
            if (_menuClosed)
            {
                _hostTextEdit = null;
            }
        }

        private void On_Undo_Clicked(object sender, EventArgs e)
        {
            _hostTextEdit.Undo();
            if (_menuClosed)
            {
                _hostTextEdit = null;
            }
        }
    }
}
