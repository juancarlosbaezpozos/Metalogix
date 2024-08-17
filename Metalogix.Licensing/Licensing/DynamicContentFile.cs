using Metalogix.Licensing.LicenseServer;
using Metalogix.Licensing.Logging;
using Metalogix.Licensing.Storage;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Metalogix.Licensing
{
    public class DynamicContentFile : AbstractFile
    {
        private string m_sLockMutexName;

        private long m_lLoadTimeUsedData;

        private long m_lLoadTimeUsedItems;

        private readonly string m_sDataID;

        private readonly bool m_bForceLoadOldValue;

        public bool DataExistsInStorage { get; private set; }

        public long UsedData { get; set; }

        public long UsedItems { get; set; }

        public DynamicContentFile(IDataStorage storage) : this(storage, null, true, null)
        {
        }

        public DynamicContentFile(IDataStorage storage, string dataID, string lockMutexName) : this(storage, dataID,
            false, lockMutexName)
        {
        }

        public DynamicContentFile(IDataStorage storage, string dataID, bool forceLoadOldValue, string lockMutexName) :
            base(storage)
        {
            this.m_sLockMutexName = lockMutexName;
            this.m_sDataID = (dataID != null ? dataID.Trim() : string.Empty);
            this.m_bForceLoadOldValue = forceLoadOldValue;
            if (base.Exists)
            {
                this.Load();
            }
        }

        public sealed override void Load()
        {
            bool flag;
            Logger.Debug.Write("DynamicContentFile >> Load: Entered");
            if (!base.StorageHandler.Exists)
            {
                Logger.Warning.Write("DynamicContentFile >> Load: Not exists, throwing exception");
                throw new FileNotFoundException("Content file doesn't exists.");
            }

            base.StorageHandler.Load();
            ClientVersionSettings clientVersionSetting = new ClientVersionSettings(base.StorageHandler);
            this.ReadUsageData(out this.m_lLoadTimeUsedData, out this.m_lLoadTimeUsedItems, out flag,
                clientVersionSetting);
            this.UsedData = this.m_lLoadTimeUsedData;
            this.UsedItems = this.m_lLoadTimeUsedItems;
            this.DataExistsInStorage = flag;
            Logger.Debug.Write("DynamicContentFile >> Load: Data valid, load OK.");
        }

        private void ReadUsageData(out long usedData, out long usedItems, out bool dataExists,
            ClientVersionSettings clientFacility)
        {
            string value = null;
            string str = null;
            if (!string.IsNullOrEmpty(this.m_sDataID) && clientFacility.Strict().IsUserDataTiedToServerAndKey)
            {
                value = base.StorageHandler.GetValue(string.Concat("UsedData", this.m_sDataID));
                str = base.StorageHandler.GetValue(string.Concat("UsedItems", this.m_sDataID));
            }

            if (this.m_bForceLoadOldValue && (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(str)) &&
                !clientFacility.Strict().IsUserDataTiedToServerAndKey)
            {
                value = base.StorageHandler.GetValue("UsedData");
                str = base.StorageHandler.GetValue("UsedItems");
            }

            usedData = (!string.IsNullOrEmpty(value) ? Convert.ToInt64(value) : (long)0);
            usedItems = (!string.IsNullOrEmpty(str) ? Convert.ToInt64(str) : (long)0);
            dataExists = !string.IsNullOrEmpty(value);
        }

        public override void Save()
        {
            long num;
            long num1;
            bool flag;
            GlobalLock globalLock;
            Logger.Debug.Write("DynamicContentFile >> Save: Entered");
            GlobalLock globalLock1 = null;
            if (!string.IsNullOrEmpty(this.m_sLockMutexName))
            {
                globalLock = new GlobalLock(this.m_sLockMutexName);
            }
            else
            {
                globalLock = null;
            }

            globalLock1 = globalLock;
            try
            {
                if (base.StorageHandler.Exists)
                {
                    base.StorageHandler.Load();
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                Logger.Debug.Write(string.Format("DynamicContentFile.Save -> Load Exception : {0}", exception.Message));
            }

            if (!base.StorageHandler.Exists)
            {
                long num2 = (long)0;
                num1 = num2;
                num = num2;
            }
            else
            {
                ClientVersionSettings clientVersionSetting = new ClientVersionSettings(base.StorageHandler);
                this.ReadUsageData(out num, out num1, out flag, clientVersionSetting);
            }

            if (num < (long)0)
            {
                num = (long)0;
            }

            if (num1 < (long)0)
            {
                num1 = (long)0;
            }

            long usedData = num + this.UsedData - this.m_lLoadTimeUsedData;
            long usedItems = num1 + this.UsedItems - this.m_lLoadTimeUsedItems;
            base.StorageHandler.SetValue("ClientVersion", Tools.ClientVersion.ToString());
            base.StorageHandler.SetValue(string.Concat("UsedData", this.m_sDataID), usedData.ToString());
            base.StorageHandler.SetValue(string.Concat("UsedItems", this.m_sDataID), usedItems.ToString());
            base.StorageHandler.Save();
            this.m_lLoadTimeUsedData = usedData;
            this.m_lLoadTimeUsedItems = usedItems;
            this.UsedData = this.m_lLoadTimeUsedData;
            this.UsedItems = this.m_lLoadTimeUsedItems;
            this.DataExistsInStorage = true;
            Logger.Debug.Write("DynamicContentFile >> Save: Finished successfully.");
        }
    }
}