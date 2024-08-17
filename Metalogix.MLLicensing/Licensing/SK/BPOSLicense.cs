using Metalogix;
using Metalogix.Licensing;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Metalogix.Licensing.SK
{
    public sealed class BPOSLicense : MLLicenseSKBase
    {
        private BPOSPartnerInfo _partner;

        public Metalogix.Licensing.SK.BPOSLicenseType BPOSLicenseType
        {
            get { return this.SubscriptionsInternal.LicenseType; }
        }

        public override DateTime ExpiryDate
        {
            get
            {
                if (this.LicenseType == MLLicenseType.Evaluation)
                {
                    return this._installInfo.ExpirationDate;
                }

                if (this._status == null)
                {
                    return DateTime.MaxValue;
                }

                return this._status.ExpirationDate;
            }
        }

        public int LicensedUsersCount
        {
            get { return this.SubscriptionsInternal.LicensedSeats; }
        }

        public BPOSPartnerInfo PartnerInfo
        {
            get
            {
                BPOSPartnerInfo bPOSPartnerInfo = this._partner;
                if (bPOSPartnerInfo == null)
                {
                    BPOSPartnerInfo partnerInfo = BPOSLicenseServer.Instance.GetPartnerInfo(this.LicenseKey);
                    BPOSPartnerInfo bPOSPartnerInfo1 = partnerInfo;
                    this._partner = partnerInfo;
                    bPOSPartnerInfo = bPOSPartnerInfo1;
                }

                return bPOSPartnerInfo;
            }
        }

        public IEnumerable<BPOSSubAssoc> Subscriptions
        {
            get { return this.SubscriptionsInternal; }
        }

        private BPOSSubAssocCollection SubscriptionsInternal
        {
            get { return (BPOSSubAssocCollection)this._status; }
            set { this._status = value; }
        }

        public int UsedUsersCount
        {
            get { return this.SubscriptionsInternal.UsedSeats; }
        }

        public BPOSLicense()
        {
            this.SetStatus(new BPOSSubAssocCollection());
        }

        public BPOSLicense(string key) : base(key)
        {
        }

        public override ILicenseStatus CheckOnline(LicenseProxy proxy)
        {
            if (!base.IsSet)
            {
                return null;
            }

            Logger.Debug.WriteFormat("BPOSLicense >> CheckOnline: Checking license key online, proxy '{0}'",
                new object[] { proxy });
            BPOSSubAssocCollection bPOSSubAssocCollections = new BPOSSubAssocCollection();
            bPOSSubAssocCollections.SetLicense(this);
            bPOSSubAssocCollections.LoadFromServer(proxy);
            Logger.Debug.WriteFormat("BPOSLicense >> CheckOnline: License successfully checked", new object[0]);
            return bPOSSubAssocCollections;
        }

        protected override MLLicenseType GetLicenseType(bool throwException)
        {
            if (MLLicenseProviderSK.IsRestrictionDisabled)
            {
                return MLLicenseType.Commercial;
            }

            if (this._type == MLLicenseType.Invalid)
            {
                MLLicenseSKBase.ThrowLicenseExceptionIfSet(throwException, "Incorrect license key.");
                return MLLicenseType.Invalid;
            }

            if (!base.IsSet)
            {
                MLLicenseSKBase.ThrowLicenseExceptionIfSet(throwException, "License key not set.");
            }

            if (this._status == null || !this._status.IsLoaded)
            {
                return this._type;
            }

            switch (this._status.StatusCode)
            {
                case SKLicenseStatus.Valid:
                {
                    return MLLicenseType.Commercial;
                }
                case SKLicenseStatus.Invalid:
                case SKLicenseStatus.Unreserved:
                {
                    MLLicenseSKBase.ThrowLicenseExceptionIfSet(throwException,
                        "License key was determined as invalid by the license server.");
                    return MLLicenseType.Invalid;
                }
                case SKLicenseStatus.Disabled:
                {
                    MLLicenseSKBase.ThrowLicenseExceptionIfSet(throwException,
                        "License key was disabled on the license server, please contact the support for details.");
                    return MLLicenseType.Invalid;
                }
                case SKLicenseStatus.Expired:
                {
                    MLLicenseSKBase.ThrowLicenseExceptionIfSet(throwException, "License key has expired.");
                    return MLLicenseType.Invalid;
                }
                default:
                {
                    MLLicenseSKBase.ThrowLicenseExceptionIfSet(throwException,
                        "License key was determined as invalid by the license server.");
                    return MLLicenseType.Invalid;
                }
            }
        }

        public BPOSSubAssoc GetSubscriptionAssociation(string subscriptionGUID)
        {
            Logger.Debug.WriteFormat(
                "BPOSLicense >> GetSubscriptionAssociation: Getting subscription association '{0}'",
                new object[] { subscriptionGUID });
            BPOSSubAssoc item = this.SubscriptionsInternal[subscriptionGUID];
            Logger.Debug.WriteFormat("BPOSLicense >> GetSubscriptionAssociation: successfully finished '{0}'",
                new object[] { item });
            return item;
        }

        public bool IsSubscriptionLicensed(string subsciptionGUID)
        {
            if (MLLicenseProviderSK.IsRestrictionDisabled)
            {
                Logger.Warning.WriteFormat("BPOSLicense >> IsSubscriptionLicensed: Restriction is disabled",
                    new object[0]);
                return true;
            }

            BPOSSubAssoc item = this.SubscriptionsInternal[subsciptionGUID];
            Logger.Debug.WriteFormat("BPOSLicense >> IsSubscriptionLicensed: subscription '{0}'",
                new object[] { item });
            if (item == null)
            {
                return false;
            }

            if (item.Status == BPOSSubscriptionStatus.Partner || item.Status == BPOSSubscriptionStatus.PartnerPending)
            {
                return true;
            }

            return item.Status == BPOSSubscriptionStatus.Purchased;
        }

        public override void Load()
        {
            try
            {
                this._installInfo.Load();
                string str = RegistryHelper.LoadValue(RegistryHelper.Base.LocalMachine,
                    string.Concat(MLLicenseProviderSK.RegistrySoftwareBase, SKLP.Get.InitData.RegistryBase),
                    "LicenseKey") as string;
                if (str != null)
                {
                    Logger.Debug.WriteFormat("BPOSLicense >> Load: License key loaded from registry '{0}'",
                        new object[] { str });
                    base.SetKey(str);
                    this.SubscriptionsInternal = (BPOSSubAssocCollection)this.CheckOnline(SKLP.Get.Proxy);
                }
                else
                {
                    Logger.Debug.WriteFormat(
                        string.Format("BPOSLicense >> Load: Registry key '{0}' doesn`t extists.",
                            string.Concat(SKLP.Get.InitData.RegistryBase, "\\LicenseKey")), new object[0]);
                }
            }
            catch (LicenseHackedException licenseHackedException)
            {
                throw;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                Trace.WriteLine(string.Concat("MLLicenseSK >> Load: ", exception));
                throw new Exception("Unable to load license key.", exception);
            }
        }

        public void Reload()
        {
            Logger.Debug.WriteFormat("BPOSLicense >> Reload: started", new object[0]);
            this.SubscriptionsInternal.LoadFromServer();
            this._partner = null;
            Logger.Debug.WriteFormat("BPOSLicense >> Reload: successfully finished", new object[0]);
        }

        public override void Save()
        {
            try
            {
                RegistryHelper.SaveValue(RegistryHelper.Base.LocalMachine,
                    string.Concat(MLLicenseProviderSK.RegistrySoftwareBase, SKLP.Get.InitData.RegistryBase),
                    "LicenseKey", (base.IsSet ? this.LicenseKey : ""));
                Trace.WriteLine("MLLicenseSK >> Save >> License key were set successfully.");
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                Trace.WriteLine(string.Concat("MLLicenseSK >> Unable to set the license key. ", exception));
                throw new Exception("Unable to save license key.", exception);
            }
        }

        internal override void SetStatus(ILicenseStatus status)
        {
            MLLicenseType licenseType = this.LicenseType;
            try
            {
                Logger.Debug.WriteFormat("BPOSLicense >> SetStatus: started '{0}'", new object[] { status });
                if (status == null)
                {
                    throw new ArgumentNullException("Status");
                }

                if (!(status is BPOSSubAssocCollection))
                {
                    throw new Exception("Incorrect status. Status has to be BPOSSubAssocCollection");
                }

                if (this.SubscriptionsInternal != null)
                {
                    this.SubscriptionsInternal.StatusChanged -=
                        new EventHandler(this.SubscriptionsInternal_StatusChanged);
                }

                this.SubscriptionsInternal = (BPOSSubAssocCollection)status;
                this.SubscriptionsInternal.SetLicense(this);
                this.SubscriptionsInternal.StatusChanged += new EventHandler(this.SubscriptionsInternal_StatusChanged);
                Logger.Debug.WriteFormat("BPOSLicense >> SetStatus: successfully finished", new object[0]);
            }
            finally
            {
                if (licenseType != this.LicenseType)
                {
                    base.FireStatusChanged();
                }
            }
        }

        internal override void SetStatus(ILicenseStatus status, bool isNewStatus)
        {
            this.SetStatus(status);
        }

        public void SetSubscriptionAssociation(string subscriptionGUID, string subscriptionID, long seats)
        {
            BPOSSubAssoc bPOSSubAssoc;
            if (this.BPOSLicenseType == Metalogix.Licensing.SK.BPOSLicenseType.Value)
            {
                DateTime? nullable = null;
                bPOSSubAssoc = new BPOSSubAssoc(subscriptionID, subscriptionGUID, seats, nullable,
                    BPOSSubscriptionStatus.Purchased, null);
            }
            else
            {
                bPOSSubAssoc = new BPOSSubAssoc(subscriptionID, subscriptionGUID, seats);
            }

            BPOSSubAssoc bPOSSubAssoc1 = bPOSSubAssoc;
            Logger.Debug.WriteFormat(
                "BPOSLicense >> SetSubscriptionAssociation: Creating new subscription association '{0}'",
                new object[] { bPOSSubAssoc1 });
            this.SubscriptionsInternal.Add(bPOSSubAssoc1);
            Logger.Debug.WriteFormat("BPOSLicense >> SetSubscriptionAssociation: successfully finished", new object[0]);
        }

        private void SubscriptionsInternal_StatusChanged(object sender, EventArgs e)
        {
            base.FireStatusChanged();
        }

        public void UpdateSubscriptionAssociation(string subscriptionGUID, string subscriptionID, long seats)
        {
            BPOSSubAssoc bPOSSubAssoc;
            if (this.BPOSLicenseType == Metalogix.Licensing.SK.BPOSLicenseType.Value)
            {
                DateTime? nullable = null;
                bPOSSubAssoc = new BPOSSubAssoc(subscriptionID, subscriptionGUID, seats, nullable,
                    BPOSSubscriptionStatus.Purchased, null);
            }
            else
            {
                bPOSSubAssoc = new BPOSSubAssoc(subscriptionID, subscriptionGUID, seats);
            }

            BPOSSubAssoc bPOSSubAssoc1 = bPOSSubAssoc;
            Logger.Debug.WriteFormat(
                "BPOSLicense >> UpdateSubscriptionAssociation: Updating subscription association '{0}'",
                new object[] { bPOSSubAssoc1 });
            this.SubscriptionsInternal.Add(bPOSSubAssoc1);
            Logger.Debug.WriteFormat("BPOSLicense >> UpdateSubscriptionAssociation: successfully finished",
                new object[0]);
        }
    }
}