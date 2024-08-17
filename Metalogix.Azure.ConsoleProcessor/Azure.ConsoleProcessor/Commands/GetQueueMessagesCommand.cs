using Metalogix.Azure;
using Metalogix.Azure.ConsoleProcessor;
using System;

namespace Metalogix.Azure.ConsoleProcessor.Commands
{
	internal class GetQueueMessagesCommand : ICommand
	{
		public GetQueueMessagesCommand()
		{
		}

		public string Execute(string[] args)
		{
			int num;
			if (!int.TryParse(args[2], out num))
			{
				throw new ArgumentException(string.Format("Unable to parse value '{0}' for Execution Method '{1}'", args[2], "Get Queue Messages"), "numberOfMessagesToGet");
			}
			QueueMessagesResponse queueMessages = (new AzureBlobStorageManager()).GetQueueMessages(args[1], num);
			return this.Serialize<QueueMessagesResponse>(queueMessages);
		}
	}
}