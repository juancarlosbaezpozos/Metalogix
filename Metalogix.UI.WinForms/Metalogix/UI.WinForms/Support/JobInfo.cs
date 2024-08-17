using Metalogix.Actions;
using Metalogix.Core.Support;
using Metalogix.Explorer;
using Metalogix.Jobs;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Metalogix.UI.WinForms.Support
{
	public class JobInfo : IHasSupportInfo
	{
		public Metalogix.Jobs.Job Job
		{
			get;
			set;
		}

		public JobInfo()
		{
		}

		private void WriteConnectionSupportInfo(TextWriter output, IXMLAbleList nodes)
		{
			if (nodes == null || nodes.Count < 1)
			{
				output.WriteLine("Connection is either not required or not found.");
				return;
			}
			Node item = nodes[0] as Node;
			if (item == null)
			{
				return;
			}
			Connection connection = item.Connection;
			if (connection == null)
			{
				return;
			}
			IHasSupportInfo hasSupportInfo = connection as IHasSupportInfo;
			if (hasSupportInfo == null)
			{
				output.WriteLine(string.Concat(connection.GetType().Name, " does not have support info."));
				return;
			}
			using (StringWriter stringWriter = new StringWriter())
			{
				hasSupportInfo.WriteSupportInfo(stringWriter);
				string str = stringWriter.ToString();
				string[] newLine = new string[] { Environment.NewLine };
				string[] strArrays = str.Split(newLine, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < (int)strArrays.Length; i++)
				{
					output.WriteLine("\t{0}", strArrays[i]);
				}
			}
		}

		private void WriteSourceInfo(TextWriter output)
		{
			try
			{
				output.WriteLine("Job Source:");
				this.WriteConnectionSupportInfo(output, this.Job.SourceList);
			}
			catch (Exception exception)
			{
				output.WriteLine("Unable to retrieve source information. (Exception: {0})", exception.Message);
			}
		}

		public void WriteSupportInfo(TextWriter output)
		{
			try
			{
				if (this.Job.Parent != null && this.Job.Parent.JobHistoryDb != null)
				{
					output.WriteLine("Job Database Type: {0}", this.Job.Parent.JobHistoryDb.AdapterType);
				}
				output.WriteLine("Job Action: {0}", this.Job.Action.Name);
				this.WriteSourceInfo(output);
				this.WriteTargetInfo(output);
			}
			catch (Exception exception)
			{
				output.WriteLine("Unable to retrieve job info. (Exception: {0})", exception.Message);
			}
		}

		private void WriteTargetInfo(TextWriter output)
		{
			try
			{
				output.WriteLine("Job Target:");
				this.WriteConnectionSupportInfo(output, this.Job.TargetList);
			}
			catch (Exception exception)
			{
				output.WriteLine("Unable to retrieve target information. (Exception: {0})", exception.Message);
			}
		}
	}
}