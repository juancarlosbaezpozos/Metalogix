using Metalogix.Azure;
using Metalogix.Azure.ConsoleProcessor;
using System;

namespace Metalogix.Azure.ConsoleProcessor.Commands
{
	public class ListContainerCommand : ICommand
	{
		public ListContainerCommand()
		{
		}

		public string Execute(string[] args)
		{
			ListResponse listResponse = (new AzureBlobStorageManager()).ListContainer(args[1]);
			return this.Serialize<ListResponse>(listResponse);
		}
	}
}