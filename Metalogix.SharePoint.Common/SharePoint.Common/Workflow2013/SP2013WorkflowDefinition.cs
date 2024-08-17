using Metalogix.SharePoint.Common.Enums;
using System;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Common.Workflow2013
{
	public class SP2013WorkflowDefinition
	{
		public string FormField
		{
			get;
			set;
		}

		public Guid Id
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public SP2013WorkflowProperty[] Properties
		{
			get;
			set;
		}

		public bool Published
		{
			get;
			set;
		}

		public Guid RestrictToScope
		{
			get;
			set;
		}

		public SP2013WorkflowScopeType RestrictToType
		{
			get;
			set;
		}

		public string Xaml
		{
			get;
			set;
		}

		public SP2013WorkflowDefinition()
		{
		}

		public SP2013WorkflowDefinition Clone()
		{
			SP2013WorkflowDefinition sP2013WorkflowDefinition = new SP2013WorkflowDefinition()
			{
				Id = this.Id,
				Name = this.Name,
				Published = this.Published,
				Xaml = this.Xaml,
				RestrictToType = this.RestrictToType,
				RestrictToScope = this.RestrictToScope,
				Properties = this.Properties,
				FormField = this.FormField
			};
			return sP2013WorkflowDefinition;
		}
	}
}