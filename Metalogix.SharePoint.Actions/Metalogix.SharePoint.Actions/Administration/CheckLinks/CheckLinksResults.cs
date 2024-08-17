using Metalogix.SharePoint.Administration;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Metalogix.SharePoint.Actions.Administration.CheckLinks
{
	public class CheckLinksResults
	{
		private List<SPCheckLink> m_Successes = new List<SPCheckLink>();

		private List<SPCheckLink> m_Failures = new List<SPCheckLink>();

		private List<SPCheckLink> m_FlaggedSuccesses = new List<SPCheckLink>();

		private List<SPCheckLink> m_FlaggedFailures = new List<SPCheckLink>();

		public int Count
		{
			get
			{
				return this.Successes.Count + this.Failures.Count + this.FlaggedSuccesses.Count + this.FlaggedFailures.Count;
			}
		}

		public List<SPCheckLink> Failures
		{
			get
			{
				return this.m_Failures;
			}
			set
			{
				this.m_Failures = value;
			}
		}

		public List<SPCheckLink> FlaggedFailures
		{
			get
			{
				return this.m_FlaggedFailures;
			}
			set
			{
				this.m_FlaggedFailures = value;
			}
		}

		public List<SPCheckLink> FlaggedSuccesses
		{
			get
			{
				return this.m_FlaggedSuccesses;
			}
			set
			{
				this.m_FlaggedSuccesses = value;
			}
		}

		public List<SPCheckLink> Successes
		{
			get
			{
				return this.m_Successes;
			}
			set
			{
				this.m_Successes = value;
			}
		}

		public CheckLinksResults()
		{
		}

		public bool Add(SPCheckLink linkResult)
		{
			bool flag;
			try
			{
				if (linkResult.IsFlagged)
				{
					if (!linkResult.IsValidLink)
					{
						this.FlaggedFailures.Add(linkResult);
					}
					else
					{
						this.FlaggedSuccesses.Add(linkResult);
					}
				}
				else if (!linkResult.IsValidLink)
				{
					this.Failures.Add(linkResult);
				}
				else
				{
					this.Successes.Add(linkResult);
				}
				return true;
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		public bool Append(CheckLinksResults toAdd)
		{
			bool flag;
			try
			{
				this.Successes.AddRange(toAdd.Successes);
				this.Failures.AddRange(toAdd.Failures);
				this.FlaggedSuccesses.AddRange(toAdd.FlaggedSuccesses);
				this.FlaggedFailures.AddRange(toAdd.FlaggedFailures);
				return true;
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		public void WriteXml(XmlTextWriter writer, bool bIncludeParentUrls)
		{
			this.WriteXml(this.Failures, writer, bIncludeParentUrls);
			this.WriteXml(this.Successes, writer, bIncludeParentUrls);
			this.WriteXml(this.FlaggedFailures, writer, bIncludeParentUrls);
			this.WriteXml(this.FlaggedSuccesses, writer, bIncludeParentUrls);
		}

		private void WriteXml(List<SPCheckLink> links, XmlTextWriter writer, bool bIncludeParentUrls)
		{
			if (writer != null)
			{
				foreach (SPCheckLink link in links)
				{
					link.WriteToXml(writer, bIncludeParentUrls);
				}
			}
		}
	}
}