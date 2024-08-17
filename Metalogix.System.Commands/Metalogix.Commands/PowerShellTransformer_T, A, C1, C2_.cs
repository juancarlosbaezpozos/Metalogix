using Metalogix.Actions;
using Metalogix.Transformers;
using Metalogix.Transformers.Attributes;
using System;
using System.Collections;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;

namespace Metalogix.Commands
{
	[TransformerCardinality(Metalogix.Actions.Cardinality.ZeroOrMore)]
	public class PowerShellTransformer<T, A, C1, C2> : Transformer<T, A, C1, C2, PowerShellTransformerOptions>
	where A : Metalogix.Actions.Action
	where C1 : IEnumerable
	where C2 : IEnumerable
	{
		private const string REPOSITORY_KEY = "ScriptTransformers";

		private readonly object _runspaceLock;

		private int _runspaceRefCount;

		private Runspace _runspace;

		public override string Name
		{
			get
			{
				return "Invoke PowerShell Script";
			}
		}

		public PowerShellTransformer()
		{
		}

		public override void BeginTransformation(A action, C1 sources, C2 targets)
		{
			string beginTranformScriptFileName;
			lock (this._runspaceLock)
			{
				if (this._runspace == null)
				{
					this._runspace = RunspaceFactory.CreateRunspace();
					this._runspace.Open();
				}
				this._runspaceRefCount++;
			}
			string beginScript = null;
			try
			{
				beginScript = this.GetBeginScript(action);
				if (!string.IsNullOrEmpty(beginScript))
				{
					this._runspace.SessionStateProxy.SetVariable("Action", action);
					this._runspace.SessionStateProxy.SetVariable("Sources", sources);
					this._runspace.SessionStateProxy.SetVariable("Targets", targets);
					this._runspace.SessionStateProxy.SetVariable("Transformer", this);
					using (Pipeline pipeline = this._runspace.CreatePipeline())
					{
						pipeline.Commands.AddScript(beginScript);
						pipeline.Invoke();
					}
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				if (base.Options.BeginScriptLocation == ScriptFileLocation.File)
				{
					beginTranformScriptFileName = base.Options.BeginTranformScriptFileName;
				}
				else
				{
					beginTranformScriptFileName = null;
				}
				LogItem logItem = new LogItem("Invoke Begin Transform Script", beginTranformScriptFileName, null, null, ActionOperationStatus.Failed)
				{
					Exception = exception,
					SourceContent = beginScript
				};
				base.FireOperationStarted(logItem);
				base.FireOperationFinished(logItem);
			}
		}

		public LogItem CreateLogItem(string name)
		{
			return new LogItem(name, null, null, null, ActionOperationStatus.Running);
		}

		public override void EndTransformation(A action, C1 sources, C2 targets)
		{
			string endTransformScriptFileName;
			string endScript = null;
			try
			{
				endScript = this.GetEndScript(action);
				if (!string.IsNullOrEmpty(endScript))
				{
					this._runspace.SessionStateProxy.SetVariable("Action", action);
					this._runspace.SessionStateProxy.SetVariable("Sources", sources);
					this._runspace.SessionStateProxy.SetVariable("Targets", targets);
					this._runspace.SessionStateProxy.SetVariable("Transformer", this);
					using (Pipeline pipeline = this._runspace.CreatePipeline())
					{
						pipeline.Commands.AddScript(endScript);
						pipeline.Invoke();
					}
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				if (base.Options.EndScriptLocation == ScriptFileLocation.File)
				{
					endTransformScriptFileName = base.Options.EndTransformScriptFileName;
				}
				else
				{
					endTransformScriptFileName = null;
				}
				LogItem logItem = new LogItem("Invoke End Transform Script", endTransformScriptFileName, null, null, ActionOperationStatus.Failed)
				{
					Exception = exception,
					SourceContent = endScript
				};
				base.FireOperationStarted(logItem);
				base.FireOperationFinished(logItem);
			}
			lock (this._runspaceLock)
			{
				this._runspaceRefCount--;
				if (this._runspaceRefCount <= 0 && this._runspace != null)
				{
					this._runspace.Close();
					this._runspace.Dispose();
					this._runspace = null;
				}
			}
		}

		private string GetBeginScript(Metalogix.Actions.Action action)
		{
			switch (base.Options.BeginScriptLocation)
			{
				case ScriptFileLocation.None:
				{
					return null;
				}
				case ScriptFileLocation.File:
				{
					return PowerShellTransformer<T, A, C1, C2>.GetScriptValue(action, base.Options.BeginTranformScriptFileName);
				}
				case ScriptFileLocation.Configuration:
				{
					return base.Options.FullBeginTransformScript;
				}
				default:
				{
					return null;
				}
			}
		}

		private string GetEndScript(Metalogix.Actions.Action action)
		{
			switch (base.Options.EndScriptLocation)
			{
				case ScriptFileLocation.None:
				{
					return null;
				}
				case ScriptFileLocation.File:
				{
					return PowerShellTransformer<T, A, C1, C2>.GetScriptValue(action, base.Options.EndTransformScriptFileName);
				}
				case ScriptFileLocation.Configuration:
				{
					return base.Options.FullEndTransformScript;
				}
				default:
				{
					return null;
				}
			}
		}

		private static string GetScriptValue(Metalogix.Actions.Action action, string fileName)
		{
			string valueForKey = action.TransformationRepository.GetValueForKey("ScriptTransformers", fileName);
			if (valueForKey == null)
			{
				string str = null;
				if ((new FileInfo(fileName)).Exists)
				{
					str = File.ReadAllText(fileName);
				}
				action.TransformationRepository.Add("ScriptTransformers", fileName, str);
				valueForKey = str;
			}
			return valueForKey;
		}

		private string GetTransformScript(Metalogix.Actions.Action action)
		{
			switch (base.Options.TransformScriptLocation)
			{
				case ScriptFileLocation.None:
				{
					return null;
				}
				case ScriptFileLocation.File:
				{
					return PowerShellTransformer<T, A, C1, C2>.GetScriptValue(action, base.Options.TransformScriptFileName);
				}
				case ScriptFileLocation.Configuration:
				{
					return base.Options.FullTransformScript;
				}
				default:
				{
					return null;
				}
			}
		}

		public override T Transform(T dataObject, A action, C1 sources, C2 targets)
		{
			T t;
			string transformScriptFileName;
			string str;
			string transformScript = null;
			T t1 = dataObject;
			bool flag = false;
			lock (this._runspaceLock)
			{
				if (this._runspace == null)
				{
					this._runspace = RunspaceFactory.CreateRunspace();
					this._runspace.Open();
					flag = true;
				}
				try
				{
					try
					{
						transformScript = this.GetTransformScript(action);
						if (!string.IsNullOrEmpty(transformScript))
						{
							this._runspace.SessionStateProxy.SetVariable("DataObject", dataObject);
							this._runspace.SessionStateProxy.SetVariable("Action", action);
							this._runspace.SessionStateProxy.SetVariable("Sources", sources);
							this._runspace.SessionStateProxy.SetVariable("Targets", targets);
							this._runspace.SessionStateProxy.SetVariable("Transformer", this);
							using (Pipeline pipeline = this._runspace.CreatePipeline())
							{
								pipeline.Commands.AddScript(transformScript);
								pipeline.Invoke();
							}
							object variable = this._runspace.SessionStateProxy.GetVariable("DataObject");
							PSObject pSObject = variable as PSObject;
							if (pSObject != null)
							{
								variable = pSObject.BaseObject;
							}
							if (variable == null || typeof(T).IsAssignableFrom(variable.GetType()))
							{
								t1 = (T)variable;
							}
							else
							{
								if (base.Options.TransformScriptLocation == ScriptFileLocation.File)
								{
									transformScriptFileName = base.Options.TransformScriptFileName;
								}
								else
								{
									transformScriptFileName = null;
								}
								LogItem logItem = new LogItem("Invoke Transform Script", transformScriptFileName, dataObject.ToString(), null, ActionOperationStatus.Failed)
								{
									Information = string.Format("DataObject is not of the correct type {0}    Expected: {1}{0}    Result: {2}", Environment.NewLine, typeof(T).Name, variable.GetType().Name),
									SourceContent = transformScript
								};
								base.FireOperationStarted(logItem);
								base.FireOperationFinished(logItem);
								t1 = default(T);
							}
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						if (base.Options.TransformScriptLocation == ScriptFileLocation.File)
						{
							str = base.Options.TransformScriptFileName;
						}
						else
						{
							str = null;
						}
						LogItem logItem1 = new LogItem("Invoke Transform Script", str, dataObject.ToString(), null, ActionOperationStatus.Failed)
						{
							Exception = exception,
							SourceContent = transformScript
						};
						base.FireOperationStarted(logItem1);
						base.FireOperationFinished(logItem1);
						t1 = default(T);
					}
				}
				finally
				{
					if (flag)
					{
						this._runspace.Close();
						this._runspace.Dispose();
						this._runspace = null;
					}
				}
				t = t1;
			}
			return t;
		}
	}
}