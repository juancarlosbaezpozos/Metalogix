using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Metalogix.SharePoint.Adapters
{
    public sealed class WebPartTemplateResourceManager
    {
        public const string BASE_TEMPLATE_FILE_LOCATION = "Metalogix.SharePoint.Adapters.WebPartTemplates.Templates";

        private const string WEB_PART_PAGE_TEMPLATE_FILE_PATTERN = "spstd{0}.aspx";

        private const string WIKI_PAGE_TEMPLATE_FILE = "wkpstd.aspx";

        private const string DASHBOARD_PAGE_TEMPLATE_FILE_PATTERN = "DashboardTemplate{0}.aspx";

        private Metalogix.SharePoint.Adapters.SharePointVersion m_SharePointVersion =
            new Metalogix.SharePoint.Adapters.SharePointVersion();

        private WebPartTemplateResourceLocation m_resourceLocation =
            WebPartTemplateResourceLocation.EmbeddedWithinAssembly;

        private string m_sCustomTemplatePath;

        private string m_sBaseTemplateDirectory;

        private int[] m_possibleLanguages;

        private string m_WebPartPageTemplateDirectory;

        private string m_WikiPageTemplateDirectory;

        private string m_DashboardPageTemplateDirectory;

        public string BaseTemplateDirectory
        {
            get
            {
                if (this.m_sBaseTemplateDirectory == null)
                {
                    if (this.m_resourceLocation != WebPartTemplateResourceLocation.CustomFileLocation ||
                        string.IsNullOrEmpty(this.m_sCustomTemplatePath))
                    {
                        string str = null;
                        string sharePointIsapiFolderFromRegistry = Utils.GetSharePointIsapiFolderFromRegistry();
                        str = sharePointIsapiFolderFromRegistry.Trim(new char[] { '\\' });
                        int num = str.LastIndexOf("\\", StringComparison.Ordinal);
                        str = str.Substring(0, num);
                        str = string.Concat(str, "\\TEMPLATE\\");
                        this.m_sBaseTemplateDirectory = str;
                    }
                    else
                    {
                        this.m_sBaseTemplateDirectory = this.m_sCustomTemplatePath;
                    }
                }

                return this.m_sBaseTemplateDirectory;
            }
        }

        private string DashboardPageTemplateDirectory
        {
            get
            {
                if (this.m_DashboardPageTemplateDirectory == null)
                {
                    this.m_DashboardPageTemplateDirectory =
                        string.Concat(this.BaseTemplateDirectory, "FEATURES\\BizAppsCTypes");
                }

                return this.m_DashboardPageTemplateDirectory;
            }
        }

        public WebPartTemplateResourceLocation ResourceLocation
        {
            get { return this.m_resourceLocation; }
        }

        public Metalogix.SharePoint.Adapters.SharePointVersion SharePointVersion
        {
            get { return this.m_SharePointVersion; }
        }

        private string WebPartPageTemplateDirectory
        {
            get
            {
                if (this.m_WebPartPageTemplateDirectory == null)
                {
                    string baseTemplateDirectory = this.BaseTemplateDirectory;
                    bool flag = false;
                    int[] mPossibleLanguages = this.m_possibleLanguages;
                    for (int i = 0; i < (int)mPossibleLanguages.Length; i++)
                    {
                        int num = mPossibleLanguages[i];
                        if ((new DirectoryInfo(string.Concat(baseTemplateDirectory, num, "\\STS\\DOCTEMP\\SMARTPGS")))
                            .Exists)
                        {
                            baseTemplateDirectory =
                                string.Concat(baseTemplateDirectory, num, "\\STS\\DOCTEMP\\SMARTPGS");
                            flag = true;
                        }
                    }

                    if (!flag)
                    {
                        baseTemplateDirectory = string.Concat(baseTemplateDirectory, "1033\\STS\\DOCTEMP\\SMARTPGS");
                    }

                    this.m_possibleLanguages = null;
                    this.m_WebPartPageTemplateDirectory = baseTemplateDirectory;
                }

                return this.m_WebPartPageTemplateDirectory;
            }
        }

        private string WikiPageTemplateDirectory
        {
            get
            {
                if (this.m_WikiPageTemplateDirectory == null)
                {
                    this.m_WikiPageTemplateDirectory = string.Concat(this.BaseTemplateDirectory, "DOCUMENTTEMPLATES");
                }

                return this.m_WikiPageTemplateDirectory;
            }
        }

        public WebPartTemplateResourceManager(WebPartTemplateResourceLocation location,
            Metalogix.SharePoint.Adapters.SharePointVersion sharePointVersion, string sCustomTemplatePath)
        {
            this.m_SharePointVersion = sharePointVersion;
            this.m_resourceLocation = location;
            this.m_sCustomTemplatePath = sCustomTemplatePath;
        }

        public WebPartTemplateResourceManager(WebPartTemplateResourceLocation location, List<int> languages,
            Metalogix.SharePoint.Adapters.SharePointVersion sharePointVersion, string sCustomTemplatePath)
        {
            this.m_SharePointVersion = sharePointVersion;
            this.m_resourceLocation = location;
            this.m_possibleLanguages = languages.ToArray();
            this.m_sCustomTemplatePath = sCustomTemplatePath;
        }

        public byte[] GetDashboardTemplate(int iTemplateId)
        {
            if (this.SharePointVersion.IsSharePoint2003)
            {
                throw new Exception(
                    "Cannot retrieve dashboard template. SharePoint 2003 does not contain Report Libraries");
            }

            if (iTemplateId < 0 || iTemplateId > 2)
            {
                throw new Exception(string.Concat(
                    "An invalid dashboard page template ID is being used to create a new dashboard page. Invalid id = ",
                    iTemplateId.ToString(), "."));
            }

            string str = "";
            str = (iTemplateId != 0 ? string.Concat(iTemplateId.ToString(), "CV") : "H");
            byte[] numArray = null;
            numArray = (this.ResourceLocation != WebPartTemplateResourceLocation.EmbeddedWithinAssembly
                ? this.GetDashboardTemplateFromSharepointDirectories(str)
                : this.GetDashboardTemplateFromAssembly(str));
            return numArray;
        }

        private byte[] GetDashboardTemplateFromAssembly(string sTemplateId)
        {
            return this.GetTemplateFromAssembly("Features.BizAppsCTypes",
                string.Format("DashboardTemplate{0}.aspx", sTemplateId));
        }

        private byte[] GetDashboardTemplateFromSharepointDirectories(string sTemplateId)
        {
            string str = string.Concat(this.DashboardPageTemplateDirectory, "\\",
                string.Format("DashboardTemplate{0}.aspx", sTemplateId));
            return this.GetTemplateFromSharepointDirectories(str);
        }

        public byte[] GetTemplate(int iTemplateId)
        {
            if (iTemplateId < 1 || iTemplateId > 8)
            {
                throw new Exception(string.Concat(
                    "An invalid web part page template ID is being used to create a new web part page. Invalid id = ",
                    iTemplateId.ToString(), "."));
            }

            byte[] numArray = null;
            numArray = (this.ResourceLocation != WebPartTemplateResourceLocation.EmbeddedWithinAssembly
                ? this.GetTemplateFromSharepointDirectories(iTemplateId)
                : this.GetTemplateFromAssembly(iTemplateId));
            return numArray;
        }

        private byte[] GetTemplateFromAssembly(string sFolderPath, string sTemplateFileName)
        {
            string str;
            byte[] numArray;
            if (this.SharePointVersion.IsSharePoint2003)
            {
                str = "60.";
            }
            else if (this.SharePointVersion.IsSharePoint2007)
            {
                str = "12.Template.";
            }
            else if (!this.SharePointVersion.IsSharePoint2010)
            {
                if (!this.SharePointVersion.IsSharePoint2013OrLater)
                {
                    throw new Exception("No embedded template data for the current SharePoint version.");
                }

                str = "15.Template.";
            }
            else
            {
                str = "14.Template.";
            }

            string str1 = string.Concat("Metalogix.SharePoint.Adapters.WebPartTemplates.Templates._", str, sFolderPath);
            str1 = string.Concat(str1, ".", sTemplateFileName);
            Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(str1);
            if (manifestResourceStream == null)
            {
                throw new Exception(string.Concat("The resource: '", str1, "' did not exist within the assembly: ",
                    Assembly.GetExecutingAssembly().Location));
            }

            using (BinaryReader binaryReader = new BinaryReader(manifestResourceStream))
            {
                numArray = binaryReader.ReadBytes((int)manifestResourceStream.Length);
            }

            return numArray;
        }

        private byte[] GetTemplateFromAssembly(int iTemplateId)
        {
            string str = string.Format("spstd{0}.aspx", iTemplateId.ToString());
            return this.GetTemplateFromAssembly("STS.DOCTEMP.SMARTPGS", str);
        }

        private byte[] GetTemplateFromSharepointDirectories(string sTemplateFilePath)
        {
            if (File.Exists(sTemplateFilePath))
            {
                FileStream fileStream = new FileStream(sTemplateFilePath, FileMode.Open, FileAccess.Read);
                byte[] array = new byte[fileStream.Length];
                fileStream.Read(array, 0, (int)fileStream.Length);
                return array;
            }

            throw new Exception("Could not find the web part page template file '" + sTemplateFilePath + "'.");
        }

        private byte[] GetTemplateFromSharepointDirectories(int iTemplateId)
        {
            string str = string.Concat(this.WebPartPageTemplateDirectory, "\\",
                string.Format("spstd{0}.aspx", iTemplateId.ToString()));
            return this.GetTemplateFromSharepointDirectories(str);
        }

        public byte[] GetWikiTemplate()
        {
            if (this.SharePointVersion.IsSharePoint2003)
            {
                throw new Exception("Cannot retrieve wiki template. SharePoint 2003 does not contain wiki libraries");
            }

            byte[] numArray = null;
            numArray = (this.ResourceLocation != WebPartTemplateResourceLocation.EmbeddedWithinAssembly
                ? this.GetWikiTemplateFromSharepointDirectories()
                : this.GetWikiTemplateFromAssembly());
            return numArray;
        }

        private byte[] GetWikiTemplateFromAssembly()
        {
            return this.GetTemplateFromAssembly("DocumentTemplates", "wkpstd.aspx");
        }

        private byte[] GetWikiTemplateFromSharepointDirectories()
        {
            string str = string.Concat(this.WikiPageTemplateDirectory, "\\wkpstd.aspx");
            return this.GetTemplateFromSharepointDirectories(str);
        }
    }
}