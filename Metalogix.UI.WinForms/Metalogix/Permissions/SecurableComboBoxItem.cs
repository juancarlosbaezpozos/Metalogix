using Metalogix.Explorer;
using System;

namespace Metalogix.Permissions
{
	internal class SecurableComboBoxItem
	{
		private ISecurableObject m_node;

		public ISecurableObject SecurableObject
		{
			get
			{
				return this.m_node;
			}
		}

		public SecurableComboBoxItem(ISecurableObject node)
		{
			this.m_node = node;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			return this.ToString().Equals(obj.ToString());
		}

		public override int GetHashCode()
		{
			return this.GetHashCode();
		}

		public override string ToString()
		{
			if (this.m_node == null || !(this.m_node is Node))
			{
				return string.Empty;
			}
			return (this.m_node as Node).DisplayUrl;
		}
	}
}