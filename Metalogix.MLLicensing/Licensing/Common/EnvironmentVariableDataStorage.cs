using Metalogix;
using Metalogix.Core;
using Metalogix.DataStructures.Generic;
using Metalogix.Licensing.Cryptography;
using Metalogix.Licensing.Storage;
using Metalogix.Utilities;
using System;

namespace Metalogix.Licensing.Common
{
    internal class EnvironmentVariableDataStorage : IDataStorage, IDisposable
    {
        private readonly string _variableName;

        private readonly bool _isSecure;

        private readonly CommonSerializableTable<string, string> _values;

        public bool Exists
        {
            get
            {
                return !string.IsNullOrEmpty(ConfigurationVariables.GetConfigurationValue<string>(this._variableName));
            }
        }

        public EnvironmentVariableDataStorage(string variableName, bool isSecure)
        {
            if (variableName == null)
            {
                throw new ArgumentNullException("variableName");
            }

            this._variableName = variableName;
            this._isSecure = isSecure;
            this._values = new CommonSerializableTable<string, string>();
            ConfigurationVariables.InitializeConfigurationVariable<string>(ResourceScope.ApplicationAndUserSpecific,
                this._variableName, string.Empty);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
        }

        ~EnvironmentVariableDataStorage()
        {
            this.Dispose(false);
        }

        public string GetValue(string valueName)
        {
            string item = this._values[valueName];
            Logger.Debug.WriteFormat("EnvironmentVariableDataStorage >> GetValue: Key={0}; Val={1}",
                new object[] { valueName, item });
            return item;
        }

        public void Load()
        {
            Logger.Debug.Write("EnvironmentVariableDataStorage >> Load: Entered");
            string configurationValue = ConfigurationVariables.GetConfigurationValue<string>(this._variableName);
            configurationValue = (this._isSecure ? Crypter.Decrypt(configurationValue) : configurationValue);
            Logger.Debug.WriteFormat("EnvironmentVariableDataStorage >> Load: Values loaded={0}",
                new object[] { configurationValue });
            this._values.Clear();
            this._values.FromXML(XmlUtility.StringToXmlNode(configurationValue));
        }

        public void Save()
        {
            string str;
            Logger.Debug.Write("EnvironmentVariableDataStorage >> Save: Entered");
            ILogMethods debug = Logger.Debug;
            object[] objArray = new object[] { this._variableName, this._isSecure, this._values.ToString() };
            debug.WriteFormat("EnvironmentVariableDataStorage >> Save: saving to File={0}; IsSec={1}; Values={2}",
                objArray);
            str = (this._isSecure ? Crypter.Encrypt(this._values.ToXML()) : this._values.ToXML());
            ConfigurationVariables.SetConfigurationValue<string>(this._variableName, str);
            Logger.Debug.Write("EnvironmentVariableDataStorage >> Save: successfull.");
        }

        public void SetValue(string name, string value)
        {
            Logger.Debug.WriteFormat("EnvironmentVariableDataStorage >> SetValue: Key={0}; Val={1}",
                new object[] { name, value });
            this._values[name] = value;
        }
    }
}