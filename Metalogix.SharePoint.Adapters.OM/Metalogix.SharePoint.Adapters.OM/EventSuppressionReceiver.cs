using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Metalogix.SharePoint.Adapters.OM
{
    public class EventSuppressionReceiver : SPItemEventReceiver
    {
        private int m_iCount;

        private static object s_oLockSuppressorDictionary;

        private static Dictionary<int, EventSuppressionReceiver> s_dictEventSuppressors;

        static EventSuppressionReceiver()
        {
            EventSuppressionReceiver.s_oLockSuppressorDictionary = new object();
            EventSuppressionReceiver.s_dictEventSuppressors = new Dictionary<int, EventSuppressionReceiver>();
        }

        private EventSuppressionReceiver()
        {
        }

        public static void AllowEvents()
        {
            EventSuppressionReceiver.GetSuppressor().AllowEventsInternal();
        }

        private void AllowEventsInternal()
        {
            this.m_iCount--;
            if (this.m_iCount == 0)
            {
                base.EnableEventFiring();
                if (this.EventSuppressionCompleted != null)
                {
                    this.EventSuppressionCompleted(this, null);
                }
            }
        }

        private static EventSuppressionReceiver GetSuppressor()
        {
            EventSuppressionReceiver eventSuppressionReceiver = null;
            int managedThreadId = Thread.CurrentThread.ManagedThreadId;
            lock (EventSuppressionReceiver.s_oLockSuppressorDictionary)
            {
                if (!EventSuppressionReceiver.s_dictEventSuppressors.ContainsKey(managedThreadId))
                {
                    eventSuppressionReceiver = new EventSuppressionReceiver();
                    eventSuppressionReceiver.EventSuppressionCompleted +=
                        new EventHandler(EventSuppressionReceiver.ManagedEventSuppressionCompleted);
                    EventSuppressionReceiver.s_dictEventSuppressors.Add(managedThreadId, eventSuppressionReceiver);
                }
                else
                {
                    eventSuppressionReceiver = EventSuppressionReceiver.s_dictEventSuppressors[managedThreadId];
                }
            }

            return eventSuppressionReceiver;
        }

        private static void ManagedEventSuppressionCompleted(object sender, EventArgs e)
        {
            EventSuppressionReceiver eventSuppressionReceiver = sender as EventSuppressionReceiver;
            if (eventSuppressionReceiver != null)
            {
                lock (EventSuppressionReceiver.s_oLockSuppressorDictionary)
                {
                    eventSuppressionReceiver.EventSuppressionCompleted -=
                        new EventHandler(EventSuppressionReceiver.ManagedEventSuppressionCompleted);
                    int managedThreadId = Thread.CurrentThread.ManagedThreadId;
                    if (EventSuppressionReceiver.s_dictEventSuppressors.ContainsKey(managedThreadId) &&
                        (object)EventSuppressionReceiver.s_dictEventSuppressors[managedThreadId] ==
                        (object)eventSuppressionReceiver)
                    {
                        EventSuppressionReceiver.s_dictEventSuppressors.Remove(managedThreadId);
                    }
                }
            }
        }

        public static void SuppressEvents()
        {
            EventSuppressionReceiver.GetSuppressor().SuppressEventsInternal();
        }

        private void SuppressEventsInternal()
        {
            if (this.m_iCount == 0)
            {
                base.DisableEventFiring();
            }

            this.m_iCount++;
        }

        private event EventHandler EventSuppressionCompleted;
    }
}