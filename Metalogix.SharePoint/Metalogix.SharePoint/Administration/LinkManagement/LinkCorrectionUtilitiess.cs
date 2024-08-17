using System;

namespace Metalogix.SharePoint.Administration.LinkManagement
{
	public class LinkCorrectionUtilitiess
	{
		public LinkCorrectionUtilitiess()
		{
		}

		public static Uri GetUriWithoutBookmark(Uri uriToCorrect)
		{
			Uri uri;
			if (!(uriToCorrect == null))
			{
				Uri uri1 = uriToCorrect;
				if (!string.IsNullOrEmpty(uri1.Fragment))
				{
					int length = uri1.Fragment.Length;
					string absoluteUri = uri1.AbsoluteUri;
					absoluteUri = absoluteUri.Remove(absoluteUri.Length - length, length);
					uri1 = new Uri(absoluteUri);
				}
				uri = uri1;
			}
			else
			{
				uri = null;
			}
			return uri;
		}

		public static Uri ResolveUrl(string sUrl)
		{
			Uri uri = null;
			try
			{
				uri = new Uri(sUrl);
			}
			catch (UriFormatException uriFormatException)
			{
				uri = null;
			}
			return uri;
		}

		public static Uri ResolveUrl(Uri uriBase, string sRelativeUrl)
		{
			Uri uri = null;
			try
			{
				try
				{
					uri = new Uri(sRelativeUrl);
				}
				catch (UriFormatException uriFormatException)
				{
					uri = null;
				}
			}
			finally
			{
				if ((uri == null || uri.Host != uriBase.Host ? true : uri.Scheme != uriBase.Scheme))
				{
					try
					{
						uri = new Uri(uriBase, sRelativeUrl);
					}
					catch (UriFormatException uriFormatException1)
					{
						uri = null;
					}
				}
			}
			return uri;
		}
	}
}