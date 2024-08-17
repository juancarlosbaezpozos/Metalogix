using Metalogix.Azure;
using Metalogix.Azure.ConsoleProcessor;
using System;

namespace Metalogix.Azure.ConsoleProcessor.Commands
{
	internal class GetQueueMessageCommand : ICommand
	{
		public GetQueueMessageCommand()
		{
		}

		public string Execute(string[] args)
		{
			QueueMessageResponse queueMessage = (new AzureBlobStorageManager()).GetQueueMessage(args[1]);
			return this.Serialize<QueueMessageResponse>(queueMessage);
		}
	}
}