using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Metalogix.Explorer;

namespace Metalogix.Permissions
{
    public class MappableRoleControl : UserControl
    {
        private ISecurableObject[] m_sources;

        private ISecurableObject[] m_targets;

        private MappableRoleRegistry m_mappings;

        private IComparer<Role> m_comparer;

        private IContainer components;

        private SplitContainer splitContainerInner;

        private Button w_buttonMap;

        private Button w_buttonUnmap;

        private Label w_labelSource;

        private ListView w_listViewSourceRoles;

        private Label w_labelTarget;

        private ListView w_listViewTargetRoles;

        private ListView w_listViewMappings;

        private Label label3;

        private SplitContainer splitContainerOuter;

        private ColumnHeader columnHeader1;

        private ColumnHeader columnHeader2;

        private ColumnHeader columnHeader4;

        private ColumnHeader columnHeader5;

        private ComboBox w_comboBoxTargets;

        private ComboBox w_comboBoxSources;

        private Button w_buttonAutoMap;

        public IComparer<Role> AutoMapComparer
        {
            get
            {
                return m_comparer;
            }
            set
            {
                m_comparer = value;
                w_buttonAutoMap.Enabled = m_comparer != null;
            }
        }

        public MappableRoleRegistry RoleMappings
        {
            get
            {
                return m_mappings;
            }
            set
            {
                m_mappings = value;
            }
        }

        public ISecurableObject[] Sources
        {
            get
            {
                return m_sources;
            }
            set
            {
                if (m_mappings == null)
                {
                    throw new ArgumentNullException("Mapping Registry cannot be null.");
                }
                m_sources = value;
                if (m_sources != null)
                {
                    FillSecurables(m_sources, w_comboBoxSources);
                }
            }
        }

        public ISecurableObject[] Targets
        {
            get
            {
                return m_targets;
            }
            set
            {
                if (m_mappings == null)
                {
                    throw new ArgumentNullException("Mapping Registry cannot be null.");
                }
                m_targets = value;
                if (m_targets != null)
                {
                    FillSecurables(m_targets, w_comboBoxTargets);
                }
            }
        }

        public MappableRoleControl()
        {
            InitializeComponent();
            RoleMappings = new MappableRoleRegistry();
        }

        public MappableRoleControl(string sourceLabel, string targetLabel)
        {
            InitializeComponent();
            w_labelSource.Text = sourceLabel;
            w_labelTarget.Text = targetLabel;
            RoleMappings = new MappableRoleRegistry();
        }

        public MappableRoleControl(string sourceLabel, string targetLabel, ISecurableObject[] sources, ISecurableObject[] targets, MappableRoleRegistry mappings, IComparer<Role> automapper)
        {
            InitializeComponent();
            w_labelSource.Text = sourceLabel;
            w_labelTarget.Text = targetLabel;
            RoleMappings = mappings;
            Sources = sources;
            Targets = targets;
            AutoMapComparer = automapper;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void FillMappings(ISecurableObject source)
        {
            try
            {
                w_listViewMappings.BeginUpdate();
                w_listViewMappings.Items.Clear();
                if (source.Roles == null)
                {
                    return;
                }
                foreach (Role role in source.Roles)
                {
                    if (role is MappableRole && (role as MappableRole).TargetRole != null)
                    {
                        MapRoleUI(new RoleListViewItem(role));
                    }
                }
            }
            catch (Exception exception)
            {
                GlobalServices.ErrorHandler.HandleException(exception);
            }
            finally
            {
                w_listViewMappings.EndUpdate();
            }
        }

        private void FillRoles(ISecurableObject source, ListView target)
        {
            try
            {
                if (source == null)
                {
                    return;
                }
                target.BeginUpdate();
                target.Items.Clear();
                if (source.Principals == null)
                {
                    return;
                }
                foreach (Role role in source.Roles)
                {
                    if (role is MappableRole || target != w_listViewSourceRoles)
                    {
                        RoleListViewItem roleListViewItem = new RoleListViewItem(role);
                        if (!target.Items.Contains(roleListViewItem))
                        {
                            target.Items.Add(roleListViewItem);
                        }
                    }
                }
                target.Sorting = SortOrder.Ascending;
            }
            catch (Exception exception)
            {
                GlobalServices.ErrorHandler.HandleException(exception);
            }
            finally
            {
                target.EndUpdate();
            }
        }

        private void FillSecurables(IEnumerable<ISecurableObject> sources, ComboBox target)
        {
            try
            {
                target.BeginUpdate();
                target.Items.Clear();
                if (sources == null)
                {
                    return;
                }
                foreach (ISecurableObject source in sources)
                {
                    target.Items.Add(new SecurableComboBoxItem(source));
                }
                if (target.Items.Count > 0)
                {
                    target.SelectedIndex = 0;
                    w_comboBox_SelectionChangeCommitted(target, new EventArgs());
                }
            }
            catch (Exception exception)
            {
                GlobalServices.ErrorHandler.HandleException(exception);
            }
            finally
            {
                target.EndUpdate();
            }
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.Permissions.MappableRoleControl));
            this.splitContainerInner = new System.Windows.Forms.SplitContainer();
            this.w_comboBoxSources = new System.Windows.Forms.ComboBox();
            this.w_labelSource = new System.Windows.Forms.Label();
            this.w_listViewSourceRoles = new System.Windows.Forms.ListView();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.w_buttonAutoMap = new System.Windows.Forms.Button();
            this.w_comboBoxTargets = new System.Windows.Forms.ComboBox();
            this.w_labelTarget = new System.Windows.Forms.Label();
            this.w_listViewTargetRoles = new System.Windows.Forms.ListView();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.w_buttonMap = new System.Windows.Forms.Button();
            this.w_buttonUnmap = new System.Windows.Forms.Button();
            this.w_listViewMappings = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.label3 = new System.Windows.Forms.Label();
            this.splitContainerOuter = new System.Windows.Forms.SplitContainer();
            this.splitContainerInner.Panel1.SuspendLayout();
            this.splitContainerInner.Panel2.SuspendLayout();
            this.splitContainerInner.SuspendLayout();
            this.splitContainerOuter.Panel1.SuspendLayout();
            this.splitContainerOuter.Panel2.SuspendLayout();
            this.splitContainerOuter.SuspendLayout();
            base.SuspendLayout();
            componentResourceManager.ApplyResources(this.splitContainerInner, "splitContainerInner");
            this.splitContainerInner.Name = "splitContainerInner";
            this.splitContainerInner.Panel1.Controls.Add(this.w_comboBoxSources);
            this.splitContainerInner.Panel1.Controls.Add(this.w_labelSource);
            this.splitContainerInner.Panel1.Controls.Add(this.w_listViewSourceRoles);
            this.splitContainerInner.Panel2.Controls.Add(this.w_buttonAutoMap);
            this.splitContainerInner.Panel2.Controls.Add(this.w_comboBoxTargets);
            this.splitContainerInner.Panel2.Controls.Add(this.w_labelTarget);
            this.splitContainerInner.Panel2.Controls.Add(this.w_listViewTargetRoles);
            this.splitContainerInner.Panel2.Controls.Add(this.w_buttonMap);
            componentResourceManager.ApplyResources(this.w_comboBoxSources, "w_comboBoxSources");
            this.w_comboBoxSources.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.w_comboBoxSources.FormattingEnabled = true;
            this.w_comboBoxSources.Name = "w_comboBoxSources";
            this.w_comboBoxSources.SelectionChangeCommitted += new System.EventHandler(w_comboBox_SelectionChangeCommitted);
            componentResourceManager.ApplyResources(this.w_labelSource, "w_labelSource");
            this.w_labelSource.Name = "w_labelSource";
            componentResourceManager.ApplyResources(this.w_listViewSourceRoles, "w_listViewSourceRoles");
            this.w_listViewSourceRoles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1] { this.columnHeader4 });
            this.w_listViewSourceRoles.FullRowSelect = true;
            this.w_listViewSourceRoles.HideSelection = false;
            this.w_listViewSourceRoles.MultiSelect = false;
            this.w_listViewSourceRoles.Name = "w_listViewSourceRoles";
            this.w_listViewSourceRoles.UseCompatibleStateImageBehavior = false;
            this.w_listViewSourceRoles.View = System.Windows.Forms.View.Details;
            componentResourceManager.ApplyResources(this.columnHeader4, "columnHeader4");
            componentResourceManager.ApplyResources(this.w_buttonAutoMap, "w_buttonAutoMap");
            this.w_buttonAutoMap.Name = "w_buttonAutoMap";
            this.w_buttonAutoMap.UseVisualStyleBackColor = true;
            this.w_buttonAutoMap.Click += new System.EventHandler(w_buttonAutoMap_Click);
            componentResourceManager.ApplyResources(this.w_comboBoxTargets, "w_comboBoxTargets");
            this.w_comboBoxTargets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.w_comboBoxTargets.FormattingEnabled = true;
            this.w_comboBoxTargets.Name = "w_comboBoxTargets";
            this.w_comboBoxTargets.SelectionChangeCommitted += new System.EventHandler(w_comboBox_SelectionChangeCommitted);
            componentResourceManager.ApplyResources(this.w_labelTarget, "w_labelTarget");
            this.w_labelTarget.Name = "w_labelTarget";
            componentResourceManager.ApplyResources(this.w_listViewTargetRoles, "w_listViewTargetRoles");
            this.w_listViewTargetRoles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1] { this.columnHeader5 });
            this.w_listViewTargetRoles.FullRowSelect = true;
            this.w_listViewTargetRoles.HideSelection = false;
            this.w_listViewTargetRoles.MultiSelect = false;
            this.w_listViewTargetRoles.Name = "w_listViewTargetRoles";
            this.w_listViewTargetRoles.UseCompatibleStateImageBehavior = false;
            this.w_listViewTargetRoles.View = System.Windows.Forms.View.Details;
            componentResourceManager.ApplyResources(this.columnHeader5, "columnHeader5");
            componentResourceManager.ApplyResources(this.w_buttonMap, "w_buttonMap");
            this.w_buttonMap.Name = "w_buttonMap";
            this.w_buttonMap.UseVisualStyleBackColor = true;
            this.w_buttonMap.Click += new System.EventHandler(w_buttonMap_Click);
            componentResourceManager.ApplyResources(this.w_buttonUnmap, "w_buttonUnmap");
            this.w_buttonUnmap.Name = "w_buttonUnmap";
            this.w_buttonUnmap.UseVisualStyleBackColor = true;
            this.w_buttonUnmap.Click += new System.EventHandler(w_buttonUnmap_Click);
            componentResourceManager.ApplyResources(this.w_listViewMappings, "w_listViewMappings");
            System.Windows.Forms.ListView.ColumnHeaderCollection columns = this.w_listViewMappings.Columns;
            System.Windows.Forms.ColumnHeader[] columnHeaderArray = new System.Windows.Forms.ColumnHeader[2] { this.columnHeader1, this.columnHeader2 };
            columns.AddRange(columnHeaderArray);
            this.w_listViewMappings.FullRowSelect = true;
            this.w_listViewMappings.HideSelection = false;
            this.w_listViewMappings.MultiSelect = false;
            this.w_listViewMappings.Name = "w_listViewMappings";
            this.w_listViewMappings.UseCompatibleStateImageBehavior = false;
            this.w_listViewMappings.View = System.Windows.Forms.View.Details;
            componentResourceManager.ApplyResources(this.columnHeader1, "columnHeader1");
            componentResourceManager.ApplyResources(this.columnHeader2, "columnHeader2");
            componentResourceManager.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            componentResourceManager.ApplyResources(this.splitContainerOuter, "splitContainerOuter");
            this.splitContainerOuter.Name = "splitContainerOuter";
            this.splitContainerOuter.Panel1.Controls.Add(this.splitContainerInner);
            this.splitContainerOuter.Panel2.Controls.Add(this.w_listViewMappings);
            this.splitContainerOuter.Panel2.Controls.Add(this.label3);
            this.splitContainerOuter.Panel2.Controls.Add(this.w_buttonUnmap);
            componentResourceManager.ApplyResources(this, "$this");
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(this.splitContainerOuter);
            base.Name = "MappableRoleControl";
            this.splitContainerInner.Panel1.ResumeLayout(false);
            this.splitContainerInner.Panel1.PerformLayout();
            this.splitContainerInner.Panel2.ResumeLayout(false);
            this.splitContainerInner.Panel2.PerformLayout();
            this.splitContainerInner.ResumeLayout(false);
            this.splitContainerOuter.Panel1.ResumeLayout(false);
            this.splitContainerOuter.Panel2.ResumeLayout(false);
            this.splitContainerOuter.Panel2.PerformLayout();
            this.splitContainerOuter.ResumeLayout(false);
            base.ResumeLayout(false);
        }

        private void Instance_OnMappingsChanged(object sender, CollectionChangeEventArgs e)
        {
            SecurableComboBoxItem selectedItem = w_comboBoxSources.SelectedItem as SecurableComboBoxItem;
            SecurableComboBoxItem securableComboBoxItem = w_comboBoxTargets.SelectedItem as SecurableComboBoxItem;
            if (selectedItem == null || securableComboBoxItem == null)
            {
                return;
            }
            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    if (sender.Equals(MappableRoleRegistry.GenerateKey(selectedItem.SecurableObject as Node, securableComboBoxItem.SecurableObject as Node)))
                    {
                        MapRoleUI(new RoleListViewItem(e.Element as Role));
                    }
                    break;
                case CollectionChangeAction.Remove:
                    if (sender.Equals(MappableRoleRegistry.GenerateKey(selectedItem.SecurableObject as Node, securableComboBoxItem.SecurableObject as Node)))
                    {
                        UnmapRoleUI(new RoleMappingListViewItem(e.Element as MappableRole));
                    }
                    break;
                case CollectionChangeAction.Refresh:
                    w_listViewMappings.Items.Clear();
                    if (!sender.Equals(MappableRoleRegistry.GenerateKey(selectedItem.SecurableObject as Node, securableComboBoxItem.SecurableObject as Node)))
                    {
                        break;
                    }
                    FillRoles(selectedItem.SecurableObject, w_listViewSourceRoles);
                    if (e.Element == null)
                    {
                        break;
                    }
                {
                    foreach (MappableRole item in e.Element as MappableRoleCollection)
                    {
                        MapRoleUI(new RoleListViewItem(item));
                    }
                    break;
                }
            }
        }

        private void MapRoleUI(RoleListViewItem source)
        {
            if (!(source.Role is MappableRole))
            {
                throw new ArgumentException("Source is not a mappable object.");
            }
            MappableRole role = source.Role as MappableRole;
            RoleListViewItem roleListViewItem = null;
            foreach (RoleListViewItem item in w_listViewSourceRoles.Items)
            {
                if (!source.Equals(item))
                {
                    continue;
                }
                roleListViewItem = item;
                break;
            }
            if (roleListViewItem != null)
            {
                w_listViewSourceRoles.Items.Remove(roleListViewItem);
            }
            w_listViewMappings.Items.Add(new RoleMappingListViewItem(role));
        }

        public void RegisterListener()
        {
            try
            {
                if (m_mappings != null)
                {
                    m_mappings.OnMappingsChanged += Instance_OnMappingsChanged;
                }
            }
            finally
            {
                w_comboBox_SelectionChangeCommitted(w_comboBoxSources, new EventArgs());
            }
        }

        private void UnmapRoleUI(RoleMappingListViewItem source)
        {
            source.MappableRole.TargetRole = null;
            RoleListViewItem roleListViewItem = new RoleListViewItem(source.MappableRole);
            RoleMappingListViewItem roleMappingListViewItem = null;
            foreach (RoleMappingListViewItem item in w_listViewMappings.Items)
            {
                if (!source.Equals(item))
                {
                    continue;
                }
                roleMappingListViewItem = item;
                break;
            }
            if (roleMappingListViewItem != null)
            {
                w_listViewMappings.Items.Remove(roleMappingListViewItem);
            }
            w_listViewSourceRoles.Items.Add(roleListViewItem);
        }

        public void UnregisterListener()
        {
            if (m_mappings != null)
            {
                m_mappings.OnMappingsChanged -= Instance_OnMappingsChanged;
            }
        }

        private void w_buttonAutoMap_Click(object sender, EventArgs e)
        {
            try
            {
                if (w_comboBoxSources.SelectedItem == null && w_comboBoxTargets.SelectedItem == null)
                {
                    return;
                }
                Dictionary<Role, Role> roles = new Dictionary<Role, Role>();
                foreach (RoleListViewItem item in w_listViewSourceRoles.Items)
                {
                    foreach (RoleListViewItem roleListViewItem in w_listViewTargetRoles.Items)
                    {
                        if (m_comparer.Compare(item.Role, roleListViewItem.Role) != 0)
                        {
                            continue;
                        }
                        roles.Add(item.Role, roleListViewItem.Role);
                        break;
                    }
                }
                foreach (Role key in roles.Keys)
                {
                    (key as MappableRole).TargetRole = roles[key];
                    SecurableComboBoxItem selectedItem = w_comboBoxSources.SelectedItem as SecurableComboBoxItem;
                    SecurableComboBoxItem securableComboBoxItem = w_comboBoxTargets.SelectedItem as SecurableComboBoxItem;
                    m_mappings.AddOrUpdateMapping(MappableRoleRegistry.GenerateKey(selectedItem.SecurableObject as Node, securableComboBoxItem.SecurableObject as Node), key as MappableRole);
                }
            }
            catch (Exception exception)
            {
                GlobalServices.ErrorHandler.HandleException(exception);
            }
        }

        private void w_buttonMap_Click(object sender, EventArgs e)
        {
            try
            {
                if (w_listViewSourceRoles.SelectedItems != null && w_listViewTargetRoles.SelectedItems != null && w_listViewSourceRoles.SelectedItems.Count != 0 && w_listViewTargetRoles.SelectedItems.Count != 0)
                {
                    RoleListViewItem item = w_listViewSourceRoles.SelectedItems[0] as RoleListViewItem;
                    RoleListViewItem roleListViewItem = w_listViewTargetRoles.SelectedItems[0] as RoleListViewItem;
                    (item.Role as MappableRole).TargetRole = roleListViewItem.Role;
                    SecurableComboBoxItem selectedItem = w_comboBoxSources.SelectedItem as SecurableComboBoxItem;
                    SecurableComboBoxItem securableComboBoxItem = w_comboBoxTargets.SelectedItem as SecurableComboBoxItem;
                    m_mappings.AddOrUpdateMapping(MappableRoleRegistry.GenerateKey(selectedItem.SecurableObject as Node, securableComboBoxItem.SecurableObject as Node), item.Role as MappableRole);
                }
            }
            catch (Exception exception)
            {
                GlobalServices.ErrorHandler.HandleException(exception);
            }
        }

        private void w_buttonUnmap_Click(object sender, EventArgs e)
        {
            try
            {
                if (w_listViewMappings.SelectedItems != null && w_listViewMappings.SelectedItems.Count != 0)
                {
                    RoleMappingListViewItem item = w_listViewMappings.SelectedItems[0] as RoleMappingListViewItem;
                    SecurableComboBoxItem selectedItem = w_comboBoxSources.SelectedItem as SecurableComboBoxItem;
                    SecurableComboBoxItem securableComboBoxItem = w_comboBoxTargets.SelectedItem as SecurableComboBoxItem;
                    m_mappings.RemoveMapping(MappableRoleRegistry.GenerateKey(selectedItem.SecurableObject as Node, securableComboBoxItem.SecurableObject as Node), item.MappableRole);
                }
            }
            catch (Exception exception)
            {
                GlobalServices.ErrorHandler.HandleException(exception);
            }
        }

        private void w_comboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                if (sender == null)
                {
                    return;
                }
                ComboBox comboBox = sender as ComboBox;
                if (comboBox.SelectedItem != null)
                {
                    if (comboBox != w_comboBoxSources)
                    {
                        FillRoles((comboBox.SelectedItem as SecurableComboBoxItem).SecurableObject, w_listViewTargetRoles);
                    }
                    else
                    {
                        FillRoles((comboBox.SelectedItem as SecurableComboBoxItem).SecurableObject, w_listViewSourceRoles);
                    }
                    if (w_comboBoxTargets.SelectedItem != null && w_comboBoxSources.SelectedItem != null)
                    {
                        SecurableComboBoxItem selectedItem = w_comboBoxSources.SelectedItem as SecurableComboBoxItem;
                        SecurableComboBoxItem securableComboBoxItem = w_comboBoxTargets.SelectedItem as SecurableComboBoxItem;
                        m_mappings.GetMappings(MappableRoleRegistry.GenerateKey(selectedItem.SecurableObject as Node, securableComboBoxItem.SecurableObject as Node));
                    }
                }
            }
            catch (Exception exception)
            {
                GlobalServices.ErrorHandler.HandleException(exception);
            }
        }
    }
}
