using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Metalogix.SharePoint.Adapters
{
	public class ServiceFaultHandler : IErrorHandler
	{
		public ServiceFaultHandler()
		{
		}

		public bool HandleError(Exception error)
		{
			return true;
		}

		public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
		{
			ExceptionFault exceptionFault = new ExceptionFault(error);
			fault = Message.CreateMessage(version, "ExceptionThrown", exceptionFault);
		}
	}
}