using Metalogix.Transformers;
using System;

namespace Metalogix.Commands
{
	public class PowerShellTransformerOptions : TransformerOptions
	{
		private ScriptFileLocation _beginFileLocation;

		private ScriptFileLocation _transformFileLocation;

		private ScriptFileLocation _endFileLocation;

		private string _beginScriptFileName;

		private string _transformScriptFileName;

		private string _endScriptFileName;

		private string _fullBeginScript;

		private string _fullTransformScript;

		private string _fullEndScript;

		public ScriptFileLocation BeginScriptLocation
		{
			get
			{
				return this._beginFileLocation;
			}
			set
			{
				this._beginFileLocation = value;
			}
		}

		public string BeginTranformScriptFileName
		{
			get
			{
				return this._beginScriptFileName;
			}
			set
			{
				this._beginScriptFileName = value;
			}
		}

		public ScriptFileLocation EndScriptLocation
		{
			get
			{
				return this._endFileLocation;
			}
			set
			{
				this._endFileLocation = value;
			}
		}

		public string EndTransformScriptFileName
		{
			get
			{
				return this._endScriptFileName;
			}
			set
			{
				this._endScriptFileName = value;
			}
		}

		public string FullBeginTransformScript
		{
			get
			{
				return this._fullBeginScript;
			}
			set
			{
				this._fullBeginScript = value;
			}
		}

		public string FullEndTransformScript
		{
			get
			{
				return this._fullEndScript;
			}
			set
			{
				this._fullEndScript = value;
			}
		}

		public string FullTransformScript
		{
			get
			{
				return this._fullTransformScript;
			}
			set
			{
				this._fullTransformScript = value;
			}
		}

		public string TransformScriptFileName
		{
			get
			{
				return this._transformScriptFileName;
			}
			set
			{
				this._transformScriptFileName = value;
			}
		}

		public ScriptFileLocation TransformScriptLocation
		{
			get
			{
				return this._transformFileLocation;
			}
			set
			{
				this._transformFileLocation = value;
			}
		}

		public PowerShellTransformerOptions()
		{
		}
	}
}