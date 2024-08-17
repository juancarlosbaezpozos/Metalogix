using Metalogix;
using Metalogix.DataStructures.Generic;
using System;
using System.IO;
using System.Reflection;
using System.Security;
using System.Xml;

namespace Metalogix.Jobs
{
    public abstract class JobsSettings
    {
        private static JobsSettings.MostRecentlyUsedList s_jobHistoryMRU;

        private static JobsSettings.JobsSettingsTable s_jobHistorySettings;

        public static SecureString AdapterContext
        {
            get
            {
                if (!JobsSettings.JobHistorySettings.ContainsKey("AdapterContext"))
                {
                    return null;
                }

                return Cryptography.DecryptText(JobsSettings.JobHistorySettings["AdapterContext"]);
            }
            set
            {
                JobsSettings.JobHistorySettings["AdapterContext"] =
                    Cryptography.EncryptText(value, Cryptography.ProtectionScope.CurrentUser, null);
                JobsSettings.JobHistorySettings.Save();
            }
        }

        public static string AdapterType
        {
            get
            {
                if (!JobsSettings.JobHistorySettings.ContainsKey("AdapterType"))
                {
                    return null;
                }

                return JobsSettings.JobHistorySettings["AdapterType"];
            }
            set
            {
                JobsSettings.JobHistorySettings["AdapterType"] = value;
                JobsSettings.JobHistorySettings.Save();
            }
        }

        public static string CurrentJobHistoryFile
        {
            get
            {
                if (JobsSettings.JobHistoryMRU.Count <= 0)
                {
                    return JobsSettings.DefaultJobHistoryFile;
                }

                return (string)JobsSettings.JobHistoryMRU[0];
            }
        }

        private static string DefaultJobHistoryFile
        {
            get { return string.Concat(ApplicationData.ApplicationPath, "JobHistory.lst"); }
        }

        public static JobsSettings.MostRecentlyUsedList JobHistoryMRU
        {
            get
            {
                if (JobsSettings.s_jobHistoryMRU == null)
                {
                    JobsSettings.s_jobHistoryMRU =
                        new JobsSettings.MostRecentlyUsedList(JobsSettings.JobHistoryMRUFile, 8);
                }

                return JobsSettings.s_jobHistoryMRU;
            }
        }

        private static string JobHistoryMRUFile
        {
            get { return string.Concat(ApplicationData.ApplicationPath, "JobHistoryMRU.xml"); }
        }

        public static JobsSettings.JobsSettingsTable JobHistorySettings
        {
            get
            {
                if (JobsSettings.s_jobHistorySettings == null)
                {
                    JobsSettings.s_jobHistorySettings =
                        new JobsSettings.JobsSettingsTable(JobsSettings.JobHistorySettingsFile);
                }

                return JobsSettings.s_jobHistorySettings;
            }
        }

        private static string JobHistorySettingsFile
        {
            get { return string.Concat(ApplicationData.ApplicationPath, "JobHistorySettings.xml"); }
        }

        protected JobsSettings()
        {
        }

        public class JobsSettingsTable : SerializableTable<string, string>
        {
            private readonly string m_sFileName;

            public string FileName
            {
                get { return this.m_sFileName; }
            }

            public override bool IsReadOnly
            {
                get { return false; }
            }

            public JobsSettingsTable(XmlNode xmlNode_0)
            {
                this.FromXML(xmlNode_0);
            }

            public JobsSettingsTable(string sFileName)
            {
                this.m_sFileName = sFileName;
                if (File.Exists(sFileName))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(sFileName);
                    this.FromXML(xmlDocument.FirstChild);
                }
            }

            public void Save()
            {
                if (ApplicationData.IsWeb)
                {
                    return;
                }

                File.WriteAllText(this.FileName, base.ToXML());
            }
        }

        public class MostRecentlyUsedList : SerializableList<string>
        {
            private int m_iMaxItems;

            private string m_sFileName;

            public string FileName
            {
                get { return this.m_sFileName; }
            }

            public override bool IsReadOnly
            {
                get { return false; }
            }

            public override bool IsSet
            {
                get { return false; }
            }

            public override string this[string string_0]
            {
                get
                {
                    if (base.Contains(string_0))
                    {
                        return string_0;
                    }

                    return "";
                }
            }

            public int MaxItems
            {
                get { return this.m_iMaxItems; }
            }

            public MostRecentlyUsedList(XmlNode xmlNode_0)
            {
                this.FromXML(xmlNode_0);
            }

            public MostRecentlyUsedList(string sFileName, int iMaxItems)
            {
                this.m_iMaxItems = iMaxItems;
                this.m_sFileName = sFileName;
                if (File.Exists(sFileName))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(sFileName);
                    this.FromXML(xmlDocument.FirstChild);
                }
            }

            public void Add(string sFileName)
            {
                if (base.Count >= 1 && (string)this[0] == sFileName)
                {
                    return;
                }

                if (base.Contains(sFileName))
                {
                    base.RemoveFromCollection(sFileName);
                }

                if (base.Count == this.MaxItems)
                {
                    base.RemoveIndex(base.Count - 1);
                }

                base.Insert(0, sFileName);
                this.Save();
            }

            public void Delete(string sFileName)
            {
                if (!base.Contains(sFileName))
                {
                    return;
                }

                base.RemoveFromCollection(sFileName);
                this.Save();
            }

            private void Save()
            {
                if (ApplicationData.IsWeb)
                {
                    return;
                }

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(base.ToXML());
                xmlDocument.Save(this.FileName);
            }
        }
    }
}