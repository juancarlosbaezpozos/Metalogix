using Metalogix.SharePoint;
using Metalogix.SharePoint.Actions;
using Metalogix.SharePoint.Actions.Migration;
using Metalogix.SharePoint.Actions.Properties;
using Metalogix.SharePoint.Adapters;
using Metalogix.SharePoint.Migration;
using Metalogix.Transformers;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Metalogix.SharePoint.Actions.Transform
{
	public class FormPageContentUpdater : PreconfiguredTransformer<SPFile, CopyListForms, SPFileCollection, SPFileCollection>
	{
		private CopyListForms CallingAction
		{
			get;
			set;
		}

		public override string Name
		{
			get
			{
				return "List Form Page Content Updater";
			}
		}

		public FormPageContentUpdater()
		{
		}

		public override void BeginTransformation(CopyListForms action, SPFileCollection sources, SPFileCollection targets)
		{
			this.CallingAction = action;
		}

		public override void EndTransformation(CopyListForms action, SPFileCollection sources, SPFileCollection targets)
		{
			this.CallingAction = null;
		}

		private void GetFormPageContentArea(string content, SharePointVersion version, out int startIdx, out int length)
		{
			string str;
			startIdx = -1;
			length = -1;
			str = (!version.IsSharePoint2003 ? "<asp:Content ?[^>]* ContentPlaceHolderId=\"PlaceHolderMain\"" : "<PlaceHolder ?[^>]* id=\"MSO_ContentDiv\"");
			Match match = Regex.Match(content, str, RegexOptions.IgnoreCase);
			if (match.Success)
			{
				AspxUtils.GetAreaBetweenTags(content, match.Index, out startIdx, out length);
			}
		}

		private void GetWebPartArea(string content, out int startIdx, out int length)
		{
			startIdx = -1;
			length = -1;
			try
			{
				if (!string.IsNullOrEmpty(content))
				{
					int num = content.IndexOf("<WebPartPages:ListFormWebPart ", StringComparison.OrdinalIgnoreCase);
					if (num > 0)
					{
						AspxUtils.GetTotalAreaOfTag(content, num, out startIdx, out length);
					}
				}
			}
			catch (Exception exception)
			{
				startIdx = -1;
				length = -1;
			}
		}

		private void MakeFormPageTemplateByVersion(string targetContent, SharePointVersion targetVersion, out string template, out int contentAreaIdx)
		{
			int num;
			int num1;
			string str;
			int num2;
			if (!string.IsNullOrEmpty(targetContent))
			{
				this.GetFormPageContentArea(targetContent, targetVersion, out num, out num1);
				if (num < 0)
				{
					throw new Exception(Resources.FailedToLocateTargetFormContentArea);
				}
				template = targetContent.Remove(num, num1);
				contentAreaIdx = num;
				return;
			}
			if (targetVersion.IsSharePoint2007)
			{
				str = "Metalogix.SharePoint.Actions.Migration.FormPages.2007FormPageTemplate.aspx";
			}
			else if (targetVersion.IsSharePoint2010)
			{
				str = "Metalogix.SharePoint.Actions.Migration.FormPages.2010FormPageTemplate.aspx";
			}
			else if (!targetVersion.IsSharePoint2013)
			{
				if (!targetVersion.IsSharePoint2016)
				{
					throw new Exception(string.Concat("No form template stored for SharePoint version ", targetVersion.VersionNumber));
				}
				str = "Metalogix.SharePoint.Actions.Migration.FormPages.2016FormPageTemplate.aspx";
			}
			else
			{
				str = "Metalogix.SharePoint.Actions.Migration.FormPages.2013FormPageTemplate.aspx";
			}
			using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(str))
			{
				template = (new StreamReader(manifestResourceStream)).ReadToEnd();
			}
			this.GetFormPageContentArea(template, targetVersion, out contentAreaIdx, out num2);
		}

		public override SPFile Transform(SPFile sourceForm, CopyListForms action, SPFileCollection sources, SPFileCollection targets)
		{
			string str = Encoding.UTF8.GetString(sourceForm.GetContent());
			string str1 = null;
			SPFile item = targets[sourceForm.Name];
			if (item != null)
			{
				str1 = Encoding.UTF8.GetString(item.GetContent());
			}
			str = this.UpdateFileContentForCopy(str, str1);
			if (sources.Web.Adapter.SharePointVersion.MajorVersion != targets.Web.Adapter.SharePointVersion.MajorVersion)
			{
				str = this.UpdateFileContentForCrossVersionCopy(str, str1, sources.Web.Adapter.SharePointVersion, targets.Web.Adapter.SharePointVersion);
			}
			sourceForm.SetContent(Encoding.UTF8.GetBytes(str));
			return sourceForm;
		}

		private string UpdateFileContentForCopy(string sourceContent, string targetContent)
		{
			int num;
			int num1;
			int num2;
			int num3;
			sourceContent = MigrationUtils.UpdateGuidsInFile(sourceContent, this.CallingAction.LinkCorrector);
			if (!string.IsNullOrEmpty(targetContent))
			{
				this.GetWebPartArea(targetContent, out num, out num1);
				if (num >= 0)
				{
					this.GetWebPartArea(sourceContent, out num2, out num3);
					if (num2 >= 0)
					{
						string str = targetContent.Substring(num, num1);
						sourceContent = sourceContent.Remove(num2, num3);
						return sourceContent.Insert(num2, str);
					}
				}
			}
			sourceContent = Regex.Replace(sourceContent, "(<DetailLink>|Url=&quot;)(.*?)(</DetailLink>|&quot;)", (Match m) => string.Concat(m.Groups[1].Value, this.CallingAction.LinkCorrector.CorrectUrl(m.Groups[2].Value), m.Groups[3].Value), RegexOptions.IgnoreCase);
			return sourceContent;
		}

		private string UpdateFileContentForCrossVersionCopy(string sourceContent, string targetContent, SharePointVersion sourceVersion, SharePointVersion targetVersion)
		{
			string str;
			int num;
			int num1;
			int num2;
			this.MakeFormPageTemplateByVersion(targetContent, targetVersion, out str, out num);
			this.GetFormPageContentArea(sourceContent, sourceVersion, out num1, out num2);
			if (num1 < 0)
			{
				throw new Exception(Resources.FailedToLocateSourceFormContentArea);
			}
			sourceContent = sourceContent.Substring(num1, num2);
			return str.Insert(num, sourceContent);
		}
	}
}