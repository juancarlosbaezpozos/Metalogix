using Metalogix.Azure;
using Metalogix.Azure.ConsoleProcessor;
using System;

namespace Metalogix.Azure.ConsoleProcessor.Commands
{
	internal class CreateQueueCommand : ICommand
	{
		public CreateQueueCommand()
		{
		}

		public string Execute(string[] args)
		{
			QueueAccessPermissions queueAccessPermission;
			AzureContainerManager azureContainerManager = new AzureContainerManager();
			if (!Enum.TryParse<QueueAccessPermissions>(args[3], out queueAccessPermission))
			{
				throw new ArgumentException(string.Format("Unable to parse value '{0}' for Execution Method '{1}'", args[3], "Create Queue"), "queueAccessPermissions");
			}
			CreateQueueResponse createQueueResponse = azureContainerManager.CreateQueueIfNotExists(args[1], args[2], queueAccessPermission);
			return this.Serialize<CreateQueueResponse>(createQueueResponse);
		}
	}
}