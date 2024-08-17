using Metalogix;
using Metalogix.Core;
using Metalogix.Data.Filters;
using Metalogix.Utilities;
using System;
using System.Collections;
using System.Management.Automation;
using System.Reflection;
using System.Xml;

namespace Metalogix.Commands
{
	[Cmdlet("New", "MetalogixSerializableObject")]
	public class NewMetalogixSerializableObjectCmdlet : AssemblyBindingCmdlet
	{
		private const string ASSEMBLYQUALIFIEDNAME = "{0}, {1}, Version={2}, Culture=neutral, PublicKeyToken=1bd76498c7c4cba4";

		private string m_sType;

		private string m_sAssembly;

		private string m_sXML;

		private bool m_bEnumerate;

		[Parameter(Mandatory=true, Position=1, HelpMessage="The name of the assembly containing the type to be created.")]
		public string AssemblyName
		{
			get
			{
				return this.m_sAssembly;
			}
			set
			{
				this.m_sAssembly = value;
			}
		}

		[Parameter(Mandatory=false, HelpMessage="If set, any collections returned by the cmdlet will enumerated. Otherwise, collections will be returned as a single object.")]
		public SwitchParameter Enumerate
		{
			get
			{
				return this.m_bEnumerate;
			}
			set
			{
				this.m_bEnumerate = value;
			}
		}

		[Parameter(Mandatory=true, Position=2, HelpMessage="The XML representation of a single serializable Metalogix object.")]
		public string SerializedValue
		{
			get
			{
				return this.m_sXML;
			}
			set
			{
				this.m_sXML = value;
			}
		}

		[Parameter(Mandatory=true, Position=0, HelpMessage="The name of the type of object to create.")]
		public string TypeName
		{
			get
			{
				return this.m_sType;
			}
			set
			{
				this.m_sType = value;
			}
		}

		public NewMetalogixSerializableObjectCmdlet()
		{
		}

		protected override void BeginProcessing()
		{
			base.BeginProcessing();
			string str = TypeUtils.UpdateType(string.Format("{0}, {1}, Version={2}, Culture=neutral, PublicKeyToken=1bd76498c7c4cba4", this.TypeName, this.AssemblyName, ConfigurationVariables.AssemblyVersionString));
			Type type = Type.GetType(str, false, false);
			if (type == null)
			{
				base.ThrowTerminatingError(new ErrorRecord(new ArgumentException(string.Concat("The type could not be found based on the information provided: ", str), "TypeName"), "InvalidArgument", ErrorCategory.InvalidArgument, str));
			}
			try
			{
				object obj = null;
				if (!type.IsInterface || type != typeof(IFilterExpression))
				{
					XmlNode xmlNode = XmlUtility.StringToXmlNode(this.SerializedValue);
					if (type.GetConstructor(new Type[] { typeof(XmlNode) }) == null)
					{
						Type[] typeArray = new Type[] { typeof(XmlNode) };
						MethodInfo method = type.GetMethod("FromXML", BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis, typeArray, null);
						if (method == null)
						{
							base.ThrowTerminatingError(new ErrorRecord(new ArgumentException(string.Concat("The type found is not a serializable type: ", str), "TypeName"), "InvalidArgument", ErrorCategory.InvalidArgument, str));
						}
						obj = Activator.CreateInstance(type);
						object[] objArray = new object[] { xmlNode };
						method.Invoke(obj, objArray);
					}
					else
					{
						object[] objArray1 = new object[] { xmlNode };
						obj = Activator.CreateInstance(type, objArray1);
					}
				}
				else
				{
					obj = FilterExpression.ParseExpression(this.SerializedValue);
				}
				if (!this.m_bEnumerate)
				{
					base.WriteObject(obj);
				}
				else
				{
					this.WriteObjectEnumerateRecursively(obj);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				base.ThrowTerminatingError(new ErrorRecord((exception.InnerException != null ? exception.InnerException : exception), "UnexpectedError", ErrorCategory.InvalidArgument, str));
			}
		}

		protected override void EndProcessing()
		{
			base.EndProcessing();
		}

		private void WriteObjectEnumerateRecursively(object objValue)
		{
			if (!(objValue is IEnumerable))
			{
				base.WriteObject(objValue);
			}
			else
			{
				foreach (object obj in (IEnumerable)objValue)
				{
					this.WriteObjectEnumerateRecursively(obj);
				}
			}
		}
	}
}