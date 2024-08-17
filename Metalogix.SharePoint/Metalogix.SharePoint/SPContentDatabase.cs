using System;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPContentDatabase
	{
		public int CurrentSiteCount
		{
			get;
			private set;
		}

		public string DatabaseString
		{
			get
			{
				return string.Format("{0}.{1}", this.Server, this.Name);
			}
		}

		public ulong DiskSizeRequired
		{
			get;
			private set;
		}

		public string DisplayName
		{
			get;
			private set;
		}

		public bool IsReadOnly
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			private set;
		}

		public string Server
		{
			get;
			private set;
		}

		public string Status
		{
			get;
			private set;
		}

		public SPContentDatabase(XmlNode node)
		{
			this.FromXml(node);
		}

		private void FromXml(XmlNode node)
		{
			XmlAttribute itemOf = node.Attributes["Server"];
			this.Server = (itemOf != null ? itemOf.Value : "");
			XmlAttribute xmlAttribute = node.Attributes["Name"];
			this.Name = (xmlAttribute != null ? xmlAttribute.Value : "");
			XmlAttribute itemOf1 = node.Attributes["DisplayName"];
			this.DisplayName = (itemOf1 != null ? itemOf1.Value : "");
			XmlAttribute xmlAttribute1 = node.Attributes["DiskSizeRequired"];
			if (xmlAttribute1 != null)
			{
				ulong num = (ulong)0;
				if (ulong.TryParse(xmlAttribute1.Value, out num))
				{
					this.DiskSizeRequired = num;
				}
			}
			XmlAttribute itemOf2 = node.Attributes["CurrentSiteCount"];
			if (itemOf2 != null)
			{
				int num1 = 0;
				if (int.TryParse(itemOf2.Value, out num1))
				{
					this.CurrentSiteCount = num1;
				}
			}
			XmlAttribute xmlAttribute2 = node.Attributes["IsReadOnly"];
			if (xmlAttribute2 != null)
			{
				bool flag = false;
				if (!bool.TryParse(xmlAttribute2.Value, out flag))
				{
					this.IsReadOnly = false;
				}
				else
				{
					this.IsReadOnly = flag;
				}
			}
			XmlAttribute itemOf3 = node.Attributes["Status"];
			this.Status = (itemOf3 != null ? itemOf3.Value : "");
		}
	}
}