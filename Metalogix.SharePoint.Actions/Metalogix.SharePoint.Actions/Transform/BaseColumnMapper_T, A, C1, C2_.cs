using Metalogix.Actions;
using Metalogix.Explorer;
using Metalogix.SharePoint;
using Metalogix.SharePoint.Options.Migration;
using Metalogix.SharePoint.Options.Migration.Mapping;
using Metalogix.SharePoint.Options.Transform;
using Metalogix.Transformers;
using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Actions.Transform
{
	public abstract class BaseColumnMapper<T, A, C1, C2> : Transformer<T, A, C1, C2, ColumnMappingOptions>
	where A : Metalogix.Actions.Action
	where C1 : NodeCollection
	where C2 : NodeCollection
	{
		public override string Name
		{
			get
			{
				return "Column Mapping";
			}
		}

		protected BaseColumnMapper()
		{
		}

		protected ColumnMappings GetColumnMappings(SPList sourceList, PasteListItemOptions options)
		{
			ColumnMappings columnMapping = null;
			if (options.ColumnMappings != null && (options.MapColumns || options.ColumnMappings.AutoMapCreatedAndModified || options.ColumnMappings.FieldsFilter != null && options.FilterFields) && options.ColumnMappings.ContainsColumnChanges)
			{
				columnMapping = ColumnMappings.MergeMappings(columnMapping, options.ColumnMappings);
			}
			if (options.ListFieldsFilterExpression != null && options.FilterFields)
			{
				ColumnMappings columnMapping1 = new ColumnMappings()
				{
					FieldsFilter = options.ListFieldsFilterExpression
				};
				columnMapping = ColumnMappings.MergeMappings(columnMapping, columnMapping1);
			}
			if (options.ApplyNewContentTypes && options.ContentTypeApplicationObjects != null)
			{
				foreach (ContentTypeApplicationOptionsCollection contentTypeApplicationObject in options.ContentTypeApplicationObjects)
				{
					if (!contentTypeApplicationObject.AppliesTo(sourceList))
					{
						continue;
					}
					if (contentTypeApplicationObject.ColumnMappings == null || !contentTypeApplicationObject.ColumnMappings.ContainsColumnChanges)
					{
						break;
					}
					columnMapping = ColumnMappings.MergeMappings(columnMapping, contentTypeApplicationObject.ColumnMappings);
					break;
				}
			}
			return columnMapping;
		}
	}
}