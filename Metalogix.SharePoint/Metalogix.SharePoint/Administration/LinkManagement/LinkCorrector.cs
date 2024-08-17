using Metalogix.Explorer;
using Metalogix.SharePoint;
using System;
using System.ComponentModel;
using System.Threading;
using System.Web;

namespace Metalogix.SharePoint.Administration.LinkManagement
{
	[LinkCorrectorDisplayName("Unnamed")]
	public abstract class LinkCorrector : ILinkCorrector
	{
		private bool m_bAborted;

		private bool m_bPaused;

		protected ILinkDictionary m_linkDictionary;

		protected bool m_bReportOnly;

		protected LinkCorrector()
		{
		}

		public void Abort()
		{
			this.m_bAborted = true;
		}

		protected bool Aborting()
		{
			bool flag;
			if (!this.m_bAborted)
			{
				while (this.m_bPaused)
				{
					if (!this.m_bAborted)
					{
						Thread.Sleep(100);
					}
					else
					{
						flag = true;
						return flag;
					}
				}
				flag = false;
			}
			else
			{
				flag = true;
			}
			return flag;
		}

		public abstract bool CorrectComponent(object oComponent, ILinkDictionary linkDictionary, bool bReportOnly);

		protected void FireErrorEncountered(object sender, string sMessage, string sURL)
		{
			if (this.ErrorEncountered != null)
			{
				this.ErrorEncountered(sender, new ErrorEncounteredEventArgs(sMessage, sURL));
			}
		}

		protected void FireLinkCorrected(object sender, SPListItem spListItem, string sFieldName, string sOldLink, LinkInfo linkNew)
		{
			this.FireLinkCorrected(sender, spListItem, sFieldName, sOldLink, linkNew, linkNew.Name);
		}

		protected void FireLinkCorrected(object sender, SPListItem spListItem, string sFieldName, string sOldLink, LinkInfo linkNew, string sLinkText)
		{
			if (this.LinkCorrected != null)
			{
				string name = spListItem.ParentList.Name;
				string url = spListItem.ParentList.Url;
				string str = "untitled";
				PropertyDescriptor item = spListItem.GetProperties()["Title"];
				if (item != null)
				{
					object value = item.GetValue(spListItem);
					if (value != null)
					{
						str = value.ToString();
					}
				}
				LinkInfo linkInfo = new LinkInfo(name, url);
				LinkInfo linkInfo1 = new LinkInfo(str, spListItem.DisplayUrl);
				LinkCorrectedEventArgs linkCorrectedEventArg = new LinkCorrectedEventArgs(sOldLink, linkNew, sLinkText, linkInfo, linkInfo1, sFieldName);
				this.LinkCorrected(sender, linkCorrectedEventArg);
			}
		}

		protected static LinkCorrector.CorrectedLinkResult GetCorrectedLink(string sLinkValue, string sSourceRef, ILinkDictionary mappingsHash, bool bMakeRootRelative)
		{
			LinkCorrector.CorrectedLinkResult correctedLinkResult;
			Uri uri;
			if (!(sLinkValue == null ? false : mappingsHash != null))
			{
				correctedLinkResult = new LinkCorrector.CorrectedLinkResult(false);
			}
			else if (!sLinkValue.StartsWith("#"))
			{
				string absoluteUri = sLinkValue;
				string fragment = "";
				if (sSourceRef == null)
				{
					uri = null;
				}
				else
				{
					uri = new Uri(sSourceRef);
				}
				Uri uri1 = uri;
				Uri uri2 = LinkCorrectionUtilitiess.ResolveUrl(sLinkValue);
				if ((!(uri2 == null) || !(uri1 != null) ? false : Uri.IsWellFormedUriString(sSourceRef, UriKind.Absolute)))
				{
					uri2 = LinkCorrectionUtilitiess.ResolveUrl(uri1, sLinkValue);
				}
				if (!(uri2 == null))
				{
					absoluteUri = LinkCorrectionUtilitiess.GetUriWithoutBookmark(uri2).AbsoluteUri;
					absoluteUri = HttpUtility.HtmlDecode(absoluteUri);
					if (mappingsHash.IgnoreQueryString)
					{
						int num = absoluteUri.IndexOf("?");
						if (num >= 0)
						{
							absoluteUri = absoluteUri.Substring(0, num);
						}
					}
					fragment = uri2.Fragment;
					absoluteUri = absoluteUri.ToLower();
					object obj = mappingsHash.LookUp(absoluteUri);
					if (obj == null)
					{
						char[] chrArray = new char[] { '/', '\\' };
						string str = absoluteUri.TrimEnd(chrArray);
						if (absoluteUri != str)
						{
							obj = mappingsHash.LookUp(str);
						}
					}
					if ((obj == null ? false : obj.GetType().Equals(typeof(LinkInfo))))
					{
						LinkInfo linkInfo = (LinkInfo)obj;
						string uRL = linkInfo.URL;
						if (bMakeRootRelative)
						{
							uRL = (new Uri(uRL)).AbsolutePath;
						}
						string str1 = string.Concat(uRL, fragment);
						correctedLinkResult = new LinkCorrector.CorrectedLinkResult(true, str1, linkInfo);
					}
					else
					{
						correctedLinkResult = new LinkCorrector.CorrectedLinkResult(false);
					}
				}
				else
				{
					correctedLinkResult = new LinkCorrector.CorrectedLinkResult(false);
				}
			}
			else
			{
				correctedLinkResult = new LinkCorrector.CorrectedLinkResult(false);
			}
			return correctedLinkResult;
		}

		public void Pause()
		{
			this.m_bPaused = true;
		}

		public void Resume()
		{
			this.m_bPaused = false;
		}

		public abstract bool SupportsComponent(object oComponent);

		public event ErrorEncounteredEventHandler ErrorEncountered;

		public event LinkCorrectedEventHandler LinkCorrected;

		protected class CorrectedLinkResult
		{
			public bool m_bFoundMatch;

			public string m_sCorrectedLink;

			public LinkInfo m_linkMatch;

			public string CorrectedLink
			{
				get
				{
					return this.m_sCorrectedLink;
				}
			}

			public bool FoundMatch
			{
				get
				{
					return this.m_bFoundMatch;
				}
			}

			public LinkInfo LinkMatch
			{
				get
				{
					return this.m_linkMatch;
				}
			}

			public CorrectedLinkResult(bool bFoundMatch)
			{
				this.m_bFoundMatch = bFoundMatch;
			}

			public CorrectedLinkResult(bool bFoundMatch, string sCorrectedLink, LinkInfo linkMatch)
			{
				this.m_bFoundMatch = bFoundMatch;
				this.m_sCorrectedLink = sCorrectedLink;
				this.m_linkMatch = linkMatch;
			}
		}
	}
}