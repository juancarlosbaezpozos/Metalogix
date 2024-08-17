using System;

namespace Metalogix.SharePoint.Database
{
	public class DatabaseRestorationDialogs
	{
		public ShowDialogPromptAction ConnectToMdfDatabaseDialog;

		public ShowDialogPromptAction GetDatabaseBackupVersionsDialog;

		public ShowDialogPromptAction InitializeFileLocationsDialog;

		public ShowDialogPromptAction RestoreDatabaseFromBakDialog;

		public ShowDialogPromptAction RestoreDatabaseFromMdfDialog;

		public ShowDialogPromptAction RunRestoreDialog;

		public DatabaseRestorationDialogs(IShowDialogPromptHandler connect, IShowDialogPromptHandler get, IShowDialogPromptHandler init, IShowDialogPromptHandler restoreFromBak, IShowDialogPromptHandler restoreFromMdf, IShowDialogPromptHandler run)
		{
			this.ConnectToMdfDatabaseDialog = this.GetAction(connect);
			this.GetDatabaseBackupVersionsDialog = this.GetAction(get);
			this.InitializeFileLocationsDialog = this.GetAction(init);
			this.RestoreDatabaseFromBakDialog = this.GetAction(restoreFromBak);
			this.RestoreDatabaseFromMdfDialog = this.GetAction(restoreFromMdf);
			this.RunRestoreDialog = this.GetAction(run);
		}

		private ShowDialogPromptAction GetAction(IShowDialogPromptHandler handler)
		{
			IShowDialogPromptHandler showDialogPromptHandler = handler;
			return new ShowDialogPromptAction(showDialogPromptHandler.Handle);
		}
	}
}