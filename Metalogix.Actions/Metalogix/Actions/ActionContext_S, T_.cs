using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Actions
{
    public class ActionContext<S, T> : IDisposable
        where S : IXMLAbleList
        where T : IXMLAbleList
    {
        public S Sources { get; set; }

        public T Targets { get; set; }

        public ActionContext()
        {
        }

        public ActionContext(S sources, T targets)
        {
            this.Sources = sources;
            this.Targets = targets;
        }

        public void Dispose()
        {
        }
    }
}