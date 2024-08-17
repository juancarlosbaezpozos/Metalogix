using Metalogix.Metabase.DataTypes;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Widgets.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Widgets
{
	public class TextMonikerEditor : Form, ITextMonikerEditorView
	{
		private ToolStrip w_toolStrip;

		private ToolStripLabel w_tslRecord;

		private ToolStripSplitButton w_tsButtonPrevRecord;

		private ToolStripMenuItem w_tsMnuPrev;

		private ToolStripMenuItem w_tsMnuFirst;

		private ToolStripSplitButton w_tsButtonNextRecord;

		private ToolStripMenuItem w_tsMnuNext;

		private ToolStripMenuItem w_tsMnuLast;

		private ToolStripSeparator toolStripSeparator1;

		private ToolStripLabel w_tslProperty;

		private ToolStripComboBox w_comboProperties;

		private TabControl w_tabControl;

		private TabPage w_tabPageText;

		private Button w_btnApply;

		private Button w_btnOK;

		private Button w_btnCancel;

		private System.Windows.Forms.ContextMenuStrip w_contextMenuStrip;

		private ToolStripMenuItem w_copyToolStripMenuItem;

		private ToolStripMenuItem w_pasteToolStripMenuItem;

		private ToolStripMenuItem w_cutToolStripMenuItem;

		private ToolStripMenuItem w_deleteToolStripMenuItem;

		private ToolStripSeparator w_toolStripSeparator2;

		private ToolStripMenuItem w_selectAllTtoolStripMenuItem;

		private ToolStripMenuItem w_undoToolStripMenuItem;

		private ToolStripSeparator w_toolStripSeparator1;

		private RichTextBox w_txtBox;

		private IContainer components;

		private readonly ITextMonikerEditorPresenter _presenter;

		public string EditorText
		{
			get
			{
				return this.w_txtBox.Text;
			}
			set
			{
				this.w_txtBox.Text = value;
			}
		}

		public bool NextTransverseEnabled
		{
			set
			{
				this.w_tsButtonNextRecord.Enabled = value;
			}
		}

		public bool PrevTransverseEnabled
		{
			set
			{
				this.w_tsButtonPrevRecord.Enabled = value;
			}
		}

		public string Title
		{
			set
			{
				this.Text = value;
			}
		}

		public TextMonikerEditor(IList<TextMoniker> textMonikerList)
		{
			this.InitializeComponent();
			this._presenter = new TextMonikerEditorPresenter(this, textMonikerList);
		}

		public void AddPropertyDescriptor(PropertyDescriptor pd)
		{
			this.w_comboProperties.Items.Add(pd);
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
			this.components = new System.ComponentModel.Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(TextMonikerEditor));
			this.w_btnOK = new Button();
			this.w_btnCancel = new Button();
			this.w_tabControl = new TabControl();
			this.w_tabPageText = new TabPage();
			this.w_txtBox = new RichTextBox();
			this.w_contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.w_undoToolStripMenuItem = new ToolStripMenuItem();
			this.w_toolStripSeparator1 = new ToolStripSeparator();
			this.w_cutToolStripMenuItem = new ToolStripMenuItem();
			this.w_copyToolStripMenuItem = new ToolStripMenuItem();
			this.w_pasteToolStripMenuItem = new ToolStripMenuItem();
			this.w_deleteToolStripMenuItem = new ToolStripMenuItem();
			this.w_toolStripSeparator2 = new ToolStripSeparator();
			this.w_selectAllTtoolStripMenuItem = new ToolStripMenuItem();
			this.w_tsButtonNextRecord = new ToolStripSplitButton();
			this.w_tsMnuNext = new ToolStripMenuItem();
			this.w_tsMnuLast = new ToolStripMenuItem();
			this.w_toolStrip = new ToolStrip();
			this.w_tslRecord = new ToolStripLabel();
			this.w_tsButtonPrevRecord = new ToolStripSplitButton();
			this.w_tsMnuPrev = new ToolStripMenuItem();
			this.w_tsMnuFirst = new ToolStripMenuItem();
			this.toolStripSeparator1 = new ToolStripSeparator();
			this.w_tslProperty = new ToolStripLabel();
			this.w_comboProperties = new ToolStripComboBox();
			this.w_btnApply = new Button();
			this.w_tabControl.SuspendLayout();
			this.w_tabPageText.SuspendLayout();
			this.w_contextMenuStrip.SuspendLayout();
			this.w_toolStrip.SuspendLayout();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.w_btnOK, "w_btnOK");
			this.w_btnOK.Name = "w_btnOK";
			this.w_btnOK.Click += new EventHandler(this.On_OK_Click);
			componentResourceManager.ApplyResources(this.w_btnCancel, "w_btnCancel");
			this.w_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_btnCancel.Name = "w_btnCancel";
			this.w_btnCancel.Click += new EventHandler(this.On_Cancel_Click);
			componentResourceManager.ApplyResources(this.w_tabControl, "w_tabControl");
			this.w_tabControl.Controls.Add(this.w_tabPageText);
			this.w_tabControl.Name = "w_tabControl";
			this.w_tabControl.SelectedIndex = 0;
			this.w_tabPageText.Controls.Add(this.w_txtBox);
			componentResourceManager.ApplyResources(this.w_tabPageText, "w_tabPageText");
			this.w_tabPageText.Name = "w_tabPageText";
			this.w_tabPageText.UseVisualStyleBackColor = true;
			this.w_txtBox.BorderStyle = BorderStyle.FixedSingle;
			this.w_txtBox.ContextMenuStrip = this.w_contextMenuStrip;
			componentResourceManager.ApplyResources(this.w_txtBox, "w_txtBox");
			this.w_txtBox.Name = "w_txtBox";
			this.w_contextMenuStrip.BackColor = SystemColors.Control;
			ToolStripItemCollection items = this.w_contextMenuStrip.Items;
			ToolStripItem[] wUndoToolStripMenuItem = new ToolStripItem[] { this.w_undoToolStripMenuItem, this.w_toolStripSeparator1, this.w_cutToolStripMenuItem, this.w_copyToolStripMenuItem, this.w_pasteToolStripMenuItem, this.w_deleteToolStripMenuItem, this.w_toolStripSeparator2, this.w_selectAllTtoolStripMenuItem };
			items.AddRange(wUndoToolStripMenuItem);
			this.w_contextMenuStrip.Name = "w_contextMenuStrip";
			componentResourceManager.ApplyResources(this.w_contextMenuStrip, "w_contextMenuStrip");
			this.w_contextMenuStrip.Opening += new CancelEventHandler(this.w_contextMenuStrip_Opening);
			this.w_undoToolStripMenuItem.Name = "w_undoToolStripMenuItem";
			componentResourceManager.ApplyResources(this.w_undoToolStripMenuItem, "w_undoToolStripMenuItem");
			this.w_undoToolStripMenuItem.Click += new EventHandler(this.w_undoToolStripMenuItem_Click);
			this.w_toolStripSeparator1.Name = "w_toolStripSeparator1";
			componentResourceManager.ApplyResources(this.w_toolStripSeparator1, "w_toolStripSeparator1");
			this.w_cutToolStripMenuItem.Name = "w_cutToolStripMenuItem";
			componentResourceManager.ApplyResources(this.w_cutToolStripMenuItem, "w_cutToolStripMenuItem");
			this.w_cutToolStripMenuItem.Click += new EventHandler(this.w_cutToolStripMenuItem_Click);
			this.w_copyToolStripMenuItem.Name = "w_copyToolStripMenuItem";
			componentResourceManager.ApplyResources(this.w_copyToolStripMenuItem, "w_copyToolStripMenuItem");
			this.w_copyToolStripMenuItem.Click += new EventHandler(this.w_copyToolStripMenuItem_Click);
			this.w_pasteToolStripMenuItem.Name = "w_pasteToolStripMenuItem";
			componentResourceManager.ApplyResources(this.w_pasteToolStripMenuItem, "w_pasteToolStripMenuItem");
			this.w_pasteToolStripMenuItem.Click += new EventHandler(this.w_pasteToolStripMenuItem_Click);
			this.w_deleteToolStripMenuItem.Name = "w_deleteToolStripMenuItem";
			componentResourceManager.ApplyResources(this.w_deleteToolStripMenuItem, "w_deleteToolStripMenuItem");
			this.w_deleteToolStripMenuItem.Click += new EventHandler(this.w_deleteToolStripMenuItem_Click);
			this.w_toolStripSeparator2.Name = "w_toolStripSeparator2";
			componentResourceManager.ApplyResources(this.w_toolStripSeparator2, "w_toolStripSeparator2");
			this.w_selectAllTtoolStripMenuItem.Name = "w_selectAllTtoolStripMenuItem";
			componentResourceManager.ApplyResources(this.w_selectAllTtoolStripMenuItem, "w_selectAllTtoolStripMenuItem");
			this.w_selectAllTtoolStripMenuItem.Click += new EventHandler(this.w_selectAllTtoolStripMenuItem_Click);
			ToolStripItemCollection dropDownItems = this.w_tsButtonNextRecord.DropDownItems;
			ToolStripItem[] wTsMnuNext = new ToolStripItem[] { this.w_tsMnuNext, this.w_tsMnuLast };
			dropDownItems.AddRange(wTsMnuNext);
			componentResourceManager.ApplyResources(this.w_tsButtonNextRecord, "w_tsButtonNextRecord");
			this.w_tsButtonNextRecord.Name = "w_tsButtonNextRecord";
			this.w_tsButtonNextRecord.ButtonClick += new EventHandler(this.On_mnuNextRecord_Click);
			this.w_tsMnuNext.Name = "w_tsMnuNext";
			componentResourceManager.ApplyResources(this.w_tsMnuNext, "w_tsMnuNext");
			this.w_tsMnuNext.Click += new EventHandler(this.On_mnuNextRecord_Click);
			this.w_tsMnuLast.Name = "w_tsMnuLast";
			componentResourceManager.ApplyResources(this.w_tsMnuLast, "w_tsMnuLast");
			this.w_tsMnuLast.Click += new EventHandler(this.On_mnuLastRecord_Click);
			this.w_toolStrip.GripStyle = ToolStripGripStyle.Hidden;
			ToolStripItemCollection toolStripItemCollections = this.w_toolStrip.Items;
			ToolStripItem[] wTslRecord = new ToolStripItem[] { this.w_tslRecord, this.w_tsButtonPrevRecord, this.w_tsButtonNextRecord, this.toolStripSeparator1, this.w_tslProperty, this.w_comboProperties };
			toolStripItemCollections.AddRange(wTslRecord);
			componentResourceManager.ApplyResources(this.w_toolStrip, "w_toolStrip");
			this.w_toolStrip.Name = "w_toolStrip";
			this.w_tslRecord.Name = "w_tslRecord";
			componentResourceManager.ApplyResources(this.w_tslRecord, "w_tslRecord");
			ToolStripItemCollection dropDownItems1 = this.w_tsButtonPrevRecord.DropDownItems;
			ToolStripItem[] wTsMnuPrev = new ToolStripItem[] { this.w_tsMnuPrev, this.w_tsMnuFirst };
			dropDownItems1.AddRange(wTsMnuPrev);
			componentResourceManager.ApplyResources(this.w_tsButtonPrevRecord, "w_tsButtonPrevRecord");
			this.w_tsButtonPrevRecord.Name = "w_tsButtonPrevRecord";
			this.w_tsButtonPrevRecord.ButtonClick += new EventHandler(this.On_mnuPrevRecord_Click);
			this.w_tsMnuPrev.Name = "w_tsMnuPrev";
			componentResourceManager.ApplyResources(this.w_tsMnuPrev, "w_tsMnuPrev");
			this.w_tsMnuPrev.Click += new EventHandler(this.On_mnuPrevRecord_Click);
			this.w_tsMnuFirst.Name = "w_tsMnuFirst";
			componentResourceManager.ApplyResources(this.w_tsMnuFirst, "w_tsMnuFirst");
			this.w_tsMnuFirst.Click += new EventHandler(this.On_mnuFirstRecord_Click);
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			componentResourceManager.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
			this.w_tslProperty.Name = "w_tslProperty";
			componentResourceManager.ApplyResources(this.w_tslProperty, "w_tslProperty");
			this.w_comboProperties.Name = "w_comboProperties";
			componentResourceManager.ApplyResources(this.w_comboProperties, "w_comboProperties");
			this.w_comboProperties.Sorted = true;
			this.w_comboProperties.SelectedIndexChanged += new EventHandler(this.On_mnuComboProperties_SelectedIndexChanged);
			componentResourceManager.ApplyResources(this.w_btnApply, "w_btnApply");
			this.w_btnApply.Name = "w_btnApply";
			this.w_btnApply.Click += new EventHandler(this.On_Apply_Click);
			componentResourceManager.ApplyResources(this, "$this");
			base.Controls.Add(this.w_btnApply);
			base.Controls.Add(this.w_tabControl);
			base.Controls.Add(this.w_btnCancel);
			base.Controls.Add(this.w_btnOK);
			base.Controls.Add(this.w_toolStrip);
			base.MinimizeBox = false;
			base.Name = "TextMonikerEditor";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			this.w_tabControl.ResumeLayout(false);
			this.w_tabPageText.ResumeLayout(false);
			this.w_contextMenuStrip.ResumeLayout(false);
			this.w_toolStrip.ResumeLayout(false);
			this.w_toolStrip.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void On_Apply_Click(object sender, EventArgs e)
		{
			this._presenter.SaveText(false);
		}

		private void On_Cancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			base.Close();
		}

		private void On_mnuComboProperties_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this._presenter != null)
			{
				this._presenter.ChangeProperty(this.w_comboProperties.SelectedItem as PropertyDescriptor);
			}
		}

		private void On_mnuFirstRecord_Click(object sender, EventArgs e)
		{
			this._presenter.JumpToRecord(0);
		}

		private void On_mnuLastRecord_Click(object sender, EventArgs e)
		{
			this._presenter.JumpToRecord(this._presenter.RecordIndexMax);
		}

		private void On_mnuNextRecord_Click(object sender, EventArgs e)
		{
			this._presenter.JumpToRecord(this._presenter.RecordIndex + 1);
		}

		private void On_mnuPrevRecord_Click(object sender, EventArgs e)
		{
			this._presenter.JumpToRecord(this._presenter.RecordIndex - 1);
		}

		private void On_OK_Click(object sender, EventArgs e)
		{
			base.DialogResult = System.Windows.Forms.DialogResult.OK;
			this._presenter.SaveText(false);
			base.Close();
		}

		public void SelectPropertyDescriptor(PropertyDescriptor value)
		{
			this.w_comboProperties.SelectedItem = value;
		}

		public bool ShowSaveConfirmationBox()
		{
			return FlatXtraMessageBox.Show("Do you wish to save changes first?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes;
		}

		private void w_contextMenuStrip_Opening(object sender, CancelEventArgs e)
		{
			this.w_undoToolStripMenuItem.Enabled = this.w_txtBox.CanUndo;
			this.w_cutToolStripMenuItem.Enabled = this.w_txtBox.SelectedText.Length > 0;
			this.w_copyToolStripMenuItem.Enabled = this.w_txtBox.SelectedText.Length > 0;
			this.w_pasteToolStripMenuItem.Enabled = Clipboard.ContainsText();
			this.w_selectAllTtoolStripMenuItem.Enabled = this.w_txtBox.Text.Length > 0;
			this.w_deleteToolStripMenuItem.Enabled = this.w_txtBox.SelectedText.Length > 0;
		}

		private void w_copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.w_txtBox.Copy();
		}

		private void w_cutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.w_txtBox.Cut();
		}

		private void w_deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int selectionStart = this.w_txtBox.SelectionStart;
			this.w_txtBox.Text = string.Concat(this.w_txtBox.Text.Substring(0, selectionStart), this.w_txtBox.Text.Substring(selectionStart + this.w_txtBox.SelectionLength));
			this.w_txtBox.SelectionStart = selectionStart;
			this.w_txtBox.SelectionLength = 0;
		}

		private void w_pasteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.w_txtBox.Paste();
		}

		private void w_selectAllTtoolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.w_txtBox.SelectAll();
		}

		private void w_undoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.w_txtBox.Undo();
		}
	}
}