using Metalogix.Actions;
using Metalogix.Licensing;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Attributes;
using Metalogix.UI.WinForms.Properties;
using System;

namespace Metalogix.UI.WinForms.Transformers
{
	[IsAdvancedPrecondition(true)]
	[RequiresFullEdition(false)]
	public class EnableCustomTransformersSetting : BooleanApplicationSetting
	{
		public override string DisplayText
		{
			get
			{
				return Resources.AppSetting_EnableCustomTransformers;
			}
		}

		public override string ImageName
		{
			get
			{
				return "Metalogix.UI.WinForms.Icons.Settings.EnableCustomTransformers16.png";
			}
		}

		public override string LargeImageName
		{
			get
			{
				return "Metalogix.UI.WinForms.Icons.Settings.EnableCustomTransformers32.png";
			}
		}

		public override bool Value
		{
			get
			{
				return ActionConfigurationVariables.EnableCustomTransformers;
			}
			set
			{
				ActionConfigurationVariables.EnableCustomTransformers = value;
			}
		}

		public EnableCustomTransformersSetting()
		{
		}
	}
}