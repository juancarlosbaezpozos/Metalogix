using Metalogix.Licensing;
using Metalogix.Telemetry;
using Metalogix.Utilities;
using PreEmptive.SoS.Client.Messages;
using PreEmptive.SoS.Metalogix.SharePoint.Telemetry;
using PreEmptive.SoS.Runtime;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalogix.SharePoint.Telemetry
{
	public class Telemetry
	{
		protected static string _instanceId;

		public static string InstanceId
		{
			get
			{
				string empty = string.Empty;
				string str = string.Empty;
				try
				{
					if (string.IsNullOrEmpty(Metalogix.SharePoint.Telemetry.Telemetry._instanceId))
					{
						MLLicense license = MLLicenseProvider.Instance.GetLicense(null, null, null, false) as MLLicense;
						if (license != null)
						{
							empty = license.LicenseKey;
							str = (string.IsNullOrEmpty(license.Organization) ? string.Empty : license.Organization.ToString());
						}
						if (!string.IsNullOrEmpty(empty))
						{
							object[] objArray = new object[] { Guid.Empty, empty, Environment.MachineName, SystemUtils.EncodeTo64(str) };
							Metalogix.SharePoint.Telemetry.Telemetry._instanceId = string.Format("~1~{0}|{1}|{2}|{3}", objArray);
						}
						else
						{
							return string.Empty;
						}
					}
				}
				catch (Exception exception)
				{
					Trace.WriteLine(string.Concat("Telemetry Instance Id : ", exception.ToString()));
				}
				return Metalogix.SharePoint.Telemetry.Telemetry._instanceId;
			}
		}

		static Telemetry()
		{
			Metalogix.SharePoint.Telemetry.Telemetry._instanceId = string.Empty;
		}

		public Telemetry()
		{
		}

		public static void Initialize()
		{
			try
			{
				Access.BusinessInfo = Access.CreateBusinessInformation("3aa12aaa-510d-4047-a19b-4364abbb1767", "Metalogix - Content Matrix");
				Access.ApplicationInfo = Access.CreateApplicationInformation("613d61e3-1eed-465a-a3e0-2b061627ad03", "Content Matrix Console - SharePoint Edition", "8.3.0.3", "Full Release");
				Access.Setup(Metalogix.SharePoint.Telemetry.Telemetry.InstanceId, Client.OptIn, false, BinaryInfo.Get());
				Access.Send(new SessionLifeCycleMessage()
				{
					Event = new EventInformation()
					{
						Code = "Session.Start"
					},
					Binary = BinaryInfo.Get()
				});
				MLLicenseProvider.InitializeTelemetry();
			}
			catch (Exception exception)
			{
				Trace.WriteLine(string.Concat("Telemetry Initialize : ", exception.ToString()));
			}
		}

		public static void TearDown()
		{
			try
			{
				try
				{
					MLLicenseProvider.TearDownTelemetry();
				}
				catch (Exception exception)
				{
					Trace.WriteLine(string.Concat("Telemetry TearDown : ", exception.ToString()));
				}
			}
			finally
			{
				Access.Send(new SessionLifeCycleMessage()
				{
					Event = new EventInformation()
					{
						Code = "Session.Stop"
					},
					Binary = BinaryInfo.Get()
				});
				Access.Teardown();
			}
		}
	}
}