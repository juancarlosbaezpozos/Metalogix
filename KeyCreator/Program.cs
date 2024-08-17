using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyCreator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Generar();
        }

        public static void Generar()
        {
            MLLicenseCA lc = new MLLicenseCA();
            lc.Name = "Carlos P";
            lc.Organization = "IBM";
            lc.Email = "jc@mx1.ibm.com";
            lc.Product = "Content Matrix Console - SharePoint Edition";
            lc.LicenseType = MLLicenseType.Commercial;
            lc.SerialNumber = Guid.NewGuid();
            lc.ExpiryDate = DateTime.Now.AddYears(1);
            lc.TotalPages = 1;
            lc.Signature = "firma";

            string sProductName = "Migrador";
            string sLicenseFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Metalogix\" + sProductName + @"\Product.lic";
            lc.SaveToFile(sLicenseFilePath);
        }
    }
}
