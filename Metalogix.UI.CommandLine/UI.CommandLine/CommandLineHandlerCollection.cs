using System;
using System.Collections.Generic;
using System.Reflection;

namespace Metalogix.UI.CommandLine
{
	internal class CommandLineHandlerCollection : List<ICommandLineHandler>
	{
		public CommandLineHandlerCollection()
		{
		}

		public ICommandLineHandler GetHandler(CommandLineParamsCollection param)
		{
			ICommandLineHandler commandLineHandler;
			List<ICommandLineHandler>.Enumerator enumerator = base.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ICommandLineHandler current = enumerator.Current;
					if (!current.CanHandle(param))
					{
						continue;
					}
					commandLineHandler = current;
					return commandLineHandler;
				}
				return null;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return commandLineHandler;
		}

		public void Load()
		{
			Assembly entryAssembly = Assembly.GetEntryAssembly();
			List<AssemblyName> assemblyNames = new List<AssemblyName>(entryAssembly.GetReferencedAssemblies())
			{
				entryAssembly.GetName()
			};
			string fullName = typeof(ICommandLineHandler).FullName;
			foreach (AssemblyName assemblyName in assemblyNames)
			{
				Type[] exportedTypes = Assembly.Load(assemblyName).GetExportedTypes();
				for (int i = 0; i < (int)exportedTypes.Length; i++)
				{
					Type type = exportedTypes[i];
					try
					{
						if (type.IsClass && !type.IsAbstract && type.GetInterface(fullName) != null)
						{
							ICommandLineHandler commandLineHandler = (ICommandLineHandler)Activator.CreateInstance(type);
							if (!base.Contains(commandLineHandler))
							{
								base.Add(commandLineHandler);
							}
						}
					}
					catch (Exception exception)
					{
					}
				}
			}
		}
	}
}