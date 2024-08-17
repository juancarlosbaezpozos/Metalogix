using System;

namespace Metalogix.SharePoint.Actions.Migration.Pipeline
{
	public interface IEventPrinter
	{
		string PrintEvent(IEvent e);
	}
}