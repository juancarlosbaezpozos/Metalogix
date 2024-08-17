using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Metalogix.UI.WinForms.Actions
{
    public partial class PleaseWaitDialog : XtraForm
    {
        protected BackgroundWorker _worker;

        protected Action<BackgroundWorker> _action;

        private readonly object _lock = new object();

        private Exception _operationException;

        private IContainer components;

        private MarqueeProgressBarControl marqueeProgress;

        private LabelControl lbl_Connect;

        private SimpleButton btn_Cancel;

        protected string ConnectTo
        {
            get
            {
                return lbl_Connect.Text;
            }
            set
            {
                lbl_Connect.Text = value;
            }
        }

        protected PleaseWaitDialog()
        {
            InitializeComponent();
            _worker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };
            _worker.DoWork += backgroundWorker_DoWork;
            _worker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _action(sender as BackgroundWorker);
            }
            catch (Exception exception)
            {
                _operationException = exception;
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (base.DialogResult != DialogResult.Cancel)
                {
                    if (e.Error != null)
                    {
                        throw (e.Error.InnerException != null) ? e.Error.InnerException : e.Error;
                    }
                    base.DialogResult = DialogResult.OK;
                }
            }
            finally
            {
                DeRegisterWorker();
                Close();
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ConnectWaitDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btn_Cancel.Visible)
            {
                DeRegisterWorker();
                return;
            }
            lock (_lock)
            {
                if (_worker != null && _worker.IsBusy)
                {
                    e.Cancel = true;
                }
            }
        }

        private void DeRegisterWorker()
        {
            lock (_lock)
            {
                if (_worker != null)
                {
                    if (_worker.WorkerSupportsCancellation && _worker.IsBusy)
                    {
                        _worker.CancelAsync();
                    }
                    _worker.DoWork -= backgroundWorker_DoWork;
                    _worker.RunWorkerCompleted -= backgroundWorker_RunWorkerCompleted;
                    _worker.Dispose();
                    _worker = null;
                }
            }
        }

        private void DisableCancellation()
        {
            btn_Cancel.Visible = false;
            btn_Cancel.Enabled = false;
            base.ControlBox = false;
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
            this.marqueeProgress = new DevExpress.XtraEditors.MarqueeProgressBarControl();
            this.lbl_Connect = new DevExpress.XtraEditors.LabelControl();
            this.btn_Cancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)this.marqueeProgress.Properties).BeginInit();
            base.SuspendLayout();
            this.marqueeProgress.EditValue = 0;
            this.marqueeProgress.Location = new System.Drawing.Point(11, 70);
            this.marqueeProgress.Name = "marqueeProgress";
            this.marqueeProgress.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.marqueeProgress.Size = new System.Drawing.Size(376, 17);
            this.marqueeProgress.TabIndex = 0;
            this.lbl_Connect.AutoEllipsis = true;
            this.lbl_Connect.Location = new System.Drawing.Point(12, 15);
            this.lbl_Connect.Name = "lbl_Connect";
            this.lbl_Connect.Size = new System.Drawing.Size(375, 39);
            this.lbl_Connect.TabIndex = 1;
            this.lbl_Connect.Text = "Connecting to...";
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Location = new System.Drawing.Point(312, 93);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_Cancel.TabIndex = 3;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.Click += new System.EventHandler(btn_Cancel_Click);
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = this.btn_Cancel;
            base.ClientSize = new System.Drawing.Size(398, 125);
            base.Controls.Add(this.btn_Cancel);
            base.Controls.Add(this.lbl_Connect);
            base.Controls.Add(this.marqueeProgress);
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            base.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(420, 164);
            base.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(376, 153);
            base.Name = "PleaseWaitDialog";
            base.ShowIcon = false;
            base.ShowInTaskbar = false;
            base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Please wait...";
            base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(ConnectWaitDialog_FormClosing);
            base.Shown += new System.EventHandler(OnShown);
            ((System.ComponentModel.ISupportInitialize)this.marqueeProgress.Properties).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void OnShown(object sender, EventArgs e)
        {
            _worker.RunWorkerAsync();
        }

        public static bool ShowWaitDialog(string connectMessage, Action<BackgroundWorker> action, bool cancellable = true)
        {
            try
            {
                using (PleaseWaitDialog pleaseWaitDialog = new PleaseWaitDialog())
                {
                    pleaseWaitDialog._action = action;
                    pleaseWaitDialog.ConnectTo = connectMessage ?? string.Empty;
                    if (!cancellable)
                    {
                        pleaseWaitDialog._worker.WorkerSupportsCancellation = false;
                        pleaseWaitDialog.DisableCancellation();
                    }
                    if (pleaseWaitDialog.ShowDialog() != DialogResult.Cancel)
                    {
                        if (pleaseWaitDialog._operationException != null)
                        {
                            throw pleaseWaitDialog._operationException;
                        }
                        return true;
                    }
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                GlobalServices.ErrorHandler.HandleException(exception.Message, exception);
            }
            return false;
        }
    }
}
