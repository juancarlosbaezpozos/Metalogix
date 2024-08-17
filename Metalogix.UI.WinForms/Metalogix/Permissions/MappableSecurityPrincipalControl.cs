using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Metalogix.UI.WinForms;

namespace Metalogix.Permissions
{
    public class MappableSecurityPrincipalControl : UserControl
    {
        private ISecurableObject[] m_sources;

        private ISecurableObject[] m_targets;

        private MappableSecurityPrincipalCollection m_mappings;

        private IComparer<SecurityPrincipal> m_comparer;

        private IContainer components;

        private SplitContainer splitContainerInner;

        private Button w_buttonMap;

        private Button w_buttonUnmap;

        private Label w_labelSourceLabel;

        private ListView w_listViewSourceUsers;

        private Label w_labelTargetLabel;

        private ListView w_listViewTargetUsers;

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

        private ColumnHeader columnHeader6;

        private ColumnHeader columnHeader3;

        public IComparer<SecurityPrincipal> AutoMapComparer
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

        public MappableSecurityPrincipalCollection PrincipalMappings
        {
            get
            {
                return m_mappings;
            }
            set
            {
                m_mappings = value;
                RefreshMappingsUI();
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

        public MappableSecurityPrincipalControl()
            : this(null, null)
        {
        }

        public MappableSecurityPrincipalControl(string sSourceLabel, string sTargetLabel)
            : this(sSourceLabel, sTargetLabel, null, null, null, null)
        {
        }

        public MappableSecurityPrincipalControl(string sSourceLabel, string sTargetLabel, ISecurableObject[] sources, ISecurableObject[] targets, MappableSecurityPrincipalCollection mappings, IComparer<SecurityPrincipal> automapper)
        {
            InitializeComponent();
            if (sources != null)
            {
                Sources = sources;
            }
            if (targets != null)
            {
                Targets = targets;
            }
            PrincipalMappings = ((mappings == null) ? (PrincipalMappings = new MappableSecurityPrincipalCollection(null)) : mappings);
            AutoMapComparer = automapper;
            if (sSourceLabel != null)
            {
                w_labelSourceLabel.Text = sSourceLabel;
            }
            if (sTargetLabel != null)
            {
                w_labelTargetLabel.Text = sTargetLabel;
            }
            w_listViewSourceUsers.ListViewItemSorter = new ListViewColumnSorter();
            w_listViewTargetUsers.ListViewItemSorter = new ListViewColumnSorter();
            w_listViewMappings.ListViewItemSorter = new ListViewColumnSorter();
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
                if (source.Principals == null)
                {
                    return;
                }
                foreach (SecurityPrincipal principal in (IEnumerable<SecurityPrincipal>)source.Principals)
                {
                    if (principal is MappableSecurityPrincipal && (principal as MappableSecurityPrincipal).TargetPrincipal != null)
                    {
                        MapPrincipalUI(new PrincipalListViewItem(principal));
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

        private void FillPrincipals(ISecurableObject source, ListView target)
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
                foreach (SecurityPrincipal principal in (IEnumerable<SecurityPrincipal>)source.Principals)
                {
                    if (principal is MappableSecurityPrincipal || target != w_listViewSourceUsers)
                    {
                        PrincipalListViewItem principalListViewItem = new PrincipalListViewItem(principal);
                        if (!target.Items.Contains(principalListViewItem))
                        {
                            target.Items.Add(principalListViewItem);
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
            this.splitContainerInner = new System.Windows.Forms.SplitContainer();
            this.w_comboBoxSources = new System.Windows.Forms.ComboBox();
            this.w_labelSourceLabel = new System.Windows.Forms.Label();
            this.w_listViewSourceUsers = new System.Windows.Forms.ListView();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.w_buttonAutoMap = new System.Windows.Forms.Button();
            this.w_comboBoxTargets = new System.Windows.Forms.ComboBox();
            this.w_labelTargetLabel = new System.Windows.Forms.Label();
            this.w_listViewTargetUsers = new System.Windows.Forms.ListView();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
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
            this.splitContainerInner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerInner.Location = new System.Drawing.Point(0, 0);
            this.splitContainerInner.Name = "splitContainerInner";
            this.splitContainerInner.Panel1.Controls.Add(this.w_comboBoxSources);
            this.splitContainerInner.Panel1.Controls.Add(this.w_labelSourceLabel);
            this.splitContainerInner.Panel1.Controls.Add(this.w_listViewSourceUsers);
            this.splitContainerInner.Panel2.Controls.Add(this.w_buttonAutoMap);
            this.splitContainerInner.Panel2.Controls.Add(this.w_comboBoxTargets);
            this.splitContainerInner.Panel2.Controls.Add(this.w_labelTargetLabel);
            this.splitContainerInner.Panel2.Controls.Add(this.w_listViewTargetUsers);
            this.splitContainerInner.Panel2.Controls.Add(this.w_buttonMap);
            this.splitContainerInner.Size = new System.Drawing.Size(752, 261);
            this.splitContainerInner.SplitterDistance = 346;
            this.splitContainerInner.TabIndex = 0;
            this.w_comboBoxSources.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.w_comboBoxSources.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.w_comboBoxSources.FormattingEnabled = true;
            this.w_comboBoxSources.Location = new System.Drawing.Point(3, 30);
            this.w_comboBoxSources.Name = "w_comboBoxSources";
            this.w_comboBoxSources.Size = new System.Drawing.Size(340, 21);
            this.w_comboBoxSources.TabIndex = 2;
            this.w_comboBoxSources.SelectionChangeCommitted += new System.EventHandler(w_comboBox_SelectionChangeCommitted);
            this.w_labelSourceLabel.AutoSize = true;
            this.w_labelSourceLabel.Location = new System.Drawing.Point(3, 8);
            this.w_labelSourceLabel.Name = "w_labelSourceLabel";
            this.w_labelSourceLabel.Size = new System.Drawing.Size(70, 13);
            this.w_labelSourceLabel.TabIndex = 1;
            this.w_labelSourceLabel.Text = "Source Label";
            this.w_listViewSourceUsers.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            System.Windows.Forms.ListView.ColumnHeaderCollection columns = this.w_listViewSourceUsers.Columns;
            System.Windows.Forms.ColumnHeader[] columnHeaderArray = new System.Windows.Forms.ColumnHeader[2] { this.columnHeader4, this.columnHeader6 };
            columns.AddRange(columnHeaderArray);
            this.w_listViewSourceUsers.FullRowSelect = true;
            this.w_listViewSourceUsers.HideSelection = false;
            this.w_listViewSourceUsers.Location = new System.Drawing.Point(3, 57);
            this.w_listViewSourceUsers.MultiSelect = false;
            this.w_listViewSourceUsers.Name = "w_listViewSourceUsers";
            this.w_listViewSourceUsers.Size = new System.Drawing.Size(340, 201);
            this.w_listViewSourceUsers.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.w_listViewSourceUsers.TabIndex = 0;
            this.w_listViewSourceUsers.UseCompatibleStateImageBehavior = false;
            this.w_listViewSourceUsers.View = System.Windows.Forms.View.Details;
            this.w_listViewSourceUsers.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(w_listView_ColumnClick);
            this.columnHeader4.Text = "Name";
            this.columnHeader4.Width = 249;
            this.columnHeader6.Text = "Type";
            this.columnHeader6.Width = 87;
            this.w_buttonAutoMap.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.w_buttonAutoMap.Enabled = false;
            this.w_buttonAutoMap.Location = new System.Drawing.Point(324, 86);
            this.w_buttonAutoMap.Name = "w_buttonAutoMap";
            this.w_buttonAutoMap.Size = new System.Drawing.Size(75, 23);
            this.w_buttonAutoMap.TabIndex = 5;
            this.w_buttonAutoMap.Text = "Auto Map";
            this.w_buttonAutoMap.UseVisualStyleBackColor = true;
            this.w_buttonAutoMap.Click += new System.EventHandler(w_buttonAutoMap_Click);
            this.w_comboBoxTargets.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.w_comboBoxTargets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.w_comboBoxTargets.FormattingEnabled = true;
            this.w_comboBoxTargets.Location = new System.Drawing.Point(3, 30);
            this.w_comboBoxTargets.Name = "w_comboBoxTargets";
            this.w_comboBoxTargets.Size = new System.Drawing.Size(318, 21);
            this.w_comboBoxTargets.TabIndex = 4;
            this.w_comboBoxTargets.SelectionChangeCommitted += new System.EventHandler(w_comboBox_SelectionChangeCommitted);
            this.w_labelTargetLabel.AutoSize = true;
            this.w_labelTargetLabel.Location = new System.Drawing.Point(0, 8);
            this.w_labelTargetLabel.Name = "w_labelTargetLabel";
            this.w_labelTargetLabel.Size = new System.Drawing.Size(67, 13);
            this.w_labelTargetLabel.TabIndex = 1;
            this.w_labelTargetLabel.Text = "Target Label";
            this.w_listViewTargetUsers.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            System.Windows.Forms.ListView.ColumnHeaderCollection columnHeaderCollections = this.w_listViewTargetUsers.Columns;
            System.Windows.Forms.ColumnHeader[] columnHeaderArray1 = new System.Windows.Forms.ColumnHeader[2] { this.columnHeader5, this.columnHeader3 };
            columnHeaderCollections.AddRange(columnHeaderArray1);
            this.w_listViewTargetUsers.FullRowSelect = true;
            this.w_listViewTargetUsers.HideSelection = false;
            this.w_listViewTargetUsers.Location = new System.Drawing.Point(3, 57);
            this.w_listViewTargetUsers.MultiSelect = false;
            this.w_listViewTargetUsers.Name = "w_listViewTargetUsers";
            this.w_listViewTargetUsers.Size = new System.Drawing.Size(318, 201);
            this.w_listViewTargetUsers.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.w_listViewTargetUsers.TabIndex = 0;
            this.w_listViewTargetUsers.UseCompatibleStateImageBehavior = false;
            this.w_listViewTargetUsers.View = System.Windows.Forms.View.Details;
            this.w_listViewTargetUsers.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(w_listView_ColumnClick);
            this.columnHeader5.Text = "Name";
            this.columnHeader5.Width = 230;
            this.columnHeader3.Text = "Type";
            this.columnHeader3.Width = 84;
            this.w_buttonMap.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.w_buttonMap.Location = new System.Drawing.Point(324, 57);
            this.w_buttonMap.Name = "w_buttonMap";
            this.w_buttonMap.Size = new System.Drawing.Size(75, 23);
            this.w_buttonMap.TabIndex = 3;
            this.w_buttonMap.Text = "Map";
            this.w_buttonMap.UseVisualStyleBackColor = true;
            this.w_buttonMap.Click += new System.EventHandler(w_buttonMap_Click);
            this.w_buttonUnmap.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.w_buttonUnmap.Location = new System.Drawing.Point(674, 25);
            this.w_buttonUnmap.Name = "w_buttonUnmap";
            this.w_buttonUnmap.Size = new System.Drawing.Size(75, 23);
            this.w_buttonUnmap.TabIndex = 4;
            this.w_buttonUnmap.Text = "Unmap";
            this.w_buttonUnmap.UseVisualStyleBackColor = true;
            this.w_buttonUnmap.Click += new System.EventHandler(w_buttonUnmap_Click);
            this.w_listViewMappings.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            System.Windows.Forms.ListView.ColumnHeaderCollection columns1 = this.w_listViewMappings.Columns;
            System.Windows.Forms.ColumnHeader[] columnHeaderArray2 = new System.Windows.Forms.ColumnHeader[2] { this.columnHeader1, this.columnHeader2 };
            columns1.AddRange(columnHeaderArray2);
            this.w_listViewMappings.FullRowSelect = true;
            this.w_listViewMappings.HideSelection = false;
            this.w_listViewMappings.Location = new System.Drawing.Point(3, 25);
            this.w_listViewMappings.MultiSelect = false;
            this.w_listViewMappings.Name = "w_listViewMappings";
            this.w_listViewMappings.Size = new System.Drawing.Size(668, 214);
            this.w_listViewMappings.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.w_listViewMappings.TabIndex = 7;
            this.w_listViewMappings.UseCompatibleStateImageBehavior = false;
            this.w_listViewMappings.View = System.Windows.Forms.View.Details;
            this.w_listViewMappings.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(w_listView_ColumnClick);
            this.columnHeader1.Text = "Source Principal";
            this.columnHeader1.Width = 258;
            this.columnHeader2.Text = "Target Principal";
            this.columnHeader2.Width = 298;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Principal Mappings:";
            this.splitContainerOuter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerOuter.Location = new System.Drawing.Point(0, 0);
            this.splitContainerOuter.Name = "splitContainerOuter";
            this.splitContainerOuter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitContainerOuter.Panel1.Controls.Add(this.splitContainerInner);
            this.splitContainerOuter.Panel2.Controls.Add(this.w_listViewMappings);
            this.splitContainerOuter.Panel2.Controls.Add(this.label3);
            this.splitContainerOuter.Panel2.Controls.Add(this.w_buttonUnmap);
            this.splitContainerOuter.Size = new System.Drawing.Size(752, 507);
            this.splitContainerOuter.SplitterDistance = 261;
            this.splitContainerOuter.TabIndex = 9;
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.Controls.Add(this.splitContainerOuter);
            base.Name = "MappableSecurityPrincipalControl";
            base.Size = new System.Drawing.Size(752, 507);
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

        private void MapPrincipalUI(PrincipalListViewItem source)
        {
            if (!(source.Principal is MappableSecurityPrincipal))
            {
                throw new ArgumentException("Source is not a mappable object.");
            }
            MappableSecurityPrincipal principal = source.Principal as MappableSecurityPrincipal;
            PrincipalListViewItem principalListViewItem = null;
            foreach (PrincipalListViewItem item in w_listViewSourceUsers.Items)
            {
                if (!source.Equals(item))
                {
                    continue;
                }
                principalListViewItem = item;
                break;
            }
            if (principalListViewItem != null)
            {
                w_listViewSourceUsers.Items.Remove(principalListViewItem);
            }
            w_listViewMappings.Items.Add(new PrincipalMappingListViewItem(principal));
        }

        private void RefreshMappingsUI()
        {
            if (w_comboBoxSources.SelectedItem == null)
            {
                return;
            }
            w_listViewMappings.Items.Clear();
            FillPrincipals((w_comboBoxSources.SelectedItem as SecurableComboBoxItem).SecurableObject, w_listViewSourceUsers);
            foreach (MappableSecurityPrincipal principalMapping in (IEnumerable<SecurityPrincipal>)PrincipalMappings)
            {
                MapPrincipalUI(new PrincipalListViewItem(principalMapping));
            }
        }

        private void UnmapPrincipalUI(PrincipalMappingListViewItem source)
        {
            source.MappablePrincipal.TargetPrincipal = null;
            PrincipalListViewItem principalListViewItem = new PrincipalListViewItem(source.MappablePrincipal);
            PrincipalMappingListViewItem principalMappingListViewItem = null;
            foreach (PrincipalMappingListViewItem item in w_listViewMappings.Items)
            {
                if (!source.Equals(item))
                {
                    continue;
                }
                principalMappingListViewItem = item;
                break;
            }
            if (principalMappingListViewItem != null)
            {
                w_listViewMappings.Items.Remove(principalMappingListViewItem);
            }
            w_listViewSourceUsers.Items.Add(principalListViewItem);
        }

        private void w_buttonAutoMap_Click(object sender, EventArgs e)
        {
            try
            {
                if (w_comboBoxSources.SelectedItem == null && w_comboBoxTargets.SelectedItem == null)
                {
                    return;
                }
                Dictionary<SecurityPrincipal, SecurityPrincipal> securityPrincipals = new Dictionary<SecurityPrincipal, SecurityPrincipal>();
                foreach (PrincipalListViewItem item in w_listViewSourceUsers.Items)
                {
                    foreach (PrincipalListViewItem principalListViewItem in w_listViewTargetUsers.Items)
                    {
                        if (m_comparer.Compare(item.Principal, principalListViewItem.Principal) != 0)
                        {
                            continue;
                        }
                        securityPrincipals.Add(item.Principal, principalListViewItem.Principal);
                        break;
                    }
                }
                foreach (SecurityPrincipal key in securityPrincipals.Keys)
                {
                    (key as MappableSecurityPrincipal).TargetPrincipal = securityPrincipals[key];
                    if (PrincipalMappings[key.PrincipalName] != null)
                    {
                        PrincipalMappings.Remove(key);
                    }
                    PrincipalMappings.AddPrincipal(key);
                    MapPrincipalUI(new PrincipalListViewItem(key));
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
                if (w_listViewSourceUsers.SelectedItems != null && w_listViewTargetUsers.SelectedItems != null && w_listViewSourceUsers.SelectedItems.Count != 0 && w_listViewTargetUsers.SelectedItems.Count != 0)
                {
                    PrincipalListViewItem item = w_listViewSourceUsers.SelectedItems[0] as PrincipalListViewItem;
                    PrincipalListViewItem principalListViewItem = w_listViewTargetUsers.SelectedItems[0] as PrincipalListViewItem;
                    (item.Principal as MappableSecurityPrincipal).TargetPrincipal = principalListViewItem.Principal;
                    if (PrincipalMappings[item.Principal] != null)
                    {
                        PrincipalMappings.DeletePrincipal(item.Principal);
                    }
                    PrincipalMappings.AddPrincipal(item.Principal);
                    MapPrincipalUI(new PrincipalListViewItem(item.Principal));
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
                    PrincipalMappingListViewItem item = w_listViewMappings.SelectedItems[0] as PrincipalMappingListViewItem;
                    if (PrincipalMappings.Contains(item.MappablePrincipal))
                    {
                        PrincipalMappings.Remove(item.MappablePrincipal);
                        UnmapPrincipalUI(new PrincipalMappingListViewItem(item.MappablePrincipal));
                    }
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
                        FillPrincipals((comboBox.SelectedItem as SecurableComboBoxItem).SecurableObject, w_listViewTargetUsers);
                    }
                    else
                    {
                        FillPrincipals((comboBox.SelectedItem as SecurableComboBoxItem).SecurableObject, w_listViewSourceUsers);
                    }
                }
                RefreshMappingsUI();
            }
            catch (Exception exception)
            {
                GlobalServices.ErrorHandler.HandleException(exception);
            }
        }

        private void w_listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (sender == null)
            {
                return;
            }
            ListView listView = sender as ListView;
            ListViewColumnSorter listViewItemSorter = listView.ListViewItemSorter as ListViewColumnSorter;
            if (e.Column != -1)
            {
                if (e.Column != listViewItemSorter.SortColumn)
                {
                    listViewItemSorter.SortColumn = e.Column;
                    listViewItemSorter.Order = SortOrder.Ascending;
                }
                else if (listViewItemSorter.Order != SortOrder.Ascending)
                {
                    listViewItemSorter.Order = SortOrder.Ascending;
                }
                else
                {
                    listViewItemSorter.Order = SortOrder.Descending;
                }
                listView.Sort();
            }
        }
    }
}
