using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Metalogix.Metabase.Options;

namespace Metalogix.UI.WinForms.Metabase
{
    public class ExportToCSVControl : UserControl
    {
        private string[] _excludedFields = new string[6] { "Credentials", "ErrorMsg", "MetabaseConnection", "FileSize", "Extension", "SourcePath" };

        private string[] _includedFields = new string[4] { "Author", "Editor", "Name", "Source URL" };

        private ExportToCSVOptions m_options;

        private PropertyDescriptorCollection m_properties;

        private bool _isExcelFile;

        private readonly TreeViewEventHandler m_treeViewAfterCheckEventHandler;

        private IContainer components;

        private Button w_btnBrowse;

        private TextBox w_txtTargetFile;

        private Label w_lblTargetFile;

        private Label w_lblColumnsToExport;

        protected TreeView w_treeColumnsToExport;

        protected CheckBox w_cbIncludeTypes;

        public bool IsExcelFile
        {
            get
		{
			return _isExcelFile;
		}
            set
		{
			_isExcelFile = value;
			if (value)
			{
				w_cbIncludeTypes.Text = "Include column type information in Excel headers";
			}
			else
			{
				w_cbIncludeTypes.Text = "Include column type information in CSV headers";
			}
		}
        }

        public ExportToCSVOptions Options
        {
            set
		{
			if (value != null)
			{
				m_options = value;
				LoadUI();
			}
		}
        }

        public PropertyDescriptorCollection Properties
        {
            set
		{
			m_properties = value;
			LoadUI();
		}
        }

        public ExportToCSVControl()
	{
		InitializeComponent();
		m_treeViewAfterCheckEventHandler = On_treeColumnsToExport_AfterCheck;
		w_treeColumnsToExport.AfterCheck += m_treeViewAfterCheckEventHandler;
	}

        protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

        protected virtual string GetPropertyDescriptorGroup(PropertyDescriptor pd)
	{
		return pd.Category;
	}

        private void InitializeComponent()
	{
		this.w_btnBrowse = new System.Windows.Forms.Button();
		this.w_txtTargetFile = new System.Windows.Forms.TextBox();
		this.w_lblTargetFile = new System.Windows.Forms.Label();
		this.w_lblColumnsToExport = new System.Windows.Forms.Label();
		this.w_treeColumnsToExport = new System.Windows.Forms.TreeView();
		this.w_cbIncludeTypes = new System.Windows.Forms.CheckBox();
		base.SuspendLayout();
		this.w_btnBrowse.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.w_btnBrowse.Location = new System.Drawing.Point(388, 1);
		this.w_btnBrowse.Name = "w_btnBrowse";
		this.w_btnBrowse.Size = new System.Drawing.Size(75, 23);
		this.w_btnBrowse.TabIndex = 2;
		this.w_btnBrowse.Text = "Browse...";
		this.w_btnBrowse.UseVisualStyleBackColor = true;
		this.w_btnBrowse.Click += new System.EventHandler(On_btnBrowse_Click);
		this.w_txtTargetFile.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.w_txtTargetFile.Location = new System.Drawing.Point(63, 3);
		this.w_txtTargetFile.Name = "w_txtTargetFile";
		this.w_txtTargetFile.Size = new System.Drawing.Size(319, 20);
		this.w_txtTargetFile.TabIndex = 1;
		this.w_lblTargetFile.AutoSize = true;
		this.w_lblTargetFile.Location = new System.Drawing.Point(2, 6);
		this.w_lblTargetFile.Name = "w_lblTargetFile";
		this.w_lblTargetFile.Size = new System.Drawing.Size(55, 13);
		this.w_lblTargetFile.TabIndex = 0;
		this.w_lblTargetFile.Text = "Output file";
		this.w_lblColumnsToExport.AutoSize = true;
		this.w_lblColumnsToExport.Location = new System.Drawing.Point(2, 26);
		this.w_lblColumnsToExport.Name = "w_lblColumnsToExport";
		this.w_lblColumnsToExport.Size = new System.Drawing.Size(164, 13);
		this.w_lblColumnsToExport.TabIndex = 3;
		this.w_lblColumnsToExport.Text = "Select property columns to export";
		this.w_treeColumnsToExport.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.w_treeColumnsToExport.CheckBoxes = true;
		this.w_treeColumnsToExport.Location = new System.Drawing.Point(2, 44);
		this.w_treeColumnsToExport.Name = "w_treeColumnsToExport";
		this.w_treeColumnsToExport.Size = new System.Drawing.Size(461, 230);
		this.w_treeColumnsToExport.TabIndex = 4;
		this.w_cbIncludeTypes.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.w_cbIncludeTypes.AutoSize = true;
		this.w_cbIncludeTypes.Location = new System.Drawing.Point(5, 280);
		this.w_cbIncludeTypes.Name = "w_cbIncludeTypes";
		this.w_cbIncludeTypes.Size = new System.Drawing.Size(251, 17);
		this.w_cbIncludeTypes.TabIndex = 5;
		this.w_cbIncludeTypes.Text = "Include column type information in CSV headers";
		this.w_cbIncludeTypes.UseVisualStyleBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.w_cbIncludeTypes);
		base.Controls.Add(this.w_btnBrowse);
		base.Controls.Add(this.w_txtTargetFile);
		base.Controls.Add(this.w_lblTargetFile);
		base.Controls.Add(this.w_lblColumnsToExport);
		base.Controls.Add(this.w_treeColumnsToExport);
		base.Name = "ExportToCSVControl";
		base.Size = new System.Drawing.Size(466, 299);
		base.ResumeLayout(false);
		base.PerformLayout();
	}

        protected virtual void LoadUI()
	{
		if (m_options == null || m_properties == null)
		{
			return;
		}
		w_txtTargetFile.Text = m_options.TargetFilename;
		w_cbIncludeTypes.Checked = m_options.IncludeTypes;
		w_treeColumnsToExport.AfterCheck -= m_treeViewAfterCheckEventHandler;
		w_treeColumnsToExport.Nodes.Clear();
		foreach (PropertyDescriptor mProperty in m_properties)
		{
			if (!IsExcelFile || (!_excludedFields.Contains(mProperty.DisplayName) && !mProperty.IsReadOnly) || _includedFields.Contains(mProperty.DisplayName))
			{
				string propertyDescriptorGroup = GetPropertyDescriptorGroup(mProperty);
				TreeNode[] treeNodeArray = w_treeColumnsToExport.Nodes.Find(propertyDescriptorGroup, searchAllChildren: false);
				if (treeNodeArray.Length == 0)
				{
					TreeNode[] treeNodeArray1 = new TreeNode[1] { w_treeColumnsToExport.Nodes.Add(propertyDescriptorGroup, propertyDescriptorGroup) };
					treeNodeArray = treeNodeArray1;
					treeNodeArray[0].Checked = true;
				}
				TreeNode description = treeNodeArray[0].Nodes.Add(mProperty.Name, mProperty.DisplayName);
				description.ToolTipText = mProperty.Description;
				description.Checked = m_options.ColumnsToExport.Contains(mProperty.Name);
				treeNodeArray[0].Checked = treeNodeArray[0].Checked && description.Checked;
			}
		}
		w_treeColumnsToExport.AfterCheck += m_treeViewAfterCheckEventHandler;
	}

        private void On_btnBrowse_Click(object sender, EventArgs e)
	{
		SaveFileDialog saveFileDialog = new SaveFileDialog();
		if (!IsExcelFile)
		{
			saveFileDialog.Filter = "CSV files|*.csv|All files|*.*";
			saveFileDialog.DefaultExt = ".csv";
		}
		else
		{
			saveFileDialog.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
			saveFileDialog.DefaultExt = "xlsx";
		}
		saveFileDialog.FileName = w_txtTargetFile.Text;
		if (saveFileDialog.ShowDialog() == DialogResult.OK)
		{
			w_txtTargetFile.Text = saveFileDialog.FileName;
		}
	}

        private void On_treeColumnsToExport_AfterCheck(object sender, TreeViewEventArgs e)
	{
		w_treeColumnsToExport.AfterCheck -= m_treeViewAfterCheckEventHandler;
		foreach (TreeNode node in e.Node.Nodes)
		{
			node.Checked = e.Node.Checked;
		}
		if (e.Node.Parent != null)
		{
			bool flag = true;
			foreach (TreeNode treeNode in e.Node.Parent.Nodes)
			{
				if (treeNode.Checked)
				{
					continue;
				}
				flag = false;
				break;
			}
			e.Node.Parent.Checked = flag;
		}
		w_treeColumnsToExport.AfterCheck += m_treeViewAfterCheckEventHandler;
	}

        public ExportToCSVOptions SaveOptions()
	{
		if (string.IsNullOrEmpty(w_txtTargetFile.Text))
		{
			FlatXtraMessageBox.Show("Please select an output file", "No Output File", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			w_txtTargetFile.Focus();
			return null;
		}
		FileInfo fileInfo = new FileInfo(w_txtTargetFile.Text);
		if (fileInfo.Exists && FlatXtraMessageBox.Show("Output file: \"" + fileInfo.Name + "\" already exists. Do you wish to overwrite it?", "Duplicate", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
		{
			w_txtTargetFile.Focus();
			return null;
		}
		m_options.TargetFilename = w_txtTargetFile.Text;
		m_options.IncludeTypes = w_cbIncludeTypes.Checked;
		m_options.ColumnsToExport.Clear();
		foreach (TreeNode node in w_treeColumnsToExport.Nodes)
		{
			foreach (TreeNode treeNode in node.Nodes)
			{
				if (treeNode.Checked)
				{
					m_options.ColumnsToExport.Add(treeNode.Name);
				}
			}
		}
		return m_options;
	}
    }
}
