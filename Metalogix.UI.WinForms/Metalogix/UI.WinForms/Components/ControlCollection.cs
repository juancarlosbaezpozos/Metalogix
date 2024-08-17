using Metalogix.DataStructures.Generic;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Components
{
	public class ControlCollection : SerializableList<UserControl>
	{
		public override Type CollectionType
		{
			get
			{
				if (base.Count == 0)
				{
					return base.CollectionType;
				}
				return this[0].GetType();
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public override bool IsSet
		{
			get
			{
				return true;
			}
		}

		public override UserControl this[UserControl key]
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public ControlCollection(List<UserControl> controls) : base(controls.ToArray())
		{
		}

		public ControlCollection(UserControl control) : base(new UserControl[] { control })
		{
		}
	}
}