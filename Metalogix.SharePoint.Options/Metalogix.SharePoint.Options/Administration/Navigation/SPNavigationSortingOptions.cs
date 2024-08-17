using Metalogix.Actions;
using System;

namespace Metalogix.SharePoint.Options.Administration.Navigation
{
	public class SPNavigationSortingOptions : ActionOptions
	{
		private bool m_SortAscending = true;

		private ChangeNavigationSettingsOptions.SortingType m_OverallSortingType = ChangeNavigationSettingsOptions.SortingType.Automatic;

		private ChangeNavigationSettingsOptions.SortingType m_PublishingPageSortingType = ChangeNavigationSettingsOptions.SortingType.Automatic;

		private ChangeNavigationSettingsOptions.SortByColumn m_SortingColumn = ChangeNavigationSettingsOptions.SortByColumn.Title;

		private ChangeNavigationSettingsOptions.SortingModifiedFlags m_SortingOptionsModified = new ChangeNavigationSettingsOptions.SortingModifiedFlags();

		public ChangeNavigationSettingsOptions.SortByColumn ColumnToSortBy
		{
			get
			{
				return this.m_SortingColumn;
			}
			set
			{
				this.m_SortingColumn = value;
			}
		}

		public ChangeNavigationSettingsOptions.SortingType OverallSortingType
		{
			get
			{
				return this.m_OverallSortingType;
			}
			set
			{
				this.m_OverallSortingType = value;
			}
		}

		public ChangeNavigationSettingsOptions.SortingType PublishingPageSortingType
		{
			get
			{
				return this.m_PublishingPageSortingType;
			}
			set
			{
				this.m_PublishingPageSortingType = value;
			}
		}

		public bool SortAscending
		{
			get
			{
				return this.m_SortAscending;
			}
			set
			{
				this.m_SortAscending = value;
			}
		}

		public ChangeNavigationSettingsOptions.SortingModifiedFlags SortingOptionsModified
		{
			get
			{
				return this.m_SortingOptionsModified;
			}
			set
			{
				this.m_SortingOptionsModified = value;
			}
		}

		public SPNavigationSortingOptions()
		{
		}
	}
}