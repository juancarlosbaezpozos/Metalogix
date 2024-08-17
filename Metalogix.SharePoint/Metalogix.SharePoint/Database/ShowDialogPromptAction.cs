using System;

namespace Metalogix.SharePoint.Database
{
	public delegate bool ShowDialogPromptAction(out object response, params object[] inputs);
}