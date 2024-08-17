using Metalogix;
using Metalogix.Actions;
using Metalogix.DataStructures;
using Metalogix.Explorer;
using Metalogix.Metabase;
using Metalogix.Permissions;
using Metalogix.SharePoint.Adapters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPSearchResult : SPNode, ListItem, Metalogix.Explorer.Node, Metalogix.DataStructures.IComparable, ICustomTypeDescriptor
	{
		private bool m_bFetchFailed = false;

		private Metalogix.Explorer.Location m_searchLocation = null;

		private SPNode m_baseNode = null;

		private XmlNode m_resultsXML = null;

		private readonly static CultureInfo s_cultureInfo;

		private Metalogix.Explorer.Location m_linkedLocation = null;

		private SPNode BaseNode
		{
			get
			{
				SPNode sPNode;
				sPNode = (this.m_baseNode != null ? this.m_baseNode : this.m_searchLocation.GetNode() as SPNode);
				return sPNode;
			}
		}

		public override Metalogix.Permissions.Credentials Credentials
		{
			get
			{
				Metalogix.Permissions.Credentials credentials;
				if (!this.HasNode)
				{
					credentials = null;
				}
				else
				{
					credentials = this.Node.Credentials;
				}
				return credentials;
			}
		}

		public override string DisplayName
		{
			get
			{
				string str;
				str = (!this.HasNode ? this.m_resultsXML.Attributes[""].Value : this.Node.DisplayName);
				return str;
			}
		}

		public override string DisplayUrl
		{
			get
			{
				string str;
				str = (!this.HasNode ? this.Node.Url : this.Node.DisplayUrl);
				return str;
			}
		}

		public bool FetchFailed
		{
			get
			{
				return this.m_bFetchFailed;
			}
		}

		public bool HasNode
		{
			get
			{
				return this.m_linkedLocation != null;
			}
		}

		public override System.Drawing.Image Image
		{
			get
			{
				System.Drawing.Image image;
				image = (this.ResultType != typeof(SPListItem) ? ImageCache.GetImage(this.ImageName, base.GetType().Assembly) : ImageCache.GetIconByExtensionAsImage(this.ImageName));
				return image;
			}
		}

		public override string ImageName
		{
			get
			{
				if (this.m_sImageName == null)
				{
					this.m_sImageName = this.GetImageName();
				}
				return this.m_sImageName;
			}
		}

		public override string this[string sFieldName]
		{
			get
			{
				string str;
				if (this.m_resultsXML.Attributes[sFieldName] == null)
				{
					str = (!this.HasNode ? "" : this.Node[sFieldName]);
				}
				else
				{
					string value = this.m_resultsXML.Attributes[sFieldName].Value;
					if ((sFieldName == "Created" ? true : sFieldName == "Modified"))
					{
						value = Utils.ParseDateAsUtc(value).ToString();
					}
					str = value;
				}
				return str;
			}
		}

		public override string LinkableUrl
		{
			get
			{
				string str;
				str = (!this.HasNode ? this.Node.Url : this.Node.LinkableUrl);
				return str;
			}
		}

		public Metalogix.Explorer.Location LinkedLocation
		{
			get
			{
				if (this.m_linkedLocation == null)
				{
					SPWeb node = this.WebLocation.GetNode() as SPWeb;
					string value = this.m_resultsXML.Attributes["FileName"].Value;
					if (node != null)
					{
						if (this.ResultType != typeof(SPWeb))
						{
							string str = this.m_resultsXML.Attributes["ListTitle"].Value;
							SPList sPList = null;
							foreach (SPList list in node.Lists)
							{
								if (list.Title == str)
								{
									sPList = list;
									break;
								}
							}
							if (this.ResultType != typeof(SPList))
							{
								SPFolder item = sPList;
								if (sPList != null)
								{
									string name = sPList.Name;
									string[] strArrays = this.m_resultsXML.Attributes["Path"].Value.Split(new char[] { '/' });
									int num = 0;
									if (name != "")
									{
										string[] strArrays1 = strArrays;
										int num1 = 0;
										while (num1 < (int)strArrays1.Length)
										{
											string str1 = strArrays1[num1];
											num++;
											if (!(str1 == name))
											{
												num1++;
											}
											else
											{
												break;
											}
										}
									}
									for (int i = num; i < (int)strArrays.Length; i++)
									{
										if (strArrays[i].Length > 0)
										{
											item = item.SubFolders[strArrays[i]] as SPFolder;
										}
									}
									if (this.ResultType != typeof(SPFolder))
									{
										string value1 = this.m_resultsXML.Attributes["Id"].Value;
										int num2 = int.Parse(value1);
										this.m_linkedLocation = item.Items.GetItemByID(num2).Location;
									}
									else
									{
										this.m_linkedLocation = item.SubFolders[value].Location;
									}
								}
							}
							else
							{
								this.m_linkedLocation = sPList.Location;
							}
						}
						else
						{
							this.m_linkedLocation = node.Location;
						}
					}
					this.m_bFetchFailed = this.m_linkedLocation == null;
					this.m_baseNode = null;
					if (this.Changed != null)
					{
						this.Changed(this, new PropertyChangedEventArgs("Node"));
					}
				}
				return this.m_linkedLocation;
			}
		}

		public override string Name
		{
			get
			{
				string str;
				str = (!this.HasNode ? this.m_resultsXML.Attributes[""].Value : this.Node.Name);
				return str;
			}
		}

		public SPNode Node
		{
			get
			{
				return this.LinkedLocation.GetNode() as SPNode;
			}
		}

		public override string Path
		{
			get
			{
				string str;
				str = (!this.HasNode ? this.Url : this.Node.Path);
				return str;
			}
		}

		public Type ResultType
		{
			get
			{
				Type type;
				int num = int.Parse(this.m_resultsXML.Attributes["Type"].Value);
				if (num == 2)
				{
					type = typeof(SPWeb);
				}
				else if (num != 1)
				{
					type = typeof(SPListItem);
				}
				else
				{
					type = ((this.m_resultsXML.Attributes["Id"] == null ? false : !string.IsNullOrEmpty(this.m_resultsXML.Attributes["Id"].Value)) ? typeof(SPFolder) : typeof(SPList));
				}
				return type;
			}
		}

		public override string ServerRelativeUrl
		{
			get
			{
				string serverRelativeUrl;
				if (!this.HasNode)
				{
					string value = this.m_resultsXML.Attributes["Path"].Value;
					if (this.m_resultsXML.Attributes["FileName"] != null)
					{
						value = string.Concat(value, "/", this.m_resultsXML.Attributes["FileName"].Value);
					}
					serverRelativeUrl = value;
				}
				else
				{
					serverRelativeUrl = this.Node.ServerRelativeUrl;
				}
				return serverRelativeUrl;
			}
		}

		public override string Url
		{
			get
			{
				string url;
				if (!this.HasNode)
				{
					string value = this.m_resultsXML.Attributes["Path"].Value;
					value = string.Concat(this.BaseNode.Adapter.ServerDisplayName, "/", value);
					if (this.m_resultsXML.Attributes["FileName"] != null)
					{
						value = string.Concat(value, "/", this.m_resultsXML.Attributes["FileName"].Value);
					}
					url = value;
				}
				else
				{
					url = this.Node.Url;
				}
				return url;
			}
		}

		public ListItemVersionCollection VersionHistory
		{
			get
			{
				return null;
			}
		}

		private Metalogix.Explorer.Location WebLocation
		{
			get
			{
				Metalogix.Explorer.Location location;
				if (!this.HasNode)
				{
					string serverRelativeUrl = this.BaseNode.ServerRelativeUrl;
					char[] chrArray = new char[] { '/' };
					string[] strArrays = serverRelativeUrl.Split(chrArray);
					string str = strArrays[(int)strArrays.Length - 1];
					string value = this.m_resultsXML.Attributes["WebPath"].Value;
					chrArray = new char[] { '/' };
					string[] strArrays1 = value.Split(chrArray);
					int num = 0;
					if (str != "")
					{
						string[] strArrays2 = strArrays1;
						int num1 = 0;
						while (num1 < (int)strArrays2.Length)
						{
							string str1 = strArrays2[num1];
							num++;
							if (!(str1 == str))
							{
								num1++;
							}
							else
							{
								break;
							}
						}
					}
					string path = this.m_searchLocation.Path;
					string str2 = string.Concat(this.BaseNode.Adapter.ServerDisplayName, this.m_resultsXML.Attributes["Path"].Value);
					for (int i = num; i < (int)strArrays1.Length; i++)
					{
						path = string.Concat(path, "/", strArrays1[i]);
					}
					chrArray = new char[] { '/' };
					location = new Metalogix.Explorer.Location(path.TrimEnd(chrArray), str2, this.m_searchLocation.ConnectionString);
				}
				else
				{
					location = this.Node.Location;
				}
				return location;
			}
		}

		public override bool WriteVirtually
		{
			get
			{
				return false;
			}
		}

		public override string XML
		{
			get
			{
				return this.m_resultsXML.OuterXml;
			}
		}

		static SPSearchResult()
		{
			SPSearchResult.s_cultureInfo = new CultureInfo("en-US", false);
		}

		public SPSearchResult(SPNode baseNode, XmlNode node) : base(baseNode.Adapter, null)
		{
			this.m_baseNode = baseNode;
			this.m_searchLocation = baseNode.Location;
			this.m_resultsXML = node;
			this.UpdateDateTimes();
		}

		protected override void ClearChildNodes()
		{
		}

		protected override void ClearExcessNodeData()
		{
			base.ClearExcessNodeData();
			if (this.HasNode)
			{
				this.Node.Dispose();
			}
		}

		public override DummyNode CloneDummy()
		{
			return this.Node.CloneDummy();
		}

		private string ConvertDateTimeToLocalTime(string sResultValue, TimeZoneInformation timeZone, IFormatProvider formatProvider)
		{
			DateTime localTime = timeZone.UtcToLocalTime(Utils.ParseDateAsUtc(sResultValue));
			return localTime.ToString(formatProvider);
		}

		protected override Metalogix.Explorer.Node[] FetchChildNodes()
		{
			return new Metalogix.Explorer.Node[0];
		}

		public override void FetchChildren()
		{
		}

		private string GetImageName()
		{
			string imageName;
			if (this.ResultType == typeof(SPListItem))
			{
				string value = this.m_resultsXML.Attributes["FileName"].Value;
				if (value == null)
				{
					imageName = null;
				}
				else if (value.IndexOf(".") >= 0)
				{
					imageName = string.Concat("file.", value.Substring(value.LastIndexOf('.') - 1));
				}
				else
				{
					imageName = null;
				}
			}
			else if (this.ResultType != typeof(SPList))
			{
				object[] customAttributes = this.ResultType.GetCustomAttributes(typeof(ImageAttribute), true);
				if ((int)customAttributes.Length != 1)
				{
					imageName = null;
				}
				else
				{
					imageName = ((ImageAttribute)customAttributes[0]).ImageName;
				}
			}
			else
			{
				ListTemplateType listTemplateType = (ListTemplateType)Enum.Parse(typeof(ListTemplateType), this.m_resultsXML.Attributes["ListTemplate"].Value);
				ListTemplateType listTemplateType1 = listTemplateType;
				switch (listTemplateType1)
				{
					case ListTemplateType.CustomList:
					{
						imageName = "Metalogix.SharePoint.Icons.GenericList.ico";
						break;
					}
					case ListTemplateType.DocumentLibrary:
					{
						imageName = "Metalogix.SharePoint.Icons.DocumentLibrary.ico";
						break;
					}
					case ListTemplateType.Survey:
					{
						imageName = "Metalogix.SharePoint.Icons.Survey.ico";
						break;
					}
					case ListTemplateType.Links:
					{
						imageName = "Metalogix.SharePoint.Icons.Links.ico";
						break;
					}
					case ListTemplateType.Announcements:
					{
						imageName = "Metalogix.SharePoint.Icons.GenericList.ico";
						break;
					}
					case ListTemplateType.Contacts:
					{
						imageName = "Metalogix.SharePoint.Icons.Contacts.ico";
						break;
					}
					case ListTemplateType.Events:
					{
						imageName = "Metalogix.SharePoint.Icons.Events.ico";
						break;
					}
					case ListTemplateType.Tasks:
					{
						imageName = "Metalogix.SharePoint.Icons.Tasks.ico";
						break;
					}
					case ListTemplateType.DiscussionBoard:
					{
						imageName = "Metalogix.SharePoint.Icons.DiscussionBoard.ico";
						break;
					}
					case ListTemplateType.PictureLibrary:
					{
						imageName = "Metalogix.SharePoint.Icons.PictureLibrary.ico";
						break;
					}
					default:
					{
						if (listTemplateType1 == ListTemplateType.TasksWithTimelineAndHierarchy)
						{
							goto case ListTemplateType.Tasks;
						}
						goto case ListTemplateType.Announcements;
					}
				}
			}
			return imageName;
		}

		public override Metalogix.Explorer.Node GetNodeByPath(string sPath)
		{
			return this.Node.GetNodeByPath(sPath);
		}

		public override Metalogix.Explorer.Node GetNodeByUrl(string sURL)
		{
			return this.Node.GetNodeByUrl(sURL);
		}

		public override XmlNode GetNodeXML()
		{
			return this.m_resultsXML;
		}

		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptor[] propertyDescriptorArray = new PropertyDescriptor[0];
			if (this.HasNode)
			{
				PropertyDescriptorCollection customProperties = this.Node.GetCustomProperties(attributes);
				propertyDescriptorArray = new PropertyDescriptor[customProperties.Count];
				customProperties.CopyTo(propertyDescriptorArray, 0);
			}
			PropertyDescriptorCollection searchResultProperties = this.GetSearchResultProperties();
			int length = (int)propertyDescriptorArray.Length + searchResultProperties.Count;
			PropertyDescriptor[] searchResultPropertyDescriptor = new PropertyDescriptor[length];
			searchResultProperties.CopyTo(searchResultPropertyDescriptor, 0);
			int count = searchResultProperties.Count;
			PropertyDescriptor[] propertyDescriptorArray1 = propertyDescriptorArray;
			for (int i = 0; i < (int)propertyDescriptorArray1.Length; i++)
			{
				searchResultPropertyDescriptor[count] = new SearchResultPropertyDescriptor(propertyDescriptorArray1[i]);
				count++;
			}
			return new PropertyDescriptorCollection(searchResultPropertyDescriptor);
		}

		private PropertyDescriptorCollection GetSearchResultProperties()
		{
			XmlNode nodeXML = this.GetNodeXML();
			PropertyDescriptor[] xmlPropertyDescriptor = new PropertyDescriptor[nodeXML.Attributes.Count];
			int num = 0;
			foreach (XmlAttribute attribute in nodeXML.Attributes)
			{
				string name = attribute.Name;
				object[] objArray = new object[] { nodeXML.Name, attribute.Name };
				string str = string.Format("./@{1}", objArray);
				Attribute[] categoryAttribute = new Attribute[] { new CategoryAttribute("Search Properties") };
				xmlPropertyDescriptor[num] = new XmlPropertyDescriptor(name, str, categoryAttribute);
				num++;
			}
			return new PropertyDescriptorCollection(xmlPropertyDescriptor);
		}

		protected override Workspace GetVirtualWorkspace()
		{
			return null;
		}

		public override bool IsEqual(Metalogix.DataStructures.IComparable targetComparable, DifferenceLog differencesOutput, ComparisonOptions options)
		{
			bool location;
			if (targetComparable is SPSearchResult)
			{
				location = base.Location == ((SPSearchResult)targetComparable).Location;
			}
			else
			{
				differencesOutput.Write("The target comparable is not a search result");
				location = false;
			}
			return location;
		}

		private void UpdateDateTimes()
		{
			TimeZoneInformation timeZoneInformation;
			SPNode mBaseNode = this.m_baseNode;
			SPWeb sPWeb = mBaseNode as SPWeb;
			while (true)
			{
				if ((sPWeb != null ? true : mBaseNode.Parent == null))
				{
					break;
				}
				mBaseNode = (SPNode)mBaseNode.Parent;
				sPWeb = mBaseNode as SPWeb;
			}
			timeZoneInformation = (sPWeb == null ? TimeZoneInformation.GetLocalTimeZone() : sPWeb.TimeZone);
			XmlAttribute itemOf = this.m_resultsXML.Attributes["Created"];
			itemOf.Value = this.ConvertDateTimeToLocalTime(itemOf.Value, timeZoneInformation, SPSearchResult.s_cultureInfo);
			itemOf = this.m_resultsXML.Attributes["Modified"];
			itemOf.Value = this.ConvertDateTimeToLocalTime(itemOf.Value, timeZoneInformation, SPSearchResult.s_cultureInfo);
		}

		public event PropertyChangedEventHandler Changed;
	}
}