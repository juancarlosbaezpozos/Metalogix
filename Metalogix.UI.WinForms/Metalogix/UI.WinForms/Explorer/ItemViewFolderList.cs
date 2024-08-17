using Metalogix.Actions;
using Metalogix.Data;
using Metalogix.Explorer;
using Metalogix.ObjectResolution;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Metalogix.UI.WinForms.Explorer
{
	public class ItemViewFolderList : IXMLAbleList, IXmlable, IEnumerable
	{
		private List<ItemViewFolder> m_Folders = new List<ItemViewFolder>();

		public Type CollectionType
		{
			get
			{
				if (this.Count == 0 || this[0] == null)
				{
					return null;
				}
				return this[0].GetType();
			}
		}

		public int Count
		{
			get
			{
				return this.m_Folders.Count;
			}
		}

		public object this[int index]
		{
			get
			{
				return this.m_Folders[index];
			}
		}

		public ItemViewFolderList()
		{
		}

		public ItemViewFolderList(ItemViewFolder[] folders)
		{
			this.m_Folders.AddRange(folders);
		}

		public virtual void FromXML(XmlNode xmlNode)
		{
			if (xmlNode.Name == "ItemViewFolderList")
			{
				foreach (XmlNode xmlNodes in xmlNode.SelectNodes(".//Location"))
				{
					Location location = new Location(xmlNodes);
					this.m_Folders.Add(new ItemViewFolder((Folder)location.GetNode()));
				}
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_Folders.GetEnumerator();
		}

		public NodeCollection ToNodeCollection()
		{
			Node[] viewFolderNode = new Node[this.m_Folders.Count];
			for (int i = 0; i < this.m_Folders.Count; i++)
			{
				viewFolderNode[i] = this.m_Folders[i].ViewFolderNode;
			}
			return new NodeCollection(viewFolderNode);
		}

		public override string ToString()
		{
			return this.ToNodeCollection().ToString();
		}

		public virtual string ToXML()
		{
			StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
			this.ToXML(new XmlTextWriter(stringWriter));
			return stringWriter.ToString();
		}

		public virtual void ToXML(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("ItemViewFolderList");
			for (int i = 0; i < this.m_Folders.Count; i++)
			{
				this.m_Folders[i].ViewFolderNode.Location.ToXML(xmlWriter);
			}
			xmlWriter.WriteEndElement();
		}
	}
}