using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.Transformers;
using System;

namespace Metalogix.SharePoint.Actions.Transform
{
	public class MapUsers : PreconfiguredTransformer<SPUser, CopyUsersAction, SPUserCollection, SPUserCollection>
	{
		public override string Name
		{
			get
			{
				return "Map Users";
			}
		}

		public MapUsers()
		{
		}

		public override void BeginTransformation(CopyUsersAction action, SPUserCollection sources, SPUserCollection targets)
		{
		}

		public override void EndTransformation(CopyUsersAction action, SPUserCollection sources, SPUserCollection targets)
		{
		}

		public override SPUser Transform(SPUser dataObject, CopyUsersAction action, SPUserCollection sources, SPUserCollection targets)
		{
			return SPGlobalMappings.Map(dataObject, action.IsTargetSharePointOnline) as SPUser;
		}
	}
}