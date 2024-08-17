using System;

namespace Metalogix.Azure
{
	public interface IAzureContainerFactory
	{
		bool CanDelete
		{
			get;
		}

		IAzureContainerInstance NewInstance(int id);
	}
}