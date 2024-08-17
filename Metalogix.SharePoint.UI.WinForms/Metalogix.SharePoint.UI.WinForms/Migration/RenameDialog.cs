using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.Data.Filters;
using Metalogix.DataStructures;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Components;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class RenameDialog : CollapsableForm
	{
		private Type m_NodeType;

		private PropertyDescriptorCollection m_DescCollection;

		private SPNode m_Node;

		private bool m_bIsSubFolder;

		private bool m_bIsRuleBased;

		private TransformationTask m_Task;

		private string m_sTargetTitle;

		private string m_sTargetName;

		private bool m_bForceRename;

		private IContainer components;

		private TextEdit w_tbTargetName;

		private TextEdit w_tbTargetTitle;

		private SimpleButton w_btnOK;

		private SimpleButton w_btnCancel;

		private LabelControl w_lblSourceName;

		private LabelControl w_lblSourceTitle;

		private PanelControl w_plName;

		private PanelControl w_plTitle;

		private TextEditContextMenu _textEditContextMenu;

		public bool ForceRename
		{
			set
			{
				m_bForceRename = value;
			}
		}

		public string TargetName
		{
			get
			{
				return m_sTargetName;
			}
			set
			{
				m_sTargetName = value;
				w_tbTargetName.Text = value;
			}
		}

		public string TargetTitle
		{
			set
			{
				m_sTargetTitle = value;
				w_tbTargetTitle.Text = value;
			}
		}

		public TransformationTask Task => m_Task;

		public RenameDialog(SPNode node)
			: this(node, bIsRoot: true)
		{
		}

		public RenameDialog(SPNode node, bool bIsRoot)
		{
			if (node != null)
			{
				InitializeComponent();
				Type type = node.GetType();
				if (bIsRoot || !(type == typeof(SPFolder)))
				{
					m_NodeType = type;
				}
				else
				{
					m_bIsSubFolder = true;
					m_NodeType = typeof(SPListItem);
				}
				m_Node = node;
				m_DescCollection = node.GetProperties();
				PopulateDialog();
			}
		}

		public RenameDialog(SPNode node, bool bIsRoot, bool bIsRuleBased)
		{
			if (node == null)
			{
				return;
			}
			InitializeComponent();
			if (!bIsRuleBased)
			{
				Type type = node.GetType();
				if (bIsRoot || !(type == typeof(SPFolder)))
				{
					m_NodeType = type;
				}
				else
				{
					m_bIsSubFolder = true;
					m_NodeType = typeof(SPListItem);
				}
			}
			else
			{
				m_NodeType = typeof(SPNode);
				m_bIsSubFolder = false;
				m_bIsRuleBased = bIsRuleBased;
				m_bForceRename = false;
			}
			m_Node = node;
			m_DescCollection = node.GetProperties();
			PopulateDialog();
		}

		private bool CheckUrlCharacters()
		{
			char[] anyOf = new char[12]
			{
				'#', '%', '&', '*', '{', '}', '\\', ':', '<', '>',
				'?', '/'
			};
			if (w_tbTargetName.Text.IndexOfAny(anyOf) >= 0)
			{
				FlatXtraMessageBox.Show("Illegal Characters in Name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			if (m_NodeType.ToString() == "Metalogix.SharePoint.SPFolder")
			{
				char[] anyOf2 = new char[12]
				{
					'#', '%', '&', '*', '{', '}', '\\', ':', '<', '>',
					'?', '/'
				};
				if (w_tbTargetTitle.Text.IndexOfAny(anyOf2) >= 0)
				{
					FlatXtraMessageBox.Show("Illegal Characters in Target Value", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return false;
				}
			}
			return true;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.SharePoint.UI.WinForms.Migration.RenameDialog));
			this.w_tbTargetName = new DevExpress.XtraEditors.TextEdit();
			this._textEditContextMenu = new Metalogix.UI.WinForms.TextEditContextMenu();
			this.w_tbTargetTitle = new DevExpress.XtraEditors.TextEdit();
			this.w_btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.w_btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.w_lblSourceName = new DevExpress.XtraEditors.LabelControl();
			this.w_lblSourceTitle = new DevExpress.XtraEditors.LabelControl();
			this.w_plName = new DevExpress.XtraEditors.PanelControl();
			this.w_plTitle = new DevExpress.XtraEditors.PanelControl();
			((System.ComponentModel.ISupportInitialize)this.w_tbTargetName.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_tbTargetTitle.Properties).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.w_plName).BeginInit();
			this.w_plName.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.w_plTitle).BeginInit();
			this.w_plTitle.SuspendLayout();
			base.SuspendLayout();
			resources.ApplyResources(this.w_tbTargetName, "w_tbTargetName");
			this.w_tbTargetName.Name = "w_tbTargetName";
			this.w_tbTargetName.Properties.ContextMenuStrip = this._textEditContextMenu;
			this._textEditContextMenu.Name = "TextEditContextMenu";
			resources.ApplyResources(this._textEditContextMenu, "_textEditContextMenu");
			resources.ApplyResources(this.w_tbTargetTitle, "w_tbTargetTitle");
			this.w_tbTargetTitle.Name = "w_tbTargetTitle";
			this.w_tbTargetTitle.Properties.ContextMenuStrip = this._textEditContextMenu;
			resources.ApplyResources(this.w_btnOK, "w_btnOK");
			this.w_btnOK.Name = "w_btnOK";
			this.w_btnOK.Click += new System.EventHandler(On_OK_Clicked);
			resources.ApplyResources(this.w_btnCancel, "w_btnCancel");
			this.w_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_btnCancel.Name = "w_btnCancel";
			this.w_btnCancel.Click += new System.EventHandler(On_Cancel_Clicked);
			resources.ApplyResources(this.w_lblSourceName, "w_lblSourceName");
			this.w_lblSourceName.Name = "w_lblSourceName";
			resources.ApplyResources(this.w_lblSourceTitle, "w_lblSourceTitle");
			this.w_lblSourceTitle.Name = "w_lblSourceTitle";
			resources.ApplyResources(this.w_plName, "w_plName");
			this.w_plName.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plName.Controls.Add(this.w_tbTargetName);
			this.w_plName.Controls.Add(this.w_lblSourceName);
			this.w_plName.Name = "w_plName";
			resources.ApplyResources(this.w_plTitle, "w_plTitle");
			this.w_plTitle.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.w_plTitle.Controls.Add(this.w_lblSourceTitle);
			this.w_plTitle.Controls.Add(this.w_tbTargetTitle);
			this.w_plTitle.Name = "w_plTitle";
			base.AcceptButton = this.w_btnOK;
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.w_btnCancel;
			base.Controls.Add(this.w_btnCancel);
			base.Controls.Add(this.w_btnOK);
			base.Controls.Add(this.w_plName);
			base.Controls.Add(this.w_plTitle);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "RenameDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((System.ComponentModel.ISupportInitialize)this.w_tbTargetName.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_tbTargetTitle.Properties).EndInit();
			((System.ComponentModel.ISupportInitialize)this.w_plName).EndInit();
			this.w_plName.ResumeLayout(false);
			this.w_plName.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.w_plTitle).EndInit();
			this.w_plTitle.ResumeLayout(false);
			this.w_plTitle.PerformLayout();
			base.ResumeLayout(false);
		}

		private void On_Cancel_Clicked(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
			Close();
		}

		private void On_OK_Clicked(object sender, EventArgs e)
		{
			if (m_bForceRename && w_tbTargetName.Text.ToLower() == m_DescCollection["Name"].GetValue(m_Node).ToString().ToLower())
			{
				FlatXtraMessageBox.Show("Must enter a new value for the Name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (m_NodeType != typeof(SPFolder))
			{
				if (w_tbTargetTitle.Text == (string)m_DescCollection["Title"].GetValue(m_Node) && w_tbTargetName.Text == (string)m_DescCollection["Name"].GetValue(m_Node))
				{
					Close();
				}
				if (w_tbTargetTitle.Text == "" && w_tbTargetName.Text == "")
				{
					FlatXtraMessageBox.Show("Target Name or Title must have a value", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return;
				}
			}
			else if (w_tbTargetTitle.Text == "")
			{
				FlatXtraMessageBox.Show("Target Title must have a value", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			m_Task = new TransformationTask();
			if (m_bIsSubFolder && m_Node.LinkableUrl == null)
			{
				FlatXtraMessageBox.Show("Folder renaming from a database source requires a host mapping. Please establish a host mapping and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				m_Task = null;
				return;
			}
			string sPattern = (string)m_DescCollection["DisplayUrl"].GetValue(m_Node);
			if (!m_bIsRuleBased)
			{
				m_Task.ApplyTo = new FilterExpression(FilterOperand.Equals, m_NodeType, "DisplayUrl", sPattern, bIsCaseSensitive: false, bIsBaseFilter: false);
			}
			if (!CheckUrlCharacters())
			{
				m_Task = null;
				return;
			}
			if (m_bIsRuleBased)
			{
				if (w_tbTargetTitle.Text != "")
				{
					m_Task.ChangeOperations.Add("Title", w_tbTargetTitle.Text.Trim());
					m_Task.ChangeOperations.Add("FileLeafRef", w_tbTargetTitle.Text.Trim());
				}
			}
			else if (!typeof(SPWeb).IsAssignableFrom(m_NodeType) && !typeof(SPList).IsAssignableFrom(m_NodeType))
			{
				if (w_tbTargetTitle.Text != "")
				{
					m_Task.ChangeOperations.Add("FileLeafRef", w_tbTargetTitle.Text.Trim());
				}
			}
			else if (w_tbTargetTitle.Text != "")
			{
				m_Task.ChangeOperations.Add("Title", w_tbTargetTitle.Text.Trim());
			}
			if (w_tbTargetName.Text != "")
			{
				m_Task.ChangeOperations.Add("Name", w_tbTargetName.Text.Trim());
			}
			base.DialogResult = DialogResult.OK;
			Close();
		}

		public void PopulateDialog()
		{
			string text = null;
			string text2 = null;
			string text3 = null;
			bool flag = false;
			if (!m_bIsRuleBased)
			{
				if (typeof(SPWeb).IsAssignableFrom(m_NodeType))
				{
					text = "Web's";
					text2 = m_DescCollection["WebName"].GetValue(m_Node).ToString();
					object value = m_DescCollection["Title"].GetValue(m_Node);
					text3 = ((value != null) ? value.ToString() : "");
				}
				else if (!typeof(SPList).IsAssignableFrom(m_NodeType))
				{
					text = "Folder's";
					object value2 = m_DescCollection["FileLeafRef"].GetValue(m_Node);
					text3 = ((value2 != null) ? value2.ToString() : "");
					text2 = "";
					flag = true;
				}
				else
				{
					text = "List's";
					text2 = m_DescCollection["Name"].GetValue(m_Node).ToString();
					object value3 = m_DescCollection["Title"].GetValue(m_Node);
					text3 = ((value3 != null) ? value3.ToString() : "");
				}
			}
			int val = 0;
			int num = 0;
			if (m_bIsRuleBased)
			{
				w_lblSourceName.Text = "Change " + text + "Name to :";
				w_lblSourceTitle.Text = "Change " + text + "Title to :";
				val = w_lblSourceName.Width;
			}
			else if (flag)
			{
				LabelControl labelControl = w_lblSourceTitle;
				string[] array = new string[5] { "Change ", text, " Title and Name from '", text3, "' to :" };
				labelControl.Text = string.Concat(array);
				HideControl(w_plName);
			}
			else
			{
				LabelControl labelControl2 = w_lblSourceName;
				string[] array2 = new string[5] { "Change ", text, " Name from '", text2, "' to :" };
				labelControl2.Text = string.Concat(array2);
				LabelControl labelControl3 = w_lblSourceTitle;
				string[] array3 = new string[5] { "Change ", text, " Title from '", text3, "' to :" };
				labelControl3.Text = string.Concat(array3);
				val = w_lblSourceName.Width;
			}
			num = w_lblSourceTitle.Width;
			MinimumSize = new Size(240 + Math.Max(val, num), base.Height);
			MaximumSize = new Size(240 + Math.Max(val, num), base.Height);
			Update();
			if (m_sTargetName != null)
			{
				w_tbTargetName.Text = m_sTargetName;
			}
			if (m_sTargetTitle != null)
			{
				w_tbTargetTitle.Text = m_sTargetTitle;
			}
		}
	}
}
