using Metalogix.Connectivity.Proxy;
using Metalogix.Deployment;
using Metalogix.Licensing;
using Metalogix.Licensing.LicenseServer.Service;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Proxy;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Deployment
{
	public class UICalls : IUICalls
	{
		public UICalls()
		{
		}

		public void ExitApplication()
		{
			Application.Exit();
		}

		public MLLicense GetLicense()
		{
			return UIApplication.INSTANCE.GetLicense();
		}

		public int GetProductId()
		{
			return UIApplication.INSTANCE.ProductId;
		}

		public string GetProductName()
		{
			return UIApplication.INSTANCE.ProductName;
		}

		public bool InvokeAnotherProcessesRunningDialog(AnotherProcessInfo[] anotherProcesses)
		{
			if (DialogResult.OK != (new AnotherProcessesDlg()
			{
				AnotherProcesses = anotherProcesses
			}).ShowDialog())
			{
				return false;
			}
			return true;
		}

		public string InvokeFileDownloadDialog(string fileURL, ref MLProxy proxy)
		{
			string empty = string.Empty;
			FileDownloadDlg fileDownloadDlg = new FileDownloadDlg(proxy)
			{
				SourceURL = fileURL
			};
			if (fileDownloadDlg.ShowDialog() == DialogResult.OK)
			{
				empty = fileDownloadDlg.TargetFile;
			}
			proxy = fileDownloadDlg.Proxy;
			fileDownloadDlg.Dispose();
			return empty;
		}

		public void InvokeInstallDialog(string filePath)
		{
			Process process = new Process();
			process.StartInfo.FileName = filePath;
			process.Start();
			Application.Exit();
		}

		public AutomaticUpdater.NewReleaseUserResponseType InvokeNotifyUserAboutNewReleaseDialog(LatestProductReleaseInfo releaseInfo, AutomaticUpdater updater)
		{
			AutomaticUpdater.NewReleaseUserResponseType newReleaseUserResponseType = AutomaticUpdater.NewReleaseUserResponseType.Cancel;
			FormGetLatestProductRelease formGetLatestProductRelease = new FormGetLatestProductRelease(releaseInfo, updater);
			DialogResult dialogResult = formGetLatestProductRelease.ShowDialog();
			formGetLatestProductRelease.Dispose();
			switch (dialogResult)
			{
				case DialogResult.OK:
				{
					newReleaseUserResponseType = AutomaticUpdater.NewReleaseUserResponseType.Update;
					break;
				}
				case DialogResult.Cancel:
				{
					newReleaseUserResponseType = AutomaticUpdater.NewReleaseUserResponseType.Cancel;
					break;
				}
				case DialogResult.Abort:
				case DialogResult.Retry:
				case DialogResult.Yes:
				{
					newReleaseUserResponseType = AutomaticUpdater.NewReleaseUserResponseType.Cancel;
					break;
				}
				case DialogResult.Ignore:
				{
					newReleaseUserResponseType = AutomaticUpdater.NewReleaseUserResponseType.SkipVersion;
					break;
				}
				case DialogResult.No:
				{
					if (updater.Settings.AutoUpdateSettings != AutomaticUpdaterSettings.AutoUpdateSettingType.TurnOffCompletely)
					{
						newReleaseUserResponseType = AutomaticUpdater.NewReleaseUserResponseType.TurnOffCompletely;
						break;
					}
					else
					{
						newReleaseUserResponseType = AutomaticUpdater.NewReleaseUserResponseType.AutoUpdate;
						break;
					}
				}
				default:
				{
					goto case DialogResult.Yes;
				}
			}
			return newReleaseUserResponseType;
		}

		public MLProxy InvokeProxyDialog(MLProxy proxy)
		{
			MLProxy mLProxy = null;
			ProxyDlg proxyDlg = new ProxyDlg();
			if (proxy != null)
			{
				proxyDlg.Proxy = proxy;
			}
			proxyDlg.ShowEnabled = true;
			if (proxyDlg.ShowDialog() == DialogResult.OK)
			{
				mLProxy = proxyDlg.Proxy;
			}
			return mLProxy;
		}

		public void ShowError(string message)
		{
			this.ShowMessage(message, "Error", false);
		}

		public bool ShowMessage(string message, string title, bool showCancelButton)
		{
			DialogResult dialogResult;
			bool flag = false;
			dialogResult = (!showCancelButton ? FlatXtraMessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk) : FlatXtraMessageBox.Show(message, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk));
			if (dialogResult == DialogResult.OK)
			{
				flag = true;
			}
			return flag;
		}
	}
}