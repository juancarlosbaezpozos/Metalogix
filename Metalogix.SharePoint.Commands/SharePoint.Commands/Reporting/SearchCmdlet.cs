using Metalogix.Commands;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Reporting;
using Metalogix.SharePoint.Adapters;
using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace Metalogix.SharePoint.Commands.Reporting
{
	[Cmdlet("Search", "SharePointSite")]
	public class SearchCmdlet : ActionCmdlet
	{
		private string m_sSearchTerm;

		private int m_iResultCap = 10;

		private SPSearchParameters m_searchParameters = new SPSearchParameters();

		protected override Type ActionType
		{
			get
			{
				return typeof(SearchAction);
			}
		}

		[Parameter(HelpMessage="If specified, search results must have been created by an author matching this value.")]
		public string Author
		{
			get
			{
				return this.Parameters.CreatedBy;
			}
			set
			{
				this.Parameters.CreatedBy = value;
			}
		}

		[Parameter(HelpMessage="If specified, search results must include this value in their content type name.")]
		public string ContentType
		{
			get
			{
				return this.Parameters.ContentType;
			}
			set
			{
				this.Parameters.ContentType = value;
			}
		}

		[Parameter(HelpMessage="If specified, search results must have been created after this date.")]
		public DateTime CreatedAfter
		{
			get
			{
				return this.Parameters.CreatedAfter;
			}
			set
			{
				this.Parameters.CreatedAfter = value;
			}
		}

		[Parameter(HelpMessage="If specified, search results must have been created before this date.")]
		public DateTime CreatedBefore
		{
			get
			{
				return this.Parameters.CreatedBefore;
			}
			set
			{
				this.Parameters.CreatedBefore = value;
			}
		}

		[Parameter(HelpMessage="If specified, search results must have been modified by an editor matching this value.")]
		public string Editor
		{
			get
			{
				return this.Parameters.ModifiedBy;
			}
			set
			{
				this.Parameters.ModifiedBy = value;
			}
		}

		[Parameter(HelpMessage="Indicates that the search results should include documents.")]
		public SwitchParameter IncludeDocuments
		{
			get
			{
				return this.Parameters.IncludeDocuments;
			}
			set
			{
				this.Parameters.IncludeDocuments = value;
			}
		}

		[Parameter(HelpMessage="Indicates that the search results should include folders.")]
		public SwitchParameter IncludeFolders
		{
			get
			{
				return this.Parameters.IncludeFolders;
			}
			set
			{
				this.Parameters.IncludeFolders = value;
			}
		}

		[Parameter(HelpMessage="Indicates that the search results should include items.")]
		public SwitchParameter IncludeItems
		{
			get
			{
				return this.Parameters.IncludeItems;
			}
			set
			{
				this.Parameters.IncludeItems = value;
			}
		}

		[Parameter(HelpMessage="Indicates that the search results should include lists.")]
		public SwitchParameter IncludeLists
		{
			get
			{
				return this.Parameters.IncludeLists;
			}
			set
			{
				this.Parameters.IncludeLists = value;
			}
		}

		[Parameter(HelpMessage="Indicates that the search results should include sites.")]
		public SwitchParameter IncludeSites
		{
			get
			{
				return this.Parameters.IncludeSites;
			}
			set
			{
				this.Parameters.IncludeSites = value;
			}
		}

		[Parameter(HelpMessage="Indicates that the search term must match one of the searched fields exactly.")]
		public SwitchParameter MatchExactly
		{
			get
			{
				return this.Parameters.MatchExactly;
			}
			set
			{
				this.Parameters.MatchExactly = value;
			}
		}

		[Parameter(Position=1, HelpMessage="The number of search results to output.")]
		public int MaxResults
		{
			get
			{
				return this.m_iResultCap;
			}
			set
			{
				this.m_iResultCap = value;
			}
		}

		[Parameter(HelpMessage="If specified, search results must have been modified after this date.")]
		public DateTime ModifiedAfter
		{
			get
			{
				return this.Parameters.ModifiedAfter;
			}
			set
			{
				this.Parameters.ModifiedAfter = value;
			}
		}

		[Parameter(HelpMessage="If specified, search results must have been modified before this date.")]
		public DateTime ModifiedBefore
		{
			get
			{
				return this.Parameters.ModifiedBefore;
			}
			set
			{
				this.Parameters.ModifiedBefore = value;
			}
		}

		public SPSearchParameters Parameters
		{
			get
			{
				return this.m_searchParameters;
			}
		}

		[Parameter(HelpMessage="Indicates that data within all subwebs should be included in the search.")]
		public SwitchParameter Recursive
		{
			get
			{
				return this.Parameters.Recursive;
			}
			set
			{
				this.Parameters.Recursive = value;
			}
		}

		[Parameter(Position=0, ValueFromPipeline=false, ValueFromPipelineByPropertyName=false, HelpMessage="The search term to use.")]
		public string SearchTerm
		{
			get
			{
				return this.m_sSearchTerm;
			}
			set
			{
				this.m_sSearchTerm = value;
			}
		}

		public SearchCmdlet()
		{
		}

		protected SPNode GetNodeFromResult(SPSearchResult result, SPNode searchNode)
		{
			object nodeByPath;
			char[] chrArray = new char[] { '/' };
			string str = string.Concat(searchNode.Adapter.ServerDisplayName.TrimEnd(chrArray), chrArray[0], result["WebPath"].TrimStart(chrArray));
			if (!str.StartsWith(searchNode.DisplayUrl))
			{
				return null;
			}
			string str1 = str.Substring(searchNode.DisplayUrl.Length);
			if (str1.Length == 0)
			{
				nodeByPath = searchNode;
			}
			else
			{
				nodeByPath = searchNode.GetNodeByPath(str1);
			}
			SPWeb sPWeb = nodeByPath as SPWeb;
			if (sPWeb == null || result.ResultType == typeof(SPWeb))
			{
				return sPWeb;
			}
			string item = result["ListTitle"];
			SPList sPList = null;
			foreach (SPList list in sPWeb.Lists)
			{
				if (list.Title != item)
				{
					continue;
				}
				sPList = list;
				break;
			}
			if (sPList == null || result.ResultType == typeof(SPList))
			{
				return sPList;
			}
			SPFolder sPFolder = sPList;
			string item1 = result["FileName"];
			string name = sPList.Name;
			string[] strArrays = result["Path"].Split(new char[] { '/' });
			int num = 0;
			if (name != "")
			{
				string[] strArrays1 = strArrays;
				for (int i = 0; i < (int)strArrays1.Length; i++)
				{
					string str2 = strArrays1[i];
					num++;
					if (str2 == name)
					{
						break;
					}
				}
			}
			for (int j = num; j < (int)strArrays.Length; j++)
			{
				if (strArrays[j].Length > 0)
				{
					sPFolder = sPFolder.SubFolders[strArrays[j]] as SPFolder;
				}
			}
			if (sPFolder == null)
			{
				return sPFolder;
			}
			if (result.ResultType == typeof(SPFolder))
			{
				return sPFolder.SubFolders[item1] as SPNode;
			}
			int num1 = int.Parse(result["Id"]);
			return sPFolder.Items.GetItemByID(num1);
		}

		protected override void ProcessRecord()
		{
			base.ProcessRecord();
		}

		protected override void Run()
		{
			List<Node> nodes = new List<Node>();
			if (!(base.Target is SPServer))
			{
				nodes.Add(base.Target);
			}
			else
			{
				nodes.AddRange(((SPServer)base.Target).Sites.ToArray());
			}
			foreach (Node node in nodes)
			{
				SPSearchResultsCollection sPSearchResultsCollection = new SPSearchResultsCollection(node as SPNode, this.SearchTerm)
				{
					Parameters = this.Parameters
				};
				sPSearchResultsCollection.ExecuteSearch();
				int num = 0;
				foreach (SPSearchResult sPSearchResult in sPSearchResultsCollection)
				{
					if (!base.Quiet)
					{
						base.WriteProgress(new ProgressRecord(1, "Searching", string.Concat("Linking result: ", sPSearchResult.Url)));
					}
					if (num == this.MaxResults)
					{
						break;
					}
					SPNode nodeFromResult = this.GetNodeFromResult(sPSearchResult, sPSearchResultsCollection.SearchNode);
					if (nodeFromResult == null)
					{
						continue;
					}
					base.WriteObject(nodeFromResult);
					num++;
				}
			}
		}
	}
}