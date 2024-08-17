using Metalogix;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Metalogix.Licensing.SK
{
    public class BPOSSubAssocCollection : IEnumerable<BPOSSubAssoc>, IEnumerable, ILicenseStatus
    {
        private const string _REGISTRY_KEY = "LicenseCheck";

        private readonly List<BPOSSubAssoc> _innerList = new List<BPOSSubAssoc>();

        private BPOSLicense _license;

        private SKLicenseStatus _status = SKLicenseStatus.Invalid;

        private LicenseStatus _licInfo;

        private BPOSLicenseType _licenseType;

        private bool _isLoaded;

        public DateTime ExpirationDate
        {
            get
            {
                if (this._licInfo == null)
                {
                    return DateTime.MinValue;
                }

                return this._licInfo.ExpirationDate;
            }
        }

        public bool IsLoaded
        {
            get { return this._isLoaded; }
        }

        public BPOSSubAssoc this[string subscriptionGUID]
        {
            get
            {
                BPOSSubAssoc bPOSSubAssoc;
                List<BPOSSubAssoc>.Enumerator enumerator = this._innerList.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        BPOSSubAssoc current = enumerator.Current;
                        if (string.Compare(current.Guid, subscriptionGUID, StringComparison.OrdinalIgnoreCase) != 0)
                        {
                            continue;
                        }

                        bPOSSubAssoc = current;
                        return bPOSSubAssoc;
                    }

                    return null;
                }
                finally
                {
                    ((IDisposable)enumerator).Dispose();
                }

                return bPOSSubAssoc;
            }
        }

        public int LicensedSeats
        {
            get
            {
                if (this._licInfo == null)
                {
                    return 0;
                }

                return this._licInfo.LicensedStorageOrAccounts;
            }
        }

        public BPOSLicenseType LicenseType
        {
            get { return this._licenseType; }
        }

        public SKLicenseStatus StatusCode
        {
            get { return this._status; }
        }

        public int UsedSeats
        {
            get
            {
                if (this._licInfo == null)
                {
                    return 0;
                }

                return Convert.ToInt32(this._licInfo.UsedStorageOrUsers / (long)1073741824);
            }
        }

        public BPOSSubAssocCollection()
        {
        }

        public BPOSSubAssocCollection(IEnumerable<BPOSSubAssoc> items)
        {
            this._innerList.AddRange(items);
        }

        internal void Add(BPOSSubAssoc assoc)
        {
            if (this._license == null)
            {
                throw new Exception("License object has to be set before calling Add.");
            }

            if (!this._license.IsSet)
            {
                throw new Exception("License key has to be set before calling Add.");
            }

            Association association = Association.Create(assoc);
            BPOSSubAssoc byID = this.GetByID(assoc.ID);
            BPOSSubAssoc byGUID = this.GetByGUID(assoc.Guid);
            string str = this._license.LicenseKey.Replace("-", "");
            foreach (Association association1 in BPOSLicenseServer.Instance.SetAssociation(
                         new Association[] { association }, str))
            {
                this._innerList.Add(BPOSSubAssoc.Create(association1));
            }

            if (byID != null)
            {
                this._innerList.Remove(byID);
            }

            if (byGUID != null && byID != byGUID)
            {
                this._innerList.Remove(byGUID);
            }

            this.FireStatusChanged();
        }

        public ILicenseStatus Clone()
        {
            return new BPOSSubAssocCollection(this._innerList);
        }

        private void FireStatusChanged()
        {
            if (this.StatusChanged != null)
            {
                this.StatusChanged(this, EventArgs.Empty);
            }
        }

        public BPOSSubAssoc GetByGUID(string subscriptionGUID)
        {
            return this[subscriptionGUID];
        }

        public BPOSSubAssoc GetByID(string subscriptionID)
        {
            BPOSSubAssoc bPOSSubAssoc;
            List<BPOSSubAssoc>.Enumerator enumerator = this._innerList.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    BPOSSubAssoc current = enumerator.Current;
                    if (string.Compare(current.ID, subscriptionID, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        continue;
                    }

                    bPOSSubAssoc = current;
                    return bPOSSubAssoc;
                }

                return null;
            }
            finally
            {
                ((IDisposable)enumerator).Dispose();
            }

            return bPOSSubAssoc;
        }

        public IEnumerator<BPOSSubAssoc> GetEnumerator()
        {
            return this._innerList.GetEnumerator();
        }

        internal void LoadFromServer()
        {
            this.LoadFromServer(SKLP.Get.Proxy);
        }

        internal void LoadFromServer(LicenseProxy proxy)
        {
            this._isLoaded = false;
            if (this._license == null)
            {
                throw new Exception("License object has to be set before calling LoadFromServer.");
            }

            if (!this._license.IsSet)
            {
                Logger.Debug.Write("BPOSSubAssocCollection >> LoadFromServer: license is not set, skipping.");
                return;
            }

            this._status = SKLicenseStatus.Invalid;
            try
            {
                Logger.Debug.Write("BPOSSubAssocCollection >> LoadFromServer: reading license info.");
                this._licInfo = BPOSLicenseServer.Instance.ReadLicenseInfo(this._license.LicenseKey, proxy);
                Logger.Debug.WriteFormat("BPOSSubAssocCollection >> LoadFromServer: license status got, info={0}.",
                    new object[] { this._licInfo });
                this._innerList.Clear();
                if (this._licInfo.StatusCode == SKLicenseStatus.Valid ||
                    this._licInfo.StatusCode == SKLicenseStatus.Evaluation)
                {
                    BPOSLicenseInfo bPOSLicenseInfo =
                        BPOSLicenseServer.Instance.CheckLicense(this._license.LicenseKey.Replace("-", ""));
                    ILogMethods debug = Logger.Debug;
                    object[] licenseType = new object[]
                        { bPOSLicenseInfo.License.LicenseType, (int)bPOSLicenseInfo.Associations.Length };
                    debug.WriteFormat("BPOSSubAssocCollection >> CheckLicense: lic.Type={0}, assoc.count={1}.",
                        licenseType);
                    switch (bPOSLicenseInfo.License.LicenseType)
                    {
                        case LicType.POR:
                        {
                            this._licenseType = BPOSLicenseType.Por;
                            break;
                        }
                        case LicType.Commercial:
                        {
                            this._licenseType = BPOSLicenseType.Value;
                            break;
                        }
                        default:
                        {
                            throw new Exception("Incorrect license type.");
                        }
                    }

                    Association[] associations = bPOSLicenseInfo.Associations;
                    for (int i = 0; i < (int)associations.Length; i++)
                    {
                        Association association = associations[i];
                        this._innerList.Add(BPOSSubAssoc.Create(association));
                    }
                }

                this._status = this._licInfo.StatusCode;
                this._isLoaded = true;
            }
            finally
            {
                this.FireStatusChanged();
            }
        }

        internal void SetLicense(BPOSLicense license)
        {
            this._license = license;
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this._innerList.GetEnumerator();
        }

        public override string ToString()
        {
            object[] count = new object[] { this._innerList.Count, this._license, this._status, null };
            count[3] = (this._licInfo != null ? this._licInfo.ExpirationDate : DateTime.MinValue);
            return string.Format("InnerList.Count={0}, License={1}, Status={2}, ExpirationDate={3}", count);
        }

        internal event EventHandler StatusChanged;
    }
}