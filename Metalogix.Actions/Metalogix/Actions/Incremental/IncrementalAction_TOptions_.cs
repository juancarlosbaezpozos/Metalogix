using Metalogix.Actions;
using Metalogix.Actions.Incremental.Database;
using System;

namespace Metalogix.Actions.Incremental
{
    public abstract class IncrementalAction<TOptions> : Metalogix.Actions.Action<TOptions>, IIncrementalAction
        where TOptions : Metalogix.Actions.ActionOptions
    {
        private readonly static object Lock;

        private volatile MappingConnection _incrementalMappings;

        public MappingConnection IncrementalMappings
        {
            get
            {
                if (this._incrementalMappings == null)
                {
                    lock (IncrementalAction<TOptions>.Lock)
                    {
                        if (this._incrementalMappings == null)
                        {
                            this._incrementalMappings =
                                MappingDatabaseProvider.Instance.GetIncrementalDatabase().Connect();
                        }
                    }
                }

                return this._incrementalMappings;
            }
        }

        static IncrementalAction()
        {
            IncrementalAction<TOptions>.Lock = new object();
        }

        protected IncrementalAction()
        {
        }
    }
}