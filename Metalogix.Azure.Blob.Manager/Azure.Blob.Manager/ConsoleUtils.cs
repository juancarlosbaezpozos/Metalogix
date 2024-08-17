using Metalogix;
using Metalogix.Azure;
using Metalogix.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace Metalogix.Azure.Blob.Manager
{
	public static class ConsoleUtils
	{
		private const string ProcessorName = "Metalogix.Azure.ConsoleProcessor.exe";

		private static string _processorPathName;

		public static string ProcessorPathName
		{
			get
			{
				if (ConsoleUtils._processorPathName == null)
				{
					bool flag = false;
					try
					{
						DirectoryInfo parent = (new DirectoryInfo(Assembly.GetExecutingAssembly().Location)).Parent;
						if (parent != null)
						{
							string str = Path.Combine(parent.FullName, "Metalogix.Azure.ConsoleProcessor.exe");
							if (!File.Exists(str))
							{
								flag = true;
							}
							else
							{
								ConsoleUtils._processorPathName = str;
							}
						}
					}
					catch
					{
						flag = true;
					}
					if (flag)
					{
						ConsoleUtils._processorPathName = Path.Combine(ApplicationData.CommonApplicationPath, "Metalogix.Azure.ConsoleProcessor.exe");
					}
				}
				return ConsoleUtils._processorPathName;
			}
		}

		private static T DeserializeString<T>(string xml)
		{
			T t;
			using (StringReader stringReader = new StringReader(xml))
			{
				t = (T)(new XmlSerializer(typeof(T))).Deserialize(stringReader);
				stringReader.Close();
			}
			return t;
		}

		private static T GetLaunchExceptionResult<T>(Exception ex, string fileName, string args)
		where T : Response, new()
		{
			StringBuilder stringBuilder = new StringBuilder();
			ExceptionDetail exceptionMessageAndDetail = ExceptionUtils.GetExceptionMessageAndDetail(ex);
			stringBuilder.AppendLine(string.Format("Error occured in Launching Process: '{0}'", fileName));
			stringBuilder.AppendLine(string.Format("Arguments: '{0}'", args));
			stringBuilder.AppendLine();
			stringBuilder.AppendLine(exceptionMessageAndDetail.Message);
			stringBuilder.AppendLine(exceptionMessageAndDetail.Detail);
			T str = Activator.CreateInstance<T>();
			str.Success = false;
			str.Details = stringBuilder.ToString();
			return str;
		}

		private static T HandleProcessResult<T>(string processStandardOut)
		where T : Response, new()
		{
			T t;
			if (string.IsNullOrEmpty(processStandardOut))
			{
				T t1 = Activator.CreateInstance<T>();
				t1.Success = false;
				t1.Details = "Console.StandardOutput is empty! Please check that the correct version of Metalogix.Azure.ConsoleProcessor is used";
				t = t1;
			}
			else
			{
				t = ConsoleUtils.DeserializeString<T>(processStandardOut);
			}
			return t;
		}

		public static T Launch<T>(string args)
		where T : Response, new()
		{
			T launchExceptionResult;
			T t;
			using (Process process = new Process())
			{
				ProcessStartInfo processStartInfo = new ProcessStartInfo()
				{
					UseShellExecute = false,
					CreateNoWindow = true,
					ErrorDialog = false,
					FileName = ConsoleUtils.ProcessorPathName,
					Arguments = args,
					RedirectStandardOutput = true
				};
				ProcessStartInfo processStartInfo1 = processStartInfo;
				process.StartInfo = processStartInfo1;
				try
				{
					process.Start();
					string end = process.StandardOutput.ReadToEnd();
					process.WaitForExit();
					launchExceptionResult = ConsoleUtils.HandleProcessResult<T>(end);
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					launchExceptionResult = ConsoleUtils.GetLaunchExceptionResult<T>(exception, processStartInfo1.FileName, processStartInfo1.Arguments);
					t = launchExceptionResult;
					return t;
				}
				return launchExceptionResult;
			}
			return t;
		}
	}
}