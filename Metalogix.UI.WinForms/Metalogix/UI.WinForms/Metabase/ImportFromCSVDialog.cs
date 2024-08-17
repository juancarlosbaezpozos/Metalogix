using Metalogix.Metabase.Options;
using Metalogix.UI.WinForms;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Metabase
{
	public class ImportFromCSVDialog : Form
	{
		private ImportFromCSVOptions m_options;

		private bool _isExcelFile;

		private IContainer components;

		private CheckBox w_cbOverwriteFullRows;

		private CheckBox w_cbCreateNewColumns;

		private CheckBox w_cbAddNewRows;

		private Label w_lSourceFile;

		private TextBox w_tbSourcePath;

		private Button w_bBrowse;

		private Button w_bCancel;

		private Button w_bOk;

		private GroupBox w_gbOptions;

		private OpenFileDialog w_openFileDialog;

		private CheckBox w_cbIgnoreMetalogixID;

		private CheckBox w_cbVerboseLogging;

		private CheckBox w_cbPreview;

		public bool IsExcelFile
		{
			get
			{
				return this._isExcelFile;
			}
			set
			{
				this._isExcelFile = value;
				if (value)
				{
					this.w_openFileDialog.DefaultExt = "xlsx";
					this.w_openFileDialog.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
					this.Text = "Import From Excel File";
					return;
				}
				this.w_openFileDialog.DefaultExt = "csv";
				this.w_openFileDialog.Filter = "CSV Files|*.csv|All files|*.*";
				this.Text = "Import From CSV File";
			}
		}

		public ImportFromCSVOptions Options
		{
			get
			{
				return this.m_options;
			}
			set
			{
				this.m_options = value;
			}
		}

		public bool ShowAddNewRows
		{
			get
			{
				return this.w_cbAddNewRows.Visible;
			}
			set
			{
				this.w_cbAddNewRows.Visible = value;
			}
		}

		public bool ShowIgnoreMetalogixID
		{
			get
			{
				return this.w_cbIgnoreMetalogixID.Visible;
			}
			set
			{
				this.w_cbIgnoreMetalogixID.Visible = value;
			}
		}

		public ImportFromCSVDialog()
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

		private void InitializeComponent()
		{
			this.w_cbOverwriteFullRows = new CheckBox();
			this.w_cbCreateNewColumns = new CheckBox();
			this.w_cbAddNewRows = new CheckBox();
			this.w_lSourceFile = new Label();
			this.w_tbSourcePath = new TextBox();
			this.w_bBrowse = new Button();
			this.w_bCancel = new Button();
			this.w_bOk = new Button();
			this.w_gbOptions = new GroupBox();
			this.w_cbPreview = new CheckBox();
			this.w_cbVerboseLogging = new CheckBox();
			this.w_cbIgnoreMetalogixID = new CheckBox();
			this.w_openFileDialog = new OpenFileDialog();
			this.w_gbOptions.SuspendLayout();
			base.SuspendLayout();
			this.w_cbOverwriteFullRows.AutoSize = true;
			this.w_cbOverwriteFullRows.Location = new Point(6, 42);
			this.w_cbOverwriteFullRows.Name = "w_cbOverwriteFullRows";
			this.w_cbOverwriteFullRows.Size = new System.Drawing.Size(271, 17);
			this.w_cbOverwriteFullRows.TabIndex = 1;
			this.w_cbOverwriteFullRows.Text = "Allow overwriting non-empty fields with empty values";
			this.w_cbOverwriteFullRows.UseVisualStyleBackColor = true;
			this.w_cbCreateNewColumns.AutoSize = true;
			this.w_cbCreateNewColumns.Location = new Point(6, 65);
			this.w_cbCreateNewColumns.Name = "w_cbCreateNewColumns";
			this.w_cbCreateNewColumns.Size = new System.Drawing.Size(164, 17);
			this.w_cbCreateNewColumns.TabIndex = 2;
			this.w_cbCreateNewColumns.Text = "Allow creating new properties";
			this.w_cbCreateNewColumns.UseVisualStyleBackColor = true;
			this.w_cbAddNewRows.AutoSize = true;
			this.w_cbAddNewRows.Location = new Point(6, 88);
			this.w_cbAddNewRows.Name = "w_cbAddNewRows";
			this.w_cbAddNewRows.Size = new System.Drawing.Size(137, 17);
			this.w_cbAddNewRows.TabIndex = 3;
			this.w_cbAddNewRows.Text = "Allow adding new Items";
			this.w_cbAddNewRows.UseVisualStyleBackColor = true;
			this.w_lSourceFile.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			this.w_lSourceFile.AutoSize = true;
			this.w_lSourceFile.Location = new Point(6, 167);
			this.w_lSourceFile.Name = "w_lSourceFile";
			this.w_lSourceFile.Size = new System.Drawing.Size(60, 13);
			this.w_lSourceFile.TabIndex = 6;
			this.w_lSourceFile.Text = "Source file:";
			this.w_tbSourcePath.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.w_tbSourcePath.Location = new Point(69, 164);
			this.w_tbSourcePath.Name = "w_tbSourcePath";
			this.w_tbSourcePath.Size = new System.Drawing.Size(236, 20);
			this.w_tbSourcePath.TabIndex = 7;
			this.w_bBrowse.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.w_bBrowse.Location = new Point(311, 162);
			this.w_bBrowse.Name = "w_bBrowse";
			this.w_bBrowse.Size = new System.Drawing.Size(75, 23);
			this.w_bBrowse.TabIndex = 8;
			this.w_bBrowse.Text = "Browse...";
			this.w_bBrowse.UseVisualStyleBackColor = true;
			this.w_bBrowse.Click += new EventHandler(this.On_bBrowse_Click);
			this.w_bCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.w_bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_bCancel.Location = new Point(329, 210);
			this.w_bCancel.Name = "w_bCancel";
			this.w_bCancel.Size = new System.Drawing.Size(75, 23);
			this.w_bCancel.TabIndex = 2;
			this.w_bCancel.Text = "Cancel";
			this.w_bCancel.UseVisualStyleBackColor = true;
			this.w_bOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.w_bOk.Location = new Point(248, 210);
			this.w_bOk.Name = "w_bOk";
			this.w_bOk.Size = new System.Drawing.Size(75, 23);
			this.w_bOk.TabIndex = 1;
			this.w_bOk.Text = "OK";
			this.w_bOk.UseVisualStyleBackColor = true;
			this.w_bOk.Click += new EventHandler(this.On_bOk_Click);
			this.w_gbOptions.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.w_gbOptions.Controls.Add(this.w_cbPreview);
			this.w_gbOptions.Controls.Add(this.w_cbVerboseLogging);
			this.w_gbOptions.Controls.Add(this.w_cbIgnoreMetalogixID);
			this.w_gbOptions.Controls.Add(this.w_cbOverwriteFullRows);
			this.w_gbOptions.Controls.Add(this.w_cbCreateNewColumns);
			this.w_gbOptions.Controls.Add(this.w_cbAddNewRows);
			this.w_gbOptions.Controls.Add(this.w_bBrowse);
			this.w_gbOptions.Controls.Add(this.w_tbSourcePath);
			this.w_gbOptions.Controls.Add(this.w_lSourceFile);
			this.w_gbOptions.Location = new Point(12, 12);
			this.w_gbOptions.Name = "w_gbOptions";
			this.w_gbOptions.Size = new System.Drawing.Size(392, 192);
			this.w_gbOptions.TabIndex = 0;
			this.w_gbOptions.TabStop = false;
			this.w_gbOptions.Text = "Options";
			this.w_cbPreview.AutoSize = true;
			this.w_cbPreview.Location = new Point(6, 134);
			this.w_cbPreview.Name = "w_cbPreview";
			this.w_cbPreview.Size = new System.Drawing.Size(142, 17);
			this.w_cbPreview.TabIndex = 5;
			this.w_cbPreview.Text = "Preview before importing";
			this.w_cbPreview.UseVisualStyleBackColor = true;
			this.w_cbVerboseLogging.AutoSize = true;
			this.w_cbVerboseLogging.Location = new Point(6, 111);
			this.w_cbVerboseLogging.Name = "w_cbVerboseLogging";
			this.w_cbVerboseLogging.Size = new System.Drawing.Size(278, 17);
			this.w_cbVerboseLogging.TabIndex = 4;
			this.w_cbVerboseLogging.Text = "Enable logging for failed imports only (Recommended)";
			this.w_cbVerboseLogging.UseVisualStyleBackColor = true;
			this.w_cbIgnoreMetalogixID.AutoSize = true;
			this.w_cbIgnoreMetalogixID.Location = new Point(6, 19);
			this.w_cbIgnoreMetalogixID.Name = "w_cbIgnoreMetalogixID";
			this.w_cbIgnoreMetalogixID.Size = new System.Drawing.Size(301, 17);
			this.w_cbIgnoreMetalogixID.TabIndex = 1;
			this.w_cbIgnoreMetalogixID.Text = "Find existing Items by SourceURL, rather than MetalogixID";
			this.w_cbIgnoreMetalogixID.UseVisualStyleBackColor = true;
			this.w_openFileDialog.DefaultExt = "csv";
			this.w_openFileDialog.Filter = "CSV Files|*.csv|All files|*.*";
			base.AcceptButton = this.w_bOk;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.w_bCancel;
			base.ClientSize = new System.Drawing.Size(416, 245);
			base.Controls.Add(this.w_gbOptions);
			base.Controls.Add(this.w_bOk);
			base.Controls.Add(this.w_bCancel);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ImportFromCSVDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Import From CSV File";
			base.Shown += new EventHandler(this.On_ImportFromCSVDialog_Shown);
			this.w_gbOptions.ResumeLayout(false);
			this.w_gbOptions.PerformLayout();
			base.ResumeLayout(false);
		}

		private void LoadUI()
		{
			if (this.m_options == null)
			{
				return;
			}
			this.w_cbOverwriteFullRows.Checked = this.m_options.OverwriteFullRows;
			this.w_cbCreateNewColumns.Checked = this.m_options.CreateNewColumns;
			this.w_cbAddNewRows.Checked = this.m_options.AddNewRows;
			this.w_cbIgnoreMetalogixID.Checked = this.m_options.IgnoreMetalogixId;
			this.w_tbSourcePath.Text = this.m_options.SourceFileName;
			this.w_cbVerboseLogging.Checked = this.m_options.FailureLogging;
			this.w_cbPreview.Checked = this.m_options.Preview;
		}

		private void On_bBrowse_Click(object sender, EventArgs e)
		{
			this.w_openFileDialog.FileName = this.w_tbSourcePath.Text;
			if (this.w_openFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
			{
				return;
			}
			this.w_tbSourcePath.Text = this.w_openFileDialog.FileName;
		}

		private void On_bOk_Click(object sender, EventArgs e)
		{
			if (File.Exists(this.w_tbSourcePath.Text))
			{
				this.SaveUI();
				base.DialogResult = System.Windows.Forms.DialogResult.OK;
				base.Close();
				return;
			}
			string str = "The specified file does not exist. Please select a valid CSV file.";
			if (this.IsExcelFile)
			{
				str = "The specified file does not exist. Please select a valid Excel file.";
			}
			FlatXtraMessageBox.Show(str, "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			this.w_tbSourcePath.Focus();
		}

		private void On_ImportFromCSVDialog_Shown(object sender, EventArgs e)
		{
			this.LoadUI();
		}

		private void SaveUI()
		{
			this.m_options = new ImportFromCSVOptions()
			{
				OverwriteFullRows = this.w_cbOverwriteFullRows.Checked,
				CreateNewColumns = this.w_cbCreateNewColumns.Checked,
				AddNewRows = this.w_cbAddNewRows.Checked,
				IgnoreMetalogixId = this.w_cbIgnoreMetalogixID.Checked,
				SourceFileName = this.w_tbSourcePath.Text,
				FailureLogging = this.w_cbVerboseLogging.Checked,
				Preview = this.w_cbPreview.Checked
			};
		}
	}
}