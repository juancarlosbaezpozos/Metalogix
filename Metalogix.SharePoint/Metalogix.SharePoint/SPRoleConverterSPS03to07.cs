using System;

namespace Metalogix.SharePoint
{
	public class SPRoleConverterSPS03to07 : SPRoleConverter03to07
	{
		public override Type SourceRoleType
		{
			get
			{
				return typeof(SPS2003Role);
			}
		}

		public SPRoleConverterSPS03to07()
		{
		}
	}
}