using DevExpress.XtraEditors;
using Metalogix.UI.WinForms.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms
{
	public static class FlatXtraMessageBox
	{
		private static Dictionary<MessageBoxIcon, Icon> s_icons;

		private static ReaderWriterLockSlim s_lock;

		static FlatXtraMessageBox()
		{
			FlatXtraMessageBox.s_icons = new Dictionary<MessageBoxIcon, Icon>();
			FlatXtraMessageBox.s_lock = new ReaderWriterLockSlim();
		}

		private static DialogResult[] MessageBoxButtonsToDialogResults(MessageBoxButtons buttons)
		{
			if (!Enum.IsDefined(typeof(MessageBoxButtons), buttons))
			{
				throw new InvalidEnumArgumentException("buttons", (int)buttons, typeof(DialogResult));
			}
			switch (buttons)
			{
				case MessageBoxButtons.OK:
				{
					return new DialogResult[] { DialogResult.OK };
				}
				case MessageBoxButtons.OKCancel:
				{
					return new DialogResult[] { DialogResult.OK, DialogResult.Cancel };
				}
				case MessageBoxButtons.AbortRetryIgnore:
				{
					return new DialogResult[] { DialogResult.Abort, DialogResult.Retry, DialogResult.Ignore };
				}
				case MessageBoxButtons.YesNoCancel:
				{
					return new DialogResult[] { DialogResult.Yes, DialogResult.No, DialogResult.Cancel };
				}
				case MessageBoxButtons.YesNo:
				{
					return new DialogResult[] { DialogResult.Yes, DialogResult.No };
				}
				case MessageBoxButtons.RetryCancel:
				{
					return new DialogResult[] { DialogResult.Retry, DialogResult.Cancel };
				}
			}
			throw new ArgumentException("buttons");
		}

		private static int MessageBoxDefaultButtonToInt(MessageBoxDefaultButton defButton)
		{
			if (!Enum.IsDefined(typeof(MessageBoxDefaultButton), defButton))
			{
				throw new InvalidEnumArgumentException("defaultButton", (int)defButton, typeof(DialogResult));
			}
			MessageBoxDefaultButton messageBoxDefaultButton = defButton;
			if (messageBoxDefaultButton == MessageBoxDefaultButton.Button1)
			{
				return 0;
			}
			if (messageBoxDefaultButton == MessageBoxDefaultButton.Button2)
			{
				return 1;
			}
			if (messageBoxDefaultButton != MessageBoxDefaultButton.Button3)
			{
				throw new ArgumentException("defaultButton");
			}
			return 2;
		}

		private static Icon MessageBoxIconToIcon(MessageBoxIcon iconType)
		{
			Icon icon;
			if (!Enum.IsDefined(typeof(MessageBoxIcon), iconType))
			{
				throw new InvalidEnumArgumentException("icon", (int)iconType, typeof(DialogResult));
			}
			Icon icon1 = null;
			FlatXtraMessageBox.s_lock.EnterReadLock();
			try
			{
				if (FlatXtraMessageBox.s_icons.TryGetValue(iconType, out icon1))
				{
					icon = icon1;
					return icon;
				}
			}
			finally
			{
				FlatXtraMessageBox.s_lock.ExitReadLock();
			}
			if (icon1 != null)
			{
				return icon1;
			}
			FlatXtraMessageBox.s_lock.EnterWriteLock();
			try
			{
				if (!FlatXtraMessageBox.s_icons.TryGetValue(iconType, out icon1))
				{
					Bitmap jobStatusFailed32 = null;
					MessageBoxIcon messageBoxIcon = iconType;
					if (messageBoxIcon <= MessageBoxIcon.Hand)
					{
						if (messageBoxIcon == MessageBoxIcon.None)
						{
							FlatXtraMessageBox.s_icons.Add(iconType, null);
							icon = null;
							return icon;
						}
						else
						{
							if (messageBoxIcon != MessageBoxIcon.Hand)
							{
								throw new ArgumentException("icon");
							}
							jobStatusFailed32 = Resources.JobStatus_Failed_32;
						}
					}
					else if (messageBoxIcon == MessageBoxIcon.Question)
					{
						jobStatusFailed32 = Resources.BlueHelp32;
					}
					else if (messageBoxIcon == MessageBoxIcon.Exclamation)
					{
						jobStatusFailed32 = Resources.JobStatus_Warning_32;
					}
					else
					{
						if (messageBoxIcon != MessageBoxIcon.Asterisk)
						{
							throw new ArgumentException("icon");
						}
						jobStatusFailed32 = Resources.About32;
					}
					if (jobStatusFailed32 == null)
					{
						throw new ArgumentException("icon");
					}
					icon1 = Icon.FromHandle(jobStatusFailed32.GetHicon());
					FlatXtraMessageBox.s_icons.Add(iconType, icon1);
					icon = icon1;
				}
				else
				{
					icon = icon1;
				}
			}
			finally
			{
				FlatXtraMessageBox.s_lock.ExitWriteLock();
			}
			return icon;
		}

		public static DialogResult Show(string text)
		{
			return FlatXtraMessageBox.Show(null, text, "", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
		}

		public static DialogResult Show(IWin32Window owner, string text)
		{
			return FlatXtraMessageBox.Show(owner, text, "", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
		}

		public static DialogResult Show(string text, string caption)
		{
			return FlatXtraMessageBox.Show(null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
		}

		public static DialogResult Show(IWin32Window owner, string text, string caption)
		{
			return FlatXtraMessageBox.Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
		}

		public static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
		{
			return FlatXtraMessageBox.Show(null, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
		}

		public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons)
		{
			return FlatXtraMessageBox.Show(owner, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
		}

		public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		{
			return FlatXtraMessageBox.Show(null, text, caption, buttons, icon, MessageBoxDefaultButton.Button1);
		}

		public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		{
			return FlatXtraMessageBox.Show(owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button1);
		}

		public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
		{
			return FlatXtraMessageBox.Show(null, text, caption, buttons, icon, defaultButton);
		}

		public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
		{
			if (string.IsNullOrEmpty(caption))
			{
				caption = " ";
			}
			return XtraMessageBox.Show(owner, text, caption, FlatXtraMessageBox.MessageBoxButtonsToDialogResults(buttons), FlatXtraMessageBox.MessageBoxIconToIcon(icon), FlatXtraMessageBox.MessageBoxDefaultButtonToInt(defaultButton), icon);
		}
	}
}