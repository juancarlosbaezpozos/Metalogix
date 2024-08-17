using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace Metalogix.UI.WinForms.Attributes
{
	public class FormIconAttribute : UIAttribute
	{
		private readonly string m_resourceName;

		public string ResourceName
		{
			get
			{
				return this.m_resourceName;
			}
		}

		public FormIconAttribute(string iconResourceName)
		{
			this.m_resourceName = iconResourceName;
		}

		public Icon GetIcon()
		{
			Icon icon;
			using (Stream manifestResourceStream = Assembly.GetEntryAssembly().GetManifestResourceStream(this.ResourceName))
			{
				icon = new Icon(manifestResourceStream);
			}
			return icon;
		}
	}
}