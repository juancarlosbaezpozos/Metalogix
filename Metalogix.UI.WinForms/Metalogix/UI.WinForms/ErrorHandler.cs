using DevExpress.XtraEditors;
using Metalogix;
using Metalogix.Actions;
using Metalogix.Interfaces;
using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms
{
	public class ErrorHandler : Metalogix.ErrorHandler
	{
		public ErrorHandler(IErrorFormatter formatter) : base(formatter)
		{
		}

		public override void HandleException(Exception exc)
		{
			if (!(exc is ConditionalDetailException))
			{
				using (ErrorHandlerForm errorHandlerForm = new ErrorHandlerForm(exc))
				{
					errorHandlerForm.ShowDialog();
				}
			}
			else
			{
				using (ErrorHandlerForm errorHandlerForm1 = new ErrorHandlerForm(exc.Message, null))
				{
					errorHandlerForm1.ShowDialog();
				}
			}
		}

		public override void HandleException(string message, Exception exc)
		{
			if (exc is ConditionalDetailException)
			{
				exc = null;
			}
			using (ErrorHandlerForm errorHandlerForm = new ErrorHandlerForm(message, exc))
			{
				errorHandlerForm.ShowDialog();
			}
		}

		public override void HandleException(string message, ErrorIcon icon)
		{
			using (ErrorHandlerForm errorHandlerForm = new ErrorHandlerForm(null, message, icon))
			{
				errorHandlerForm.ShowDialog();
			}
		}

		public override void HandleException(string caption, string message, ErrorIcon icon)
		{
			using (ErrorHandlerForm errorHandlerForm = new ErrorHandlerForm(caption, message, icon))
			{
				errorHandlerForm.ShowDialog();
			}
		}

		public override void HandleException(string caption, string message, Exception exception)
		{
			using (ErrorHandlerForm errorHandlerForm = new ErrorHandlerForm(caption, message, exception, ErrorIcon.Error))
			{
				errorHandlerForm.ShowDialog();
			}
		}

		public override void HandleException(string caption, string message, Exception exc, ErrorIcon icon)
		{
			if (exc is ConditionalDetailException)
			{
				exc = null;
			}
			using (ErrorHandlerForm errorHandlerForm = new ErrorHandlerForm(caption, message, exc, icon))
			{
				errorHandlerForm.ShowDialog();
			}
		}

		public void HandleException(Form owner, Exception exc)
		{
			if (owner != null && owner.InvokeRequired)
			{
				owner.Invoke(new MethodInvoker(() => this.HandleException(owner, exc)));
				return;
			}
			using (ErrorHandlerForm errorHandlerForm = new ErrorHandlerForm(exc))
			{
				if (owner == null || !owner.IsHandleCreated || owner.IsDisposed)
				{
					errorHandlerForm.ShowDialog();
				}
				else
				{
					errorHandlerForm.ShowDialog(owner);
				}
			}
		}

		public void HandleException(Form owner, string message, Exception exc)
		{
			if (owner != null && owner.InvokeRequired)
			{
				owner.Invoke(new MethodInvoker(() => this.HandleException(owner, message, exc)));
				return;
			}
			using (ErrorHandlerForm errorHandlerForm = new ErrorHandlerForm(message, exc))
			{
				errorHandlerForm.ShowDialog(owner);
			}
		}

		public void HandleException(Form owner, string caption, string message, Exception exc, ErrorIcon icon)
		{
			if (owner != null && owner.InvokeRequired)
			{
				owner.Invoke(new MethodInvoker(() => this.HandleException(owner, caption, message, exc, icon)));
				return;
			}
			using (ErrorHandlerForm errorHandlerForm = new ErrorHandlerForm(caption, message, exc, icon))
			{
				errorHandlerForm.ShowDialog(owner);
			}
		}

		public void HandleException(Control owner, Exception exc)
		{
			if (owner == null || !owner.InvokeRequired)
			{
				this.HandleException(owner.FindForm(), exc);
				return;
			}
			owner.Invoke(new MethodInvoker(() => this.HandleException(owner, exc)));
		}

		public void HandleException(Control owner, string message, Exception exc)
		{
			if (owner == null || !owner.InvokeRequired)
			{
				this.HandleException(owner.FindForm(), message, exc);
				return;
			}
			owner.Invoke(new MethodInvoker(() => this.HandleException(owner, message, exc)));
		}

		public void HandleException(Control owner, string caption, string message, Exception exc)
		{
			if (owner == null || !owner.InvokeRequired)
			{
				this.HandleException(owner.FindForm(), caption, message, exc, ErrorIcon.Error);
				return;
			}
			owner.Invoke(new MethodInvoker(() => this.HandleException(owner, caption, message, exc)));
		}
	}
}