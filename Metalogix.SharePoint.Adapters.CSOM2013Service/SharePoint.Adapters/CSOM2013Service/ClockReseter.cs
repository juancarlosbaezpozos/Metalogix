using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Metalogix.SharePoint.Adapters.CSOM2013Service
{
	public class ClockReseter : IDispatchMessageInspector
	{
		public ClockReseter()
		{
		}

		public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
		{
			TickingClock.ResetTimer();
			return null;
		}

		public void BeforeSendReply(ref Message reply, object correlationState)
		{
		}
	}
}