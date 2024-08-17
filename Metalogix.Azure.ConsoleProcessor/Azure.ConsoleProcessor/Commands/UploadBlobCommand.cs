using Metalogix.Azure;
using Metalogix.Azure.ConsoleProcessor;
using System;

namespace Metalogix.Azure.ConsoleProcessor.Commands
{
	internal class UploadBlobCommand : ICommand
	{
		public UploadBlobCommand()
		{
		}

		public string Execute(string[] args)
		{
			UploadResponse uploadResponse;
			AzureBlobStorageManager azureBlobStorageManager = new AzureBlobStorageManager();
			if ((int)args.Length <= 4)
			{
				uploadResponse = azureBlobStorageManager.UploadBlob(args[1], args[2], args[3], null);
			}
			else
			{
				azureBlobStorageManager.IsEncryptionUsed = bool.Parse(args[4]);
				uploadResponse = azureBlobStorageManager.UploadBlob(args[1], args[2], args[3], Convert.FromBase64String(args[5]));
			}
			return this.Serialize<UploadResponse>(uploadResponse);
		}
	}
}