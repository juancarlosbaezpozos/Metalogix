using System;

namespace Metalogix.SharePoint.Actions.Migration.HealthScore
{
	public interface IHealthScoreProvider
	{
		int GetHealthScore();
	}
}