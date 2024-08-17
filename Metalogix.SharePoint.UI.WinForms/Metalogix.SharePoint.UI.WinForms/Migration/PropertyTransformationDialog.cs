using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Metalogix.Data.Filters;
using Metalogix.DataStructures;
using Metalogix.Explorer;
using Metalogix.SharePoint.Properties;
using Metalogix.SharePoint.UI.WinForms.Migration.Mapping;
using Metalogix.UI.WinForms.Data.Filters;
using Metalogix.UI.WinForms.Explorer;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public class PropertyTransformationDialog : GenericSiteLevelMappingDialog
	{
		private TransformationTaskCollection m_Tasks;

		private IContainer components;

		public TransformationTaskCollection Tasks
		{
			get
			{
				return m_Tasks;
			}
			set
			{
				m_Tasks = value;
				LoadOptionsUI();
			}
		}

		protected override IUrlParser UrlParser
		{
			get
			{
				IUrlParser urlParser = _urlParser;
				if (urlParser == null)
				{
					urlParser = (_urlParser = new TransformationTaskFilterParser());
				}
				return urlParser;
			}
		}

		public PropertyTransformationDialog(NodeCollection CurrentNodeCollection, string sDialogTitle)
			: base(CurrentNodeCollection)
		{
			Type[] collection = new Type[4]
			{
				typeof(SPWeb),
				typeof(SPList),
				typeof(SPFolder),
				typeof(SPDiscussionList)
			};
			base.ExplorerSelectableTypes = new List<Type>(collection);
			m_Tasks = new TransformationTaskCollection();
			base.DialogTitle = sDialogTitle;
			base.ApplyOptionText = Resources.RenameDialogApplyOption;
			base.CreateRuleText = Resources.RenameDialogCreateRule;
			base.OptionsLabelText = Resources.RenameDialogOptionsLabel;
			base.OptionColumnText = Resources.RenameDialogOptionsColumn;
			base.OptionAppliedValuesText = Resources.RenameDialogOptionsValue;
			base.EditOptionText = Resources.RenameDialogEditOption;
			base.RemoveOptionText = Resources.RenameDialogRemoveOption;
			SetDialogText();
		}

		protected override void CheckValidNode(ExplorerTreeNode treeNode, ref bool bEnabledCreateNodeOption, ref bool bEnabledCreateListRuleOption)
		{
			bEnabledCreateNodeOption = treeNode != null;
			bEnabledCreateListRuleOption = false;
		}

		protected override void CreateNewApplyOptions(ExplorerTreeNode treeNode)
		{
			ExplorerTreeNode selectedNode = base.SelectedNode;
			if (selectedNode == null)
			{
				return;
			}
			foreach (MappingOption mapping in base.MappingList)
			{
				if (((TransformationTask)mapping.SubObject).ApplyTo is FilterExpression)
				{
					SPNode sPNode = (SPNode)selectedNode.Node;
					if (!((string)sPNode.GetProperties()["DisplayUrl"].GetValue(sPNode) != ((FilterExpression)((TransformationTask)mapping.SubObject).ApplyTo).Pattern))
					{
						EditOptionAppliedValue(mapping);
						return;
					}
				}
			}
			RenameDialog renameDialog = new RenameDialog((SPNode)selectedNode.Node, selectedNode.Parent == null);
			if (renameDialog.ShowDialog() == DialogResult.OK && renameDialog.Task != null)
			{
				MappingOption option = new MappingOption
				{
					SubObject = renameDialog.Task,
					Option = ((FilterExpression)renameDialog.Task.ApplyTo).Pattern,
					OptionAppliedValue = renameDialog.Task.TaskToUIString()
				};
				AddOption(option);
				UpdateOptionsColumnWidths();
			}
		}

		protected override void CreateRule()
		{
			Dictionary<Type, string> dictionary = new Dictionary<Type, string>();
			dictionary.Add(typeof(SPWeb), Resources.SitePluralizedName);
			dictionary.Add(typeof(SPList), Resources.ListPluralizedName);
			dictionary.Add(typeof(SPListItem), Resources.FolderPluralizedName);
			Dictionary<Type, string> applyToTypes = dictionary;
			FilterExpressionEditorAndTypeSelectorDialog filterExpressionEditorAndTypeSelectorDialog = new FilterExpressionEditorAndTypeSelectorDialog(applyToTypes)
			{
				Title = "Node Filter Conditions"
			};
			filterExpressionEditorAndTypeSelectorDialog.ShowDialog();
			if (filterExpressionEditorAndTypeSelectorDialog.DialogResult == DialogResult.OK && filterExpressionEditorAndTypeSelectorDialog.FilterExpression != null)
			{
				RenameDialog renameDialog = new RenameDialog(base.NodeCollection[0] as SPNode, bIsRoot: false, bIsRuleBased: true);
				MappingOption mappingOption = new MappingOption
				{
					Option = filterExpressionEditorAndTypeSelectorDialog.FilterExpression.GetLogicString()
				};
				if (renameDialog.ShowDialog() == DialogResult.OK)
				{
					TransformationTask task = renameDialog.Task;
					task.ApplyTo = filterExpressionEditorAndTypeSelectorDialog.FilterExpression;
					mappingOption.OptionAppliedValue = renameDialog.Task.TaskToUIString();
					mappingOption.SubObject = task;
					AddOption(mappingOption);
					UpdateOptionsColumnWidths();
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

		protected override void EditOptionAppliedValue(MappingOption option)
		{
			TransformationTask transformationTask = (TransformationTask)option.SubObject;
			Node node = null;
			if (transformationTask.ApplyTo is FilterExpression)
			{
				node = base.NodeCollection.GetNodeByUrl(option.Option);
			}
			RenameDialog renameDialog = null;
			if (node != null)
			{
				renameDialog = ((node.Parent != null) ? new RenameDialog((SPNode)node, bIsRoot: false) : new RenameDialog((SPNode)node, bIsRoot: true));
				if (transformationTask.ChangeOperations.ContainsKey("Name"))
				{
					renameDialog.TargetName = transformationTask.ChangeOperations["Name"];
				}
				if (transformationTask.ChangeOperations.ContainsKey("Title"))
				{
					renameDialog.TargetTitle = transformationTask.ChangeOperations["Title"];
				}
				if (transformationTask.ChangeOperations.ContainsKey("FileLeafRef"))
				{
					renameDialog.TargetTitle = transformationTask.ChangeOperations["FileLeafRef"];
				}
			}
			else
			{
				renameDialog = new RenameDialog(base.NodeCollection[0] as SPNode, bIsRoot: false, bIsRuleBased: true);
			}
			if (renameDialog.ShowDialog() == DialogResult.OK && renameDialog.Task != null && renameDialog.Task != null)
			{
				renameDialog.Task.ApplyTo = transformationTask.ApplyTo;
				option.SubObject = renameDialog.Task;
				option.Option = ((FilterExpression)renameDialog.Task.ApplyTo).Pattern;
				option.OptionAppliedValue = renameDialog.Task.TaskToUIString();
			}
		}

		protected override void EditRule(MappingOption option)
		{
			TransformationTask transformationTask = (TransformationTask)option.SubObject;
			Dictionary<Type, string> dictionary = new Dictionary<Type, string>();
			dictionary.Add(typeof(SPWeb), Resources.SitePluralizedName);
			dictionary.Add(typeof(SPList), Resources.ListPluralizedName);
			dictionary.Add(typeof(SPListItem), Resources.FolderPluralizedName);
			Dictionary<Type, string> applyToTypes = dictionary;
			List<string> list = new List<string>();
			FilterExpression firstFilterExpression = GetFirstFilterExpression(((TransformationTask)option.SubObject).ApplyTo);
			if (firstFilterExpression != null)
			{
				foreach (string appliesToType in firstFilterExpression.AppliesToTypes)
				{
					list.Add(appliesToType);
				}
			}
			FilterExpressionEditorAndTypeSelectorDialog filterExpressionEditorAndTypeSelectorDialog = new FilterExpressionEditorAndTypeSelectorDialog(applyToTypes, list)
			{
				Title = "Node Filter Conditions",
				FilterExpression = transformationTask.ApplyTo
			};
			if (filterExpressionEditorAndTypeSelectorDialog.ShowDialog() == DialogResult.OK && filterExpressionEditorAndTypeSelectorDialog.FilterExpression != null)
			{
				transformationTask.ApplyTo = filterExpressionEditorAndTypeSelectorDialog.FilterExpression;
				option.Option = filterExpressionEditorAndTypeSelectorDialog.FilterExpression.GetLogicString();
			}
		}

		private FilterExpression GetFirstFilterExpression(IFilterExpression expression)
		{
			if (expression is FilterExpression result)
			{
				return result;
			}
			if (expression is FilterExpressionList filterExpressionList)
			{
				{
					foreach (IFilterExpression item in filterExpressionList)
					{
						FilterExpression firstFilterExpression = GetFirstFilterExpression(item);
						if (firstFilterExpression == null)
						{
							continue;
						}
						return firstFilterExpression;
					}
					return null;
				}
			}
			return null;
		}

		private new void InitializeComponent()
		{
			base.InitializeComponent();
		}

		protected override void LoadOptionsUI()
		{
			ClearOptions();
			foreach (TransformationTask task in m_Tasks)
			{
				MappingOption option = new MappingOption
				{
					Option = task.ApplyTo.GetLogicString(),
					SubObject = task,
					OptionAppliedValue = task.TaskToUIString()
				};
				AddOption(option);
			}
			UpdateOptionsColumnWidths();
		}

		protected override void SaveUI()
		{
			Tasks.TransformationTasks.Clear();
			foreach (MappingOption mapping in base.MappingList)
			{
				Tasks.TransformationTasks.Add((TransformationTask)mapping.SubObject);
			}
		}
	}
}
