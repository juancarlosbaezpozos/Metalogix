using Metalogix;
using Metalogix.Interfaces;
using Metalogix.UI.WinForms.Properties;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms
{
	public class UIMarketingSplashForm : Form
	{
		private const int CountdownCount = 5;

		private IContainer components;

		private DoubleBufferedPanel panelContainer;

		private CheckBox checkBoxDontShow;

		private LinkLabel linkLabelFeatures;

		private LinkLabel linkLabelCaseStudies;

		private LinkLabel linkLabelDownload;

		private LinkLabel linkLabelImage;

		private LinkLabel linkLabelDemo;

		private PictureBox pictureBoxClose;

		public UIMarketingSplashForm.Product DisplayedProduct
		{
			get;
			private set;
		}

	    // Metalogix.UI.WinForms.UIMarketingSplashForm
	    public UIMarketingSplashForm(UIMarketingSplashForm.Product displayProduct = UIMarketingSplashForm.Product.StoragePoint, bool showOptOut = true)
	    {
	        try
	        {
	            this.DisplayedProduct = displayProduct;
	            this.InitializeComponent();
	            this.checkBoxDontShow.Visible = showOptOut;
	            switch (this.DisplayedProduct)
	            {
	                case UIMarketingSplashForm.Product.Replicator:
	                    this.panelContainer.BackgroundImage = Resources.ReplicatorBG;
	                    this.linkLabelDemo.Text = Resources.UIMarketingSplashForm_Replicator_DemoText;
	                    this.linkLabelDownload.Text = Resources.UIMarketingSplashForm_Replicator_DownloadTrialText;
	                    this.linkLabelFeatures.Text = Resources.UIMarketingSplashForm_Replicator_FeaturesText;
	                    break;
	                case UIMarketingSplashForm.Product.StoragePoint:
	                    this.panelContainer.BackgroundImage = Resources.StoragePointBG;
	                    this.linkLabelDemo.Text = Resources.UIMarketingSplashForm_StoragePoint_DemoText;
	                    this.linkLabelDownload.Text = Resources.UIMarketingSplashForm_StoragePoint_DownloadTrialText;
	                    this.linkLabelFeatures.Text = Resources.UIMarketingSplashForm_StoragePoint_FeaturesText;
	                    break;
	                case UIMarketingSplashForm.Product.MetalogixAcademyContentMatrix:
	                    this.panelContainer.BackgroundImage = Resources.MetalogixAcademyContentMatrixBG;
	                    this.linkLabelDemo.Text = null;
	                    this.linkLabelDemo.Visible = false;
	                    this.linkLabelDemo.Enabled = false;
	                    this.linkLabelFeatures.AutoSize = true;
	                    this.linkLabelFeatures.Text = Resources.UIMarketingSplashForm_MetalogixAcademyOverviewText;
	                    this.linkLabelCaseStudies.Location = new System.Drawing.Point(280, 390);
	                    this.linkLabelCaseStudies.AutoSize = true;
	                    this.linkLabelCaseStudies.Text = Resources.UIMarketingSplashForm_MetalogixAcademyContent_MatrixText;
	                    this.linkLabelDownload.Text = null;
	                    this.linkLabelDownload.Enabled = false;
	                    this.linkLabelDownload.Visible = false;
	                    break;
	            }
	            base.Opacity = 0.0;
	        }
	        catch (System.Exception ex)
	        {
	            GlobalServices.ErrorHandler.HandleException("Show Splash Error", string.Format("Error showing marketing splash screen : {0}", ex.Message), ex, ErrorIcon.Warning);
	        }
	    }

        private void buttonClose_MouseEnter(object sender, EventArgs e)
		{
			this.pictureBoxClose.Image = Resources.CloseOn;
		}

		private void buttonClose_MouseLeave(object sender, EventArgs e)
		{
			this.pictureBoxClose.Image = Resources.CloseOff;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void FadeIn()
		{
			if (base.InvokeRequired)
			{
				base.Invoke(new Action(this.FadeIn));
				return;
			}
			int num = (UISettings.IsRemoteSession ? 1 : 10);
			float single = 1f / (float)num;
			while (num > 0)
			{
				num--;
				UIMarketingSplashForm opacity = this;
				opacity.Opacity = opacity.Opacity + (double)single;
				this.Refresh();
				Thread.Sleep(10);
			}
		}

		private void InitializeComponent()
		{
			this.panelContainer = new DoubleBufferedPanel();
			this.pictureBoxClose = new PictureBox();
			this.linkLabelDemo = new LinkLabel();
			this.linkLabelImage = new LinkLabel();
			this.linkLabelDownload = new LinkLabel();
			this.linkLabelCaseStudies = new LinkLabel();
			this.linkLabelFeatures = new LinkLabel();
			this.checkBoxDontShow = new CheckBox();
			this.panelContainer.SuspendLayout();
			((ISupportInitialize)this.pictureBoxClose).BeginInit();
			base.SuspendLayout();
			this.panelContainer.BackColor = Color.Transparent;
			this.panelContainer.BackgroundImageLayout = ImageLayout.Center;
			this.panelContainer.BorderStyle = BorderStyle.FixedSingle;
			this.panelContainer.Controls.Add(this.pictureBoxClose);
			this.panelContainer.Controls.Add(this.linkLabelDemo);
			this.panelContainer.Controls.Add(this.linkLabelImage);
			this.panelContainer.Controls.Add(this.linkLabelDownload);
			this.panelContainer.Controls.Add(this.linkLabelCaseStudies);
			this.panelContainer.Controls.Add(this.linkLabelFeatures);
			this.panelContainer.Controls.Add(this.checkBoxDontShow);
			this.panelContainer.Dock = DockStyle.Fill;
			this.panelContainer.ForeColor = Color.Teal;
			this.panelContainer.Location = new Point(0, 0);
			this.panelContainer.Margin = new System.Windows.Forms.Padding(0);
			this.panelContainer.Name = "panelContainer";
			this.panelContainer.Size = new System.Drawing.Size(700, 480);
			this.panelContainer.TabIndex = 0;
			this.pictureBoxClose.Image = Resources.CloseOff;
			this.pictureBoxClose.InitialImage = null;
			this.pictureBoxClose.Location = new Point(653, 20);
			this.pictureBoxClose.Margin = new System.Windows.Forms.Padding(0);
			this.pictureBoxClose.Name = "pictureBoxClose";
			this.pictureBoxClose.Size = new System.Drawing.Size(30, 30);
			this.pictureBoxClose.TabIndex = 6;
			this.pictureBoxClose.TabStop = false;
			this.pictureBoxClose.Click += new EventHandler(this.pictureBoxClose_Click);
			this.pictureBoxClose.MouseEnter += new EventHandler(this.buttonClose_MouseEnter);
			this.pictureBoxClose.MouseLeave += new EventHandler(this.buttonClose_MouseLeave);
			this.linkLabelDemo.ActiveLinkColor = Color.Black;
			this.linkLabelDemo.BackColor = Color.Transparent;
			this.linkLabelDemo.Font = new System.Drawing.Font("Lucida Sans Unicode", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.linkLabelDemo.Image = Resources.DemoButtonOff;
			this.linkLabelDemo.ImageAlign = ContentAlignment.MiddleLeft;
			this.linkLabelDemo.LinkBehavior = LinkBehavior.NeverUnderline;
			this.linkLabelDemo.LinkColor = Color.Black;
			this.linkLabelDemo.Location = new Point(44, 323);
			this.linkLabelDemo.Name = "linkLabelDemo";
			this.linkLabelDemo.Size = new System.Drawing.Size(217, 48);
			this.linkLabelDemo.TabIndex = 5;
			this.linkLabelDemo.TextAlign = ContentAlignment.MiddleLeft;
			this.linkLabelDemo.VisitedLinkColor = Color.Black;
			this.linkLabelDemo.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabelDemo_LinkClicked);
			this.linkLabelDemo.MouseEnter += new EventHandler(this.linkLabelDemo_MouseEnter);
			this.linkLabelDemo.MouseLeave += new EventHandler(this.linkLabelDemo_MouseLeave);
			this.linkLabelImage.BackColor = Color.Transparent;
			this.linkLabelImage.Cursor = Cursors.Hand;
			this.linkLabelImage.Location = new Point(27, 36);
			this.linkLabelImage.Name = "linkLabelImage";
			this.linkLabelImage.Size = new System.Drawing.Size(643, 340);
			this.linkLabelImage.TabIndex = 3;
			this.linkLabelImage.Click += new EventHandler(this.LinkLabelImageOnClick);
			this.linkLabelDownload.ActiveLinkColor = Color.FromArgb(43, 76, 121);
			this.linkLabelDownload.BackColor = Color.Transparent;
			this.linkLabelDownload.Font = new System.Drawing.Font("Lucida Sans Unicode", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.linkLabelDownload.LinkColor = Color.FromArgb(90, 135, 197);
			this.linkLabelDownload.Location = new Point(437, 390);
			this.linkLabelDownload.Name = "linkLabelDownload";
			this.linkLabelDownload.Size = new System.Drawing.Size(210, 21);
			this.linkLabelDownload.TabIndex = 2;
			this.linkLabelDownload.TextAlign = ContentAlignment.MiddleLeft;
			this.linkLabelDownload.VisitedLinkColor = Color.FromArgb(90, 135, 197);
			this.linkLabelDownload.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabelDownload_LinkClicked);
			this.linkLabelDownload.MouseEnter += new EventHandler(this.linkLabel_MouseEnter);
			this.linkLabelDownload.MouseLeave += new EventHandler(this.linkLabel_MouseLeave);
			this.linkLabelCaseStudies.ActiveLinkColor = Color.FromArgb(43, 76, 121);
			this.linkLabelCaseStudies.BackColor = Color.Transparent;
			this.linkLabelCaseStudies.Font = new System.Drawing.Font("Lucida Sans Unicode", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.linkLabelCaseStudies.LinkColor = Color.FromArgb(90, 135, 197);
			this.linkLabelCaseStudies.Location = new Point(221, 390);
			this.linkLabelCaseStudies.Name = "linkLabelCaseStudies";
			this.linkLabelCaseStudies.Size = new System.Drawing.Size(210, 21);
			this.linkLabelCaseStudies.TabIndex = 0;
			this.linkLabelCaseStudies.TabStop = true;
			this.linkLabelCaseStudies.Text = "Customer Case Studies";
			this.linkLabelCaseStudies.TextAlign = ContentAlignment.MiddleCenter;
			this.linkLabelCaseStudies.VisitedLinkColor = Color.FromArgb(90, 135, 197);
			this.linkLabelCaseStudies.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabelCaseStudies_LinkClicked);
			this.linkLabelCaseStudies.MouseEnter += new EventHandler(this.linkLabel_MouseEnter);
			this.linkLabelCaseStudies.MouseLeave += new EventHandler(this.linkLabel_MouseLeave);
			this.linkLabelFeatures.ActiveLinkColor = Color.FromArgb(43, 76, 121);
			this.linkLabelFeatures.BackColor = Color.Transparent;
			this.linkLabelFeatures.Font = new System.Drawing.Font("Lucida Sans Unicode", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.linkLabelFeatures.LinkColor = Color.FromArgb(90, 135, 197);
			this.linkLabelFeatures.Location = new Point(50, 390);
			this.linkLabelFeatures.Name = "linkLabelFeatures";
			this.linkLabelFeatures.Size = new System.Drawing.Size(164, 21);
			this.linkLabelFeatures.TabIndex = 1;
			this.linkLabelFeatures.TextAlign = ContentAlignment.MiddleRight;
			this.linkLabelFeatures.VisitedLinkColor = Color.FromArgb(90, 135, 197);
			this.linkLabelFeatures.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabelFeatures_LinkClicked);
			this.linkLabelFeatures.MouseEnter += new EventHandler(this.linkLabel_MouseEnter);
			this.linkLabelFeatures.MouseLeave += new EventHandler(this.linkLabel_MouseLeave);
			this.checkBoxDontShow.AutoSize = true;
			this.checkBoxDontShow.BackColor = Color.Transparent;
			this.checkBoxDontShow.Font = new System.Drawing.Font("Lucida Sans Unicode", 6.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.checkBoxDontShow.ForeColor = Color.Black;
			this.checkBoxDontShow.Location = new Point(55, 421);
			this.checkBoxDontShow.Name = "checkBoxDontShow";
			this.checkBoxDontShow.Size = new System.Drawing.Size(164, 18);
			this.checkBoxDontShow.TabIndex = 0;
			this.checkBoxDontShow.Text = "Don't show this screen again.";
			this.checkBoxDontShow.UseVisualStyleBackColor = false;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.ClientSize = new System.Drawing.Size(700, 480);
			base.Controls.Add(this.panelContainer);
			this.DoubleBuffered = true;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "UIMarketingSplashForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Other Metalogix Products";
			base.FormClosing += new FormClosingEventHandler(this.UIMarketingSplashForm_FormClosing);
			base.Shown += new EventHandler(this.UIMarketingSplashForm_Shown);
			this.panelContainer.ResumeLayout(false);
			this.panelContainer.PerformLayout();
			((ISupportInitialize)this.pictureBoxClose).EndInit();
			base.ResumeLayout(false);
		}

		private void linkLabel_MouseEnter(object sender, EventArgs e)
		{
			LinkLabel linkLabel = sender as LinkLabel;
			if (linkLabel == null)
			{
				return;
			}
			linkLabel.LinkColor = Color.FromArgb(43, 76, 121);
		}

		private void linkLabel_MouseLeave(object sender, EventArgs e)
		{
			LinkLabel linkLabel = sender as LinkLabel;
			if (linkLabel == null)
			{
				return;
			}
			linkLabel.LinkColor = Color.FromArgb(90, 135, 197);
		}

		private void linkLabelCaseStudies_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			switch (this.DisplayedProduct)
			{
				case UIMarketingSplashForm.Product.Replicator:
				{
					UIMarketingSplashForm.OpenUrl("http://www.metalogix.com/resources/promo/splash1-3");
					return;
				}
				case UIMarketingSplashForm.Product.StoragePoint:
				{
					UIMarketingSplashForm.OpenUrl("http://www.metalogix.com/resources/promo/splash2-3");
					return;
				}
				case UIMarketingSplashForm.Product.MetalogixAcademyContentMatrix:
				{
					UIMarketingSplashForm.OpenUrl("http://www.metalogix.com/Support/Metalogix-Academy/Content-Matrix.aspx");
					return;
				}
				default:
				{
					return;
				}
			}
		}

		private void linkLabelDemo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			switch (this.DisplayedProduct)
			{
				case UIMarketingSplashForm.Product.Replicator:
				{
					UIMarketingSplashForm.OpenUrl("http://www.metalogix.com/resources/promo/splash1-1");
					return;
				}
				case UIMarketingSplashForm.Product.StoragePoint:
				{
					UIMarketingSplashForm.OpenUrl("http://www.metalogix.com/resources/promo/splash2-1");
					return;
				}
				default:
				{
					return;
				}
			}
		}

		private void linkLabelDemo_MouseEnter(object sender, EventArgs e)
		{
			this.linkLabelDemo.Image = Resources.DemoButtonOn;
		}

		private void linkLabelDemo_MouseLeave(object sender, EventArgs e)
		{
			this.linkLabelDemo.Image = Resources.DemoButtonOff;
		}

		private void linkLabelDownload_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			switch (this.DisplayedProduct)
			{
				case UIMarketingSplashForm.Product.Replicator:
				{
					UIMarketingSplashForm.OpenUrl("http://www.metalogix.com/resources/promo/splash1-4");
					return;
				}
				case UIMarketingSplashForm.Product.StoragePoint:
				{
					UIMarketingSplashForm.OpenUrl("http://www.metalogix.com/resources/promo/splash2-4");
					return;
				}
				default:
				{
					return;
				}
			}
		}

		private void linkLabelFeatures_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			switch (this.DisplayedProduct)
			{
				case UIMarketingSplashForm.Product.Replicator:
				{
					UIMarketingSplashForm.OpenUrl("http://www.metalogix.com/resources/promo/splash1-2");
					return;
				}
				case UIMarketingSplashForm.Product.StoragePoint:
				{
					UIMarketingSplashForm.OpenUrl("http://www.metalogix.com/resources/promo/splash2-2");
					return;
				}
				case UIMarketingSplashForm.Product.MetalogixAcademyContentMatrix:
				{
					UIMarketingSplashForm.OpenUrl("http://www.metalogix.com/Support/Metalogix-Academy");
					return;
				}
				default:
				{
					return;
				}
			}
		}

		private void LinkLabelImageOnClick(object sender, EventArgs eventArgs)
		{
			switch (this.DisplayedProduct)
			{
				case UIMarketingSplashForm.Product.Replicator:
				{
					UIMarketingSplashForm.OpenUrl("http://www.metalogix.com/resources/promo/splash1-1");
					return;
				}
				case UIMarketingSplashForm.Product.StoragePoint:
				{
					UIMarketingSplashForm.OpenUrl("http://www.metalogix.com/resources/promo/splash2-1");
					return;
				}
				case UIMarketingSplashForm.Product.MetalogixAcademyContentMatrix:
				{
					UIMarketingSplashForm.OpenUrl("http://www.metalogix.com/Support/Metalogix-Academy/Content-Matrix.aspx");
					return;
				}
				default:
				{
					return;
				}
			}
		}

		private static void OpenUrl(string sUrl)
		{
			try
			{
				using (Process process = new Process())
				{
					process.StartInfo.FileName = sUrl;
					process.StartInfo.UseShellExecute = true;
					process.Start();
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				GlobalServices.ErrorHandler.HandleException("Open URL Error", string.Format("Error opening URL \"{0}\" : {1}", sUrl, exception.Message), exception, ErrorIcon.Warning);
			}
		}

		private void pictureBoxClose_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		public static void ShowSplashIfRequired(Form parent)
		{
			if (!UIConfigurationVariables.ShowMarketingSplashScreen)
			{
				return;
			}
			int marketingSplashCountdown = UIConfigurationVariables.MarketingSplashCountdown;
			int num = marketingSplashCountdown - 1;
			marketingSplashCountdown = num;
			if (num <= 0)
			{
				marketingSplashCountdown = 5;
				using (UIMarketingSplashForm uIMarketingSplashForm = new UIMarketingSplashForm(UIMarketingSplashForm.Product.MetalogixAcademyContentMatrix, true))
				{
					uIMarketingSplashForm.ShowDialog(parent);
				}
				UIConfigurationVariables.LastMarketingSplashProductShown = UIMarketingSplashForm.Product.MetalogixAcademyContentMatrix.ToString();
			}
			UIConfigurationVariables.MarketingSplashCountdown = marketingSplashCountdown;
		}

		private void UIMarketingSplashForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!this.checkBoxDontShow.Checked)
			{
				return;
			}
			UIConfigurationVariables.ShowMarketingSplashScreen = false;
			UIConfigurationVariables.MarketingSplashCountdown = 5;
		}

		private void UIMarketingSplashForm_Shown(object sender, EventArgs e)
		{
			if (ApplicationData.IsDesignMode())
			{
				return;
			}
			(new Thread(new ThreadStart(this.FadeIn))).Start();
		}

		public enum Product
		{
			Replicator,
			StoragePoint,
			MetalogixAcademyContentMatrix
		}
	}
}