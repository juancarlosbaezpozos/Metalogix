using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Metalogix.Azure.ConsoleProcessor
{
	public static class Serializer
	{
		public static string Serialize<T>(this ICommand command, T objectToSerialize)
		{
			return Serializer.Serialize<T>(objectToSerialize).ToString();
		}

		public static StringBuilder Serialize<T>(T objectToSerialize)
		{
			XmlWriterSettings xmlWriterSetting = new XmlWriterSettings()
			{
				OmitXmlDeclaration = true,
				Encoding = Encoding.UTF8
			};
			XmlWriterSettings xmlWriterSetting1 = xmlWriterSetting;
			StringBuilder stringBuilder = new StringBuilder();
			using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSetting1))
			{
				(new XmlSerializer(typeof(T))).Serialize(xmlWriter, objectToSerialize);
				xmlWriter.Flush();
				xmlWriter.Close();
			}
			return stringBuilder;
		}
	}
}