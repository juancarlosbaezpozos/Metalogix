using Metalogix.SharePoint.Adapters;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPAttachmentCollection
	{
		private SPListItem m_parentItem;

		private ArrayList m_data;

		public int Count
		{
			get
			{
				return this.m_data.Count;
			}
		}

		public SPAttachment this[int iIndex]
		{
			get
			{
				return (SPAttachment)this.m_data[iIndex];
			}
		}

		public SPAttachment this[string sFileName]
		{
			get
			{
				SPAttachment sPAttachment;
				foreach (SPAttachment sPAttachment1 in this)
				{
					if (sPAttachment1.FileName == sFileName)
					{
						sPAttachment = sPAttachment1;
						return sPAttachment;
					}
				}
				sPAttachment = null;
				return sPAttachment;
			}
		}

		public SPAttachmentCollection(SPListItem parentItem)
		{
			this.m_parentItem = parentItem;
			this.m_data = new ArrayList();
		}

		public bool ContainsAttachment(string sFileName)
		{
			bool flag;
			foreach (SPAttachment sPAttachment in this)
			{
				if (sPAttachment.FileName == sFileName)
				{
					flag = true;
					return flag;
				}
			}
			flag = false;
			return flag;
		}

		public void FetchData()
		{
			this.m_data.Clear();
			string attachments = this.m_parentItem.Adapter.Reader.GetAttachments(this.m_parentItem.ParentList.ConstantID, this.m_parentItem.ConstantID);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(attachments);
			XmlNode xmlNodes = this.m_parentItem.AttachVirtualData(xmlDocument.FirstChild, "Attachments");
			foreach (XmlNode xmlNodes1 in xmlNodes.SelectNodes("//Attachment"))
			{
				this.m_data.Add(new SPAttachment(this.m_parentItem, xmlNodes1));
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_data.GetEnumerator();
		}

		public byte[][] GetFileContents()
		{
			long num = (long)0;
			return this.GetFileContents(false, out num);
		}

		public byte[][] GetFileContents(bool bGetRefsIfExternalized, out long lBytes)
		{
			byte[][] numArray = new byte[this.Count][];
			lBytes = (long)0;
			for (int i = 0; i < this.Count; i++)
			{
				SPAttachment item = this[i];
				byte[] numArray1 = (!bGetRefsIfExternalized || !item.IsExternalized || !SharePointConfigurationVariables.AllowDBWriting || item.BinaryAvailable ? item.Binary : item.GetBlobRef());
				numArray[i] = numArray1;
				if (!item.IsExternalized)
				{
					lBytes += (long)((int)numArray1.Length);
				}
				else
				{
					lBytes += item.FileSize;
				}
			}
			return numArray;
		}

		public string[] GetFileNames()
		{
			string[] fileName = new string[this.m_data.Count];
			for (int i = 0; i < this.Count; i++)
			{
				fileName[i] = this[i].FileName;
			}
			return fileName;
		}

		public void UpdateAttachments(string[] fileNames, byte[][] fileContents)
		{
			if ((fileNames == null ? false : (int)fileNames.Length > 0))
			{
				if (!this.m_parentItem.WriteVirtually)
				{
					this.m_parentItem.ParentList.Adapter.Writer.UpdateListItem(this.m_parentItem.ParentList.ConstantID.ToString(), this.m_parentItem.ParentFolder.ToString(), this.m_parentItem.ConstantID, this.m_parentItem.XML, fileNames, fileContents, new UpdateListItemOptions());
					this.FetchData();
				}
				else
				{
					StringBuilder stringBuilder = new StringBuilder();
					int num = 0;
					string[] strArrays = fileNames;
					for (int i = 0; i < (int)strArrays.Length; i++)
					{
						string str = strArrays[i];
						SPAttachment item = this[str];
						if (item == null)
						{
							stringBuilder.Append(item.XML);
						}
						else
						{
							string str1 = string.Format("<Attachment LeafName=\"{0}\" />", str);
							stringBuilder.Append(str1);
							item = new SPAttachment(this.m_parentItem, XmlUtility.StringToXmlNode(str1));
							this.m_data.Add(item);
						}
						item.Binary = fileContents[num];
						num++;
					}
					StringBuilder stringBuilder1 = new StringBuilder();
					foreach (SPAttachment sPAttachment in this)
					{
						stringBuilder1.Append(sPAttachment.XML);
					}
					string str2 = "<Attachments>{0}</Attachments>";
					XmlNode xmlNode = XmlUtility.StringToXmlNode(string.Format(str2, stringBuilder1.ToString()));
					XmlNode xmlNodes = XmlUtility.StringToXmlNode(string.Format(str2, stringBuilder.ToString()));
					this.m_parentItem.SaveVirtualData(xmlNode, xmlNodes, "Attachments");
				}
			}
		}
	}
}