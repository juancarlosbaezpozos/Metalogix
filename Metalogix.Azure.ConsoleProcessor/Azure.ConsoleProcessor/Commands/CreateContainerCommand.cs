using Metalogix.Azure;
using Metalogix.Azure.ConsoleProcessor;
using System;

namespace Metalogix.Azure.ConsoleProcessor.Commands
{
	internal class CreateContainerCommand : ICommand
	{
		public CreateContainerCommand()
		{
		}

		public string Execute(string[] args)
		{
			BlobAccessPermissions blobAccessPermission;
			AzureContainerManager azureContainerManager = new AzureContainerManager();
			if (!Enum.TryParse<BlobAccessPermissions>(args[3], out blobAccessPermission))
			{
				throw new ArgumentException(string.Format("Unable to parse value '{0}' for Execution Method '{1}'", args[3], "Create Container"), "blobAccessPermissions");
			}
			ContainerCreateResponse containerCreateResponse = azureContainerManager.CreateContainerIfNotExists(args[1], args[2], blobAccessPermission);
			return this.Serialize<ContainerCreateResponse>(containerCreateResponse);
		}
	}
}