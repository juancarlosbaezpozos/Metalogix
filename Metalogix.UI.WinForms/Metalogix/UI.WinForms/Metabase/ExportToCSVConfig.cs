using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.Metabase.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Metalogix.Metabase.Options;

namespace Metalogix.UI.WinForms.Metabase
{
	[ActionConfig(new Type[] { typeof(ExportToCSVAction) })]
	public class ExportToCSVConfig : IActionConfig
	{
		protected bool IsExcelFile
		{
			get;
			set;
		}

		public ExportToCSVConfig()
		{
		}

		public ConfigurationResult Configure(ActionConfigContext context)
		{
			Node item = (Node)context.ActionContext.Targets[0];
			if (item == null)
			{
				return ConfigurationResult.Cancel;
			}
			List<PropertyDescriptor> propertyDescriptors = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor property in item.GetProperties())
			{
				if (typeof(ExplorerNode).GetProperty(property.Name) != null)
				{
					continue;
				}
				propertyDescriptors.Add(property);
			}
			ExportToCSVDialog exportToCSVDialog = new ExportToCSVDialog(this.IsExcelFile)
			{
				Options = context.GetActionOptions<ExportToCSVOptions>(),
				Properties = new PropertyDescriptorCollection(propertyDescriptors.ToArray())
			};
			if (exportToCSVDialog.ShowDialog() != DialogResult.OK)
			{
				return ConfigurationResult.Cancel;
			}
			context.ActionOptions = exportToCSVDialog.Options;
			return ConfigurationResult.Run;
		}
	}
}