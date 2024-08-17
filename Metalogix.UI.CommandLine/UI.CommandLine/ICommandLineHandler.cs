using System;

namespace Metalogix.UI.CommandLine
{
	public interface ICommandLineHandler
	{
		string HelpText
		{
			get;
		}

		bool CanHandle(CommandLineParamsCollection parameters);

		void Handle(CommandLineParamsCollection pars);
	}
}