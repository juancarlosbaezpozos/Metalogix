using Metalogix;
using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace Metalogix.SharePoint.Options.Administration.Navigation
{
	public class ModifiedStatusActionOptions : ActionOptions
	{
		private Set<string> m_ModifiedOptions = new Set<string>();

		public ModifiedStatusActionOptions()
		{
		}

		public void ExportModifiedOptions(Set<string> modifiedOptions)
		{
			if (modifiedOptions == null)
			{
				modifiedOptions = new Set<string>();
			}
			foreach (string mModifiedOption in this.m_ModifiedOptions)
			{
				modifiedOptions.Add(mModifiedOption);
			}
		}

		public override void FromXML(XmlNode xmlNode)
		{
			base.FromXML(xmlNode);
			this.m_ModifiedOptions.FromXML(xmlNode);
		}

		protected void MarkOptionAsModified(PropertyInfo property)
		{
			this.m_ModifiedOptions.Add(property.Name);
		}

		public override void ToXML(XmlWriter xmlTextWriter)
		{
			base.ToXML(xmlTextWriter);
			this.m_ModifiedOptions.ToXML(xmlTextWriter);
		}
	}
}