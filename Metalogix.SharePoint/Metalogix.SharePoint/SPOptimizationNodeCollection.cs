using Metalogix.DataStructures.Generic;
using System;
using System.Reflection;

namespace Metalogix.SharePoint
{
	public class SPOptimizationNodeCollection : SerializableIndexedList<SPOptimizationNode>
	{
		private bool m_bHasUniqueValuesAtThisLevel = false;

		public bool HasUniqueValuesAtThisLevel
		{
			get
			{
				return this.m_bHasUniqueValuesAtThisLevel;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public SPOptimizationNode this[string sName]
		{
			get
			{
				return base[sName];
			}
		}

		internal SPOptimizationNodeCollection() : base("Name")
		{
		}

		public override void Add(SPOptimizationNode item)
		{
			this.m_bHasUniqueValuesAtThisLevel = (this.m_bHasUniqueValuesAtThisLevel ? true : item.HasUniqueValues);
			base.Add(item);
		}
	}
}