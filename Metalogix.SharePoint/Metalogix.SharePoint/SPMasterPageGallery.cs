using Metalogix.SharePoint.Properties;
using System;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPMasterPageGallery : SPList
	{
		private readonly object _pageLayoutsLock = new object();

		private SPPageLayoutCollection _pageLayouts = null;

		public SPPageLayoutCollection PageLayouts
		{
			get
			{
				SPPageLayoutCollection pageLayoutCollection = this.GetPageLayoutCollection(false, (SPPageLayoutCollection layouts) => layouts.FetchData());
				return pageLayoutCollection;
			}
		}

		public SPMasterPageGallery(SPWeb parentWeb, XmlNode listXML) : base(parentWeb, listXML)
		{
		}

		private SPPageLayoutCollection GetPageLayoutCollection(bool bForceFetch, SPPageLayoutCollectionFetchDelegate fetchDeleg)
		{
			if (fetchDeleg == null)
			{
				throw new ArgumentNullException("fetchDeleg", Resources.SPMasterPageGallery_GetPageLayoutCollection_Could_not_fetch_page_layout_collection_);
			}
			SPPageLayoutCollection sPPageLayoutCollection = this._pageLayouts;
			if (sPPageLayoutCollection == null)
			{
				bool flag = false;
				lock (this._pageLayoutsLock)
				{
					if (this._pageLayouts == null)
					{
						this._pageLayouts = new SPPageLayoutCollection(this);
						flag = true;
					}
					sPPageLayoutCollection = this._pageLayouts;
				}
				if ((flag ? false : bForceFetch))
				{
					fetchDeleg(this._pageLayouts);
				}
			}
			else if (bForceFetch)
			{
				fetchDeleg(this._pageLayouts);
			}
			return sPPageLayoutCollection;
		}
	}
}