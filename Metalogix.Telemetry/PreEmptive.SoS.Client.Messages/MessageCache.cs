using PreEmptive.SoS.Client.MessageProxies;
using System;
using System.Collections;

namespace PreEmptive.SoS.Client.Messages
{
    [Serializable]
    public class MessageCache
    {
        private PreEmptive.SoS.Client.Messages.BusinessInformation business;

        private PreEmptive.SoS.Client.Messages.ApplicationInformation application;

        private DateTime timeSent;

        private string apiLanguage;

        private string apiVersion;

        private string instanceId;

        private PreEmptive.SoS.Client.MessageProxies.MessageCache proxy;

        private PreEmptive.SoS.Client.Messages.Message[] messages;

        public string ApiLanguage
        {
            get { return this.apiLanguage; }
            set { this.apiLanguage = value; }
        }

        public string ApiVersion
        {
            get { return this.apiVersion; }
            set { this.apiVersion = value; }
        }

        public PreEmptive.SoS.Client.Messages.ApplicationInformation Application
        {
            get { return this.application; }
            set { this.application = value; }
        }

        public Guid ApplicationGroupId
        {
            get { return this.proxy.ApplicationGroupId; }
            set { this.proxy.ApplicationGroupId = value; }
        }

        public PreEmptive.SoS.Client.Messages.BusinessInformation Business
        {
            get { return this.business; }
            set { this.business = value; }
        }

        public string InstanceId
        {
            get { return this.instanceId; }
            set { this.instanceId = value; }
        }

        public PreEmptive.SoS.Client.MessageProxies.Message[] Messages
        {
            get { return this.proxy.Messages; }
        }

        internal PreEmptive.SoS.Client.MessageProxies.MessageCache Proxy
        {
            get { return this.proxy; }
        }

        public DateTime TimeSent
        {
            get { return this.timeSent; }
            set { this.timeSent = value; }
        }

        public MessageCache()
        {
            this.proxy = new PreEmptive.SoS.Client.MessageProxies.MessageCache()
            {
                Id = Guid.NewGuid()
            };
        }

        public void AddToApplicationGroup(Guid guid_0)
        {
            if (guid_0 == Guid.Empty)
            {
                throw new ArgumentException("Argument cannot be empty", "id");
            }

            this.proxy.ApplicationGroupId = guid_0;
        }

        public PreEmptive.SoS.Client.Messages.MessageCache Clone()
        {
            PreEmptive.SoS.Client.Messages.MessageCache messageCache = new PreEmptive.SoS.Client.Messages.MessageCache()
            {
                apiLanguage = this.apiLanguage,
                apiVersion = this.apiVersion,
                application = this.application,
                business = this.business,
                instanceId = this.instanceId,
                timeSent = this.timeSent
            };
            messageCache.SetMessagesArray(this.messages);
            return messageCache;
        }

        public void FillInProxy()
        {
            if (this.messages != null)
            {
                PreEmptive.SoS.Client.Messages.Message[] messageArray = this.messages;
                for (int i = 0; i < (int)messageArray.Length; i++)
                {
                    PreEmptive.SoS.Client.Messages.Message applicationGroupId = messageArray[i];
                    if (applicationGroupId.SessionId == Guid.Empty)
                    {
                        applicationGroupId.SessionId = this.ApplicationGroupId;
                    }

                    applicationGroupId.FillInProxy();
                }

                int num = 0;
                this.proxy.Messages = new PreEmptive.SoS.Client.MessageProxies.Message[(int)this.messages.Length];
                PreEmptive.SoS.Client.Messages.Message[] messageArray1 = this.messages;
                for (int j = 0; j < (int)messageArray1.Length; j++)
                {
                    PreEmptive.SoS.Client.Messages.Message message = messageArray1[j];
                    int num1 = num;
                    num = num1 + 1;
                    this.proxy.Messages[num1] = message.Proxy;
                }
            }

            this.proxy.ApiLanguage = this.apiLanguage;
            this.proxy.ApiVersion = this.apiVersion;
            this.proxy.SchemaVersion = SchemaVersionValue.Item020000;
            this.proxy.TimeSentUtc = this.timeSent;
            this.proxy.InstanceId = this.instanceId;
            if (this.business != null)
            {
                this.business.FillInProxy(this.proxy);
            }

            if (this.application != null)
            {
                this.application.FillInProxy(this.proxy);
            }
        }

        internal void SetMessages(ArrayList flatList)
        {
            this.messages =
                (PreEmptive.SoS.Client.Messages.Message[])flatList.ToArray(
                    typeof(PreEmptive.SoS.Client.Messages.Message));
        }

        internal void SetMessagesArray(PreEmptive.SoS.Client.Messages.Message[] flatList)
        {
            this.messages = flatList;
        }

        public PreEmptive.SoS.Client.Messages.MessageCache Split()
        {
            this.proxy.Id = Guid.NewGuid();
            PreEmptive.SoS.Client.Messages.MessageCache messageCache = this.Clone();
            if ((int)this.messages.Length == 1)
            {
                return null;
            }

            PreEmptive.SoS.Client.Messages.Message[] messageArray =
                new PreEmptive.SoS.Client.Messages.Message[(int)this.messages.Length / 2];
            PreEmptive.SoS.Client.Messages.Message[] messageArray1 =
                new PreEmptive.SoS.Client.Messages.Message
                    [(int)this.messages.Length / 2 + (int)this.messages.Length % 2];
            Array.Copy(this.messages, messageArray, (int)messageArray.Length);
            Array.Copy(this.messages, (int)messageArray.Length - 1, messageArray1, 0, (int)messageArray1.Length);
            this.SetMessagesArray(messageArray);
            messageCache.SetMessagesArray(messageArray1);
            return messageCache;
        }
    }
}