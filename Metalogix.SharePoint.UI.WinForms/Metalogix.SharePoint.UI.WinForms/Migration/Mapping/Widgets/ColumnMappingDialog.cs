using Metalogix.Data.Mapping;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Properties;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Data.Mapping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace Metalogix.SharePoint.UI.WinForms.Migration.Mapping.Widgets
{
	public class ColumnMappingDialog : Form
	{
		private Dictionary<string, IListFilter> m_sourceFilters = new Dictionary<string, IListFilter>();

		private Dictionary<string, IListFilter> m_targetFilters = new Dictionary<string, IListFilter>();

		private SPList m_sourceList;

		private SPList m_targetList;

		private SPWeb m_targetWeb;

		private SPFieldCollection m_availableSiteColumns;

		private object m_oAvailableColumnsLock = new object();

		private ListSummaryItem[] m_summaryItems;

		private IContainer components;

		private Button w_bCancel;

		private Button w_bOkay;

		private ListMapperControl w_listMapperControl;

		public bool AllowCreation
		{
			get
			{
				return this.w_listMapperControl.AllowNewSource;
			}
			set
			{
				this.w_listMapperControl.AllowNewSource = value;
			}
		}

		public bool AllowDeletion
		{
			get
			{
				return this.w_listMapperControl.AllowDeletion;
			}
			set
			{
				this.w_listMapperControl.AllowDeletion = value;
			}
		}

		private SPFieldCollection AvailableSiteColumns
		{
			get
			{
				lock (this.m_oAvailableColumnsLock)
				{
					if (this.m_availableSiteColumns == null && this.m_targetWeb != null)
					{
						this.m_availableSiteColumns = this.m_targetWeb.GetAvailableColumns(false);
					}
				}
				return this.m_availableSiteColumns;
			}
		}

		public bool ShowSourceOnSource
		{
			get
			{
				return this.w_listMapperControl.ShowSourceOnSource;
			}
			set
			{
				this.w_listMapperControl.ShowSourceOnSource = value;
			}
		}

		public bool ShowSourceOnTarget
		{
			get
			{
				return this.w_listMapperControl.ShowSourceOnTarget;
			}
			set
			{
				this.w_listMapperControl.ShowSourceOnTarget = value;
			}
		}

		public object[] SourceItems
		{
			get
			{
				return this.w_listMapperControl.SourceItems;
			}
			set
			{
				this.w_listMapperControl.SourceItems = value;
			}
		}

		public ListSummaryItem[] SummaryItems
		{
			get
			{
				return this.m_summaryItems;
			}
			set
			{
				this.w_listMapperControl.RenderViewSource(new SPFieldView());
				this.w_listMapperControl.RenderViewTarget(new SPFieldView());
				try
				{
					this.w_listMapperControl.SummaryItems = value;
					this.m_summaryItems = value;
				}
				finally
				{
					this.w_listMapperControl.RenderViewSource(new SPFieldDisplayView());
					this.w_listMapperControl.RenderViewTarget(new SPFieldDisplayView());
				}
			}
		}

		public object[] TargetItems
		{
			get
			{
				return this.w_listMapperControl.TargetItems;
			}
			set
			{
				this.w_listMapperControl.TargetItems = value;
			}
		}

		public ColumnMappingDialog(Node sourceNode, Node targetNode)
		{
			this.InitializeComponent();
			this.InitializeMappingControlGeneralSettings();
			this.InitializeMappingControlSourceForLists(sourceNode);
			this.InitializeMappingControlTargetForLists(targetNode);
			this.InitializeDialogSizeAndLocation();
		}

		public ColumnMappingDialog(Node sourceNode, IEnumerable<SPContentType> targetContentTypes)
		{
			this.InitializeComponent();
			this.InitializeMappingControlGeneralSettings();
			this.InitializeMappingControlSourceForLists(sourceNode);
			this.InitializeMappingControlTargetForContentTypes(targetContentTypes);
			this.InitializeDialogSizeAndLocation();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private List<SPField> FilterColumns(SPFieldCollection fieldCollection, SPList list)
		{
			List<SPField> sPFields = new List<SPField>();
			foreach (SPField sPField in fieldCollection)
			{
				if (!this.IncludeField(sPField, list))
				{
					continue;
				}
				sPFields.Add(sPField);
			}
			return sPFields;
		}

		private SPFieldCollection GetFields(Node node)
		{
			SPList list = this.GetList(node);
			if (list == null)
			{
				return null;
			}
			return (SPFieldCollection)list.Fields;
		}

		private SPList GetList(Node node)
		{
			if (node is SPListItem)
			{
				return ((SPListItem)node).ParentList;
			}
			if (node is SPList)
			{
				return (SPList)node;
			}
			if (!(node is SPFolder))
			{
				return null;
			}
			return ((SPFolder)node).ParentList;
		}

		private bool IncludeField(SPField field, SPList list)
		{
			bool flag = (field.FieldXML.Attributes["ReadOnly"] == null ? false : field.FieldXML.Attributes["ReadOnly"].Value.ToLower() == "true");
			return Utils.IsWritableColumn(field.Name, flag, field.Type, (int)list.BaseTemplate, false, field.FieldXML.Attributes["BdcField"] != null, false);
		}

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ColumnMappingDialog));
			this.w_bCancel = new Button();
			this.w_bOkay = new Button();
			this.w_listMapperControl = new ListMapperControl();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.w_bCancel, "w_bCancel");
			this.w_bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.w_bCancel.Name = "w_bCancel";
			this.w_bCancel.UseVisualStyleBackColor = true;
			this.w_bCancel.Click += new EventHandler(this.w_bCancel_Click);
			componentResourceManager.ApplyResources(this.w_bOkay, "w_bOkay");
			this.w_bOkay.Name = "w_bOkay";
			this.w_bOkay.UseVisualStyleBackColor = true;
			this.w_bOkay.Click += new EventHandler(this.w_bOkay_Click);
			this.w_listMapperControl.AllowAutomap = false;
			this.w_listMapperControl.AllowNewSource = true;
			this.w_listMapperControl.AllowNewTarget = false;
			this.w_listMapperControl.AllowDeletion = false;
			this.w_listMapperControl.AllowNewTagCreation = true;
			componentResourceManager.ApplyResources(this.w_listMapperControl, "w_listMapperControl");
			this.w_listMapperControl.Name = "w_listMapperControl";
			this.w_listMapperControl.RemoveSourceOnMap = false;
			this.w_listMapperControl.RemoveTargetOnMap = true;
			this.w_listMapperControl.SelectedSource = null;
			this.w_listMapperControl.SelectedTarget = null;
			this.w_listMapperControl.ShowBottomToolStrip = false;
			this.w_listMapperControl.ShowGroups = true;
			this.w_listMapperControl.ShowSourceOnSource = true;
			this.w_listMapperControl.ShowSourceOnTarget = true;
			this.w_listMapperControl.SourceItems = null;
			this.w_listMapperControl.Sources = null;
			this.w_listMapperControl.SummaryItems = new ListSummaryItem[0];
			this.w_listMapperControl.TargetItems = null;
			this.w_listMapperControl.Targets = null;
			this.w_listMapperControl.OnNewSourceButtonClicked += new ListMapperControl.NewButtonClickedEventHandler(this.w_listMapperControl_OnNewButtonClicked);
			base.AcceptButton = this.w_bOkay;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.w_bCancel;
			base.Controls.Add(this.w_listMapperControl);
			base.Controls.Add(this.w_bOkay);
			base.Controls.Add(this.w_bCancel);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ColumnMappingDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.ResumeLayout(false);
		}

		private void InitializeDialogSizeAndLocation()
		{
			bool flag = false;
			int height = base.Size.Height;
			int width = base.Size.Width;
			if (SystemInformation.PrimaryMonitorMaximizedWindowSize.Height < base.Size.Height)
			{
				height = SystemInformation.PrimaryMonitorMaximizedWindowSize.Height - 20;
				flag = true;
			}
			System.Drawing.Size primaryMonitorMaximizedWindowSize = SystemInformation.PrimaryMonitorMaximizedWindowSize;
			System.Drawing.Size size = base.Size;
			if (primaryMonitorMaximizedWindowSize.Width - 10 < size.Width)
			{
				width = SystemInformation.PrimaryMonitorMaximizedWindowSize.Width - 20;
				flag = true;
			}
			if (flag)
			{
				base.Size = new System.Drawing.Size(width, height);
			}
		}

		private void InitializeMappingControlGeneralSettings()
		{
			this.w_listMapperControl.AllowNewTagCreation = true;
		}

		private void InitializeMappingControlSourceForLists(Node sourceNode)
		{
			this.m_sourceList = this.GetList(sourceNode);
			SPFieldCollection fields = this.GetFields(sourceNode);
			if (fields == null)
			{
				return;
			}
			List<SPField> sPFields = this.FilterColumns(fields, this.m_sourceList);
			List<SPField> sPFields1 = new List<SPField>();
			List<SPField> sPFields2 = new List<SPField>();
			this.SortColumns(sPFields, ref sPFields2, ref sPFields1);
			List<string> strs = new List<string>();
			this.m_sourceFilters.Clear();
			this.m_sourceFilters.Add("All Columns", new AllFieldTypeFilter());
			this.m_sourceFilters.Add("Site Columns", new SPSiteFieldTypeFilter(sPFields1));
			this.m_sourceFilters.Add("List Columns", new SPListFieldTypeFilter(sPFields2));
			strs.AddRange(new string[] { "All Columns", "Site Columns", "List Columns" });
			try
			{
				foreach (SPContentType contentType in this.m_sourceList.ContentTypes)
				{
					this.m_sourceFilters.Add(contentType.Name, new SPContentTypeFieldFilter(contentType));
					strs.Add(contentType.Name);
				}
			}
			catch
			{
			}
			this.w_listMapperControl.OnSourceChanged += new SourceChangedEventHandler(this.w_listMapperControl_OnSourceChanged);
			this.w_listMapperControl.Sources = strs.ToArray();
			this.w_listMapperControl.AddSourceColumn("FieldType", "Field Type", true);
			this.w_listMapperControl.SourceItems = sPFields.ToArray();
			this.w_listMapperControl.SelectedSource = "All Columns";
			this.SortColumnNames("Source");
		}

		private void InitializeMappingControlTargetForContentTypes(IEnumerable<SPContentType> targetContentTypes)
		{
			List<SPField> sPFields = new List<SPField>();
			List<string> strs = new List<string>();
			foreach (SPContentType targetContentType in targetContentTypes)
			{
				foreach (SPField definedField in targetContentType.GetDefinedFields())
				{
					if (strs.Contains(definedField.Name))
					{
						continue;
					}
					strs.Add(definedField.Name);
					sPFields.Add(definedField);
				}
				this.m_targetWeb = targetContentType.ParentCollection.ParentWeb;
			}
			List<string> strs1 = new List<string>();
			string name = null;
			List<SPField> sPFields1 = new List<SPField>();
			object obj = new object[] { strs, sPFields1 };
			if (this.m_availableSiteColumns != null)
			{
				this.LoadSiteColumns(strs, sPFields1);
			}
			this.m_targetFilters.Clear();
			this.m_targetFilters.Add("All Columns", new AllFieldTypeFilter());
			this.m_targetFilters.Add("Site Columns", new SPSiteFieldTypeFilter(new List<SPField>()));
			strs1.AddRange(new string[] { "All Columns", "Site Columns" });
			foreach (SPContentType sPContentType in targetContentTypes)
			{
				this.m_targetFilters.Add(sPContentType.Name, new SPContentTypeFieldFilter(sPContentType));
				strs1.Add(sPContentType.Name);
				if (name != null)
				{
					continue;
				}
				name = sPContentType.Name;
			}
			if (name == null)
			{
				name = "Site Columns";
			}
			this.w_listMapperControl.OnTargetChanged += new SourceChangedEventHandler(this.w_listMapperControl_OnTargetChanged);
			this.w_listMapperControl.Targets = strs1.ToArray();
			this.w_listMapperControl.AddTargetColumn("FieldType", "Field Type", true);
			this.w_listMapperControl.TargetItems = sPFields.ToArray();
			this.w_listMapperControl.SelectedTarget = name;
			(new Thread(new ParameterizedThreadStart(this.LoadSiteColumns))).Start(obj);
		}

		private void InitializeMappingControlTargetForLists(Node targetNode)
		{
			this.m_targetList = this.GetList(targetNode);
			this.m_targetWeb = this.m_targetList.ParentWeb;
			SPFieldCollection fields = this.GetFields(targetNode);
			if (fields == null)
			{
				return;
			}
			List<SPField> sPFields = this.FilterColumns(fields, this.m_targetList);
			List<SPField> sPFields1 = new List<SPField>();
			List<SPField> sPFields2 = new List<SPField>();
			this.SortColumns(sPFields, ref sPFields2, ref sPFields1);
			List<string> strs = new List<string>();
			this.m_targetFilters.Clear();
			this.m_targetFilters.Add("All Columns", new AllFieldTypeFilter());
			this.m_targetFilters.Add("Site Columns", new SPSiteFieldTypeFilter(sPFields1));
			this.m_targetFilters.Add("List Columns", new SPListFieldTypeFilter(sPFields2));
			strs.AddRange(new string[] { "All Columns", "Site Columns", "List Columns" });
			foreach (SPContentType contentType in this.m_targetList.ContentTypes)
			{
				this.m_targetFilters.Add(contentType.Name, new SPContentTypeFieldFilter(contentType));
				strs.Add(contentType.Name);
			}
			this.w_listMapperControl.OnTargetChanged += new SourceChangedEventHandler(this.w_listMapperControl_OnTargetChanged);
			this.w_listMapperControl.Targets = strs.ToArray();
			this.w_listMapperControl.AddTargetColumn("FieldType", "Field Type", true);
			this.w_listMapperControl.TargetItems = sPFields.ToArray();
			this.w_listMapperControl.SelectedSource = "All Columns";
			this.SortColumnNames("Target");
		}

		private void LoadSiteColumns(object oParam)
		{
			object[] objArray = oParam as object[];
			List<string> strs = objArray[0] as List<string>;
			List<SPField> sPFields = objArray[1] as List<SPField>;
			this.LoadSiteColumns(strs, sPFields);
			while (!base.InvokeRequired)
			{
				Thread.Sleep(50);
			}
			ColumnMappingDialog.UpdateUIForSiteColumnsDelegate updateUIForSiteColumnsDelegate = new ColumnMappingDialog.UpdateUIForSiteColumnsDelegate(this.UpdateUIForSiteColumns);
			object[] objArray1 = new object[] { sPFields };
			base.Invoke(updateUIForSiteColumnsDelegate, objArray1);
			this.SortColumnNames("Target");
		}

		private void LoadSiteColumns(List<string> usedFieldNames, List<SPField> siteColumns)
		{
			foreach (SPField availableSiteColumn in this.AvailableSiteColumns)
			{
				if (availableSiteColumn.Hidden || usedFieldNames.Contains(availableSiteColumn.Name))
				{
					continue;
				}
				usedFieldNames.Add(availableSiteColumn.Name);
				siteColumns.Add(availableSiteColumn);
			}
		}

		private void SortColumnNames(string sourceOrTarget)
		{
			DataGridView dataGridView;
			Control[] controlArray = base.Controls.Find("w_dataGridView", true);
			if (controlArray != null)
			{
				dataGridView = (!sourceOrTarget.Equals("Target", StringComparison.InvariantCultureIgnoreCase) || (int)controlArray.Length <= 1 ? controlArray[0] as DataGridView : controlArray[1] as DataGridView);
				if (dataGridView != null)
				{
					dataGridView.Sort(dataGridView.Columns[0], ListSortDirection.Ascending);
				}
			}
		}

		private void SortColumns(List<SPField> fieldCollection, ref List<SPField> listColumns, ref List<SPField> siteColumns)
		{
			foreach (SPField sPField in fieldCollection)
			{
				if (sPField.FieldXML.Attributes["Group"] == null)
				{
					listColumns.Add(sPField);
				}
				else
				{
					siteColumns.Add(sPField);
				}
			}
		}

		public void UpdateSiteContentTypes(IEnumerable<SPContentType> updatedTargetContentTypes)
		{
			this.InitializeMappingControlTargetForContentTypes(updatedTargetContentTypes);
		}

		private void UpdateUIForSiteColumns(List<SPField> siteColumns)
		{
			SPSiteFieldTypeFilter item = this.m_targetFilters["Site Columns"] as SPSiteFieldTypeFilter;
			if (item != null)
			{
				item.Fields.AddRange(siteColumns);
			}
			foreach (SPField siteColumn in siteColumns)
			{
				if (!base.Disposing)
				{
					this.w_listMapperControl.AddTargetItem(siteColumn);
				}
				else
				{
					return;
				}
			}
		}

		private void w_bCancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			base.Close();
		}

		private void w_bOkay_Click(object sender, EventArgs e)
		{
			this.w_listMapperControl.RenderViewSummary(new SPFieldSummaryView());
			this.w_listMapperControl.RenderViewSummary(new SPNewFieldSummaryView());
			this.m_summaryItems = this.w_listMapperControl.SummaryItems;
			base.DialogResult = System.Windows.Forms.DialogResult.OK;
			base.Close();
		}

	    // Metalogix.SharePoint.UI.WinForms.Migration.Mapping.Widgets.ColumnMappingDialog
	    private void w_listMapperControl_OnNewButtonClicked(object sender, ListPickerItem[] selectedSourceItems)
	    {
	        if (selectedSourceItems == null || selectedSourceItems.Length == 0)
	        {
	            return;
	        }
	        if (selectedSourceItems.Length > 1)
	        {
	            FlatXtraMessageBox.Show(Resources.Invalid_Selection, Resources.Select_One_Source_Item, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
	            return;
	        }
	        ListPickerItem listPickerItem = selectedSourceItems[0];
	        string columnName;
	        if (listPickerItem.Tag != null && listPickerItem.Tag is SPField)
	        {
	            columnName = ((SPField)listPickerItem.Tag).DisplayName;
	        }
	        else
	        {
	            columnName = listPickerItem.Target;
	        }
	        string[] array = null;
	        if (this.m_targetWeb.ContentTypes != null)
	        {
	            SPContentTypeCollection contentTypes = this.m_targetWeb.ContentTypes;
	            array = new string[contentTypes.Count];
	            for (int i = 0; i < array.Length; i++)
	            {
	                array[i] = ((SPContentType)contentTypes[i]).Name;
	            }
	        }
	        System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
	        System.Collections.Generic.List<string> list2 = new System.Collections.Generic.List<string>();
	        if (this.AvailableSiteColumns != null)
	        {
	            SPFieldCollection availableSiteColumns = this.AvailableSiteColumns;
	            foreach (SPField sPField in availableSiteColumns)
	            {
	                if (!string.IsNullOrEmpty(sPField.DisplayName) && !list.Contains(sPField.DisplayName))
	                {
	                    list.Add(sPField.DisplayName);
	                }
	                System.Xml.XmlAttribute xmlAttribute = sPField.FieldXML.Attributes["Group"];
	                if (xmlAttribute != null && !string.IsNullOrEmpty(xmlAttribute.Value) && xmlAttribute.Value != "_Hidden" && xmlAttribute.Value != "Not Found" && !list2.Contains(xmlAttribute.Value))
	                {
	                    list2.Add(xmlAttribute.Value);
	                }
	            }
	        }
	        NewColumnDialog newColumnDialog = new NewColumnDialog();
	        newColumnDialog.ColumnName = columnName;
	        newColumnDialog.ContentTypeOptions = array;
	        newColumnDialog.SiteColumnGroupOptions = list2.ToArray();
	        bool flag = false;
	        while (true)
	        {
	            flag = false;
	            DialogResult dialogResult = newColumnDialog.ShowDialog();
	            if (dialogResult != DialogResult.OK)
	            {
	                break;
	            }
	            foreach (SPField sPField2 in this.m_targetList.Fields)
	            {
	                if (sPField2.DisplayName == newColumnDialog.ColumnName && FlatXtraMessageBox.Show(Resources.Column_Name_Already_Exists, Resources.Invalid_Column_Name, MessageBoxButtons.OKCancel, MessageBoxIcon.Hand) == DialogResult.OK)
	                {
	                    flag = true;
	                    break;
	                }
	                if (!flag && newColumnDialog.CreateAsSiteColumn && list.Contains(newColumnDialog.ColumnName) && FlatXtraMessageBox.Show(Resources.Site_Column_Name_Already_Exists, Resources.Invalid_Column_Name, MessageBoxButtons.OKCancel, MessageBoxIcon.Hand) == DialogResult.OK)
	                {
	                    flag = true;
	                    break;
	                }
	            }
	            if (!flag)
	            {
	                goto Block_10;
	            }
	        }
	        return;
	        Block_10:
	        ListPickerItem listPickerItem2 = new ListPickerItem();
	        listPickerItem2.Target = newColumnDialog.ColumnName;
	        listPickerItem2.TargetType = listPickerItem.TargetType;
	        listPickerItem2.Tag = listPickerItem2;
	        foreach (string current in listPickerItem.CustomColumns.Keys)
	        {
	            listPickerItem2.CustomColumns.Add(new System.Collections.Generic.KeyValuePair<string, object>(current, listPickerItem.CustomColumns[current]));
	        }
	        if (listPickerItem2.CustomColumns.ContainsKey("FieldType"))
	        {
	            listPickerItem2.CustomColumns["FieldType"] = (newColumnDialog.CreateAsSiteColumn ? "Site Column" : "List Column");
	        }
	        if (newColumnDialog.CreateAsSiteColumn)
	        {
	            if (listPickerItem2.CustomColumns.ContainsKey("SiteColumnGroup"))
	            {
	                listPickerItem2.CustomColumns["SiteColumnGroup"] = newColumnDialog.SiteColumnGroup;
	            }
	            else
	            {
	                listPickerItem2.CustomColumns.Add(new System.Collections.Generic.KeyValuePair<string, object>("SiteColumnGroup", newColumnDialog.SiteColumnGroup));
	            }
	            if (newColumnDialog.AddToContentType)
	            {
	                if (listPickerItem2.CustomColumns.ContainsKey("ContentType"))
	                {
	                    listPickerItem2.CustomColumns["ContentType"] = newColumnDialog.ContentType;
	                }
	                else
	                {
	                    listPickerItem2.CustomColumns.Add(new System.Collections.Generic.KeyValuePair<string, object>("ContentType", newColumnDialog.ContentType));
	                }
	            }
	        }
	        this.w_listMapperControl.AddTargetItem(listPickerItem2);
	        this.w_listMapperControl.Map(listPickerItem, listPickerItem2);
	    }

        private void w_listMapperControl_OnSourceChanged(object sender, SourceChangedEventArgs e)
		{
			if (this.m_sourceFilters.ContainsKey(e.Tag.ToString()))
			{
				this.w_listMapperControl.FilterSource(this.m_sourceFilters[e.Tag.ToString()]);
			}
		}

		private void w_listMapperControl_OnTargetChanged(object sender, SourceChangedEventArgs e)
		{
			if (this.m_targetFilters.ContainsKey(e.Tag.ToString()))
			{
				this.w_listMapperControl.FilterTarget(this.m_targetFilters[e.Tag.ToString()]);
			}
		}

		private delegate void UpdateUIForSiteColumnsDelegate(List<SPField> siteColumns);
	}
}