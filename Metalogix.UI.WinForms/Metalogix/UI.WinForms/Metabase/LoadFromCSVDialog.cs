using Metalogix;
using Metalogix.Explorer;
using Metalogix.Interfaces;
using Metalogix.Metabase;
using Metalogix.Metabase.Actions;
using Metalogix.Metabase.DataTypes;
using Metalogix.UI.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Metabase
{
	public class LoadFromCSVDialog : Form
	{
		private BackgroundWorker _worker;

		private IContainer components;

		private TextBox w_textBoxCsvFile;

		private Button w_buttonOpenCsvFile;

		private Label label1;

		private ComboBox w_comboBoxSeparator;

		private Label label2;

		private ComboBox w_comboBoxIDColumn;

		private Label label3;

		private Label label4;

		private ProgressBar w_progressBar;

		private Button w_buttonCancel;

		private Button w_buttonImport;

		private GroupBox groupBox1;

		private Label w_labelMessage;

		private Label w_labelStatus;

		private Button buttonClose;

		public Metalogix.Explorer.Node Node
		{
			get;
			set;
		}

		public Metalogix.Metabase.Workspace Workspace
		{
			get;
			set;
		}

		public LoadFromCSVDialog()
		{
			this.InitializeComponent();
			this.Initialize();
		}

	    // Metalogix.UI.WinForms.Metabase.LoadFromCSVDialog
	    private void _worker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
	    {
	        try
	        {
	            LoadFromCSVOptions loadFromCSVOptions = (LoadFromCSVOptions)e.Argument;
	            System.ComponentModel.PropertyDescriptorCollection properties = this.Node.GetProperties();
	            if (loadFromCSVOptions != null)
	            {
	                char c = System.Convert.ToChar(loadFromCSVOptions.Separator);
	                string csvFile = loadFromCSVOptions.CsvFile;
	                int idIndex = loadFromCSVOptions.IdIndex;
	                using (System.IO.FileStream fileStream = new System.IO.FileStream(csvFile, System.IO.FileMode.Open))
	                {
	                    using (System.IO.StreamReader streamReader = new System.IO.StreamReader(fileStream))
	                    {
	                        string text = streamReader.ReadLine();
	                        string[] array = text.Split(new char[]
	                        {
	                            c
	                        });
	                        System.ComponentModel.PropertyDescriptor[] array2 = new System.ComponentModel.PropertyDescriptor[array.Length];
	                        for (int i = 0; i < array.Length; i++)
	                        {
	                            string text2 = array[i];
	                            int num;
	                            string text3;
	                            System.Type type;
	                            if (text2.EndsWith(")") && (num = text2.LastIndexOf("(", System.StringComparison.Ordinal)) != -1)
	                            {
	                                text3 = text2.Substring(0, num).Trim(new char[]
	                                {
	                                    ' '
	                                });
	                                string text4 = text2.Substring(num).Trim(new char[]
	                                {
	                                    '(',
	                                    ')',
	                                    ' '
	                                });
	                                type = System.Type.GetType(text4);
	                                if (type == null)
	                                {
	                                    if (string.Equals(typeof(Url).FullName, text4))
	                                    {
	                                        type = typeof(Url);
	                                    }
	                                    else if (string.Equals(typeof(TextMoniker).FullName, text4))
	                                    {
	                                        type = typeof(TextMoniker);
	                                    }
	                                    else
	                                    {
	                                        type = typeof(string);
	                                    }
	                                }
	                            }
	                            else
	                            {
	                                text3 = text2;
	                                type = typeof(string);
	                            }
	                            if (properties == null || properties[text3] == null || !properties[text3].IsReadOnly)
	                            {
	                                array2[i] = this.Workspace.EnsureProperty(text3, type, null);
	                            }
	                        }
	                        this.Workspace.CommitChanges();
	                        RecordList records = this.Workspace.Records;
	                        records.Clear();
	                        int num2 = 0;
	                        while (!streamReader.EndOfStream)
	                        {
	                            string text5 = streamReader.ReadLine();
	                            if (!string.IsNullOrEmpty(text5))
	                            {
	                                if (this._worker.CancellationPending)
	                                {
	                                    records.CommitChanges();
	                                    e.Cancel = true;
	                                    return;
	                                }
	                                System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
	                                string text6 = c.ToString(System.Globalization.CultureInfo.InvariantCulture);
	                                while (!string.IsNullOrEmpty(text5))
	                                {
	                                    int num3 = 0;
	                                    int length = text5.Length;
	                                    bool flag = false;
	                                    bool flag2 = text5.StartsWith("\"");
	                                    while (!flag && num3 <= length)
	                                    {
	                                        if ((!flag2 && num3 == length) || (!flag2 && text5.Substring(num3, 1) == text6) || (flag2 && num3 == length - 1 && text5.EndsWith("\"")) || (flag2 && text5.Substring(num3, 2) == "\"" + text6))
	                                        {
	                                            flag = true;
	                                        }
	                                        else
	                                        {
	                                            num3++;
	                                        }
	                                    }
	                                    if (flag2)
	                                    {
	                                        if (num3 > length || !text5.Substring(num3, 1).StartsWith("\""))
	                                        {
	                                            throw new System.FormatException("Invalid CSV format: " + text5.Substring(0, num3));
	                                        }
	                                        num3++;
	                                    }
	                                    string text7 = text5.Substring(0, num3).Replace("\"\"", "\"");
	                                    if (num3 < length)
	                                    {
	                                        text5 = text5.Substring(num3 + 1);
	                                    }
	                                    else
	                                    {
	                                        text5 = "";
	                                    }
	                                    if (flag2)
	                                    {
	                                        if (text7.StartsWith("\""))
	                                        {
	                                            text7 = text7.Substring(1);
	                                        }
	                                        if (text7.EndsWith("\""))
	                                        {
	                                            text7 = text7.Substring(0, text7.Length - 1);
	                                        }
	                                    }
	                                    list.Add(text7);
	                                }
	                                string text8 = list[idIndex];
	                                Record rec = records.AddNew(System.Guid.NewGuid(), text8);
	                                num2++;
	                                if (num2 % 500 == 0)
	                                {
	                                    this._worker.ReportProgress((int)((double)fileStream.Position / (double)fileStream.Length * 100.0), text8);
	                                }
	                                for (int j = 0; j < list.Count; j++)
	                                {
	                                    if (j != idIndex)
	                                    {
	                                        System.ComponentModel.PropertyDescriptor propertyDescriptor = array2[j];
	                                        if (propertyDescriptor != null && !propertyDescriptor.IsReadOnly)
	                                        {
	                                            string text9 = list[j];
	                                            if (!string.IsNullOrEmpty(text9))
	                                            {
	                                                if (propertyDescriptor.Name == "MigratedURL")
	                                                {
	                                                    text9 = " ";
	                                                }
	                                                this.SetPropertyValue(propertyDescriptor, rec, text9);
	                                            }
	                                        }
	                                    }
	                                }
	                            }
	                        }
	                        records.CommitChanges();
	                        ExplorerNode explorerNode = this.Node as ExplorerNode;
	                        if (explorerNode != null)
	                        {
	                            explorerNode.Refresh();
	                        }
	                    }
	                }
	            }
	        }
	        catch (System.Exception exc)
	        {
	            GlobalServices.ErrorHandler.HandleException(exc);
	        }
	    }

        private void _worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			this.w_progressBar.Value = e.ProgressPercentage;
			this.w_labelMessage.Text = string.Format("Importing node '{0}'...", e.UserState);
		}

		private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			try
			{
				if (!e.Cancelled)
				{
					this.w_labelStatus.Text = (e.Error == null ? "Successfully Completed" : string.Format("Error: {0}", e.Error.Message));
					this.w_progressBar.Value = this.w_progressBar.Maximum;
				}
				else
				{
					this.w_labelStatus.Text = "Cancelled";
				}
			}
			finally
			{
				this._worker.Dispose();
				this._worker = null;
				this.w_textBoxCsvFile.Enabled = true;
				this.w_buttonOpenCsvFile.Enabled = true;
				this.w_comboBoxSeparator.Enabled = true;
				this.w_comboBoxIDColumn.Enabled = true;
				this.w_buttonImport.Visible = true;
				this.w_buttonCancel.Visible = false;
				this.buttonClose.Enabled = true;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void Initialize()
		{
			this.w_comboBoxSeparator.Items.Add(SeparatorTypes.Comma);
			this.w_comboBoxSeparator.Items.Add(SeparatorTypes.Space);
			this.w_comboBoxSeparator.Items.Add(SeparatorTypes.Tab);
			this.w_comboBoxSeparator.SelectedIndex = 0;
		}

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(LoadFromCSVDialog));
			this.w_textBoxCsvFile = new TextBox();
			this.w_buttonOpenCsvFile = new Button();
			this.label1 = new Label();
			this.w_comboBoxSeparator = new ComboBox();
			this.label2 = new Label();
			this.w_comboBoxIDColumn = new ComboBox();
			this.label3 = new Label();
			this.label4 = new Label();
			this.w_progressBar = new ProgressBar();
			this.w_buttonCancel = new Button();
			this.w_buttonImport = new Button();
			this.groupBox1 = new GroupBox();
			this.w_labelMessage = new Label();
			this.w_labelStatus = new Label();
			this.buttonClose = new Button();
			this.groupBox1.SuspendLayout();
			base.SuspendLayout();
			this.w_textBoxCsvFile.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			this.w_textBoxCsvFile.Location = new Point(125, 27);
			this.w_textBoxCsvFile.Name = "w_textBoxCsvFile";
			this.w_textBoxCsvFile.Size = new System.Drawing.Size(229, 20);
			this.w_textBoxCsvFile.TabIndex = 0;
			this.w_textBoxCsvFile.TextChanged += new EventHandler(this.w_textBoxCsvFile_TextChanged);
			this.w_buttonOpenCsvFile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			this.w_buttonOpenCsvFile.Image = (Image)componentResourceManager.GetObject("w_buttonOpenCsvFile.Image");
			this.w_buttonOpenCsvFile.Location = new Point(360, 24);
			this.w_buttonOpenCsvFile.Name = "w_buttonOpenCsvFile";
			this.w_buttonOpenCsvFile.Size = new System.Drawing.Size(30, 25);
			this.w_buttonOpenCsvFile.TabIndex = 1;
			this.w_buttonOpenCsvFile.UseVisualStyleBackColor = true;
			this.w_buttonOpenCsvFile.Click += new EventHandler(this.w_buttonOpenCsvFile_Click);
			this.label1.AutoSize = true;
			this.label1.Location = new Point(15, 30);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(75, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "CSV File Path:";
			this.w_comboBoxSeparator.DropDownStyle = ComboBoxStyle.DropDownList;
			this.w_comboBoxSeparator.FormattingEnabled = true;
			this.w_comboBoxSeparator.Location = new Point(125, 53);
			this.w_comboBoxSeparator.Name = "w_comboBoxSeparator";
			this.w_comboBoxSeparator.Size = new System.Drawing.Size(83, 21);
			this.w_comboBoxSeparator.TabIndex = 3;
			this.label2.AutoSize = true;
			this.label2.Location = new Point(15, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(94, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Column Separator:";
			this.w_comboBoxIDColumn.DropDownStyle = ComboBoxStyle.DropDownList;
			this.w_comboBoxIDColumn.Enabled = false;
			this.w_comboBoxIDColumn.FormattingEnabled = true;
			this.w_comboBoxIDColumn.Location = new Point(125, 80);
			this.w_comboBoxIDColumn.Name = "w_comboBoxIDColumn";
			this.w_comboBoxIDColumn.Size = new System.Drawing.Size(135, 21);
			this.w_comboBoxIDColumn.TabIndex = 5;
			this.label3.AutoSize = true;
			this.label3.Location = new Point(15, 83);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(104, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "SourceURL Column:";
			this.label4.AutoSize = true;
			this.label4.Location = new Point(9, 9);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(243, 13);
			this.label4.TabIndex = 7;
			this.label4.Text = "Please choose the CSV file to load metadata from:";
			this.w_progressBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			this.w_progressBar.Location = new Point(6, 19);
			this.w_progressBar.Name = "w_progressBar";
			this.w_progressBar.Size = new System.Drawing.Size(366, 23);
			this.w_progressBar.TabIndex = 8;
			this.w_buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.w_buttonCancel.Location = new Point(234, 187);
			this.w_buttonCancel.Name = "w_buttonCancel";
			this.w_buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.w_buttonCancel.TabIndex = 9;
			this.w_buttonCancel.Text = "Cancel";
			this.w_buttonCancel.UseVisualStyleBackColor = true;
			this.w_buttonCancel.Visible = false;
			this.w_buttonCancel.Click += new EventHandler(this.w_buttonCancel_Click);
			this.w_buttonImport.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.w_buttonImport.Location = new Point(234, 187);
			this.w_buttonImport.Name = "w_buttonImport";
			this.w_buttonImport.Size = new System.Drawing.Size(75, 23);
			this.w_buttonImport.TabIndex = 10;
			this.w_buttonImport.Text = "Import";
			this.w_buttonImport.UseVisualStyleBackColor = true;
			this.w_buttonImport.Click += new EventHandler(this.w_buttonImport_Click);
			this.groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.groupBox1.Controls.Add(this.w_labelMessage);
			this.groupBox1.Controls.Add(this.w_progressBar);
			this.groupBox1.Location = new Point(12, 114);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(378, 67);
			this.groupBox1.TabIndex = 11;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Details";
			this.w_labelMessage.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.w_labelMessage.ForeColor = SystemColors.ButtonShadow;
			this.w_labelMessage.Location = new Point(6, 45);
			this.w_labelMessage.Name = "w_labelMessage";
			this.w_labelMessage.Size = new System.Drawing.Size(366, 19);
			this.w_labelMessage.TabIndex = 9;
			this.w_labelMessage.Text = "Import not started.";
			this.w_labelStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			this.w_labelStatus.AutoSize = true;
			this.w_labelStatus.Location = new Point(13, 192);
			this.w_labelStatus.Name = "w_labelStatus";
			this.w_labelStatus.Size = new System.Drawing.Size(0, 13);
			this.w_labelStatus.TabIndex = 12;
			this.buttonClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonClose.Location = new Point(315, 187);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(75, 23);
			this.buttonClose.TabIndex = 13;
			this.buttonClose.Text = "Close";
			this.buttonClose.UseVisualStyleBackColor = true;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(398, 222);
			base.ControlBox = false;
			base.Controls.Add(this.buttonClose);
			base.Controls.Add(this.w_labelStatus);
			base.Controls.Add(this.groupBox1);
			base.Controls.Add(this.w_buttonImport);
			base.Controls.Add(this.w_buttonCancel);
			base.Controls.Add(this.label4);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.w_comboBoxIDColumn);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.w_comboBoxSeparator);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.w_buttonOpenCsvFile);
			base.Controls.Add(this.w_textBoxCsvFile);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "LoadFromCSVDialog";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Load Metadata From CSV File";
			this.groupBox1.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void SetPropertyValue(PropertyDescriptor pd, Record rec, object oValue)
		{
			if (oValue != null)
			{
				if (pd.PropertyType == typeof(DateTime) && !(oValue is DateTime))
				{
					DateTime now = DateTime.Now;
					if (!DateTime.TryParseExact(oValue.ToString(), "yyyy-MM-ddtthh:mm:ssz", null, DateTimeStyles.None, out now))
					{
						oValue = null;
					}
					else
					{
						if (now.Year < 1753)
						{
							now = now.AddYears(1753 - now.Year);
						}
						oValue = now;
					}
				}
				else if (typeof(ISmartDataType).IsAssignableFrom(pd.PropertyType))
				{
					RecordPropertyDescriptor item = rec.GetProperties()[pd.Name] as RecordPropertyDescriptor;
					if (item == null)
					{
						oValue = null;
					}
					else
					{
						string str = oValue as string;
						oValue = DataTypeUtils.CreateInstance(pd.PropertyType, item);
						(oValue as ISmartDataType).DeserializeFromUserFriendlyString(str);
					}
				}
				else if (!typeof(TextMoniker).IsAssignableFrom(pd.PropertyType))
				{
					oValue = Record.DeserializeValue(rec, pd.Name, oValue, pd.PropertyType);
				}
				else
				{
					string str1 = oValue as string;
					oValue = Record.DeserializeValue(rec, pd.Name, oValue, pd.PropertyType);
					TextMoniker textMoniker = oValue as TextMoniker;
					if (textMoniker != null && str1 != null)
					{
						textMoniker.SetFullText(str1);
					}
				}
			}
			if (oValue != null && pd is RecordPropertyDescriptor && pd.PropertyType == typeof(string) && oValue.ToString().Length > 256)
			{
				rec.ParentWorkspace.ExpandPropertyStorage((RecordPropertyDescriptor)pd);
				pd = rec.ParentWorkspace.Records.GetItemProperties(null)[pd.Name];
			}
			if (!pd.IsReadOnly || typeof(TextMoniker).IsAssignableFrom(pd.PropertyType))
			{
				pd.SetValue(rec, oValue);
			}
		}

		private void w_buttonCancel_Click(object sender, EventArgs e)
		{
			this._worker.CancelAsync();
		}

		private void w_buttonImport_Click(object sender, EventArgs e)
		{
			if (!File.Exists(this.w_textBoxCsvFile.Text))
			{
				FlatXtraMessageBox.Show("Csv file does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (this.w_comboBoxSeparator.SelectedItem == null)
			{
				FlatXtraMessageBox.Show("Separator cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (this.w_comboBoxIDColumn.SelectedItem == null)
			{
				FlatXtraMessageBox.Show("Id cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (FlatXtraMessageBox.Show("Are you sure you want to overwrite all records inside of this workspace with the metadata inside of the CSV file?", "Overwrite workspace", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != System.Windows.Forms.DialogResult.Yes)
			{
				return;
			}
			LoadFromCSVOptions loadFromCSVOption = new LoadFromCSVOptions()
			{
				CsvFile = this.w_textBoxCsvFile.Text,
				Separator = (SeparatorTypes)this.w_comboBoxSeparator.SelectedItem,
				IdIndex = this.w_comboBoxIDColumn.SelectedIndex
			};
			this.w_labelStatus.Text = "";
			this.w_labelMessage.Text = "Preparing to start...";
			if (this._worker == null)
			{
				this._worker = new BackgroundWorker()
				{
					WorkerReportsProgress = true,
					WorkerSupportsCancellation = true
				};
				this._worker.DoWork += new DoWorkEventHandler(this._worker_DoWork);
				this._worker.ProgressChanged += new ProgressChangedEventHandler(this._worker_ProgressChanged);
				this._worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this._worker_RunWorkerCompleted);
				this._worker.RunWorkerAsync(loadFromCSVOption);
			}
			this.w_textBoxCsvFile.Enabled = false;
			this.w_buttonOpenCsvFile.Enabled = false;
			this.w_comboBoxSeparator.Enabled = false;
			this.w_comboBoxIDColumn.Enabled = false;
			this.w_buttonImport.Visible = false;
			this.w_buttonCancel.Visible = true;
			this.w_progressBar.Value = this.w_progressBar.Minimum;
			this.buttonClose.Enabled = false;
		}

		private void w_buttonOpenCsvFile_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.RestoreDirectory = false;
				openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				openFileDialog.CheckFileExists = true;
				openFileDialog.Filter = "CSV (Comma separated) (*.csv) | *.csv";
				openFileDialog.Multiselect = false;
				openFileDialog.FileName = this.w_textBoxCsvFile.Text;
				if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					this.w_textBoxCsvFile.Text = openFileDialog.FileName;
				}
			}
		}

		private void w_textBoxCsvFile_TextChanged(object sender, EventArgs e)
		{
			try
			{
				this.w_comboBoxIDColumn.Items.Clear();
				string text = this.w_textBoxCsvFile.Text;
				if (File.Exists(text))
				{
					using (StreamReader streamReader = new StreamReader(text))
					{
						char chr = Convert.ToChar(this.w_comboBoxSeparator.SelectedItem);
						string str = streamReader.ReadLine();
						if (!string.IsNullOrEmpty(str))
						{
							string[] strArrays = str.Split(new char[] { chr });
							this.w_comboBoxIDColumn.Items.AddRange(strArrays);
							this.w_comboBoxIDColumn.SelectedIndex = 0;
						}
					}
				}
			}
			finally
			{
				this.w_comboBoxIDColumn.Enabled = this.w_comboBoxIDColumn.Items.Count != 0;
			}
		}
	}
}