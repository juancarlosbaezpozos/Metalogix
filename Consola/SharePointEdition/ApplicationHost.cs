using Metalogix.Actions;
using Metalogix.Core;
using Metalogix.DataResolution;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Administration;
using Metalogix.SharePoint.Adapters.CSOM2013Client;
using Metalogix.SharePoint.Adapters.DB;
using Metalogix.SharePoint.Options;
using Metalogix.SharePoint.UI.WinForms;
using Metalogix.UI.CommandLine;
using Metalogix.UI.WinForms;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Metalogix.SharePointEdition
{
    public static class ApplicationHost
    {
        private const string C_ErrorHandlerCaption = "Metalogix Exception Handler";

        private const string C_CrashLogFile = "CrashLog.txt";

        private const string C_NonCriticalLogFile = "NonCriticalError.txt";

        private const string C_LoggingError = "Error occured in logging an error to \"{0}\"";

        private const string C_LoggingErrorCaption = "Saving {0} Error";

        private static UISplashForm s_initLoadingDialog;

        private static volatile bool s_bLoadingDialogClosed;

        private static readonly object LOADING_DIALOG_LOCK;

        public static Type[] TypeReferences
        {
            get
            {
                return new Type[]
                {
                    typeof(SPNode), typeof(ConnectToSharePoint), typeof(SharePointActionOptions),
                    typeof(ScopableTabbableControl), typeof(DBAdapter)
                };
            }
        }

        static ApplicationHost()
        {
            s_initLoadingDialog = null;
            s_bLoadingDialogClosed = false;
            LOADING_DIALOG_LOCK = new object();
        }

        private static void ActivateMainForm()
        {
            if (!UIApplication.INSTANCE.MainForm.InvokeRequired)
            {
                UIApplication.INSTANCE.MainForm.Activate();
                return;
            }

            SplashDialogDelegate splashDialogDelegate = new SplashDialogDelegate(ActivateMainForm);
            UIApplication.INSTANCE.MainForm.Invoke(splashDialogDelegate);
        }

        private static void ApplicationThreadException(object sender, ThreadExceptionEventArgs t)
        {
            HandleNonCriticalException(t.Exception);
        }

        private static void CloseSplashDialog()
        {
            if (!s_bLoadingDialogClosed)
            {
                if (s_initLoadingDialog != null)
                {
                    if (s_initLoadingDialog.InvokeRequired)
                    {
                        try
                        {
                            s_initLoadingDialog.Invoke(new SplashDialogDelegate(CloseSplashDialog));
                        }
                        catch (InvalidAsynchronousStateException invalidAsynchronousStateException)
                        {
                        }

                        return;
                    }

                    lock (LOADING_DIALOG_LOCK)
                    {
                        s_bLoadingDialogClosed = true;
                        try
                        {
                            s_initLoadingDialog.BringToFront();
                            s_initLoadingDialog.RequestClose();
                        }
                        catch (Exception exception)
                        {
                        }
                    }
                }

                s_bLoadingDialogClosed = true;
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (!e.IsTerminating)
            {
                HandleNonCriticalException(e.ExceptionObject as Exception);
            }
            else
            {
                try
                {
                    HandleTerminatingException(e.ExceptionObject as Exception);
                }
                finally
                {
                    Environment.Exit(1);
                }
            }
        }

        private static void DisplaySplashDialog()
        {
            lock (LOADING_DIALOG_LOCK)
            {
                if (!s_bLoadingDialogClosed)
                {
                    s_initLoadingDialog = UIApplication.INSTANCE.NewSplashScreen();
                }
                else
                {
                    return;
                }
            }

            if (!s_bLoadingDialogClosed)
            {
                s_initLoadingDialog.FormClosed +=
                    new FormClosedEventHandler((object sender, FormClosedEventArgs e) => ActivateMainForm());
                s_initLoadingDialog.ShowDialog();
                return;
            }
        }

        [DllImport("dwmapi.dll", CharSet = CharSet.None, ExactSpelling = false, PreserveSig = false)]
        private static extern bool DwmIsCompositionEnabled();

        private static byte[] GetExceptionLoggingInBytes(Exception ex)
        {
            return Encoding.Unicode.GetBytes(GetExceptionLoggingString(ex));
        }

        private static string GetExceptionLoggingString(Exception ex)
        {
            string str;
            using (StringWriter stringWriter = new StringWriter())
            {
                DateTime now = DateTime.Now;
                stringWriter.WriteLine(string.Format("{0} : {1} has encountered an error.", now.ToString(),
                    Application.ProductName));
                stringWriter.WriteLine(string.Empty);
                GlobalServices.ErrorFormatter.FormatException(ex, stringWriter);
                str = stringWriter.ToString();
            }

            return str;
        }

        private static void HandleErrorDisplay(string caption, string msg, Exception ex)
        {
            if (!CommandLineHandler.IsCommandLineControl())
            {
                GlobalServices.ErrorHandler.HandleException(caption, msg, ex, ErrorIcon.Error);
                return;
            }

            Console.WriteLine("*** APPLICATION ERROR ***");
            Console.WriteLine(string.Format("{0} : {1} ({2})", caption, msg, ex.Message));
        }

        private static void HandleNonCriticalException(Exception ex)
        {
            string text = Path.Combine(ApplicationData.ApplicationPath, "NonCriticalError.txt");
            try
            {
                using (FileStream fileStream = new FileStream(text, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    byte[] exceptionLoggingInBytes = GetExceptionLoggingInBytes(ex);
                    byte[] array = new byte[(long)exceptionLoggingInBytes.Length + fileStream.Length];
                    exceptionLoggingInBytes.CopyTo(array, 0);
                    fileStream.Read(array, exceptionLoggingInBytes.Length, (int)fileStream.Length);
                    fileStream.Seek(0L, SeekOrigin.Begin);
                    fileStream.Write(array, 0, array.Length);
                    fileStream.Flush();
                    fileStream.Close();
                }
            }
            catch (Exception ex2)
            {
                HandleErrorDisplay(string.Format("Saving {0} Error", "NonCriticalError.txt"),
                    string.Format("Error occured in logging an error to \"{0}\"", text), ex2);
            }

            HandleErrorDisplay("Metalogix Exception Handler",
                string.Format("A non-critical error has occured.{0}A log of this error can be found at : \"{1}\"",
                    Environment.NewLine, text), ex);
        }

        private static void HandleTerminatingException(Exception ex)
        {
            string str = Path.Combine(ApplicationData.ApplicationPath, "CrashLog.txt");
            try
            {
                using (FileStream fileStream = new FileStream(str, FileMode.Create))
                {
                    byte[] exceptionLoggingInBytes = GetExceptionLoggingInBytes(ex);
                    fileStream.Write(exceptionLoggingInBytes, 0, (int)exceptionLoggingInBytes.Length);
                    fileStream.Flush();
                    fileStream.Close();
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                HandleErrorDisplay(string.Format("Saving {0} Error", "CrashLog.txt"),
                    string.Format("Error occured in logging an error to \"{0}\"", str), exception);
            }

            HandleErrorDisplay("Metalogix Exception Handler",
                string.Format(
                    "An error has caused application to exit abnormally.{0}A log of this error can be found at : \"{1}\"",
                    Environment.NewLine, str), ex);
        }

        public static bool HasWritePermissionOnDir(string folderPath, out Exception exPermission)
        {
            bool flag;
            try
            {
                exPermission = null;
                Guid guid = Guid.NewGuid();
                string str = Path.Combine(folderPath, string.Concat(guid.ToString("N"), ".txt"));
                File.AppendAllText(str, "Write Test");
                File.Delete(str);
                flag = true;
            }
            catch (Exception exception)
            {
                exPermission = exception;
                flag = false;
            }

            return flag;
        }

        internal static void Run()
        {
            try
            {
                ApplicationData.IsSharePointEdition = true;
                Application.ThreadException += new ThreadExceptionEventHandler(ApplicationThreadException);
                AppDomain.CurrentDomain.UnhandledException +=
                    new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                if (!CommandLineHandler.IsCommandLineControl())
                {
                    try
                    {
                        if (DwmIsCompositionEnabled())
                        {
                            Application.EnableVisualStyles();
                            Application.SetCompatibleTextRenderingDefault(false);
                        }
                    }
                    catch (Exception exception)
                    {
                    }

                    if (FileDirUtils.HasWritePermission())
                    {
                        ThreadStart threadStart = new ThreadStart(new SplashDialogDelegate(DisplaySplashDialog).Invoke);
                        (new Thread(threadStart)).Start();
                        LicenseProviderInitializationData licenseProviderInitializationDataFromAssembly =
                            LicenseProviderInitializationData.GetLicenseProviderInitializationDataFromAssembly(
                                ApplicationData.MainAssembly);
                        MLLicenseProvider.TryInitialize(licenseProviderInitializationDataFromAssembly);
                        Telemetry.Initialize();
                        if (UIApplication.INSTANCE.GetLicense() != null)
                        {
                            try
                            {
                                ActionOptionsProvider.DefaultsProvider = new ActionOptionsProvider(
                                    new FolderDataResolver(ActionOptionsProvider.DefaultActionDefaultsFolderPath));
                                ActionOptionsProvider.UpgradeOldDefaultOptionsFile();
                            }
                            catch (Exception exception1)
                            {
                                GlobalServices.ErrorHandler.HandleException(exception1);
                            }

                            try
                            {
                                DataResolver folderDataResolver;
                                if (string.IsNullOrEmpty(ActionConfigurationVariables.JobTemplateResolver))
                                {
                                    folderDataResolver =
                                        new FolderDataResolver(ActionOptionsProvider.DefaultActionTemplatesFolderPath);
                                    ActionConfigurationVariables.JobTemplateResolver = folderDataResolver.ToXML();
                                }
                                else
                                {
                                    folderDataResolver =
                                        DataResolver.CreateDataResolver(
                                            ActionConfigurationVariables.JobTemplateResolver);
                                }

                                ActionOptionsProvider.TemplatesProvider = new ActionOptionsProvider(folderDataResolver);
                            }
                            catch (Exception exception2)
                            {
                                GlobalServices.ErrorHandler.HandleException(exception2);
                            }

                            try
                            {
                                CSOMClientAdapter.EnsureServiceEndpointIsRunningForApplication();
                            }
                            catch (Exception exception3)
                            {
                                GlobalServices.ErrorHandler.HandleException(exception3);
                            }

                            UIApplication.INSTANCE.MainForm.StartPosition = FormStartPosition.CenterScreen;
                            UIApplication.INSTANCE.MainForm.Shown +=
                                new EventHandler((object sender, EventArgs e) => CloseSplashDialog());
                            UIApplication.INSTANCE.MainForm.Opacity = 0;
                            Application.Run(UIApplication.INSTANCE.MainForm);
                            Telemetry.TearDown();
                        }
                        else
                        {
                            CloseSplashDialog();
                        }
                    }
                }
                else
                {
                    MLLicenseProvider.Initialize(
                        LicenseProviderInitializationData.GetLicenseProviderInitializationDataFromAssembly(
                            ApplicationData.MainAssembly));
                    Telemetry.Initialize();
                    if (UIApplication.INSTANCE.GetLicense() != null)
                    {
                        MainWindowWrapper mainWindowWrapper = new MainWindowWrapper();
                        CommandLineHandler.Handle();
                        Telemetry.TearDown();
                    }
                }
            }
            catch (Exception exception4)
            {
                HandleTerminatingException(exception4);
            }
        }

        private delegate void SplashDialogDelegate();
    }
}