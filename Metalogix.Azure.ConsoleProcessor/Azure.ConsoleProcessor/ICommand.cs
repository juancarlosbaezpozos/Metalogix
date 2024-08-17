using System;

namespace Metalogix.Azure.ConsoleProcessor
{
	public interface ICommand
	{
		string Execute(string[] args);
	}
}