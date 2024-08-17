using Metalogix.Actions;
using Metalogix.Transformers;
using System;

namespace Metalogix.UI.WinForms.Interfaces
{
	public interface ITransformerTabConfig
	{
		Metalogix.Actions.Action Action
		{
			get;
			set;
		}

		ActionContext Context
		{
			get;
			set;
		}

		TransformerCollection Transformers
		{
			get;
			set;
		}
	}
}