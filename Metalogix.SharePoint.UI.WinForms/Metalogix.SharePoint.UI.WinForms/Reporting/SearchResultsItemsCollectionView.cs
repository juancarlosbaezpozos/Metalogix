using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.UI.Standard.Explorer;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Metalogix.SharePoint.UI.WinForms.Reporting
{
	public class SearchResultsItemsCollectionView : STItemCollectionView
	{
		public override IXMLAbleList SelectedObjects
		{
			get
			{
				List<SPNode> sPNodes = new List<SPNode>();
				foreach (SPSearchResult selectedObject in base.SelectedObjects)
				{
					sPNodes.Add(selectedObject.Node);
				}
				return new NodeCollection(sPNodes.ToArray());
			}
		}

		public SearchResultsItemsCollectionView()
		{
		}
	}
}