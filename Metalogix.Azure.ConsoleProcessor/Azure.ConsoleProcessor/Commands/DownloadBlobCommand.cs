using Metalogix.Azure;
using Metalogix.Azure.ConsoleProcessor;
using System;

namespace Metalogix.Azure.ConsoleProcessor.Commands
{
	internal class DownloadBlobCommand : ICommand
	{
		public DownloadBlobCommand()
		{
		}

		public string Execute(string[] args)
		{
			DownloadResponse downloadResponse;
			AzureBlobStorageManager azureBlobStorageManager = new AzureBlobStorageManager();
			if ((int)args.Length <= 5)
			{
				downloadResponse = azureBlobStorageManager.DownloadBlob(args[1], args[2], args[3], null);
			}
			else
			{
				azureBlobStorageManager.IsEncryptionUsed = bool.Parse(args[4]);
				downloadResponse = azureBlobStorageManager.DownloadBlob(args[1], args[2], args[3], Convert.FromBase64String(args[5]));
			}
			return this.Serialize<DownloadResponse>(downloadResponse);
		}
	}
}