using Metalogix.DataStructures.Generic;
using Metalogix.Permissions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Metalogix.SharePoint
{
	public class SPRole2007 : SPRole
	{
		private static string[] s_listRights;

		private static string[] s_webRights;

		public override string[] AvailableRights
		{
			get
			{
				return Enum.GetNames(typeof(SPRights2007));
			}
		}

		public string[] ListRights
		{
			get
			{
				return SPRole2007.s_listRights;
			}
		}

		public long PermissionMask
		{
			get
			{
				long num = long.Parse(this.m_XML.Attributes["PermMask"].Value);
				return num;
			}
		}

		public string[] WebRights
		{
			get
			{
				return SPRole2007.s_webRights;
			}
		}

		static SPRole2007()
		{
			string[] strArrays = new string[] { "ViewListItems", "AddListItems", "EditListItems", "DeleteListItems", "ApproveItems", "OpenItems", "ViewVersions", "DeleteVersions", "CancelCheckout", "ManageLists", "Open", "ViewPages", "ManagePermissions", "EnumeratePermissions", "BrowseUserInfo", "BrowseDirectories" };
			SPRole2007.s_listRights = strArrays;
			strArrays = new string[] { "ManagePersonalViews", "ViewFormPages", "AddAndCustomizePages", "ApplyThemeAndBorder", "ApplyStyleSheets", "ViewUsageData", "CreateSSCSite", "ManageSubwebs", "CreateGroups", "AddDelPrivateWebParts", "UpdatePersonalWebParts", "ManageWeb", "UseClientIntegration", "UseRemoteAPIs", "ManageAlerts", "CreateAlerts", "EditMyUserInfo" };
			SPRole2007.s_webRights = strArrays;
		}

		public SPRole2007(XmlNode xml) : base(xml)
		{
		}

		public static void AddRightsDependencies(SPRights2007 right, Set<SPRights2007> dependencies)
		{
			SPRights2007[] sPRights2007Array;
			SPRights2007 sPRights2007;
			SPRights2007[] sPRights2007Array1;
			SPRights2007[] sPRights2007Array2;
			int i;
			SPRights2007 sPRights20071 = right;
			if (sPRights20071 <= SPRights2007.ApplyStyleSheets)
			{
				if (sPRights20071 <= SPRights2007.CancelCheckout)
				{
					if (sPRights20071 <= SPRights2007.ApproveItems)
					{
						if (sPRights20071 <= SPRights2007.EditListItems)
						{
							if (sPRights20071 >= SPRights2007.ViewListItems)
							{
								switch ((int)((long)sPRights20071 - (long)SPRights2007.ViewListItems))
								{
									case 0:
									{
										sPRights2007Array1 = new SPRights2007[] { SPRights2007.ViewPages };
										sPRights2007Array = sPRights2007Array1;
										sPRights2007Array2 = sPRights2007Array;
										for (i = 0; i < (int)sPRights2007Array2.Length; i++)
										{
											sPRights2007 = sPRights2007Array2[i];
											if (!dependencies.Contains(sPRights2007))
											{
												dependencies.Add(sPRights2007);
											}
											SPRole2007.AddRightsDependencies(sPRights2007, dependencies);
										}
										return;
									}
									case 1:
									{
										sPRights2007Array1 = new SPRights2007[] { SPRights2007.ViewListItems };
										sPRights2007Array = sPRights2007Array1;
										sPRights2007Array2 = sPRights2007Array;
										for (i = 0; i < (int)sPRights2007Array2.Length; i++)
										{
											sPRights2007 = sPRights2007Array2[i];
											if (!dependencies.Contains(sPRights2007))
											{
												dependencies.Add(sPRights2007);
											}
											SPRole2007.AddRightsDependencies(sPRights2007, dependencies);
										}
										return;
									}
									case 2:
									{
										sPRights2007Array = new SPRights2007[0];
										sPRights2007Array2 = sPRights2007Array;
										for (i = 0; i < (int)sPRights2007Array2.Length; i++)
										{
											sPRights2007 = sPRights2007Array2[i];
											if (!dependencies.Contains(sPRights2007))
											{
												dependencies.Add(sPRights2007);
											}
											SPRole2007.AddRightsDependencies(sPRights2007, dependencies);
										}
										return;
									}
									case 3:
									{
										sPRights2007Array1 = new SPRights2007[] { SPRights2007.ViewListItems };
										sPRights2007Array = sPRights2007Array1;
										sPRights2007Array2 = sPRights2007Array;
										for (i = 0; i < (int)sPRights2007Array2.Length; i++)
										{
											sPRights2007 = sPRights2007Array2[i];
											if (!dependencies.Contains(sPRights2007))
											{
												dependencies.Add(sPRights2007);
											}
											SPRole2007.AddRightsDependencies(sPRights2007, dependencies);
										}
										return;
									}
								}
							}
							else
							{
								sPRights2007Array = new SPRights2007[0];
								sPRights2007Array2 = sPRights2007Array;
								for (i = 0; i < (int)sPRights2007Array2.Length; i++)
								{
									sPRights2007 = sPRights2007Array2[i];
									if (!dependencies.Contains(sPRights2007))
									{
										dependencies.Add(sPRights2007);
									}
									SPRole2007.AddRightsDependencies(sPRights2007, dependencies);
								}
								return;
							}
						}
						if (sPRights20071 == SPRights2007.DeleteListItems)
						{
							sPRights2007Array1 = new SPRights2007[] { SPRights2007.ViewListItems };
							sPRights2007Array = sPRights2007Array1;
						}
						else
						{
							if (sPRights20071 != SPRights2007.ApproveItems)
							{
								sPRights2007Array = new SPRights2007[0];
								sPRights2007Array2 = sPRights2007Array;
								for (i = 0; i < (int)sPRights2007Array2.Length; i++)
								{
									sPRights2007 = sPRights2007Array2[i];
									if (!dependencies.Contains(sPRights2007))
									{
										dependencies.Add(sPRights2007);
									}
									SPRole2007.AddRightsDependencies(sPRights2007, dependencies);
								}
								return;
							}
							sPRights2007Array1 = new SPRights2007[] { SPRights2007.EditListItems };
							sPRights2007Array = sPRights2007Array1;
						}
					}
					else if (sPRights20071 <= SPRights2007.ViewVersions)
					{
						if (sPRights20071 == SPRights2007.OpenItems)
						{
							sPRights2007Array1 = new SPRights2007[] { SPRights2007.ViewListItems };
							sPRights2007Array = sPRights2007Array1;
						}
						else
						{
							if (sPRights20071 != SPRights2007.ViewVersions)
							{
								sPRights2007Array = new SPRights2007[0];
								sPRights2007Array2 = sPRights2007Array;
								for (i = 0; i < (int)sPRights2007Array2.Length; i++)
								{
									sPRights2007 = sPRights2007Array2[i];
									if (!dependencies.Contains(sPRights2007))
									{
										dependencies.Add(sPRights2007);
									}
									SPRole2007.AddRightsDependencies(sPRights2007, dependencies);
								}
								return;
							}
							sPRights2007Array1 = new SPRights2007[] { SPRights2007.OpenItems };
							sPRights2007Array = sPRights2007Array1;
						}
					}
					else if (sPRights20071 == SPRights2007.DeleteVersions)
					{
						sPRights2007Array1 = new SPRights2007[] { SPRights2007.ViewVersions };
						sPRights2007Array = sPRights2007Array1;
					}
					else
					{
						if (sPRights20071 != SPRights2007.CancelCheckout)
						{
							sPRights2007Array = new SPRights2007[0];
							sPRights2007Array2 = sPRights2007Array;
							for (i = 0; i < (int)sPRights2007Array2.Length; i++)
							{
								sPRights2007 = sPRights2007Array2[i];
								if (!dependencies.Contains(sPRights2007))
								{
									dependencies.Add(sPRights2007);
								}
								SPRole2007.AddRightsDependencies(sPRights2007, dependencies);
							}
							return;
						}
						sPRights2007Array1 = new SPRights2007[] { SPRights2007.ViewListItems };
						sPRights2007Array = sPRights2007Array1;
					}
				}
				else if (sPRights20071 <= SPRights2007.Open)
				{
					if (sPRights20071 <= SPRights2007.ManageLists)
					{
						if (sPRights20071 == SPRights2007.ManagePersonalViews)
						{
							sPRights2007Array1 = new SPRights2007[] { SPRights2007.ViewListItems };
							sPRights2007Array = sPRights2007Array1;
						}
						else
						{
							if (sPRights20071 != SPRights2007.ManageLists)
							{
								sPRights2007Array = new SPRights2007[0];
								sPRights2007Array2 = sPRights2007Array;
								for (i = 0; i < (int)sPRights2007Array2.Length; i++)
								{
									sPRights2007 = sPRights2007Array2[i];
									if (!dependencies.Contains(sPRights2007))
									{
										dependencies.Add(sPRights2007);
									}
									SPRole2007.AddRightsDependencies(sPRights2007, dependencies);
								}
								return;
							}
							sPRights2007Array1 = new SPRights2007[] { SPRights2007.ManagePersonalViews };
							sPRights2007Array = sPRights2007Array1;
						}
					}
					else if (sPRights20071 == SPRights2007.ViewFormPages)
					{
						sPRights2007Array1 = new SPRights2007[] { SPRights2007.Open };
						sPRights2007Array = sPRights2007Array1;
					}
					else
					{
						if (sPRights20071 != SPRights2007.Open)
						{
							sPRights2007Array = new SPRights2007[0];
							sPRights2007Array2 = sPRights2007Array;
							for (i = 0; i < (int)sPRights2007Array2.Length; i++)
							{
								sPRights2007 = sPRights2007Array2[i];
								if (!dependencies.Contains(sPRights2007))
								{
									dependencies.Add(sPRights2007);
								}
								SPRole2007.AddRightsDependencies(sPRights2007, dependencies);
							}
							return;
						}
						sPRights2007Array = new SPRights2007[0];
					}
				}
				else if (sPRights20071 <= SPRights2007.AddAndCustomizePages)
				{
					if (sPRights20071 == SPRights2007.ViewPages)
					{
						sPRights2007Array1 = new SPRights2007[] { SPRights2007.Open };
						sPRights2007Array = sPRights2007Array1;
					}
					else
					{
						if (sPRights20071 != SPRights2007.AddAndCustomizePages)
						{
							sPRights2007Array = new SPRights2007[0];
							sPRights2007Array2 = sPRights2007Array;
							for (i = 0; i < (int)sPRights2007Array2.Length; i++)
							{
								sPRights2007 = sPRights2007Array2[i];
								if (!dependencies.Contains(sPRights2007))
								{
									dependencies.Add(sPRights2007);
								}
								SPRole2007.AddRightsDependencies(sPRights2007, dependencies);
							}
							return;
						}
						sPRights2007Array1 = new SPRights2007[] { SPRights2007.ViewListItems, SPRights2007.BrowseDirectories };
						sPRights2007Array = sPRights2007Array1;
					}
				}
				else if (sPRights20071 == SPRights2007.ApplyThemeAndBorder)
				{
					sPRights2007Array1 = new SPRights2007[] { SPRights2007.ViewPages };
					sPRights2007Array = sPRights2007Array1;
				}
				else
				{
					if (sPRights20071 != SPRights2007.ApplyStyleSheets)
					{
						sPRights2007Array = new SPRights2007[0];
						sPRights2007Array2 = sPRights2007Array;
						for (i = 0; i < (int)sPRights2007Array2.Length; i++)
						{
							sPRights2007 = sPRights2007Array2[i];
							if (!dependencies.Contains(sPRights2007))
							{
								dependencies.Add(sPRights2007);
							}
							SPRole2007.AddRightsDependencies(sPRights2007, dependencies);
						}
						return;
					}
					sPRights2007Array1 = new SPRights2007[] { SPRights2007.ViewPages };
					sPRights2007Array = sPRights2007Array1;
				}
			}
			else if (sPRights20071 <= SPRights2007.AddDelPrivateWebParts)
			{
				if (sPRights20071 <= SPRights2007.CreateGroups)
				{
					if (sPRights20071 <= SPRights2007.CreateSSCSite)
					{
						if (sPRights20071 == SPRights2007.ViewUsageData)
						{
							sPRights2007Array1 = new SPRights2007[] { SPRights2007.ViewPages };
							sPRights2007Array = sPRights2007Array1;
						}
						else
						{
							if (sPRights20071 != SPRights2007.CreateSSCSite)
							{
								sPRights2007Array = new SPRights2007[0];
								sPRights2007Array2 = sPRights2007Array;
								for (i = 0; i < (int)sPRights2007Array2.Length; i++)
								{
									sPRights2007 = sPRights2007Array2[i];
									if (!dependencies.Contains(sPRights2007))
									{
										dependencies.Add(sPRights2007);
									}
									SPRole2007.AddRightsDependencies(sPRights2007, dependencies);
								}
								return;
							}
							sPRights2007Array = new SPRights2007[0];
						}
					}
					else if (sPRights20071 == SPRights2007.ManageSubwebs)
					{
						sPRights2007Array1 = new SPRights2007[] { SPRights2007.BrowseUserInfo };
						sPRights2007Array = sPRights2007Array1;
					}
					else
					{
						if (sPRights20071 != SPRights2007.CreateGroups)
						{
							sPRights2007Array = new SPRights2007[0];
							sPRights2007Array2 = sPRights2007Array;
							for (i = 0; i < (int)sPRights2007Array2.Length; i++)
							{
								sPRights2007 = sPRights2007Array2[i];
								if (!dependencies.Contains(sPRights2007))
								{
									dependencies.Add(sPRights2007);
								}
								SPRole2007.AddRightsDependencies(sPRights2007, dependencies);
							}
							return;
						}
						sPRights2007Array1 = new SPRights2007[] { SPRights2007.BrowseUserInfo };
						sPRights2007Array = sPRights2007Array1;
					}
				}
				else if (sPRights20071 <= SPRights2007.BrowseDirectories)
				{
					if (sPRights20071 == SPRights2007.ManagePermissions)
					{
						sPRights2007Array1 = new SPRights2007[] { SPRights2007.EnumeratePermissions };
						sPRights2007Array = sPRights2007Array1;
					}
					else
					{
						if (sPRights20071 != SPRights2007.BrowseDirectories)
						{
							sPRights2007Array = new SPRights2007[0];
							sPRights2007Array2 = sPRights2007Array;
							for (i = 0; i < (int)sPRights2007Array2.Length; i++)
							{
								sPRights2007 = sPRights2007Array2[i];
								if (!dependencies.Contains(sPRights2007))
								{
									dependencies.Add(sPRights2007);
								}
								SPRole2007.AddRightsDependencies(sPRights2007, dependencies);
							}
							return;
						}
						sPRights2007Array1 = new SPRights2007[] { SPRights2007.ViewPages };
						sPRights2007Array = sPRights2007Array1;
					}
				}
				else if (sPRights20071 == SPRights2007.BrowseUserInfo)
				{
					sPRights2007Array1 = new SPRights2007[] { SPRights2007.Open };
					sPRights2007Array = sPRights2007Array1;
				}
				else
				{
					if (sPRights20071 != SPRights2007.AddDelPrivateWebParts)
					{
						sPRights2007Array = new SPRights2007[0];
						sPRights2007Array2 = sPRights2007Array;
						for (i = 0; i < (int)sPRights2007Array2.Length; i++)
						{
							sPRights2007 = sPRights2007Array2[i];
							if (!dependencies.Contains(sPRights2007))
							{
								dependencies.Add(sPRights2007);
							}
							SPRole2007.AddRightsDependencies(sPRights2007, dependencies);
						}
						return;
					}
					sPRights2007Array1 = new SPRights2007[] { SPRights2007.UpdatePersonalWebParts };
					sPRights2007Array = sPRights2007Array1;
				}
			}
			else if (sPRights20071 <= SPRights2007.UseRemoteAPIs)
			{
				if (sPRights20071 <= SPRights2007.ManageWeb)
				{
					if (sPRights20071 == SPRights2007.UpdatePersonalWebParts)
					{
						sPRights2007Array1 = new SPRights2007[] { SPRights2007.ViewListItems };
						sPRights2007Array = sPRights2007Array1;
					}
					else
					{
						if (sPRights20071 != SPRights2007.ManageWeb)
						{
							sPRights2007Array = new SPRights2007[0];
							sPRights2007Array2 = sPRights2007Array;
							for (i = 0; i < (int)sPRights2007Array2.Length; i++)
							{
								sPRights2007 = sPRights2007Array2[i];
								if (!dependencies.Contains(sPRights2007))
								{
									dependencies.Add(sPRights2007);
								}
								SPRole2007.AddRightsDependencies(sPRights2007, dependencies);
							}
							return;
						}
						sPRights2007Array1 = new SPRights2007[] { SPRights2007.EnumeratePermissions, SPRights2007.AddAndCustomizePages };
						sPRights2007Array = sPRights2007Array1;
					}
				}
				else if (sPRights20071 == SPRights2007.UseClientIntegration)
				{
					sPRights2007Array1 = new SPRights2007[] { SPRights2007.UseRemoteAPIs };
					sPRights2007Array = sPRights2007Array1;
				}
				else
				{
					if (sPRights20071 != SPRights2007.UseRemoteAPIs)
					{
						sPRights2007Array = new SPRights2007[0];
						sPRights2007Array2 = sPRights2007Array;
						for (i = 0; i < (int)sPRights2007Array2.Length; i++)
						{
							sPRights2007 = sPRights2007Array2[i];
							if (!dependencies.Contains(sPRights2007))
							{
								dependencies.Add(sPRights2007);
							}
							SPRole2007.AddRightsDependencies(sPRights2007, dependencies);
						}
						return;
					}
					sPRights2007Array1 = new SPRights2007[] { SPRights2007.Open };
					sPRights2007Array = sPRights2007Array1;
				}
			}
			else if (sPRights20071 <= SPRights2007.CreateAlerts)
			{
				if (sPRights20071 == SPRights2007.ManageAlerts)
				{
					sPRights2007Array1 = new SPRights2007[] { SPRights2007.CreateAlerts };
					sPRights2007Array = sPRights2007Array1;
				}
				else
				{
					if (sPRights20071 != SPRights2007.CreateAlerts)
					{
						sPRights2007Array = new SPRights2007[0];
						sPRights2007Array2 = sPRights2007Array;
						for (i = 0; i < (int)sPRights2007Array2.Length; i++)
						{
							sPRights2007 = sPRights2007Array2[i];
							if (!dependencies.Contains(sPRights2007))
							{
								dependencies.Add(sPRights2007);
							}
							SPRole2007.AddRightsDependencies(sPRights2007, dependencies);
						}
						return;
					}
					sPRights2007Array1 = new SPRights2007[] { SPRights2007.ViewListItems };
					sPRights2007Array = sPRights2007Array1;
				}
			}
			else if (sPRights20071 == SPRights2007.EditMyUserInfo)
			{
				sPRights2007Array1 = new SPRights2007[] { SPRights2007.BrowseUserInfo };
				sPRights2007Array = sPRights2007Array1;
			}
			else
			{
				if (sPRights20071 != SPRights2007.EnumeratePermissions)
				{
					sPRights2007Array = new SPRights2007[0];
					sPRights2007Array2 = sPRights2007Array;
					for (i = 0; i < (int)sPRights2007Array2.Length; i++)
					{
						sPRights2007 = sPRights2007Array2[i];
						if (!dependencies.Contains(sPRights2007))
						{
							dependencies.Add(sPRights2007);
						}
						SPRole2007.AddRightsDependencies(sPRights2007, dependencies);
					}
					return;
				}
				sPRights2007Array1 = new SPRights2007[] { SPRights2007.ViewVersions, SPRights2007.BrowseDirectories, SPRights2007.BrowseUserInfo };
				sPRights2007Array = sPRights2007Array1;
			}
			sPRights2007Array2 = sPRights2007Array;
			for (i = 0; i < (int)sPRights2007Array2.Length; i++)
			{
				sPRights2007 = sPRights2007Array2[i];
				if (!dependencies.Contains(sPRights2007))
				{
					dependencies.Add(sPRights2007);
				}
				SPRole2007.AddRightsDependencies(sPRights2007, dependencies);
			}
		}

		private static SPRights2007[] DecodeMask(long mask)
		{
			long[] numArray = new long[35];
			int num = 0;
			ArrayList arrayLists = new ArrayList();
			foreach (long value in Enum.GetValues(typeof(SPRights2007)))
			{
				numArray[num] = value;
				num++;
			}
			for (int i = 34; i > 0; i--)
			{
				if ((mask < numArray[i] ? false : numArray[i] >= (long)0))
				{
					mask -= numArray[i];
					arrayLists.Add((SPRights2007)numArray[i]);
				}
			}
			SPRights2007[] item = new SPRights2007[arrayLists.Count];
			for (int j = 0; j < arrayLists.Count; j++)
			{
				item[j] = (SPRights2007)arrayLists[j];
			}
			return item;
		}

		public static SPRights2007[] GetRightsDependencies(SPRights2007 right)
		{
			Set<SPRights2007> set = new Set<SPRights2007>();
			SPRole2007.AddRightsDependencies(right, set);
			SPRights2007[] sPRights2007Array = new SPRights2007[set.Count];
			int num = 0;
			foreach (SPRights2007 sPRights2007 in set)
			{
				sPRights2007Array[num] = sPRights2007;
				num++;
			}
			return sPRights2007Array;
		}

		protected override string[] GetRightsDescription()
		{
			long num = long.Parse(this.m_XML.Attributes["PermMask"].Value);
			SPRights2007[] values = SPRole2007.DecodeMask(num);
			int length = (int)values.Length;
			if (((int)values.Length != 1 ? false : values[0] == SPRights2007.FullMask))
			{
				values = (SPRights2007[])Enum.GetValues(typeof(SPRights2007));
				length = (int)values.Length - 2;
			}
			string[] str = new string[length];
			int num1 = 0;
			SPRights2007[] sPRights2007Array = values;
			for (int i = 0; i < (int)sPRights2007Array.Length; i++)
			{
				SPRights2007 sPRights2007 = sPRights2007Array[i];
				if ((sPRights2007.ToString() == "FullMask" ? false : sPRights2007.ToString() != "EmptyMask"))
				{
					str[num1] = sPRights2007.ToString();
					num1++;
				}
			}
			return str;
		}
	}
}