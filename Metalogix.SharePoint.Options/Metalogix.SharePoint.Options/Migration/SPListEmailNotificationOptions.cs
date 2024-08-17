using Metalogix;
using Metalogix.Actions;
using Metalogix.DataStructures;
using System;

namespace Metalogix.SharePoint.Options.Migration
{
	public class SPListEmailNotificationOptions : OptionsBase
	{
		private bool m_bRecursive = true;

		private bool m_bRenameSpecificNodes;

		private TransformationTaskCollection m_renamingTransformations = new TransformationTaskCollection();

		public bool Recursive
		{
			get
			{
				return this.m_bRecursive;
			}
			set
			{
				this.m_bRecursive = value;
			}
		}

		public bool RenameSpecificNodes
		{
			get
			{
				return this.m_bRenameSpecificNodes;
			}
			set
			{
				this.m_bRenameSpecificNodes = value;
			}
		}

		[UsesStickySettings(false)]
		public TransformationTaskCollection RenamingTransformations
		{
			get
			{
				return this.m_renamingTransformations;
			}
			set
			{
				this.m_renamingTransformations = value;
			}
		}

		public SPListEmailNotificationOptions()
		{
		}
	}
}