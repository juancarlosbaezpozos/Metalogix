using Metalogix.SharePoint;
using System;
using System.Collections;

namespace Metalogix.SharePoint.Administration.LinkManagement
{
	public interface ILinkDictionary
	{
		bool IgnoreQueryString
		{
			get;
			set;
		}

		bool IncludeInUpdate
		{
			get;
		}

		int KeyCount
		{
			get;
		}

		ICollection LinkTagNames
		{
			get;
		}

		string LookupProperty
		{
			get;
		}

		object LookUp(object oOldLink);

		void MapContentSourceURLs(SPFolder spNode);

		event ErrorEncounteredEventHandler ErrorEncountered;

		event LinkDuplicateHandler LinkDuplicateFound;

		event MappingItemEventHandler MappingItem;
	}
}