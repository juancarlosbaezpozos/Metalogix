using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.UI.WinForms.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
    [ControlImage("Metalogix.SharePoint.UI.WinForms.Icons.Migration.MySitesOptions.png")]
    [Metalogix.UI.WinForms.Components.ControlName("MySite Options")]
    [UsesGroupBox(false)]
    public class TCMySitesOptions : ScopableTabbableControl
    {
        private SPLanguage m_selectedLanguage;
        private SPMySiteOptions m_options;
        private List<SPSite> AllMySites;
        private object AllMySitesLock = new object();
        private SPWebTemplateCollection m_targetWebTemplates;
        private SPWebApplicationCollection m_webApps;
        private List<SPSite> m_mySites = new List<SPSite>();
        private object AvailableMySitesLock = new object();
        private CommonSerializableList<SPSite> m_mySitesToExclude = new CommonSerializableList<SPSite>();
        private object MySitesToExcludeLock = new object();
        private CommonSerializableList<string> m_mySitesToExcludeStrings = new CommonSerializableList<string>();
        private AutoResetEvent mySiteFetch = new AutoResetEvent(false);
        private Thread uiLoadingThread;
        private Thread sourceGettingThread;
        private IContainer components;
        private GroupControl gbSelectMySiteData;
        private LabelControl lManagedPath;
        private LabelControl w_lblWebApp;
        private GroupControl gbChooseMySites;
        private LabelControl w_lblToCopy;
        private LabelControl w_lblToIgnore;
        private SimpleButton w_btnAddToCopy;
        private SimpleButton w_btnAddToIgnore;
        private ListView w_lvMySitesToCopy;
        private ListView w_lvMySitesToIgnore;
        private PanelControl w_pLoadingPanel;
        private LabelControl w_lLoading;
        private MarqueeProgressBarControl marqueeBar1;
        private ComboBoxEdit w_cmbPath;
        private ComboBoxEdit w_cmbWebApp;

        private List<SPSite> AvailableMySites
        {
            get => this.m_mySites;
            set
            {
                if (value == null)
                    return;
                this.m_mySites.Clear();
                foreach (SPSite spSite in value)
                    this.m_mySites.Add(spSite);
                this.UpdateMySiteListings();
            }
        }

        private CommonSerializableList<SPSite> MySitesToExclude
        {
            get => this.m_mySitesToExclude;
            set
            {
                if (value == null)
                    return;
                this.m_mySitesToExclude.Clear();
                foreach (SPSite spSite in (SerializableList<SPSite>)value)
                    this.m_mySitesToExclude.Add(spSite);
                this.UpdateMySiteListings();
            }
        }

        public CommonSerializableList<string> MySitesToExcludeStrings
        {
            get => this.m_mySitesToExcludeStrings;
            set
            {
                this.m_mySitesToExcludeStrings = value;
                if (this.Target == null || this.m_mySitesToExcludeStrings == null)
                    return;
                foreach (string sitesToExcludeString in (SerializableList<string>)this.m_mySitesToExcludeStrings)
                {
                    lock (this.AllMySitesLock)
                    {
                        foreach (SPSite allMySite in this.AllMySites)
                        {
                            if (!(sitesToExcludeString != allMySite.DisplayUrl))
                            {
                                lock (this.MySitesToExcludeLock)
                                {
                                    this.MySitesToExclude.Add(allMySite);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public SPMySiteOptions Options
        {
            get => this.m_options;
            set
            {
                this.m_options = value;
                if (this.m_options == null)
                    return;
                this.LoadUIAsync();
            }
        }

        public string SelectedPath
        {
            get
            {
                return this.w_cmbPath.SelectedItem == null ? (string)null : this.w_cmbPath.SelectedItem.ToString();
            }
            set
            {
                if (value == null)
                    return;
                foreach (object obj in (CollectionBase)this.w_cmbPath.Properties.Items)
                {
                    if (!(value != obj.ToString()))
                    {
                        this.w_cmbPath.SelectedItem = obj;
                        break;
                    }
                }
            }
        }

        public SPWebApplication SelectedWebApplication
        {
            get => (SPWebApplication)this.w_cmbWebApp.SelectedItem;
            set
            {
                if (!(value != (SPWebApplication)null))
                    return;
                foreach (object obj in (CollectionBase)this.w_cmbWebApp.Properties.Items)
                {
                    if (!(value.Name != ((ExplorerNode)obj).Name))
                    {
                        this.w_cmbWebApp.SelectedItem = obj;
                        this.UpdatePathList();
                        break;
                    }
                }
            }
        }

        public SPBaseServer Source
        {
            get
            {
                return this.SourceNodes == null || this.SourceNodes.Count == 0 ? (SPBaseServer)null : this.SourceNodes[0] as SPBaseServer;
            }
        }

        public override NodeCollection SourceNodes
        {
            get => base.SourceNodes;
            set
            {
                base.SourceNodes = value;
                this.GetMySitesAsync();
            }
        }

        public SPBaseServer Target
        {
            get
            {
                return this.TargetNodes == null || this.TargetNodes.Count == 0 ? (SPBaseServer)null : this.TargetNodes[0] as SPBaseServer;
            }
        }

        public SPWebTemplateCollection TargetWebTemplates
        {
            get => this.m_targetWebTemplates;
            set => this.m_targetWebTemplates = value;
        }

        public SPWebApplicationCollection WebApplications
        {
            get => this.m_webApps;
            set
            {
                if (value == null)
                    return;
                this.m_webApps = value;
                this.w_cmbWebApp.Properties.Items.Clear();
                foreach (SPWebApplication spWebApplication in (SerializableList<Metalogix.Explorer.Node>)value)
                    this.w_cmbWebApp.Properties.Items.Add((object)spWebApplication);
                this.SelectedWebApplication = (SPWebApplication)this.w_cmbWebApp.Properties.Items[0];
            }
        }

        public TCMySitesOptions() => this.InitializeComponent();

        public override void CancelOperation()
        {
            // ISSUE: reference to a compiler-generated method
            base.CancelOperation();
            if (this.sourceGettingThread != null)
                this.sourceGettingThread.Abort();
            if (this.uiLoadingThread == null)
                return;
            this.uiLoadingThread.Abort();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            // ISSUE: reference to a compiler-generated method
            base.Dispose(disposing);
        }

        private void GetMySites()
        {
            try
            {
                // ISSUE: reference to a compiler-generated method
                this.TriggerAsyncStarted();
                if (this.Source != null)
                {
                    lock (this.AllMySitesLock)
                        this.AllMySites = new List<SPSite>();
                    if (!(this.Source is SPTenantMySiteHost))
                    {
                        foreach (SPSite site in (SerializableList<Metalogix.Explorer.Node>)this.Source.Sites)
                        {
                            try
                            {
                                if (site.IsMySiteTemplate)
                                {
                                    lock (this.AllMySitesLock)
                                        this.AllMySites.Add(site);
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                    else
                    {
                        foreach (SPSite site in (SerializableList<Metalogix.Explorer.Node>)this.Source.Sites)
                        {
                            lock (this.AllMySitesLock)
                                this.AllMySites.Add(site);
                        }
                    }
                }
                this.mySiteFetch.Set();
            }
            catch (ThreadAbortException ex)
            {
            }
        }

        private void GetMySitesAsync()
        {
            this.sourceGettingThread = new Thread(new ThreadStart(this.GetMySites));
            this.sourceGettingThread.Start();
        }

        private void InitializeComponent()
        {
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(TCMySitesOptions));
            this.gbSelectMySiteData = new GroupControl();
            this.w_cmbPath = new ComboBoxEdit();
            this.w_cmbWebApp = new ComboBoxEdit();
            this.lManagedPath = new LabelControl();
            this.w_lblWebApp = new LabelControl();
            this.gbChooseMySites = new GroupControl();
            this.w_lblToCopy = new LabelControl();
            this.w_lblToIgnore = new LabelControl();
            this.w_btnAddToCopy = new SimpleButton();
            this.w_btnAddToIgnore = new SimpleButton();
            this.w_lvMySitesToCopy = new ListView();
            this.w_lvMySitesToIgnore = new ListView();
            this.w_pLoadingPanel = new PanelControl();
            this.marqueeBar1 = new MarqueeProgressBarControl();
            this.w_lLoading = new LabelControl();
            this.gbSelectMySiteData.BeginInit();
            this.gbSelectMySiteData.SuspendLayout();
            this.w_cmbPath.Properties.BeginInit();
            this.w_cmbWebApp.Properties.BeginInit();
            this.gbChooseMySites.BeginInit();
            this.gbChooseMySites.SuspendLayout();
            this.w_pLoadingPanel.BeginInit();
            this.w_pLoadingPanel.SuspendLayout();
            this.marqueeBar1.Properties.BeginInit();
            this.SuspendLayout();
            componentResourceManager.ApplyResources((object)this.gbSelectMySiteData, "gbSelectMySiteData");
            this.gbSelectMySiteData.Controls.Add((Control)this.w_cmbPath);
            this.gbSelectMySiteData.Controls.Add((Control)this.w_cmbWebApp);
            this.gbSelectMySiteData.Controls.Add((Control)this.lManagedPath);
            this.gbSelectMySiteData.Controls.Add((Control)this.w_lblWebApp);
            this.gbSelectMySiteData.Name = "gbSelectMySiteData";
            this.gbSelectMySiteData.ShowCaption = false;
            componentResourceManager.ApplyResources((object)this.w_cmbPath, "w_cmbPath");
            this.w_cmbPath.Name = "w_cmbPath";
            this.w_cmbPath.Properties.Buttons.AddRange(new EditorButton[1]
            {
        new EditorButton()
            });
            componentResourceManager.ApplyResources((object)this.w_cmbWebApp, "w_cmbWebApp");
            this.w_cmbWebApp.Name = "w_cmbWebApp";
            this.w_cmbWebApp.Properties.Buttons.AddRange(new EditorButton[1]
            {
        new EditorButton()
            });
            this.w_cmbWebApp.SelectedIndexChanged += new EventHandler(this.w_cmbWebApp_SelectedIndexChanged);
            componentResourceManager.ApplyResources((object)this.lManagedPath, "lManagedPath");
            this.lManagedPath.Name = "lManagedPath";
            componentResourceManager.ApplyResources((object)this.w_lblWebApp, "w_lblWebApp");
            this.w_lblWebApp.Name = "w_lblWebApp";
            componentResourceManager.ApplyResources((object)this.gbChooseMySites, "gbChooseMySites");
            this.gbChooseMySites.Controls.Add((Control)this.w_lblToCopy);
            this.gbChooseMySites.Controls.Add((Control)this.w_lblToIgnore);
            this.gbChooseMySites.Controls.Add((Control)this.w_btnAddToCopy);
            this.gbChooseMySites.Controls.Add((Control)this.w_btnAddToIgnore);
            this.gbChooseMySites.Controls.Add((Control)this.w_lvMySitesToCopy);
            this.gbChooseMySites.Controls.Add((Control)this.w_lvMySitesToIgnore);
            this.gbChooseMySites.Name = "gbChooseMySites";
            this.gbChooseMySites.ShowCaption = false;
            this.gbChooseMySites.Resize += new EventHandler(this.On_Resize);
            componentResourceManager.ApplyResources((object)this.w_lblToCopy, "w_lblToCopy");
            this.w_lblToCopy.Name = "w_lblToCopy";
            componentResourceManager.ApplyResources((object)this.w_lblToIgnore, "w_lblToIgnore");
            this.w_lblToIgnore.Name = "w_lblToIgnore";
            componentResourceManager.ApplyResources((object)this.w_btnAddToCopy, "w_btnAddToCopy");
            this.w_btnAddToCopy.Name = "w_btnAddToCopy";
            this.w_btnAddToCopy.Click += new EventHandler(this.w_btnAddToIgnore_Click);
            componentResourceManager.ApplyResources((object)this.w_btnAddToIgnore, "w_btnAddToIgnore");
            this.w_btnAddToIgnore.Name = "w_btnAddToIgnore";
            this.w_btnAddToIgnore.Click += new EventHandler(this.w_btnAddToCopy_Click);
            componentResourceManager.ApplyResources((object)this.w_lvMySitesToCopy, "w_lvMySitesToCopy");
            this.w_lvMySitesToCopy.BackColor = Color.White;
            this.w_lvMySitesToCopy.HideSelection = false;
            this.w_lvMySitesToCopy.Name = "w_lvMySitesToCopy";
            this.w_lvMySitesToCopy.UseCompatibleStateImageBehavior = false;
            this.w_lvMySitesToCopy.View = View.List;
            this.w_lvMySitesToIgnore.Activation = ItemActivation.OneClick;
            componentResourceManager.ApplyResources((object)this.w_lvMySitesToIgnore, "w_lvMySitesToIgnore");
            this.w_lvMySitesToIgnore.BackColor = Color.White;
            this.w_lvMySitesToIgnore.HideSelection = false;
            this.w_lvMySitesToIgnore.Name = "w_lvMySitesToIgnore";
            this.w_lvMySitesToIgnore.UseCompatibleStateImageBehavior = false;
            this.w_lvMySitesToIgnore.View = View.List;
            componentResourceManager.ApplyResources((object)this.w_pLoadingPanel, "w_pLoadingPanel");
            this.w_pLoadingPanel.BorderStyle = BorderStyles.NoBorder;
            this.w_pLoadingPanel.Controls.Add((Control)this.marqueeBar1);
            this.w_pLoadingPanel.Controls.Add((Control)this.w_lLoading);
            this.w_pLoadingPanel.Name = "w_pLoadingPanel";
            componentResourceManager.ApplyResources((object)this.marqueeBar1, "marqueeBar1");
            this.marqueeBar1.Name = "marqueeBar1";
            componentResourceManager.ApplyResources((object)this.w_lLoading, "w_lLoading");
            this.w_lLoading.Name = "w_lLoading";
            this.Controls.Add((Control)this.w_pLoadingPanel);
            this.Controls.Add((Control)this.gbSelectMySiteData);
            this.Controls.Add((Control)this.gbChooseMySites);
            this.Name = nameof(TCMySitesOptions);
            componentResourceManager.ApplyResources((object)this, "$this");
            this.gbSelectMySiteData.EndInit();
            this.gbSelectMySiteData.ResumeLayout(false);
            this.gbSelectMySiteData.PerformLayout();
            this.w_cmbPath.Properties.EndInit();
            this.w_cmbWebApp.Properties.EndInit();
            this.gbChooseMySites.EndInit();
            this.gbChooseMySites.ResumeLayout(false);
            this.gbChooseMySites.PerformLayout();
            this.w_pLoadingPanel.EndInit();
            this.w_pLoadingPanel.ResumeLayout(false);
            this.w_pLoadingPanel.PerformLayout();
            this.marqueeBar1.Properties.EndInit();
            this.ResumeLayout(false);
        }

        protected override void LoadUI()
        {
            bool flag1 = false;
            try
            {
                this.SetDialogContentEnabled(false);
                this.mySiteFetch.WaitOne();
                this.MySitesToExcludeStrings = this.m_options.MySitesToExclude;
                try
                {
                    if (this.Target != null)
                        this.SetWebApplications(this.Target.WebApplications);
                }
                catch (Exception ex)
                {
                    GlobalServices.ErrorHandler.HandleException("Target server web applications and languages were unable to be enumerated. Check the migrating user credentials. :", ex);
                    return;
                }
                if (this.Source != null)
                {
                    lock (this.AvailableMySitesLock)
                        this.AvailableMySites.Clear();
                    Dictionary<SPSite, bool> dictionary = new Dictionary<SPSite, bool>();
                    lock (this.MySitesToExcludeLock)
                    {
                        foreach (SPSite key in (SerializableList<SPSite>)this.MySitesToExclude)
                            dictionary.Add(key, true);
                    }
                    lock (this.MySitesToExcludeLock)
                        this.MySitesToExclude.Clear();
                    lock (this.AllMySitesLock)
                    {
                        foreach (SPSite allMySite in this.AllMySites)
                        {
                            if (dictionary.ContainsKey(allMySite))
                            {
                                lock (this.MySitesToExcludeLock)
                                    this.MySitesToExclude.Add(allMySite);
                            }
                            else
                            {
                                lock (this.AvailableMySitesLock)
                                {
                                    if (!this.AvailableMySites.Contains(allMySite))
                                        this.AvailableMySites.Add(allMySite);
                                }
                            }
                        }
                    }
                    this.UpdateMySiteListings();
                    bool flag2 = false;
                    try
                    {
                        this.SetWebAppEnabled(this.Options.WebApplicationName == null || !this.Options.SelfServiceCreateMode && this.Target != null && this.Target.ShowAllSites);
                        if (this.Options.WebApplicationName != null && this.Target.WebApplications[this.Options.WebApplicationName] != null)
                        {
                            this.SetSelectedWebApplication((SPWebApplication)this.Target.WebApplications[this.Options.WebApplicationName]);
                            this.SetSelectedPath(this.Options.Path);
                            flag2 = true;
                        }
                        if (!flag2)
                        {
                            foreach (SPWebApplication webApplication in (SerializableList<Metalogix.Explorer.Node>)this.Target.WebApplications)
                            {
                                if (webApplication.IsMySitePortal)
                                {
                                    this.SetSelectedWebApplication(webApplication);
                                    if (webApplication.Paths.Count > 0)
                                    {
                                        this.SetSelectedPath(webApplication.Paths[0].ToString());
                                        break;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string message = ex.Message;
                    }
                }
                this.SetDialogContentEnabled(true);
            }
            catch (ThreadAbortException ex)
            {
                flag1 = true;
            }
            finally
            {
                if (!flag1)
                {
                    // ISSUE: reference to a compiler-generated method
                    this.TriggerAsyncCompleted();
                }
            }
        }

        private void LoadUIAsync()
        {
            // ISSUE: reference to a compiler-generated method
            this.uiLoadingThread = new Thread(LoadUI);
            this.uiLoadingThread.Start();
        }

        private void MySitesToExcludeToStrings(CommonSerializableList<SPSite> ExcludedMySites)
        {
            if (this.Options.MySitesToExclude != null)
                this.Options.MySitesToExclude.Clear();
            foreach (ExplorerNode excludedMySite in (SerializableList<SPSite>)ExcludedMySites)
                this.Options.MySitesToExclude.Add(excludedMySite.DisplayUrl);
        }

        private void On_Resize(object sender, EventArgs e)
        {
            int num1 = this.gbChooseMySites.Width / 2 - 30;
            int num2 = 50;
            this.w_lvMySitesToIgnore.Width = num1;
            this.w_btnAddToIgnore.Location = new Point(this.w_lvMySitesToIgnore.Location.X + 5 + this.w_lvMySitesToIgnore.Width, this.w_lvMySitesToIgnore.Location.Y + this.w_lvMySitesToIgnore.Height / 2 - 24);
            this.w_btnAddToCopy.Location = new Point(this.w_lvMySitesToIgnore.Location.X + 5 + this.w_lvMySitesToIgnore.Width, this.w_lvMySitesToIgnore.Location.Y + this.w_lvMySitesToIgnore.Height / 2 + 3);
            ListView wLvMySitesToCopy = this.w_lvMySitesToCopy;
            Point location1 = this.w_lvMySitesToIgnore.Location;
            Point location2 = this.w_lvMySitesToIgnore.Location;
            wLvMySitesToCopy.Location = new Point(location1.X + this.w_lvMySitesToIgnore.Width + num2, location2.Y);
            this.w_lblToIgnore.Location = new Point(this.w_lvMySitesToCopy.Location.X, this.w_lblToIgnore.Location.Y);
            this.w_lvMySitesToCopy.Width = num1;
        }

        public override bool SaveUI()
        {
            this.Options.WebApplicationName = this.w_cmbWebApp.Text;
            this.Options.Path = this.w_cmbPath.Text;
            this.Options.URL = this.w_cmbPath.Text;
            lock (this.MySitesToExcludeLock)
                this.MySitesToExcludeToStrings(this.MySitesToExclude);
            lock (this.AvailableMySitesLock)
                this.Options.MySitesToInclude = this.AvailableMySites;
            return true;
        }

        private void SetDialogContentEnabled(bool enabled)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((Delegate)new TCMySitesOptions.SetDialogContentEnabledDelegate(this.SetDialogContentEnabled), (object)enabled);
            }
            else
            {
                this.gbSelectMySiteData.Enabled = enabled;
                this.gbChooseMySites.Enabled = enabled;
                bool visible = this.w_pLoadingPanel.Visible;
                this.w_pLoadingPanel.Visible = !enabled;
                this.w_pLoadingPanel.Enabled = !enabled;
                if (!visible || this.w_pLoadingPanel.Visible)
                    return;
                this.gbChooseMySites.Height += this.w_pLoadingPanel.Height;
            }
        }

        private void SetLoadingPanelEnabled(bool enabled)
        {
            if (!this.InvokeRequired)
                this.w_pLoadingPanel.Enabled = enabled;
            else
                this.BeginInvoke((Delegate)new TCMySitesOptions.SetDialogContentEnabledDelegate(this.SetLoadingPanelVisibility), (object)enabled);
        }

        private void SetLoadingPanelVisibility(bool visible)
        {
            if (!this.InvokeRequired)
                this.w_pLoadingPanel.Visible = visible;
            else
                this.BeginInvoke((Delegate)new TCMySitesOptions.SetDialogContentEnabledDelegate(this.SetLoadingPanelVisibility), (object)visible);
        }

        private void SetSelectedPath(string selectedPath)
        {
            if (!this.InvokeRequired)
                this.SelectedPath = selectedPath;
            else
                this.BeginInvoke((Delegate)new TCMySitesOptions.SetSelectedPathDelegate(this.SetSelectedPath), (object)selectedPath);
        }

        private void SetSelectedWebApplication(SPWebApplication webApplication)
        {
            if (!this.InvokeRequired)
                this.SelectedWebApplication = webApplication;
            else
                this.BeginInvoke((Delegate)new TCMySitesOptions.SetSelectedWebApplicationDelegate(this.SetSelectedWebApplication), (object)webApplication);
        }

        private void SetWebAppEnabled(bool enabled)
        {
            if (!this.InvokeRequired)
                this.w_cmbWebApp.Enabled = enabled;
            else
                this.BeginInvoke((Delegate)new TCMySitesOptions.SetWebAppEnabledDelegate(this.SetWebAppEnabled), (object)enabled);
        }

        private void SetWebApplications(SPWebApplicationCollection webApplications)
        {
            if (!this.InvokeRequired)
                this.WebApplications = webApplications;
            else
                this.BeginInvoke((Delegate)new TCMySitesOptions.SetWebApplicationsDelegate(this.SetWebApplications), (object)webApplications);
        }

        private void UISort()
        {
            this.w_lvMySitesToIgnore.Sort();
            this.w_lvMySitesToCopy.Sort();
        }

        private void UpdateLanguageData()
        {
            this.TargetWebTemplates = this.m_selectedLanguage.Templates;
        }

        private void UpdateMySiteListings()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((Delegate)new TCMySitesOptions.UpdateMySiteListingsDelegate(this.UpdateMySiteListings), (object[])null);
            }
            else
            {
                this.w_lvMySitesToCopy.Items.Clear();
                this.w_lvMySitesToIgnore.Items.Clear();
                lock (this.MySitesToExcludeLock)
                {
                    foreach (SPSite site in (SerializableList<SPSite>)this.MySitesToExclude)
                        this.w_lvMySitesToIgnore.Items.Add((ListViewItem)new TCMySitesOptions.SPSiteListViewItem(site));
                }
                lock (this.AvailableMySitesLock)
                {
                    foreach (SPSite availableMySite in this.AvailableMySites)
                        this.w_lvMySitesToCopy.Items.Add((ListViewItem)new TCMySitesOptions.SPSiteListViewItem(availableMySite));
                }
                this.UISort();
            }
        }

        private void UpdatePathList()
        {
            this.w_cmbPath.Properties.Items.Clear();
            foreach (SPPath path in this.SelectedWebApplication.Paths)
            {
                if (path.IsWildcard)
                    this.w_cmbPath.Properties.Items.Add((object)path);
            }
            this.w_cmbPath.SelectedIndex = 0;
        }

        private void w_btnAddToCopy_Click(object sender, EventArgs e)
        {
            foreach (TCMySitesOptions.SPSiteListViewItem selectedItem in this.w_lvMySitesToIgnore.SelectedItems)
            {
                lock (this.AvailableMySitesLock)
                    this.AvailableMySites.Add(selectedItem.MySite);
                lock (this.MySitesToExcludeLock)
                    this.MySitesToExclude.Remove(selectedItem.MySite);
                this.w_lvMySitesToIgnore.Items.Remove((ListViewItem)selectedItem);
                this.w_lvMySitesToCopy.Items.Add((ListViewItem)selectedItem);
            }
            this.UISort();
        }

        private void w_btnAddToIgnore_Click(object sender, EventArgs e)
        {
            foreach (TCMySitesOptions.SPSiteListViewItem selectedItem in this.w_lvMySitesToCopy.SelectedItems)
            {
                lock (this.MySitesToExcludeLock)
                    this.MySitesToExclude.Add(selectedItem.MySite);
                lock (this.AvailableMySitesLock)
                    this.AvailableMySites.Remove(selectedItem.MySite);
                this.w_lvMySitesToCopy.Items.Remove((ListViewItem)selectedItem);
                this.w_lvMySitesToIgnore.Items.Add((ListViewItem)selectedItem);
            }
            this.UISort();
        }

        private void w_cmbWebApp_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.UpdatePathList();
        }

        private delegate void SetDialogContentEnabledDelegate(bool enabled);

        private delegate void SetSelectedPathDelegate(string selectedPath);

        private delegate void SetSelectedWebApplicationDelegate(SPWebApplication webApplication);

        private delegate void SetWebAppEnabledDelegate(bool enabled);

        private delegate void SetWebApplicationsDelegate(SPWebApplicationCollection webApplications);

        private class SPSiteListViewItem : ListViewItem
        {
            private SPSite m_spSite;

            public SPSite MySite
            {
                get => this.m_spSite;
                set => this.m_spSite = value;
            }

            public SPSiteListViewItem(SPSite site)
            {
                this.MySite = site;
                this.Text = site.ServerRelativeUrl;
            }
        }

        private delegate void UpdateMySiteListingsDelegate();
    }
}
