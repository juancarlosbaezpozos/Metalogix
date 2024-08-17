using Metalogix;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Metalogix.SharePoint.Migration
{
	public static class MigrationResourceManager
	{
		private const int BUFFER_SIZE = 1048576;

		private const string RESOURCE_ASSEMBLY_FOLDER = "Metalogix.SharePoint.Migration.Resources.";

		private static Dictionary<string, byte[]> s_currentlyCachedResources;

		public static string PortalListingListKey
		{
			get
			{
				return "PortalListingList.xml";
			}
		}

		static MigrationResourceManager()
		{
			MigrationResourceManager.s_currentlyCachedResources = new Dictionary<string, byte[]>();
		}

		public static void CacheResource(string sResource)
		{
			byte[] resourceFromFile = MigrationResourceManager.GetResourceFromFile(sResource);
			if (!MigrationResourceManager.s_currentlyCachedResources.ContainsKey(sResource))
			{
				MigrationResourceManager.s_currentlyCachedResources.Add(sResource, resourceFromFile);
			}
			else
			{
				MigrationResourceManager.s_currentlyCachedResources[sResource] = resourceFromFile;
			}
		}

		private static void CopyResourceToFile(FileInfo file, string sResourcePath)
		{
			Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(sResourcePath);
			try
			{
				if (manifestResourceStream == null)
				{
					throw new Exception(string.Concat("The resource: '", sResourcePath, "' did not exist within the assembly: ", Assembly.GetExecutingAssembly().Location));
				}
				FileStream fileStream = file.Create();
				try
				{
					byte[] numArray = new byte[1048576];
					for (int i = manifestResourceStream.Read(numArray, 0, 1048576); i > 0; i = manifestResourceStream.Read(numArray, 0, 1048576))
					{
						fileStream.Write(numArray, 0, i);
						fileStream.Flush();
					}
				}
				finally
				{
					if (fileStream != null)
					{
						((IDisposable)fileStream).Dispose();
					}
				}
			}
			finally
			{
				if (manifestResourceStream != null)
				{
					((IDisposable)manifestResourceStream).Dispose();
				}
			}
		}

		public static void EnsureAllOnFileSystem()
		{
			string[] manifestResourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
			for (int i = 0; i < (int)manifestResourceNames.Length; i++)
			{
				string str = manifestResourceNames[i];
				if (str.StartsWith("Metalogix.SharePoint.Migration.Resources.", StringComparison.OrdinalIgnoreCase))
				{
					MigrationResourceManager.EnsureResourceIsOnFileSystem(str.Remove(0, "Metalogix.SharePoint.Migration.Resources.".Length));
				}
			}
		}

		private static string EnsureResourceIsOnFileSystem(string sResource)
		{
			string str = string.Concat(ApplicationData.ApplicationPath, "/Resources/", sResource);
			FileInfo fileInfo = new FileInfo(str);
			if (!fileInfo.Exists)
			{
				DirectoryInfo directory = fileInfo.Directory;
				if (!directory.Exists)
				{
					directory.Create();
				}
				MigrationResourceManager.CopyResourceToFile(fileInfo, string.Concat("Metalogix.SharePoint.Migration.Resources.", sResource));
			}
			return str;
		}

		public static byte[] GetResource(string sResource)
		{
			byte[] numArray;
			byte[] numArray1;
			numArray1 = (!MigrationResourceManager.s_currentlyCachedResources.TryGetValue(sResource, out numArray) ? MigrationResourceManager.GetResourceFromFile(sResource) : numArray);
			return numArray1;
		}

	    private static byte[] GetResourceFromFile(string sResource)
	    {
	        string path = MigrationResourceManager.EnsureResourceIsOnFileSystem(sResource);
	        byte[] result;
	        using (FileStream fileStream = new FileStream(path, FileMode.Open))
	        {
	            byte[] array = new byte[fileStream.Length];
	            fileStream.Read(array, 0, array.Length);
	            result = array;
	        }
	        return result;
	    }

        public static void ReleaseAllResources()
		{
			MigrationResourceManager.s_currentlyCachedResources.Clear();
		}

		public static void ReleaseResource(string sResource)
		{
			MigrationResourceManager.s_currentlyCachedResources.Remove(sResource);
		}
	}
}