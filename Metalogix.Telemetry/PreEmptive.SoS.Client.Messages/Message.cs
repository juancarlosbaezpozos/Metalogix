using PreEmptive.SoS.Client.MessageProxies;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PreEmptive.SoS.Client.Messages
{
    [Serializable]
    public abstract class Message
    {
        private PreEmptive.SoS.Client.MessageProxies.Message proxy;

        private PreEmptive.SoS.Client.Messages.EventInformation eventInfo;

        private PreEmptive.SoS.Client.Messages.BinaryInformation binInfo;

        private ArrayList extendedKeys;

        private Guid sessionId;

        public PreEmptive.SoS.Client.Messages.BinaryInformation Binary
        {
            get { return this.binInfo; }
            set { this.binInfo = value; }
        }

        public PreEmptive.SoS.Client.Messages.EventInformation Event
        {
            get { return this.eventInfo; }
            set { this.eventInfo = value; }
        }

        public int ExtendedKeyCount
        {
            get
            {
                if (this.extendedKeys == null)
                {
                    return 0;
                }

                return this.extendedKeys.Count;
            }
        }

        public Guid Id
        {
            get { return this.proxy.Id; }
        }

        internal PreEmptive.SoS.Client.MessageProxies.Message Proxy
        {
            get { return this.proxy; }
        }

        public Guid SessionId
        {
            get { return this.sessionId; }
            set { this.sessionId = value; }
        }

        public DateTime TimeStamp
        {
            get { return this.proxy.TimeStampUtc; }
        }

        protected Message()
        {
            this.proxy = this.CreateProxy();
            this.proxy.Id = Guid.NewGuid();
            this.proxy.TimeStampUtc = DateTime.UtcNow;
        }

        public void AddExtendedKey(PreEmptive.SoS.Client.Messages.ExtendedKey extendedKey_0)
        {
            if (extendedKey_0 == null)
            {
                return;
            }

            if (this.extendedKeys == null)
            {
                this.extendedKeys = new ArrayList();
            }

            this.extendedKeys.Add(extendedKey_0);
        }

        public void AddExtendedKey(string string_0, string value)
        {
            this.AddExtendedKey(new PreEmptive.SoS.Client.Messages.ExtendedKey(string_0, value));
        }

        public void AddExtendedKeyObjectAsString(string string_0, object value)
        {
            this.AddExtendedKey(new PreEmptive.SoS.Client.Messages.ExtendedKey(string_0,
                PreEmptive.SoS.Client.Messages.ExtendedKey.GetValueAsString(value)));
        }

        public void AddExtendedKeys(IDictionary dict)
        {
            if (dict != null)
            {
                foreach (string key in dict.Keys)
                {
                    string str = Convert.ToString(dict[key]);
                    this.AddExtendedKey(new PreEmptive.SoS.Client.Messages.ExtendedKey(key, str));
                }
            }
        }

        public void AddExtendedKeys(IDictionary<string, string> dict)
        {
            if (dict != null)
            {
                foreach (string key in dict.Keys)
                {
                    this.AddExtendedKey(new PreEmptive.SoS.Client.Messages.ExtendedKey(key, dict[key]));
                }
            }
        }

        protected abstract PreEmptive.SoS.Client.MessageProxies.Message CreateProxy();

        internal virtual void FillInProxy()
        {
            this.SyncEventInformation();
            this.SyncExtendedKeys();
            if (this.binInfo != null)
            {
                this.binInfo.FillInProxy(this.proxy);
            }

            this.proxy.SessionId = this.sessionId;
        }

        public PreEmptive.SoS.Client.Messages.ExtendedKey GetExtendedKeyAt(int index)
        {
            if (this.extendedKeys == null || index < 0 || index >= this.extendedKeys.Count)
            {
                return null;
            }

            return (PreEmptive.SoS.Client.Messages.ExtendedKey)this.extendedKeys[index];
        }

        public void RemoveExtendedKeyAt(int index)
        {
            if (this.extendedKeys == null)
            {
                return;
            }

            if (index >= 0 && index < this.extendedKeys.Count)
            {
                this.extendedKeys.RemoveAt(index);
            }
        }

        public virtual void ResetTimeStampUtc()
        {
            this.proxy.TimeStampUtc = DateTime.UtcNow;
        }

        private void SyncEventInformation()
        {
            if (this.eventInfo == null)
            {
                this.proxy.Event = null;
                return;
            }

            this.eventInfo.FillInProxy(this.proxy);
        }

        private void SyncExtendedKeys()
        {
            if (this.extendedKeys == null || this.extendedKeys.Count <= 0)
            {
                this.proxy.ExtendedInformation = null;
                return;
            }

            this.proxy.ExtendedInformation =
                new PreEmptive.SoS.Client.MessageProxies.ExtendedKey[this.extendedKeys.Count];
            for (int i = 0; i < this.extendedKeys.Count; i++)
            {
                this.proxy.ExtendedInformation[i] = new PreEmptive.SoS.Client.MessageProxies.ExtendedKey();
                this.proxy.ExtendedInformation[i].Key =
                    ((PreEmptive.SoS.Client.Messages.ExtendedKey)this.extendedKeys[i]).Key;
                this.proxy.ExtendedInformation[i].Value =
                    ((PreEmptive.SoS.Client.Messages.ExtendedKey)this.extendedKeys[i]).Value;
                this.proxy.ExtendedInformation[i].DataType =
                    ((PreEmptive.SoS.Client.Messages.ExtendedKey)this.extendedKeys[i]).DataType;
            }
        }
    }
}