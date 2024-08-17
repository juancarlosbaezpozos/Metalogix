using Metalogix.SharePoint;
using Metalogix.SharePoint.Properties;
using Metalogix.Utilities;
using Microsoft.Deployment.Compression;
using Microsoft.Deployment.Compression.Cab;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Metalogix.SharePoint.Migration
{
	public class InfoPathTemplate
	{
		private const string ManifestFileName = "manifest.xsf";

		private bool? m_bIsTemplateBrowserActivated = null;

		private byte[] m_fileBytes = null;

		private string m_absoluteUrl = "";

		public InfoPathTemplate(byte[] fileBytes, string absoluteUrl)
		{
			if (fileBytes == null)
			{
				throw new ArgumentNullException("fileBytes");
			}
			if ((int)fileBytes.Length == 0)
			{
				throw new ArgumentNullException("fileBytes");
			}
			if (string.IsNullOrEmpty(absoluteUrl))
			{
				throw new ArgumentNullException("absoluteUrl");
			}
			this.m_fileBytes = fileBytes;
			this.m_absoluteUrl = absoluteUrl;
		}

		public string GetChangedFields(LinkCorrector linkCorrector, InfoPathTemplate.FormDefinitionFile file)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<Fields>");
			foreach (InfoPathTemplate.FormDefinitionFile.LookUpField lookUpFiedsForReLink in file.GetLookUpFiedsForReLink())
			{
				stringBuilder.Append("<Field");
				stringBuilder.Append(string.Concat(" Type=\"", lookUpFiedsForReLink.Field.Type, "\""));
				stringBuilder.Append(string.Concat(" Name=\"", lookUpFiedsForReLink.Field.InternalName, "\""));
				stringBuilder.Append(string.Concat(" DisplayName=\"", lookUpFiedsForReLink.Field.InternalName, "\""));
				stringBuilder.Append(string.Concat(" Mult=\"", (lookUpFiedsForReLink.Field.IsMulti ? "TRUE" : "FALSE"), "\""));
				stringBuilder.Append(string.Concat(" Required=\"", lookUpFiedsForReLink.Field.Required, "\""));
				stringBuilder.Append(string.Concat(" ShowField=\"", lookUpFiedsForReLink.Field.SourceDataObjectFieldName, "\""));
				stringBuilder.Append(string.Concat(" List=\"", lookUpFiedsForReLink.DataObject.GetAdapter().ListID, "\""));
				stringBuilder.Append(" ></Field>");
			}
			stringBuilder.Append("</Fields>");
			return stringBuilder.ToString();
		}

		public string GetReLinkedLookupFields(LinkCorrector linkCorrector, InfoPathTemplate.FormDefinitionFile file)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<Fields>");
			foreach (InfoPathTemplate.FormDefinitionFile.LookUpField lookUpFiedsForReLink in file.GetLookUpFiedsForReLink())
			{
				stringBuilder.Append("<Field");
				stringBuilder.Append(string.Concat(" Type=\"", lookUpFiedsForReLink.Field.Type, "\""));
				stringBuilder.Append(string.Concat(" Name=\"", lookUpFiedsForReLink.Field.InternalName, "\""));
				stringBuilder.Append(string.Concat(" Mult=\"", (lookUpFiedsForReLink.Field.IsMulti ? "TRUE" : "FALSE"), "\""));
				stringBuilder.Append(string.Concat(" Required=\"", lookUpFiedsForReLink.Field.Required, "\""));
				stringBuilder.Append(string.Concat(" ShowField=\"", lookUpFiedsForReLink.Field.SourceDataObjectFieldName, "\""));
				stringBuilder.Append(string.Concat(" List=\"", lookUpFiedsForReLink.DataObject.GetAdapter().ListID, "\""));
				stringBuilder.Append(" xmlns=\"http://schemas.microsoft.com/office/infopath/2007/formsServices\"");
				stringBuilder.Append(" ></Field>");
			}
			stringBuilder.Append("</Fields>");
			return stringBuilder.ToString();
		}

		public byte[] GetReLinkedTemplate(LinkCorrector linkCorrector, out string changedLookupFields, SPList list, SPContentType contentType)
		{
			string str;
			byte[] numArray;
			byte[] reLinkedTemplate = this.GetReLinkedTemplate(linkCorrector, out str);
			string str1 = (new InfoPathTemplate(reLinkedTemplate, this.m_absoluteUrl)).Unpack();
			string str2 = Path.Combine(str1, "manifest.xsf");
			InfoPathTemplate.FormDefinitionFile formDefinitionFile = new InfoPathTemplate.FormDefinitionFile(File.ReadAllText(str2));
			try
			{
				StringBuilder stringBuilder = new StringBuilder(str);
				if (File.Exists(str2))
				{
					this.LinkCorrectManifestFile(formDefinitionFile, str2, list, contentType, stringBuilder);
				}
				changedLookupFields = stringBuilder.ToString();
				numArray = this.Pack(str1);
			}
			finally
			{
				this.TryDeleteDirectory(str1);
			}
			return numArray;
		}

		public byte[] GetReLinkedTemplate(LinkCorrector linkCorrector, out string changedLookupFields)
		{
			string str = this.Unpack();
			byte[] numArray = null;
			string str1 = linkCorrector.CorrectUrl(this.m_absoluteUrl);
			string str2 = Path.Combine(str, "manifest.xsf");
			InfoPathTemplate.FormDefinitionFile formDefinitionFile = new InfoPathTemplate.FormDefinitionFile(File.ReadAllText(str2));
			this.m_bIsTemplateBrowserActivated = new bool?(formDefinitionFile.IsTemplateBrowserActivated());
			Uri uri = new Uri(this.m_absoluteUrl);
			Uri uri1 = new Uri(str1);
			StringBuilder stringBuilder = new StringBuilder(1024);
			StringBuilder stringBuilder1 = new StringBuilder(1024);
			bool flag = false;
			foreach (InfoPathTemplate.SPReference reference in formDefinitionFile.References)
			{
				flag = false;
				stringBuilder.AppendLine(Resources.InfoPathLogAttemptingToCorrectRefSplitter);
				if (reference.IsUrl)
				{
					string absoluteUrl = reference.GetAbsoluteUrl(uri);
					string relativeUrl = linkCorrector.CorrectUrl(absoluteUrl);
					if (absoluteUrl.EndsWith("/"))
					{
						if (!relativeUrl.EndsWith("/"))
						{
							relativeUrl = string.Concat(relativeUrl, "/");
						}
					}
					if (!reference.IsAbsoluteUrl)
					{
						relativeUrl = reference.GetRelativeUrl(relativeUrl, uri1);
					}
					if (!string.Equals(reference.Value, relativeUrl))
					{
						stringBuilder.Append(string.Format(Resources.FS_InfoPathLogStart, "URL", reference.ToString()));
						reference.Value = relativeUrl;
						stringBuilder.AppendLine(string.Format(Resources.FS_InfoPathLogEnd, reference.ToString()));
						flag = true;
					}
				}
				else if (!(!reference.IsGuid ? true : reference.IsContentTypeID))
				{
					string str3 = linkCorrector.MapGuid(reference.Value);
					if (!string.IsNullOrEmpty(str3))
					{
						if ((!reference.Value.EndsWith("}") ? false : !str3.EndsWith("}")))
						{
							str3 = string.Concat("{", str3, "}");
						}
						stringBuilder.Append(string.Format(Resources.FS_InfoPathLogStart, "GUID", reference.ToString()));
						reference.Value = str3;
						stringBuilder.AppendLine(string.Format(Resources.FS_InfoPathLogEnd, reference.ToString()));
						flag = true;
					}
				}
				else if (reference.IsContentTypeID)
				{
					string str4 = linkCorrector.MapString(reference.Value);
					if (!string.IsNullOrEmpty(str4))
					{
						if ((!reference.Value.EndsWith("}") ? false : !str4.EndsWith("}")))
						{
							str4 = string.Concat("{", str4, "}");
						}
						stringBuilder.Append(string.Format(Resources.FS_InfoPathLogStart, "CT", reference.ToString()));
						reference.Value = str4;
						stringBuilder.AppendLine(string.Format(Resources.FS_InfoPathLogEnd, reference.ToString()));
						flag = true;
					}
				}
				if (!flag)
				{
					stringBuilder.AppendLine(string.Format(Resources.FS_InfoPathRefNotChanged, reference.ToString()));
				}
				if ((!flag ? false : !reference.Name.StartsWith("xmlns:", StringComparison.OrdinalIgnoreCase)))
				{
					stringBuilder.AppendLine(string.Format(Resources.FS_Xml, reference.NodeXml));
				}
				stringBuilder.AppendLine();
			}
			changedLookupFields = stringBuilder.ToString();
			File.WriteAllText(str2, formDefinitionFile.ToXml(false));
			numArray = this.Pack(str);
			this.TryDeleteDirectory(str);
			return numArray;
		}

		public bool IsTemplateBrowserActivated()
		{
			if (!this.m_bIsTemplateBrowserActivated.HasValue)
			{
				bool flag = false;
				string str = this.Unpack();
				string str1 = Path.Combine(str, "manifest.xsf");
				flag = (new InfoPathTemplate.FormDefinitionFile(File.ReadAllText(str1))).IsTemplateBrowserActivated();
				this.TryDeleteDirectory(str);
				this.m_bIsTemplateBrowserActivated = new bool?(flag);
			}
			return this.m_bIsTemplateBrowserActivated.Value;
		}

		private void LinkCorrectManifestFile(InfoPathTemplate.FormDefinitionFile file, string manifestFile, SPList list, SPContentType contentType, StringBuilder changeLog)
		{
			foreach (InfoPathTemplate.FormDefinitionFile.SPListAdapterRW sPListAdapter in file.SPListAdapters)
			{
				string listID = sPListAdapter.ListID;
				if ((listID == null ? false : list != null))
				{
					string str = (new Guid(list.ID)).ToString("B");
					changeLog.AppendFormat("Updating sharepointListAdapterRW.sharePointListID from '{0}' to '{1}'.", listID, str).AppendLine();
					sPListAdapter.ListID = str;
				}
				string contentTypeID = sPListAdapter.ContentTypeID;
				if ((contentTypeID == null ? false : contentType != null))
				{
					changeLog.AppendFormat("Updating sharepointListAdapterRW.contentTypeID from '{0}' to '{1}'.", contentTypeID, contentType.ContentTypeID).AppendLine();
					sPListAdapter.ContentTypeID = contentType.ContentTypeID;
				}
			}
			File.WriteAllText(manifestFile, file.ToXml(false));
		}

		private byte[] Pack(string dirPath)
		{
			byte[] numArray = null;
			string tempFileName = Path.GetTempFileName();
			try
			{
				try
				{
					(new CabInfo(tempFileName)).Pack(dirPath, false, CompressionLevel.Max, null);
					numArray = File.ReadAllBytes(tempFileName);
				}
				catch (Exception exception)
				{
					throw new Exception("InfoPath template packing failed", exception);
				}
			}
			finally
			{
				this.TryDeleteFile(tempFileName);
			}
			return numArray;
		}

		private bool TryDeleteDirectory(string directoryFilePath)
		{
			bool flag = false;
			if (!string.IsNullOrEmpty(directoryFilePath))
			{
				try
				{
					Directory.Delete(directoryFilePath, true);
					flag = true;
				}
				catch (Exception exception)
				{
				}
			}
			return flag;
		}

		private bool TryDeleteFile(string filePath)
		{
			bool flag = false;
			if (!string.IsNullOrEmpty(filePath))
			{
				try
				{
					File.Delete(filePath);
					flag = true;
				}
				catch (Exception exception)
				{
				}
			}
			return flag;
		}

		private string Unpack()
		{
			string tempFileName = null;
			string tempPath = "";
			try
			{
				tempFileName = Path.GetTempFileName();
				FileStream fileStream = File.Open(tempFileName, FileMode.Open);
				fileStream.Write(this.m_fileBytes, 0, (int)this.m_fileBytes.Length);
				fileStream.Flush();
				fileStream.Close();
				tempPath = Path.GetTempPath();
				Guid guid = Guid.NewGuid();
				tempPath = Path.Combine(tempPath, guid.ToString());
				(new CabInfo(tempFileName)).Unpack(tempPath);
			}
			catch (Exception exception)
			{
				throw new Exception("InfoPath template unpacking failed.", exception);
			}
			return tempPath;
		}

		public class FormDefinitionFile
		{
			private object m_lock;

			private XmlDocument m_xDoc;

			private XmlNamespaceManager m_nsMgr;

			private List<InfoPathTemplate.FormDefinitionFile.SPListAdapterRW> m_SPListAdapters;

			private List<InfoPathTemplate.SPReference> m_references;

			private List<InfoPathTemplate.FormDefinitionFile.DataObject> m_dataObjects;

			public ReadOnlyCollection<InfoPathTemplate.SPReference> References
			{
				get
				{
					lock (this.m_lock)
					{
						if (this.m_references == null)
						{
							this.m_references = new List<InfoPathTemplate.SPReference>();
							this.FillXmlReferences(this.m_xDoc.DocumentElement);
						}
					}
					return this.m_references.AsReadOnly();
				}
			}

			public ReadOnlyCollection<InfoPathTemplate.FormDefinitionFile.SPListAdapterRW> SPListAdapters
			{
				get
				{
					ReadOnlyCollection<InfoPathTemplate.FormDefinitionFile.SPListAdapterRW> sPListAdapterRWs;
					if (this.m_SPListAdapters == null)
					{
						XmlNodeList xmlNodeLists = this.m_xDoc.DocumentElement.SelectNodes("//xsf:xDocumentClass/xsf:query/xsf:sharepointListAdapterRW", this.m_nsMgr);
						this.m_SPListAdapters = new List<InfoPathTemplate.FormDefinitionFile.SPListAdapterRW>();
						foreach (XmlNode xmlNodes in xmlNodeLists)
						{
							this.m_SPListAdapters.Add(new InfoPathTemplate.FormDefinitionFile.SPListAdapterRW(this, xmlNodes));
						}
						sPListAdapterRWs = this.m_SPListAdapters.AsReadOnly();
					}
					else
					{
						sPListAdapterRWs = this.m_SPListAdapters.AsReadOnly();
					}
					return sPListAdapterRWs;
				}
			}

			public FormDefinitionFile(string xml)
			{
				this.m_xDoc = new XmlDocument();
				this.m_xDoc.LoadXml(xml);
				this.m_nsMgr = new XmlNamespaceManager(this.m_xDoc.NameTable);
				this.m_nsMgr.AddNamespace("xsf", "http://schemas.microsoft.com/office/infopath/2003/solutionDefinition");
				this.m_nsMgr.AddNamespace("xsf2", "http://schemas.microsoft.com/office/infopath/2006/solutionDefinition/extensions");
				this.m_nsMgr.AddNamespace("xsf3", "http://schemas.microsoft.com/office/infopath/2009/solutionDefinition/extensions");
				this.m_nsMgr.AddNamespace("msxsl", "urn:schemas-microsoft-com:xslt");
				this.m_nsMgr.AddNamespace("xd", "http://schemas.microsoft.com/office/infopath/2003");
				this.m_nsMgr.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
				this.m_nsMgr.AddNamespace("xdUtil", "http://schemas.microsoft.com/office/infopath/2003/xslt/Util");
				this.m_nsMgr.AddNamespace("xdXDocument", "http://schemas.microsoft.com/office/infopath/2003/xslt/xDocument");
				this.m_nsMgr.AddNamespace("xdMath", "http://schemas.microsoft.com/office/infopath/2003/xslt/Math");
				this.m_nsMgr.AddNamespace("xdDate", "http://schemas.microsoft.com/office/infopath/2003/xslt/Date");
				this.m_nsMgr.AddNamespace("xdExtension", "http://schemas.microsoft.com/office/infopath/2003/xslt/extension");
				this.m_nsMgr.AddNamespace("xdEnvironment", "http://schemas.microsoft.com/office/infopath/2006/xslt/environment");
				this.m_nsMgr.AddNamespace("xdUser", "http://schemas.microsoft.com/office/infopath/2006/xslt/User");
				this.m_nsMgr.AddNamespace("my", "http://schemas.microsoft.com/office/infopath/2009/WSSList/cmeDataFields");
				this.m_nsMgr.AddNamespace("dfs", "http://schemas.microsoft.com/office/infopath/2003/dataFormSolution");
				this.m_nsMgr.AddNamespace("d", "http://schemas.microsoft.com/office/infopath/2009/WSSList/dataFields");
				this.m_nsMgr.AddNamespace("pc", "http://schemas.microsoft.com/office/infopath/2007/PartnerControls");
				this.m_nsMgr.AddNamespace("xdServerInfo", "http://schemas.microsoft.com/office/infopath/2009/xslt/ServerInfo");
				this.m_nsMgr.AddNamespace("ma", "http://schemas.microsoft.com/office/2009/metadata/properties/metaAttributes");
				this.m_nsMgr.AddNamespace("q", "http://schemas.microsoft.com/office/infopath/2009/WSSList/queryFields");
				this.m_nsMgr.AddNamespace("dms", "http://schemas.microsoft.com/office/2009/documentManagement/types");
			}

			public ReadOnlyCollection<InfoPathTemplate.FormDefinitionFile.DataObject> DataObjects()
			{
				lock (this.m_lock)
				{
					if (this.m_dataObjects == null)
					{
						this.m_dataObjects = new List<InfoPathTemplate.FormDefinitionFile.DataObject>();
						this.FillXmlDataObjects();
					}
				}
				return this.m_dataObjects.AsReadOnly();
			}

			private void FillXmlDataObjects()
			{
				if (this.m_xDoc != null)
				{
					foreach (XmlNode xmlNodes in this.m_xDoc.DocumentElement.SelectNodes("//xsf:xDocumentClass/xsf:dataObjects/xsf:dataObject", this.m_nsMgr))
					{
						this.m_dataObjects.Add(new InfoPathTemplate.FormDefinitionFile.DataObject(this, xmlNodes));
					}
				}
			}

			private void FillXmlReferences(XmlNode xmlElement)
			{
				if (xmlElement != null)
				{
					if (InfoPathTemplate.SPReference.IsSPReference(xmlElement.Name, xmlElement.Value))
					{
						this.m_references.Add(new InfoPathTemplate.SPReference(this, xmlElement, null));
					}
					if (xmlElement.Attributes != null)
					{
						foreach (XmlAttribute attribute in xmlElement.Attributes)
						{
							if (InfoPathTemplate.SPReference.IsSPReference(attribute.Name, attribute.Value))
							{
								this.m_references.Add(new InfoPathTemplate.SPReference(this, xmlElement, attribute.Name));
							}
						}
					}
					if (xmlElement.ChildNodes != null)
					{
						foreach (XmlNode childNode in xmlElement.ChildNodes)
						{
							this.FillXmlReferences(childNode);
						}
					}
				}
			}

			public ReadOnlyCollection<InfoPathTemplate.FormDefinitionFile.LookUpField> GetLookUpFiedsForReLink()
			{
				List<InfoPathTemplate.FormDefinitionFile.LookUpField> lookUpFields = new List<InfoPathTemplate.FormDefinitionFile.LookUpField>();
				foreach (InfoPathTemplate.FormDefinitionFile.SPListAdapterRW sPListAdapter in this.SPListAdapters)
				{
					foreach (InfoPathTemplate.FormDefinitionFile.SPListAdapterRW.Field field in sPListAdapter.Fields)
					{
						if (field.IsLookupField)
						{
							foreach (InfoPathTemplate.FormDefinitionFile.DataObject dataObject in this.DataObjects())
							{
								if (field.SourceDataObjectName.Equals(dataObject.Name, StringComparison.OrdinalIgnoreCase))
								{
									lookUpFields.Add(new InfoPathTemplate.FormDefinitionFile.LookUpField(field, dataObject, this.m_nsMgr));
								}
							}
						}
					}
				}
				return lookUpFields.AsReadOnly();
			}

			public bool IsTemplateBrowserActivated()
			{
				bool flag = false;
				XmlNode xmlNodes = this.m_xDoc.SelectSingleNode("//xsf2:solutionPropertiesExtension/xsf2:wss", this.m_nsMgr);
				if (xmlNodes != null)
				{
					if (xmlNodes.Attributes["browserEnable"] != null)
					{
						if (xmlNodes.Attributes["browserEnable"].Value.Equals("yes", StringComparison.OrdinalIgnoreCase))
						{
							flag = true;
						}
					}
				}
				return flag;
			}

			public string ToXml(bool formatted)
			{
				string str;
				if (formatted)
				{
					StringBuilder stringBuilder = new StringBuilder();
					XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder));
					try
					{
						xmlTextWriter.Formatting = Formatting.Indented;
						this.m_xDoc.Save(xmlTextWriter);
					}
					finally
					{
						if (xmlTextWriter != null)
						{
							((IDisposable)xmlTextWriter).Dispose();
						}
					}
					str = stringBuilder.ToString();
				}
				else
				{
					str = this.m_xDoc.OuterXml;
				}
				return str;
			}

			public string ToXml()
			{
				return this.ToXml(false);
			}

			public class DataObject
			{
				private InfoPathTemplate.FormDefinitionFile m_file;

				private XmlNamespaceManager m_xmlnm;

				private XmlNode m_dataObjectNode;

				public string Name
				{
					get
					{
						string value = "";
						XmlAttribute attribute = XmlUtility.GetAttribute(this.m_dataObjectNode, null, "Name", false);
						if (attribute != null)
						{
							value = attribute.Value;
						}
						return value;
					}
				}

				public XmlNode sharepointListAdapterNode
				{
					get
					{
						return this.m_dataObjectNode.SelectSingleNode("//xsf:dataObject/xsf:query/xsf:sharepointListAdapterRW", this.m_xmlnm);
					}
				}

				public DataObject(InfoPathTemplate.FormDefinitionFile file, XmlNode dataObjectNode)
				{
					if (dataObjectNode == null)
					{
						throw new ArgumentNullException("dataObjectNode");
					}
					this.m_dataObjectNode = dataObjectNode;
					this.m_xmlnm = file.m_nsMgr;
					this.m_file = file;
				}

				public InfoPathTemplate.FormDefinitionFile.SPListAdapterRW GetAdapter()
				{
					return new InfoPathTemplate.FormDefinitionFile.SPListAdapterRW(this.m_file, this.sharepointListAdapterNode);
				}
			}

			public class LookUpField
			{
				private XmlNamespaceManager m_xnm;

				public InfoPathTemplate.FormDefinitionFile.DataObject DataObject
				{
					get;
					set;
				}

				public InfoPathTemplate.FormDefinitionFile.SPListAdapterRW.Field Field
				{
					get;
					set;
				}

				public LookUpField(InfoPathTemplate.FormDefinitionFile.SPListAdapterRW.Field field, InfoPathTemplate.FormDefinitionFile.DataObject dataObject, XmlNamespaceManager xnm)
				{
					this.Field = field;
					this.DataObject = dataObject;
					this.m_xnm = xnm;
				}
			}

			public class SPListAdapterRW
			{
				private InfoPathTemplate.FormDefinitionFile m_definitionFile;

				private XmlNode m_xNode;

				private XmlNamespaceManager m_nsMgr;

				private List<InfoPathTemplate.FormDefinitionFile.SPListAdapterRW.Field> m_fields;

				private object m_lock;

				public string ContentTypeID
				{
					get
					{
						return (this.m_xNode.Attributes["contentTypeID"] != null ? this.m_xNode.Attributes["contentTypeID"].Value : string.Empty);
					}
					set
					{
						if (this.m_xNode.Attributes["contentTypeID"] == null)
						{
							throw new Exception("Attribute doesn't exist.");
						}
						this.m_xNode.Attributes["contentTypeID"].Value = value;
					}
				}

				public ReadOnlyCollection<InfoPathTemplate.FormDefinitionFile.SPListAdapterRW.Field> Fields
				{
					get
					{
						lock (this.m_lock)
						{
							if (this.m_fields == null)
							{
								this.m_fields = new List<InfoPathTemplate.FormDefinitionFile.SPListAdapterRW.Field>();
								this.FillFields();
							}
						}
						return this.m_fields.AsReadOnly();
					}
				}

				public string ListID
				{
					get
					{
						return (this.m_xNode.Attributes["sharePointListID"] != null ? this.m_xNode.Attributes["sharePointListID"].Value : string.Empty);
					}
					set
					{
						if (this.m_xNode.Attributes["sharePointListID"] == null)
						{
							throw new Exception("Attribute doesn't exist.");
						}
						this.m_xNode.Attributes["sharePointListID"].Value = value;
					}
				}

				public string RelativeListUrl
				{
					get
					{
						return (this.m_xNode.Attributes["relativeListUrl"] != null ? this.m_xNode.Attributes["relativeListUrl"].Value : string.Empty);
					}
					set
					{
						if (this.m_xNode.Attributes["relativeListUrl"] == null)
						{
							throw new Exception("Attribute doesn't exist.");
						}
						this.m_xNode.Attributes["relativeListUrl"].Value = value;
					}
				}

				public string SiteURL
				{
					get
					{
						return (this.m_xNode.Attributes["siteURL"] != null ? this.m_xNode.Attributes["siteURL"].Value : string.Empty);
					}
					set
					{
						if (this.m_xNode.Attributes["siteURL"] == null)
						{
							throw new Exception("Attribute doesn't exist.");
						}
						this.m_xNode.Attributes["siteURL"].Value = value;
					}
				}

				internal SPListAdapterRW(InfoPathTemplate.FormDefinitionFile parentDefinitionFile, XmlNode node)
				{
					this.m_definitionFile = parentDefinitionFile;
					this.m_xNode = node;
					this.m_nsMgr = parentDefinitionFile.m_nsMgr;
				}

				private void FillFields()
				{
					foreach (XmlNode xmlNodes in this.m_xNode.SelectNodes("//xsf:field", this.m_nsMgr))
					{
						this.m_fields.Add(new InfoPathTemplate.FormDefinitionFile.SPListAdapterRW.Field(xmlNodes));
					}
				}

				public class Field
				{
					private XmlNode m_fieldNode;

					public string InternalName
					{
						get
						{
							string value = XmlUtility.GetAttribute(this.m_fieldNode, null, "internalName", false).Value;
							return value;
						}
					}

					public bool IsLookupField
					{
						get
						{
							bool flag = false;
							string type = this.Type;
							if ((type.Equals("LookupMulti", StringComparison.OrdinalIgnoreCase) ? true : type.Equals("Lookup", StringComparison.OrdinalIgnoreCase)))
							{
								flag = true;
							}
							return flag;
						}
					}

					public bool IsMulti
					{
						get
						{
							bool flag = false;
							if (this.Type.Equals("LookupMulti", StringComparison.OrdinalIgnoreCase))
							{
								flag = true;
							}
							return flag;
						}
					}

					public string Required
					{
						get
						{
							string str = "FALSE";
							XmlAttribute attribute = XmlUtility.GetAttribute(this.m_fieldNode, null, "required", false);
							if (attribute != null)
							{
								str = (attribute.Value.Equals("yes") ? "TRUE" : "FALSE");
							}
							return str;
						}
					}

					public string SourceDataObjectFieldName
					{
						get
						{
							string value = "";
							if (this.IsLookupField)
							{
								value = XmlUtility.GetAttribute(this.m_fieldNode, null, "showFieldName", false).Value;
							}
							return value;
						}
					}

					public string SourceDataObjectName
					{
						get
						{
							string value = "";
							if (this.IsLookupField)
							{
								value = XmlUtility.GetAttribute(this.m_fieldNode, null, "auxDomName", false).Value;
							}
							return value;
						}
					}

					public string Type
					{
						get
						{
							string value = XmlUtility.GetAttribute(this.m_fieldNode, null, "type", false).Value;
							return value;
						}
					}

					public Field(XmlNode fieldNode)
					{
						this.m_fieldNode = fieldNode;
					}
				}
			}
		}

		public class SPReference
		{
			private InfoPathTemplate.FormDefinitionFile m_definitionFile;

			private XmlNode m_xNode;

			private string m_attributeName;

			private readonly static Regex s_guidRegEx;

			private readonly static Regex s_ctidRegEx;

			public bool IsAbsoluteUrl
			{
				get
				{
					string value = this.Value;
					return (!this.IsUrl ? false : value.StartsWith("http", StringComparison.OrdinalIgnoreCase));
				}
			}

			public bool IsAttribute
			{
				get
				{
					return !string.IsNullOrEmpty(this.m_attributeName);
				}
			}

			public bool IsContentTypeID
			{
				get
				{
					return InfoPathTemplate.SPReference.s_ctidRegEx.IsMatch(this.Value);
				}
			}

			public bool IsGuid
			{
				get
				{
					return InfoPathTemplate.SPReference.s_guidRegEx.IsMatch(this.Value);
				}
			}

			public bool IsUrl
			{
				get
				{
					return InfoPathTemplate.SPReference.IsHtmlLink(this.Value);
				}
			}

			public string Name
			{
				get
				{
					return (this.IsAttribute ? this.m_xNode.Attributes[this.m_attributeName].Name : this.m_xNode.Name);
				}
			}

			public string NodeXml
			{
				get
				{
					return this.m_xNode.OuterXml;
				}
			}

			public string Value
			{
				get
				{
					return (this.IsAttribute ? this.m_xNode.Attributes[this.m_attributeName].Value : this.m_xNode.Value);
				}
				set
				{
					if (!this.IsAttribute)
					{
						this.m_xNode.Value = value;
					}
					else
					{
						this.m_xNode.Attributes[this.m_attributeName].Value = value;
					}
				}
			}

			static SPReference()
			{
				InfoPathTemplate.SPReference.s_guidRegEx = new Regex("^(\\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\\}{0,1})$");
				InfoPathTemplate.SPReference.s_ctidRegEx = new Regex("^(0x([0-9a-fA-F]+))$");
			}

			internal SPReference(InfoPathTemplate.FormDefinitionFile parentDefinitionFile, XmlNode node, string attributeName)
			{
				this.m_definitionFile = parentDefinitionFile;
				this.m_xNode = node;
				this.m_attributeName = attributeName;
			}

			public string GetAbsoluteUrl(Uri baseUrl)
			{
				string str;
				if (!this.IsUrl)
				{
					throw new Exception("The reference is not a URL.");
				}
				str = (!this.IsAbsoluteUrl ? (new Uri(baseUrl, this.Value)).ToString() : this.Value);
				return str;
			}

			public string GetRelativeUrl(string absoluteUrl, Uri baseUrl)
			{
				string str = baseUrl.ToString();
				int num = str.LastIndexOf('/');
				string str1 = str.Substring(0, num + 1);
				Uri uri = new Uri(str1);
				return uri.MakeRelativeUri(new Uri(absoluteUrl)).ToString();
			}

			private static bool IsHtmlLink(string v)
			{
				bool flag;
				bool flag1 = (v.StartsWith("http:", StringComparison.OrdinalIgnoreCase) ? true : v.StartsWith("https:", StringComparison.OrdinalIgnoreCase));
				if (!flag1)
				{
					if (string.Equals(v, ".."))
					{
						flag = false;
						return flag;
					}
					if (v.StartsWith("..", StringComparison.OrdinalIgnoreCase))
					{
						if (!Regex.IsMatch(v, "\\.\\.\\/\\w+:"))
						{
							flag1 = true;
						}
					}
					else if (v.StartsWith("/", StringComparison.OrdinalIgnoreCase))
					{
						if (!Regex.IsMatch(v, "\\/\\w+:|\\/\\w+\\/@"))
						{
							flag1 = true;
						}
					}
				}
				flag = flag1;
				return flag;
			}

			internal static bool IsSPReference(string name, string value)
			{
				bool flag = false;
				if (!(string.IsNullOrEmpty(value) ? false : !string.IsNullOrEmpty(name)))
				{
					flag = false;
				}
				else if ((value.StartsWith("http://schemas.microsoft.com", StringComparison.OrdinalIgnoreCase) || value.StartsWith("http://www.w3.org/", StringComparison.OrdinalIgnoreCase) || value.StartsWith("/dfs", StringComparison.OrdinalIgnoreCase) ? false : !value.StartsWith("/my:myFields/", StringComparison.OrdinalIgnoreCase)))
				{
					flag = (InfoPathTemplate.SPReference.IsHtmlLink(value) || InfoPathTemplate.SPReference.s_guidRegEx.IsMatch(value) ? true : InfoPathTemplate.SPReference.s_ctidRegEx.IsMatch(value));
				}
				else
				{
					flag = false;
				}
				return flag;
			}

			public string SetAsRelativeUrl(string absoluteUrl, Uri baseUrl)
			{
				this.Value = this.GetRelativeUrl(absoluteUrl, baseUrl);
				return this.Value;
			}

			public override string ToString()
			{
				return (this.IsAttribute ? string.Format("{0}[{1}] = {2}", this.m_xNode.Name, this.m_attributeName, this.Value) : string.Format("{0}.Value = {1}", this.m_xNode.Name, this.Value));
			}
		}
	}
}