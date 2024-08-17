using System;

namespace Metalogix.SharePoint.Database
{
	public interface IShowDialogPromptHandler
	{
		bool Handle(out object response, params object[] inputs);
	}
}