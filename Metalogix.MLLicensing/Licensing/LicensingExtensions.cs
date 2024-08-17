using Metalogix.Licensing.LicenseServer;
using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Licensing
{
    public static class LicensingExtensions
    {
        public static ProductFlags GetFlagsFromProduct(this Product product)
        {
            Product modernProduct = product;
            if (product.IsLegacyProduct())
            {
                modernProduct = product.GetModernProduct();
            }

            string name = Enum.GetName(typeof(Product), modernProduct);
            ProductFlags productFlag = ProductFlags.Unknown;
            try
            {
                productFlag = (ProductFlags)Enum.Parse(typeof(ProductFlags), name);
            }
            catch
            {
                productFlag = ProductFlags.Unknown;
            }

            return productFlag;
        }

        public static Product GetLegacyProduct(this Product product)
        {
            switch (product)
            {
                case Product.CMCPublicFolder:
                    return Product.MMEPF;
                case (Product)666:
                case Product.CMEmail:
                case (Product)668:
                case (Product)669:
                case (Product)670:
                case (Product)671:
                case (Product)677:
                    break;
                case Product.CMCSharePoint:
                    return Product.MMS;
                case Product.CMCFileShare:
                    return Product.MMSFiles;
                case Product.CMCWebsite:
                    return Product.MMSWebsite;
                case Product.CMCeRoom:
                    return Product.MMSeRoom;
                case Product.CMCOracleAndStellent:
                    return Product.MMSOCS;
                case Product.CMCBlogsAndWikis:
                    return Product.MMSBlogsWikis;
                case Product.CMCGoogle:
                    return Product.MMSGoogleApps;
                default:
                    if (product == Product.CMCDocumentum)
                    {
                        return Product.MMSDocumentum;
                    }

                    if (product == Product.UnifiedContentMatrixKey)
                    {
                        return Product.EnterpriseMigrationKey;
                    }

                    break;
            }

            return product;
        }

        public static Product GetModernProduct(this Product product)
        {
            switch (product)
            {
                case Product.MMEPF:
                    return Product.CMCPublicFolder;
                case Product.BPOSML:
                case Product.MMEmail:
                case (Product)568:
                case (Product)569:
                case (Product)570:
                case (Product)571:
                case Product.MMSMCMS2002:
                    break;
                case Product.MMS:
                    return Product.CMCSharePoint;
                case Product.MMSFiles:
                    return Product.CMCFileShare;
                case Product.MMSWebsite:
                    return Product.CMCWebsite;
                case Product.MMSeRoom:
                    return Product.CMCeRoom;
                case Product.MMSOCS:
                    return Product.CMCOracleAndStellent;
                case Product.MMSBlogsWikis:
                    return Product.CMCBlogsAndWikis;
                case Product.MMSGoogleApps:
                    return Product.CMCGoogle;
                default:
                    if (product == Product.MMSDocumentum)
                    {
                        return Product.CMCDocumentum;
                    }

                    if (product == Product.EnterpriseMigrationKey)
                    {
                        return Product.UnifiedContentMatrixKey;
                    }

                    break;
            }

            return product;
        }


        public static bool IsContentMatrixExpress(this Product product)
        {
            return product == Product.UnifiedContentMatrixExpressKey;
        }

        public static bool IsLegacyProduct(this Product product)
        {
            if (product == Product.SRM || product == Product.MMEPF || product == Product.MMS ||
                product == Product.MMSBlogsWikis || product == Product.MMSDocumentum || product == Product.MMSFiles ||
                product == Product.MMSGoogleApps || product == Product.MMSMCMS2002 || product == Product.MMSOCS ||
                product == Product.MMSWebsite || product == Product.MMSeRoom)
            {
                return true;
            }

            return product == Product.EnterpriseMigrationKey;
        }
    }
}