using Metalogix.Licensing.LicenseServer;
using System;
using System.IO;

namespace Metalogix.Licensing.Common
{
    public class MigrationLicensingConverter : ILicensingConverter
    {
        private readonly string _oldLicensePath;

        private LicenseKey _newLicenseKey;

        private string _oldLicenseKey;

        public bool Exists
        {
            get { return File.Exists(this._oldLicensePath); }
        }

        public LicenseKey Key
        {
            get { return this._newLicenseKey; }
        }

        public string OldKey
        {
            get
            {
                if (!this.Exists)
                {
                    return null;
                }

                if (this._oldLicenseKey == null)
                {
                    using (TextReader streamReader = new StreamReader(this._oldLicensePath))
                    {
                        this._oldLicenseKey = streamReader.ReadToEnd().Trim();
                    }
                }

                return this._oldLicenseKey;
            }
        }

        public MigrationLicensingConverter(string oldLicensePath)
        {
            if (oldLicensePath == null)
            {
                throw new ArgumentNullException("oldLicensePath");
            }

            this._oldLicensePath = oldLicensePath;
        }

        public void Convert(MLLicenseProviderCommon man)
        {
            this._newLicenseKey = new LicenseKey(man.ConvertOldLicenseKey(this.OldKey));
        }
    }
}