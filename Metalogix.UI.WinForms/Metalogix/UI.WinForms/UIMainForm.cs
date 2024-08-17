using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.UserSkins;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using Metalogix.Actions;
using Metalogix.Actions.Remoting;
using Metalogix.Core;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.Jobs;
using Metalogix.Licensing;
using Metalogix.Licensing.Common;
using Metalogix.MLLicensing.Properties;
using Metalogix.Telemetry.Accumulators;
using Metalogix.UI.WinForms.Actions;
using Metalogix.UI.WinForms.Attributes;
using Metalogix.UI.WinForms.Deployment;
using Metalogix.UI.WinForms.Documentation;
using Metalogix.UI.WinForms.Explorer;
using Metalogix.UI.WinForms.Interfaces;
using Metalogix.UI.WinForms.Jobs;
using Metalogix.UI.WinForms.Properties;
using Metalogix.UI.WinForms.RemotePowerShell.Actions;
using Metalogix.UI.WinForms.RemotePowerShell.Wizard.Actions;
using Metalogix.UI.WinForms.Support;
using Metalogix.UI.WinForms.Tooltips;
using Metalogix.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace Metalogix.UI.WinForms
{
	[System.ComponentModel.LicenseProvider(typeof(MLLicenseProvider))]
	public partial class UIMainForm : RibbonForm
	{
		public delegate void ApplicationSettingDelegate(object sender, System.EventArgs e);

		private delegate void FadeDelegate();

		private bool _requiresActionContextRebuild;

		private UIApplication m_Application;

		private ApplicationControl m_LeftApplicationControl;

		private ApplicationControl m_RightApplicationControl;

		private System.ComponentModel.IContainer components;

		private SplitContainer splitContainerH;

		protected SplitContainer splitContainerV;

		private JobListFullControl w_jobListFullControl;

		private ImageCollection imageCollectionSmall;

		private RibbonControl ribbon;

		private BarButtonItem barButtonItemRefresh;

		private BarButtonItem barButtonItemAgentWizard;

		private BarButtonItem barButtonItemAgent;

		private BarButtonItem barButtonItemJobQueue;

		private BarCheckItem barCheckItemSecondExplorer;

		private BarCheckItem barCheckItemShowJobsList;

		private BarCheckItem barCheckItemShowCheckBoxes;

		private BarCheckItem barCheckItemEnableSticky;

		private BarButtonItem barButtonItemResetSticky;

		private BarButtonItem barButtonItemFacebook;

		private BarButtonItem barButtonItemTwitter;

		private BarButtonItem barButtonItemBlog;

		private BarButtonItem barButtonItemMetalogixAcademy;

		private BarButtonItem barButtonItemSupportForum;

		private BarButtonItem barButtonItemContactSupport;

		private BarButtonItem barButtonItemHelpTopics;

		private BarButtonItem barButtonItemAbout;

		private BarButtonItem barButtonItemUpdateCheck;

		private BarButtonItem barButtonItemUpdateLicence;

		private ImageCollection imageCollectionLarge;

		private RibbonPage ribbonPageConnection;

		private RibbonPageGroup ribbonPageGroupConnect;

		private RibbonPageGroup ribbonPageGroupRefresh;

		private RibbonPageGroup ribbonPageGroupAgent;

		private RibbonPage ribbonPageView;

		private RibbonPageGroup ribbonPageGroupViewShowViews;

		private RibbonPageGroup ribbonPageGroupViewListOptions;

		private RibbonPage ribbonPageSettings;

		private RibbonPageGroup ribbonPageGroupPreserve;

		private RibbonPageGroup ribbonPageGroupGeneralSettings;

		private RibbonPageGroup ribbonPageGroupSocial;

		private RibbonPageGroup ribbonPageGroupSupport;

		private RibbonPage ribbonPageHelp;

		private RibbonPageGroup ribbonPageGroupHelp;

		private RibbonPageGroup ribbonPageGroupUpgrade;

		private BarButtonItem barButtonItemLinkedIn;

		private ApplicationMenu applicationMenu1;

		private DefaultLookAndFeel DefaultLookAndFeel;

		private BarButtonItem barButtonItemReplicator;

		private BarButtonItem barButtonItemStoragePoint;

		private RibbonPageGroup ribbonPageGroupProducts;

		private BarButtonItem barButtonManageActionTemplates;

		private DefaultToolTipController _defaultToolTipController;

		private BarButtonItem barButtonItemCreateSupportPackage;

		private Control FocusedContextMenuContainer
		{
			get
			{
				Control control = null;
				if (this.LeftApplicationControl != null)
				{
					control = this.LeftApplicationControl.FocusedSelectableContainer;
				}
				if (control == null && this.RightApplicationControl != null)
				{
					control = this.RightApplicationControl.FocusedSelectableContainer;
				}
				return control ?? this.w_jobListFullControl;
			}
		}

		[System.ComponentModel.Browsable(false)]
		public UIApplication Application
		{
			get
			{
				return this.m_Application;
			}
			set
			{
				this.m_Application = value;
				if (this.Application == null)
				{
					return;
				}
				this.RefreshApplicationControls();
			}
		}

		public ActionPaletteControl ActionPaletteControl
		{
			get;
			private set;
		}

		public ApplicationControl LeftApplicationControl
		{
			get
			{
				return this.m_LeftApplicationControl;
			}
			set
			{
				this.m_LeftApplicationControl = value;
				if (this.LeftApplicationControl == null)
				{
					return;
				}
				this.LeftApplicationControl.Dock = DockStyle.Fill;
				this.LeftApplicationControl.ContextMenuStrip = this.ActionPaletteControl;
				this.LeftApplicationControl.OnSelectedNodeChanged += delegate(Node node)
				{
					this._requiresActionContextRebuild = true;
				};
				this.splitContainerV.Panel1.Controls.Clear();
				this.splitContainerV.Panel1.Controls.Add(this.LeftApplicationControl);
			}
		}

		public ApplicationControl RightApplicationControl
		{
			get
			{
				return this.m_RightApplicationControl;
			}
			set
			{
				this.m_RightApplicationControl = value;
				if (this.RightApplicationControl == null)
				{
					return;
				}
				this.RightApplicationControl.Dock = DockStyle.Fill;
				this.RightApplicationControl.ContextMenuStrip = this.ActionPaletteControl;
				this.RightApplicationControl.OnSelectedNodeChanged += delegate(Node node)
				{
					this._requiresActionContextRebuild = true;
				};
				this.splitContainerV.Panel2.Controls.Clear();
				this.splitContainerV.Panel2.Controls.Add(this.RightApplicationControl);
			}
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (this._requiresActionContextRebuild)
			{
				this.RebuildActionContextMenu();
				this._requiresActionContextRebuild = false;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		public UIMainForm()
		{
			this.InitializeComponent();
			this.InitializeToolTips();
			bool enabled = this.w_jobListFullControl.DataSource != null && JobsSettings.AdapterType.Equals(JobHistoryAdapterType.Agent.ToString());
			this.barButtonItemAgent.Enabled = enabled;
			this.barButtonItemJobQueue.Enabled = enabled;
			SkinManager.Default.RegisterAssembly(typeof(MetalogixSkins).Assembly);
			SkinManager.EnableFormSkins();
			UserLookAndFeel.Default.SetSkinStyle("Metalogix 2017");
			MLLicenseProvider.LicenseUpdated += new System.EventHandler(this.OnLicenseUpdated);
			ConfigurationVariables.ConfigurationVariablesChanged += new ConfigurationVariables.ConfigurationVariablesChangedHander(this.OnConfigurationVariablesChanged);
			this.w_jobListFullControl.OnDataSourceChanged += new System.EventHandler(this.w_jobListFullControl_OnChildTextChanged);
		}

		public void InitializeToolTips()
		{
			UIMainForm.AddSuperTipToBarItem(this.barButtonItemRefresh, this.barButtonItemRefresh.Name);
			UIMainForm.AddSuperTipToBarItem(this.barButtonItemAgentWizard, this.barButtonItemAgentWizard.Name);
			UIMainForm.AddSuperTipToBarItem(this.barButtonItemAgent, this.barButtonItemAgent.Name);
			UIMainForm.AddSuperTipToBarItem(this.barButtonItemJobQueue, this.barButtonItemJobQueue.Name);
			UIMainForm.AddSuperTipToBarItem(this.barCheckItemSecondExplorer, this.barCheckItemSecondExplorer.Name);
			UIMainForm.AddSuperTipToBarItem(this.barCheckItemShowJobsList, this.barCheckItemShowJobsList.Name);
			UIMainForm.AddSuperTipToBarItem(this.barCheckItemShowCheckBoxes, this.barCheckItemShowCheckBoxes.Name);
			UIMainForm.AddSuperTipToBarItem(this.barCheckItemEnableSticky, this.barCheckItemEnableSticky.Name);
			UIMainForm.AddSuperTipToBarItem(this.barButtonItemResetSticky, this.barButtonItemResetSticky.Name);
			UIMainForm.AddSuperTipToBarItem(this.barButtonManageActionTemplates, this.barButtonManageActionTemplates.Name);
			UIMainForm.AddSuperTipToBarItem(this.barButtonItemFacebook, this.barButtonItemFacebook.Name);
			UIMainForm.AddSuperTipToBarItem(this.barButtonItemBlog, this.barButtonItemBlog.Name);
			UIMainForm.AddSuperTipToBarItem(this.barButtonItemMetalogixAcademy, this.barButtonItemMetalogixAcademy.Name);
			UIMainForm.AddSuperTipToBarItem(this.barButtonItemSupportForum, this.barButtonItemSupportForum.Name);
			UIMainForm.AddSuperTipToBarItem(this.barButtonItemContactSupport, this.barButtonItemContactSupport.Name);
			UIMainForm.AddSuperTipToBarItem(this.barButtonItemHelpTopics, this.barButtonItemHelpTopics.Name);
			UIMainForm.AddSuperTipToBarItem(this.barButtonItemAbout, this.barButtonItemAbout.Name);
			UIMainForm.AddSuperTipToBarItem(this.barButtonItemUpdateCheck, this.barButtonItemUpdateCheck.Name);
			UIMainForm.AddSuperTipToBarItem(this.barButtonItemUpdateLicence, this.barButtonItemUpdateLicence.Name);
			UIMainForm.AddSuperTipToBarItem(this.barButtonItemLinkedIn, this.barButtonItemLinkedIn.Name);
			UIMainForm.AddSuperTipToBarItem(this.barButtonItemReplicator, this.barButtonItemReplicator.Name);
			UIMainForm.AddSuperTipToBarItem(this.barButtonItemStoragePoint, this.barButtonItemStoragePoint.Name);
			UIMainForm.AddSuperTipToBarItem(this.barButtonItemTwitter, this.barButtonItemTwitter.Name);
		}

		public static void AddSuperTipToBarItem(BarItem item, string stringName)
		{
			string toolTip = TooltipManager.GetToolTip(stringName, null);
			if (toolTip != null)
			{
				SuperToolTip superToolTip = new SuperToolTip();
				ToolTipTitleItem toolTipTitleItem = new ToolTipTitleItem();
				toolTipTitleItem.Text = toolTip;
				superToolTip.Items.Add(toolTipTitleItem);
				item.SuperTip = superToolTip;
			}
		}

		public static void AddSuperTipToTypedBarItem(BarItem item, System.Type type)
		{
			string typedToolTip = TooltipManager.GetTypedToolTip(type);
			if (typedToolTip != null)
			{
				SuperToolTip superToolTip = new SuperToolTip();
				ToolTipTitleItem toolTipTitleItem = new ToolTipTitleItem();
				toolTipTitleItem.Text = typedToolTip;
				superToolTip.Items.Add(toolTipTitleItem);
				item.SuperTip = superToolTip;
			}
		}

		private void w_jobListFullControl_OnChildTextChanged(object sender, System.EventArgs e)
		{
			this.barButtonItemAgent.Enabled = (bool)sender;
			this.barButtonItemJobQueue.Enabled = (bool)sender;
		}

		public void SaveSettings()
		{
			try
			{
				SystemInfoTelemetry.SendTelemetry();
				base.SuspendLayout();
				UISettings.AppSettings.IsMaximized = (base.WindowState == FormWindowState.Maximized);
				UISettings.AppSettings.WindowSize = ((base.WindowState == FormWindowState.Maximized || base.WindowState == FormWindowState.Minimized) ? base.RestoreBounds.Size : base.Size);
				UISettings.AppSettings.ExplorerHeight = this.splitContainerH.SplitterDistance;
				UISettings.AppSettings.LeftExplorerWidth = this.splitContainerV.SplitterDistance;
				UISettings.AppSettings.FlushSettings();
			}
			finally
			{
				base.ResumeLayout();
			}
		}

		private void BarCheckItemShowCheckBoxesCheckedChanged(object sender, ItemClickEventArgs e)
		{
			UISettings.AppSettings.ShowExplorerCheckBoxes = this.barCheckItemShowCheckBoxes.Checked;
			if (this.LeftApplicationControl != null)
			{
				this.LeftApplicationControl.ShowExplorerCheckBoxes = UISettings.AppSettings.ShowExplorerCheckBoxes;
			}
			if (this.RightApplicationControl != null)
			{
				this.RightApplicationControl.ShowExplorerCheckBoxes = UISettings.AppSettings.ShowExplorerCheckBoxes;
			}
		}

		private void ViewOptionCheckedChanged(object sender, System.EventArgs e)
		{
			BarCheckItem barCheckItem = sender as BarCheckItem;
			if (barCheckItem != null)
			{
				System.Type type = (System.Type)barCheckItem.Tag;
				ExplorerViewOption explorerViewOption = (ExplorerViewOption)System.Activator.CreateInstance(type);
				explorerViewOption.StoreSetting(barCheckItem.Checked);
			}
			if (this.LeftApplicationControl != null)
			{
				this.LeftApplicationControl.Redraw();
			}
			if (this.RightApplicationControl != null)
			{
				this.RightApplicationControl.Redraw();
			}
		}

		private void UIMainFormShown(object sender, System.EventArgs e)
		{
			if (ApplicationData.IsDesignMode())
			{
				return;
			}
			System.Threading.ThreadStart start = new System.Threading.ThreadStart(this.FadeIn);
			System.Threading.Thread thread = new System.Threading.Thread(start);
			thread.Start();
		}


	    public void FadeIn()
	    {
	        try
	        {
	            if (!base.InvokeRequired)
	            {
	                int num = (UISettings.IsRemoteSession ? 1 : 10);
	                float single = 1f / (float)num;
	                while (num > 0)
	                {
	                    num--;
	                    UIMainForm opacity = this;
	                    opacity.Opacity = opacity.Opacity + (double)single;
	                    this.Refresh();
	                    Thread.Sleep(10);
	                }
	                if (this.w_jobListFullControl.ReLoadJobsList)
	                {
	                    this.w_jobListFullControl.HandleUnableToOpenJobList();
	                }
	                UIMarketingSplashForm.ShowSplashIfRequired(this);
	                //this.DisplayNag(true);
	                if (JobsSettings.AdapterType.Equals(JobHistoryAdapterType.Agent.ToString(), StringComparison.InvariantCultureIgnoreCase))
	                {
	                    (new Thread(() => this.AutoRefershAllAgents())).Start();
	                    this.DisplayQueuedJobsAlert();
	                }
	            }
	            else
	            {
	                base.Invoke(new UIMainForm.FadeDelegate(this.FadeIn));
	            }
	        }
	        catch (Exception exception)
	        {
	            GlobalServices.ErrorHandler.HandleException("Error", "An error occurred while loading application.", exception, ErrorIcon.Error);
	        }
	    }

        private void DisplayNag(bool bDisplayOfflineWarning = true)
		{
			MLLicense license = this.Application.GetLicense();
			if (license == null)
			{
				return;
			}
			if (license.LicenseType == MLLicenseType.Evaluation)
			{
				FlatXtraMessageBox.Show(this, this.Application.LicenseTrialWarningMessage, "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			}
			else if (license.LicenseType == MLLicenseType.Partner)
			{
				FlatXtraMessageBox.Show(this, Metalogix.UI.WinForms.Properties.Resources.UIMainForm_DisplayNag_You_are_using_a_Partner_License, Metalogix.UI.WinForms.Properties.Resources.UIMainForm_DisplayNag_Partner_License_Info, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			}
			MLLicenseCommon mLLicenseCommon = license as MLLicenseCommon;
			if (mLLicenseCommon == null)
			{
				return;
			}
			if (mLLicenseCommon.OfflineValidityDays <= 0)
			{
				FlatXtraMessageBox.Show(this, Metalogix.MLLicensing.Properties.Resources.License_Offline_Validation_Exceeded, Metalogix.UI.WinForms.Properties.Resources.UIMainForm_DisplayNag_License_Synchronization_Info, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			}
			else if (bDisplayOfflineWarning && mLLicenseCommon.OfflineValidityDays > 0 && mLLicenseCommon.OfflineValidityDays < 15)
			{
				FlatXtraMessageBox.Show(this, string.Format(Metalogix.UI.WinForms.Properties.Resources.UIMainForm_DisplayNag_Your_license_could_not_be_validated_Days_remaining, mLLicenseCommon.OfflineValidityDays, (mLLicenseCommon.OfflineValidityDays > 1) ? "s" : ""), Metalogix.UI.WinForms.Properties.Resources.UIMainForm_DisplayNag_License_Synchronization_Info, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			}
			this.DisplayAlert(mLLicenseCommon);
		}

		private void AutoRefershAllAgents()
		{
			try
			{
				RemoteJobScheduler.Instance.RefreshAllAgents();
			}
			catch (System.Exception exception)
			{
				Logging.LogExceptionToTextFileWithEventLogBackup(exception, "An error occurred while refreshing all agents.", true);
			}
		}

		private void DisplayQueuedJobsAlert()
		{
			try
			{
				RemoteJobScheduler.Instance.AddRunningJobs();
				System.Collections.Generic.List<Job> jobs = RemoteJobScheduler.Instance.GetJobs(ActionStatus.Queued);
				if (jobs.Count > 0)
				{
					string text = string.Format("There is/are {0} queued job(s) in the Agent Database. Would you like to start running the queued job(s) now? \r\n \r\n Note: If you select 'No', the job(s) will be removed from the queue.", jobs.Count);
					DialogResult dialogResult = FlatXtraMessageBox.Show(text, "Queued Jobs", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
					if (dialogResult == DialogResult.Yes)
					{
						if (!RemoteJobScheduler.Instance.IsJobRunning)
						{
							RemoteJobScheduler.Instance.Run();
						}
						RemoteJobScheduler.Instance.ProcessJobQueue(this, null);
						IXMLAbleList source = new CommonSerializableList<UIMainForm>
						{
							this
						};
						ActionPaletteControl.ActionClick(new ManageQueueAction(), source, null);
					}
					else if (dialogResult == DialogResult.No)
					{
						foreach (Job current in jobs)
						{
							RemoteJobScheduler.Instance.UpdateJobInfo(current.JobID, ActionStatus.NotRunning, null);
							string text2 = System.IO.Path.Combine(Utils.MetalogixTempPath, current.JobID + ".ps1");
							if (System.IO.File.Exists(text2))
							{
								Metalogix.Actions.Remoting.FileUtils.DeleteScript(text2);
							}
						}
					}
				}
			}
			catch (System.Exception exception)
			{
				Logging.LogExceptionToTextFileWithEventLogBackup(exception, "An error occurred while retrieving queued / running jobs.", true);
			}
		}

		private void DisplayAlert(MLLicenseCommon lic)
		{
			long num;
			double percentage;
			if (lic.IsLegacyProduct || lic.IsContentMatrixExpress)
			{
				num = lic.UsedDataFull;
				percentage = lic.UsagePercentage;
			}
			else
			{
				num = lic.UsedLicensedData;
				percentage = 100.0 * (double)num / (double)lic.LicensedData;
			}
			string text = "dd-MMM-yyyy";
			System.Collections.Generic.Dictionary<string, string> dictionary = new System.Collections.Generic.Dictionary<string, string>();
			System.DateTime value = System.DateTime.ParseExact(System.DateTime.UtcNow.ToString(text, System.Globalization.CultureInfo.InvariantCulture), text, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);
			System.DateTime licenseExpirationDate = System.DateTime.ParseExact(lic.ExpirationDate.ToString(text, System.Globalization.CultureInfo.InvariantCulture), text, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);
			System.DateTime maintenanceExpirationDate = System.DateTime.ParseExact(lic.MaintenanceExpirationDate.ToString(text, System.Globalization.CultureInfo.InvariantCulture), text, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);
			bool isDisplaySuppress = value.CompareTo(System.DateTime.ParseExact(UIConfigurationVariables.DateToShowLicenseAlert, text, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None)) > 0;
			int days = licenseExpirationDate.Subtract(value).Days;
			int days2 = maintenanceExpirationDate.Subtract(value).Days;
			this.CheckLicenseUsage(lic, percentage, num, isDisplaySuppress, dictionary);
			this.CheckLicenseExpiry(days, licenseExpirationDate, isDisplaySuppress, dictionary, text);
			this.CheckMaintenanceExpiry(days2, maintenanceExpirationDate, isDisplaySuppress, dictionary, text);
			if (dictionary.Count > 0)
			{
				AdvancedAlertDialogBox advancedAlertDialogBox = new AdvancedAlertDialogBox(dictionary, Metalogix.MLLicensing.Properties.Resources.License_Alert_Title, Metalogix.MLLicensing.Properties.Resources.Alert_Title, Metalogix.MLLicensing.Properties.Resources.Contact_Sales_For_Limit_About_To_Exceed, string.Format(Metalogix.MLLicensing.Properties.Resources.Suppress_Alert_Message, UIConfigurationVariables.DaysToSuppressLicenseAlert));
				advancedAlertDialogBox.ShowDialog();
			}
		}

		private void CheckLicenseUsage(MLLicenseCommon license, double percentage, long usedData, bool isDisplaySuppress, System.Collections.Generic.Dictionary<string, string> alertMessages)
		{
			if (percentage > 80.0 && percentage < 100.0)
			{
				if (!license.DataLimitExceeded && isDisplaySuppress)
				{
					alertMessages.Add(string.Format(Metalogix.MLLicensing.Properties.Resources.LicenseData_Usage_Warning, percentage.ToString("n2")), string.Format(Metalogix.MLLicensing.Properties.Resources.License_Data_Usage, Format.FormatSize(new long?(usedData)), Format.FormatSize(new long?(license.LicensedData))));
					return;
				}
			}
			else if (license.DataLimitExceeded)
			{
				alertMessages.Add(string.Format("Error: {0}", Metalogix.MLLicensing.Properties.Resources.License_Limit_Exceeded), string.Format(Metalogix.MLLicensing.Properties.Resources.License_Data_Usage, Format.FormatSize(new long?(usedData)), Format.FormatSize(new long?(license.LicensedData))));
			}
		}

		private void CheckLicenseExpiry(int daysBeforeLicenseExpiry, System.DateTime licenseExpirationDate, bool isDisplaySuppress, System.Collections.Generic.Dictionary<string, string> alertMessages, string MetalogixDateFormat)
		{
			if (daysBeforeLicenseExpiry >= 0 && daysBeforeLicenseExpiry <= UIConfigurationVariables.DaysToSuppressLicenseAlert && isDisplaySuppress)
			{
				alertMessages.Add(string.Format(Metalogix.MLLicensing.Properties.Resources.License_About_To_Expire, daysBeforeLicenseExpiry), string.Format(Metalogix.MLLicensing.Properties.Resources.License_Expiry_Date, licenseExpirationDate.ToString(MetalogixDateFormat)));
				return;
			}
			if (daysBeforeLicenseExpiry < 0)
			{
				alertMessages.Add(string.Format("Error: {0}", Metalogix.MLLicensing.Properties.Resources.License_Has_Expired), string.Format(Metalogix.MLLicensing.Properties.Resources.License_Expiry_Date, licenseExpirationDate.ToString(MetalogixDateFormat)));
			}
		}

		private void CheckMaintenanceExpiry(int daysBeforeMaintenanceExpiry, System.DateTime maintenanceExpirationDate, bool isDisplaySuppress, System.Collections.Generic.Dictionary<string, string> alertMessages, string MetalogixDateFormat)
		{
			if (daysBeforeMaintenanceExpiry >= 0 && daysBeforeMaintenanceExpiry <= UIConfigurationVariables.DaysToSuppressLicenseAlert && isDisplaySuppress)
			{
				alertMessages.Add(string.Format(Metalogix.MLLicensing.Properties.Resources.Maintenance_About_To_Expire, daysBeforeMaintenanceExpiry), string.Format(Metalogix.MLLicensing.Properties.Resources.Support_Maintenance_Expiry_Date, maintenanceExpirationDate.ToString(MetalogixDateFormat)));
				return;
			}
			if (daysBeforeMaintenanceExpiry < 0)
			{
				alertMessages.Add(string.Format("Error: {0}", Metalogix.MLLicensing.Properties.Resources.Support_Maintenance_Expired), string.Format(Metalogix.MLLicensing.Properties.Resources.Support_Maintenance_Expiry_Date, maintenanceExpirationDate.ToString(MetalogixDateFormat)));
			}
		}

		private void AddAlertMessageInList(string displayMessage, string displayInformation, System.Collections.Generic.Dictionary<string, string> alertMessage)
		{
			alertMessage.Add(displayMessage, displayInformation);
		}

		private static T GetApplicationAttribute<T>() where T : System.Attribute
		{
			System.Reflection.Assembly entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
			if (entryAssembly == null)
			{
				return default(T);
			}
			object[] customAttributes = entryAssembly.GetCustomAttributes(typeof(T), true);
			if (customAttributes.Length == 0)
			{
				return default(T);
			}
			return (T)((object)customAttributes[0]);
		}

		private static T[] GetApplicationAttributes<T>() where T : System.Attribute
		{
			System.Reflection.Assembly entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
			if (entryAssembly == null)
			{
				return null;
			}
			object[] customAttributes = entryAssembly.GetCustomAttributes(typeof(T), true);
			if (customAttributes.Length == 0)
			{
				return null;
			}
			return (T[])customAttributes;
		}

		private void LoadViewOptionsFromAssembly()
		{
			try
			{
				bool flag = false;
				System.Reflection.Assembly entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
				System.Reflection.AssemblyName[] referencedAssemblies = entryAssembly.GetReferencedAssemblies();
				System.Type typeFromHandle = typeof(ExplorerViewOption);
				System.Reflection.AssemblyName[] array = referencedAssemblies;
				for (int i = 0; i < array.Length; i++)
				{
					System.Reflection.AssemblyName assemblyRef = array[i];
					try
					{
						System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(assemblyRef);
						System.Type[] exportedTypes = assembly.GetExportedTypes();
						System.Type[] array2 = exportedTypes;
						for (int j = 0; j < array2.Length; j++)
						{
							System.Type type = array2[j];
							try
							{
								if (!type.IsAbstract && !(type.BaseType == null) && type.IsSubclassOf(typeFromHandle))
								{
									object obj = System.Activator.CreateInstance(type);
									ExplorerViewOption explorerViewOption = (ExplorerViewOption)obj;
									BarCheckItem barCheckItem = new BarCheckItem
									{
										Caption = explorerViewOption.GetName(),
										Glyph = explorerViewOption.GetImage(),
										LargeGlyph = explorerViewOption.GetLargeImage(),
										Tag = type
									};
									barCheckItem.CheckedChanged += new ItemClickEventHandler(this.ViewOptionCheckedChanged);
									barCheckItem.Checked = explorerViewOption.GetSetting();
									UIMainForm.AddSuperTipToTypedBarItem(barCheckItem, type);
									this.ribbon.Items.Add(barCheckItem);
									this.ribbonPageGroupViewListOptions.ItemLinks.Add(barCheckItem);
									flag = true;
								}
							}
							catch (System.Exception)
							{
							}
						}
					}
					catch (System.Exception)
					{
					}
				}
				if (flag)
				{
					this.ribbonPageGroupViewListOptions.Visible = true;
				}
			}
			catch (System.Exception)
			{
			}
		}

		private void LoadApplicationSettingsOptions()
		{
			ApplicationSettingAttribute[] applicationAttributes = UIMainForm.GetApplicationAttributes<ApplicationSettingAttribute>();
			if (applicationAttributes != null)
			{
				foreach (ApplicationSettingAttribute current in from a in applicationAttributes
				orderby a.OrderIndex
				select a)
				{
					this.AddButtonToApplicationSettingsGroup(current);
				}
			}
		}

		private void BarButtonItemUpdateCheckItemClick(object sender, ItemClickEventArgs e)
		{
			UpdaterService.CheckForUpdate(true, true);
		}

		private void UIMainForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.SaveSettings();
			if (this.w_jobListFullControl != null && this.w_jobListFullControl.DataSource != null)
			{
				this.w_jobListFullControl.DataSource.Dispose();
			}
		}

		private void BarButtonItemRefreshItemClick(object sender, ItemClickEventArgs e)
		{
			this.barButtonItemRefresh.Enabled = false;
			this.Cursor = Cursors.WaitCursor;
			base.SuspendLayout();
			try
			{
				Metalogix.Explorer.Settings.RefreshActiveConnections();
				if (this.LeftApplicationControl != null)
				{
					this.LeftApplicationControl.DataSource = null;
					this.LeftApplicationControl.DataSource = Metalogix.Explorer.Settings.ActiveConnections;
				}
				if (this.RightApplicationControl != null)
				{
					this.RightApplicationControl.DataSource = null;
					this.RightApplicationControl.DataSource = Metalogix.Explorer.Settings.ActiveConnections;
				}
				LongAccumulator.Message.Send("All Connections Refreshed", 1L, null);
			}
			catch (System.Exception ex)
			{
				GlobalServices.ErrorHandler.HandleException("Refresh Connections Error", "Refreshing all connections failed: " + ex.Message, ex, ErrorIcon.Error);
			}
			finally
			{
				base.ResumeLayout();
				this.Cursor = Cursors.Default;
				this.barButtonItemRefresh.Enabled = true;
			}
		}

		private void BarCheckItemSecondExplorerCheckedChanged(object sender, ItemClickEventArgs e)
		{
			UISettings.AppSettings.ShowSecondExplorer = this.barCheckItemSecondExplorer.Checked;
		}

		private void BarCheckItemShowJobsListCheckedChanged(object sender, ItemClickEventArgs e)
		{
			UISettings.AppSettings.ShowJobsList = this.barCheckItemShowJobsList.Checked;
		}

		private void BarCheckItemEnableStickyCheckedChanged(object sender, ItemClickEventArgs e)
		{
			if (UIConfigurationVariables.EnableStickySettings != this.barCheckItemEnableSticky.Checked)
			{
				StringAccumulator.Message.Send("SettingRibbonClicked", this.barCheckItemEnableSticky.Caption, this.barCheckItemEnableSticky.Checked.ToString(), false, null);
			}
			UIConfigurationVariables.EnableStickySettings = this.barCheckItemEnableSticky.Checked;
		}

		private void BarButtonItemResetStickyItemClick(object sender, ItemClickEventArgs e)
		{
			if (FlatXtraMessageBox.Show(Metalogix.UI.WinForms.Properties.Resources.Preserve_Configuration_Options_Reset_Question, Metalogix.UI.WinForms.Properties.Resources.Preserve_Configuration_Options_Reset_Title, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
			{
				ActionOptionsProvider.ResetDefaultOptions();
				StringAccumulator.Message.Send("SettingRibbonClicked", this.barButtonItemResetSticky.Caption, false, null);
			}
		}

		private void BarButtonItemHelpTopicsItemClick(object sender, ItemClickEventArgs e)
		{
			try
			{
				DocumentationHelper.ShowHelp(this, string.Empty);
			}
			catch (System.Exception ex)
			{
				GlobalServices.ErrorHandler.HandleException("Help Error", string.Format("Error opening Help file : {0}", ex.Message), ex, ErrorIcon.Error);
			}
		}

		private void BarButtonItemAboutItemClick(object sender, ItemClickEventArgs e)
		{
			UIAboutForm uIAboutForm = new UIAboutForm
			{
				Application = this.Application
			};
			uIAboutForm.ShowDialog();
		}

		private void BarButtonItemUpdateLicenceItemClick(object sender, ItemClickEventArgs e)
		{
			if (this.Application == null)
			{
				return;
			}
			this.Application.SetLicense(true);
		}

		private void BarButtonItemFacebookItemClick(object sender, ItemClickEventArgs e)
		{
			UIMainForm.OpenUrl("http://www.facebook.com/pages/Waltham-MA/Metalogix/191463486446");
		}

		private void BarButtonItemTwitterItemClick(object sender, ItemClickEventArgs e)
		{
			UIMainForm.OpenUrl("http://twitter.com/metalogix");
		}

		private void BarButtonItemLinkedInItemClick(object sender, ItemClickEventArgs e)
		{
			UIMainForm.OpenUrl("http://www.linkedin.com/company/239787");
		}

		private void BarButtonItemBlogItemClick(object sender, ItemClickEventArgs e)
		{
			UIMainForm.OpenUrl("http://www.metalogix.com/blog/");
		}

		private void BarButtonItemMetalogixAcademyItemClick(object sender, ItemClickEventArgs e)
		{
			using (UIMarketingSplashForm uIMarketingSplashForm = new UIMarketingSplashForm(UIMarketingSplashForm.Product.MetalogixAcademyContentMatrix, false))
			{
				uIMarketingSplashForm.ShowDialog();
			}
		}

		private void BarButtonItemSupportForumItemClick(object sender, ItemClickEventArgs e)
		{
			UIMainForm.OpenUrl("http://community.metalogix.net/");
		}

		private void BarButtonItemContactSupportItemClick(object sender, ItemClickEventArgs e)
		{
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder("mailto:support@metalogix.net?subject=" + UIApplication.INSTANCE.ProductName + ": Support Request&body=%0A%0A%0A%0A%0A==================== Client Info ====================", 1000);
			try
			{
				string osName = UIMainForm.GetOsName();
				string versionString = System.Environment.OSVersion.VersionString;
				stringBuilder.Append("%0AEnvironment:");
				stringBuilder.Append("%0AOperating System: ");
				stringBuilder.Append(osName);
				stringBuilder.Append("%0AOS Version: ");
				stringBuilder.Append(versionString);
				if (this.Application != null)
				{
					stringBuilder.Append("%0A%0AProduct Info:");
					stringBuilder.Append("%0AProduct Name: ");
					stringBuilder.Append(this.Application.ProductName);
					stringBuilder.Append("%0AProduct Version: ");
					stringBuilder.Append(this.Application.ProductVersion);
					MLLicense license = this.Application.GetLicense();
					if (license != null)
					{
						stringBuilder.Append("%0A%0ALicense: ");
						stringBuilder.Append(license.LicenseType);
						string[] licenseCustomInfo = license.GetLicenseCustomInfo();
						for (int i = 0; i < licenseCustomInfo.Length; i++)
						{
							string str = licenseCustomInfo[i];
							stringBuilder.Append("%0A" + str);
						}
						stringBuilder.Append("%0AKey: ");
						stringBuilder.Append(license.LicenseKey);
						stringBuilder.Append("%0ARegistered To:");
						stringBuilder.Append("%0AName: ");
						stringBuilder.Append(license.Name);
						stringBuilder.Append("%0AEmail: ");
						stringBuilder.Append(license.Email);
						stringBuilder.Append("%0AOrganization: ");
						stringBuilder.Append(license.Organization);
						stringBuilder.Append("%0AExpiry Date: ");
						stringBuilder.Append(license.ExpiryDate.ToString("dd-MMM-yyyy"));
						System.Diagnostics.Process.Start(stringBuilder.ToString());
					}
				}
			}
			catch (System.Exception ex)
			{
				ex.Data.Add("Support e-mail", stringBuilder.ToString());
				GlobalServices.ErrorHandler.HandleException("Contact Support Error", "An error occurred in contacting support : " + ex.Message, ex, ErrorIcon.Error);
			}
		}

		private void barButtonItemReplicator_ItemClick(object sender, ItemClickEventArgs e)
		{
			using (UIMarketingSplashForm uIMarketingSplashForm = new UIMarketingSplashForm(UIMarketingSplashForm.Product.Replicator, false))
			{
				uIMarketingSplashForm.ShowDialog();
			}
		}

		private void barButtonItemStoragePoint_ItemClick(object sender, ItemClickEventArgs e)
		{
			using (UIMarketingSplashForm uIMarketingSplashForm = new UIMarketingSplashForm(UIMarketingSplashForm.Product.StoragePoint, false))
			{
				uIMarketingSplashForm.ShowDialog();
			}
		}

		private void buttonBarManageActionTemplates_ItemClick(object sender, ItemClickEventArgs e)
		{
			ActionTemplateManager actionTemplateManager = new ActionTemplateManager(ActionOptionsProvider.TemplatesProvider);
			actionTemplateManager.ShowDialog();
			StringAccumulator.Message.Send("SettingRibbonClicked", this.barButtonManageActionTemplates.Caption, false, null);
		}

		private static void OpenUrl(string sUrl)
		{
			try
			{
				using (System.Diagnostics.Process process = new System.Diagnostics.Process())
				{
					process.StartInfo.FileName = sUrl;
					process.StartInfo.UseShellExecute = true;
					process.Start();
				}
			}
			catch (System.Exception ex)
			{
				GlobalServices.ErrorHandler.HandleException("Open URL Error", string.Format("Error opening URL \"{0}\" : {1}", sUrl, ex.Message), ex, ErrorIcon.Warning);
			}
		}

		private static string GetOsName()
		{
			System.OperatingSystem oSVersion = System.Environment.OSVersion;
			string result = "UNKNOWN";
			switch (oSVersion.Platform)
			{
			case System.PlatformID.Win32Windows:
			{
				int minor = oSVersion.Version.Minor;
				if (minor != 0)
				{
					if (minor != 10)
					{
						if (minor == 90)
						{
							result = "Windows Me";
						}
					}
					else
					{
						result = ((oSVersion.Version.Revision.ToString(System.Globalization.CultureInfo.InvariantCulture) == "2222A") ? "Windows 98 Second Edition" : "Windows 98");
					}
				}
				else
				{
					result = "Windows 95";
				}
				break;
			}
			case System.PlatformID.Win32NT:
				switch (oSVersion.Version.Major)
				{
				case 3:
					result = "Windows NT 3.51";
					break;
				case 4:
					result = "Windows NT 4.0";
					break;
				case 5:
					switch (oSVersion.Version.Minor)
					{
					case 0:
						result = "Windows 2000";
						break;
					case 1:
						result = "Windows XP";
						break;
					case 2:
						result = "Windows Server 2003";
						break;
					}
					break;
				case 6:
					result = "Windows Vista / Windows Server 2008";
					break;
				}
				break;
			}
			return result;
		}

		private void OnLicenseUpdated(object sender, System.EventArgs e)
		{
			this.UpdateApplicationSettings();
		}

		private void OnConfigurationVariablesChanged(object sender, ConfigurationVariables.ConfigVarsChangedArgs configVarsChangedArgs)
		{
			this.UpdateApplicationSettings();
		}

		private void UIMainFormLoad(object sender, System.EventArgs e)
		{
			if (ApplicationData.IsDesignMode())
			{
				return;
			}
			try
			{
				if (this.RightApplicationControl != null && this.RightApplicationControl.DataSource == null)
				{
					this.RightApplicationControl.DataSource = Metalogix.Explorer.Settings.ActiveConnections;
				}
				if (this.LeftApplicationControl != null && this.LeftApplicationControl.DataSource == null)
				{
					this.LeftApplicationControl.DataSource = Metalogix.Explorer.Settings.ActiveConnections;
				}
				SecondExplorerAttribute applicationAttribute = UIMainForm.GetApplicationAttribute<SecondExplorerAttribute>();
				if (applicationAttribute != null && !applicationAttribute.ExplorerEnabled)
				{
					if (this.barCheckItemSecondExplorer.Links != null && this.barCheckItemSecondExplorer.Links.Count != 0)
					{
						this.barCheckItemSecondExplorer.Links.Clear();
					}
					this.barCheckItemSecondExplorer.Checked = false;
				}
				this.LoadApplicationSettingsOptions();
				this.ribbonPageGroupViewListOptions.ItemLinks.Clear();
				this.ribbonPageGroupViewListOptions.Visible = false;
				this.LoadViewOptionsFromAssembly();
				this.barCheckItemShowCheckBoxes.Checked = UISettings.AppSettings.ShowExplorerCheckBoxes;
				System.Drawing.Size windowSize = UISettings.AppSettings.WindowSize;
				if (!windowSize.IsEmpty)
				{
					int x = base.Location.X + (base.Size.Width - windowSize.Width) / 2;
					int y = base.Location.Y + (base.Size.Height - windowSize.Height) / 2;
					base.Size = windowSize;
					base.Location = new System.Drawing.Point(x, y);
				}
				if (UISettings.AppSettings.IsMaximized)
				{
					base.WindowState = FormWindowState.Maximized;
				}
				if (UISettings.AppSettings.ExplorerHeight >= 0 && UISettings.AppSettings.ExplorerHeight > this.splitContainerH.Panel1MinSize && UISettings.AppSettings.ExplorerHeight < this.splitContainerH.Width - this.splitContainerH.Panel2MinSize)
				{
					this.splitContainerH.SplitterDistance = UISettings.AppSettings.ExplorerHeight;
				}
				if (UISettings.AppSettings.LeftExplorerWidth >= 0 && UISettings.AppSettings.LeftExplorerWidth > this.splitContainerV.Panel1MinSize && UISettings.AppSettings.LeftExplorerWidth < this.splitContainerV.Width - this.splitContainerV.Panel2MinSize)
				{
					this.splitContainerV.SplitterDistance = UISettings.AppSettings.LeftExplorerWidth;
				}
				this.GetApplicableConnectActions();
			}
			catch (System.Exception)
			{
			}
			try
			{
				this.barCheckItemEnableSticky.Checked = UIConfigurationVariables.EnableStickySettings;
				this.barCheckItemShowJobsList.Checked = UISettings.AppSettings.ShowJobsList;
				if (this.barCheckItemSecondExplorer.Links != null && this.barCheckItemSecondExplorer.Links.Count != 0)
				{
					this.barCheckItemSecondExplorer.Checked = UISettings.AppSettings.ShowSecondExplorer;
					this.splitContainerV.Panel2Collapsed = !UISettings.AppSettings.ShowSecondExplorer;
				}
				else
				{
					this.barCheckItemSecondExplorer.Checked = false;
					this.splitContainerV.Panel2Collapsed = true;
				}
				this.splitContainerH.Panel2Collapsed = !UISettings.AppSettings.ShowJobsList;
			}
			catch (System.Exception)
			{
			}
			try
			{
				MLLicense license = this.Application.GetLicense();
				this.w_jobListFullControl.ShowLicenseUsage = (license != null && license.GetType().IsAssignableFrom(typeof(MLLicenseCommon)));
			}
			catch (System.Exception)
			{
			}
			UISettings.AppSettings.SettingsChanged += new UIApplicationSettings.SettingsChangedHander(this.AppSettingsSettingsChanged);
			NavigateAction.NavigationRequest += new NavigationRequestHandler(this.NavigateActionNavigationRequest);
			JobUIHelper.JobsLaunching += new JobUIHelper.JobsLaunchingHandler(this.On_JobsLaunching);
		}

		private void UpdateApplicationSettings()
		{
			foreach (BarItemLink barItemLink in this.ribbonPageGroupGeneralSettings.ItemLinks)
			{
				ApplicationSettingAttribute applicationSettingAttribute = barItemLink.Item.Tag as ApplicationSettingAttribute;
				if (applicationSettingAttribute != null)
				{
					barItemLink.Visible = applicationSettingAttribute.IsValid;
					if (barItemLink.Caption.Equals(Metalogix.UI.WinForms.Properties.Resources.AppSetting_ShowAdvanced, System.StringComparison.InvariantCultureIgnoreCase) && ApplicationData.GetProductName().Equals("Content Matrix Console - SharePoint Edition", System.StringComparison.InvariantCultureIgnoreCase))
					{
						BarCheckItem barCheckItem = barItemLink.Item as BarCheckItem;
						if (barCheckItem != null)
						{
							this.SetVisibilityForConfigurationOptions(barCheckItem.Checked);
						}
					}
				}
			}
		}

		private void SetVisibilityForConfigurationOptions(bool isVisible)
		{
			this.barCheckItemEnableSticky.Links[0].Visible = isVisible;
			this.barButtonItemResetSticky.Links[0].Visible = isVisible;
			this.ribbonPageGroupPreserve.Text = (isVisible ? "Configuration Options" : string.Empty);
		}

		private void AppSettingsSettingsChanged()
		{
			if (this.barCheckItemSecondExplorer.Links != null && this.barCheckItemSecondExplorer.Links.Count != 0)
			{
				this.splitContainerV.Panel2Collapsed = !UISettings.AppSettings.ShowSecondExplorer;
			}
			else
			{
				this.splitContainerV.Panel2Collapsed = true;
			}
			this.splitContainerH.Panel2Collapsed = !UISettings.AppSettings.ShowJobsList;
		}

		private void NavigateActionNavigationRequest(NavigationRequestEventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				base.SuspendLayout();
				System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
				xmlDocument.LoadXml(e.XmlLocation);
				System.Xml.XmlNodeList xmlNodeList = xmlDocument.SelectNodes("//Location");
				System.Collections.Generic.List<Location> list = new System.Collections.Generic.List<Location>(xmlNodeList.Count);
				foreach (System.Xml.XmlNode xmlNode in xmlNodeList)
				{
					list.Add(new Location(xmlNode));
				}
				if (e.NavPreference == NavigationPreference.Left || !UISettings.AppSettings.ShowSecondExplorer)
				{
					if (this.LeftApplicationControl != null)
					{
						this.LeftApplicationControl.NavigateToLocation(list);
					}
				}
				else if (this.RightApplicationControl != null)
				{
					this.RightApplicationControl.NavigateToLocation(list);
				}
			}
			catch (System.Exception ex)
			{
				GlobalServices.ErrorHandler.HandleException("Navigation Request Error", "Unable to navigate to location: " + ex.Message, ex, ErrorIcon.Error);
			}
			finally
			{
				base.ResumeLayout();
				this.Cursor = Cursors.Default;
			}
		}

		private void On_JobsLaunching(Job[] jobs)
		{
			//this.DisplayNag(false);
			if (this.w_jobListFullControl.DataSource == null)
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < jobs.Length; i++)
			{
				Job job = jobs[i];
				if (job.Parent == null)
				{
					this.w_jobListFullControl.DataSource.Jobs.Add(job);
					flag = true;
				}
			}
			if (flag)
			{
				this.w_jobListFullControl.DataSource.Jobs.Update();
			}
		}

		private void RefreshApplicationControls()
		{
			this.RefreshApplicationControl(this.LeftApplicationControl);
			this.RefreshApplicationControl(this.RightApplicationControl);
		}

		private void RefreshApplicationControl(ApplicationControl control)
		{
			if (control == null)
			{
				return;
			}
			control.ExplorerMultiSelectEnabled = this.Application.MultiSelectEnabled;
			control.ExplorerMultiSelectLimitationMethod = this.Application.MultiSelectLimitationMethod;
			if (this.Application.ItemDataConverterEnabled)
			{
				control.ItemsViewDataConverter = this.Application.ItemDataConverter;
			}
		}

		private void RebuildActionContextMenu()
		{
			this.ActionPaletteControl.HostingControl = this.FocusedContextMenuContainer;
			this.ActionPaletteControl.BuildActionMenu();
		}

		private void GetApplicableConnectActions()
		{
			XMLAbleList souceSelections = new XMLAbleList();
			XMLAbleList targetSelections = new XMLAbleList();
			ActionCollection applicableActions = this.ActionPaletteControl.Actions.GetActions(typeof(ISharePointConnectAction)).GetApplicableActions(souceSelections, targetSelections);
			ActionCollection applicableActions2 = this.ActionPaletteControl.Actions.GetActions(typeof(IConnectAction)).GetApplicableActions(souceSelections, targetSelections);
			foreach (Metalogix.Actions.Action current in ((System.Collections.Generic.IEnumerable<Metalogix.Actions.Action>)applicableActions2))
			{
				if (!applicableActions.Contains(current) && this.connectActionEnabled(current))
				{
					this.AddButtonToRibbonPagesGroup(current);
				}
			}
			foreach (Metalogix.Actions.Action current2 in ((System.Collections.Generic.IEnumerable<Metalogix.Actions.Action>)applicableActions))
			{
				this.AddButtonToRibbonPagesGroup(current2);
			}
		}

		private bool connectActionEnabled(Metalogix.Actions.Action action)
		{
			bool result = true;
			string name;
			if ((name = action.Name) != null)
			{
				if (!(name == "Connect to WordPress"))
				{
					if (!(name == "Connect to Atom Feed"))
					{
						if (name == "Connect to Blogger")
						{
							result = UIConfigurationVariables.ShowBloggerConnection;
						}
					}
					else
					{
						result = UIConfigurationVariables.ShowAtomFeedConnection;
					}
				}
				else
				{
					result = UIConfigurationVariables.ShowWordPressConnection;
				}
			}
			return result;
		}

		private void AddButtonToRibbonPagesGroup(Metalogix.Actions.Action action)
		{
			BarButtonItem barButtonItem = new BarButtonItem
			{
				Caption = action.DisplayName,
				Glyph = action.GetImage(null),
				LargeGlyph = action.GetLargeImage(null),
				RibbonStyle = RibbonItemStyles.Default,
				Tag = action
			};
			UIMainForm.AddSuperTipToTypedBarItem(barButtonItem, action.ActionType);
			barButtonItem.ItemClick += new ItemClickEventHandler(this.ButtonOnItemClick);
			this.ribbon.Items.Add(barButtonItem);
			this.ribbonPageGroupConnect.ItemLinks.Add(barButtonItem);
		}

	    // Metalogix.UI.WinForms.UIMainForm
	    private void AddButtonToApplicationSettingsGroup(ApplicationSettingAttribute applicationSetting)
	    {
	        IBooleanApplicationSetting booleanApplicationSetting = applicationSetting.Setting as IBooleanApplicationSetting;
	        BarItem barItem = (booleanApplicationSetting != null) ? (BarItem) new BarCheckItem() : new BarButtonItem();
	        barItem.Caption = applicationSetting.DisplayText;
	        barItem.Glyph = applicationSetting.GetImage(null);
	        barItem.LargeGlyph = applicationSetting.GetLargeImage(null);
	        ItemClickEventHandler value = applicationSetting.CreateSettingEventHandler();
	        barItem.ItemClick += value;
	        barItem.Tag = applicationSetting;
	        UIMainForm.AddSuperTipToTypedBarItem(barItem, applicationSetting.SettingType);
	        BarCheckItem barCheckItem = barItem as BarCheckItem;
	        if (barCheckItem != null)
	        {
	            barCheckItem.Checked = booleanApplicationSetting.Value;
	        }
	        if (barItem.Caption.Equals(Metalogix.UI.WinForms.Properties.Resources.AppSetting_ShowAdvanced, System.StringComparison.InvariantCultureIgnoreCase) && ApplicationData.GetProductName().Equals("Content Matrix Console - SharePoint Edition", System.StringComparison.InvariantCultureIgnoreCase))
	        {
	            this.SetVisibilityForConfigurationOptions(booleanApplicationSetting != null && booleanApplicationSetting.Value);
	        }
	        this.ribbon.Items.Add(barItem);
	        BarItemLink barItemLink = this.ribbonPageGroupGeneralSettings.ItemLinks.Add(barItem);
	        barItemLink.Visible = applicationSetting.IsValid;
	    }

        private void ButtonOnItemClick(object sender, ItemClickEventArgs itemClickEventArgs)
		{
			BarButtonItem barButtonItem = itemClickEventArgs.Item as BarButtonItem;
			Metalogix.Actions.Action action = barButtonItem.Tag as Metalogix.Actions.Action;
			if (this.Application.GetLicense() == null)
			{
				return;
			}
			System.Activator.CreateInstance(action.GetType());
			IXMLAbleList source = new XMLAbleList();
			IXMLAbleList target = new XMLAbleList();
			ActionPaletteControl.ActionClick(action, source, target);
		}

		private void BarButtonItemAgentWizardClick(object sender, ItemClickEventArgs itemClickEventArgs)
		{
			IXMLAbleList source = new CommonSerializableList<JobListFullControl>
			{
				this.w_jobListFullControl
			};
			IXMLAbleList target = new XMLAbleList();
			ActionPaletteControl.ActionClick(new AgentWizardAction(), source, target);
		}

		private void BarButtonItemAgentClick(object sender, ItemClickEventArgs itemClickEventArgs)
		{
			BarItem arg_06_0 = itemClickEventArgs.Item;
			Metalogix.Actions.Action action = new ManageAgentsAction();
			IXMLAbleList source = new XMLAbleList();
			IXMLAbleList target = new XMLAbleList();
			ActionPaletteControl.ActionClick(action, source, target);
		}

		private void BarButtonItemJobQueueClick(object sender, ItemClickEventArgs itemClickEventArgs)
		{
			Metalogix.Actions.Action action = new ManageQueueAction();
			IXMLAbleList source = new CommonSerializableList<UIMainForm>
			{
				this
			};
			IXMLAbleList target = new XMLAbleList();
			ActionPaletteControl.ActionClick(action, source, target);
		}

		private void barButtonItemCreateSupportPackage_ItemClick(object sender, ItemClickEventArgs e)
		{
			try
			{
				if (this.w_jobListFullControl != null && this.w_jobListFullControl.JobList != null)
				{
					Job[] selectedJobs = this.w_jobListFullControl.JobList.SelectedJobs;
					if (selectedJobs == null || selectedJobs.Length == 0)
					{
						FlatXtraMessageBox.Show("Please select a job from the job list at the bottom to generate the support zip file from.", "Select A Job", MessageBoxButtons.OK);
					}
					else if (selectedJobs.Length > 1)
					{
						FlatXtraMessageBox.Show("Please select only one job to generate the support zip file from.", "Select Only One Job", MessageBoxButtons.OK);
					}
					else
					{
						Job job = selectedJobs.First<Job>();
						CreateSupportPackageAction createSupportPackageAction = new CreateSupportPackageAction();
						IXMLAbleList iXMLAbleList = null;
						IXMLAbleList selectedObjects = this.w_jobListFullControl.JobList.SelectedObjects;
						if (createSupportPackageAction.Configure(ref iXMLAbleList, ref selectedObjects) == ConfigurationResult.Run)
						{
							string saveFile = createSupportPackageAction.CreateSupportPackage(job);
							createSupportPackageAction.PromptToOpen(saveFile);
						}
					}
				}
			}
			catch (System.Exception exc)
			{
				GlobalServices.ErrorHandler.HandleException(exc);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(UIMainForm));
			CommonSerializableList<object> sourceOverride = new CommonSerializableList<object>();
			this.ribbon = new RibbonControl();
			this.applicationMenu1 = new ApplicationMenu(this.components);
			this.imageCollectionSmall = new ImageCollection(this.components);
			this.barButtonItemRefresh = new BarButtonItem();
			this.barButtonItemAgentWizard = new BarButtonItem();
			this.barButtonItemAgent = new BarButtonItem();
			this.barButtonItemJobQueue = new BarButtonItem();
			this.barCheckItemSecondExplorer = new BarCheckItem();
			this.barCheckItemShowJobsList = new BarCheckItem();
			this.barCheckItemShowCheckBoxes = new BarCheckItem();
			this.barCheckItemEnableSticky = new BarCheckItem();
			this.barButtonItemResetSticky = new BarButtonItem();
			this.barButtonItemFacebook = new BarButtonItem();
			this.barButtonItemTwitter = new BarButtonItem();
			this.barButtonItemBlog = new BarButtonItem();
			this.barButtonItemMetalogixAcademy = new BarButtonItem();
			this.barButtonItemSupportForum = new BarButtonItem();
			this.barButtonItemContactSupport = new BarButtonItem();
			this.barButtonItemHelpTopics = new BarButtonItem();
			this.barButtonItemAbout = new BarButtonItem();
			this.barButtonItemUpdateCheck = new BarButtonItem();
			this.barButtonItemUpdateLicence = new BarButtonItem();
			this.barButtonItemLinkedIn = new BarButtonItem();
			this.barButtonItemReplicator = new BarButtonItem();
			this.barButtonItemStoragePoint = new BarButtonItem();
			this.barButtonManageActionTemplates = new BarButtonItem();
			this.barButtonItemCreateSupportPackage = new BarButtonItem();
			this.imageCollectionLarge = new ImageCollection(this.components);
			this.ribbonPageConnection = new RibbonPage();
			this.ribbonPageGroupConnect = new RibbonPageGroup();
			this.ribbonPageGroupRefresh = new RibbonPageGroup();
			this.ribbonPageGroupAgent = new RibbonPageGroup();
			this.ribbonPageView = new RibbonPage();
			this.ribbonPageGroupViewShowViews = new RibbonPageGroup();
			this.ribbonPageGroupViewListOptions = new RibbonPageGroup();
			this.ribbonPageSettings = new RibbonPage();
			this.ribbonPageGroupPreserve = new RibbonPageGroup();
			this.ribbonPageGroupGeneralSettings = new RibbonPageGroup();
			this.ribbonPageHelp = new RibbonPage();
			this.ribbonPageGroupHelp = new RibbonPageGroup();
			this.ribbonPageGroupUpgrade = new RibbonPageGroup();
			this.ribbonPageGroupSupport = new RibbonPageGroup();
			this.ribbonPageGroupSocial = new RibbonPageGroup();
			this.ribbonPageGroupProducts = new RibbonPageGroup();
			this.splitContainerH = new SplitContainer();
			this.splitContainerV = new SplitContainer();
			this.w_jobListFullControl = new JobListFullControl();
			this.ActionPaletteControl = new ActionPaletteControl();
			this.DefaultLookAndFeel = new DefaultLookAndFeel(this.components);
			this._defaultToolTipController = new DefaultToolTipController(this.components);
			((System.ComponentModel.ISupportInitialize)this.ribbon).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.applicationMenu1).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.imageCollectionSmall).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.imageCollectionLarge).BeginInit();
			this.splitContainerH.Panel1.SuspendLayout();
			this.splitContainerH.Panel2.SuspendLayout();
			this.splitContainerH.SuspendLayout();
			this.splitContainerV.SuspendLayout();
			base.SuspendLayout();
			this.ribbon.ApplicationButtonDropDownControl = this.applicationMenu1;
			this.ribbon.ApplicationIcon = Metalogix.UI.WinForms.Properties.Resources.app_icon;
			this.ribbon.Cursor = Cursors.Default;
			this.ribbon.ExpandCollapseItem.Id = 0;
			this.ribbon.Images = this.imageCollectionSmall;
			this.ribbon.Items.AddRange(new BarItem[]
			{
				this.ribbon.ExpandCollapseItem,
				this.barButtonItemRefresh,
				this.barButtonItemAgentWizard,
				this.barButtonItemAgent,
				this.barButtonItemJobQueue,
				this.barCheckItemSecondExplorer,
				this.barCheckItemShowJobsList,
				this.barCheckItemShowCheckBoxes,
				this.barCheckItemEnableSticky,
				this.barButtonItemResetSticky,
				this.barButtonItemFacebook,
				this.barButtonItemTwitter,
				this.barButtonItemBlog,
				this.barButtonItemMetalogixAcademy,
				this.barButtonItemSupportForum,
				this.barButtonItemContactSupport,
				this.barButtonItemHelpTopics,
				this.barButtonItemAbout,
				this.barButtonItemUpdateCheck,
				this.barButtonItemUpdateLicence,
				this.barButtonItemLinkedIn,
				this.barButtonItemReplicator,
				this.barButtonItemStoragePoint,
				this.barButtonManageActionTemplates,
				this.barButtonItemCreateSupportPackage
			});
			this.ribbon.LargeImages = this.imageCollectionLarge;
			this.ribbon.Location = new System.Drawing.Point(0, 0);
			this.ribbon.MaxItemId = 53;
			this.ribbon.Name = "ribbon";
			this.ribbon.Pages.AddRange(new RibbonPage[]
			{
				this.ribbonPageConnection,
				this.ribbonPageView,
				this.ribbonPageSettings,
				this.ribbonPageHelp
			});
			this.ribbon.ShowApplicationButton = DefaultBoolean.False;
			this.ribbon.ShowExpandCollapseButton = DefaultBoolean.True;
			this.ribbon.ShowFullScreenButton = DefaultBoolean.True;
			this.ribbon.Size = new System.Drawing.Size(811, 145);
			this.applicationMenu1.Name = "applicationMenu1";
			this.applicationMenu1.Ribbon = this.ribbon;
			this.imageCollectionSmall.ImageStream = (ImageCollectionStreamer)componentResourceManager.GetObject("imageCollectionSmall.ImageStream");
			this.imageCollectionSmall.Images.SetKeyName(0, "ContactSupport32.png");
			this.imageCollectionSmall.Images.SetKeyName(1, "SupportForum16.png");
			this.imageCollectionSmall.Images.SetKeyName(2, "ConnectToKnowledgeBase16x16.png");
			this.imageCollectionSmall.Images.SetKeyName(3, "OurBlog16.png");
			this.imageCollectionSmall.Images.SetKeyName(4, "Twitter16.png");
			this.imageCollectionSmall.Images.SetKeyName(5, "Facebook16.png");
			this.imageCollectionSmall.Images.SetKeyName(6, "UpdateLicense16.png");
			this.imageCollectionSmall.Images.SetKeyName(7, "CheckForUpdates16.png");
			this.imageCollectionSmall.Images.SetKeyName(8, "About16.png");
			this.imageCollectionSmall.Images.SetKeyName(9, "HelpTopics16.png");
			this.imageCollectionSmall.Images.SetKeyName(10, "ResetConfigurationOptions16.png");
			this.imageCollectionSmall.Images.SetKeyName(11, "PreserveConfigurationOptions16.png");
			this.imageCollectionSmall.Images.SetKeyName(12, "ShowCheckboxes16.png");
			this.imageCollectionSmall.Images.SetKeyName(13, "ShowJobsList16.png");
			this.imageCollectionSmall.Images.SetKeyName(14, "Show2ndExplorer16.png");
			this.imageCollectionSmall.Images.SetKeyName(15, "RefreshConnection16.png");
			this.imageCollectionSmall.Images.SetKeyName(16, "RemoveConnection16x16.png");
			this.imageCollectionSmall.Images.SetKeyName(17, "Linkedin16.png");
			this.imageCollectionSmall.Images.SetKeyName(18, "Replicator16X16.png");
			this.imageCollectionSmall.Images.SetKeyName(19, "StoragePoint16x16.png");
			/*this.imageCollectionSmall.Images.SetKeyName(20, "ManageJobConfigurations16.png");
			this.imageCollectionSmall.Images.SetKeyName(21, "MetalogixAcademyContentMatrix16.png");
			this.imageCollectionSmall.Images.SetKeyName(22, "envelope_sm.png");
			this.imageCollectionSmall.Images.SetKeyName(23, "ManageAgents16.png");
			this.imageCollectionSmall.Images.SetKeyName(24, "ManageQueue16.png");
			this.imageCollectionSmall.Images.SetKeyName(25, "Config Wizard.png");*/
			this.barButtonItemAgentWizard.Caption = "Configure Distributed Migration";
			this.barButtonItemAgentWizard.Id = 5;
			this.barButtonItemAgentWizard.ImageIndex = 25;
			this.barButtonItemAgentWizard.LargeImageIndex = 25;
			this.barButtonItemAgentWizard.Name = "barButtonItemAgentWizard";
			this.barButtonItemAgentWizard.ItemClick += new ItemClickEventHandler(this.BarButtonItemAgentWizardClick);
			this.barButtonItemAgent.Caption = "Manage Agents";
			this.barButtonItemAgent.Id = 5;
			this.barButtonItemAgent.ImageIndex = 23;
			this.barButtonItemAgent.LargeImageIndex = 23;
			this.barButtonItemAgent.Name = "barButtonItemAgent";
			this.barButtonItemAgent.ItemClick += new ItemClickEventHandler(this.BarButtonItemAgentClick);
			this.barButtonItemJobQueue.Caption = "Manage Queue";
			this.barButtonItemJobQueue.Id = 5;
			this.barButtonItemJobQueue.ImageIndex = 24;
			this.barButtonItemJobQueue.LargeImageIndex = 24;
			this.barButtonItemJobQueue.Name = "barButtonItemJobQueue";
			this.barButtonItemJobQueue.ItemClick += new ItemClickEventHandler(this.BarButtonItemJobQueueClick);
			this.barButtonItemRefresh.Caption = "Refresh Connection";
			this.barButtonItemRefresh.Id = 5;
			this.barButtonItemRefresh.ImageIndex = 15;
			this.barButtonItemRefresh.LargeImageIndex = 15;
			this.barButtonItemRefresh.Name = "barButtonItemRefresh";
			this.barButtonItemRefresh.ItemClick += new ItemClickEventHandler(this.BarButtonItemRefreshItemClick);
			this.barCheckItemSecondExplorer.Caption = "Show 2nd Explorer";
			this.barCheckItemSecondExplorer.Id = 25;
			this.barCheckItemSecondExplorer.ImageIndex = 14;
			this.barCheckItemSecondExplorer.LargeImageIndex = 14;
			this.barCheckItemSecondExplorer.Name = "barCheckItemSecondExplorer";
			this.barCheckItemSecondExplorer.CheckedChanged += new ItemClickEventHandler(this.BarCheckItemSecondExplorerCheckedChanged);
			this.barCheckItemShowJobsList.Caption = "Show Jobs List";
			this.barCheckItemShowJobsList.Id = 26;
			this.barCheckItemShowJobsList.ImageIndex = 13;
			this.barCheckItemShowJobsList.LargeImageIndex = 13;
			this.barCheckItemShowJobsList.Name = "barCheckItemShowJobsList";
			this.barCheckItemShowJobsList.CheckedChanged += new ItemClickEventHandler(this.BarCheckItemShowJobsListCheckedChanged);
			this.barCheckItemShowCheckBoxes.Caption = "Show Checkboxes";
			this.barCheckItemShowCheckBoxes.Id = 27;
			this.barCheckItemShowCheckBoxes.ImageIndex = 12;
			this.barCheckItemShowCheckBoxes.LargeImageIndex = 12;
			this.barCheckItemShowCheckBoxes.Name = "barCheckItemShowCheckBoxes";
			this.barCheckItemShowCheckBoxes.CheckedChanged += new ItemClickEventHandler(this.BarCheckItemShowCheckBoxesCheckedChanged);
			this.barCheckItemEnableSticky.Caption = "Preserve Configuration Options";
			this.barCheckItemEnableSticky.Id = 30;
			this.barCheckItemEnableSticky.ImageIndex = 11;
			this.barCheckItemEnableSticky.LargeImageIndex = 11;
			this.barCheckItemEnableSticky.Name = "barCheckItemEnableSticky";
			this.barCheckItemEnableSticky.CheckedChanged += new ItemClickEventHandler(this.BarCheckItemEnableStickyCheckedChanged);
			this.barButtonItemResetSticky.Caption = "Reset Configuration Options";
			this.barButtonItemResetSticky.Id = 31;
			this.barButtonItemResetSticky.ImageIndex = 10;
			this.barButtonItemResetSticky.LargeImageIndex = 10;
			this.barButtonItemResetSticky.Name = "barButtonItemResetSticky";
			this.barButtonItemResetSticky.ItemClick += new ItemClickEventHandler(this.BarButtonItemResetStickyItemClick);
			this.barButtonItemFacebook.Caption = "Become a Fan on Facebook";
			this.barButtonItemFacebook.Id = 38;
			this.barButtonItemFacebook.ImageIndex = 5;
			this.barButtonItemFacebook.LargeImageIndex = 5;
			this.barButtonItemFacebook.Name = "barButtonItemFacebook";
			this.barButtonItemFacebook.ItemClick += new ItemClickEventHandler(this.BarButtonItemFacebookItemClick);
			this.barButtonItemTwitter.Caption = "Follow Us on Twitter";
			this.barButtonItemTwitter.Id = 39;
			this.barButtonItemTwitter.ImageIndex = 4;
			this.barButtonItemTwitter.LargeImageIndex = 4;
			this.barButtonItemTwitter.Name = "barButtonItemTwitter";
			this.barButtonItemTwitter.ItemClick += new ItemClickEventHandler(this.BarButtonItemTwitterItemClick);
			this.barButtonItemBlog.Caption = "Subscribe to Our Blog";
			this.barButtonItemBlog.Id = 40;
			this.barButtonItemBlog.ImageIndex = 3;
			this.barButtonItemBlog.LargeImageIndex = 3;
			this.barButtonItemBlog.Name = "barButtonItemBlog";
			this.barButtonItemBlog.ItemClick += new ItemClickEventHandler(this.BarButtonItemBlogItemClick);
			this.barButtonItemMetalogixAcademy.Caption = "Metalogix Academy";
			this.barButtonItemMetalogixAcademy.Id = 41;
			this.barButtonItemMetalogixAcademy.ImageIndex = 21;
			this.barButtonItemMetalogixAcademy.LargeImageIndex = 21;
			this.barButtonItemMetalogixAcademy.Name = "barButtonItemMetalogixAcademy";
			this.barButtonItemMetalogixAcademy.ItemClick += new ItemClickEventHandler(this.BarButtonItemMetalogixAcademyItemClick);
			this.barButtonItemSupportForum.Caption = "Support Forum";
			this.barButtonItemSupportForum.Id = 42;
			this.barButtonItemSupportForum.ImageIndex = 1;
			this.barButtonItemSupportForum.LargeImageIndex = 1;
			this.barButtonItemSupportForum.Name = "barButtonItemSupportForum";
			this.barButtonItemSupportForum.ItemClick += new ItemClickEventHandler(this.BarButtonItemSupportForumItemClick);
			this.barButtonItemContactSupport.Caption = "Contact Support";
			this.barButtonItemContactSupport.Id = 43;
			this.barButtonItemContactSupport.ImageIndex = 22;
			this.barButtonItemContactSupport.LargeImageIndex = 22;
			this.barButtonItemContactSupport.Name = "barButtonItemContactSupport";
			this.barButtonItemContactSupport.ItemClick += new ItemClickEventHandler(this.BarButtonItemContactSupportItemClick);
			this.barButtonItemHelpTopics.Caption = "Help Topics";
			this.barButtonItemHelpTopics.Id = 44;
			this.barButtonItemHelpTopics.ImageIndex = 9;
			this.barButtonItemHelpTopics.LargeImageIndex = 9;
			this.barButtonItemHelpTopics.Name = "barButtonItemHelpTopics";
			this.barButtonItemHelpTopics.ItemClick += new ItemClickEventHandler(this.BarButtonItemHelpTopicsItemClick);
			this.barButtonItemAbout.Caption = "About";
			this.barButtonItemAbout.Id = 45;
			this.barButtonItemAbout.ImageIndex = 8;
			this.barButtonItemAbout.LargeImageIndex = 8;
			this.barButtonItemAbout.Name = "barButtonItemAbout";
			this.barButtonItemAbout.ItemClick += new ItemClickEventHandler(this.BarButtonItemAboutItemClick);
			this.barButtonItemUpdateCheck.Caption = "Check for Updates";
			this.barButtonItemUpdateCheck.Id = 46;
			this.barButtonItemUpdateCheck.ImageIndex = 7;
			this.barButtonItemUpdateCheck.LargeImageIndex = 7;
			this.barButtonItemUpdateCheck.Name = "barButtonItemUpdateCheck";
			this.barButtonItemUpdateCheck.ItemClick += new ItemClickEventHandler(this.BarButtonItemUpdateCheckItemClick);
			this.barButtonItemUpdateLicence.Caption = "Update Licence";
			this.barButtonItemUpdateLicence.Id = 47;
			this.barButtonItemUpdateLicence.ImageIndex = 6;
			this.barButtonItemUpdateLicence.LargeImageIndex = 6;
			this.barButtonItemUpdateLicence.Name = "barButtonItemUpdateLicence";
			this.barButtonItemUpdateLicence.ItemClick += new ItemClickEventHandler(this.BarButtonItemUpdateLicenceItemClick);
			this.barButtonItemLinkedIn.Caption = "Follow Us on LinkedIn";
			this.barButtonItemLinkedIn.Id = 48;
			this.barButtonItemLinkedIn.ImageIndex = 17;
			this.barButtonItemLinkedIn.LargeImageIndex = 17;
			this.barButtonItemLinkedIn.Name = "barButtonItemLinkedIn";
			this.barButtonItemLinkedIn.ItemClick += new ItemClickEventHandler(this.BarButtonItemLinkedInItemClick);
			this.barButtonItemReplicator.Caption = "Metalogix Replicator";
			this.barButtonItemReplicator.Id = 49;
			this.barButtonItemReplicator.ImageIndex = 18;
			this.barButtonItemReplicator.LargeImageIndex = 18;
			this.barButtonItemReplicator.Name = "barButtonItemReplicator";
			this.barButtonItemReplicator.ItemClick += new ItemClickEventHandler(this.barButtonItemReplicator_ItemClick);
			this.barButtonItemStoragePoint.Caption = "Metalogix StoragePoint";
			this.barButtonItemStoragePoint.Id = 50;
			this.barButtonItemStoragePoint.ImageIndex = 19;
			this.barButtonItemStoragePoint.LargeImageIndex = 19;
			this.barButtonItemStoragePoint.Name = "barButtonItemStoragePoint";
			this.barButtonItemStoragePoint.ItemClick += new ItemClickEventHandler(this.barButtonItemStoragePoint_ItemClick);
			this.barButtonManageActionTemplates.Caption = "Manage Job Configurations";
			this.barButtonManageActionTemplates.Id = 51;
			this.barButtonManageActionTemplates.ImageIndex = 20;
			this.barButtonManageActionTemplates.LargeImageIndex = 20;
			this.barButtonManageActionTemplates.Name = "barButtonManageActionTemplates";
			this.barButtonManageActionTemplates.ItemClick += new ItemClickEventHandler(this.buttonBarManageActionTemplates_ItemClick);
			this.barButtonItemCreateSupportPackage.Caption = "Create Support Zip File";
			this.barButtonItemCreateSupportPackage.Id = 52;
			this.barButtonItemCreateSupportPackage.ImageIndex = 0;
			this.barButtonItemCreateSupportPackage.LargeImageIndex = 0;
			this.barButtonItemCreateSupportPackage.Name = "barButtonItemCreateSupportPackage";
			this.barButtonItemCreateSupportPackage.RibbonStyle = RibbonItemStyles.Large;
			this.barButtonItemCreateSupportPackage.ItemClick += new ItemClickEventHandler(this.barButtonItemCreateSupportPackage_ItemClick);
			this.imageCollectionLarge.ImageSize = new System.Drawing.Size(32, 32);
			this.imageCollectionLarge.ImageStream = (ImageCollectionStreamer)componentResourceManager.GetObject("imageCollectionLarge.ImageStream");
			this.imageCollectionLarge.Images.SetKeyName(0, "ContactSupport32.png");
			this.imageCollectionLarge.Images.SetKeyName(1, "SupportForum32.png");
			this.imageCollectionLarge.Images.SetKeyName(2, "ConnectToKnowledgeBase32x32.png");
			this.imageCollectionLarge.Images.SetKeyName(3, "OurBlog32.png");
			this.imageCollectionLarge.Images.SetKeyName(4, "Twitter32.png");
			this.imageCollectionLarge.Images.SetKeyName(5, "Facebook32.png");
			this.imageCollectionLarge.Images.SetKeyName(6, "UpdateLicense32.png");
			this.imageCollectionLarge.Images.SetKeyName(7, "CheckForUpdates32.png");
			this.imageCollectionLarge.Images.SetKeyName(8, "About32.png");
			this.imageCollectionLarge.Images.SetKeyName(9, "HelpTopics32.png");
			this.imageCollectionLarge.Images.SetKeyName(10, "ResetConfigurationOptions32.png");
			this.imageCollectionLarge.Images.SetKeyName(11, "PreserveConfigurationOptions32.png");
			this.imageCollectionLarge.Images.SetKeyName(12, "ShowCheckboxes32.png");
			this.imageCollectionLarge.Images.SetKeyName(13, "ShowJobsList32.png");
			this.imageCollectionLarge.Images.SetKeyName(14, "Show2ndExplorer32.png");
			this.imageCollectionLarge.Images.SetKeyName(15, "RefreshConnection32.png");
			this.imageCollectionLarge.Images.SetKeyName(16, "RemoveConnection32x32.png");
			this.imageCollectionLarge.Images.SetKeyName(17, "Linkedin32.png");
			this.imageCollectionLarge.Images.SetKeyName(18, "Replicator32x32.png");
			this.imageCollectionLarge.Images.SetKeyName(19, "StoragePoint32x32.png");
			/*this.imageCollectionLarge.Images.SetKeyName(20, "ManageJobConfigurations32.png");
			this.imageCollectionLarge.Images.SetKeyName(21, "MetalogixAcademyContentMatrix32.png");
			this.imageCollectionLarge.Images.SetKeyName(22, "envelope_lg.png");
			this.imageCollectionLarge.Images.SetKeyName(23, "ManageAgents32.png");
			this.imageCollectionLarge.Images.SetKeyName(24, "ManageQueue32.png");
			this.imageCollectionLarge.Images.SetKeyName(25, "Config Wizard.png");*/
			this.ribbonPageConnection.Groups.AddRange(new RibbonPageGroup[]
			{
				this.ribbonPageGroupConnect,
				this.ribbonPageGroupRefresh,
				this.ribbonPageGroupAgent
			});
			this.ribbonPageConnection.Name = "ribbonPageConnection";
			this.ribbonPageConnection.Text = "Connection";
			this.ribbonPageGroupConnect.Name = "ribbonPageGroupConnect";
			this.ribbonPageGroupConnect.ShowCaptionButton = false;
			this.ribbonPageGroupConnect.Text = "Add Connection";
			this.ribbonPageGroupRefresh.ItemLinks.Add(this.barButtonItemRefresh);
			this.ribbonPageGroupRefresh.Name = "ribbonPageGroupRefresh";
			this.ribbonPageGroupRefresh.ShowCaptionButton = false;
			this.ribbonPageGroupRefresh.Text = "General";
			this.ribbonPageGroupAgent.ItemLinks.Add(this.barButtonItemAgentWizard);
			this.ribbonPageGroupAgent.ItemLinks.Add(this.barButtonItemAgent);
			this.ribbonPageGroupAgent.ItemLinks.Add(this.barButtonItemJobQueue);
			this.ribbonPageGroupAgent.Name = "ribbonPageGroupAgent";
			this.ribbonPageGroupAgent.ShowCaptionButton = false;
			this.ribbonPageGroupAgent.Text = "Distributed Migration";
			this.ribbonPageView.Groups.AddRange(new RibbonPageGroup[]
			{
				this.ribbonPageGroupViewShowViews,
				this.ribbonPageGroupViewListOptions
			});
			this.ribbonPageView.Name = "ribbonPageView";
			this.ribbonPageView.Text = "View";
			this.ribbonPageGroupViewShowViews.ItemLinks.Add(this.barCheckItemSecondExplorer);
			this.ribbonPageGroupViewShowViews.ItemLinks.Add(this.barCheckItemShowJobsList);
			this.ribbonPageGroupViewShowViews.ItemLinks.Add(this.barCheckItemShowCheckBoxes);
			this.ribbonPageGroupViewShowViews.Name = "ribbonPageGroupViewShowViews";
			this.ribbonPageGroupViewShowViews.ShowCaptionButton = false;
			this.ribbonPageGroupViewShowViews.Text = "Views";
			this.ribbonPageGroupViewListOptions.Name = "ribbonPageGroupViewListOptions";
			this.ribbonPageGroupViewListOptions.ShowCaptionButton = false;
			this.ribbonPageGroupViewListOptions.Text = "View Options";
			this.ribbonPageGroupViewListOptions.Visible = false;
			this.ribbonPageSettings.Groups.AddRange(new RibbonPageGroup[]
			{
				this.ribbonPageGroupPreserve,
				this.ribbonPageGroupGeneralSettings
			});
			this.ribbonPageSettings.Name = "ribbonPageSettings";
			this.ribbonPageSettings.Text = "Settings";
			this.ribbonPageGroupPreserve.ItemLinks.Add(this.barCheckItemEnableSticky);
			this.ribbonPageGroupPreserve.ItemLinks.Add(this.barButtonItemResetSticky);
			this.ribbonPageGroupPreserve.ItemLinks.Add(this.barButtonManageActionTemplates);
			this.ribbonPageGroupPreserve.Name = "ribbonPageGroupPreserve";
			this.ribbonPageGroupPreserve.ShowCaptionButton = false;
			this.ribbonPageGroupPreserve.Text = "Configuration Options";
			this.ribbonPageGroupGeneralSettings.Name = "ribbonPageGroupGeneralSettings";
			this.ribbonPageGroupGeneralSettings.ShowCaptionButton = false;
			this.ribbonPageGroupGeneralSettings.Text = "General";
			this.ribbonPageHelp.Groups.AddRange(new RibbonPageGroup[]
			{
				this.ribbonPageGroupHelp,
				//this.ribbonPageGroupUpgrade,
				//this.ribbonPageGroupSupport,
				//this.ribbonPageGroupSocial,
				//this.ribbonPageGroupProducts
			});
			this.ribbonPageHelp.Name = "ribbonPageHelp";
			this.ribbonPageHelp.Text = "Help";
			this.ribbonPageGroupHelp.ItemLinks.Add(this.barButtonItemHelpTopics);
			this.ribbonPageGroupHelp.ItemLinks.Add(this.barButtonItemAbout);
			this.ribbonPageGroupHelp.Name = "ribbonPageGroupHelp";
			this.ribbonPageGroupHelp.ShowCaptionButton = false;
			this.ribbonPageGroupHelp.Text = "Help";
			this.ribbonPageGroupUpgrade.ItemLinks.Add(this.barButtonItemUpdateCheck);
			this.ribbonPageGroupUpgrade.ItemLinks.Add(this.barButtonItemUpdateLicence);
			this.ribbonPageGroupUpgrade.Name = "ribbonPageGroupUpgrade";
			this.ribbonPageGroupUpgrade.ShowCaptionButton = false;
			this.ribbonPageGroupUpgrade.Text = "Update";
			this.ribbonPageGroupSupport.ItemLinks.Add(this.barButtonItemMetalogixAcademy);
			this.ribbonPageGroupSupport.ItemLinks.Add(this.barButtonItemSupportForum);
			this.ribbonPageGroupSupport.ItemLinks.Add(this.barButtonItemCreateSupportPackage);
			this.ribbonPageGroupSupport.ItemLinks.Add(this.barButtonItemContactSupport);
			this.ribbonPageGroupSupport.Name = "ribbonPageGroupSupport";
			this.ribbonPageGroupSupport.ShowCaptionButton = false;
			this.ribbonPageGroupSupport.Text = "Support";
			this.ribbonPageGroupSocial.ItemLinks.Add(this.barButtonItemFacebook);
			this.ribbonPageGroupSocial.ItemLinks.Add(this.barButtonItemTwitter);
			this.ribbonPageGroupSocial.ItemLinks.Add(this.barButtonItemLinkedIn);
			this.ribbonPageGroupSocial.ItemLinks.Add(this.barButtonItemBlog);
			this.ribbonPageGroupSocial.Name = "ribbonPageGroupSocial";
			this.ribbonPageGroupSocial.ShowCaptionButton = false;
			this.ribbonPageGroupSocial.Text = "Social";
			this.ribbonPageGroupProducts.AllowTextClipping = false;
			this.ribbonPageGroupProducts.ItemLinks.Add(this.barButtonItemReplicator);
			this.ribbonPageGroupProducts.ItemLinks.Add(this.barButtonItemStoragePoint);
			this.ribbonPageGroupProducts.Name = "ribbonPageGroupProducts";
			this.ribbonPageGroupProducts.ShowCaptionButton = false;
			this.ribbonPageGroupProducts.Text = "Other Metalogix Products";
			this._defaultToolTipController.SetAllowHtmlText(this.splitContainerH, DefaultBoolean.Default);
			this.splitContainerH.Dock = DockStyle.Fill;
			this.splitContainerH.Location = new System.Drawing.Point(0, 145);
			this.splitContainerH.Name = "splitContainerH";
			this.splitContainerH.Orientation = Orientation.Horizontal;
			this._defaultToolTipController.SetAllowHtmlText(this.splitContainerH.Panel1, DefaultBoolean.Default);
			this.splitContainerH.Panel1.Controls.Add(this.splitContainerV);
			this._defaultToolTipController.SetAllowHtmlText(this.splitContainerH.Panel2, DefaultBoolean.Default);
			this.splitContainerH.Panel2.Controls.Add(this.w_jobListFullControl);
			this.splitContainerH.Size = new System.Drawing.Size(811, 615);
			this.splitContainerH.SplitterDistance = 357;
			this.splitContainerH.TabIndex = 10;
			this._defaultToolTipController.SetAllowHtmlText(this.splitContainerV, DefaultBoolean.Default);
			this.splitContainerV.BackColor = System.Drawing.Color.White;
			this.splitContainerV.Dock = DockStyle.Fill;
			this.splitContainerV.Location = new System.Drawing.Point(0, 0);
			this.splitContainerV.Name = "splitContainerV";
			this._defaultToolTipController.SetAllowHtmlText(this.splitContainerV.Panel1, DefaultBoolean.Default);
			this._defaultToolTipController.SetAllowHtmlText(this.splitContainerV.Panel2, DefaultBoolean.Default);
			this.splitContainerV.Size = new System.Drawing.Size(811, 357);
			this.splitContainerV.SplitterDistance = 388;
			this.splitContainerV.TabIndex = 0;
			this._defaultToolTipController.SetAllowHtmlText(this.w_jobListFullControl, DefaultBoolean.Default);
			this.w_jobListFullControl.Dock = DockStyle.Fill;
			this.w_jobListFullControl.Location = new System.Drawing.Point(0, 0);
			this.w_jobListFullControl.Name = "w_jobListFullControl";
			this.w_jobListFullControl.ShowLicenseUsage = false;
			this.w_jobListFullControl.Size = new System.Drawing.Size(811, 254);
			this.w_jobListFullControl.TabIndex = 0;
			this.w_jobListFullControl.UseMostRecentlyUsedList = true;
			this._defaultToolTipController.SetAllowHtmlText(this.ActionPaletteControl, DefaultBoolean.Default);
			this.ActionPaletteControl.HostingControl = null;
			this.ActionPaletteControl.LegalType = null;
			this.ActionPaletteControl.Name = "ActionPaletteControl";
			this.ActionPaletteControl.Size = new System.Drawing.Size(61, 4);
			this.ActionPaletteControl.SourceOverride = sourceOverride;
			this.ActionPaletteControl.UseSourceOverride = false;
			this.DefaultLookAndFeel.LookAndFeel.SkinName = "Office 2013";
		    this.DefaultLookAndFeel.LookAndFeel.Style = LookAndFeelStyle.Flat;
		    this.DefaultLookAndFeel.LookAndFeel.UseDefaultLookAndFeel = false;
            this._defaultToolTipController.DefaultController.AutoPopDelay = 15000;
			this._defaultToolTipController.DefaultController.CloseOnClick = DefaultBoolean.True;
			base.AllowFormGlass = DefaultBoolean.False;
			this._defaultToolTipController.SetAllowHtmlText(this, DefaultBoolean.Default);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(811, 760);
			base.Controls.Add(this.splitContainerH);
			base.Controls.Add(this.ribbon);
			base.Name = "UIMainForm";
			this.Ribbon = this.ribbon;
			base.StartPosition = FormStartPosition.CenterScreen;
			base.FormClosed += new FormClosedEventHandler(this.UIMainForm_FormClosed);
			base.Load += new System.EventHandler(this.UIMainFormLoad);
			base.Shown += new System.EventHandler(this.UIMainFormShown);
			((System.ComponentModel.ISupportInitialize)this.ribbon).EndInit();
			((System.ComponentModel.ISupportInitialize)this.applicationMenu1).EndInit();
			((System.ComponentModel.ISupportInitialize)this.imageCollectionSmall).EndInit();
			((System.ComponentModel.ISupportInitialize)this.imageCollectionLarge).EndInit();
			this.splitContainerH.Panel1.ResumeLayout(false);
			this.splitContainerH.Panel2.ResumeLayout(false);
			this.splitContainerH.ResumeLayout(false);
			this.splitContainerV.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
