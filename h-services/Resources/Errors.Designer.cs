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
    internal class Errors {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Errors() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Hylasoft.Services.Resources.Errors", typeof(Errors).Assembly);
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
        ///   Looks up a localized string similar to Subscribed tag was incomplete..
        /// </summary>
        internal static string DataTagNullError {
            get {
                return ResourceManager.GetString("DataTagNullError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A logical fallacy as occured in h-services..
        /// </summary>
        internal static string LogicalFallacy {
            get {
                return ResourceManager.GetString("LogicalFallacy", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find Address &apos;{0}&apos; in Server &apos;{1}&apos;..
        /// </summary>
        internal static string NetworkSocketAddressNotFound {
            get {
                return ResourceManager.GetString("NetworkSocketAddressNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No handler was defined for this endpoint..
        /// </summary>
        internal static string NoHandlerDefinedForSocketMonitor {
            get {
                return ResourceManager.GetString("NoHandlerDefinedForSocketMonitor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Provided schedule was null..
        /// </summary>
        internal static string ScheduleNullError {
            get {
                return ResourceManager.GetString("ScheduleNullError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} failed to continue..
        /// </summary>
        internal static string ServiceContinueFailed {
            get {
                return ResourceManager.GetString("ServiceContinueFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} failed to initialize..
        /// </summary>
        internal static string ServiceInitializationFailed {
            get {
                return ResourceManager.GetString("ServiceInitializationFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} failed to pause..
        /// </summary>
        internal static string ServicePauseFailed {
            get {
                return ResourceManager.GetString("ServicePauseFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} failed to start..
        /// </summary>
        internal static string ServiceStartFailed {
            get {
                return ResourceManager.GetString("ServiceStartFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} failed to stop..
        /// </summary>
        internal static string ServiceStopFailed {
            get {
                return ResourceManager.GetString("ServiceStopFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Timed out waiting on {0} to start..
        /// </summary>
        internal static string TimedOutWaitingOnStart {
            get {
                return ResourceManager.GetString("TimedOutWaitingOnStart", resourceCulture);
            }
        }
    }
}
