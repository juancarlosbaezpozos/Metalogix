using Metalogix.Core.OperationLog;
using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Actions.Migration.Nintex.Mappings
{
	public interface IMapper
	{
		HashSet<Guid> UpdateFile(string file, OperationReporting opReport);
	}
}