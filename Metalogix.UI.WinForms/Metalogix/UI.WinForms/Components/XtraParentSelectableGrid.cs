using DevExpress.XtraGrid;
using Metalogix.Actions;
using Metalogix.DataStructures.Generic;
using System;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Components
{
	internal class XtraParentSelectableGrid : GridControl, IHasSelectableObjects
	{
		public IXMLAbleList SelectedObjects
		{
			get
			{
				IHasSelectableObjects hasSelectableObject;
				Control parent = base.Parent;
				if (parent == null)
				{
					return new CommonSerializableList<object>();
				}
				do
				{
					hasSelectableObject = parent as IHasSelectableObjects;
					parent = parent.Parent;
				}
				while (hasSelectableObject == null && parent != null);
				if (hasSelectableObject == null)
				{
					return new CommonSerializableList<object>();
				}
				return hasSelectableObject.SelectedObjects;
			}
		}

		public XtraParentSelectableGrid()
		{
		}
	}
}