using Metalogix.Azure;
using Metalogix.Azure.ConsoleProcessor.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Metalogix.Azure.ConsoleProcessor
{
	internal class AzureConsoleProcessor
	{
		private readonly static int MinimumArgLength;

		private readonly static Dictionary<ExecutionMethod, ICommand> Commands;

		static AzureConsoleProcessor()
		{
			AzureConsoleProcessor.MinimumArgLength = 1;
			Dictionary<ExecutionMethod, ICommand> executionMethods = new Dictionary<ExecutionMethod, ICommand>()
			{
				{ ExecutionMethod.CreateContainer, new CreateContainerCommand() },
				{ ExecutionMethod.CreateQueue, new CreateQueueCommand() },
				{ ExecutionMethod.DeleteContainer, new DeleteContainerCommand() },
				{ ExecutionMethod.DeleteQueue, new DeleteQueueCommand() },
				{ ExecutionMethod.DownloadBlob, new DownloadBlobCommand() },
				{ ExecutionMethod.GetQueueMessage, new GetQueueMessageCommand() },
				{ ExecutionMethod.GetQueueMessages, new GetQueueMessagesCommand() },
				{ ExecutionMethod.ListContainer, new ListContainerCommand() },
				{ ExecutionMethod.UploadBlob, new UploadBlobCommand() }
			};
			AzureConsoleProcessor.Commands = executionMethods;
		}

		public AzureConsoleProcessor()
		{
		}

		private static int Main(string[] args)
		{
			ExecutionMethod executionMethod;
			MethodResultCode methodResultCode = MethodResultCode.Success;
			if ((int)args.Length < AzureConsoleProcessor.MinimumArgLength)
			{
				Console.WriteLine("Not enough parameters");
				return -1;
			}
			string str = args[0];
			if (!Enum.TryParse<ExecutionMethod>(str, true, out executionMethod))
			{
				Console.WriteLine(string.Concat("Execution method '", str, "' not supported"));
				return -1;
			}
			if (!AzureConsoleProcessor.Commands.ContainsKey(executionMethod))
			{
				throw new MissingMethodException(string.Format("AzureControlProcessor - MethodName '{0}' not implemented.", executionMethod));
			}
			try
			{
				string str1 = AzureConsoleProcessor.Commands[executionMethod].Execute(args);
				Console.Out.WriteLine(str1);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				methodResultCode = MethodResultCode.Error;
				Response response = new Response()
				{
					Success = false,
					Details = string.Format("{0}{1}{2}", exception.Message, Environment.NewLine, exception.StackTrace)
				};
				StringBuilder stringBuilder = Serializer.Serialize<Response>(response);
				Console.Out.WriteLine(stringBuilder.ToString());
			}
			Console.Out.Flush();
			return (int)methodResultCode;
		}
	}
}