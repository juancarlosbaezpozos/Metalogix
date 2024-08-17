using System;
using System.Collections.Generic;

namespace Metalogix.SharePoint.Options.Transform
{
	[Obsolete]
	public static class FieldToManagedMetadataRepository
	{
		private static volatile Dictionary<string, Dictionary<string, ManagedMetadataTargetField>> m_transforms;

		private readonly static object objLock;

		private readonly static object objEditLock;

		private static Dictionary<string, Dictionary<string, ManagedMetadataTargetField>> Transforms
		{
			get
			{
				if (FieldToManagedMetadataRepository.m_transforms == null)
				{
					lock (FieldToManagedMetadataRepository.objLock)
					{
						if (FieldToManagedMetadataRepository.m_transforms == null)
						{
							FieldToManagedMetadataRepository.m_transforms = new Dictionary<string, Dictionary<string, ManagedMetadataTargetField>>();
						}
					}
				}
				return FieldToManagedMetadataRepository.m_transforms;
			}
		}

		static FieldToManagedMetadataRepository()
		{
			FieldToManagedMetadataRepository.m_transforms = null;
			FieldToManagedMetadataRepository.objLock = new object();
			FieldToManagedMetadataRepository.objEditLock = new object();
		}

		public static void Add(string listGuid, string sourceFieldName, ManagedMetadataTargetField targetFieldMMD)
		{
			lock (FieldToManagedMetadataRepository.objEditLock)
			{
				if (!FieldToManagedMetadataRepository.Transforms.ContainsKey(listGuid))
				{
					FieldToManagedMetadataRepository.Transforms.Add(listGuid, new Dictionary<string, ManagedMetadataTargetField>());
				}
				if (!FieldToManagedMetadataRepository.Transforms[listGuid].ContainsKey(sourceFieldName))
				{
					FieldToManagedMetadataRepository.Transforms[listGuid].Add(sourceFieldName, targetFieldMMD);
				}
				else
				{
					FieldToManagedMetadataRepository.Transforms[listGuid][sourceFieldName] = targetFieldMMD;
				}
			}
		}

		public static void Clear(string listGuid)
		{
			lock (FieldToManagedMetadataRepository.objEditLock)
			{
				if (FieldToManagedMetadataRepository.Transforms.ContainsKey(listGuid))
				{
					FieldToManagedMetadataRepository.Transforms[listGuid].Clear();
					FieldToManagedMetadataRepository.Transforms[listGuid] = null;
					FieldToManagedMetadataRepository.Transforms.Remove(listGuid);
				}
			}
		}

		public static Dictionary<string, ManagedMetadataTargetField> GetTransformedFieldsForList(string listGuid)
		{
			Dictionary<string, ManagedMetadataTargetField> strs;
			Dictionary<string, ManagedMetadataTargetField> item;
			lock (FieldToManagedMetadataRepository.objEditLock)
			{
				if (FieldToManagedMetadataRepository.Transforms.ContainsKey(listGuid))
				{
					item = FieldToManagedMetadataRepository.Transforms[listGuid];
				}
				else
				{
					item = null;
				}
				strs = item;
			}
			return strs;
		}

		public static void Remove(string listGuid, string sourceFieldName)
		{
			lock (FieldToManagedMetadataRepository.objEditLock)
			{
				if (FieldToManagedMetadataRepository.Transforms.ContainsKey(listGuid) && FieldToManagedMetadataRepository.Transforms[listGuid].ContainsKey(sourceFieldName))
				{
					FieldToManagedMetadataRepository.Transforms[listGuid][sourceFieldName] = null;
					FieldToManagedMetadataRepository.Transforms[listGuid].Remove(sourceFieldName);
				}
			}
		}
	}
}