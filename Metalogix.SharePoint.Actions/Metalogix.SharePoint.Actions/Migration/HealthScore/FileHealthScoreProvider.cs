using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Actions.Migration.HealthScore
{
	public class FileHealthScoreProvider : IHealthScoreProvider
	{
		private readonly string _healthScoreFile;

		private readonly object _lock = new object();

		private readonly FileSystemWatcher _watcher;

		private int _debugHealthScore;

		private volatile bool _isInitialized;

		public FileHealthScoreProvider(string serverHealthFile)
		{
			if (!File.Exists(serverHealthFile))
			{
				throw new FileNotFoundException("Server health file is not found.  This file should only contain the health score. ie. 8", serverHealthFile);
			}
			this._debugHealthScore = 5;
			this._healthScoreFile = serverHealthFile;
			this._watcher = new FileSystemWatcher(Path.GetDirectoryName(this._healthScoreFile), Path.GetFileName(this._healthScoreFile));
		}

		public int GetHealthScore()
		{
			lock (this._lock)
			{
				if (!this._isInitialized)
				{
					this.Initialize();
					this._isInitialized = true;
				}
			}
			return this._debugHealthScore;
		}

		private void Initialize()
		{
			this.InitializeHealthScoreFile();
			this.InitializeWatcher();
		}

		private void InitializeHealthScoreFile()
		{
			if (!File.Exists(this._healthScoreFile))
			{
				File.WriteAllText(this._healthScoreFile, this._debugHealthScore.ToString(CultureInfo.InvariantCulture));
				return;
			}
			int.TryParse(File.ReadAllText(this._healthScoreFile), out this._debugHealthScore);
		}

		private void InitializeWatcher()
		{
			this._watcher.Changed += new FileSystemEventHandler((object sender, FileSystemEventArgs e) => {
				try
				{
					if (e.ChangeType == WatcherChangeTypes.Changed)
					{
						int.TryParse(File.ReadAllText(e.FullPath), out this._debugHealthScore);
					}
				}
				catch (IOException oException)
				{
				}
			});
			this._watcher.EnableRaisingEvents = true;
		}
	}
}