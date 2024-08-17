using Metalogix;
using Metalogix.Deployment;
using Metalogix.UI.WinForms;
using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Deployment
{
	public static class UpdaterService
	{
		private static AutomaticUpdater _automaticUpdater;

		private static bool _checking;

		private static AutomaticUpdater Updater
		{
			get
			{
				if (UpdaterService._automaticUpdater == null)
				{
					UpdaterService._automaticUpdater = new AutomaticUpdater(new AutomaticUpdaterSettings(new UICalls()));
					UpdaterService._automaticUpdater.CheckForUpdateCompleted += new EventHandler<CheckForUpdateCompletedEventArgs>(UpdaterService.au_CheckForUpdateCompleted);
				}
				return UpdaterService._automaticUpdater;
			}
		}

		private static void au_CheckForUpdateCompleted(object sender, CheckForUpdateCompletedEventArgs e)
		{
			try
			{
				try
				{
					if (e.UpdateNeeded)
					{
						bool flag = false;
						if (UIApplication.INSTANCE.MainForm.InvokeRequired)
						{
							UIApplication.INSTANCE.MainForm.Invoke(new MethodInvoker(() => {
								if (UpdaterService.Updater.StartUpdate())
								{
									flag = true;
									Application.Exit();
								}
							}));
						}
						else if (UpdaterService.Updater.StartUpdate())
						{
							flag = true;
							Application.Exit();
						}
						if (flag)
						{
							return;
						}
					}
					else if (e.Tag is bool && (bool)e.Tag)
					{
						FlatXtraMessageBox.Show("You already have the latest version installed.", "Check for Updates", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
					}
				}
				catch (Exception exception)
				{
					Logger.Error.Write("Failed to check or download updates.", exception);
				}
			}
			finally
			{
				UpdaterService._checking = false;
			}
		}

		internal static void CheckForUpdate(bool notifyIfNoUpdate)
		{
			UpdaterService.CheckForUpdate(notifyIfNoUpdate, true);
		}

		internal static void CheckForUpdate(bool notifyIfNoUpdate, bool ignoreTurnOffSettings)
		{
			if (!UpdaterService._checking)
			{
				UpdaterService._checking = true;
				UpdaterService.Updater.CheckForUpdateAsync(notifyIfNoUpdate, ignoreTurnOffSettings);
			}
		}
	}
}