using Metalogix;
using Metalogix.Client;
using Metalogix.DataResolution;
using Metalogix.Explorer;
using Metalogix.Licensing;
using Metalogix.Licensing.LicenseServer;
using Metalogix.Metabase.Attributes;
using Metalogix.ObjectResolution;
using Metalogix.SharePoint;
using Metalogix.SharePoint.UI.WinForms.ExternalConnections;
using Metalogix.SharePoint.UI.WinForms.Mapping;
using Metalogix.SharePoint.Utilities;
using Metalogix.SharePointEdition;
using Metalogix.UI.Standard;
using Metalogix.UI.WinForms;
using Metalogix.UI.WinForms.Attributes;
using Metalogix.UI.WinForms.Explorer;
using Metalogix.UI.WinForms.Metabase;
using Metalogix.UI.WinForms.Transformers;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

[assembly: ApplicationIcon("Metalogix.SharePointEdition.Images.ApplicationIcon.png")]
[assembly: ApplicationSetting(typeof(ManageNintexConnectionsSettings), 5)]
[assembly: ApplicationSetting(typeof(GlobalMappingsSetting), 3)]
[assembly: ApplicationSetting(typeof(MetabaseSettingsEditorSetting), 4)]
[assembly: ApplicationSetting(typeof(ShowAdvancedSetting), 1)]
[assembly: ApplicationSetting(typeof(ThreadSettingsEditorSetting), 2)]
[assembly: ApplicationSetting(typeof(EnableCustomTransformersSetting), 6)]
[assembly: AssemblyCompany("Metalogix International GmbH")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCopyright("Copyright © Metalogix International GmbH 2017")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyProduct("Migrador")]
[assembly: AssemblyTitle("Migrador")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyVersion("8.3.0.3")]
[assembly: CompilationRelaxations(8)]
[assembly: ComVisible(false)]
[assembly: DataUnitName("B")]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly:
    DefaultObjectResolver(typeof(ClientDataRepositoryResolver), typeof(DataResolver), typeof(DataRepositoryLink))]
[assembly: DefaultObjectResolver(typeof(PersistentConnectionsResolver), typeof(Node), typeof(Location))]
[assembly: DefaultObjectResolver(typeof(ResourceFileTableResolver), typeof(ResourceTable), typeof(ResourceTableLink))]
[assembly: FormIcon("Metalogix.SharePointEdition.Images.ContentMatrixIcon.ico")]
[assembly: FormType(typeof(StandardMainForm))]
[assembly: Guid("fb558806-bc5d-4b21-abd0-511d7b6bf616")]
[assembly: ItemDataConverter(true, typeof(ItemsViewLocalTimeDataConverter))]
[assembly: ItemsView(typeof(SPNode), ItemsViewType.Standard)]
[assembly: LegacyProduct(Product.MMS)]
[assembly: LicenseDataFormatterName("Metalogix.Format, Metalogix.Core")]
[assembly: LicenseFileName("ProductNew.lic")]
[assembly: LicenseFileStorage(LicenseStorageLocation.Common)]
[assembly: MultiSelect(true, typeof(MultiSelectLimiter))]
[assembly: OldStyleLicenseName("Product.lic")]
[assembly: Product(Product.CMCSharePoint)]
[assembly: ProductName("Content Matrix Console - SharePoint Edition")]
[assembly: ProviderType(LicenseProviderType.Common)]
[assembly: ProxyFilePath("LicenseServerProxySettings.dat", LicenseStorageLocation.Common, LicenseStorageType.File)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: SecondExplorer(true)]
[assembly: SecureStorage(true)]
[assembly: ShowMetabaseActionsInMenu(true)]
[assembly:
    SplashScreen("Metalogix.SharePointEdition.Images.ContentMatrix-SharePoint Edition.png",
        "Metalogix.SharePointEdition.Images.ContentMatrixExpress-SharePoint Edition.png")]
[assembly: UsageFileName("Usage.lic")]