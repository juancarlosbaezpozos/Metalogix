using Metalogix.SharePoint.Options;
using System;

namespace Metalogix.SharePoint.UI.WinForms.Migration
{
	public struct MigrationModeChangedInfo
	{
		private MigrationMode m_newMigrationMode;

		private bool m_bOverwritingOrUpdatingItems;

		private bool m_bPropagatingItemDeletions;

		public MigrationMode NewMigrationMode
		{
			get
			{
				return this.m_newMigrationMode;
			}
			set
			{
				this.m_newMigrationMode = value;
			}
		}

		public bool OverwritingOrUpdatingItems
		{
			get
			{
				return this.m_bOverwritingOrUpdatingItems;
			}
			set
			{
				this.m_bOverwritingOrUpdatingItems = value;
			}
		}

		public bool PropagatingItemDeletions
		{
			get
			{
				return this.m_bPropagatingItemDeletions;
			}
			set
			{
				this.m_bPropagatingItemDeletions = value;
			}
		}
	}
}