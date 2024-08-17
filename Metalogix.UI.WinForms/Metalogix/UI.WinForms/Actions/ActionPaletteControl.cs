using Metalogix;
using Metalogix.Actions;
using Metalogix.Core.Properties;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.Interfaces;
using Metalogix.Jobs;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Jobs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace Metalogix.UI.WinForms.Actions
{
    public class ActionPaletteControl : ContextMenuStrip
    {
        private Type _legalType;
        private IContainer components;
        private ActionCollection m_actions;
        private Control m_hostingControl;
        private IXMLAbleList m_selectedSources;
        private IXMLAbleList m_selectedTargets;
        protected List<Keys> m_UsedShortcutKeys;

        public static event JobLaunchingHandler JobLaunching;

        private IXMLAbleList _sourceOverride = new CommonSerializableList<object>();

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IXMLAbleList SourceOverride
        {
            get
            {
                return this._sourceOverride;
            }
            set
            {
                if (value == null)
                {
                    this._sourceOverride = new CommonSerializableList<object>();
                }
                this._sourceOverride = value;
            }
        }

        public bool UseSourceOverride
        {
            get;
            set;
        }

        public ActionPaletteControl()
        {
            this.m_actions = new ActionCollection();
            this.m_selectedSources = new XMLAbleList();
            this.m_selectedTargets = new XMLAbleList();
            this.m_UsedShortcutKeys = new List<Keys>();
            this.InitializeComponent();
            this.LoadActions();
        }

        public ActionPaletteControl(Type typeRestriction)
        {
            this.InitializeComponent();
            this._legalType = typeRestriction;
            this.LoadActions();
            ActionLicenseProvider.Instance.ActionLicenseProviderUpdated += new ActionLicenseProviderUpdatedHandler(this.On_ActionLicenseProviderUpdated);
        }

        public ActionPaletteControl(IContainer container, Type typeRestriction = null)
        {
            this.m_actions = new ActionCollection();
            this.m_selectedSources = new XMLAbleList();
            this.m_selectedTargets = new XMLAbleList();
            this.m_UsedShortcutKeys = new List<Keys>();
            container.Add(this);
            this.InitializeComponent();
            this._legalType = typeRestriction;
            this.LoadActions();
            ActionLicenseProvider.Instance.ActionLicenseProviderUpdated += new ActionLicenseProviderUpdatedHandler(this.On_ActionLicenseProviderUpdated);
        }

        private void AddMenus(ActionContext actionContext, ToolStripItemCollection parentMenuCollection, ActionPaletteMenu parentMenuSource)
        {
            parentMenuSource.SubGroups.Sort();
            for (int i = 0; i < parentMenuSource.SubGroups.Count; i++)
            {
                ActionPaletteGroup group = parentMenuSource.SubGroups[i];
                if (group.SubMenus.Count != 0)
                {
                    if (((i > 0) && (parentMenuCollection.Count > 0)) && !(parentMenuCollection[parentMenuCollection.Count - 1] is ToolStripSeparator))
                    {
                        parentMenuCollection.Add("-");
                    }
                    group.SubMenus.Sort();
                    for (int j = 0; j < group.SubMenus.Count; j++)
                    {
                        ActionPaletteMenu menu = group.SubMenus[j];
                        ToolStripMenuItem item = new ToolStripMenuItem
                        {
                            Text = menu.MenuText
                        };
                        int index = item.Text.IndexOf(':');
                        if (index >= 0)
                        {
                            item.Text = item.Text.Substring(index + 1).Trim();
                        }
                        Metalogix.Actions.Action action = menu.Action;
                        if (action != null)
                        {
                            Image image = action.GetImage(actionContext);
                            item.Image = image;
                            item.Tag = action;
                            item.Enabled = menu.Action.EnabledOn(this.Source, this.Target);
                            item.Click += new EventHandler(this.On_ActionClick);
                            Keys none = Keys.None;
                            switch (action.ShortcutKeys)
                            {
                                case ShortcutAction.Copy:
                                    none = Keys.Control | Keys.C;
                                    break;

                                case ShortcutAction.Delete:
                                    none = Keys.Delete;
                                    break;

                                case ShortcutAction.Paste:
                                    none = Keys.Control | Keys.V;
                                    break;

                                case ShortcutAction.Refresh:
                                    none = Keys.F5;
                                    break;

                                case ShortcutAction.Properties:
                                    none = Keys.Control | Keys.P;
                                    break;

                                case ShortcutAction.Rename:
                                    none = Keys.F2;
                                    break;

                                case ShortcutAction.Find:
                                    none = Keys.Control | Keys.F;
                                    break;

                                case ShortcutAction.FindAndReplace:
                                    none = Keys.Control | Keys.H;
                                    break;

                                case ShortcutAction.Save:
                                    none = Keys.Control | Keys.S;
                                    break;

                                case ShortcutAction.SaveAs:
                                    none = Keys.Control | Keys.Shift | Keys.S;
                                    break;
                            }
                            if (action.UseShortcutKeys && !this.m_UsedShortcutKeys.Contains(none))
                            {
                                item.ShortcutKeys = none;
                                this.m_UsedShortcutKeys.Add(item.ShortcutKeys);
                            }
                        }
                        parentMenuCollection.Add(item);
                        if ((action != null) && action.Batchable)
                        {
                            string batchableName = action.GetBatchableName(actionContext);
                            ToolStripMenuItem item2 = new BatchToolStripMenuItem
                            {
                                Text = batchableName,
                                Tag = action,
                                Enabled = item.Enabled
                            };
                            item2.Click += new EventHandler(this.On_ActionClick);
                            parentMenuCollection.Add(item2);
                        }
                        if (menu.SubGroups.Count > 0)
                        {
                            this.AddMenus(actionContext, item.DropDownItems, menu);
                            if (item.DropDownItems.Count == 0)
                            {
                                parentMenuCollection.Remove(item);
                                continue;
                            }
                            bool flag = false;
                            foreach (ToolStripMenuItem item3 in item.DropDownItems)
                            {
                                if (item3.Enabled)
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            item.Enabled = flag;
                        }
                    }
                }
            }
        }

        public void AddMenus(ToolStripItemCollection parentMenuCollection, ActionCollection actions, bool bFlat)
        {
            ActionContext context = new ActionContext(this.Source, this.Target);
            this.AddMenus(parentMenuCollection, actions, context, bFlat);
        }

        public void AddMenus(ToolStripItemCollection parentMenuCollection, ActionCollection actions, ActionContext context, bool bFlat)
        {
            ActionPaletteMenu parentMenuSource = new ActionPaletteMenu(context, actions, bFlat);
            this.AddMenus(context, parentMenuCollection, parentMenuSource);
        }

        public virtual void BuildActionMenu()
        {
            try
            {
                base.SuspendLayout();
                this.Items.Clear();
                this.m_UsedShortcutKeys.Clear();
                ActionCollection applicableActions = this.Actions.GetApplicableActions(this.Source, this.Target);
                this.AddMenus(this.Items, applicableActions, false);
            }
            catch (Exception)
            {
            }
            finally
            {
                base.ResumeLayout();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void FireJobLaunching(Job job)
        {
            if (JobLaunching != null)
            {
                JobLaunching(job);
            }
        }

        private void InitializeComponent()
        {
            this.components = new Container();
        }

        private void LoadActions()
        {
            if (Actions != null)
            {
                if (Actions.Count > 0)
                {
                    this.Actions.Clear();
                }

                this.Actions.AddRange(ActionCollection.AvailableActions.ToArray());
            }
        }

        private void On_ActionClick(object sender, EventArgs e)
        {
            string str;
            IXMLAbleList nodeCollection;
            ToolStripItem toolStripItem = (ToolStripItem)sender;
            bool flag = toolStripItem is ActionPaletteControl.BatchToolStripMenuItem;
            Metalogix.Actions.Action tag = (Metalogix.Actions.Action)toolStripItem.Tag;
            Metalogix.Actions.Action action = (Metalogix.Actions.Action)Activator.CreateInstance(tag.GetType());
            if (action.SourceCardinality == Cardinality.Zero)
            {
                nodeCollection = new NodeCollection();
            }
            else
            {
                nodeCollection = this.Source;
            }
            IXMLAbleList xMLAbleLists = nodeCollection;
            IXMLAbleList target = this.Target;
            if (action.GetCollectionsViolateSourceTargetRestrictions(xMLAbleLists, target, out str))
            {
                FlatXtraMessageBox.Show(str, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            bool flag1 = (UIConfigurationVariables.ShowAdvanced ? false : ApplicationData.GetProductName().Equals("Content Matrix Console - SharePoint Edition", StringComparison.InvariantCultureIgnoreCase));
            if (!action.IsRunAsync)
            {
                try
                {
                    if (!flag1 && action.UseStickySettings && UIConfigurationVariables.EnableStickySettings)
                    {
                        ActionOptionsProvider.SetOptionsToDefault(action);
                    }
                    ConfigurationResult configurationResult = action.Configure(ref xMLAbleLists, ref target);
                    if (configurationResult != ConfigurationResult.Cancel)
                    {
                        if (action.UseStickySettings)
                        {
                            ActionOptionsProvider.UpdateDefaultOptionsXml(action);
                        }
                        System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
                        if (this.HostingControl != null)
                        {
                            this.HostingControl.FindForm().Refresh();
                        }
                        if (configurationResult == ConfigurationResult.Run)
                        {
                            action.Run(xMLAbleLists, target);
                        }
                        System.Windows.Forms.Cursor.Current = Cursors.Default;
                    }
                }
                catch (Exception exception1)
                {
                    Exception exception = exception1;
                    System.Windows.Forms.Cursor.Current = Cursors.Default;
                    GlobalServices.ErrorHandler.HandleException(exception);
                }
            }
            else
            {
                try
                {
                    if (!flag1 && action.UseStickySettings && UIConfigurationVariables.EnableStickySettings)
                    {
                        ActionOptionsProvider.SetOptionsToDefault(action);
                    }
                    ConfigurationResult configurationResult1 = action.Configure(ref xMLAbleLists, ref target);
                    if (configurationResult1 != ConfigurationResult.Cancel)
                    {
                        if (this.HostingControl != null)
                        {
                            this.HostingControl.FindForm().Refresh();
                        }
                        if (action.UseStickySettings)
                        {
                            ActionOptionsProvider.UpdateDefaultOptionsXml(action);
                        }
                        if (!action.LaunchAsJob)
                        {
                            PleaseWaitDialog.ShowWaitDialog(action.DisplayName, (BackgroundWorker worker) => action.Run(xMLAbleLists, target), false);
                        }
                        else
                        {
                            Job job = new Job(action, xMLAbleLists, target);
                            if (flag || configurationResult1 != ConfigurationResult.Run)
                            {
                                JobUIHelper.SaveJob(job);
                            }
                            else
                            {
                                JobUIHelper.RunJob(job, this.HostingControl.TopLevelControl as Form, xMLAbleLists, target);
                            }
                        }
                    }
                }
                catch (Exception exception3)
                {
                    Exception exception2 = exception3;
                    System.Windows.Forms.Cursor.Current = Cursors.Default;
                    GlobalServices.ErrorHandler.HandleException(exception2);
                }
            }
        }



        private void On_ActionLicenseProviderUpdated()
        {
            ActionCollection.RefreshAvailableActions();
            this.LoadActions();
        }

        protected override void OnOpening(CancelEventArgs e)
        {
            this.m_hostingControl = base.SourceControl;
            this.BuildActionMenu();
            if (this.Items.Count == 0)
            {
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            return base.ProcessDialogKey(keyData);
        }

        public void Show(Control c, Point p, IXMLAbleList sources, IXMLAbleList targets)
        {
            this.m_selectedSources = sources;
            this.m_selectedTargets = targets;
            base.Show(c, p);
        }

        public static void ActionClick(Metalogix.Actions.Action action, IXMLAbleList source, IXMLAbleList target)
        {
            try
            {
                if ((UIConfigurationVariables.ShowAdvanced ? true : !ApplicationData.GetProductName().Equals("Content Matrix Console - SharePoint Edition", StringComparison.InvariantCultureIgnoreCase)) && action.UseStickySettings && UIConfigurationVariables.EnableStickySettings)
                {
                    ActionOptionsProvider.SetOptionsToDefault(action);
                }
                ConfigurationResult configurationResult = action.Configure(ref source, ref target);
                if (configurationResult != ConfigurationResult.Cancel)
                {
                    if (action.UseStickySettings)
                    {
                        ActionOptionsProvider.UpdateDefaultOptionsXml(action);
                    }
                    if (action.IsRunAsync)
                    {
                        PleaseWaitDialog.ShowWaitDialog(action.DisplayName, (BackgroundWorker worker) => action.Run(source, target), false);
                    }
                    else if (configurationResult == ConfigurationResult.Run)
                    {
                        System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
                        action.Run(source, target);
                        System.Windows.Forms.Cursor.Current = Cursors.Default;
                    }
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                System.Windows.Forms.Cursor.Current = Cursors.Default;
                GlobalServices.ErrorHandler.HandleException(exception);
            }
        }

        public ActionCollection Actions
        {
            get
            {
                return this.m_actions;
            }
        }

        public Control HostingControl
        {
            get
            {
                return this.m_hostingControl;
            }
            set
            {
                this.m_hostingControl = value;
            }
        }

        protected IXMLAbleList Source
        {
            get
            {
                try
                {
                    IXMLAbleList dataObject = (IXMLAbleList)ClipBoard.GetDataObject();
                    if (dataObject != null)
                    {
                        return dataObject;
                    }
                }
                catch (Exception)
                {
                }
                return new XMLAbleList();
            }
        }

        protected virtual IXMLAbleList Target
        {
            get
            {
                if ((this.HostingControl != null) && (this.HostingControl.GetType().GetInterface(typeof(IHasSelectableObjects).FullName) != null))
                {
                    IXMLAbleList selectedObjects = ((IHasSelectableObjects)this.HostingControl).SelectedObjects;
                    if (selectedObjects != null)
                    {
                        return selectedObjects;
                    }
                }
                return new XMLAbleList();
            }
        }

        private class BatchToolStripMenuItem : ToolStripMenuItem
        {
            public bool IsBatchAction
            {
                get
                {
                    return true;
                }
            }
        }

        public Type LegalType
        {
            get
            {
                return this._legalType;
            }
            set
            {
                this._legalType = value;
                this.LoadActions();
            }
        }

        public delegate void JobLaunchingHandler(Job job);
    }
}