using Metalogix.Actions;
using Metalogix.Licensing;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Options;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Actions.Administration
{
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[Name("Crawl SharePoint")]
	[ShowInMenus(false)]
	[SourceCardinality(Cardinality.Zero)]
	[TargetCardinality(Cardinality.OneOrMore)]
	public class CrawlAction : SharePointAction<SharePointActionOptions>
	{
		private object m_Context;

		private CrawlAction.ProcessSiteCollectionDelegate m_SiteCollProcessor;

		private CrawlAction.ProcessWebDelegate m_WebProcessor;

		private CrawlAction.ProcessListDelegate m_ListProcessor;

		private CrawlAction.ProcessFolderDelegate m_FolderProcessor;

		private CrawlAction.ProcessListItemDelgate m_ItemProcessor;

		private bool m_bCrawlSiteCollections;

		private bool m_bCrawlWebs;

		private bool m_bCrawlLists;

		private bool m_bCrawlFolders;

		private bool m_bCrawlItems;

		public object CrawlContext
		{
			get
			{
				return this.m_Context;
			}
			set
			{
				this.m_Context = value;
			}
		}

		protected bool CrawlingFolders
		{
			get
			{
				return this.m_bCrawlFolders;
			}
			set
			{
				this.m_bCrawlFolders = value;
			}
		}

		protected bool CrawlingItems
		{
			get
			{
				return this.m_bCrawlItems;
			}
			set
			{
				this.m_bCrawlItems = value;
			}
		}

		protected bool CrawlingLists
		{
			get
			{
				return this.m_bCrawlLists;
			}
			set
			{
				this.m_bCrawlLists = value;
			}
		}

		protected bool CrawlingSiteCollections
		{
			get
			{
				return this.m_bCrawlSiteCollections;
			}
			set
			{
				this.m_bCrawlSiteCollections = value;
			}
		}

		protected bool CrawlingWebs
		{
			get
			{
				return this.m_bCrawlWebs;
			}
			set
			{
				this.m_bCrawlWebs = value;
			}
		}

		public CrawlAction.ProcessFolderDelegate FolderProcessor
		{
			get
			{
				return this.m_FolderProcessor;
			}
			set
			{
				this.m_FolderProcessor = value;
				this.CrawlingFolders = (this.m_FolderProcessor != null ? true : this.CrawlingItems);
			}
		}

		public CrawlAction.ProcessListItemDelgate ItemProcessor
		{
			get
			{
				return this.m_ItemProcessor;
			}
			set
			{
				this.m_ItemProcessor = value;
				this.CrawlingItems = this.m_ItemProcessor != null;
			}
		}

		public CrawlAction.ProcessListDelegate ListProcessor
		{
			get
			{
				return this.m_ListProcessor;
			}
			set
			{
				this.m_ListProcessor = value;
				this.CrawlingLists = (this.m_ListProcessor != null || this.CrawlingFolders ? true : this.CrawlingItems);
			}
		}

		public CrawlAction.ProcessSiteCollectionDelegate SiteCollectionProcessor
		{
			get
			{
				return this.m_SiteCollProcessor;
			}
			set
			{
				this.m_SiteCollProcessor = value;
				this.CrawlingSiteCollections = (this.m_SiteCollProcessor != null || this.CrawlingWebs || this.CrawlingLists || this.CrawlingFolders ? true : this.CrawlingItems);
			}
		}

		public CrawlAction.ProcessWebDelegate WebProcessor
		{
			get
			{
				return this.m_WebProcessor;
			}
			set
			{
				this.m_WebProcessor = value;
				this.CrawlingWebs = (this.m_WebProcessor != null || this.CrawlingLists || this.CrawlingFolders ? true : this.CrawlingItems);
			}
		}

		public CrawlAction()
		{
		}

		public override bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
		{
			bool flag;
			if (targetSelections.Count <= 0)
			{
				return false;
			}
			IEnumerator enumerator = targetSelections.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (this.IsApplicableObject(enumerator.Current))
					{
						continue;
					}
					flag = false;
					return flag;
				}
				return true;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return flag;
		}

		protected void CrawlFolder(SPFolder folderToCrawl)
		{
			if (folderToCrawl == null || !this.CrawlingFolders)
			{
				return;
			}
			this.ProcessFolder(folderToCrawl);
			this.CrawlFolderContents(folderToCrawl);
		}

		protected void CrawlFolderContents(SPFolder folderToCrawl)
		{
			if (folderToCrawl == null)
			{
				return;
			}
			this.CrawlItems(folderToCrawl.Items);
			foreach (SPFolder subFolder in folderToCrawl.SubFolders)
			{
				this.CrawlFolder(subFolder);
			}
		}

		protected void CrawlItems(SPListItemCollection itemsToCrawl)
		{
			if (itemsToCrawl == null || !this.CrawlingItems)
			{
				return;
			}
			foreach (SPListItem sPListItem in itemsToCrawl)
			{
				this.ProcessItem(sPListItem);
			}
		}

		protected void CrawlList(SPList listToCrawl)
		{
			if (listToCrawl == null || !this.CrawlingLists)
			{
				return;
			}
			this.ProcessList(listToCrawl);
			this.CrawlFolderContents(listToCrawl);
		}

		public void CrawlObjects(IEnumerable crawlObjectsList)
		{
			if (crawlObjectsList == null)
			{
				return;
			}
			foreach (object obj in crawlObjectsList)
			{
				if (obj is SPSite)
				{
					this.CrawlSiteCollection((SPSite)obj);
				}
				else if (obj is SPWeb)
				{
					this.CrawlWeb((SPWeb)obj);
				}
				else if (obj is SPList)
				{
					this.CrawlList((SPList)obj);
				}
				else if (!(obj is SPFolder))
				{
					if (!(obj is SPListItem))
					{
						continue;
					}
					this.ProcessItem((SPListItem)obj);
				}
				else
				{
					this.CrawlFolder((SPFolder)obj);
				}
			}
		}

		protected void CrawlSiteCollection(SPSite siteToCrawl)
		{
			if (siteToCrawl == null || !this.CrawlingSiteCollections)
			{
				return;
			}
			this.ProcessSiteCollection(siteToCrawl);
			foreach (SPWeb subWeb in siteToCrawl.SubWebs)
			{
				this.CrawlWeb(subWeb);
			}
		}

		protected void CrawlWeb(SPWeb webToCrawl)
		{
			if (webToCrawl == null || !this.CrawlingWebs)
			{
				return;
			}
			this.ProcessWeb(webToCrawl);
			foreach (SPWeb subWeb in webToCrawl.SubWebs)
			{
				this.CrawlWeb(subWeb);
			}
			foreach (SPList list in webToCrawl.Lists)
			{
				this.CrawlList(list);
			}
		}

		private bool IsApplicableObject(object o)
		{
			if (!(o is SPSite) && !(o is SPWeb) && !(o is SPList) && !(o is SPFolder) && !(o is SPListItem))
			{
				return false;
			}
			return true;
		}

		private void ProcessFolder(SPFolder folderToCrawl)
		{
			if (this.FolderProcessor != null)
			{
				this.FolderProcessor(folderToCrawl, this.CrawlContext);
			}
		}

		private void ProcessItem(SPListItem currentItem)
		{
			if (this.ItemProcessor != null)
			{
				this.ItemProcessor(currentItem, this.CrawlContext);
			}
		}

		private void ProcessList(SPList listToCrawl)
		{
			if (this.ListProcessor != null)
			{
				this.ListProcessor(listToCrawl, this.CrawlContext);
			}
		}

		private void ProcessSiteCollection(SPSite siteToCrawl)
		{
			if (this.SiteCollectionProcessor != null)
			{
				this.SiteCollectionProcessor(siteToCrawl, this.CrawlContext);
			}
		}

		private void ProcessWeb(SPWeb webToCrawl)
		{
			if (this.WebProcessor != null)
			{
				this.WebProcessor(webToCrawl, this.CrawlContext);
			}
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			if (target != null && target.Count > 0)
			{
				this.CrawlObjects(target);
			}
		}

		public delegate void ProcessFolderDelegate(SPFolder folderToProcess, object crawlContext);

		public delegate void ProcessListDelegate(SPList listToProcess, object crawlContext);

		public delegate void ProcessListItemDelgate(SPListItem itemToProcess, object crawlContext);

		public delegate void ProcessSiteCollectionDelegate(SPSite siteToProcess, object crawlContext);

		public delegate void ProcessWebDelegate(SPWeb webToProcess, object crawlContext);
	}
}