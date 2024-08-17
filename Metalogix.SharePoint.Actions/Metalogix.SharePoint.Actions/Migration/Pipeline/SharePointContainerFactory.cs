using Metalogix.Azure;
using Metalogix.SharePoint.Adapters;
using System;

namespace Metalogix.SharePoint.Actions.Migration.Pipeline
{
	public class SharePointContainerFactory : IAzureContainerFactory
	{
		private readonly IMigrationPipeline _pipeline;

		public bool CanDelete
		{
			get
			{
				return false;
			}
		}

		public SharePointContainerFactory(IMigrationPipeline pipeline)
		{
			this._pipeline = pipeline;
		}

		public IAzureContainerInstance NewInstance(int id)
		{
			return new SharePointContainerInstance(this._pipeline);
		}
	}
}