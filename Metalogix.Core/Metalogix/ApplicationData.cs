using Metalogix.Core;
using Metalogix.Core.Properties;
using Metalogix.Utilities;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Metalogix
{
    public class ApplicationData
    {
        public static bool bStylesEnabled;

        public static string s_sCompanyFolderName;

        private static Assembly s_mainAssembly;

        private static string s_sApplicationDataFolder;

        private static string s_sCommonDataFolder;

        private static string s_companyPath;

        private static string s_sUserApplicationPath;

        private static string s_commonDataPath;

        private static string s_commonApplicationDataPath;

        private static string s_commonApplicationPath;

        private static bool? m_bIsWeb;

        public static string ApplicationDataFolder
        {
            get
            {
                if (!string.IsNullOrEmpty(ApplicationData.s_sApplicationDataFolder))
                {
                    return ApplicationData.s_sApplicationDataFolder;
                }

                string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                if (string.IsNullOrEmpty(folderPath))
                {
                    folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    if (string.IsNullOrEmpty(folderPath))
                    {
                        folderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                    }
                }

                if (string.IsNullOrEmpty(folderPath))
                {
                    throw new ArgumentNullException("ApplicationDataFolder",
                        Resources.CouldNotFindConfigurationVariablesPath);
                }

                ApplicationData.s_sApplicationDataFolder = folderPath;
                return ApplicationData.s_sApplicationDataFolder;
            }
            set { ApplicationData.s_sApplicationDataFolder = value; }
        }

        public static string ApplicationPath
        {
            get
            {
                if (!string.IsNullOrEmpty(ApplicationData.s_sUserApplicationPath))
                {
                    return ApplicationData.s_sUserApplicationPath;
                }

                string productName = null;
                string companyPath = ApplicationData.CompanyPath;
                if (ApplicationData.MainAssembly == null)
                {
                    if (!ApplicationData.IsDesignMode())
                    {
                        throw new ApplicationException("Main assembly could not be found.");
                    }

                    return Path.GetTempPath();
                }

                productName = ApplicationData.GetProductName();
                if (string.IsNullOrEmpty(productName))
                {
                    throw new InvalidOperationException("Can't find application path!");
                }

                companyPath = Path.Combine(companyPath, productName);
                DirectoryInfo directoryInfo = new DirectoryInfo(companyPath);
                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }

                directoryInfo = null;
                ApplicationData.s_sUserApplicationPath = string.Concat(companyPath, "\\");
                return ApplicationData.s_sUserApplicationPath;
            }
        }

        public static string CommonApplicationDataPath
        {
            get
            {
                if (ApplicationData.s_commonApplicationDataPath == null)
                {
                    string commonDataPath = ApplicationData.CommonDataPath;
                    string productName = ApplicationData.GetProductName();
                    if (string.IsNullOrEmpty(productName))
                    {
                        throw new InvalidOperationException("Can't find application path!");
                    }

                    commonDataPath = Path.Combine(commonDataPath, productName);
                    DirectoryInfo directoryInfo = new DirectoryInfo(commonDataPath);
                    if (!directoryInfo.Exists)
                    {
                        directoryInfo.Create();
                    }

                    ApplicationData.s_commonApplicationDataPath = commonDataPath;
                }

                return ApplicationData.s_commonApplicationDataPath;
            }
        }

        public static string CommonApplicationPath
        {
            get
            {
                if (ApplicationData.s_commonApplicationPath == null)
                {
                    string environmentVariable = Environment.GetEnvironmentVariable("CommonProgramFiles(x86)");
                    if (string.IsNullOrEmpty(environmentVariable))
                    {
                        environmentVariable = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);
                    }

                    string str = Path.Combine(environmentVariable, ApplicationData.CompanyFolderName);
                    if (!Directory.Exists(str))
                    {
                        try
                        {
                            Directory.CreateDirectory(str);
                        }
                        catch (Exception exception)
                        {
                        }
                    }

                    ApplicationData.s_commonApplicationPath = str;
                }

                return ApplicationData.s_commonApplicationPath;
            }
        }

        public static string CommonDataFolder
        {
            get
            {
                if (!string.IsNullOrEmpty(ApplicationData.s_sCommonDataFolder))
                {
                    return ApplicationData.s_sCommonDataFolder;
                }

                return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            }
            set { ApplicationData.s_sCommonDataFolder = value; }
        }

        public static string CommonDataPath
        {
            get
            {
                if (ApplicationData.s_commonDataPath != null)
                {
                    return ApplicationData.s_commonDataPath;
                }

                string commonDataFolder = ApplicationData.CommonDataFolder;
                if (!string.IsNullOrEmpty(ApplicationData.CompanyFolderName))
                {
                    commonDataFolder = Path.Combine(commonDataFolder, ApplicationData.CompanyFolderName);
                }

                commonDataFolder = string.Concat(commonDataFolder, "\\");
                if (!Directory.Exists(commonDataFolder))
                {
                    Logging.CreateDirectory(commonDataFolder);
                }

                ApplicationData.s_commonDataPath = commonDataFolder;
                return ApplicationData.s_commonDataPath;
            }
            set { ApplicationData.s_commonDataPath = value; }
        }

        public static string CompanyFolderName
        {
            get { return ApplicationData.s_sCompanyFolderName ?? "Metalogix"; }
            set { ApplicationData.s_sCompanyFolderName = value; }
        }

        public static string CompanyPath
        {
            get
            {
                if (ApplicationData.s_companyPath == null)
                {
                    string applicationDataFolder = ApplicationData.ApplicationDataFolder;
                    applicationDataFolder = Path.Combine(applicationDataFolder, ApplicationData.CompanyFolderName);
                    DirectoryInfo directoryInfo = new DirectoryInfo(applicationDataFolder);
                    if (!directoryInfo.Exists)
                    {
                        directoryInfo.Create();
                    }

                    ApplicationData.s_companyPath = applicationDataFolder;
                }

                return ApplicationData.s_companyPath;
            }
        }

        public static bool IsSharePointEdition { get; set; }

        public static bool IsWeb
        {
            get { return ApplicationData.m_bIsWeb.GetValueOrDefault(); }
            set { ApplicationData.m_bIsWeb = new bool?(value); }
        }

        public static Assembly MainAssembly
        {
            get
            {
                if (ApplicationData.IsDesignMode())
                {
                    return null;
                }

                if (ApplicationData.s_mainAssembly == null)
                {
                    try
                    {
                        Assembly entryAssembly = Assembly.GetEntryAssembly();
                        if (entryAssembly != null && ApplicationData.GetSignature(entryAssembly.FullName) ==
                            ApplicationData.GetSignature(Assembly.GetExecutingAssembly().FullName))
                        {
                            ApplicationData.s_mainAssembly = entryAssembly;
                        }
                    }
                    catch
                    {
                    }

                    if (ApplicationData.s_mainAssembly == null)
                    {
                        DirectoryInfo parent = (new DirectoryInfo(Assembly.GetExecutingAssembly().Location)).Parent;
                        FileInfo[] files = parent.GetFiles("*.exe");
                        for (int i = 0; i < (int)files.Length; i++)
                        {
                            FileInfo fileInfo = files[i];
                            try
                            {
                                Assembly assembly = Assembly.LoadFrom(fileInfo.FullName);
                                if (ApplicationData.GetSignature(assembly.FullName) ==
                                    ApplicationData.GetSignature(Assembly.GetExecutingAssembly().FullName))
                                {
                                    ApplicationData.s_mainAssembly = assembly;
                                    break;
                                }
                            }
                            catch
                            {
                            }
                        }

                        if (ApplicationData.s_mainAssembly == null)
                        {
                            throw new Exception("Failed to find the main application assembly.");
                        }
                    }
                }

                return ApplicationData.s_mainAssembly;
            }
            set
            {
                if (value != null && ApplicationData.GetSignature(value.FullName) !=
                    ApplicationData.GetSignature(Assembly.GetExecutingAssembly().FullName))
                {
                    throw new ArgumentException("Specified executable is not a Metalogix assembly");
                }

                ApplicationData.s_mainAssembly = value;
                ApplicationData.s_sApplicationDataFolder = null;
                ApplicationData.s_sCommonDataFolder = null;
                ApplicationData.s_sUserApplicationPath = null;
                ApplicationData.s_commonApplicationDataPath = null;
                ApplicationData.FireMainAssemblyChanged();
            }
        }

        static ApplicationData()
        {
            ApplicationData.bStylesEnabled = false;
            ApplicationData.s_sCompanyFolderName = null;
            ApplicationData.s_mainAssembly = null;
            ApplicationData.s_sApplicationDataFolder = null;
            ApplicationData.s_sCommonDataFolder = null;
            ApplicationData.s_companyPath = null;
            ApplicationData.s_sUserApplicationPath = null;
            ApplicationData.s_commonApplicationDataPath = null;
            ApplicationData.s_commonApplicationPath = null;
        }

        public ApplicationData()
        {
        }

        private static void FireMainAssemblyChanged()
        {
            if (ApplicationData.MainAssemblyChanged != null)
            {
                ApplicationData.MainAssemblyChanged(ApplicationData.s_mainAssembly, new EventArgs());
            }
        }

        public static string GetProductName()
        {
            string product = null;
            try
            {
                Attribute customAttribute =
                    Attribute.GetCustomAttribute(ApplicationData.MainAssembly, typeof(AssemblyProductAttribute));
                product = ((AssemblyProductAttribute)customAttribute).Product;
            }
            catch (Exception exception)
            {
            }

            return product;
        }

        [Obsolete("Please use Metalogix.Utilities.ReflectionUtils instead.")]
        public static string GetSignature(string sAssemblyFullName)
        {
            return ReflectionUtils.GetSignature(sAssemblyFullName);
        }

        public static bool IsDesignMode()
        {
            return false;
        }

        [Obsolete("Please use Metalogix.Utilities.ReflectionUtils instead.")]
        public static bool IsMetalogixAssembly(Assembly assembly)
        {
            return ReflectionUtils.IsMetalogixAssembly(assembly.FullName);
        }

        [Obsolete("Please use Metalogix.Utilities.ReflectionUtils instead.")]
        public static bool IsMetalogixAssembly(string sAssemblyFullName)
        {
            return ReflectionUtils.IsMetalogixAssembly(sAssemblyFullName);
        }

        public static event EventHandler MainAssemblyChanged;
    }
}