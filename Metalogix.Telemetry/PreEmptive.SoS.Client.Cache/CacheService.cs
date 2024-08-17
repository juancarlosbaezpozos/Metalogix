using PreEmptive.SoS.Client.MessageProxies;
using PreEmptive.SoS.Client.Messages;
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.Cache
{
    public sealed class CacheService : IDisposable
    {
        private const double MAX_DELTA = 300000;

        private Hashtable cache;

        private ReaderWriterLock cacheLock;

        private Thread serviceThread;

        private bool serviceAlive;

        private bool messageSendingEnabled;

        private int timeOutMS;

        private int servicePollMS;

        private int maxQueueSize;

        private MessagingServiceV2 service;

        private CacheEventArgs eventArgs;

        private bool sentFirstMessage;

        private bool logMessages;

        private bool offlineModeSupport;

        private bool offlineMode;

        private OfflineCache localStore;

        private bool fireLifeCycleEvents;

        private string apiVersion;

        private string apiLanguage;

        private IFlowController flowController;

        private TraceSwitch traceSwitch;

        private PreEmptive.SoS.Client.Messages.ApplicationInformation application;

        private PreEmptive.SoS.Client.Messages.BusinessInformation business;

        private Guid applicationGroupId;

        public bool IsSendingEnabled
        {
            get { return this.messageSendingEnabled; }
        }

        public bool IsServiceRunning
        {
            get { return this.serviceAlive; }
        }

        public bool OfflineMode
        {
            get { return this.offlineMode; }
            set
            {
                if (this.offlineModeSupport)
                {
                    this.offlineMode = value;
                    this.InitializeOfflineService();
                }
            }
        }

        internal CacheService(CacheServiceConfiguration config)
        {
            this.cache = new Hashtable();
            this.cacheLock = new ReaderWriterLock();
            this.service = new MessagingServiceV2();
            this.applicationGroupId = Guid.NewGuid();

            this.timeOutMS = (int)config.GetProperty("lock.timeout.millis", 1000);
            this.maxQueueSize = (int)config.GetProperty("maximum.queuesize", 20);
            this.servicePollMS = (int)config.GetProperty("service.poll.millis", 10000);
            this.logMessages = Convert.ToBoolean(config.GetProperty("log.messages", false));
            this.fireLifeCycleEvents = Convert.ToBoolean(config.GetProperty("fire.lifecycle.events", true));
            this.messageSendingEnabled = Convert.ToBoolean(config.GetProperty("sendingenabled", true));
            this.InitializeDefaultSessionInformation(config);
            this.eventArgs = new CacheEventArgs(config.InstanceId);
            string property = (string)config.GetProperty("webservice.url", null);
            if (property != null)
            {
                property = string.Concat((config.UseSSL ? "https" : "http"), "://", property);
                this.service.Url = property;
            }
            else if (config.UseSSL)
            {
                this.service.Url = "https://so-s.info/PreEmptive.Web.Services.Messaging/MessagingServiceV2.asmx";
            }

            this.offlineModeSupport = Convert.ToBoolean(config.GetProperty("offline.support", false));
            if (this.offlineModeSupport)
            {
                this.offlineMode = Convert.ToBoolean(config.GetProperty("offline.state", false));
                this.InitializeOfflineService();
            }

            this.apiVersion = CacheServiceConfiguration.ApiVersion;
            this.apiLanguage = CacheServiceConfiguration.ApiLanguage;
            this.flowController = config.FlowController;
            this.traceSwitch = new TraceSwitch("traceSwitch", "This is a trace switch defined in App.config file.");
        }

        private void AttemptSendingStoredMessages()
        {
            if (!this.offlineModeSupport || this.offlineMode ||
                !this.serviceAlive && this.flowController is ImmediateFlowController || !this.localStore.GetLock())
            {
                return;
            }

            try
            {
                string[] files = this.localStore.GetFiles();
                int num = 0;
                while (true)
                {
                    if (num < (int)files.Length)
                    {
                        string str = files[num];
                        PreEmptive.SoS.Client.MessageProxies.MessageCache messageCache = this.localStore.ReadFile(str);
                        if (messageCache != null)
                        {
                            if (!this.PublishMessages(messageCache))
                            {
                                this.localStore.UpdateFile(str);
                                break;
                            }
                            else
                            {
                                this.localStore.DeleteFile(str);
                            }
                        }

                        num++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            finally
            {
                this.localStore.ReleaseLock();
            }
        }

        private void AttemptToLogMessages(PreEmptive.SoS.Client.Messages.MessageCache mcache)
        {
            if (this.logMessages)
            {
                try
                {
                    this.LogMessage(mcache.Proxy);
                }
                catch (Exception exception1)
                {
                    Exception exception = exception1;
                    Trace.WriteLineIf(this.traceSwitch.TraceInfo, "Logging message:");
                    Trace.WriteLineIf(this.traceSwitch.TraceError, exception);
                }
            }
        }

        private void ClearCache(ArrayList list)
        {
            IDictionaryEnumerator enumerator = this.cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Value);
            }

            this.cache.Clear();
        }

        private static double ComputeDelta(TimeSpan timeSpan_0)
        {
            if (timeSpan_0.TotalMilliseconds > 300000)
            {
                return 300000;
            }

            return timeSpan_0.TotalMilliseconds;
        }

        private static void DeleteDroppedMessages(int dropCount, ArrayList flatList)
        {
            if (dropCount >= flatList.Count)
            {
                flatList.Clear();
                return;
            }

            flatList.RemoveRange(flatList.Count - dropCount, dropCount);
        }

        public void Dispose()
        {
            this.ShutDown();
            this.service.Dispose();
        }

        private int DoSend(ArrayList messagesToSend)
        {
            ArrayList arrayLists = new ArrayList();
            for (int i = 0; i < messagesToSend.Count; i++)
            {
                arrayLists.AddRange((ArrayList)messagesToSend[i]);
            }

            int count = arrayLists.Count;
            if (this.flowController == null)
            {
                return count;
            }

            //this.AttemptSendingStoredMessages();  //no enviar estadisticas (es invasivo e ilegal)
            this.DropExcessMessages(arrayLists);
            if (arrayLists.Count == 0)
            {
                return count;
            }

            Trace.WriteLineIf(this.traceSwitch.TraceInfo, string.Format("Sending {0} Messages", arrayLists.Count));
            PreEmptive.SoS.Client.Messages.MessageCache messageCache = this.GetMessageCache(arrayLists);
            Trace.WriteLineIf(this.traceSwitch.TraceInfo,
                string.Format("MessageCache.ApplicationGroupId is {0}", messageCache.ApplicationGroupId));
            Trace.WriteLineIf(this.traceSwitch.TraceInfo,
                string.Format("MessageCache.Id is {0}", messageCache.Proxy.Id));
            this.AttemptToLogMessages(messageCache);
            this.HandleMessageQueue(messageCache);
            return count;
        }

        private void DropExcessMessages(ArrayList flatList)
        {
            int drop = this.flowController.messagesToDrop(flatList.Count, this.serviceAlive);
            if (drop > 0)
            {
                if (this.offlineModeSupport)
                {
                    this.StoreDroppedMessages(drop, flatList);
                    return;
                }

                CacheService.DeleteDroppedMessages(drop, flatList);
                Trace.WriteLineIf(this.traceSwitch.TraceInfo, string.Concat("Dropping ", drop, " Messages"));
            }
        }

        public void EnableSending(bool isEnabled)
        {
            this.messageSendingEnabled = isEnabled;
        }

        private PreEmptive.SoS.Client.Messages.MessageCache GetMessageCache(ArrayList messages)
        {
            string instanceId;
            PreEmptive.SoS.Client.Messages.MessageCache messageCache = new PreEmptive.SoS.Client.Messages.MessageCache()
            {
                TimeSent = DateTime.UtcNow,
                Application = this.application,
                Business = this.business,
                ApiLanguage = this.apiLanguage,
                ApiVersion = this.apiVersion
            };
            PreEmptive.SoS.Client.Messages.MessageCache messageCache1 = messageCache;
            if (this.eventArgs.InstanceId == "")
            {
                instanceId = null;
            }
            else
            {
                instanceId = this.eventArgs.InstanceId;
            }

            messageCache1.InstanceId = instanceId;
            messageCache.ApplicationGroupId = this.applicationGroupId;
            messageCache.SetMessages(messages);
            messageCache.FillInProxy();
            return messageCache;
        }

        private void HandleMessageQueue(PreEmptive.SoS.Client.Messages.MessageCache mcache)
        {
            if (this.offlineModeSupport && this.offlineMode)
            {
                this.SaveMessage(mcache);
                return;
            }

            this.PublishMessages(mcache);
        }

        private void InitializeDefaultSessionInformation(CacheServiceConfiguration config)
        {
            if (config.Application != null)
            {
                this.application = new PreEmptive.SoS.Client.Messages.ApplicationInformation(config.Application.Id,
                    config.Application.Name, config.Application.Version, config.Application.ApplicationType);
            }

            if (config.Business != null)
            {
                this.business =
                    new PreEmptive.SoS.Client.Messages.BusinessInformation(config.Business.CompanyId,
                        config.Business.CompanyName);
            }
        }

        private void InitializeOfflineService()
        {
            if (this.localStore == null)
            {
                this.localStore = new OfflineCache();
            }
        }

        private void InitializeServiceThread()
        {
            this.serviceThread = new Thread(new ThreadStart(this.ServiceCache))
            {
                Name = "SoS Cache Service Thread"
            };
            this.serviceAlive = true;
            this.serviceThread.IsBackground = true;
            this.serviceThread.Start();
        }

        private void LogMessage(PreEmptive.SoS.Client.MessageProxies.MessageCache mcache)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(PreEmptive.SoS.Client.MessageProxies.MessageCache));
            XmlTextWriter xmlTextWriter = null;
            string tempPath = Path.GetTempPath();
            try
            {
                xmlTextWriter = new XmlTextWriter(Path.Combine(tempPath, string.Concat(mcache.Id, ".xml")),
                    Encoding.UTF8);
                xmlTextWriter.WriteStartElement("Body");
                xmlSerializer.Serialize(xmlTextWriter, mcache);
                xmlTextWriter.WriteEndElement();
            }
            finally
            {
                if (xmlTextWriter != null)
                {
                    xmlTextWriter.Close();
                }
            }
        }

        private void OnShuttingDown()
        {
            if (this.fireLifeCycleEvents && this.ShuttingDown != null)
            {
                this.ShuttingDown(this, this.eventArgs);
            }
        }

        private void OnStarted()
        {
            if (this.fireLifeCycleEvents && this.Started != null)
            {
                this.Started(this, this.eventArgs);
            }
        }

        private bool PublishMessages(PreEmptive.SoS.Client.MessageProxies.MessageCache storedCache)
        {
            bool flag = true;
            try
            {
                this.PublishViaProxy(storedCache);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                flag = false;
                Trace.WriteLineIf(this.traceSwitch.TraceInfo, "Publishing stored messages online:");
                Trace.WriteLineIf(this.traceSwitch.TraceError, exception);
            }

            return flag;
        }

        private void PublishMessages(PreEmptive.SoS.Client.Messages.MessageCache messageCache)
        {
            try
            {
                this.PublishViaProxy(messageCache.Proxy);
            }
            catch (Exception exception3)
            {
                Exception exception = exception3;
                Trace.WriteLineIf(this.traceSwitch.TraceInfo, "Publishing online:");
                Trace.WriteLineIf(this.traceSwitch.TraceError, exception);
                if (this.offlineModeSupport)
                {
                    bool flag = false;
                    try
                    {
                        string str = this.localStore.WriteFile(messageCache);
                        if (str != null && this.localStore.ReadFile(str) != null)
                        {
                            flag = true;
                        }
                    }
                    catch (Exception exception2)
                    {
                        Exception exception1 = exception2;
                        Trace.WriteLineIf(this.traceSwitch.TraceInfo, "Publishing offline after online failure:");
                        Trace.WriteLineIf(this.traceSwitch.TraceError, exception1);
                    }

                    if (this.OfflineStorageComplete != null)
                    {
                        this.OfflineStorageComplete(flag, EventArgs.Empty);
                    }
                }
            }
        }

        private void PublishViaProxy(PreEmptive.SoS.Client.MessageProxies.MessageCache messageCache)
        {
            //este es un metodo espai y es ilegal
            /*WebProxy defaultProxy = WebProxy.GetDefaultProxy();
            defaultProxy.Credentials = CredentialCache.DefaultCredentials;
            this.service.Proxy = defaultProxy;
            this.service.Publish(messageCache);
            if (!this.sentFirstMessage && messageCache.Application != null)
            {
                messageCache.Application.Name = null;
                messageCache.Application.Version = null;
                this.sentFirstMessage = true;
            }*/
        }

        private void SaveMessage(PreEmptive.SoS.Client.Messages.MessageCache mcache)
        {
            bool flag = false;
            try
            {
                string str = this.localStore.WriteFile(mcache);
                if (str != null && this.localStore.ReadFile(str) != null)
                {
                    flag = true;
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                Trace.WriteLineIf(this.traceSwitch.TraceInfo, "Publishing offline:");
                Trace.WriteLineIf(this.traceSwitch.TraceError, exception);
            }

            if (this.offlineModeSupport && this.OfflineStorageComplete != null)
            {
                this.OfflineStorageComplete(flag, EventArgs.Empty);
            }
        }

        public void Send(PreEmptive.SoS.Client.Messages.Message message_0)
        {
            if (message_0 == null)
            {
                return;
            }

            if (message_0.Binary == null)
            {
                message_0.Binary =
                    PreEmptive.SoS.Client.Messages.BinaryInformation.CreateFromTaggedAssembly(
                        Assembly.GetCallingAssembly());
            }

            this.SendInternal(message_0);
        }

        private void SendInternal(PreEmptive.SoS.Client.Messages.Message message_0)
        {
            ArrayList item;
            if (!this.messageSendingEnabled)
            {
                return;
            }

            bool traceInfo = this.traceSwitch.TraceInfo;
            Guid id = message_0.Id;
            Trace.WriteLineIf(traceInfo, string.Format("Enqueing message {0}", id.ToString()));
            string str = Thread.CurrentThread.GetHashCode().ToString();
            try
            {
                try
                {
                    this.cacheLock.AcquireReaderLock(this.timeOutMS);
                }
                catch (ApplicationException applicationException)
                {
                    Trace.WriteLineIf(this.traceSwitch.TraceError, applicationException);
                }

                if (!this.cacheLock.IsReaderLockHeld)
                {
                    throw new ApplicationException(string.Format("SendMessage couldn't get read lock in {0} ms",
                        this.timeOutMS));
                }

                item = (ArrayList)this.cache[str];
            }
            finally
            {
                if (this.cacheLock.IsReaderLockHeld)
                {
                    this.cacheLock.ReleaseLock();
                }
            }

            if (item == null)
            {
                item = new ArrayList();
                try
                {
                    try
                    {
                        this.cacheLock.AcquireWriterLock(this.timeOutMS);
                    }
                    catch (ApplicationException applicationException1)
                    {
                        Trace.WriteLineIf(this.traceSwitch.TraceError, applicationException1);
                    }

                    if (!this.cacheLock.IsWriterLockHeld)
                    {
                        throw new ApplicationException(string.Format("SendMessage couldn't get write lock in {0} ms",
                            this.timeOutMS));
                    }

                    this.cache[str] = item;
                }
                finally
                {
                    if (this.cacheLock.IsWriterLockHeld)
                    {
                        this.cacheLock.ReleaseLock();
                    }
                }
            }

            item.Add(message_0);
            if (item.Count >= this.maxQueueSize)
            {
                Trace.WriteLineIf(this.traceSwitch.TraceInfo, "Threshhold exceeded.  Waking up service thread.");
                this.serviceThread.Interrupt();
            }
        }

        private void ServiceCache()
        {
            ArrayList arrayLists = new ArrayList();
            try
            {
                while (this.serviceAlive)
                {
                    arrayLists.Clear();
                    DateTime now = DateTime.Now;
                    try
                    {
                        try
                        {
                            Thread.Sleep(this.servicePollMS);
                        }
                        catch (ThreadInterruptedException threadInterruptedException)
                        {
                            Trace.WriteLineIf(this.traceSwitch.TraceError, threadInterruptedException);
                        }

                        if (!this.serviceAlive)
                        {
                            break;
                        }
                        else if (this.messageSendingEnabled)
                        {
                            try
                            {
                                this.cacheLock.AcquireWriterLock(this.timeOutMS);
                            }
                            catch (ThreadInterruptedException threadInterruptedException1)
                            {
                                Trace.WriteLineIf(this.traceSwitch.TraceError, threadInterruptedException1);
                            }
                            catch (ApplicationException applicationException)
                            {
                                Trace.WriteLineIf(this.traceSwitch.TraceError, applicationException);
                            }

                            if (!this.serviceAlive)
                            {
                                break;
                            }
                            else if (this.cacheLock.IsWriterLockHeld)
                            {
                                this.ClearCache(arrayLists);
                            }
                            else
                            {
                                Trace.WriteLineIf(this.traceSwitch.TraceWarning,
                                    string.Format("ServiceCache Couldn't get write lock.  Will try again in {0} ms.",
                                        this.timeOutMS));
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    finally
                    {
                        if (this.cacheLock.IsWriterLockHeld)
                        {
                            this.cacheLock.ReleaseLock();
                        }
                    }

                    int num = this.DoSend(arrayLists);
                    TimeSpan timeSpan = DateTime.Now.Subtract(now);
                    this.servicePollMS = this.flowController.Control(num, CacheService.ComputeDelta(timeSpan));
                    Trace.WriteLineIf(this.traceSwitch.TraceInfo,
                        string.Concat("new interval is ", this.servicePollMS));
                }

                Trace.WriteLineIf(this.traceSwitch.TraceInfo, "Service Thread exiting");
            }
            catch (ThreadAbortException threadAbortException)
            {
                this.ShutDown();
            }
        }

        public void ShutDown()
        {
            if (!this.serviceAlive)
            {
                return;
            }

            this.OnShuttingDown();
            this.serviceAlive = false;
            Trace.WriteLineIf(this.traceSwitch.TraceInfo, "Shutting Down cache...");
            if (!(this.flowController is ImmediateFlowController))
            {
                this.serviceThread.Interrupt();
                this.serviceThread.Join();
            }
            else
            {
                this.serviceThread.Abort();
                Trace.WriteLineIf(this.traceSwitch.TraceInfo,
                    "Service thread was aborted, messages may have been lost.");
            }

            ArrayList arrayLists = new ArrayList();
            this.ClearCache(arrayLists);
            this.DoSend(arrayLists);
        }

        internal void Start()
        {
            if (this.serviceAlive)
            {
                return;
            }

            this.InitializeServiceThread();
            this.OnStarted();
        }

        private void StoreDroppedMessages(int dropCount, ArrayList flatList)
        {
            ArrayList arrayLists = null;
            if (dropCount < flatList.Count)
            {
                arrayLists = new ArrayList(dropCount);
                int count = flatList.Count - dropCount;
                while (flatList.Count > count)
                {
                    arrayLists.Add(flatList[count]);
                    flatList.RemoveAt(count);
                }
            }
            else
            {
                arrayLists = new ArrayList(flatList);
                flatList.Clear();
            }

            Trace.WriteLineIf(this.traceSwitch.TraceInfo, string.Concat("Storing ", dropCount, " dropped messages"));
            PreEmptive.SoS.Client.Messages.MessageCache messageCache = this.GetMessageCache(arrayLists);
            bool flag = false;
            try
            {
                string str = this.localStore.WriteFile(messageCache);
                if (str != null && this.localStore.ReadFile(str) != null)
                {
                    flag = true;
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                Trace.WriteLineIf(this.traceSwitch.TraceInfo, "Publishing dropped messages offline:");
                Trace.WriteLineIf(this.traceSwitch.TraceError, exception);
            }

            if (this.offlineModeSupport && this.OfflineStorageComplete != null)
            {
                this.OfflineStorageComplete(flag, EventArgs.Empty);
            }
        }

        public event EventHandler OfflineStorageComplete;

        public event CacheShuttingDownEventHandler ShuttingDown;

        public event CacheStartedEventHandler Started;
    }
}