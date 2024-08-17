using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Actions
{
    public class ActionDataCounter
    {
        private readonly object _dataUsedLock = new object();

        private readonly object _itemsUsedLock = new object();

        public long TotalDataUsed { get; private set; }

        public long TotalItemsUsed { get; private set; }

        public ActionDataCounter()
        {
            this.TotalItemsUsed = 0L;
            this.TotalDataUsed = 0L;
        }

        public void IncrementDataUsed(long amount)
        {
            if (amount < 0L)
            {
                return;
            }

            lock (this._dataUsedLock)
            {
                ActionDataCounter totalDataUsed = this;
                totalDataUsed.TotalDataUsed = totalDataUsed.TotalDataUsed + amount;
            }
        }

        public void IncrementItemsUsed(long amount)
        {
            if (amount < 0L)
            {
                return;
            }

            lock (this._itemsUsedLock)
            {
                ActionDataCounter totalItemsUsed = this;
                totalItemsUsed.TotalItemsUsed = totalItemsUsed.TotalItemsUsed + amount;
            }
        }
    }
}