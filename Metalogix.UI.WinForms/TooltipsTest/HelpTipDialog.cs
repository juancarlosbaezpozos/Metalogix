using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using Metalogix;
using Metalogix.UI.WinForms.Documentation;

namespace TooltipsTest
{
    public class HelpTipDialog : XtraForm
    {
        public const int dialogHeightAdjust = 10;

        public const int dialogGap = 5;

        public const int dialogWidth = 292;

        protected const int triangleSize = 12;

        protected const int labelGapSize = 13;

        protected bool fadeIn = true;

        protected bool finishedShowing;

        private IContainer components;

        private LinkLabel lbl_Link;

        private Timer FadeTimer;

        private LabelControl lbl_HelpTip;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= 128;
                return createParams;
            }
        }

        public string HelpTipLinkText
        {
            set
            {
                lbl_Link.Links.Clear();
                lbl_Link.Links.Add(0, lbl_Link.Text.Length, value);
            }
        }

        public string HelpTipText
        {
            get
            {
                return lbl_HelpTip.Text;
            }
            set
            {
                lbl_HelpTip.Text = value;
            }
        }

        public bool PlacedOnLeft { get; set; }

        public HelpTipDialog()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, value: true);
            BackColor = Color.Magenta;
            base.TransparencyKey = BackColor;
        }

        public void DisableHelpLink()
        {
            lbl_Link.Visible = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        protected void FadeTimer_Tick(object sender, EventArgs e)
        {
            if (base.Disposing || base.IsDisposed)
            {
                return;
            }
            if (fadeIn)
            {
                Opacity += 0.1;
                if (base.Opacity >= 1.0)
                {
                    base.Opacity = 1.0;
                    FadeTimer.Enabled = false;
                }
                return;
            }
            Opacity -= 0.1;
            if (base.Opacity <= 0.0)
            {
                base.Opacity = 0.0;
                FadeTimer.Enabled = false;
                Dispose();
            }
        }

        protected void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                base.Owner = null;
                Close();
            }
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lbl_Link = new System.Windows.Forms.LinkLabel();
            this.FadeTimer = new System.Windows.Forms.Timer(this.components);
            this.lbl_HelpTip = new DevExpress.XtraEditors.LabelControl();
            base.SuspendLayout();
            this.lbl_Link.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.lbl_Link.AutoSize = true;
            this.lbl_Link.BackColor = System.Drawing.Color.Transparent;
            this.lbl_Link.Location = new System.Drawing.Point(29, 137);
            this.lbl_Link.Name = "lbl_Link";
            this.lbl_Link.Size = new System.Drawing.Size(224, 13);
            this.lbl_Link.TabIndex = 0;
            this.lbl_Link.TabStop = true;
            this.lbl_Link.Text = "Click here for the online help about this topic.";
            this.lbl_Link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(lbl_Link_LinkClicked);
            this.FadeTimer.Interval = 10;
            this.FadeTimer.Tick += new System.EventHandler(FadeTimer_Tick);
            this.lbl_HelpTip.AllowHtmlString = true;
            this.lbl_HelpTip.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.lbl_HelpTip.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.lbl_HelpTip.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lbl_HelpTip.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lbl_HelpTip.Location = new System.Drawing.Point(29, 13);
            this.lbl_HelpTip.Name = "lbl_HelpTip";
            this.lbl_HelpTip.ShowToolTips = false;
            this.lbl_HelpTip.Size = new System.Drawing.Size(244, 0);
            this.lbl_HelpTip.TabIndex = 1;
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.ClientSize = new System.Drawing.Size(292, 162);
            base.Controls.Add(this.lbl_HelpTip);
            base.Controls.Add(this.lbl_Link);
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            base.KeyPreview = true;
            base.LookAndFeel.SkinName = "Office 2013";
            base.Name = "HelpTipDialog";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            base.TopMost = true;
            base.KeyDown += new System.Windows.Forms.KeyEventHandler(Form_KeyDown);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void lbl_Link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Link.LinkData == null)
            {
                return;
            }
            string str = e.Link.LinkData.ToString();
            try
            {
                if (!str.StartsWith("http:", StringComparison.OrdinalIgnoreCase))
                {
                    DocumentationHelper.ShowHelp(this, str);
                }
                else
                {
                    Process.Start(str);
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                GlobalServices.ErrorHandler.HandleException("Help Error", $"Error opening Help file : {exception.Message}", exception, ErrorIcon.Error);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (!e.Cancel && base.Opacity > 0.0)
            {
                fadeIn = false;
                FadeTimer.Enabled = true;
                e.Cancel = true;
            }
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            base.Owner = null;
            Close();
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!base.DesignMode)
            {
                base.Width = 292;
                lbl_HelpTip.Left = 13;
                lbl_Link.Left = 13;
                if (!PlacedOnLeft)
                {
                    lbl_HelpTip.Left += 12;
                    lbl_Link.Left += 12;
                }
                lbl_HelpTip.Width = 254;
                base.Height = (lbl_Link.Visible ? (13 + lbl_HelpTip.Height + 13 + lbl_Link.Height + 13) : (13 + lbl_HelpTip.Height + 13));
                lbl_HelpTip.Top = 13;
                lbl_Link.Top = 13 + lbl_HelpTip.Height + 13;
                fadeIn = true;
                base.Opacity = 0.0;
                FadeTimer.Enabled = true;
            }
            base.OnLoad(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Rectangle displayRectangle = DisplayRectangle;
            displayRectangle.Width--;
            displayRectangle.Height--;
            SolidBrush solidBrush = new SolidBrush(Color.White);
            Pen pen = new Pen(Color.Silver);
            Rectangle x = displayRectangle;
            int num = 4;
            Point[] pointArray;
            Point[] pointArray1;
            if (!PlacedOnLeft)
            {
                x.X += 12;
                x.Width -= 12;
                Point[] point = new Point[3]
                {
                    new Point(x.Left, x.Top + 10 + 20 - num),
                    new Point(x.Left - 12, x.Top + 10 + 10),
                    new Point(x.Left, x.Top + 10 + num)
                };
                pointArray = point;
                Point[] point1 = new Point[8]
                {
                    new Point(x.Left, x.Top),
                    new Point(x.Right, x.Top),
                    new Point(x.Right, x.Bottom),
                    new Point(x.Left, x.Bottom),
                    new Point(x.Left, x.Top + 10 + 20 - num),
                    new Point(x.Left - 12, x.Top + 10 + 10),
                    new Point(x.Left, x.Top + 10 + num),
                    new Point(x.Left, x.Top)
                };
                pointArray1 = point1;
            }
            else
            {
                x.Width -= 12;
                Point[] point2 = new Point[3]
                {
                    new Point(x.Right, x.Top + 10 + 20 - num),
                    new Point(x.Right + 12, x.Top + 10 + 10),
                    new Point(x.Right, x.Top + 10 + num)
                };
                pointArray = point2;
                Point[] pointArray2 = new Point[8]
                {
                    new Point(x.Right, x.Top),
                    new Point(x.Left, x.Top),
                    new Point(x.Left, x.Bottom),
                    new Point(x.Right, x.Bottom),
                    new Point(x.Right, x.Top + 10 + 20 - num),
                    new Point(x.Right + 12, x.Top + 10 + 10),
                    new Point(x.Right, x.Top + 10 + num),
                    new Point(x.Right, x.Top)
                };
                pointArray1 = pointArray2;
            }
            graphics.FillRectangle(solidBrush, x);
            graphics.FillPolygon(solidBrush, pointArray);
            graphics.DrawLines(pen, pointArray1);
        }
    }
}
