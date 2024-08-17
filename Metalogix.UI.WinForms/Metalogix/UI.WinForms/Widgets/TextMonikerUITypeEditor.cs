using Metalogix.Explorer;
using Metalogix.Metabase.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Widgets
{
	public class TextMonikerUITypeEditor : UITypeEditor
	{
		public TextMonikerUITypeEditor()
		{
		}

		[PermissionSet(SecurityAction.Demand, Name="FullTrust")]
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			TextMonikerEditor textMonikerEditor = new TextMonikerEditor(TextMonikerUITypeEditor.GetTextMonikerList(context, value));
			textMonikerEditor.ShowDialog();
			textMonikerEditor.Dispose();
			return value;
		}

		[PermissionSet(SecurityAction.Demand, Name="FullTrust")]
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}

		[PermissionSet(SecurityAction.Demand, Name="FullTrust")]
		public override bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			return false;
		}

		public static List<TextMoniker> GetTextMonikerList(ITypeDescriptorContext context, object value)
		{
			List<TextMoniker> textMonikers = new List<TextMoniker>();
			TextMoniker textMoniker = value as TextMoniker;
			if (textMoniker == null)
			{
				string name = context.PropertyDescriptor.Name;
				Node[] instance = (Node[])context.Instance;
				for (int i = 0; i < (int)instance.Length; i++)
				{
					Node node = instance[i];
					textMonikers.Add(new TextMoniker(name, string.Empty, node.Record));
				}
			}
			else
			{
				textMonikers.Add(textMoniker);
			}
			return textMonikers;
		}
	}
}