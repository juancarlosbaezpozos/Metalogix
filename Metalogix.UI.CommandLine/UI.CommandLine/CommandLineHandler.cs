using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Metalogix.UI.CommandLine
{
	public static class CommandLineHandler
	{
		private static CommandLineHandlerCollection _handlers;

		static CommandLineHandler()
		{
		}

		public static void Handle()
		{
			try
			{
				CommandLineParamsCollection commandLineParamsCollection = (new CommandLineParser()).Parse(Environment.GetCommandLineArgs());
				CommandLineHandler._handlers = new CommandLineHandlerCollection();
				CommandLineHandler._handlers.Load();
				ICommandLineHandler handler = CommandLineHandler._handlers.GetHandler(commandLineParamsCollection);
				if (handler == null)
				{
					throw new Exception("Invalid command line arguments were supplied.");
				}
				handler.Handle(commandLineParamsCollection);
			}
			catch (Exception exception)
			{
				Console.WriteLine("Error: {0}\n", exception.Message);
				CommandLineHandler.PrintUsage();
			}
		}

		public static bool IsCommandLineControl()
		{
			return (int)Environment.GetCommandLineArgs().Length > 1;
		}

		private static void PrintUsage()
		{
			Console.WriteLine("Usage: {0} <command> <parameters>", Path.GetFileName(Assembly.GetEntryAssembly().Location));
			Console.WriteLine("Commands:");
			if (CommandLineHandler._handlers == null || CommandLineHandler._handlers.Count <= 0)
			{
				Console.WriteLine("  No commands supported.");
			}
			else
			{
				foreach (ICommandLineHandler _handler in CommandLineHandler._handlers)
				{
					Console.WriteLine(_handler.HelpText);
				}
			}
		}
	}
}