using System;

namespace Metalogix.Actions.Remoting
{
    public static class LogHelper
    {
        public static void LogMessage(Agent agent, string message)
        {
            if (agent != null)
            {
                agent.Parent.AddLog(agent.AgentID, message);
            }
        }

        public static void LogMessage(Agent agent, Exception exception_0, string message)
        {
            message = string.Format("{0}. Error: '{1}'", message, exception_0);
            LogHelper.LogMessage(agent, message);
        }
    }
}