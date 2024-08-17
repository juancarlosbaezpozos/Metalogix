using Metalogix.Explorer;
using Metalogix.ExternalConnections;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.ExternalConnections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPAttachment
	{
		private XmlNode m_attachmentXML;

		private SPListItem m_parentItem;

		private byte[] m_binary = null;

		public byte[] Binary
		{
			get
			{
				byte[] mBinary;
				if (this.m_binary != null)
				{
					mBinary = this.m_binary;
				}
				else if (this.BinaryAvailable)
				{
					mBinary = this.GetBinary();
				}
				else
				{
					mBinary = null;
				}
				return mBinary;
			}
			set
			{
				this.m_binary = value;
			}
		}

		public bool BinaryAvailable
		{
			get
			{
				bool flag;
				if (this.m_parentItem.GetExternalConnectionsOfType<StoragePointExternalConnection>(true).Count <= 0)
				{
					flag = ((this.m_attachmentXML.Attributes["BinaryAvailable"] == null ? true : bool.Parse(this.m_attachmentXML.Attributes["BinaryAvailable"].Value)) ? true : false);
				}
				else
				{
					flag = true;
				}
				return flag;
			}
		}

		public string DirName
		{
			get
			{
				string str = string.Concat(this.m_parentItem.ParentList.ServerRelativeUrl, "/Attachments/", this.m_parentItem.ID);
				return str;
			}
		}

		public string FileName
		{
			get
			{
				return this.m_attachmentXML.Attributes["LeafName"].Value;
			}
		}

		internal long FileSize
		{
			get
			{
				long num = (long)0;
				return ((this.m_attachmentXML.Attributes["_FileSize"] == null ? true : !long.TryParse(this.m_attachmentXML.Attributes["_FileSize"].Value, out num)) ? (long)0 : num);
			}
		}

		public string ID
		{
			get
			{
				string value;
				if (this.m_attachmentXML.Attributes["DocID"] == null)
				{
					value = null;
				}
				else
				{
					value = this.m_attachmentXML.Attributes["DocID"].Value;
				}
				return value;
			}
		}

		public bool IsExternalized
		{
			get
			{
				return ((this.m_attachmentXML.Attributes["IsExternalized"] == null ? true : !bool.Parse(this.m_attachmentXML.Attributes["IsExternalized"].Value)) ? false : true);
			}
		}

		public string XML
		{
			get
			{
				return this.m_attachmentXML.OuterXml;
			}
		}

		public SPAttachment(SPListItem parentItem, XmlNode attachmentXML)
		{
			this.m_parentItem = parentItem;
			this.m_attachmentXML = attachmentXML;
		}

		public byte[] GetBinary()
		{
			byte[] document;
			if (this.BinaryAvailable)
			{
				Dictionary<int, ExternalConnection> externalConnectionsOfType = this.m_parentItem.GetExternalConnectionsOfType<StoragePointExternalConnection>(true);
				if (externalConnectionsOfType.Count <= 0)
				{
					document = this.m_parentItem.Adapter.Reader.GetDocument(this.ID, this.DirName, this.FileName, 1);
				}
				else
				{
					KeyValuePair<int, ExternalConnection> keyValuePair = externalConnectionsOfType.First<KeyValuePair<int, ExternalConnection>>();
					document = ((StoragePointExternalConnection)keyValuePair.Value).GetBLOB(this.GetBlobRef());
				}
			}
			else
			{
				document = null;
			}
			return document;
		}

		public byte[] GetBlobRef()
		{
			byte[] documentBlobRef;
			if (this.IsExternalized)
			{
				documentBlobRef = this.m_parentItem.Adapter.Reader.GetDocumentBlobRef(this.ID, this.DirName, this.FileName, 1);
			}
			else
			{
				documentBlobRef = null;
			}
			return documentBlobRef;
		}
	}
}