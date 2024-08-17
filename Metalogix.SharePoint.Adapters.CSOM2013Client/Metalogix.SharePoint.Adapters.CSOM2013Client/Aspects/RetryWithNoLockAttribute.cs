using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Adapters.CSOM2013Client;
using PostSharp.Aspects;
using PostSharp.Aspects.Internals;
using PostSharp.Extensibility;
using System;
using System.Reflection;
using System.ServiceModel;

namespace Metalogix.SharePoint.Adapters.CSOM2013Client.Aspects
{
    [AttributeUsage(AttributeTargets.Method)]
    //[HasInheritedAttribute(new long[] {  })]
    [MulticastAttributeUsage(MulticastTargets.Method)]
    [Serializable]
    public class RetryWithNoLockAttribute : MethodInterceptionAspect
    {
        public RetryWithNoLockAttribute()
        {
        }

        //[HasInheritedAttribute(new long[] { 3403990581028806597L })]
        [MethodInterceptionAdviceOptimization(MethodInterceptionAdviceOptimizations.IgnoreGetMethod)]
        public override void OnInvoke(MethodInterceptionArgs args)
        {
            CSOMClientAdapter instance = args.Instance as CSOMClientAdapter;
            if (instance == null)
            {
                throw new NotCSOMClientAdapterException();
            }

            bool flag = false;
            try
            {
                args.Proceed();
            }
            catch (CommunicationException communicationException1)
            {
                CommunicationException communicationException = communicationException1;
                Utils.LogExceptionDetails(communicationException, MethodBase.GetCurrentMethod().Name,
                    MethodBase.GetCurrentMethod().DeclaringType.Name, null);
                flag = true;
            }

            if (flag)
            {
                instance.ReleaseService();
                args.Proceed();
            }
        }
    }
}