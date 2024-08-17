namespace Metalogix.SharePointEdition
{
    public static class Telemetry
    {
        public static void Initialize()
        {
            SharePoint.Telemetry.Telemetry.Initialize();
        }

        public static void TearDown()
        {
            SharePoint.Telemetry.Telemetry.TearDown();
        }
    }
}