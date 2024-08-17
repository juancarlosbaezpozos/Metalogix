using Metalogix;
using Metalogix.Connectivity.Proxy;
using Metalogix.DataStructures.Generic;
using Metalogix.Utilities;
using System;
using System.IO;
using System.Xml;

namespace Metalogix.Explorer.Proxy
{
    public class ProxyManager : SerializableTable<string, EditProxyOptions>
    {
        private static ProxyManager _INSTANCE;

        private readonly string _file;

        public static ProxyManager Instance
        {
            get
            {
                if (ProxyManager._INSTANCE == null)
                {
                    ProxyManager._INSTANCE = new ProxyManager();
                    ProxyManager._INSTANCE.Load();
                }

                return ProxyManager._INSTANCE;
            }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        private ProxyManager()
        {
            this._file = Path.Combine(ApplicationData.ApplicationPath, "ProxySettings.xml");
        }

        public EditProxyOptions GetOrCreateSettings(Type connectionType)
        {
            return this.GetSettings(connectionType) ?? new EditProxyOptions();
        }

        public EditProxyOptions GetOrCreateSettings<T>()
            where T : Connection
        {
            return this.GetOrCreateSettings(typeof(T));
        }

        public EditProxyOptions GetSettings(Type connectionType)
        {
            string fullName = connectionType.FullName;
            if (!base.ContainsKey(fullName))
            {
                return null;
            }

            return this[fullName];
        }

        public EditProxyOptions GetSettings<T>()
            where T : Connection
        {
            return this.GetSettings(typeof(T));
        }

        public void Load()
        {
            try
            {
                if (File.Exists(this._file))
                {
                    this.Clear();
                    XmlNode xmlNode = XmlUtility.StringToXmlNode(File.ReadAllText(this._file));
                    this.FromXML(xmlNode);
                }
            }
            catch (Exception exception)
            {
            }
        }

        public void Save()
        {
            try
            {
                File.WriteAllText(this._file, base.ToXML());
            }
            catch (Exception exception)
            {
            }
        }

        public void SetSettings(Type connectionType, EditProxyOptions settings)
        {
            if (settings == null)
            {
                return;
            }

            string fullName = connectionType.FullName;
            if (base.ContainsKey(fullName))
            {
                this[fullName] = settings;
                return;
            }

            base.Add(fullName, settings);
        }

        public void SetSettings<T>(EditProxyOptions settings)
            where T : Connection
        {
            this.SetSettings(typeof(T), settings);
        }
    }
}