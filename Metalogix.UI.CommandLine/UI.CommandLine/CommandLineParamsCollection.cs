using System;
using System.Collections.Specialized;
using System.Reflection;

namespace Metalogix.UI.CommandLine
{
	public class CommandLineParamsCollection
	{
		private const string NULLPARAM = null;

		private StringDictionary _list = new StringDictionary();

		public string this[string name]
		{
			get
			{
				return this._list[name];
			}
		}

		public CommandLineParamsCollection()
		{
		}

		public void Add(string name)
		{
			this._list.Add(name, null);
		}

		public void Add(string name, string value)
		{
			this._list.Add(name, value);
		}

		public bool Contains(string name)
		{
			return this._list.ContainsKey(name);
		}

		public T Get<T>(string name)
		{
			object num;
			string item = this[name];
			if (typeof(T) != typeof(int))
			{
				num = item;
			}
			else
			{
				num = Convert.ToInt32(item);
			}
			return (T)num;
		}

		public bool HasValue(string name)
		{
			if (!this.Contains(name))
			{
				return false;
			}
			return string.Compare(this._list[name], null, StringComparison.Ordinal) != 0;
		}
	}
}