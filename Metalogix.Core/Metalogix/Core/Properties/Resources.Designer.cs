﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Metalogix.Core.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Metalogix.Core.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The requested configuration variable has not been initialized..
        /// </summary>
        public static string ConfigValueDoesNotExist_Exception {
            get {
                return ResourceManager.GetString("ConfigValueDoesNotExist_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not allocate configuration variable. The provided name already exists with different parameters..
        /// </summary>
        public static string ConfigValueExists_Exception {
            get {
                return ResourceManager.GetString("ConfigValueExists_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find a valid path for configuration variables.
        /// </summary>
        public static string CouldNotFindConfigurationVariablesPath {
            get {
                return ResourceManager.GetString("CouldNotFindConfigurationVariablesPath", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error.
        /// </summary>
        public static string Error {
            get {
                return ResourceManager.GetString("Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error Loading File Data.
        /// </summary>
        public static string FileDataLoadingCaption {
            get {
                return ResourceManager.GetString("FileDataLoadingCaption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The data file &quot;{0}&quot; contains invalid data. Values in that file will be reset to defaults..
        /// </summary>
        public static string FileDataLoadingMessage {
            get {
                return ResourceManager.GetString("FileDataLoadingMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The data file &quot;{0}&quot; could not be saved.
        /// </summary>
        public static string FileDataSaveError {
            get {
                return ResourceManager.GetString("FileDataSaveError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provided link for an object of type {0} was null.
        /// </summary>
        public static string LinkForResolutionNull {
            get {
                return ResourceManager.GetString("LinkForResolutionNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot retrieve an object of type {0} using a resolver for type {1}.
        /// </summary>
        public static string LinkResolutionTypeMismatch {
            get {
                return ResourceManager.GetString("LinkResolutionTypeMismatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot store the given setting. No scope was provided.
        /// </summary>
        public static string NoScopeError {
            get {
                return ResourceManager.GetString("NoScopeError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Warning.
        /// </summary>
        public static string Warning {
            get {
                return ResourceManager.GetString("Warning", resourceCulture);
            }
        }
    }
}
