using Metalogix;
using Metalogix.SharePoint.Options;
using System;

namespace Metalogix.SharePoint.Options.Migration
{
	public class SPMigrationModeOptions : OptionsBase
	{
		private Metalogix.SharePoint.Options.MigrationMode m_MigrationMode;

		private bool m_bOverwriteSites;

		private bool m_bUpdateSites;

		private int m_iUpdateSiteOptionsBitField;

		private bool m_bOverwriteLists = true;

		private bool m_bUpdateLists;

		private bool m_bCheckModifiedDatesForLists = true;

		private int m_iUpdateListOptionsBitField;

		private bool m_bPropagateItemDeletions;

		private bool m_bUpdateItems;

		private bool m_bCheckModifiedDatesForItemsDocuments = true;

		private ListItemCopyMode m_bItemCopyMode = ListItemCopyMode.Preserve;

		private int m_iUpdateItemOptionsBitField;

		public bool CheckModifiedDatesForItemsDocuments
		{
			get
			{
				return this.m_bCheckModifiedDatesForItemsDocuments;
			}
			set
			{
				this.m_bCheckModifiedDatesForItemsDocuments = value;
			}
		}

		public bool CheckModifiedDatesForLists
		{
			get
			{
				return this.m_bCheckModifiedDatesForLists;
			}
			set
			{
				this.m_bCheckModifiedDatesForLists = value;
			}
		}

		public ListItemCopyMode ItemCopyingMode
		{
			get
			{
				return this.m_bItemCopyMode;
			}
			set
			{
				this.m_bItemCopyMode = value;
			}
		}

		public Metalogix.SharePoint.Options.MigrationMode MigrationMode
		{
			get
			{
				return this.m_MigrationMode;
			}
			set
			{
				this.m_MigrationMode = value;
			}
		}

		public bool OverwriteLists
		{
			get
			{
				return this.m_bOverwriteLists;
			}
			set
			{
				this.m_bOverwriteLists = value;
			}
		}

		public bool OverwriteSites
		{
			get
			{
				return this.m_bOverwriteSites;
			}
			set
			{
				this.m_bOverwriteSites = value;
			}
		}

		public bool PropagateItemDeletions
		{
			get
			{
				return this.m_bPropagateItemDeletions;
			}
			set
			{
				this.m_bPropagateItemDeletions = value;
			}
		}

		public int UpdateItemOptionsBitField
		{
			get
			{
				return this.m_iUpdateItemOptionsBitField;
			}
			set
			{
				this.m_iUpdateItemOptionsBitField = value;
			}
		}

		public bool UpdateItems
		{
			get
			{
				return this.m_bUpdateItems;
			}
			set
			{
				this.m_bUpdateItems = value;
			}
		}

		public int UpdateListOptionsBitField
		{
			get
			{
				return this.m_iUpdateListOptionsBitField;
			}
			set
			{
				this.m_iUpdateListOptionsBitField = value;
			}
		}

		public bool UpdateLists
		{
			get
			{
				return this.m_bUpdateLists;
			}
			set
			{
				this.m_bUpdateLists = value;
			}
		}

		public int UpdateSiteOptionsBitField
		{
			get
			{
				return this.m_iUpdateSiteOptionsBitField;
			}
			set
			{
				this.m_iUpdateSiteOptionsBitField = value;
			}
		}

		public bool UpdateSites
		{
			get
			{
				return this.m_bUpdateSites;
			}
			set
			{
				this.m_bUpdateSites = value;
			}
		}

		public SPMigrationModeOptions()
		{
		}
	}
}