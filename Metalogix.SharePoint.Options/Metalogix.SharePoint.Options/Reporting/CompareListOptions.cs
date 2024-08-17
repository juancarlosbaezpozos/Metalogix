using Metalogix.Actions;
using System;

namespace Metalogix.SharePoint.Options.Reporting
{
	public class CompareListOptions : CompareItemOptions
	{
		private bool m_bCompareFolders = true;

		private bool m_bCompareItems = true;

		private bool m_bFilterLists;

		private string m_sListFilterExpresssion = "<And />";

		public bool CompareFolders
		{
			get
			{
				return this.m_bCompareFolders;
			}
			set
			{
				this.m_bCompareFolders = value;
			}
		}

		public bool CompareItems
		{
			get
			{
				return this.m_bCompareItems;
			}
			set
			{
				this.m_bCompareItems = value;
			}
		}

		[CmdletParameterAlias("FilterListsAndFolders")]
		public bool FilterLists
		{
			get
			{
				return this.m_bFilterLists;
			}
			set
			{
				this.m_bFilterLists = value;
			}
		}

		[CmdletParameterAlias("ListAndFolderFilterExpression")]
		public string ListFilterExpression
		{
			get
			{
				return this.m_sListFilterExpresssion;
			}
			set
			{
				this.m_sListFilterExpresssion = value;
			}
		}

		public CompareListOptions()
		{
		}
	}
}