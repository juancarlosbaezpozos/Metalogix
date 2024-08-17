using Metalogix.Actions;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Xml;

namespace Metalogix.UI.WinForms.Actions
{
	internal static class ActionTemplateUIHelper
	{
		internal static void DeleteTemplates(IEnumerable<ActionOptionsTemplate> selectedItems)
		{
			int num = selectedItems.Count<ActionOptionsTemplate>();
			if (num == 0)
			{
				return;
			}
			string str = (num > 1 ? "s" : "");
			if (FlatXtraMessageBox.Show(string.Format(Resources.ActionTemplateDeleteConfirmation, str), string.Format(Resources.ActionTemplateDeleteConfirmationTitle, str), MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
			{
				return;
			}
			selectedItems.First<ActionOptionsTemplate>().ParentProvider.DeleteTemplates(selectedItems);
		}

		internal static void ExportTemplate(IEnumerable<ActionOptionsTemplate> selectedItems)
		{
			int num = selectedItems.Count<ActionOptionsTemplate>();
			if (num == 0)
			{
				return;
			}
			if (num > 1)
			{
				FlatXtraMessageBox.Show(Resources.ActionTemplateMultipleExportWarning, Resources.ActionTemplateMultipleExportWarningTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			ActionOptionsTemplate actionOptionsTemplate = selectedItems.First<ActionOptionsTemplate>();
			SaveFileDialog saveFileDialog = new SaveFileDialog()
			{
				CheckPathExists = true,
				Filter = "XML File (.xml)|*.xml"
			};
			if (saveFileDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			XmlWriterSettings xmlWriterSetting = new XmlWriterSettings()
			{
				OmitXmlDeclaration = true
			};
			using (Stream stream = (new FileInfo(saveFileDialog.FileName)).Create())
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(stream, xmlWriterSetting))
				{
					actionOptionsTemplate.ToXml(xmlWriter);
					xmlWriter.Flush();
				}
			}
		}

		internal static string GetActionDisplayName(Type actionType)
		{
			return ActionTemplateUIHelper.GetActionDisplayName(actionType.FullName);
		}

		internal static string GetActionDisplayName(string actionTypeFullName)
		{
			Metalogix.Actions.Action item = ActionCollection.AvailableActions[actionTypeFullName];
			if (item == null)
			{
				return actionTypeFullName;
			}
			return item.DisplayName;
		}

		internal static void ImportTemplate(ActionOptionsProvider provider, Type actionTypeCheck = null)
		{
			ActionOptionsTemplate actionOptionsTemplate;
			OpenFileDialog openFileDialog = new OpenFileDialog()
			{
				CheckFileExists = true,
				Multiselect = false,
				Filter = "XML Files (.xml)|*.xml"
			};
			if (openFileDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			using (Stream stream = openFileDialog.OpenFile())
			{
				actionOptionsTemplate = provider.ImportTemplate(stream);
			}
			if (actionTypeCheck != null && actionOptionsTemplate.ActionTypeName != actionTypeCheck.FullName && FlatXtraMessageBox.Show(Resources.ActionTemplateImportTypeMismatch, Resources.ActionTemplateImportTypeMismatchTitle, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation) != DialogResult.Yes)
			{
				return;
			}
			IEnumerable<string> optionsTemplatesForAction = 
				from t in (IEnumerable<ActionOptionsTemplate>)provider.GetOptionsTemplatesForAction(actionOptionsTemplate.ActionTypeName)
				select t.TemplateName;
			List<string> strs = new List<string>(optionsTemplatesForAction);
			if (strs.Contains(actionOptionsTemplate.TemplateName))
			{
				if (FlatXtraMessageBox.Show(string.Format(Resources.ActionTemplateNameCollisionWarning, actionOptionsTemplate.TemplateName, ActionTemplateUIHelper.GetActionDisplayName(actionOptionsTemplate.ActionTypeName)), Resources.ActionTemplateNameCollisionWarningHeader, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation) != DialogResult.Yes)
				{
					return;
				}
				string str = string.Concat(actionOptionsTemplate.TemplateName, " ({0})");
				int num = 1;
				while (strs.Contains(string.Format(str, num)))
				{
					num++;
				}
				actionOptionsTemplate.TemplateName = string.Format(str, num);
			}
			actionOptionsTemplate.Commit();
		}

		private delegate void ListViewSelectedItemsDeleg(ListView.SelectedListViewItemCollection items);
	}
}