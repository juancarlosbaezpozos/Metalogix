using System;

namespace Metalogix.SharePoint.Adapters
{
    public class SharePointVersion
    {
        public const string ZERO_VERSION = "0.0.0.0";

        public const string SHAREPOINT2003_BASE_VERSION = "6.0.0.0";

        public const string SHAREPOINT2007_BASE_VERSION = "12.0.0.0";

        public const string SHAREPOINT2010_BASE_VERSION = "14.0.0.0";

        public const string SHAREPOINT2013_BASE_VERSION = "15.0.0.0";

        public readonly static Version SharePointOnlineVersion;

        private Version m_VersionNumber = new Version();

        private SharePointMajorVersion m_MajorVersion;

        public bool IsSharePoint2003
        {
            get { return this.MajorVersion == SharePointMajorVersion.SharePoint2003; }
        }

        public bool IsSharePoint2007
        {
            get { return this.MajorVersion == SharePointMajorVersion.SharePoint2007; }
        }

        public bool IsSharePoint2007OrEarlier
        {
            get
            {
                if (this.MajorVersion > SharePointMajorVersion.SharePoint2007)
                {
                    return false;
                }

                return this.MajorVersionIsSpecified;
            }
        }

        public bool IsSharePoint2007OrLater
        {
            get
            {
                if (this.MajorVersion < SharePointMajorVersion.SharePoint2007)
                {
                    return false;
                }

                return this.MajorVersionIsSpecified;
            }
        }

        public bool IsSharePoint2010
        {
            get { return this.MajorVersion == SharePointMajorVersion.SharePoint2010; }
        }

        public bool IsSharePoint2010OrEarlier
        {
            get
            {
                if (this.MajorVersion > SharePointMajorVersion.SharePoint2010)
                {
                    return false;
                }

                return this.MajorVersionIsSpecified;
            }
        }

        public bool IsSharePoint2010OrLater
        {
            get
            {
                if (this.MajorVersion < SharePointMajorVersion.SharePoint2010)
                {
                    return false;
                }

                return this.MajorVersionIsSpecified;
            }
        }

        public bool IsSharePoint2013
        {
            get { return this.MajorVersion == SharePointMajorVersion.SharePoint2013; }
        }

        public bool IsSharePoint2013OrLater
        {
            get
            {
                if (this.MajorVersion < SharePointMajorVersion.SharePoint2013)
                {
                    return false;
                }

                return this.MajorVersionIsSpecified;
            }
        }

        public bool IsSharePoint2016
        {
            get { return this.MajorVersion == SharePointMajorVersion.SharePoint2016; }
        }

        public bool IsSharePoint2016OrLater
        {
            get
            {
                if (this.MajorVersion < SharePointMajorVersion.SharePoint2016)
                {
                    return false;
                }

                return this.MajorVersionIsSpecified;
            }
        }

        public bool IsSharePointOnline
        {
            get { return this.MajorVersion == SharePointMajorVersion.SharePointOnline; }
        }

        public SharePointMajorVersion MajorVersion
        {
            get { return this.m_MajorVersion; }
        }

        public bool MajorVersionIsSpecified
        {
            get { return this.MajorVersion != SharePointMajorVersion.Unspecified; }
        }

        public Version VersionNumber
        {
            get { return this.m_VersionNumber; }
        }

        public string VersionNumberString
        {
            get { return this.m_VersionNumber.ToString(); }
        }

        static SharePointVersion()
        {
            SharePointVersion.SharePointOnlineVersion = new Version(2147483647, 0, 0, 0);
        }

        public SharePointVersion()
        {
            this.SetVersion("0.0.0.0");
        }

        public SharePointVersion(string sVersionNumber)
        {
            this.SetVersion(sVersionNumber);
        }

        public SharePointVersion(Version version)
        {
            this.SetVersion(version);
        }

        public SharePointVersion Clone()
        {
            return new SharePointVersion(this.VersionNumberString);
        }

        public override bool Equals(object obj)
        {
            return obj != null && (obj.GetType().IsAssignableFrom(typeof(SharePointVersion)) &&
                                   this.VersionNumber == ((SharePointVersion)obj).VersionNumber);
        }

        public bool Equals(SharePointVersion otherVersion)
        {
            return this.VersionNumber == otherVersion.VersionNumber;
        }

        public override int GetHashCode()
        {
            return this.GetHashCode();
        }

        public static string GetInstallationRegKey(SharePointVersion.SKU skuID)
        {
            switch (skuID)
            {
                case SharePointVersion.SKU.SharePointFoundation2010:
                {
                    return "BEED1F75-C398-4447-AEF1-E66E1F0DF91E";
                }
                case SharePointVersion.SKU.SearchServerExpress2010:
                {
                    return "1328E89E-7EC8-4F7E-809E-7E945796E511";
                }
                case SharePointVersion.SKU.SharePointServer2010StandardTrial:
                {
                    return "B2C0B444-3914-4ACB-A0B8-7CF50A8F7AA0";
                }
                case SharePointVersion.SKU.SharePointServer2010Standard:
                {
                    return "3FDFBCC8-B3E4-4482-91FA-122C6432805C";
                }
                case SharePointVersion.SKU.SharePointServer2010EnterpriseTrial:
                {
                    return "88BED06D-8C6B-4E62-AB01-546D6005FE97";
                }
                case SharePointVersion.SKU.SharePointServer2010Enterprise:
                {
                    return "D5595F62-449B-4061-B0B2-0CBAD410BB51";
                }
                case SharePointVersion.SKU.SearchServer2010Trial:
                {
                    return "BC4C1C97-9013-4033-A0DD-9DC9E6D6C887";
                }
                case SharePointVersion.SKU.SearchServer2010:
                {
                    return "08460AA2-A176-442C-BDCA-26928704D80B";
                }
                case SharePointVersion.SKU.ProjectServer2010Trial:
                {
                    return "84902853-59F6-4B20-BC7C-DE4F419FEFAD";
                }
                case SharePointVersion.SKU.ProjectServer2010:
                {
                    return "ED21638F-97FF-4A65-AD9B-6889B93065E2";
                }
                case SharePointVersion.SKU.OfficeWebCompanions2010:
                {
                    return "926E4E17-087B-47D1-8BD7-91A394BC6196";
                }
                case SharePointVersion.SKU.SharePointFoundation2013:
                {
                    return "9FF54EBC-8C12-47D7-854F-3865D4BE8118";
                }
                case SharePointVersion.SKU.ProjectServer2013Preview:
                {
                    return "BC7BAF08-4D97-462C-8411-341052402E71";
                }
                case SharePointVersion.SKU.ProjectServer2013:
                {
                    return "35466B1A-B17B-4DFB-A703-F74E2A1F5F5E";
                }
                case SharePointVersion.SKU.SharePointServer2013Preview:
                {
                    return "CBF97833-C73A-4BAF-9ED3-D47B3CFF51BE";
                }
                case SharePointVersion.SKU.SharePointServer2013:
                {
                    return "C5D855EE-F32B-4A1C-97A8-F0A28CE02F9C";
                }
                case SharePointVersion.SKU.SharePointServer2013EnterprisePreview:
                {
                    return "298A586A-E3C1-42F0-AFE0-4BCFDC2E7CD0";
                }
                case SharePointVersion.SKU.SharePointServer2013Enterprise:
                {
                    return "B7D84C2B-0754-49E4-B7BE-7EE321DCE0A9";
                }
                case SharePointVersion.SKU.MicrosoftOfficeWebAppsServer2013:
                {
                    return "D6B57A0D-AE69-4A3E-B031-1F993EE52EDC";
                }
            }

            return null;
        }

        public static bool operator ==(SharePointVersion v1, SharePointVersion v2)
        {
            return Equals(v1, v2);
        }

        public static bool operator !=(SharePointVersion v1, SharePointVersion v2)
        {
            return !Equals(v1, v2);
        }

        public static bool operator >(SharePointVersion v1, SharePointVersion v2)
        {
            if (v1 == null || v2 == null)
            {
                throw new Exception("ERROR: Null reference when trying to use SharePointVersion.> operator.");
            }

            if (v1.MajorVersion > v2.MajorVersion)
            {
                return true;
            }

            if (v1.MajorVersion != v2.MajorVersion)
            {
                return false;
            }

            return v1.VersionNumber > v2.VersionNumber;
        }

        public static bool operator >=(SharePointVersion v1, SharePointVersion v2)
        {
            if (v1 == null || v2 == null)
            {
                throw new Exception("ERROR: Null reference when trying to use SharePointVersion.>= operator.");
            }

            if (v1 == v2)
            {
                return true;
            }

            return v1 > v2;
        }

        public static bool operator <(SharePointVersion v1, SharePointVersion v2)
        {
            if (v1 == null || v2 == null)
            {
                throw new Exception("ERROR: Null reference when trying to use SharePointVersion.< operator.");
            }

            if (v1.MajorVersion < v2.MajorVersion)
            {
                return true;
            }

            if (v1.MajorVersion != v2.MajorVersion)
            {
                return false;
            }

            return v1.VersionNumber < v2.VersionNumber;
        }

        public static bool operator <=(SharePointVersion v1, SharePointVersion v2)
        {
            if (v1 == null || v2 == null)
            {
                throw new Exception("ERROR: Null reference when trying to use SharePointVersion.<= operator.");
            }

            if (v1 == v2)
            {
                return true;
            }

            return v1 < v2;
        }

        public void SetVersion(string sVersionNumber)
        {
            Version version = null;
            try
            {
                version = new Version(sVersionNumber);
            }
            catch
            {
                version = new Version("0.0.0.0");
            }

            this.SetVersion(version);
        }

        public void SetVersion(Version version)
        {
            this.m_VersionNumber = version ?? new Version("0.0.0.0");
            this.m_MajorVersion = SharePointMajorVersion.Unspecified;
            if (Enum.IsDefined(typeof(SharePointMajorVersion), this.m_VersionNumber.Major))
            {
                this.m_MajorVersion = (SharePointMajorVersion)this.m_VersionNumber.Major;
            }

            if (this.m_MajorVersion == SharePointMajorVersion.SharePoint2003Portal)
            {
                this.m_MajorVersion = SharePointMajorVersion.SharePoint2003;
            }
        }

        public override string ToString()
        {
            return this.VersionNumber.ToString();
        }

        public enum SKU
        {
            SharePointFoundation2010,
            SearchServerExpress2010,
            SharePointServer2010StandardTrial,
            SharePointServer2010Standard,
            SharePointServer2010EnterpriseTrial,
            SharePointServer2010Enterprise,
            SearchServer2010Trial,
            SearchServer2010,
            ProjectServer2010Trial,
            ProjectServer2010,
            OfficeWebCompanions2010,
            SharePointFoundation2013,
            ProjectServer2013Preview,
            ProjectServer2013,
            SharePointServer2013Preview,
            SharePointServer2013,
            SharePointServer2013EnterprisePreview,
            SharePointServer2013Enterprise,
            MicrosoftOfficeWebAppsServer2013
        }
    }
}