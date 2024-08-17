using Metalogix.Azure;
using Metalogix.Azure.ConsoleProcessor;
using System;

namespace Metalogix.Azure.ConsoleProcessor.Commands
{
	public class DeleteQueueCommand : ICommand
	{
		public DeleteQueueCommand()
		{
		}

		public string Execute(string[] args)
		{
			DeleteQueueReponse deleteQueueReponse = (new AzureContainerManager()).DeleteQueue(args[1]);
			return this.Serialize<DeleteQueueReponse>(deleteQueueReponse);
		}
	}
}