using Metalogix.Azure;
using Metalogix.Azure.ConsoleProcessor;
using System;

namespace Metalogix.Azure.ConsoleProcessor.Commands
{
	internal class DeleteContainerCommand : ICommand
	{
		public DeleteContainerCommand()
		{
		}

		public string Execute(string[] args)
		{
			AzureContainerManager azureContainerManager = new AzureContainerManager();
			DeleteContainerReponse deleteContainerReponse = azureContainerManager.DeleteContainer(args[1], args[2]);
			return this.Serialize<DeleteContainerReponse>(deleteContainerReponse);
		}
	}
}