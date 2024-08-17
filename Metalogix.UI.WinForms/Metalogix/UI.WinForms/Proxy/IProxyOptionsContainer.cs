using Metalogix.Connectivity.Proxy;
using System;

namespace Metalogix.UI.WinForms.Proxy
{
	public interface IProxyOptionsContainer
	{
		MLProxy Proxy
		{
			get;
			set;
		}
	}
}