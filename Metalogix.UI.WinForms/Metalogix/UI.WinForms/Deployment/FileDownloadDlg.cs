using Metalogix.Connectivity.Proxy;
using Metalogix.Permissions;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Proxy;
using Metalogix.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Deployment
{
	public class FileDownloadDlg : Form
	{
		private FileDownloader _downloader = new FileDownloader();

		private Thread _downloadThread;

		private bool _downloading;

		private string _targetFile;

		private string _sourceURL;

		private bool _downloaded;

		private MLProxy m_proxy;

		private IContainer components;

		private ProgressBar pamProgressBar;

		private Button roundedButtonDownload;

		private Label labelProgress;

		private Button roundedButtonClose;

		private Label labelDesc1;

		private LinkLabel linkLabelProxySettings;

		private Label labelProgressText;

		private Label label1;

		private PictureBox pictureBox;

		public MLProxy Proxy
		{
			get
			{
				return this.m_proxy;
			}
		}

		public string SourceURL
		{
			get
			{
				return this._sourceURL;
			}
			set
			{
				this._sourceURL = value;
			}
		}

		public string TargetFile
		{
			get
			{
				return this._targetFile;
			}
		}

		public FileDownloadDlg()
		{
			this.InitializeComponent();
			this.labelProgress.Text = "";
			this.pictureBox.Image = Resources.Download;
			this._downloader.ProgressChanged += new DownloadProgressHandler(this.downloader_ProgressChanged);
			this._downloader.DownloadComplete += new EventHandler(this._downloader_DownloadComplete);
			base.DialogResult = System.Windows.Forms.DialogResult.None;
		}

		public FileDownloadDlg(MLProxy proxy) : this()
		{
			this.m_proxy = proxy;
		}

		private void _downloader_DownloadComplete(object sender, EventArgs e)
		{
			this.FinalizeDownload();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void downloader_ProgressChanged(object sender, DownloadEventArgs e)
		{
			if (this._downloading)
			{
				string str = string.Format("{0} of {1} KB downloaded ({2}%)", e.CurrentFileSize / (long)1024, e.TotalFileSize / (long)1024, e.PercentDone);
				this.UpdateProgress(str);
				this.IncrementProgressBar(e.PercentDone);
			}
		}

		private void DownloadFile()
		{
			try
			{
				if (this.m_proxy.Enabled)
				{
					WebProxy webProxy = new WebProxy(string.Concat("http://", this.m_proxy.Server, ":", this.m_proxy.Port), true)
					{
						Credentials = new NetworkCredential(this.m_proxy.Credentials.UserName, this.m_proxy.Credentials.Password.ToInsecureString())
					};
					this._downloader.Proxy = webProxy;
				}
				this._downloader.Download(this._sourceURL, Path.GetDirectoryName(this._targetFile), Path.GetFileName(this._targetFile));
			}
			catch (Exception exception)
			{
				this.ShowError(exception.Message);
			}
		}

		private void FinalizeDownload()
		{
			if (base.InvokeRequired)
			{
				base.Invoke(new FileDownloadDlg.DelegateVoid(this.FinalizeDownload));
				return;
			}
			MessageBox.Show(string.Concat("The setup was downloaded successfully.", Environment.NewLine, "Click Install to continue the installation."), "Download complete", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			this._downloaded = true;
			this._downloading = false;
			this.roundedButtonDownload.Text = "Install";
			this.linkLabelProxySettings.Enabled = true;
		}

		private void IncrementProgressBar(int value)
		{
			if (!base.InvokeRequired)
			{
				this.pamProgressBar.Value = value;
				return;
			}
			FileDownloadDlg.DelegateOneInt delegateOneInt = new FileDownloadDlg.DelegateOneInt(this.IncrementProgressBar);
			object[] objArray = new object[] { value };
			base.BeginInvoke(delegateOneInt, objArray);
		}

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(FileDownloadDlg));
			this.pamProgressBar = new ProgressBar();
			this.roundedButtonDownload = new Button();
			this.labelProgress = new Label();
			this.roundedButtonClose = new Button();
			this.labelDesc1 = new Label();
			this.linkLabelProxySettings = new LinkLabel();
			this.labelProgressText = new Label();
			this.label1 = new Label();
			this.pictureBox = new PictureBox();
			((ISupportInitialize)this.pictureBox).BeginInit();
			base.SuspendLayout();
			this.pamProgressBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			this.pamProgressBar.Location = new Point(49, 113);
			this.pamProgressBar.Name = "pamProgressBar";
			this.pamProgressBar.Size = new System.Drawing.Size(418, 16);
			this.pamProgressBar.TabIndex = 2;
			this.roundedButtonDownload.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.roundedButtonDownload.Location = new Point(311, 140);
			this.roundedButtonDownload.Name = "roundedButtonDownload";
			this.roundedButtonDownload.Size = new System.Drawing.Size(75, 23);
			this.roundedButtonDownload.TabIndex = 3;
			this.roundedButtonDownload.Text = "Download";
			this.roundedButtonDownload.Click += new EventHandler(this.roundedButtonDownload_Click);
			this.labelProgress.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			this.labelProgress.Location = new Point(166, 94);
			this.labelProgress.Name = "labelProgress";
			this.labelProgress.Size = new System.Drawing.Size(301, 16);
			this.labelProgress.TabIndex = 4;
			this.labelProgress.Text = "(progress)";
			this.roundedButtonClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.roundedButtonClose.Location = new Point(392, 140);
			this.roundedButtonClose.Name = "roundedButtonClose";
			this.roundedButtonClose.Size = new System.Drawing.Size(75, 23);
			this.roundedButtonClose.TabIndex = 6;
			this.roundedButtonClose.Text = "Close";
			this.roundedButtonClose.Click += new EventHandler(this.roundedButtonClose_Click);
			this.labelDesc1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			this.labelDesc1.Location = new Point(46, 43);
			this.labelDesc1.Name = "labelDesc1";
			this.labelDesc1.Size = new System.Drawing.Size(421, 16);
			this.labelDesc1.TabIndex = 7;
			this.labelDesc1.Text = "To download the latest version, please click on Download button.";
			this.labelDesc1.TextAlign = ContentAlignment.MiddleLeft;
			this.linkLabelProxySettings.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			this.linkLabelProxySettings.LinkArea = new LinkArea(78, 4);
			this.linkLabelProxySettings.Location = new Point(47, 63);
			this.linkLabelProxySettings.Name = "linkLabelProxySettings";
			this.linkLabelProxySettings.Size = new System.Drawing.Size(418, 16);
			this.linkLabelProxySettings.TabIndex = 8;
			this.linkLabelProxySettings.TabStop = true;
			this.linkLabelProxySettings.Text = "If you access the Internet via a proxy server, enter the required information here.";
			this.linkLabelProxySettings.TextAlign = ContentAlignment.MiddleLeft;
			this.linkLabelProxySettings.UseCompatibleTextRendering = true;
			this.linkLabelProxySettings.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabelProxySettings_LinkClicked);
			this.labelProgressText.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			this.labelProgressText.Location = new Point(46, 94);
			this.labelProgressText.Name = "labelProgressText";
			this.labelProgressText.Size = new System.Drawing.Size(114, 16);
			this.labelProgressText.TabIndex = 9;
			this.labelProgressText.Text = "Download progress:";
			this.label1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			this.label1.Font = new System.Drawing.Font("Tahoma", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 238);
			this.label1.ForeColor = Color.DarkSlateGray;
			this.label1.Location = new Point(46, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(421, 32);
			this.label1.TabIndex = 11;
			this.label1.Text = "New version is available for download.";
			this.label1.TextAlign = ContentAlignment.MiddleLeft;
			this.pictureBox.Image = (Image)componentResourceManager.GetObject("pictureBox.Image");
			this.pictureBox.Location = new Point(8, 8);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(32, 32);
			this.pictureBox.TabIndex = 13;
			this.pictureBox.TabStop = false;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			base.ClientSize = new System.Drawing.Size(479, 169);
			base.Controls.Add(this.linkLabelProxySettings);
			base.Controls.Add(this.pictureBox);
			base.Controls.Add(this.labelDesc1);
			base.Controls.Add(this.labelProgressText);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.roundedButtonClose);
			base.Controls.Add(this.labelProgress);
			base.Controls.Add(this.roundedButtonDownload);
			base.Controls.Add(this.pamProgressBar);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "FileDownloadDlg";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Download and Install Update";
			((ISupportInitialize)this.pictureBox).EndInit();
			base.ResumeLayout(false);
		}

		private void InitProgressBar(int max)
		{
			if (!base.InvokeRequired)
			{
				this.roundedButtonClose.Text = "Close";
				this.pamProgressBar.Value = 0;
				this.pamProgressBar.Maximum = max;
				return;
			}
			FileDownloadDlg.DelegateOneInt delegateOneInt = new FileDownloadDlg.DelegateOneInt(this.InitProgressBar);
			object[] objArray = new object[] { max };
			base.BeginInvoke(delegateOneInt, objArray);
		}

		private void linkLabelProxySettings_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			ProxyDlg proxyDlg = new ProxyDlg();
			if (this.m_proxy != null)
			{
				proxyDlg.Proxy = this.m_proxy;
			}
			proxyDlg.ShowEnabled = true;
			if (proxyDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				this.m_proxy = proxyDlg.Proxy;
			}
		}

		private void roundedButtonClose_Click(object sender, EventArgs e)
		{
			if (this._downloading && FlatXtraMessageBox.Show("Would you like to cancel the download process?", "Cancel download", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
			{
				return;
			}
			if (this._downloading)
			{
				this._downloader.Cancel();
			}
			base.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			base.Close();
		}

		private void roundedButtonDownload_Click(object sender, EventArgs e)
		{
			if (string.Compare(this.roundedButtonDownload.Text, "Install", true) == 0)
			{
				base.DialogResult = (this._downloaded ? System.Windows.Forms.DialogResult.OK : System.Windows.Forms.DialogResult.Cancel);
				base.Close();
				return;
			}
			if (this._downloading)
			{
				if (FlatXtraMessageBox.Show("Would you like to cancel the download process?", "Cancel download", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
				{
					return;
				}
				this._downloader.Cancel();
				this._downloading = false;
				this.linkLabelProxySettings.Enabled = true;
				this.roundedButtonDownload.Text = "Download";
				this.UpdateProgress("");
				this.InitProgressBar(100);
			}
			else
			{
				using (SaveFileDialog saveFileDialog = new SaveFileDialog())
				{
					int num = this._sourceURL.LastIndexOf('.');
					string str = null;
					if (num >= 0)
					{
						string str1 = this._sourceURL.Substring(num + 1);
						str = string.Format("{0} files (*.{1})|*.{1}|", str1.ToUpperInvariant(), str1);
					}
					saveFileDialog.Filter = string.Concat(str, "All files (*.*)|*.*");
					saveFileDialog.FilterIndex = 1;
					saveFileDialog.FileName = Path.GetFileName(this._sourceURL);
					if (saveFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
					{
						return;
					}
					else
					{
						this._targetFile = saveFileDialog.FileName;
						this.InitProgressBar(100);
						this._downloadThread = new Thread(new ThreadStart(this.DownloadFile))
						{
							IsBackground = true
						};
						this._downloadThread.Start();
						this.roundedButtonDownload.Text = "Cancel";
						this.linkLabelProxySettings.Enabled = false;
						this._downloading = true;
					}
				}
			}
		}

		private void SetProgressBarMax()
		{
			if (base.InvokeRequired)
			{
				base.BeginInvoke(new FileDownloadDlg.DelegateVoid(this.SetProgressBarMax));
				return;
			}
			this.pamProgressBar.Value = this.pamProgressBar.Maximum;
		}

		private void ShowError(string error)
		{
			if (!base.InvokeRequired)
			{
				MessageBox.Show(string.Concat(error, "\nIn case of connection problems, please, check the proxy settings."), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			FileDownloadDlg.DelegateOneString delegateOneString = new FileDownloadDlg.DelegateOneString(this.ShowError);
			object[] objArray = new object[] { error };
			base.BeginInvoke(delegateOneString, objArray);
		}

		private void UpdateProgress(string text)
		{
			if (!base.InvokeRequired)
			{
				this.labelProgress.Text = text;
				return;
			}
			FileDownloadDlg.DelegateOneString delegateOneString = new FileDownloadDlg.DelegateOneString(this.UpdateProgress);
			object[] objArray = new object[] { text };
			base.Invoke(delegateOneString, objArray);
		}

		public delegate void DelegateOneInt(int val);

		public delegate void DelegateOneString(string text);

		public delegate void DelegateVoid();
	}
}