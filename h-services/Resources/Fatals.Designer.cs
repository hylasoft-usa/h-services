﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Hylasoft.Services.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Fatals {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Fatals() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Hylasoft.Services.Resources.Fatals", typeof(Fatals).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Missing status information on &apos;{0}&apos; Service within the {1} Service..
        /// </summary>
        internal static string KeepAliveServiceMissingService {
            get {
                return ResourceManager.GetString("KeepAliveServiceMissingService", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A null service reference was passed into the {0} Service..
        /// </summary>
        internal static string KeepAliveServicePassedNullService {
            get {
                return ResourceManager.GetString("KeepAliveServicePassedNullService", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} was started without a functioning endpoint..
        /// </summary>
        internal static string NetworkSocketServiceStartedWithoutEndpoint {
            get {
                return ResourceManager.GetString("NetworkSocketServiceStartedWithoutEndpoint", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} was started without a functioning socket..
        /// </summary>
        internal static string NetworkSocketServiceStartedWithoutSocket {
            get {
                return ResourceManager.GetString("NetworkSocketServiceStartedWithoutSocket", resourceCulture);
            }
        }
    }
}
