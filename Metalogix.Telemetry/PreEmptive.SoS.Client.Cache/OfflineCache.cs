using PreEmptive.SoS.Client.MessageProxies;
using PreEmptive.SoS.Client.Messages;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace PreEmptive.SoS.Client.Cache
{
    internal class OfflineCache
    {
        public const int MAX_FAILURE_COUNT = 150;

        public const int MINIMUM_TIME_TO_LIVE = 3;

        public const int MAX_BATCH_SIZE = 4194304;

        private IsolatedStorageFile isoStore;

        private TraceSwitch traceSwitch;

        private IsolatedStorageFileStream lockStream;

        private FileStream m_fs;

        public OfflineCache()
        {
            try
            {
                this.isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly,
                    null, null);
                this.traceSwitch = new TraceSwitch("traceSwitch", "This is a trace switch defined in App.config file.");
            }
            catch (Exception)
            {
            }
        }


        public bool CopyFile(string oldName, string newName)
        {
            if (this.isoStore == null)
            {
                return false;
            }

            bool flag = true;
            try
            {
                using (IsolatedStorageFileStream isolatedStorageFileStream =
                       new IsolatedStorageFileStream(oldName, FileMode.Open, this.isoStore))
                {
                    using (IsolatedStorageFileStream isolatedStorageFileStream1 =
                           new IsolatedStorageFileStream(newName, FileMode.Create, this.isoStore))
                    {
                        using (StreamReader streamReader = new StreamReader(isolatedStorageFileStream))
                        {
                            using (StreamWriter streamWriter = new StreamWriter(isolatedStorageFileStream1))
                            {
                                streamWriter.Write(streamReader.ReadToEnd());
                            }
                        }
                    }
                }
            }
            catch
            {
                flag = false;
            }

            return flag;
        }

        private string CreateRisFilename(string timeSentUtc, string string_0, int failCount, DateTime timeCreated)
        {
            return this.CreateRisFilename(timeSentUtc, string_0, failCount.ToString(),
                timeCreated.ToString("yyyyMMddHHmmssfffffff"));
        }

        private string CreateRisFilename(string timeSentUtc, string string_0, string failCount, string timeCreatedUtc)
        {
            string[] strArrays = new string[]
                { timeSentUtc, ".", string_0, ".", failCount, ".", timeCreatedUtc, ".ris" };
            return string.Concat(strArrays);
        }

        public void DeleteFile(string filename)
        {
            if (this.isoStore != null)
            {
                try
                {
                    string[] fileNames = this.isoStore.GetFileNames(filename);
                    if (fileNames != null && (int)fileNames.Length > 0)
                    {
                        this.isoStore.DeleteFile(fileNames[0]);
                    }
                }
                catch (Exception exception)
                {
                }
            }
        }

        private bool ExceedsTTLLimits(int failureCount, DateTime initialCreation, DateTime currentTime)
        {
            if (failureCount < 150)
            {
                return false;
            }

            return initialCreation.AddDays(3) <= currentTime;
        }

        public string[] GetFiles()
        {
            string[] fileNames;
            if (this.isoStore == null)
            {
                fileNames = new string[0];
            }
            else
            {
                try
                {
                    fileNames = this.isoStore.GetFileNames("*.ris");
                    Array.Sort<string>(fileNames);
                }
                catch (Exception exception)
                {
                    fileNames = new string[0];
                }
            }

            return fileNames;
        }

        public bool GetLock()
        {
            try
            {
                if (this.lockStream == null)
                {
                    this.lockStream = new IsolatedStorageFileStream("q.lck", FileMode.OpenOrCreate, this.isoStore);
                }

                if (this.m_fs == null)
                {
                    this.m_fs = typeof(IsolatedStorageFileStream).InvokeMember("m_fs",
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField, null, this.lockStream,
                        null) as FileStream;
                }

                this.m_fs.Lock(0L, 9223372036854775807L);
            }
            catch (Exception exception)
            {
                return false;
            }

            return true;
        }

        public PreEmptive.SoS.Client.MessageProxies.MessageCache ReadFile(string filename)
        {
            PreEmptive.SoS.Client.MessageProxies.MessageCache messageCache;
            if (this.isoStore == null)
            {
                return null;
            }

            try
            {
                PreEmptive.SoS.Client.MessageProxies.MessageCache utcNow = null;
                string[] fileNames = this.isoStore.GetFileNames(filename);
                if (fileNames != null && (int)fileNames.Length > 0)
                {
                    IsolatedStorageFileStream isolatedStorageFileStream =
                        new IsolatedStorageFileStream(filename, FileMode.Open, this.isoStore);
                    XmlSerializer xmlSerializer =
                        new XmlSerializer(typeof(PreEmptive.SoS.Client.MessageProxies.MessageCache));
                    XmlTextReader xmlTextReader = null;
                    try
                    {
                        xmlTextReader = new XmlTextReader(isolatedStorageFileStream);
                        utcNow =
                            (PreEmptive.SoS.Client.MessageProxies.MessageCache)xmlSerializer.Deserialize(xmlTextReader);
                    }
                    finally
                    {
                        if (xmlTextReader != null)
                        {
                            xmlTextReader.Close();
                        }
                    }

                    utcNow.TimeSentUtc = DateTime.UtcNow;
                }

                messageCache = utcNow;
            }
            catch (Exception exception)
            {
                messageCache = null;
            }

            return messageCache;
        }

        public void ReleaseLock()
        {
            if (this.lockStream != null && this.m_fs != null)
            {
                try
                {
                    try
                    {
                        this.m_fs.Unlock(0L, 9223372036854775807L);
                        this.lockStream.Close();
                    }
                    catch (Exception exception)
                    {
                    }
                }
                finally
                {
                    this.lockStream = null;
                    this.m_fs = null;
                }
            }
        }

        public void UpdateFile(string filename)
        {
            int num;
            string str;
            try
            {
                char[] chrArray = new char[] { '.' };
                string[] strArrays = filename.Split(chrArray, 5);
                string str1 = strArrays[0];
                string str2 = strArrays[1];
                if ((int)strArrays.Length <= 3)
                {
                    num = 0;
                    str = DateTime.UtcNow.ToString("yyyyMMddHHmmssfffffff");
                }
                else
                {
                    num = int.Parse(strArrays[2]);
                    str = strArrays[3];
                }

                if (this.ExceedsTTLLimits(num,
                        DateTime.ParseExact(str, "yyyyMMddHHmmssfffffff", CultureInfo.InvariantCulture),
                        DateTime.UtcNow))
                {
                    this.DeleteFile(filename);
                    Trace.WriteLineIf(this.traceSwitch.TraceInfo,
                        string.Format("The message {0} has been dropped due to exceeding its time to live.", filename));
                }
                else if (this.CopyFile(filename, this.CreateRisFilename(str1, str2, (num + 1).ToString(), str)))
                {
                    this.DeleteFile(filename);
                }
            }
            catch
            {
            }
        }

        public string WriteFile(PreEmptive.SoS.Client.Messages.MessageCache cache)
        {
            string str;
            if (this.isoStore == null)
            {
                return null;
            }

            string str1 = cache.Proxy.TimeSentUtc.Ticks.ToString();
            Guid id = cache.Proxy.Id;
            string str2 = this.CreateRisFilename(str1, id.ToString(), 0, DateTime.UtcNow);
            try
            {
                XmlSerializer xmlSerializer =
                    new XmlSerializer(typeof(PreEmptive.SoS.Client.MessageProxies.MessageCache));
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    xmlSerializer.Serialize(memoryStream, cache.Proxy);
                    memoryStream.Seek(0L, SeekOrigin.Begin);
                    if (memoryStream.Length <= 4194304L)
                    {
                        byte[] buffer = memoryStream.GetBuffer();
                        using (IsolatedStorageFileStream isolatedStorageFileStream =
                               new IsolatedStorageFileStream(str2, FileMode.Create, this.isoStore))
                        {
                            while (true)
                            {
                                try
                                {
                                    isolatedStorageFileStream.Write(buffer, 0, (int)memoryStream.Length);
                                    isolatedStorageFileStream.Close();
                                    str = str2;
                                    break;
                                }
                                catch (IsolatedStorageException isolatedStorageException)
                                {
                                    string[] files = this.GetFiles();
                                    if ((int)files.Length != 0)
                                    {
                                        this.DeleteFile(files[0]);
                                        str2 = null;
                                    }
                                    else
                                    {
                                        str = null;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        memoryStream.Close();
                        PreEmptive.SoS.Client.Messages.MessageCache messageCache = cache.Split();
                        messageCache.FillInProxy();
                        cache.FillInProxy();
                        string str3 = this.WriteFile(messageCache);
                        string str4 = this.WriteFile(cache);
                        if (str3 != null)
                        {
                            str = str4;
                        }
                        else
                        {
                            str = null;
                        }
                    }
                }
            }
            catch
            {
                str = null;
            }

            return str;
        }
    }
}