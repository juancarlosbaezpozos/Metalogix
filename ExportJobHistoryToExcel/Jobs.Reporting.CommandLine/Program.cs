using Metalogix.Jobs.Reporting;
using Metalogix.Jobs.Reporting.CommandLine.Properties;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Metalogix.Jobs.Reporting.CommandLine
{
	internal class Program
	{
		public Program()
		{
		}

		private static void Main(string[] args)
		{
			try
			{
				if (args == null || (int)args.Length == 0)
				{
					throw new ArgumentException("Command cannot be called without arguments.");
				}
				Dictionary<char, string> chrs = new Dictionary<char, string>();
				string[] strArrays = args;
				for (int i = 0; i < (int)strArrays.Length; i++)
				{
					string str = strArrays[i];
					string upper = str.Substring(0, 3).ToUpper();
					if (!upper.StartsWith("/") || !upper.EndsWith(":"))
					{
						throw new ArgumentException(string.Format("Command line parameter \"{0}\" is not correctly formatted", upper));
					}
					string str1 = str.Remove(0, 3);
					chrs.Add(upper[1], str1);
				}
				Program.Arguments argument = Program.ParseArgumentDictionary(chrs);
				Console.WriteLine(Resources.ExportStarted, DateTime.Now);
				Console.WriteLine(Resources.ExportingJobHistory, argument.OutputFileName ?? "JobHistory.xslx", argument.OutputDirectoryPath);
				Console.WriteLine(Resources.ProcessingLogItems);
				int num = 0;
				ExcelReport.ProgressChanged += new ExcelReport.ProgressChangedEventHandler((int percentageComplete) => {
					if (percentageComplete <= num)
					{
						return;
					}
					num = percentageComplete;
					Console.Write(string.Concat('\r', Resources.PercentageComplete), percentageComplete);
					if (percentageComplete != 100)
					{
						return;
					}
					Console.WriteLine();
					Console.WriteLine(Resources.SavingExcelReport);
				});
				AutoResetEvent autoResetEvent = new AutoResetEvent(false);
				ExcelReport.ExportComplete += new Action<Exception>((Exception error) => {
					if (error != null)
					{
						Console.WriteLine(Resources.ErrorDetail, error.Message);
					}
					Console.WriteLine(Resources.ExportComplete, DateTime.Now);
					autoResetEvent.Set();
				});
				switch (argument.DbType)
				{
					case DataSource.SqlCe:
					{
						if (!string.IsNullOrEmpty(argument.OutputFileName))
						{
							ExcelReport.CreateFromSqlCeFile(argument.SqlCeDbFilePath, argument.OutputDirectoryPath, argument.OutputFileName, argument.Overwrite, argument.JobsIds, true);
							break;
						}
						else
						{
							ExcelReport.CreateFromSqlCeFile(argument.SqlCeDbFilePath, argument.OutputDirectoryPath, "JobHistory.xlsx", argument.Overwrite, argument.JobsIds, true);
							break;
						}
					}
					case DataSource.SqlServer:
					{
						if (!string.IsNullOrEmpty(argument.OutputFileName))
						{
							ExcelReport.CreateFromSqlServer(argument.ServerName, argument.DatabaseName, argument.AuthType, argument.OutputDirectoryPath, argument.OutputFileName, argument.Overwrite, argument.JobsIds, argument.Username, argument.Password, true);
							break;
						}
						else
						{
							ExcelReport.CreateFromSqlServer(argument.ServerName, argument.DatabaseName, argument.AuthType, argument.OutputDirectoryPath, "JobHistory.xlsx", argument.Overwrite, argument.JobsIds, argument.Username, argument.Password, true);
							break;
						}
					}
				}
				autoResetEvent.WaitOne();
			}
			catch (ArgumentException argumentException1)
			{
				ArgumentException argumentException = argumentException1;
				Console.WriteLine(Resources.ArgumentExceptionText);
				Console.WriteLine(Resources.ErrorDetail, argumentException.Message);
				Console.WriteLine();
				Console.WriteLine(Resources.CommandUsage);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Console.WriteLine(Resources.GeneralErrorText);
				Console.WriteLine(Resources.ErrorDetail, exception.Message);
			}
		}

		private static Program.Arguments ParseArgumentDictionary(IDictionary<char, string> argsDictionary)
		{
			string str;
			string item;
			string item1;
			IEnumerable<string> strs;
			string str1;
			string item2;
			string str2;
			string item3;
			string str3;
			string item4;
			if (argsDictionary == null)
			{
				throw new ArgumentNullException("argsDictionary");
			}
			Program.Arguments argument = new Program.Arguments();
			if (argsDictionary.ContainsKey('O'))
			{
				item = argsDictionary['O'];
			}
			else
			{
				item = null;
			}
			argument.OutputDirectoryPath = item;
			if (argsDictionary.ContainsKey('F'))
			{
				item1 = argsDictionary['F'];
			}
			else
			{
				item1 = null;
			}
			argument.OutputFileName = item1;
			argument.Overwrite = (!argsDictionary.ContainsKey('W') ? false : argsDictionary['W'] == "T");
			if (argsDictionary.ContainsKey('J'))
			{
				strs = argsDictionary['J'].Split(new char[] { ',' });
			}
			else
			{
				strs = null;
			}
			argument.JobsIds = strs;
			if (argsDictionary.ContainsKey('I'))
			{
				str1 = argsDictionary['I'];
			}
			else
			{
				str1 = null;
			}
			argument.SqlCeDbFilePath = str1;
			if (argsDictionary.ContainsKey('N'))
			{
				item2 = argsDictionary['N'];
			}
			else
			{
				item2 = null;
			}
			argument.ServerName = item2;
			if (argsDictionary.ContainsKey('D'))
			{
				str2 = argsDictionary['D'];
			}
			else
			{
				str2 = null;
			}
			argument.DatabaseName = str2;
			if (argsDictionary.ContainsKey('U'))
			{
				item3 = argsDictionary['U'];
			}
			else
			{
				item3 = null;
			}
			argument.Username = item3;
			if (argsDictionary.ContainsKey('P'))
			{
				str3 = argsDictionary['P'];
			}
			else
			{
				str3 = null;
			}
			argument.Password = str3;
			Program.Arguments argument1 = argument;
			if (argsDictionary.ContainsKey('T'))
			{
				string str4 = argsDictionary['T'];
				string str5 = str4;
				if (str4 != null)
				{
					if (str5 == "S")
					{
						argument1.DbType = DataSource.SqlServer;
						if (argsDictionary.ContainsKey('A'))
						{
							item4 = argsDictionary['A'];
							str = item4;
							if (item4 != null)
							{
								if (str == "I")
								{
									argument1.AuthType = SqlAuthenticationType.Windows;
									return argument1;
								}
								else
								{
									if (str != "S")
									{
										throw new ArgumentException("Authtype (A) parameter has an incorrect value.");
									}
									argument1.AuthType = SqlAuthenticationType.SqlServer;
									return argument1;
								}
							}
							throw new ArgumentException("Authtype (A) parameter has an incorrect value.");
						}
						return argument1;
					}
					else
					{
						if (str5 != "C")
						{
							throw new ArgumentException("Dbtype (T) parameter has an incorrect value.");
						}
						argument1.DbType = DataSource.SqlCe;
						if (argsDictionary.ContainsKey('A'))
						{
							item4 = argsDictionary['A'];
							str = item4;
							if (item4 != null)
							{
								if (str == "I")
								{
									argument1.AuthType = SqlAuthenticationType.Windows;
									return argument1;
								}
								else
								{
									if (str != "S")
									{
										throw new ArgumentException("Authtype (A) parameter has an incorrect value.");
									}
									argument1.AuthType = SqlAuthenticationType.SqlServer;
									return argument1;
								}
							}
							throw new ArgumentException("Authtype (A) parameter has an incorrect value.");
						}
						return argument1;
					}
				}
				throw new ArgumentException("Dbtype (T) parameter has an incorrect value.");
			}
			if (argsDictionary.ContainsKey('A'))
			{
				item4 = argsDictionary['A'];
				str = item4;
				if (item4 != null)
				{
					if (str == "I")
					{
						argument1.AuthType = SqlAuthenticationType.Windows;
						return argument1;
					}
					else
					{
						if (str != "S")
						{
							throw new ArgumentException("Authtype (A) parameter has an incorrect value.");
						}
						argument1.AuthType = SqlAuthenticationType.SqlServer;
						return argument1;
					}
				}
				throw new ArgumentException("Authtype (A) parameter has an incorrect value.");
			}
			return argument1;
		}

		private struct Arguments
		{
			public string OutputDirectoryPath;

			public string OutputFileName;

			public bool Overwrite;

			public IEnumerable<string> JobsIds;

			public DataSource DbType;

			public string SqlCeDbFilePath;

			public string ServerName;

			public string DatabaseName;

			public SqlAuthenticationType AuthType;

			public string Username;

			public string Password;
		}
	}
}