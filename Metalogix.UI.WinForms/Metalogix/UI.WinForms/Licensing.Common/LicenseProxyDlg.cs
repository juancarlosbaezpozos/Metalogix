using Metalogix;
using Metalogix.Connectivity.Proxy;
using Metalogix.Licensing.Common;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Proxy;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Licensing.Common
{
	internal class LicenseProxyDlg : ProxyDlg
	{
		private readonly ILicensingDialogServiceProvider _server;

		private IContainer components;

		public LicenseProxyDlg(ILicensingDialogServiceProvider server)
		{
			base.TopMost = true;
			this.InitializeComponent();
			base.BringToFront();
			this._server = server;
			base.Proxy = this._server.GetLicenseProxy();
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
			base.ShowInTaskbar = false;
		}

		protected override bool ValidateInput(out string sWarningMessage)
		{
			bool flag;
			bool flag1 = base.ValidateInput(out sWarningMessage);
			MLProxy proxy = base.Proxy;
			if (!flag1 || proxy == null)
			{
				return flag1;
			}
			try
			{
				using (AutoWaitCursor autoWaitCursor = AutoWaitCursor.Create())
				{
					this._server.SetLicenseProxy(proxy);
				}
				return true;
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Logger.Error.Write("Failed to update proxy settings.", exception);
				GlobalServices.GetErrorHandlerAs<Metalogix.UI.WinForms.ErrorHandler>().HandleException(this, this._server.DialogTitle, string.Format("Failed to update the proxy settings.{0}{1}", Environment.NewLine, exception.Message), exception, ErrorIcon.Error);
				flag = false;
			}
			return flag;
		}
	}
}