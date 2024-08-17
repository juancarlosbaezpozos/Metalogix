using Metalogix.Actions;
using Metalogix.Licensing;
using System;
using System.Collections;
using System.Text;
using System.Windows.Forms;

namespace Metalogix.UI.WinForms.Actions
{
	[Batchable(false, "")]
	[LaunchAsJob(false)]
	[LicensedProducts(ProductFlags.UnifiedContentMatrixKey)]
	[RequiresWriteAccess(false)]
	[RunAsync(false)]
	[ShowInMenus(true)]
	[TargetCardinality(Cardinality.OneOrMore)]
	[TargetType(typeof(LogItem))]
	public abstract class CopyLogItemAction : Metalogix.Actions.Action, ILogAction
	{
		protected bool m_bIncludeDetails;

		public bool IncludeDetails
		{
			get
			{
				return this.m_bIncludeDetails;
			}
			set
			{
				this.m_bIncludeDetails = value;
			}
		}

		protected CopyLogItemAction()
		{
		}

		protected override void RunAction(IXMLAbleList source, IXMLAbleList target)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Concat("Time\tOperation\tItem\tSource\tTarget\tStatus\tInformation", (this.IncludeDetails ? "\tDetails\tSourceContest\tTargetContent" : "")));
			foreach (LogItem logItem in target)
			{
				StringBuilder stringBuilder1 = stringBuilder;
				object[] timeStamp = new object[] { logItem.TimeStamp, logItem.Operation, logItem.ItemName, logItem.Source, logItem.Target, logItem.Status, logItem.Information };
				stringBuilder1.Append(string.Concat(string.Format("\r\n{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", timeStamp), (this.IncludeDetails ? string.Format("{0}\t{1}\t{2}", logItem.Details, logItem.SourceContent, logItem.TargetContent) : "")));
			}
			Clipboard.SetDataObject(stringBuilder.ToString());
		}
	}
}