using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Metalogix.Actions
{
    public class SubActionCollection : IEnumerable<Metalogix.Actions.Action>, IEnumerable
    {
        private readonly List<Metalogix.Actions.Action> m_subActions = new List<Metalogix.Actions.Action>();

        public Metalogix.Actions.Action First
        {
            get
            {
                Metalogix.Actions.Action action;
                Metalogix.Actions.Action item;
                lock (this.m_subActions)
                {
                    if (this.m_subActions.Count > 0)
                    {
                        item = this.m_subActions[0];
                    }
                    else
                    {
                        item = null;
                    }

                    action = item;
                }

                return action;
            }
        }

        public SubActionCollection()
        {
        }

        public void Add(Metalogix.Actions.Action action)
        {
            lock (this.m_subActions)
            {
                if (!this.m_subActions.Contains(action))
                {
                    this.m_subActions.Add(action);
                }
                else
                {
                    return;
                }
            }

            this.FireSubActionAdded(action);
        }

        private void FireSubActionAdded(Metalogix.Actions.Action subAction)
        {
            if (this.SubActionAdded != null)
            {
                this.SubActionAdded(subAction);
            }
        }

        private void FireSubActionRemoved(Metalogix.Actions.Action subAction)
        {
            if (this.SubActionRemoved != null)
            {
                this.SubActionRemoved(subAction);
            }
        }

        public IEnumerator<Metalogix.Actions.Action> GetEnumerator()
        {
            return this.m_subActions.GetEnumerator();
        }

        internal void Remove(Metalogix.Actions.Action action)
        {
            lock (this.m_subActions)
            {
                this.m_subActions.Remove(action);
            }

            this.FireSubActionRemoved(action);
        }

        public void SetStatus(ActionStatus status)
        {
            lock (this.m_subActions)
            {
                foreach (Metalogix.Actions.Action mSubAction in this.m_subActions)
                {
                    mSubAction.SetStatus(status);
                }
            }
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.m_subActions.GetEnumerator();
        }

        public event SubActionCollectionChanged SubActionAdded;

        public event SubActionCollectionChanged SubActionRemoved;
    }
}