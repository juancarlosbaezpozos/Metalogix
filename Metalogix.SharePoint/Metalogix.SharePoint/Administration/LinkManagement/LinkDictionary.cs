using Metalogix.Explorer;
using Metalogix.SharePoint;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace Metalogix.SharePoint.Administration.LinkManagement
{
	public class LinkDictionary : ILinkDictionary
	{
		private static string s_lock;

		private string m_sLookupProperty;

		private Hashtable m_hashSourceURLs = new Hashtable(1000);

		private bool m_bIncludeInUpdate;

		private ArrayList m_arrLinkTagNames;

		private bool m_bIgnoreQueryString = true;

		public bool IgnoreQueryString
		{
			get
			{
				return this.m_bIgnoreQueryString;
			}
			set
			{
				this.m_bIgnoreQueryString = value;
			}
		}

		public bool IncludeInUpdate
		{
			get
			{
				return JustDecompileGenerated_get_IncludeInUpdate();
			}
			set
			{
				JustDecompileGenerated_set_IncludeInUpdate(value);
			}
		}

		public bool JustDecompileGenerated_get_IncludeInUpdate()
		{
			return this.m_bIncludeInUpdate;
		}

		public void JustDecompileGenerated_set_IncludeInUpdate(bool value)
		{
			this.m_bIncludeInUpdate = value;
		}

		public int KeyCount
		{
			get
			{
				return this.m_hashSourceURLs.Keys.Count;
			}
		}

		public ICollection LinkTagNames
		{
			get
			{
				lock (LinkDictionary.s_lock)
				{
					if (this.m_arrLinkTagNames == null)
					{
						this.m_arrLinkTagNames = new ArrayList(1);
						this.m_arrLinkTagNames.Add("a");
						this.m_arrLinkTagNames.Add("area");
					}
				}
				return this.m_arrLinkTagNames;
			}
		}

		public string LookupProperty
		{
			get
			{
				return this.m_sLookupProperty;
			}
		}

		static LinkDictionary()
		{
			LinkDictionary.s_lock = string.Empty;
		}

		public LinkDictionary(string sLookupProperty)
		{
			if (!string.IsNullOrEmpty(sLookupProperty))
			{
				this.m_sLookupProperty = sLookupProperty;
			}
		}

		public void AddSourceReference(string sSourceReference, string sTargetURL, string sTargetPageTitle)
		{
			if ((sSourceReference == null ? false : sSourceReference.Trim().Length != 0))
			{
				string absoluteUri = null;
				LinkInfo linkInfo = null;
				sSourceReference = (new Uri(sSourceReference)).AbsoluteUri;
				sSourceReference = sSourceReference.ToLower();
				Uri uri = LinkCorrectionUtilitiess.ResolveUrl(sSourceReference);
				if (!(uri != null))
				{
					absoluteUri = sSourceReference;
				}
				else
				{
					Uri uriWithoutBookmark = LinkCorrectionUtilitiess.GetUriWithoutBookmark(uri);
					if (uriWithoutBookmark == null)
					{
						return;
					}
					absoluteUri = uriWithoutBookmark.AbsoluteUri;
					if (this.IgnoreQueryString)
					{
						int num = absoluteUri.IndexOf("?");
						if (num >= 0)
						{
							absoluteUri = absoluteUri.Substring(0, num);
						}
					}
				}
				linkInfo = new LinkInfo(sTargetPageTitle, sTargetURL);
				if (!this.m_hashSourceURLs.ContainsKey(absoluteUri))
				{
					this.m_hashSourceURLs.Add(absoluteUri, linkInfo);
				}
				else
				{
					this.FireLinkDuplicateFound(this, absoluteUri, (LinkInfo)this.m_hashSourceURLs[absoluteUri], linkInfo);
				}
			}
		}

		private void FireErrorEncountered(string sMessage, string sURL)
		{
			if (this.ErrorEncountered != null)
			{
				this.ErrorEncountered(this, new ErrorEncounteredEventArgs(sMessage, sURL));
			}
		}

		private void FireLinkDuplicateFound(object sender, string uriSource, LinkInfo linkLoaded, LinkInfo linkDupclate)
		{
			if (this.LinkDuplicateFound != null)
			{
				LinkDuplicateEventArgs linkDuplicateEventArg = new LinkDuplicateEventArgs(uriSource, linkLoaded, linkDupclate);
				this.LinkDuplicateFound(sender, linkDuplicateEventArg);
			}
		}

		private void FireMappingItem(string sItemUrl, string sItemName, object oTag)
		{
			if (this.MappingItem != null)
			{
				this.MappingItem(this, new MappingItemEventArgs(sItemUrl, sItemName, oTag));
			}
		}

		public object LookUp(object oUriOldLink)
		{
			object item;
			string lower = oUriOldLink.ToString().ToLower();
			if (!this.m_hashSourceURLs.ContainsKey(lower))
			{
				item = null;
			}
			else
			{
				item = (LinkInfo)this.m_hashSourceURLs[lower];
			}
			return item;
		}

		public void MapContentSourceURLs(SPFolder spFolder)
		{
			if (spFolder != null)
			{
				this.MapLib(spFolder);
			}
		}

		private void MapLib(SPFolder spLibraryNode)
		{
			try
			{
				spLibraryNode.FetchChildren();
				this.FireMappingItem(spLibraryNode.ServerRelativeUrl, spLibraryNode.Name, spLibraryNode);
				foreach (SPListItem item in spLibraryNode.Items)
				{
					PropertyDescriptor propertyDescriptor = item.GetProperties()[this.m_sLookupProperty];
					object obj = (propertyDescriptor == null ? null : propertyDescriptor.GetValue(item));
					if (obj != null)
					{
						string str = obj.ToString();
						this.AddSourceReference(str, item.DisplayUrl, item.Name);
						item.Dispose();
					}
				}
				if (spLibraryNode.ParentList.BaseTemplate != ListTemplateType.O12Pages)
				{
					foreach (SPFolder subFolder in spLibraryNode.SubFolders)
					{
						this.MapLib(subFolder);
						subFolder.Dispose();
					}
				}
				spLibraryNode.ReleaseChildren();
			}
			catch
			{
			}
		}

		public event ErrorEncounteredEventHandler ErrorEncountered;

		public event LinkDuplicateHandler LinkDuplicateFound;

		public event MappingItemEventHandler MappingItem;
	}
}