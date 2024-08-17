using Metalogix.UI.WinForms.Explorer;
using System;

namespace Metalogix.UI.WinForms.Attributes
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
	public class ItemsViewAttribute : Attribute
	{
		private readonly ItemsViewType m_viewType;

		private readonly Type m_nodeType;

		public Type NodeType
		{
			get
			{
				return this.m_nodeType;
			}
		}

		public ItemsViewType ViewType
		{
			get
			{
				return this.m_viewType;
			}
		}

		public ItemsViewAttribute(Type nodeType, ItemsViewType viewType)
		{
			this.m_viewType = viewType;
			this.m_nodeType = nodeType;
		}
	}
}