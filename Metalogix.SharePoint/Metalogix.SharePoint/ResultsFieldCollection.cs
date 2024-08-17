using Metalogix.Explorer;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Metalogix.SharePoint
{
	public class ResultsFieldCollection : FieldCollection
	{
		private List<ResultsField> m_fields = new List<ResultsField>();

		public int Count
		{
			get
			{
				return this.m_fields.Count;
			}
		}

		public string XML
		{
			get
			{
				return "<Fields><Field Name=\"Path\"><Field Name=\"FileName\"><Field Name=\"Created\"><Field Name=\"Modified\"></Fields>";
			}
		}

		public ResultsFieldCollection()
		{
			this.m_fields.Add(new ResultsField("FileName", "Name", typeof(string)));
			this.m_fields.Add(new ResultsField("Title", typeof(string)));
			this.m_fields.Add(new ResultsField("ContentType", "Content Type", typeof(string)));
			this.m_fields.Add(new ResultsField("Author", typeof(string)));
			this.m_fields.Add(new ResultsField("Editor", typeof(string)));
			this.m_fields.Add(new ResultsField("Created", typeof(DateTime)));
			this.m_fields.Add(new ResultsField("Modified", typeof(DateTime)));
			this.m_fields.Add(new ResultsField("ListTitle", "List Name", typeof(string)));
			this.m_fields.Add(new ResultsField("WebTitle", "Site Name", typeof(string)));
			this.m_fields.Add(new ResultsField("Path", typeof(string)));
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_fields.GetEnumerator();
		}
	}
}