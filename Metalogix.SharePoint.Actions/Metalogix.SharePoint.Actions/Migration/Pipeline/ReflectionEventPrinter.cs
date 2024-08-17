using System;
using System.Reflection;
using System.Text;

namespace Metalogix.SharePoint.Actions.Migration.Pipeline
{
	public class ReflectionEventPrinter : IEventPrinter
	{
		public ReflectionEventPrinter()
		{
		}

		public string PrintEvent(IEvent e)
		{
			StringBuilder stringBuilder = new StringBuilder();
			PropertyInfo[] properties = e.GetType().GetProperties();
			for (int i = 0; i < (int)properties.Length; i++)
			{
				PropertyInfo propertyInfo = properties[i];
				object value = propertyInfo.GetValue(e, null);
				if (value != null)
				{
					stringBuilder.AppendLine(string.Concat(propertyInfo.Name, " = ", value));
				}
				else
				{
					stringBuilder.AppendLine(string.Concat(propertyInfo.Name, " = null"));
				}
			}
			return stringBuilder.ToString();
		}
	}
}