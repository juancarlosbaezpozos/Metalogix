using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Metalogix.Data;
using Metalogix.Data.Filters;
using Metalogix.UI.WinForms.Data.Filters;
using Metalogix.UI.WinForms.Properties;

namespace Metalogix.UI.WinForms.Data
{
    public class ConditionalMappingControl : UserControl
    {
        private static PluralOperandTranslationDictionary Translator;

        private ConditionalMappingCollection m_mappings = new ConditionalMappingCollection();

        private ConditionalMapping m_selectedMapping;

        private FilterBuilderType m_selectedType;

        private FilterBuilderType[] m_availableTypes;

        private IContainer components;

        private ListView w_lvMappings;

        private ToolStrip w_tsMappings;

        private Panel w_plMappingsList;

        private GroupBox w_gbMappings;

        private Label w_lblMap;

        private TextBox w_tbSourceName;

        private TextBox w_tbTargetName;

        private Label w_lblTo;

        private Label w_lblOn;

        private ComboBox w_cbTypeName;

        private ToolStripButton w_tsBtnAdd;

        private ToolStripButton w_tsBtnRemove;

        private SplitContainer w_splitter;

        private TextBox w_tbFilterDisplay;

        private Label w_lblWhere;

        private Button w_btnEdit;

        private Label w_lblType;

        public FilterBuilderType[] AvailableTypes
        {
            get
		{
			return m_availableTypes;
		}
            set
		{
			m_availableTypes = value;
			UpdateUI();
		}
        }

        public ConditionalMappingCollection Mappings
        {
            get
		{
			return m_mappings;
		}
            set
		{
			m_mappings.ClearCollection();
			foreach (ConditionalMapping conditionalMapping in value)
			{
				m_mappings.Add(new ConditionalMapping(conditionalMapping.SourceName, conditionalMapping.TargetName, FilterExpression.ParseExpression(conditionalMapping.Condition.ToXML())));
			}
			UpdateUI();
		}
        }

        static ConditionalMappingControl()
	{
		Translator = new PluralOperandTranslationDictionary();
	}

        public ConditionalMappingControl()
	{
		InitializeComponent();
		w_splitter.Panel2Collapsed = true;
		w_lblOn.Visible = false;
		w_cbTypeName.Visible = false;
		w_lblType.Visible = false;
	}

        private void BuildFilterString(IFilterExpression iFilter, StringBuilder sb)
	{
		if (!(iFilter is FilterExpressionList))
		{
			FilterExpression filterExpression = (FilterExpression)iFilter;
			sb.Append("\"" + filterExpression.Property + "\" " + Translator[filterExpression.Operand]);
			if (filterExpression.Pattern != null)
			{
				sb.Append(" \"" + filterExpression.Pattern + "\"");
			}
			return;
		}
		FilterExpressionList filterExpressionList = (FilterExpressionList)iFilter;
		string str = " " + filterExpressionList.Logic.ToString() + " \n";
		int num = 0;
		foreach (IFilterExpression filterExpression1 in filterExpressionList)
		{
			BuildFilterString(filterExpression1, sb);
			num++;
			if (num != filterExpressionList.Count)
			{
				sb.Append(str);
			}
		}
	}

        protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

        private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Metalogix.UI.WinForms.Data.ConditionalMappingControl));
		this.w_lvMappings = new System.Windows.Forms.ListView();
		this.w_tsMappings = new System.Windows.Forms.ToolStrip();
		this.w_tsBtnAdd = new System.Windows.Forms.ToolStripButton();
		this.w_tsBtnRemove = new System.Windows.Forms.ToolStripButton();
		this.w_plMappingsList = new System.Windows.Forms.Panel();
		this.w_gbMappings = new System.Windows.Forms.GroupBox();
		this.w_splitter = new System.Windows.Forms.SplitContainer();
		this.w_lblType = new System.Windows.Forms.Label();
		this.w_tbTargetName = new System.Windows.Forms.TextBox();
		this.w_cbTypeName = new System.Windows.Forms.ComboBox();
		this.w_lblMap = new System.Windows.Forms.Label();
		this.w_tbSourceName = new System.Windows.Forms.TextBox();
		this.w_lblOn = new System.Windows.Forms.Label();
		this.w_lblTo = new System.Windows.Forms.Label();
		this.w_tbFilterDisplay = new System.Windows.Forms.TextBox();
		this.w_lblWhere = new System.Windows.Forms.Label();
		this.w_btnEdit = new System.Windows.Forms.Button();
		this.w_tsMappings.SuspendLayout();
		this.w_plMappingsList.SuspendLayout();
		this.w_gbMappings.SuspendLayout();
		this.w_splitter.Panel1.SuspendLayout();
		this.w_splitter.Panel2.SuspendLayout();
		this.w_splitter.SuspendLayout();
		base.SuspendLayout();
		componentResourceManager.ApplyResources(this.w_lvMappings, "w_lvMappings");
		this.w_lvMappings.HideSelection = false;
		this.w_lvMappings.MultiSelect = false;
		this.w_lvMappings.Name = "w_lvMappings";
		this.w_lvMappings.UseCompatibleStateImageBehavior = false;
		this.w_lvMappings.View = System.Windows.Forms.View.List;
		this.w_lvMappings.SelectedIndexChanged += new System.EventHandler(On_SelectedMapping_Changed);
		this.w_tsMappings.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
		System.Windows.Forms.ToolStripItemCollection items = this.w_tsMappings.Items;
		System.Windows.Forms.ToolStripItem[] wTsBtnAdd = new System.Windows.Forms.ToolStripItem[2] { this.w_tsBtnAdd, this.w_tsBtnRemove };
		items.AddRange(wTsBtnAdd);
		componentResourceManager.ApplyResources(this.w_tsMappings, "w_tsMappings");
		this.w_tsMappings.Name = "w_tsMappings";
		this.w_tsBtnAdd.Image = Metalogix.UI.WinForms.Properties.Resources.Add.ToBitmap();
		componentResourceManager.ApplyResources(this.w_tsBtnAdd, "w_tsBtnAdd");
		this.w_tsBtnAdd.Name = "w_tsBtnAdd";
		this.w_tsBtnAdd.Click += new System.EventHandler(On_AddBtn_Clicked);
		this.w_tsBtnRemove.Image = Metalogix.UI.WinForms.Properties.Resources.Remove.ToBitmap();
		componentResourceManager.ApplyResources(this.w_tsBtnRemove, "w_tsBtnRemove");
		this.w_tsBtnRemove.Name = "w_tsBtnRemove";
		this.w_tsBtnRemove.Click += new System.EventHandler(On_Remove);
		componentResourceManager.ApplyResources(this.w_plMappingsList, "w_plMappingsList");
		this.w_plMappingsList.Controls.Add(this.w_lvMappings);
		this.w_plMappingsList.Controls.Add(this.w_tsMappings);
		this.w_plMappingsList.Name = "w_plMappingsList";
		componentResourceManager.ApplyResources(this.w_gbMappings, "w_gbMappings");
		this.w_gbMappings.Controls.Add(this.w_splitter);
		this.w_gbMappings.Name = "w_gbMappings";
		this.w_gbMappings.TabStop = false;
		componentResourceManager.ApplyResources(this.w_splitter, "w_splitter");
		this.w_splitter.Name = "w_splitter";
		this.w_splitter.Panel1.Controls.Add(this.w_lblType);
		this.w_splitter.Panel1.Controls.Add(this.w_tbTargetName);
		this.w_splitter.Panel1.Controls.Add(this.w_cbTypeName);
		this.w_splitter.Panel1.Controls.Add(this.w_lblMap);
		this.w_splitter.Panel1.Controls.Add(this.w_tbSourceName);
		this.w_splitter.Panel1.Controls.Add(this.w_lblOn);
		this.w_splitter.Panel1.Controls.Add(this.w_lblTo);
		this.w_splitter.Panel2.Controls.Add(this.w_tbFilterDisplay);
		this.w_splitter.Panel2.Controls.Add(this.w_lblWhere);
		this.w_splitter.Panel2.Controls.Add(this.w_btnEdit);
		componentResourceManager.ApplyResources(this.w_lblType, "w_lblType");
		this.w_lblType.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.w_lblType.Name = "w_lblType";
		componentResourceManager.ApplyResources(this.w_tbTargetName, "w_tbTargetName");
		this.w_tbTargetName.Name = "w_tbTargetName";
		this.w_tbTargetName.Leave += new System.EventHandler(On_Leaving_TargetName);
		componentResourceManager.ApplyResources(this.w_cbTypeName, "w_cbTypeName");
		this.w_cbTypeName.FormattingEnabled = true;
		this.w_cbTypeName.Name = "w_cbTypeName";
		this.w_cbTypeName.SelectedIndexChanged += new System.EventHandler(On_Type_IndexChanged);
		componentResourceManager.ApplyResources(this.w_lblMap, "w_lblMap");
		this.w_lblMap.Name = "w_lblMap";
		componentResourceManager.ApplyResources(this.w_tbSourceName, "w_tbSourceName");
		this.w_tbSourceName.Name = "w_tbSourceName";
		this.w_tbSourceName.Leave += new System.EventHandler(On_Leaving_SourceName);
		componentResourceManager.ApplyResources(this.w_lblOn, "w_lblOn");
		this.w_lblOn.Name = "w_lblOn";
		componentResourceManager.ApplyResources(this.w_lblTo, "w_lblTo");
		this.w_lblTo.Name = "w_lblTo";
		componentResourceManager.ApplyResources(this.w_tbFilterDisplay, "w_tbFilterDisplay");
		this.w_tbFilterDisplay.BackColor = System.Drawing.SystemColors.Control;
		this.w_tbFilterDisplay.Name = "w_tbFilterDisplay";
		this.w_tbFilterDisplay.ReadOnly = true;
		componentResourceManager.ApplyResources(this.w_lblWhere, "w_lblWhere");
		this.w_lblWhere.Name = "w_lblWhere";
		componentResourceManager.ApplyResources(this.w_btnEdit, "w_btnEdit");
		this.w_btnEdit.Name = "w_btnEdit";
		this.w_btnEdit.UseVisualStyleBackColor = true;
		this.w_btnEdit.Click += new System.EventHandler(On_EditButton_Clicked);
		componentResourceManager.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.w_gbMappings);
		base.Controls.Add(this.w_plMappingsList);
		base.Name = "ConditionalMappingControl";
		this.w_tsMappings.ResumeLayout(false);
		this.w_tsMappings.PerformLayout();
		this.w_plMappingsList.ResumeLayout(false);
		this.w_plMappingsList.PerformLayout();
		this.w_gbMappings.ResumeLayout(false);
		this.w_splitter.Panel1.ResumeLayout(false);
		this.w_splitter.Panel1.PerformLayout();
		this.w_splitter.Panel2.ResumeLayout(false);
		this.w_splitter.Panel2.PerformLayout();
		this.w_splitter.ResumeLayout(false);
		base.ResumeLayout(false);
	}

        private void LoadMappingUI()
	{
		if (m_selectedMapping == null)
		{
			w_tbSourceName.Text = "";
			w_tbTargetName.Text = "";
			w_gbMappings.Enabled = false;
			UpdateFilterUI(null);
		}
		else
		{
			w_tbSourceName.Text = m_selectedMapping.SourceName;
			w_tbTargetName.Text = m_selectedMapping.TargetName;
			w_gbMappings.Enabled = true;
			UpdateFilterUI(m_selectedMapping.Condition);
		}
	}

        private void On_AddBtn_Clicked(object sender, EventArgs e)
	{
		SaveMappingUI();
		ConditionalMapping conditionalMapping = new ConditionalMapping("", "", FilterExpression.ParseExpression("<And/>"));
		ListViewItem listViewItem = w_lvMappings.Items.Add(new ConditionalMappingListViewItem(conditionalMapping));
		listViewItem.Selected = true;
		m_selectedMapping = conditionalMapping;
		LoadMappingUI();
	}

        private void On_EditButton_Clicked(object sender, EventArgs e)
	{
		IFilterExpression tag = (IFilterExpression)w_tbFilterDisplay.Tag;
		FilterExpressionEditorDialog filterExpressionEditorDialog = new FilterExpressionEditorDialog
		{
			Title = $"Define Mapping Condition for {w_tbSourceName.Text} to {w_tbTargetName.Text} on {m_selectedType.ToString()}",
			FilterableTypes = new FilterBuilderType(m_selectedType.ObjectTypes, m_selectedType.AllowFreeFormEntry)
		};
		if (tag != null)
		{
			filterExpressionEditorDialog.FilterExpression = tag;
		}
		filterExpressionEditorDialog.ShowDialog();
		if (filterExpressionEditorDialog.DialogResult == DialogResult.OK)
		{
			UpdateFilterUI(filterExpressionEditorDialog.FilterExpression);
			m_selectedMapping.Condition = filterExpressionEditorDialog.FilterExpression;
		}
	}

        private void On_Leaving_SourceName(object sender, EventArgs e)
	{
		m_selectedMapping.SourceName = w_tbSourceName.Text;
	}

        private void On_Leaving_TargetName(object sender, EventArgs e)
	{
		m_selectedMapping.TargetName = w_tbTargetName.Text;
	}

        private void On_Remove(object sender, EventArgs e)
	{
		if (w_lvMappings.SelectedItems.Count > 0)
		{
			m_selectedMapping = null;
			LoadMappingUI();
			w_lvMappings.Items.Remove(w_lvMappings.SelectedItems[0]);
		}
	}

        private void On_SelectedMapping_Changed(object sender, EventArgs e)
	{
		w_tsBtnRemove.Enabled = w_lvMappings.SelectedItems.Count > 0;
		if (w_lvMappings.SelectedItems.Count == 1)
		{
			m_selectedMapping = ((ConditionalMappingListViewItem)w_lvMappings.SelectedItems[0]).Mapping;
			LoadMappingUI();
		}
	}

        private void On_Type_IndexChanged(object sender, EventArgs e)
	{
		m_selectedType = (FilterBuilderType)w_cbTypeName.SelectedItem;
	}

        private void SaveMappingUI()
	{
		if (m_selectedMapping != null)
		{
			m_selectedMapping.SuspendUpdates();
			m_selectedMapping.SourceName = w_tbSourceName.Text;
			m_selectedMapping.TargetName = w_tbTargetName.Text;
			m_selectedMapping.Condition = (IFilterExpression)w_tbFilterDisplay.Tag;
			m_selectedMapping.ResumeUpdates();
		}
	}

        public void SaveUI()
	{
		m_mappings.ClearCollection();
		foreach (ConditionalMappingListViewItem item in w_lvMappings.Items)
		{
			m_mappings.Add(item.Mapping);
		}
	}

        private void UpdateFilterUI(IFilterExpression filter)
	{
		SuspendLayout();
		if (filter == null)
		{
			w_tbFilterDisplay.Text = "";
			w_tbFilterDisplay.ScrollBars = ScrollBars.None;
		}
		else
		{
			if (!UpdateTypeSetting(filter))
			{
				filter = new FilterExpressionList(ExpressionLogic.And);
			}
			StringBuilder stringBuilder = new StringBuilder();
			BuildFilterString(filter, stringBuilder);
			TextBox wTbFilterDisplay = w_tbFilterDisplay;
			string str = stringBuilder.ToString();
			char[] chrArray = new char[1] { '\n' };
			wTbFilterDisplay.Lines = str.Split(chrArray);
			bool height = w_tbFilterDisplay.PreferredSize.Height > w_tbFilterDisplay.Height;
			bool width = w_tbFilterDisplay.PreferredSize.Width > w_tbFilterDisplay.Width;
			if (height && width)
			{
				w_tbFilterDisplay.ScrollBars = ScrollBars.Both;
			}
			else if (height)
			{
				w_tbFilterDisplay.ScrollBars = ScrollBars.Vertical;
			}
			else if (!width)
			{
				w_tbFilterDisplay.ScrollBars = ScrollBars.None;
			}
			else
			{
				w_tbFilterDisplay.ScrollBars = ScrollBars.Horizontal;
			}
		}
		w_tbFilterDisplay.Tag = filter;
		ResumeLayout();
	}

        private bool UpdateTypeSetting(IFilterExpression iFilter)
	{
		if (!(iFilter is FilterExpression) || m_availableTypes == null)
		{
			if (!(iFilter is FilterExpressionList))
			{
				return false;
			}
			FilterExpressionList filterExpressionList = (FilterExpressionList)iFilter;
			bool flag = false;
			foreach (FilterExpression item in filterExpressionList)
			{
				flag = UpdateTypeSetting(item);
				if (flag)
				{
					break;
				}
			}
			return flag;
		}
		FilterExpression filterExpression = (FilterExpression)iFilter;
		bool flag1 = false;
		int num = -1;
		while (!flag1 && num < m_availableTypes.Length)
		{
			num++;
			foreach (Type objectType in m_availableTypes[num].ObjectTypes)
			{
				if (flag1)
				{
					break;
				}
				Type baseType = objectType;
				do
				{
					flag1 = filterExpression.AppliesToTypes.Contains(baseType.Name) || filterExpression.AppliesToTypes.Contains(baseType.FullName);
					baseType = baseType.BaseType;
				}
				while (!flag1 && baseType.BaseType != typeof(object));
			}
		}
		if (flag1)
		{
			w_cbTypeName.SelectedItem = m_availableTypes[num];
		}
		return flag1;
	}

        private void UpdateUI()
	{
		if (m_availableTypes != null)
		{
			w_lblOn.Visible = true;
			w_splitter.Panel2Collapsed = false;
			if (m_availableTypes.Length != 1)
			{
				w_lblType.Visible = false;
				w_cbTypeName.Visible = true;
				w_cbTypeName.Items.Clear();
				FilterBuilderType[] mAvailableTypes = m_availableTypes;
				foreach (FilterBuilderType filterBuilderType in mAvailableTypes)
				{
					w_cbTypeName.Items.Add(filterBuilderType);
				}
				w_cbTypeName.SelectedIndex = 0;
			}
			else
			{
				w_cbTypeName.Visible = false;
				w_lblType.Visible = true;
				w_lblType.Text = m_availableTypes[0].ToString();
				m_selectedType = m_availableTypes[0];
			}
		}
		w_lvMappings.Items.Clear();
		foreach (ConditionalMapping mapping in Mappings)
		{
			w_lvMappings.Items.Add(new ConditionalMappingListViewItem(mapping));
		}
		if (w_lvMappings.Items.Count <= 0)
		{
			w_gbMappings.Enabled = false;
		}
		else
		{
			w_lvMappings.Items[0].Selected = true;
		}
		w_tsBtnRemove.Enabled = w_lvMappings.SelectedItems.Count > 0;
	}
    }
}
