using Metalogix.SharePoint;
using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Actions.Migration
{
	internal interface IPasteWebAction
	{
		Dictionary<SPWeb, SPWeb> SourceTargetNavCopyMap
		{
			get;
			set;
		}
	}
}