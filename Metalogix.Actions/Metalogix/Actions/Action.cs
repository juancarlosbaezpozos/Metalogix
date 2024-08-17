using Metalogix;
using Metalogix.Actions.Blocker;
using Metalogix.Actions.Properties;
using Metalogix.Core;
using Metalogix.Core.ObjectResolution;
using Metalogix.Data;
using Metalogix.Data.Filters;
using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using Metalogix.Explorer.Attributes;
using Metalogix.Interfaces;
using Metalogix.Jobs;
using Metalogix.Jobs.Actions;
using Metalogix.Licensing;
using Metalogix.Licensing.Common;
using Metalogix.ObjectResolution;
using Metalogix.Telemetry.MessageFormat;
using Metalogix.Threading;
using Metalogix.Transformers;
using Metalogix.Transformers.Attributes;
using Metalogix.Transformers.Interfaces;
using Metalogix.Utilities;
using Microsoft.Win32.SafeHandles;
using PreEmptive.SoS.Client.Messages;
using PreEmptive.SoS.Metalogix.Actions;
using PreEmptive.SoS.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml;

namespace Metalogix.Actions
{
    [LicensedProducts(ProductFlags.Unknown)]
    [System.ComponentModel.LicenseProvider(typeof(MLLicenseProvider))]
    public abstract class Action : IXmlable, IOperationLoggingManagement, IOperationLogging, IOperationState
    {
        protected const string RUN_ACTION_END_REACHED_KEY = "RunActionEndReached";

        private ActionDataCounter _dataCounter;

        private string m_sName;

        private string m_sImageName;

        private string m_sLargeImageName;

        private bool? m_bShowInMenus = null;

        private string m_sMenuText;

        private string m_sMenuTextPlural;

        private PluralCondition? m_menuTextPluralCondition = null;

        private List<string> m_lstUsageDataCompletions;

        private bool? m_bBatchable = null;

        private string m_sBatchableName;

        private bool? m_bAnalyzable = null;

        private bool? m_bAdvanced = null;

        private bool? m_bIncrementable = null;

        private string m_sIncrementalName;

        private bool? m_bRunAsync = null;

        private bool? m_bIsConnectivityAction = null;

        private bool? m_bRunSubActionAsync = null;

        private bool? m_bLaunchAsJob = null;

        private bool? m_bSupportsThreeStateConfiguration = null;

        private bool? m_bRequiresWriteAccess = null;

        private Type m_sourceType;

        private bool? m_bApplyToSourceSubTypes = null;

        private Cardinality? m_sourceCardinality = null;

        private Type m_targetType;

        private bool? m_bApplyToTargetSubTypes = null;

        private Cardinality? m_targetCardinality = null;

        private bool? m_bShowStatusDialog = null;

        private bool? m_bUsesStickySettings = null;

        private bool? m_bUseShortcutKeys = null;

        private ShortcutAction? m_shortcutKeys = null;

        private bool? m_bCmdletEnabled = null;

        private string m_sCmdletName;

        private string[] m_requiredSnapins;

        private bool? _isBasicModeAllowed = null;

        private Type m_type;

        private static string s_sTransformerInterfaceTypeName;

        private volatile List<ITransformerDefinition> m_SupportedDefinitions;

        private object m_oLockDefinitions = new object();

        private bool? _allowsSameSourceTarget = null;

        private ActionOptions m_options;

        protected Metalogix.Transformers.TransformationRepository _transformationRepository;

        private IList<IActionBlocker> _actionBlockers;

        private ActionStatus m_status;

        private object m_lockObjectThreadSync = new object();

        private Metalogix.Threading.ThreadManager m_threadManager;

        private System.ComponentModel.License m_license;

        private volatile bool m_bUpdateLicensing;

        private bool m_bLicensingUnitFetchedSuccessfully;

        private string m_sLicensingUnit = "Units";

        private ActionBlockerHandler m_handlerActionBlocked;

        private ActionEventHandler m_handlerOperationStarted;

        private ActionEventHandler m_handlerOperationUpdated;

        private ActionEventHandler m_handlerOperationFinished;

        private ActionLinkChangedHandler m_handlerOperationSourceLinkChanged;

        private ActionLinkChangedHandler m_handlerOperationTargetLinkChanged;

        private object m_oLockSubActionCollection = new object();

        protected SubActionCollection m_subActions;

        protected IXMLAbleList m_sources;

        protected XmlNode m_sourceXML;

        private bool m_hasConfiguration;

        private IDictionary<string, string> m_extendedTelemetryData;

        private LogItem _actionBlockedLogItem;

        public IList<IActionBlocker> ActionBlockers
        {
            get
            {
                if (this._actionBlockers == null)
                {
                    this._actionBlockers = new List<IActionBlocker>();
                }

                return this._actionBlockers;
            }
            set { this._actionBlockers = value; }
        }

        public Type ActionType
        {
            get
            {
                if (this.m_type == null)
                {
                    this.m_type = this.GetType();
                }

                return this.m_type;
            }
        }

        public bool AllowsSameSourceTarget
        {
            get
            {
                if (!this._allowsSameSourceTarget.HasValue)
                {
                    this._allowsSameSourceTarget =
                        Metalogix.Actions.AllowsSameSourceTarget.GetAllowSameSourceTarget(this.GetType());
                    if (!this._allowsSameSourceTarget.HasValue)
                    {
                        this._allowsSameSourceTarget = new bool?(true);
                    }
                }

                return this._allowsSameSourceTarget.Value;
            }
        }

        public bool Analyzable
        {
            get
            {
                if (!this.m_bAnalyzable.HasValue)
                {
                    object[] customAttributes = this.ActionType.GetCustomAttributes(typeof(AnalyzableAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_bAnalyzable = new bool?(false);
                    }
                    else
                    {
                        this.m_bAnalyzable = new bool?(((AnalyzableAttribute)customAttributes[0]).Analyzable);
                    }
                }

                return this.m_bAnalyzable.Value;
            }
        }

        public bool ApplyToSourceSubTypes
        {
            get
            {
                if (!this.m_bApplyToSourceSubTypes.HasValue)
                {
                    object[] customAttributes = this.ActionType.GetCustomAttributes(typeof(SourceTypeAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_bApplyToSourceSubTypes = new bool?(true);
                    }
                    else
                    {
                        this.m_sourceType = ((SourceTypeAttribute)customAttributes[0]).Type;
                        this.m_bApplyToSourceSubTypes =
                            new bool?(((SourceTypeAttribute)customAttributes[0]).ApplyToSubTypes);
                    }
                }

                return this.m_bApplyToSourceSubTypes.Value;
            }
        }

        public bool ApplyToTargetSubTypes
        {
            get
            {
                if (!this.m_bApplyToTargetSubTypes.HasValue)
                {
                    object[] customAttributes = this.ActionType.GetCustomAttributes(typeof(TargetTypeAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_bApplyToTargetSubTypes = new bool?(true);
                    }
                    else
                    {
                        this.m_targetType = ((TargetTypeAttribute)customAttributes[0]).Type;
                        this.m_bApplyToTargetSubTypes =
                            new bool?(((TargetTypeAttribute)customAttributes[0]).ApplyToSubTypes);
                    }
                }

                return this.m_bApplyToTargetSubTypes.Value;
            }
        }

        public bool Batchable
        {
            get
            {
                if (!this.m_bBatchable.HasValue)
                {
                    object[] customAttributes = this.ActionType.GetCustomAttributes(typeof(BatchableAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_bBatchable = new bool?(false);
                    }
                    else
                    {
                        this.m_bBatchable = new bool?(((BatchableAttribute)customAttributes[0]).Batchable);
                        this.m_sBatchableName = ((BatchableAttribute)customAttributes[0]).BatchableName;
                    }
                }

                return this.m_bBatchable.Value;
            }
        }

        public virtual string BatchableName
        {
            get { return this.m_sBatchableName; }
        }

        public bool CmdletEnabled
        {
            get
            {
                if (!this.m_bCmdletEnabled.HasValue)
                {
                    this.GetCmdletAttributes();
                }

                return this.m_bCmdletEnabled.Value;
            }
        }

        public string CmdletName
        {
            get
            {
                if (this.m_sCmdletName == null)
                {
                    this.GetCmdletAttributes();
                }

                return this.m_sCmdletName;
            }
        }

        public virtual string DisplayName
        {
            get { return this.Name; }
        }

        private bool EnableLicenseTracking
        {
            get { return true; }
        }

        public IDictionary<string, string> ExtendedTelemetryData
        {
            get { return this.m_extendedTelemetryData; }
            set { this.m_extendedTelemetryData = value; }
        }

        public virtual System.Drawing.Image Image
        {
            get
            {
                System.Drawing.Image image;
                try
                {
                    if (string.IsNullOrEmpty(this.ImageName))
                    {
                        image = null;
                    }
                    else
                    {
                        image = ImageCache.GetImage(this.ImageName, this.ActionType.Assembly);
                    }
                }
                catch (Exception exception)
                {
                    image = null;
                }

                return image;
            }
        }

        public virtual string ImageName
        {
            get
            {
                if (this.m_sImageName == null)
                {
                    object[] customAttributes = this.ActionType.GetCustomAttributes(typeof(ImageAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_sImageName = "";
                    }
                    else
                    {
                        this.m_sImageName = ((ImageAttribute)customAttributes[0]).ImageName;
                    }
                }

                return this.m_sImageName;
            }
        }

        public bool Incrementable
        {
            get
            {
                if (!this.m_bIncrementable.HasValue)
                {
                    object[] customAttributes =
                        this.ActionType.GetCustomAttributes(typeof(IncrementableAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_bIncrementable = new bool?(false);
                    }
                    else
                    {
                        this.m_bIncrementable = new bool?(((IncrementableAttribute)customAttributes[0]).Incrementable);
                        this.m_sIncrementalName = ((IncrementableAttribute)customAttributes[0]).IncrementalName;
                    }
                }

                return this.m_bIncrementable.Value;
            }
        }

        public virtual string IncrementalName
        {
            get
            {
                if (this.m_sIncrementalName == null)
                {
                    object[] customAttributes =
                        this.ActionType.GetCustomAttributes(typeof(IncrementableAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_bIncrementable = new bool?(false);
                    }
                    else
                    {
                        this.m_bIncrementable = new bool?(((IncrementableAttribute)customAttributes[0]).Incrementable);
                        this.m_sIncrementalName = ((IncrementableAttribute)customAttributes[0]).IncrementalName;
                    }
                }

                return this.m_sIncrementalName;
            }
        }

        public bool IsAdvanced
        {
            get
            {
                if (!this.m_bAdvanced.HasValue)
                {
                    object[] customAttributes = this.ActionType.GetCustomAttributes(typeof(IsAdvancedAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_bAdvanced = new bool?(false);
                    }
                    else
                    {
                        this.m_bAdvanced = new bool?(((IsAdvancedAttribute)customAttributes[0]).IsAdvanced);
                    }
                }

                return this.m_bAdvanced.Value;
            }
        }

        public bool IsBasicModeAllowed
        {
            get
            {
                if (!this._isBasicModeAllowed.HasValue)
                {
                    object[] customAttributes =
                        this.ActionType.GetCustomAttributes(typeof(BasicModeViewAllowedAttribute), true);
                    this._isBasicModeAllowed = new bool?(((int)customAttributes.Length != 1
                        ? false
                        : ((BasicModeViewAllowedAttribute)customAttributes[0]).IsBasicModeAllowed));
                }

                return this._isBasicModeAllowed.Value;
            }
        }

        public bool IsConnectivityAction
        {
            get
            {
                if (!this.m_bIsConnectivityAction.HasValue)
                {
                    object[] customAttributes =
                        this.ActionType.GetCustomAttributes(typeof(IsConnectivityActionAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_bIsConnectivityAction = new bool?(false);
                    }
                    else
                    {
                        this.m_bIsConnectivityAction =
                            new bool?(((IsConnectivityActionAttribute)customAttributes[0]).IsConnectivityAction);
                    }
                }

                return this.m_bIsConnectivityAction.Value;
            }
        }

        public bool IsOperationCancelled
        {
            get { return (this.Status == ActionStatus.Aborting ? true : this.Status == ActionStatus.Aborted); }
        }

        public bool IsRemoteJob { get; set; }

        public bool IsRunAsync
        {
            get
            {
                if (!this.m_bRunAsync.HasValue)
                {
                    object[] customAttributes = this.ActionType.GetCustomAttributes(typeof(RunAsyncAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_bRunAsync = new bool?(true);
                    }
                    else
                    {
                        this.m_bRunAsync = new bool?(((RunAsyncAttribute)customAttributes[0]).RunAsync);
                    }
                }

                return this.m_bRunAsync.Value;
            }
        }

        public bool IsUsingPowerShell { get; set; }

        public string JobID { get; set; }

        public virtual System.Drawing.Image LargeImage
        {
            get
            {
                System.Drawing.Image image;
                try
                {
                    if (string.IsNullOrEmpty(this.LargeImageName))
                    {
                        image = null;
                    }
                    else
                    {
                        image = ImageCache.GetImage(this.LargeImageName, this.ActionType.Assembly);
                    }
                }
                catch (Exception exception)
                {
                    image = null;
                }

                return image;
            }
        }

        public virtual string LargeImageName
        {
            get
            {
                if (this.m_sLargeImageName == null)
                {
                    object[] customAttributes = this.ActionType.GetCustomAttributes(typeof(LargeImageAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_sLargeImageName = "";
                    }
                    else
                    {
                        this.m_sLargeImageName = ((LargeImageAttribute)customAttributes[0]).ImageName;
                    }
                }

                return this.m_sLargeImageName;
            }
        }

        public bool LaunchAsJob
        {
            get
            {
                if (!this.m_bLaunchAsJob.HasValue)
                {
                    object[] customAttributes = this.ActionType.GetCustomAttributes(typeof(LaunchAsJobAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_bLaunchAsJob = new bool?(true);
                    }
                    else
                    {
                        this.m_bLaunchAsJob = new bool?(((LaunchAsJobAttribute)customAttributes[0]).LaunchAsJob);
                    }
                }

                return this.m_bLaunchAsJob.Value;
            }
        }

        public System.ComponentModel.License License
        {
            get { return this.m_license; }
        }

        public virtual string LicensingDescriptor
        {
            get { return Resources.License_Used; }
        }

        public virtual string LicensingUnit
        {
            get
            {
                string mSLicensingUnit;
                if (!this.m_bLicensingUnitFetchedSuccessfully)
                {
                    if (this.m_license == null)
                    {
                        try
                        {
                            this.UpdateLicense();
                            goto Label0;
                        }
                        catch
                        {
                            this.m_bLicensingUnitFetchedSuccessfully = false;
                            mSLicensingUnit = this.m_sLicensingUnit;
                        }

                        return mSLicensingUnit;
                    }

                    Label0:
                    MLLicenseCommon mLicense = this.m_license as MLLicenseCommon;
                    if (mLicense != null)
                    {
                        this.m_sLicensingUnit = mLicense.Provider.Settings.DataUnitName;
                    }

                    this.m_bLicensingUnitFetchedSuccessfully = true;
                }

                return this.m_sLicensingUnit;
            }
        }

        public virtual string MenuText
        {
            get
            {
                if (this.m_sMenuText == null)
                {
                    object[] customAttributes = this.ActionType.GetCustomAttributes(typeof(MenuTextAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_sMenuText = this.Name;
                    }
                    else
                    {
                        this.m_sMenuText = ((MenuTextAttribute)customAttributes[0]).MenuText;
                    }
                }

                return this.m_sMenuText;
            }
        }

        public virtual string MenuTextPlural
        {
            get
            {
                if (this.m_sMenuTextPlural == null)
                {
                    object[] customAttributes =
                        this.ActionType.GetCustomAttributes(typeof(MenuTextPluralAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_sMenuTextPlural = this.Name;
                    }
                    else
                    {
                        this.m_sMenuTextPlural = ((MenuTextPluralAttribute)customAttributes[0]).MenuText;
                    }
                }

                return this.m_sMenuTextPlural;
            }
        }

        public virtual PluralCondition MenuTextPluralCondition
        {
            get
            {
                if (!this.m_menuTextPluralCondition.HasValue)
                {
                    object[] customAttributes =
                        this.ActionType.GetCustomAttributes(typeof(MenuTextPluralAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_menuTextPluralCondition = new PluralCondition?(PluralCondition.None);
                    }
                    else
                    {
                        this.m_menuTextPluralCondition =
                            new PluralCondition?(((MenuTextPluralAttribute)customAttributes[0]).Condition);
                    }
                }

                if (!this.m_menuTextPluralCondition.HasValue)
                {
                    return PluralCondition.None;
                }

                return this.m_menuTextPluralCondition.Value;
            }
        }

        public virtual string Name
        {
            get
            {
                if (this.m_sName == null)
                {
                    object[] customAttributes = this.ActionType.GetCustomAttributes(typeof(NameAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_sName = this.ActionType.FullName;
                    }
                    else
                    {
                        this.m_sName = ((NameAttribute)customAttributes[0]).Name;
                    }
                }

                return this.m_sName;
            }
        }

        public virtual ActionOptions Options
        {
            get { return this.m_options; }
            set
            {
                this.m_options = value;
                this.FireOptionsChanged();
            }
        }

        public Metalogix.Actions.Action ParentAction { get; private set; }

        public virtual string[] RequiredSnapins
        {
            get
            {
                if (this.m_requiredSnapins == null)
                {
                    this.GetCmdletAttributes();
                }

                return this.m_requiredSnapins;
            }
        }

        public bool RequiresWriteAccess
        {
            get
            {
                if (!this.m_bRequiresWriteAccess.HasValue)
                {
                    object[] customAttributes =
                        this.ActionType.GetCustomAttributes(typeof(RequiresWriteAccessAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_bRequiresWriteAccess = new bool?(false);
                    }
                    else
                    {
                        this.m_bRequiresWriteAccess =
                            new bool?(((RequiresWriteAccessAttribute)customAttributes[0]).RequiresWriteAccess);
                    }
                }

                return this.m_bRequiresWriteAccess.Value;
            }
        }

        public bool RunSubActionAsync
        {
            get
            {
                if (!this.m_bRunSubActionAsync.HasValue)
                {
                    object[] customAttributes =
                        this.ActionType.GetCustomAttributes(typeof(RunSubActionAsyncAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_bRunSubActionAsync = new bool?(false);
                    }
                    else
                    {
                        this.m_bRunSubActionAsync =
                            new bool?(((RunSubActionAsyncAttribute)customAttributes[0]).RunSubActionAsync);
                    }
                }

                return this.m_bRunSubActionAsync.Value;
            }
        }

        public ShortcutAction ShortcutKeys
        {
            get
            {
                if (!this.m_shortcutKeys.HasValue)
                {
                    object[] customAttributes = this.ActionType.GetCustomAttributes(typeof(ShortcutAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_shortcutKeys = new ShortcutAction?(ShortcutAction.None);
                    }
                    else
                    {
                        this.m_bUseShortcutKeys = new bool?(((ShortcutAttribute)customAttributes[0]).UseShortcut);
                        this.m_shortcutKeys = new ShortcutAction?(((ShortcutAttribute)customAttributes[0]).Shortcut);
                    }
                }

                return this.m_shortcutKeys.Value;
            }
        }

        public virtual bool ShowInMenus
        {
            get
            {
                if (!this.m_bShowInMenus.HasValue)
                {
                    object[] customAttributes = this.ActionType.GetCustomAttributes(typeof(ShowInMenusAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_bShowInMenus = new bool?(true);
                    }
                    else
                    {
                        this.m_bShowInMenus = new bool?(((ShowInMenusAttribute)customAttributes[0]).ShowInMenus);
                    }
                }

                return this.m_bShowInMenus.Value;
            }
        }

        public bool ShowStatusDialog
        {
            get
            {
                if (!this.m_bShowStatusDialog.HasValue)
                {
                    object[] customAttributes =
                        this.ActionType.GetCustomAttributes(typeof(ShowStatusDialogAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_bShowStatusDialog = new bool?(false);
                    }
                    else
                    {
                        this.m_bShowStatusDialog =
                            new bool?(((ShowStatusDialogAttribute)customAttributes[0]).ShowStatusDialog);
                    }
                }

                return this.m_bShowStatusDialog.Value;
            }
        }

        public Cardinality SourceCardinality
        {
            get
            {
                if (!this.m_sourceCardinality.HasValue)
                {
                    object[] customAttributes =
                        this.ActionType.GetCustomAttributes(typeof(SourceCardinalityAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_sourceCardinality = new Cardinality?(Cardinality.ZeroOrMore);
                    }
                    else
                    {
                        this.m_sourceCardinality =
                            new Cardinality?(((SourceCardinalityAttribute)customAttributes[0]).Cardinality);
                    }
                }

                return this.m_sourceCardinality.Value;
            }
        }

        public Type SourceType
        {
            get
            {
                if (this.m_sourceType == null)
                {
                    object[] customAttributes = this.ActionType.GetCustomAttributes(typeof(SourceTypeAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_sourceType = typeof(object);
                    }
                    else
                    {
                        this.m_sourceType = ((SourceTypeAttribute)customAttributes[0]).Type;
                        this.m_bApplyToSourceSubTypes =
                            new bool?(((SourceTypeAttribute)customAttributes[0]).ApplyToSubTypes);
                    }
                }

                return this.m_sourceType;
            }
        }

        public ActionStatus Status
        {
            get { return this.m_status; }
        }

        public SubActionCollection SubActions
        {
            get
            {
                lock (this.m_oLockSubActionCollection)
                {
                    if (this.m_subActions == null)
                    {
                        this.m_subActions = new SubActionCollection();
                        this.m_subActions.SubActionAdded += new SubActionCollectionChanged(this.On_SubAction_Added);
                        this.m_subActions.SubActionRemoved += new SubActionCollectionChanged(this.On_SubAction_Removed);
                        this.m_handlerOperationStarted = new ActionEventHandler(this.On_OperationStarted);
                        this.m_handlerOperationUpdated = new ActionEventHandler(this.On_OperationUpdated);
                        this.m_handlerOperationFinished = new ActionEventHandler(this.On_OperationFinished);
                        this.m_handlerOperationSourceLinkChanged =
                            new ActionLinkChangedHandler(this.On_SourceLinkChanged);
                        this.m_handlerOperationTargetLinkChanged =
                            new ActionLinkChangedHandler(this.On_TargetLinkChanged);
                        this.m_handlerActionBlocked = new ActionBlockerHandler(this.On_ActionBlocked);
                    }
                }

                return this.m_subActions;
            }
        }

        public List<ITransformerDefinition> SupportedDefinitions
        {
            get
            {
                if (this.m_SupportedDefinitions == null)
                {
                    lock (this.m_oLockDefinitions)
                    {
                        if (this.m_SupportedDefinitions == null)
                        {
                            List<ITransformerDefinition> supportedDefinitions = this.GetSupportedDefinitions();
                            Type[] type = new Type[] { this.GetType() };
                            List<Type> types = new List<Type>(type);
                            this.GetSubActionSupportedDefinitions(this.GetType(), supportedDefinitions, types);
                            supportedDefinitions.Sort(new TrasformerDefinitionSorter());
                            this.m_SupportedDefinitions = supportedDefinitions;
                        }
                    }
                }

                return this.m_SupportedDefinitions;
            }
        }

        public bool SupportsThreeStateConfiguration
        {
            get
            {
                if (!this.m_bSupportsThreeStateConfiguration.HasValue)
                {
                    object[] customAttributes =
                        this.ActionType.GetCustomAttributes(typeof(SupportsThreeStateConfigurationAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_bSupportsThreeStateConfiguration = new bool?(false);
                    }
                    else
                    {
                        this.m_bSupportsThreeStateConfiguration = new bool?(
                            ((SupportsThreeStateConfigurationAttribute)customAttributes[0])
                            .SupportsThreeStateConfiguration);
                    }
                }

                return this.m_bSupportsThreeStateConfiguration.Value;
            }
        }

        public Cardinality TargetCardinality
        {
            get
            {
                if (!this.m_targetCardinality.HasValue)
                {
                    object[] customAttributes =
                        this.ActionType.GetCustomAttributes(typeof(TargetCardinalityAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_targetCardinality = new Cardinality?(Cardinality.One);
                    }
                    else
                    {
                        this.m_targetCardinality =
                            new Cardinality?(((TargetCardinalityAttribute)customAttributes[0]).Cardinality);
                    }
                }

                return this.m_targetCardinality.Value;
            }
        }

        public Type TargetType
        {
            get
            {
                if (this.m_targetType == null)
                {
                    object[] customAttributes = this.ActionType.GetCustomAttributes(typeof(TargetTypeAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_targetType = typeof(object);
                    }
                    else
                    {
                        this.m_targetType = ((TargetTypeAttribute)customAttributes[0]).Type;
                        this.m_bApplyToTargetSubTypes =
                            new bool?(((TargetTypeAttribute)customAttributes[0]).ApplyToSubTypes);
                    }
                }

                return this.m_targetType;
            }
        }

        public virtual bool ThreadingEnabled
        {
            get { return this.ThreadingStrategy.ThreadingEnabled; }
        }

        protected virtual IThreadingStrategy ThreadingStrategy
        {
            get { return SingleThreadedStrategy.Instance; }
        }

        public Metalogix.Threading.ThreadManager ThreadManager
        {
            get
            {
                if (this.m_threadManager == null)
                {
                    throw new ArgumentNullException("Thread manager has not been initialized");
                }

                return this.m_threadManager;
            }
        }

        public Metalogix.Transformers.TransformationRepository TransformationRepository
        {
            get
            {
                if (this._transformationRepository == null)
                {
                    this._transformationRepository = new Metalogix.Transformers.TransformationRepository();
                }

                return this._transformationRepository;
            }
            set { this._transformationRepository = value; }
        }

        public virtual bool UpdateLicensing
        {
            get { return this.m_bUpdateLicensing; }
        }

        public List<string> UsageDataCompletions
        {
            get
            {
                if (this.m_lstUsageDataCompletions == null)
                {
                    object[] customAttributes =
                        this.ActionType.GetCustomAttributes(typeof(UsageDataCompletionAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_lstUsageDataCompletions = new List<string>();
                    }
                    else
                    {
                        this.m_lstUsageDataCompletions =
                            ((UsageDataCompletionAttribute)customAttributes[0]).UsageDataCompletions;
                    }
                }

                return this.m_lstUsageDataCompletions;
            }
        }

        public bool UseShortcutKeys
        {
            get
            {
                if (!this.m_bUseShortcutKeys.HasValue)
                {
                    object[] customAttributes = this.ActionType.GetCustomAttributes(typeof(ShortcutAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_bUseShortcutKeys = new bool?(false);
                    }
                    else
                    {
                        this.m_bUseShortcutKeys = new bool?(((ShortcutAttribute)customAttributes[0]).UseShortcut);
                        this.m_shortcutKeys = new ShortcutAction?(((ShortcutAttribute)customAttributes[0]).Shortcut);
                    }
                }

                return this.m_bUseShortcutKeys.Value;
            }
        }

        public bool UseStickySettings
        {
            get
            {
                if (!this.m_bUsesStickySettings.HasValue)
                {
                    object[] customAttributes =
                        this.ActionType.GetCustomAttributes(typeof(UsesStickySettingsAttribute), true);
                    if ((int)customAttributes.Length != 1)
                    {
                        this.m_bUsesStickySettings = new bool?(false);
                    }
                    else
                    {
                        this.m_bUsesStickySettings =
                            new bool?(((UsesStickySettingsAttribute)customAttributes[0]).UsesStickySettings);
                    }
                }

                return this.m_bUsesStickySettings.Value;
            }
        }

        static Action()
        {
            Metalogix.Actions.Action.s_sTransformerInterfaceTypeName = typeof(ITransformer).Name;
        }

        public Action()
        {
            this._dataCounter = new ActionDataCounter();
            ActionConfigProvider.Instance.RegisterAction(this);
            bool flag = false;
            StackFrame[] frames = (new StackTrace()).GetFrames();
            int num = 0;
            while (true)
            {
                if (num < (int)frames.Length)
                {
                    MethodBase method = frames[num].GetMethod();
                    if (!method.IsConstructor)
                    {
                        Type declaringType = method.DeclaringType;
                        if (declaringType == null)
                        {
                            break;
                        }

                        while (declaringType.DeclaringType != null)
                        {
                            declaringType = declaringType.DeclaringType;
                        }

                        if (!declaringType.IsSubclassOf(typeof(Metalogix.Actions.Action)) || method.IsStatic)
                        {
                            break;
                        }

                        flag = true;
                        break;
                    }
                    else
                    {
                        num++;
                    }
                }
                else
                {
                    break;
                }
            }

            if (!flag)
            {
                ActionLicenseProvider.Instance.Validate(this.GetType(), this);
            }

            this.m_extendedTelemetryData = new Dictionary<string, string>()
            {
                { this.Name, "Action Started" }
            };
        }

        public virtual Dictionary<string, string> AnalyzeAction(Job parentJob, DateTime pivotDate)
        {
            return null;
        }

        private void AppendActionCommand(StringBuilder output, bool hasSource, string jobDb, string jobID)
        {
            output.AppendLine("# Run the action");
            output.AppendLine("foreach($Target in $TargetCollection) {");
            output.Append("    ");
            if (hasSource)
            {
                output.Append("$SourceCollection | ");
            }

            output.Append(string.Concat(this.CmdletName, " -Target $Target "));
            this.AppendPowerShellParameters(output, this.Options);
            if (jobDb != null)
            {
                output.Append(jobDb);
            }

            if (string.IsNullOrEmpty(jobID))
            {
                output.AppendLine();
            }
            else
            {
                output.Append(string.Concat(" -jobid \"", jobID, "\"")).AppendLine();
            }

            output.AppendLine("}");
        }

        private void AppendConfigurationVariableSettingsIfExists(StringBuilder output, ResourceScope scope,
            string settingsFile)
        {
            if (!File.Exists(settingsFile))
            {
                return;
            }

            output.AppendFormat(
                string.Concat("Load-MetalogixConfigurationVariableSettings -FilePath \"{0}\" -Scope {1}",
                    Environment.NewLine), settingsFile, scope);
        }

        private void AppendDefaultResolverSettings(StringBuilder output)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<string, string> allSetting in DefaultResolverSettings.GetAllSettings())
            {
                stringBuilder.AppendFormat(
                    string.Concat("Set-MetalogixDefaultResolverSetting -Name \"{0}\" -Value \"{1}\"",
                        Environment.NewLine), allSetting.Key, allSetting.Value);
            }

            if (stringBuilder.Length <= 0)
            {
                return;
            }

            output.AppendLine("# Set default resolvers");
            output.Append(stringBuilder);
            output.AppendLine();
        }

        private void AppendLoadConfigurationVariableSettings(StringBuilder output)
        {
            StringBuilder stringBuilder = new StringBuilder();
            this.AppendResourceTableIfFileResolver(stringBuilder, ConfigurationVariables.UserSpecificVariables);
            this.AppendResourceTableIfFileResolver(stringBuilder, ConfigurationVariables.ApplicationSpecificVariables);
            this.AppendResourceTableIfFileResolver(stringBuilder,
                ConfigurationVariables.ApplicationAndUserSpecificVariables);
            this.AppendResourceTableIfFileResolver(stringBuilder, ConfigurationVariables.EnvironmentVariables);
            this.AppendResourceTableIfFileResolver(stringBuilder, ConfigurationVariables.LegacyEnvironmentVariables);
            if (stringBuilder.Length <= 0)
            {
                return;
            }

            output.AppendLine("# Load configuration settings");
            output.Append(stringBuilder);
            output.AppendLine();
        }

        protected virtual void AppendPowerShellParameters(StringBuilder stringBuilder_0, object cmdletOptions)
        {
            bool flag;
            PropertyInfo[] optionParameters = this.GetOptionParameters(cmdletOptions);
            for (int i = 0; i < (int)optionParameters.Length; i++)
            {
                PropertyInfo propertyInfo = optionParameters[i];
                object[] customAttributes =
                    propertyInfo.GetCustomAttributes(typeof(CmdletEnabledParameterAttribute), true);
                bool cmdletEnabledParameter = true;
                if ((int)customAttributes.Length > 0)
                {
                    CmdletEnabledParameterAttribute cmdletEnabledParameterAttribute =
                        (CmdletEnabledParameterAttribute)customAttributes[0];
                    if (cmdletEnabledParameterAttribute.ConditionalPropertyName == null)
                    {
                        cmdletEnabledParameter = cmdletEnabledParameterAttribute.CmdletEnabledParameter;
                    }
                    else
                    {
                        PropertyInfo property = cmdletOptions.GetType()
                            .GetProperty(cmdletEnabledParameterAttribute.ConditionalPropertyName);
                        cmdletEnabledParameter = (property != null
                            ? property.GetValue(cmdletOptions, null)
                                .Equals(cmdletEnabledParameterAttribute.ExpectedValue)
                            : cmdletEnabledParameterAttribute.CmdletEnabledParameter);
                    }
                }

                if (cmdletEnabledParameter)
                {
                    string name = propertyInfo.Name;
                    object[] objArray = propertyInfo.GetCustomAttributes(typeof(CmdletParameterAliasAttribute), true);
                    if ((int)objArray.Length > 0)
                    {
                        name = ((CmdletParameterAliasAttribute)objArray[0]).Alias;
                    }

                    if (propertyInfo.PropertyType == typeof(IFilterExpression))
                    {
                        object value = propertyInfo.GetValue(cmdletOptions, null);
                        string xML = null;
                        if (value is FilterExpression)
                        {
                            xML = ((FilterExpression)value).ToXML();
                        }
                        else if (value is FilterExpressionList)
                        {
                            xML = ((FilterExpressionList)value).ToXML();
                        }

                        if (xML != null)
                        {
                            string str = propertyInfo.PropertyType.Assembly.ToString();
                            str = str.Substring(0, str.IndexOf(','));
                            object[] objArray1 = new object[]
                            {
                                name, this.EncodePowerShellParameterValue(propertyInfo.PropertyType.FullName), str,
                                this.EncodePowerShellParameterValue(xML)
                            };
                            stringBuilder_0.Append(
                                string.Format(" -{0} ( New-MetalogixSerializableObject {1} \"{2}\" {3} )", objArray1));
                        }
                    }
                    else if (typeof(XmlNode).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        object obj = propertyInfo.GetValue(cmdletOptions, null);
                        if (obj != null)
                        {
                            string[] strArrays = new string[]
                            {
                                " -", name, " ([xml]", this.EncodePowerShellParameterValue(((XmlNode)obj).OuterXml), ")"
                            };
                            stringBuilder_0.Append(string.Concat(strArrays));
                        }
                    }
                    else if (propertyInfo.PropertyType.IsClass && propertyInfo.PropertyType == typeof(string[]))
                    {
                        string[] value1 = propertyInfo.GetValue(cmdletOptions, null) as string[];
                        if (value1 != null && (int)value1.Length > 0)
                        {
                            stringBuilder_0.Append(string.Concat(" -", name, " "));
                            for (int j = 0; j < (int)value1.Length; j++)
                            {
                                stringBuilder_0.Append(this.EncodePowerShellParameterValue(value1[j]));
                                if (j < (int)value1.Length - 1)
                                {
                                    stringBuilder_0.Append(",");
                                }
                            }
                        }
                    }
                    else if (propertyInfo.PropertyType.IsClass && propertyInfo.PropertyType != typeof(string))
                    {
                        bool flatten = false;
                        object[] customAttributes1 =
                            propertyInfo.GetCustomAttributes(typeof(CmdletParameterFlattenAttribute), true);
                        if ((int)customAttributes1.Length > 0)
                        {
                            flatten = ((CmdletParameterFlattenAttribute)customAttributes1[0]).Flatten;
                        }

                        bool enumerate = false;
                        object[] customAttributes2 =
                            propertyInfo.GetCustomAttributes(typeof(CmdletParameterEnumerateAttribute), true);
                        if ((int)customAttributes2.Length > 0)
                        {
                            enumerate = ((CmdletParameterEnumerateAttribute)customAttributes2[0]).Enumerate;
                        }

                        MethodInfo method = propertyInfo.PropertyType.GetMethod("ToXML",
                            BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis,
                            Type.EmptyTypes, null);
                        ConstructorInfo constructor =
                            propertyInfo.PropertyType.GetConstructor(new Type[] { typeof(XmlNode) });
                        Type propertyType = propertyInfo.PropertyType;
                        Type[] typeArray = new Type[] { typeof(XmlNode) };
                        MethodInfo methodInfo = propertyType.GetMethod("FromXML",
                            BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis, typeArray,
                            null);
                        if (flatten || method == null)
                        {
                            flag = true;
                        }
                        else
                        {
                            flag = (constructor != null ? false : methodInfo == null);
                        }

                        flatten = flag;
                        if (!flag)
                        {
                            string str1 = propertyInfo.PropertyType.Assembly.ToString();
                            str1 = str1.Substring(0, str1.IndexOf(','));
                            object obj1 = propertyInfo.GetValue(cmdletOptions, null);
                            if (obj1 != null)
                            {
                                StringBuilder stringBuilder0 = stringBuilder_0;
                                object[] objArray2 = new object[]
                                {
                                    name, this.EncodePowerShellParameterValue(propertyInfo.PropertyType.FullName), str1,
                                    this.EncodePowerShellParameterValue(method.Invoke(obj1, null).ToString()), null
                                };
                                objArray2[4] = (enumerate ? "-Enumerate" : "");
                                stringBuilder0.Append(string.Format(
                                    " -{0} ( New-MetalogixSerializableObject {1} \"{2}\" {3} {4})", objArray2));
                            }
                        }
                        else
                        {
                            this.AppendPowerShellParameters(stringBuilder_0,
                                propertyInfo.GetValue(cmdletOptions, null));
                        }
                    }
                    else if (propertyInfo.PropertyType != typeof(bool))
                    {
                        object value2 = propertyInfo.GetValue(cmdletOptions, null);
                        if (value2 != null)
                        {
                            stringBuilder_0.Append(string.Concat(" -", name, " ",
                                this.EncodePowerShellParameterValue(value2.ToString())));
                        }
                    }
                    else if ((bool)propertyInfo.GetValue(cmdletOptions, null))
                    {
                        stringBuilder_0.Append(string.Concat(" -", name));
                    }
                }
            }
        }

        private void AppendResourceTableIfFileResolver(StringBuilder output, ResourceTableLink link)
        {
            string commonDataPath;
            if (!(ObjectResolverCatalog.GetDefaultResolver(link) is ResourceFileTableResolver))
            {
                return;
            }

            if ((ResourceScope.EnvironmentSpecific & link.Scope) == ResourceScope.EnvironmentSpecific)
            {
                commonDataPath = ApplicationData.CommonDataPath;
            }
            else if ((ResourceScope.ApplicationSpecific & link.Scope) != ResourceScope.ApplicationSpecific)
            {
                commonDataPath = ((ResourceScope.UserSpecific & link.Scope) != ResourceScope.UserSpecific
                    ? ApplicationData.ApplicationPath
                    : ApplicationData.CompanyPath);
            }
            else
            {
                commonDataPath = ApplicationData.CommonApplicationPath;
            }

            string str = Path.Combine(commonDataPath, string.Concat(link.Name, ".xml"));
            this.AppendConfigurationVariableSettingsIfExists(output, link.Scope, str);
        }

        private void AppendSource(StringBuilder output, string sourceXml)
        {
            output.AppendLine("# Load source");
            output.AppendLine(string.Concat("$SourceCollection = New-MetalogixSerializableObjectCollection ",
                this.EncodePowerShellParameterValue(sourceXml)));
            output.AppendLine();
        }

        private void AppendTarget(StringBuilder output, string targetXml)
        {
            output.AppendLine("# Load target");
            output.AppendLine(string.Concat("$TargetCollection = New-MetalogixSerializableObjectCollection ",
                this.EncodePowerShellParameterValue(targetXml)));
            output.AppendLine();
        }

        public virtual bool AppliesTo(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
        {
            return Metalogix.Actions.Action.AppliesToBase(this, sourceSelections, targetSelections);
        }

        protected static bool AppliesToBase(Metalogix.Actions.Action action, IXMLAbleList sourceSelections,
            IXMLAbleList targetSelections)
        {
            bool flag;
            if (sourceSelections == null)
            {
                sourceSelections = new XMLAbleList();
            }

            if (targetSelections == null)
            {
                targetSelections = new XMLAbleList();
            }

            if (targetSelections.Count > 0)
            {
                Node item = targetSelections[0] as Node;
                if (item != null && !action.IsConnectivityAction && item.Status == ConnectionStatus.Invalid)
                {
                    return false;
                }
            }

            Type collectionType = sourceSelections.CollectionType;
            Type type = targetSelections.CollectionType;
            if (!Metalogix.Actions.Action.CardinalityEquals(targetSelections.Count, action.TargetCardinality))
            {
                return false;
            }

            if (action.TargetType.IsInterface)
            {
                if (targetSelections.Count > 0 &&
                    (int)type.FindInterfaces(new TypeFilter(Metalogix.Actions.Action.InterfaceFilter),
                        action.TargetType.ToString()).Length == 0)
                {
                    return false;
                }
            }
            else if (targetSelections.Count > 0 && type != action.TargetType &&
                     (!action.ApplyToTargetSubTypes || !type.IsSubclassOf(action.TargetType)))
            {
                return false;
            }

            if (!Metalogix.Actions.Action.CardinalityEquals(sourceSelections.Count, action.SourceCardinality))
            {
                return false;
            }

            if (action.SourceType.IsInterface)
            {
                if (sourceSelections.Count > 0 && (int)collectionType
                        .FindInterfaces(new TypeFilter(Metalogix.Actions.Action.InterfaceFilter),
                            action.SourceType.ToString()).Length == 0)
                {
                    return false;
                }
            }
            else if (sourceSelections.Count > 0 && collectionType != action.SourceType &&
                     (!action.ApplyToSourceSubTypes || !collectionType.IsSubclassOf(action.SourceType)))
            {
                return false;
            }

            IEnumerable<SuppressActionAttribute> attributesFromType = null;
            if (type != null)
            {
                attributesFromType = ReflectionUtils.GetAttributesFromType<SuppressActionAttribute>(type);
            }

            if (attributesFromType != null && attributesFromType.Any<SuppressActionAttribute>())
            {
                using (IEnumerator<SuppressActionAttribute> enumerator = attributesFromType.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.SuppressAction != action.ActionType.FullName)
                        {
                            continue;
                        }

                        flag = false;
                        return flag;
                    }

                    return true;
                }

                return flag;
            }

            return true;
        }

        public void Cancel()
        {
            if (this.Status == ActionStatus.Running || this.Status == ActionStatus.Paused)
            {
                this.SetStatus(ActionStatus.Aborting);
            }
        }

        private static bool CardinalityEquals(int iCount, Cardinality cardinality_0)
        {
            switch (cardinality_0)
            {
                case Cardinality.Zero:
                {
                    return iCount == 0;
                }
                case Cardinality.One:
                {
                    return iCount == 1;
                }
                case Cardinality.ZeroOrOne:
                {
                    if (iCount == 0)
                    {
                        return true;
                    }

                    return iCount == 1;
                }
                case Cardinality.MoreThanOne:
                {
                    return iCount > 1;
                }
                case Cardinality.Zero | Cardinality.MoreThanOne:
                {
                    return false;
                }
                case Cardinality.OneOrMore:
                {
                    return iCount >= 1;
                }
                case Cardinality.ZeroOrMore:
                {
                    return iCount >= 0;
                }
                default:
                {
                    return false;
                }
            }
        }

        protected bool CheckForAbort()
        {
            if (this.CheckForCancel())
            {
                return true;
            }

            try
            {
                this.CheckForPause();
            }
            catch (Exception exception)
            {
                return true;
            }

            this.CheckForBlock();
            return false;
        }

        protected virtual void CheckForBlock()
        {
            Func<IActionBlocker, bool> func = null;
            bool flag = false;
            ActionUtils.TryActionUntilTrueElseThrowIfMaxTriesReached(2147483647, () =>
            {
                if (this.CheckForCancel())
                {
                    return true;
                }

                IList<IActionBlocker> actionBlockers = this.ActionBlockers;
                if (func == null)
                {
                    func = (IActionBlocker blocker) => blocker.ShouldBlock();
                }

                IActionBlocker actionBlocker = actionBlockers.FirstOrDefault<IActionBlocker>(func);
                if (actionBlocker == null)
                {
                    if (flag)
                    {
                        this.FireActionUnblocked();
                    }

                    return true;
                }

                flag = true;
                this.FireActionBlocked(actionBlocker);
                actionBlocker.BlockUntil(() =>
                {
                    if (this.CheckForCancel())
                    {
                        return true;
                    }

                    return !actionBlocker.ShouldBlock();
                });
                return false;
            });
        }

        protected bool CheckForCancel()
        {
            if (this.Status == ActionStatus.Aborting)
            {
                return true;
            }

            return false;
        }

        private void CheckForPause()
        {
            if (this.Status == ActionStatus.Paused)
            {
                while (this.Status == ActionStatus.Paused)
                {
                    if (this.CheckForCancel())
                    {
                        throw new Exception();
                    }

                    Thread.Sleep(100);
                }
            }
        }

        protected internal bool CheckNodeMetabaseConnectionExist(IXMLAbleList sourceSelections,
            IXMLAbleList targetSelections)
        {
            bool @base = Metalogix.Actions.Action.AppliesToBase(this, sourceSelections, targetSelections);
            bool flag = @base;
            if (@base)
            {
                Node item = targetSelections[0] as Node;
                if (item == null || item.MetabaseConnection == null)
                {
                    flag = false;
                }
            }

            return flag;
        }

        protected virtual void CleanUp()
        {
            this.TransformationRepository.Clear();
        }

        private void CleanUpSubActions()
        {
            List<Metalogix.Actions.Action> actions = new List<Metalogix.Actions.Action>();
            foreach (Metalogix.Actions.Action subAction in this.SubActions)
            {
                subAction.CleanUpSubActions();
                actions.Add(subAction);
            }

            foreach (Metalogix.Actions.Action action in actions)
            {
                this.SubActions.Remove(action);
            }
        }

        public virtual Metalogix.Actions.Action Clone()
        {
            Metalogix.Actions.Action action = (Metalogix.Actions.Action)Activator.CreateInstance(this.GetType());
            action.FromXML(XmlUtility.StringToXmlNode(this.ToXML()));
            return action;
        }

        public ConfigurationResult Configure(ref IXMLAbleList source, ref IXMLAbleList target,
            out bool hasConfiguration)
        {
            this.m_hasConfiguration = true;
            ConfigurationResult configurationResult = this.Configure(ref source, ref target);
            hasConfiguration = this.m_hasConfiguration;
            return configurationResult;
        }

        public virtual ConfigurationResult Configure(ref IXMLAbleList source, ref IXMLAbleList target)
        {
            ConfigurationResult configurationResult;
            int num = (source == null ? 0 : source.Count);
            if (!Metalogix.Actions.Action.CardinalityEquals(num, this.SourceCardinality))
            {
                if (num == 0)
                {
                    throw new Exception(Resources.FailedToLocateActionSources);
                }

                if (this.SourceCardinality == Cardinality.MoreThanOne)
                {
                    throw new Exception(Resources.OnlySingleActionSourceCouldBeLocated);
                }
            }

            int num1 = (target == null ? 0 : target.Count);
            if (!Metalogix.Actions.Action.CardinalityEquals(num1, this.TargetCardinality))
            {
                if (num1 == 0)
                {
                    throw new Exception(Resources.FailedToLocateActionTargets);
                }

                if (this.TargetCardinality == Cardinality.MoreThanOne)
                {
                    throw new Exception(Resources.OnlySingleActionTargetCouldBeLocated);
                }
            }

            this.ValidateMandatoryTransformers();
            IActionConfig actionConfig = ActionConfigProvider.Instance.GetActionConfig(this);
            if (actionConfig == null)
            {
                if (!this.Configure(source, target))
                {
                    return ConfigurationResult.Cancel;
                }

                return ConfigurationResult.Run;
            }

            using (ActionContext actionContext = new ActionContext(source, target))
            {
                using (ActionConfigContext actionConfigContext = new ActionConfigContext(this, actionContext))
                {
                    ConfigurationResult configurationResult1 = actionConfig.Configure(actionConfigContext);
                    source = actionContext.Sources;
                    target = actionContext.Targets;
                    configurationResult = configurationResult1;
                }
            }

            return configurationResult;
        }

        [Obsolete]
        public virtual bool Configure(IXMLAbleList source, IXMLAbleList target)
        {
            this.m_hasConfiguration = false;
            return true;
        }

        public void ConnectOperationLogging(IOperationLogging operationLogging)
        {
            operationLogging.OperationStarted += this.m_handlerOperationStarted;
            operationLogging.OperationUpdated += this.m_handlerOperationUpdated;
            operationLogging.OperationFinished += this.m_handlerOperationFinished;
        }

        protected virtual void ConnectSubaction(Metalogix.Actions.Action subAction)
        {
            if (subAction != null)
            {
                if (this.GetType() != typeof(JobRunner))
                {
                    subAction.SetStatus(this.Status);
                    subAction.m_threadManager = this.ThreadManager;
                    subAction.TransformationRepository = this.TransformationRepository;
                    subAction.ActionBlockers = this.ActionBlockers;
                    ISyncActionOptions options = this.Options as ISyncActionOptions;
                    if (options != null)
                    {
                        options.SyncOptions(subAction.Options);
                    }

                    subAction.ParentAction = this;
                }

                subAction._dataCounter = this._dataCounter;
                subAction.OperationStarted += this.m_handlerOperationStarted;
                subAction.OperationUpdated += this.m_handlerOperationUpdated;
                subAction.OperationFinished += this.m_handlerOperationFinished;
                subAction.SourceLinkChanged += this.m_handlerOperationSourceLinkChanged;
                subAction.TargetLinkChanged += this.m_handlerOperationTargetLinkChanged;
                subAction.ActionBlocked += this.m_handlerActionBlocked;
            }
        }

        public void ConnectTransformer(ITransformer transformer)
        {
            transformer.OperationStarted += this.m_handlerOperationStarted;
            transformer.OperationUpdated += this.m_handlerOperationUpdated;
            transformer.OperationFinished += this.m_handlerOperationFinished;
        }

        public Dictionary<string, string> ConvertOptionsXmlToDictionary(string actionOptionXml)
        {
            string[] strArrays = Converter.TransformXmlToText(actionOptionXml).Split(new char[] { ',' });
            Dictionary<string, string> strs = new Dictionary<string, string>();
            string[] strArrays1 = strArrays;
            for (int i = 0; i < (int)strArrays1.Length; i++)
            {
                string str = strArrays1[i];
                string[] strArrays2 = str.Split(new char[] { ':' });
                if (!strs.ContainsKey(strArrays2[0].Trim()))
                {
                    strs.Add(strArrays2[0].Trim(), strArrays2[1].Trim());
                }
            }

            return strs;
        }

        public static Metalogix.Actions.Action CreateAction(string sActionSettingsXML)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sActionSettingsXML);
            XmlNode xmlNodes = xmlDocument.SelectSingleNode("//Action");
            if (xmlNodes == null)
            {
                throw new ArgumentException("XML provided does not define an action");
            }

            if (xmlNodes.Attributes["ActionType"] == null)
            {
                throw new Exception("The 'ActionType' attribute cannot be null");
            }

            Metalogix.Actions.Action action =
                Metalogix.Actions.Action.CreateActionFromTypeName(xmlNodes.Attributes["ActionType"].Value);
            action.FromXML(xmlNodes);
            return action;
        }

        public static Metalogix.Actions.Action CreateActionFromTypeName(string actionTypeName)
        {
            Type type = Type.GetType(TypeUtils.UpdateType(actionTypeName));
            if (type == null)
            {
                throw new Exception(string.Concat("The action '", actionTypeName, "' could not be found."));
            }

            return (Metalogix.Actions.Action)Activator.CreateInstance(type);
        }

        private void CreateThreadManager()
        {
            this.m_threadManager = new Metalogix.Threading.ThreadManager(this.ThreadingStrategy);
            this.m_threadManager.AsynchronousTaskFailed += new TaskFailed(this.ProcessTaskErrorError);
        }

        public void DisconnectOperationLogging(IOperationLogging operationLogging)
        {
            operationLogging.OperationStarted -= this.m_handlerOperationStarted;
            operationLogging.OperationUpdated -= this.m_handlerOperationUpdated;
            operationLogging.OperationFinished -= this.m_handlerOperationFinished;
        }

        protected virtual void DisconnectSubaction(Metalogix.Actions.Action subAction)
        {
            if (subAction != null)
            {
                subAction.OperationStarted -= this.m_handlerOperationStarted;
                subAction.OperationUpdated -= this.m_handlerOperationUpdated;
                subAction.OperationFinished -= this.m_handlerOperationFinished;
                subAction.SourceLinkChanged -= this.m_handlerOperationSourceLinkChanged;
                subAction.TargetLinkChanged -= this.m_handlerOperationTargetLinkChanged;
                subAction.ActionBlocked -= this.m_handlerActionBlocked;
                subAction._dataCounter = null;
                subAction.TransformationRepository = null;
                subAction.ActionBlockers = null;
                subAction.ParentAction = null;
                subAction.m_threadManager = null;
            }
        }

        public void DisconnectTransformer(ITransformer transformer)
        {
            transformer.OperationStarted -= this.m_handlerOperationStarted;
            transformer.OperationUpdated -= this.m_handlerOperationUpdated;
            transformer.OperationFinished -= this.m_handlerOperationFinished;
        }

        private void DisposeThreadManager()
        {
            try
            {
                if (this.m_threadManager != null)
                {
                    this.m_threadManager.SetBufferedTasks("RunActionEndReached", false, false);
                    this.m_threadManager.Dispose();
                    this.m_threadManager.AsynchronousTaskFailed -= new TaskFailed(this.ProcessTaskErrorError);
                    this.m_threadManager = null;
                }
            }
            catch (Exception exception)
            {
            }
        }

        public virtual bool EnabledOn(IXMLAbleList sourceSelections, IXMLAbleList targetSelections)
        {
            return true;
        }

        protected string EncodePowerShellParameterValue(string sValue)
        {
            string str = sValue.Replace("`", "``");
            str = str.Replace("\"", "`\"");
            str = str.Replace("$", "`$");
            return string.Concat("\"", str, "\"");
        }

        private void EndActionBlockedLog()
        {
            if (this._actionBlockedLogItem == null)
            {
                return;
            }

            this.FireOperationFinished(this._actionBlockedLogItem);
            this._actionBlockedLogItem = null;
        }

        private void EndActionBlockedLogUsingRootAction()
        {
            Metalogix.Actions.Action rootAction = this.GetRootAction();
            lock (this.m_lockObjectThreadSync)
            {
                if (rootAction.HasLoggedActionBlocked())
                {
                    rootAction.EndActionBlockedLog();
                }
            }
        }

        private void FireActionBlocked(IActionBlocker blocker)
        {
            if (this.ActionBlocked != null)
            {
                this.ActionBlocked(this,
                    new ActionBlockerEventArgs(ActionBlockerChangeType.Blocked, blocker,
                        string.Format("The migration is paused because {0}", blocker.BlockedReason)));
            }

            ILoggableActionBlocker loggableActionBlocker = blocker as ILoggableActionBlocker;
            if (loggableActionBlocker == null)
            {
                return;
            }

            this.StartActionBlockedLogUsingRootAction(loggableActionBlocker);
        }

        protected void FireActionFinished()
        {
            if (this.ActionFinished != null)
            {
                this.ActionFinished();
            }
        }

        protected void FireActionStarted(string sSourceString, string sTargetString)
        {
            if (this.ActionStarted != null)
            {
                this.ActionStarted(this, sSourceString, sTargetString);
            }
        }

        private void FireActionUnblocked()
        {
            if (this.ActionBlocked != null)
            {
                this.ActionBlocked(this,
                    new ActionBlockerEventArgs(ActionBlockerChangeType.Unblocked, null,
                        "The migration is now resuming..."));
            }

            this.EndActionBlockedLogUsingRootAction();
        }

        protected void FireActionValidated(IXMLAbleList source, IXMLAbleList target)
        {
            if (this.ActionValidated != null)
            {
                this.ActionValidated(this, source, target);
            }
        }

        protected internal void FireOperationFinished(LogItem operation)
        {
            if (this.UpdateLicensing)
            {
                MLLicenseCommon mLicense = this.m_license as MLLicenseCommon;
                if (mLicense != null)
                {
                    mLicense.IncrementLicenseUseValues(operation.LicenseDataUsed, operation.LicenseItemsCount);
                }

                if (this._dataCounter != null)
                {
                    this._dataCounter.IncrementDataUsed(operation.LicenseDataUsed);
                    this._dataCounter.IncrementItemsUsed(operation.LicenseItemsCount);
                }
            }

            operation.FinishedTime = DateTime.Now;
            if (this.OperationFinished != null)
            {
                this.OperationFinished(operation);
            }
        }

        protected internal void FireOperationStarted(LogItem operation)
        {
            if (this.OperationStarted != null)
            {
                this.OperationStarted(operation);
            }

            if (operation != null)
            {
                operation.ActionLicensingUnit = this.LicensingUnit;
                operation.ActionLicensingDescriptor = this.LicensingDescriptor;
            }
        }

        protected internal void FireOperationUpdated(LogItem operation)
        {
            if (this.OperationUpdated != null)
            {
                this.OperationUpdated(operation);
            }
        }

        protected void FireOptionsChanged()
        {
            if (this.OptionsChanged != null)
            {
                this.OptionsChanged();
            }
        }

        protected void FireSourceLinkChanged(string link)
        {
            if (this.SourceLinkChanged != null)
            {
                this.SourceLinkChanged(link);
            }
        }

        private void FireStatusChanged(ActionStatus status)
        {
            if (this.StatusChanged != null)
            {
                this.StatusChanged(status);
            }
        }

        protected void FireTargetLinkChanged(string link)
        {
            if (this.TargetLinkChanged != null)
            {
                this.TargetLinkChanged(link);
            }
        }

        protected virtual void FromXML(XmlNode xmlNode)
        {
            if (this.Options != null)
            {
                this.Options.FromXML(xmlNode);
            }
        }

        public virtual Metalogix.Actions.Action GenerateIncrementalAction(DateTime? incrementFrom)
        {
            Metalogix.Actions.Action action = this.Clone();
            action.Options.MakeOptionsIncremental(incrementFrom);
            return action;
        }

        public virtual long GetAnalysisCost(Dictionary<string, string> analysisProperties)
        {
            return 0L;
        }

        public virtual string GetBatchableName(ActionContext context)
        {
            return this.BatchableName;
        }

        private void GetCmdletAttributes()
        {
            object[] customAttributes = this.ActionType.GetCustomAttributes(typeof(CmdletEnabledAttribute), false);
            if ((int)customAttributes.Length != 1)
            {
                this.m_bCmdletEnabled = new bool?(false);
                return;
            }

            CmdletEnabledAttribute cmdletEnabledAttribute = customAttributes[0] as CmdletEnabledAttribute;
            if (cmdletEnabledAttribute == null)
            {
                this.m_bCmdletEnabled = new bool?(false);
                return;
            }

            this.m_bCmdletEnabled = new bool?(cmdletEnabledAttribute.CmdletEnabled);
            this.m_sCmdletName = cmdletEnabledAttribute.CmdletName;
            this.m_requiredSnapins = cmdletEnabledAttribute.RequiredSnapins;
        }

        public virtual bool GetCollectionsViolateSourceTargetRestrictions(IXMLAbleList source, IXMLAbleList target,
            out string failureMessage)
        {
            bool flag;
            failureMessage = null;
            if (!this.AllowsSameSourceTarget && this.SourceCardinality != Cardinality.Zero && source != null &&
                target != null)
            {
                IEnumerator enumerator = source.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        object current = enumerator.Current;
                        IEnumerator enumerator1 = target.GetEnumerator();
                        try
                        {
                            while (enumerator1.MoveNext())
                            {
                                object obj = enumerator1.Current;
                                if (current == null || obj == null || !current.Equals(obj))
                                {
                                    continue;
                                }

                                if (source.Count != 1 || target.Count != 1)
                                {
                                    failureMessage = Resources.CannotRunOneOrMoreSourcesAlsoTarget;
                                }
                                else
                                {
                                    failureMessage = Resources.CannotRunTargetIsSameAsSource;
                                }

                                flag = true;
                                return flag;
                            }
                        }
                        finally
                        {
                            IDisposable disposable = enumerator1 as IDisposable;
                            if (disposable != null)
                            {
                                disposable.Dispose();
                            }
                        }
                    }

                    return false;
                }
                finally
                {
                    IDisposable disposable1 = enumerator as IDisposable;
                    if (disposable1 != null)
                    {
                        disposable1.Dispose();
                    }
                }

                return flag;
            }

            return false;
        }

        public virtual CompletionDetailsOrderProvider GetCompletionDetailsOrderProvider()
        {
            return CompletionDetailsOrderProvider.GetOrderProvider(this);
        }

        public virtual System.Drawing.Image GetImage(ActionContext context = null)
        {
            return this.Image;
        }

        public virtual System.Drawing.Image GetLargeImage(ActionContext context = null)
        {
            return this.LargeImage;
        }

        private void GetMandatoryTransformers(Type type, List<Type> transformerTypes, List<Type> checkedTypes)
        {
            object[] customAttributes = type.GetCustomAttributes(typeof(MandatoryTransformersAttribute), true);
            for (int i = 0; i < (int)customAttributes.Length; i++)
            {
                Type[] mandatoryTransformers =
                    ((MandatoryTransformersAttribute)customAttributes[i]).MandatoryTransformers;
                for (int j = 0; j < (int)mandatoryTransformers.Length; j++)
                {
                    Type type1 = mandatoryTransformers[j];
                    if (!transformerTypes.Contains(type1) &&
                        type1.GetInterface(Metalogix.Actions.Action.s_sTransformerInterfaceTypeName) != null)
                    {
                        transformerTypes.Add(type1);
                    }
                }
            }

            checkedTypes.Add(type);
            this.GetSubActionMandatoryTransformers(type, transformerTypes, checkedTypes);
        }

        public virtual string GetMenuText(ActionContext context)
        {
            string menuText = this.MenuText;
            bool flag = (context.Sources == null ? false : context.Sources.Count > 1);
            bool flag1 = (context.Targets == null ? false : context.Targets.Count > 1);
            if (this.MenuTextPluralCondition == PluralCondition.MultipleBoth && flag && flag1 ||
                this.MenuTextPluralCondition == PluralCondition.MultipleEither && (flag || flag1) ||
                this.MenuTextPluralCondition == PluralCondition.MultipleSources && flag ||
                this.MenuTextPluralCondition == PluralCondition.MultipleTargets && flag1)
            {
                menuText = this.MenuTextPlural;
            }

            return menuText;
        }

        public virtual PropertyInfo[] GetOptionParameters(object cmdletOptions)
        {
            return cmdletOptions.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }

        public virtual string GetPowershellCommand(string sourceXML, string targetXML, string jobsDb,
            string jobID = null)
        {
            if (!this.CmdletEnabled)
            {
                return null;
            }

            bool flag = !string.IsNullOrEmpty(sourceXML);
            if ((this.SourceCardinality == Cardinality.One || this.SourceCardinality == Cardinality.OneOrMore
                    ? true
                    : this.SourceCardinality == Cardinality.MoreThanOne) && !flag)
            {
                throw new Exception("Failed to generate powershell script. No source value specified.");
            }

            StringBuilder stringBuilder = new StringBuilder();
            this.AppendDefaultResolverSettings(stringBuilder);
            this.AppendLoadConfigurationVariableSettings(stringBuilder);
            if (flag)
            {
                this.AppendSource(stringBuilder, sourceXML);
            }

            this.AppendTarget(stringBuilder, targetXML);
            this.AppendActionCommand(stringBuilder, flag, jobsDb, jobID);
            return stringBuilder.ToString();
        }

        public virtual string GetPowerShellParameters()
        {
            StringBuilder stringBuilder = new StringBuilder();
            this.AppendPowerShellParameters(stringBuilder, this.Options);
            return stringBuilder.ToString();
        }

        private Metalogix.Actions.Action GetRootAction()
        {
            if (this.ParentAction == null)
            {
                return this;
            }

            return this.ParentAction.GetRootAction();
        }

        public virtual ActionStatusLabelProvider GetStatusLabelProvider()
        {
            return ActionStatusLabelProvider.GetStatusLabelProvider(this);
        }

        public virtual ActionStatusSummaryProvider GetStatusSummaryProvider()
        {
            return ActionStatusSummaryProvider.GetSummaryProvider(this);
        }

        private void GetSubActionMandatoryTransformers(Type type, List<Type> transformerTypes, List<Type> checkedTypes)
        {
            Attribute[] customAttributes = Attribute.GetCustomAttributes(type, typeof(SubActionTypesAttribute), true);
            for (int i = 0; i < (int)customAttributes.Length; i++)
            {
                foreach (Type subActionType in ((SubActionTypesAttribute)customAttributes[i]).SubActionTypes)
                {
                    if (checkedTypes.Contains(subActionType))
                    {
                        continue;
                    }

                    this.GetMandatoryTransformers(subActionType, transformerTypes, checkedTypes);
                }
            }
        }

        private void GetSubActionSupportedDefinitions(Type type, List<ITransformerDefinition> supportedDefinitions,
            List<Type> checkedTypes)
        {
            Attribute[] customAttributes = Attribute.GetCustomAttributes(type, typeof(SubActionTypesAttribute), true);
            for (int i = 0; i < (int)customAttributes.Length; i++)
            {
                foreach (Type subActionType in ((SubActionTypesAttribute)customAttributes[i]).SubActionTypes)
                {
                    if (checkedTypes.Contains(subActionType))
                    {
                        continue;
                    }

                    this.GetSupportedDefinitionsFromType(subActionType, supportedDefinitions, checkedTypes);
                }
            }
        }

        protected virtual List<ITransformerDefinition> GetSupportedDefinitions()
        {
            return new List<ITransformerDefinition>();
        }

        private void GetSupportedDefinitionsFromType(Type type, List<ITransformerDefinition> supportedDefinitions,
            List<Type> checkedTypes)
        {
            Metalogix.Actions.Action item = ActionCollection.AvailableActions[type];
            foreach (ITransformerDefinition transformerDefinition in (item != null
                         ? item.GetSupportedDefinitions()
                         : new List<ITransformerDefinition>()))
            {
                if (supportedDefinitions.Contains(transformerDefinition))
                {
                    continue;
                }

                supportedDefinitions.Add(transformerDefinition);
            }

            checkedTypes.Add(type);
            this.GetSubActionSupportedDefinitions(type, supportedDefinitions, checkedTypes);
        }

        protected MLLicenseCommon GetValidatedCommonLicense()
        {
            if (this.m_license == null)
            {
                this.UpdateLicense();
            }

            return this.m_license as MLLicenseCommon;
        }

        private bool HasLoggedActionBlocked()
        {
            return this._actionBlockedLogItem != null;
        }

        public static bool InterfaceFilter(Type typeObj, object criteriaObj)
        {
            if (typeObj.ToString() == criteriaObj.ToString())
            {
                return true;
            }

            return false;
        }

        private void LicensingThreadExecute(object licenseManualResetEvent)
        {
            try
            {
                ManualResetEvent manualResetEvent = (ManualResetEvent)licenseManualResetEvent;
                bool flag = false;
                bool updateLicensing = this.UpdateLicensing;
                bool flag1 = false;
                while (updateLicensing)
                {
                    this.SaveLicenseData();
                    lock (this.m_lockObjectThreadSync)
                    {
                        flag1 = (manualResetEvent.SafeWaitHandle.IsClosed
                            ? false
                            : !manualResetEvent.SafeWaitHandle.IsInvalid);
                    }

                    if (!flag1)
                    {
                        break;
                    }

                    flag = manualResetEvent.WaitOne(60000, false);
                    bool mBUpdateLicensing = this.m_bUpdateLicensing;
                    updateLicensing = mBUpdateLicensing;
                    if (!mBUpdateLicensing || flag)
                    {
                        break;
                    }

                    if (flag)
                    {
                        continue;
                    }

                    lock (this.m_lockObjectThreadSync)
                    {
                        if (manualResetEvent.SafeWaitHandle.IsInvalid || manualResetEvent.SafeWaitHandle.IsClosed)
                        {
                            break;
                        }
                        else
                        {
                            manualResetEvent.Reset();
                        }
                    }
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                string str = string.Format("LicensingTimer():Error Licensing the Running Action '{0}'{1}{2}", this.Name,
                    Environment.NewLine, exception.Message);
                this.Cancel();
                GlobalServices.ErrorHandler.HandleException(str, exception);
            }
        }

        protected virtual void LogAdditionalTelemetryDetails(IXMLAbleList source, IXMLAbleList target)
        {
        }

        private void On_ActionBlocked(object sender, ActionBlockerEventArgs e)
        {
            if (this.ActionBlocked != null)
            {
                this.ActionBlocked(sender, e);
            }
        }

        private void On_MLLicenseProvider_LicenseUpdated(object sender, EventArgs e)
        {
            this.UpdateLicense();
            this.m_bLicensingUnitFetchedSuccessfully = false;
        }

        private void On_OperationFinished(LogItem operation)
        {
            this.FireOperationFinished(operation);
        }

        private void On_OperationStarted(LogItem operation)
        {
            this.FireOperationStarted(operation);
        }

        private void On_OperationUpdated(LogItem operation)
        {
            this.FireOperationUpdated(operation);
        }

        private void On_SourceLinkChanged(string link)
        {
            this.FireSourceLinkChanged(link);
        }

        private void On_SubAction_Added(Metalogix.Actions.Action subAction)
        {
            this.ConnectSubaction(subAction);
        }

        private void On_SubAction_Removed(Metalogix.Actions.Action subAction)
        {
            this.DisconnectSubaction(subAction);
        }

        private void On_TargetLinkChanged(string link)
        {
            this.FireTargetLinkChanged(link);
        }

        public void Pause()
        {
            if (this.Status == ActionStatus.Running)
            {
                this.SetStatus(ActionStatus.Paused);
            }
        }

        private void ProcessTaskErrorError(WorkerThread thread, string taskName, Exception exception_0)
        {
            string str = string.Concat("Error running task: ", taskName);
            LogItem logItem = new LogItem(str, null, null, null, ActionOperationStatus.Failed);
            if (exception_0 is TargetInvocationException)
            {
                exception_0 = (TargetInvocationException)exception_0.InnerException;
            }

            logItem.Exception = exception_0;
            this.FireOperationStarted(logItem);
            this.FireOperationFinished(logItem);
        }

        public void Resume()
        {
            if (this.Status == ActionStatus.Paused)
            {
                this.SetStatus(ActionStatus.Running);
            }
        }

        public virtual void Run(IXMLAbleList source, IXMLAbleList target)
        {
            try
            {
                FeatureMessage expr_0A = new FeatureMessage("Action.Run");
                expr_0A.Event = new EventInformation
                {
                    Code = "Feature.Start"
                };
                expr_0A.Binary = BinaryInfo.Get();
                expr_0A.AddToGroup(Access.FeatureGroup("Action.Run", true));
                expr_0A.AddExtendedKeys(this.m_extendedTelemetryData);
                Access.Send(expr_0A);
                if (this.Status == ActionStatus.Running)
                {
                    throw new Exception("Action is already running");
                }

                if (!this.EnabledOn(source, target))
                {
                    throw new Exception(string.Format("{0} is not enabled in this context", this.Name));
                }

                if (this.RequiresWriteAccess)
                {
                    string message = null;
                    foreach (Node node in target)
                    {
                        if (!node.CheckWriteAccess(out message))
                        {
                            throw new Exception(message);
                        }
                    }
                }

                this.SetStatus(ActionStatus.Running);
                this.FireActionStarted((source != null) ? source.ToString() : null,
                    (target != null) ? target.ToString() : null);
                this.SetUp();
                Thread thread = null;
                ManualResetEvent manualResetEvent = null;
                EventHandler eventHandler = null;
                List<Node> list = new List<Node>();
                try
                {
                    bool flag = false;
                    try
                    {
                        this.UpdateLicense();
                        eventHandler = new EventHandler(this.On_MLLicenseProvider_LicenseUpdated);
                        MLLicenseProvider.LicenseUpdated += eventHandler;
                        if (this.EnableLicenseTracking && this.m_license != null && this.m_license is MLLicenseCommon)
                        {
                            this.m_bUpdateLicensing = true;
                            manualResetEvent = new ManualResetEvent(false);
                            thread = new Thread(new ParameterizedThreadStart(this.LicensingThreadExecute));
                            thread.Name = string.Format("{0}_{1}", this.Name,
                                DateTime.Now.ToString("yyyyMMdd_HHmmssfff"));
                            thread.Start(manualResetEvent);
                        }

                        try
                        {
                            if (source != null)
                            {
                                foreach (object current in source)
                                {
                                    Node node2 = current as Node;
                                    if (node2 != null)
                                    {
                                        node2.SetAsActionSource();
                                        list.Add(node2);
                                        IBugReportable bugReportable = node2.Connection as IBugReportable;
                                        if (bugReportable != null)
                                        {
                                            bugReportable.SetServerAndAdapterDetails();
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            this.SetStatus(ActionStatus.Aborted);
                            LogItem logItem = new LogItem("Obtaining Source Lock", null, null, null,
                                ActionOperationStatus.Failed);
                            logItem.Exception = ex;
                            logItem.Information = ex.Message;
                            logItem.Details = ex.StackTrace;
                            this.FireOperationStarted(logItem);
                            this.FireOperationFinished(logItem);
                            goto IL_4B2;
                        }

                        this.SetServerAndAdapterDetailsForBugReportingTool(target);
                        try
                        {
                            this.ValidateMandatoryTransformers();
                            this.FireActionValidated(source, target);
                            this.RunBeforeAction(source, target);
                            this.CreateThreadManager();
                            this.RunAction(source, target);
                            flag = true;
                        }
                        finally
                        {
                            this.DisposeThreadManager();
                            this.m_extendedTelemetryData = new Dictionary<string, string>();
                            this.RunAfterAction(source, target);
                        }
                    }
                    finally
                    {
                        if (flag)
                        {
                            this.RunPostOp(source, target);
                        }

                        this.LogAdditionalTelemetryDetails(source, target);
                        this.SetTelemetryData(source, target);
                    }
                }
                catch (Exception ex2)
                {
                    if (Assembly.GetEntryAssembly() == null)
                    {
                        throw ex2;
                    }

                    string text = ex2.Message;
                    if (text.Contains("Dependency feature") && text.EndsWith("is not activated at this scope."))
                    {
                        throw new ConditionalDetailException(
                            "A feature required for your action to succeed is not activated on your SharePoint: " +
                            text);
                    }

                    if (!(ex2 is ObjectDisposedException))
                    {
                        text = "Error running action: " + text;
                        GlobalServices.ErrorHandler.HandleException(text, ex2);
                        LogItem logItem2 = new LogItem("Error", null, null, null, ActionOperationStatus.Failed);
                        logItem2.Exception = ex2;
                        logItem2.Information = ex2.Message;
                        logItem2.Details = ex2.StackTrace;
                        this.FireOperationStarted(logItem2);
                        this.FireOperationFinished(logItem2);
                    }
                }
                finally
                {
                    try
                    {
                        if (this.Status == ActionStatus.Aborting)
                        {
                            this.SetStatus(ActionStatus.Aborted);
                        }

                        if (this.Status != ActionStatus.Aborted)
                        {
                            this.SetStatus(ActionStatus.Done);
                        }

                        foreach (Node current2 in list)
                        {
                            current2.ResetActionSourceState();
                        }

                        this.CleanUpSubActions();
                        this.CleanUp();
                        this.m_bUpdateLicensing = false;
                        if (thread != null)
                        {
                            lock (this.m_lockObjectThreadSync)
                            {
                                manualResetEvent.Set();
                                manualResetEvent.Close();
                            }
                        }

                        this.SaveLicenseData();
                    }
                    catch (Exception ex3)
                    {
                        string message2 = "An unexpected error occured while finalizing action: " + ex3.Message +
                                          Environment.NewLine +
                                          "This may cause unexpected behaviour when interacting with the connections used. A global refresh is recommended.";
                        GlobalServices.ErrorHandler.HandleException(message2, ex3);
                        LogItem logItem3 = new LogItem("Error", null, null, null, ActionOperationStatus.Failed);
                        logItem3.Exception = ex3;
                        logItem3.Information = ex3.Message;
                        logItem3.Details = ex3.StackTrace;
                        this.FireOperationStarted(logItem3);
                        this.FireOperationFinished(logItem3);
                    }
                    finally
                    {
                        if (eventHandler != null)
                        {
                            MLLicenseProvider.LicenseUpdated -= eventHandler;
                        }

                        this.m_license = null;
                        GC.Collect();
                        this.FireActionFinished();
                    }
                }

                IL_4B2: ;
            }
            finally
            {
                FeatureMessage expr_4BE = new FeatureMessage("Action.Run");
                expr_4BE.Event = new EventInformation
                {
                    Code = "Feature.Stop"
                };
                expr_4BE.Binary = BinaryInfo.Get();
                expr_4BE.AddToGroup(Access.FeatureGroup("Action.Run", false));
                expr_4BE.AddExtendedKeys(this.m_extendedTelemetryData);
                Access.Send(expr_4BE);
            }
        }

        protected virtual void Run(object oLocationValues)
        {
            if (!(oLocationValues is ActionLocations))
            {
                throw new Exception("Action location parameter is invalid");
            }

            this.Run(((ActionLocations)oLocationValues).Source, ((ActionLocations)oLocationValues).Target);
        }

        protected abstract void RunAction(IXMLAbleList source, IXMLAbleList target);

        protected virtual void RunAfterAction(IXMLAbleList source, IXMLAbleList target)
        {
        }

        public void RunAsSubAction(string sMethodName, object[] oParams, ActionContext actionContext)
        {
            IXMLAbleList sources;
            IXMLAbleList targets;
            string str;
            string str1;
            string str2;
            string str3;
            if (actionContext != null)
            {
                sources = actionContext.Sources;
            }
            else
            {
                sources = null;
            }

            IXMLAbleList xMLAbleLists = sources;
            if (actionContext != null)
            {
                targets = actionContext.Targets;
            }
            else
            {
                targets = null;
            }

            IXMLAbleList xMLAbleLists1 = targets;
            try
            {
                if (!this.EnabledOn(xMLAbleLists, xMLAbleLists1))
                {
                    throw new Exception(string.Format("{0} is not enabled in this context", this.Name));
                }

                MethodInfo method =
                    this.GetType().GetMethod(sMethodName, BindingFlags.Instance | BindingFlags.NonPublic);
                if (method == null)
                {
                    throw new ArgumentException(string.Concat("Action method not found: ", sMethodName));
                }

                if (!this.RunSubActionAsync)
                {
                    method.Invoke(this, oParams);
                }
                else
                {
                    this.ThreadManager.RequestAsynchronousOperation(oParams, method);
                }
            }
            catch (TargetInvocationException targetInvocationException1)
            {
                TargetInvocationException targetInvocationException = targetInvocationException1;
                string str4 = string.Format("SubAction: {0}", this.Name);
                if (xMLAbleLists != null)
                {
                    str = xMLAbleLists.ToString();
                }
                else
                {
                    str = null;
                }

                if (xMLAbleLists1 != null)
                {
                    str1 = xMLAbleLists1.ToString();
                }
                else
                {
                    str1 = null;
                }

                LogItem logItem = new LogItem(str4, null, str, str1, ActionOperationStatus.Warning)
                {
                    Exception = targetInvocationException.InnerException
                };
                this.FireOperationStarted(logItem);
                this.FireOperationFinished(logItem);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                string str5 = string.Format("SubAction: {0}", this.Name);
                if (xMLAbleLists != null)
                {
                    str2 = xMLAbleLists.ToString();
                }
                else
                {
                    str2 = null;
                }

                if (xMLAbleLists1 != null)
                {
                    str3 = xMLAbleLists1.ToString();
                }
                else
                {
                    str3 = null;
                }

                LogItem logItem1 = new LogItem(str5, null, str2, str3, ActionOperationStatus.Warning)
                {
                    Exception = exception
                };
                this.FireOperationStarted(logItem1);
                this.FireOperationFinished(logItem1);
            }
        }

        public void RunAsSubAction(object[] oParams, ActionContext actionContext,
            ThreadedOperationDelegate operationDelegate = null)
        {
            IXMLAbleList sources;
            IXMLAbleList targets;
            string str;
            string str1;
            string str2;
            string str3;
            if (actionContext != null)
            {
                sources = actionContext.Sources;
            }
            else
            {
                sources = null;
            }

            IXMLAbleList xMLAbleLists = sources;
            if (actionContext != null)
            {
                targets = actionContext.Targets;
            }
            else
            {
                targets = null;
            }

            IXMLAbleList xMLAbleLists1 = targets;
            if (operationDelegate == null)
            {
                Metalogix.Actions.Action action = this;
                operationDelegate = new ThreadedOperationDelegate(action.RunOperation);
            }

            try
            {
                if (!this.EnabledOn(xMLAbleLists, xMLAbleLists1))
                {
                    throw new Exception(string.Format("{0} is not enabled in this context", this.Name));
                }

                if (!this.CheckForAbort())
                {
                    if (!this.RunSubActionAsync)
                    {
                        operationDelegate(oParams);
                    }
                    else
                    {
                        this.ThreadManager.RequestAsynchronousOperation(oParams, operationDelegate);
                    }
                }
            }
            catch (TargetInvocationException targetInvocationException1)
            {
                TargetInvocationException targetInvocationException = targetInvocationException1;
                string str4 = string.Format("SubAction: {0}", this.Name);
                if (xMLAbleLists != null)
                {
                    str = xMLAbleLists.ToString();
                }
                else
                {
                    str = null;
                }

                if (xMLAbleLists1 != null)
                {
                    str1 = xMLAbleLists1.ToString();
                }
                else
                {
                    str1 = null;
                }

                LogItem logItem = new LogItem(str4, null, str, str1, ActionOperationStatus.Warning)
                {
                    Exception = targetInvocationException.InnerException
                };
                this.FireOperationStarted(logItem);
                this.FireOperationFinished(logItem);
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                string str5 = string.Format("SubAction: {0}", this.Name);
                if (xMLAbleLists != null)
                {
                    str2 = xMLAbleLists.ToString();
                }
                else
                {
                    str2 = null;
                }

                if (xMLAbleLists1 != null)
                {
                    str3 = xMLAbleLists1.ToString();
                }
                else
                {
                    str3 = null;
                }

                LogItem logItem1 = new LogItem(str5, null, str2, str3, ActionOperationStatus.Warning)
                {
                    Exception = exception
                };
                this.FireOperationStarted(logItem1);
                this.FireOperationFinished(logItem1);
            }
        }

        public void RunAsync(IXMLAbleList source, IXMLAbleList target)
        {
            ActionLocations actionLocation = new ActionLocations(source, target);
            Metalogix.Actions.Action action = this;
            (new Thread(new ParameterizedThreadStart(action.Run))).Start(actionLocation);
        }

        protected virtual void RunBeforeAction(IXMLAbleList source, IXMLAbleList target)
        {
        }

        protected virtual void RunOperation(object[] oParams)
        {
        }

        protected virtual void RunPostOp(IXMLAbleList source, IXMLAbleList target)
        {
        }

        private void SaveLicenseData()
        {
            MLLicenseCommon mLicense = this.m_license as MLLicenseCommon;
            if (mLicense != null)
            {
                mLicense.SaveUsageData();
            }
        }

        private void SendTelemetryReportData(IXMLAbleList source, IXMLAbleList target)
        {
            try
            {
                if (source != null && source.Count != 0)
                {
                    if (target != null && target.Count != 0)
                    {
                        StringWriter stringWriter = new StringWriter();
                        XmlWriterSettings xmlWriterSetting = new XmlWriterSettings()
                        {
                            OmitXmlDeclaration = true
                        };
                        using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, xmlWriterSetting))
                        {
                            xmlWriter.WriteStartElement("TelemetryReport");
                            this.WriteTelemetryReportAttributes(xmlWriter);
                            this.WriteAdapterInformation("SourceInfo", source[0] as Node, xmlWriter);
                            this.WriteAdapterInformation("TargetInfo", target[0] as Node, xmlWriter);
                            xmlWriter.WriteEndElement();
                        }

                        this.m_extendedTelemetryData.Add("JobSummary", stringWriter.ToString());
                    }
                }
            }
            catch (Exception exception)
            {
                Trace.WriteLine(string.Format("Failed to send telemetry report data. Error : {0}", exception));
            }
        }

        protected virtual void SetServerAndAdapterDetailsForBugReportingTool(IXMLAbleList target)
        {
            if (target != null)
            {
                foreach (object obj in target)
                {
                    Node node = obj as Node;
                    if (node == null)
                    {
                        continue;
                    }

                    IBugReportable connection = node.Connection as IBugReportable;
                    if (connection == null)
                    {
                        continue;
                    }

                    connection.SetServerAndAdapterDetails();
                }
            }
        }

        internal void SetStatus(ActionStatus status)
        {
            this.m_status = status;
            this.SubActions.SetStatus(status);
            this.FireStatusChanged(this.m_status);
        }

        private void SetTelemetryData(IXMLAbleList source, IXMLAbleList target)
        {
            string versionNumber;
            bool? isSharePointOnline;
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(this.ToXML());
                XmlElement documentElement = xmlDocument.DocumentElement;
                bool flag = false;
                bool flag1 = false;
                string empty = string.Empty;
                if (documentElement != null)
                {
                    if (source != null && source.Count > 0)
                    {
                        Node item = source[0] as Node;
                        if (item != null && item.Connection != null)
                        {
                            versionNumber = item.Connection.VersionNumber;
                            empty = item.Connection.AdapterName;
                            isSharePointOnline = item.Connection.IsSharePointOnline;
                            documentElement.AddOrUpdateAttribute("SourceAdapter", empty);
                            documentElement.AddOrUpdateAttribute("SourceVersion", versionNumber);
                            if (isSharePointOnline.HasValue)
                            {
                                documentElement.AddOrUpdateAttribute("IsSourceSharePointOnline",
                                    isSharePointOnline.ToString());
                            }

                            flag1 = true;
                        }
                    }

                    if (target != null && target.Count > 0)
                    {
                        Node node = target[0] as Node;
                        if (node != null && node.Connection != null)
                        {
                            versionNumber = node.Connection.VersionNumber;
                            empty = node.Connection.AdapterName;
                            isSharePointOnline = node.Connection.IsSharePointOnline;
                            documentElement.AddOrUpdateAttribute("TargetAdapter", empty);
                            documentElement.AddOrUpdateAttribute("TargetVersion", versionNumber);
                            if (isSharePointOnline.HasValue)
                            {
                                documentElement.AddOrUpdateAttribute("IsTargetSharePointOnline",
                                    isSharePointOnline.ToString());
                            }

                            flag = true;
                        }
                    }
                }

                if (flag && flag1)
                {
                    this.m_extendedTelemetryData.Add(string.Concat(this.Name, "_IsRemoteJob"),
                        this.IsRemoteJob.ToString());
                    this.m_extendedTelemetryData.Add(string.Concat(this.Name, "_IsUsingPowerShell"),
                        this.IsUsingPowerShell.ToString());
                    this.m_extendedTelemetryData.Add(string.Concat(this.Name, "_TargetAdapter"), empty);
                }

                if (flag || flag1)
                {
                    string text = Converter.TransformXmlToText(xmlDocument.OuterXml);
                    this.m_extendedTelemetryData.Add(this.Name, text);
                }
                else
                {
                    this.m_extendedTelemetryData.Add(this.Name, "Action Ended");
                }

                this.SendTelemetryReportData(source, target);
            }
            catch
            {
                this.m_extendedTelemetryData.Add(this.Name, "Action Ended");
            }
        }

        protected virtual void SetUp()
        {
        }

        private void StartActionBlockedLog(ILoggableActionBlocker blocker)
        {
            if (this._actionBlockedLogItem != null)
            {
                return;
            }

            this._actionBlockedLogItem = blocker.CreateLogItem();
            this.FireOperationStarted(this._actionBlockedLogItem);
        }

        private void StartActionBlockedLogUsingRootAction(ILoggableActionBlocker blocker)
        {
            Metalogix.Actions.Action rootAction = this.GetRootAction();
            lock (this.m_lockObjectThreadSync)
            {
                if (!rootAction.HasLoggedActionBlocked())
                {
                    rootAction.StartActionBlockedLog(blocker);
                }
            }
        }

        protected void SubAction_ActionStarted(Metalogix.Actions.Action sender, string sSourceString,
            string sTargetString)
        {
            if (this.ActionStarted != null)
            {
                this.ActionStarted(sender, sSourceString, sTargetString);
            }
        }

        public virtual string ToXML()
        {
            StringWriter stringWriter = new StringWriter(new StringBuilder(1024));
            this.ToXML(new XmlTextWriter(stringWriter));
            return stringWriter.ToString();
        }

        public virtual void ToXML(XmlWriter xmlTextWriter)
        {
            xmlTextWriter.WriteStartElement("Action");
            xmlTextWriter.WriteAttributeString("ActionType", this.GetType().AssemblyQualifiedName);
            if (this.Options != null)
            {
                this.Options.ToXML(xmlTextWriter);
            }

            xmlTextWriter.WriteEndElement();
        }

        private void UpdateLicense()
        {
            MLLicenseProviderCommon instance = MLLicenseProvider.Instance as MLLicenseProviderCommon;
            if (instance == null)
            {
                return;
            }

            this.m_license =
                instance.GetLicense(new LicenseContext(), typeof(Metalogix.Actions.Action), this, false) as MLLicense;
            if (this.m_license == null)
            {
                throw new MLLicenseException(Resources.Invalid_License);
            }
        }

        private void ValidateMandatoryTransformers()
        {
            if (this.Options == null)
            {
                return;
            }

            Type type = this.GetType();
            List<Type> types = new List<Type>();
            this.GetMandatoryTransformers(type, types, new List<Type>());
            foreach (ITransformer transformer in this.Options.Transformers)
            {
                if (types.Count <= 0)
                {
                    break;
                }

                Type type1 = transformer.GetType();
                if (!types.Contains(type1))
                {
                    continue;
                }

                types.Remove(type1);
            }

            if (types.Count > 0)
            {
                foreach (Type type2 in types)
                {
                    ITransformer transformer1 = Activator.CreateInstance(type2) as ITransformer;
                    if (transformer1 == null)
                    {
                        continue;
                    }

                    transformer1.ReadOnly = true;
                    this.Options.Transformers.Add(transformer1);
                }
            }
        }

        private void WriteAdapterInformation(string nodeName, Node node, XmlWriter writer)
        {
            if (node == null || node.Connection == null)
            {
                writer.WriteStartElement(nodeName);
                writer.WriteEndElement();
                return;
            }

            string versionNumber = node.Connection.VersionNumber;
            string adapterName = node.Connection.AdapterName;
            bool? isSharePointOnline = node.Connection.IsSharePointOnline;
            writer.WriteStartElement(nodeName);
            writer.WriteAttributeString("VersionNumber", versionNumber);
            writer.WriteAttributeString("AdapterName", adapterName);
            writer.WriteAttributeString("IsSharePointOnline",
                (isSharePointOnline.HasValue ? isSharePointOnline.ToString() : false.ToString()));
            writer.WriteEndElement();
        }

        private void WriteTelemetryReportAttributes(XmlWriter writer)
        {
            ActionDataCounter actionDataCounter = this._dataCounter;
            writer.WriteAttributeString("Action", this.Name);
            writer.WriteAttributeString("TotalDataUsed",
                (actionDataCounter == null ? "-1" : actionDataCounter.TotalDataUsed.ToString()));
            writer.WriteAttributeString("TotalItemsUsed",
                (actionDataCounter == null ? "-1" : actionDataCounter.TotalItemsUsed.ToString()));
        }

        public event ActionBlockerHandler ActionBlocked;

        public event ActionFinishedEventHandler ActionFinished;

        public event ActionStartedEventHandler ActionStarted;

        public event ActionValidatedEventHandler ActionValidated;

        public event ActionEventHandler OperationFinished;

        public event ActionEventHandler OperationStarted;

        public event ActionEventHandler OperationUpdated;

        public event ActionOptionsChangedHandler OptionsChanged;

        public event ActionLinkChangedHandler SourceLinkChanged;

        public event ActionStatusChangedHandler StatusChanged;

        public event ActionLinkChangedHandler TargetLinkChanged;
    }
}