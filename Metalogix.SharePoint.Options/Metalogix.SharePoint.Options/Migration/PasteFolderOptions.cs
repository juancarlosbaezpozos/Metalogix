using Metalogix.Actions;
using Metalogix.Data.Filters;
using Metalogix.DataStructures.Generic;
using Metalogix.SharePoint;
using System;
using System.Globalization;

namespace Metalogix.SharePoint.Options.Migration
{
	public class PasteFolderOptions : PasteListItemOptions
	{
		private bool m_bFilterFolders;

		private IFilterExpression m_folderFilterExpression = new FilterExpressionList(ExpressionLogic.And);

		private bool m_bCopyFolderPermissions = true;

		private bool m_bOverwriteLists = true;

		private int m_iUpdateFolderOptions;

		private bool m_bCopyListItems = true;

		private bool _useAzureOffice365Upload = true;

		private bool _encryptAzureMigrationJobs = true;

		public bool CopyFolderPermissions
		{
			get
			{
				return this.m_bCopyFolderPermissions;
			}
			set
			{
				this.m_bCopyFolderPermissions = value;
			}
		}

		public bool CopyListItems
		{
			get
			{
				return this.m_bCopyListItems;
			}
			set
			{
				this.m_bCopyListItems = value;
			}
		}

		public new bool EncryptAzureMigrationJobs
		{
			get
			{
				return this._encryptAzureMigrationJobs;
			}
			set
			{
				this._encryptAzureMigrationJobs = value;
			}
		}

		[CmdletEnabledParameter(false)]
		public bool FilterFolders
		{
			get
			{
				return this.m_bFilterFolders;
			}
			set
			{
				this.m_bFilterFolders = value;
			}
		}

		[CmdletEnabledParameter("FilterFolders", true)]
		public IFilterExpression FolderFilterExpression
		{
			get
			{
				return this.m_folderFilterExpression;
			}
			set
			{
				this.m_folderFilterExpression = value;
			}
		}

		public bool OverwriteFolders
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

		[CmdletEnabledParameter(false)]
		public virtual bool OverwriteLists
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

		[CmdletEnabledParameter(false)]
		public int UpdateFolderOptionsBitField
		{
			get
			{
				return this.m_iUpdateFolderOptions;
			}
			set
			{
				this.m_iUpdateFolderOptions = value;
			}
		}

		public new bool UseAzureOffice365Upload
		{
			get
			{
				return this._useAzureOffice365Upload;
			}
			set
			{
				this._useAzureOffice365Upload = value;
			}
		}

		public PasteFolderOptions()
		{
		}

		public override void MakeOptionsIncremental(DateTime? incrementFromTime)
		{
			base.MakeOptionsIncremental(incrementFromTime);
			this.OverwriteFolders = false;
			if (incrementFromTime.HasValue)
			{
				DateTime value = incrementFromTime.Value;
				FilterExpression filterExpression = new FilterExpression(FilterOperand.GreaterThanOrEqualTo, typeof(SPFolder), "Modified", value.ToString(CultureInfo.CurrentCulture), false, false, CultureInfo.CurrentCulture);
				FilterExpressionList filterExpressionList = new FilterExpressionList(ExpressionLogic.And);
				if (this.FilterFolders && this.FolderFilterExpression != null)
				{
					filterExpressionList.Add(this.FolderFilterExpression);
				}
				filterExpressionList.Add(filterExpression);
				this.FolderFilterExpression = filterExpressionList;
				this.FilterFolders = true;
			}
		}
	}
}