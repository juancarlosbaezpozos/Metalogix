using System;

namespace KeyCreator
{
    internal class Program
    {
        private static void Main()
        {
            Generar();
        }

        public static void Generar()
        {
            var lc = new MLLicenseCA();
            lc.Name = "Carlos P";
            lc.Organization = "IBM";
            lc.Email = "jc@mx1.ibm.com";
            lc.Product = "Content Matrix Console - SharePoint Edition";
            lc.LicenseType = MLLicenseType.Commercial;
            lc.SerialNumber = Guid.NewGuid();
            lc.ExpiryDate = DateTime.Now.AddYears(1);
            lc.TotalPages = 1;
            lc.Signature = "firma";

            const string sProductName = "Migrador";
            var sLicenseFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Metalogix\" + sProductName + @"\Product.lic";
            lc.SaveToFile(sLicenseFilePath);
        }
    }
}
