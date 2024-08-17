using System;

namespace Metalogix.SharePoint.Administration.LinkManagement
{
	public interface ILinkCorrector
	{
		void Abort();

		bool CorrectComponent(object oComponent, ILinkDictionary linkDictionary, bool bReportOnly);

		void Pause();

		void Resume();

		bool SupportsComponent(object oComponent);

		event ErrorEncounteredEventHandler ErrorEncountered;

		event LinkCorrectedEventHandler LinkCorrected;
	}
}