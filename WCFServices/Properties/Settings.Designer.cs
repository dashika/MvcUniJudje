﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.225
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WCFServices.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MainPage\\(([0-9]{1,}.[0-9]{1,}|[0-9]{1,},[0-9]{1,}|[0-9]{0,})\\)")]
        public string RegExp_OutContestToMainPage {
            get {
                return ((string)(this["RegExp_OutContestToMainPage"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ArchivePage\\(([0-9]{1,}.[0-9]{1,}|[0-9]{1,},[0-9]{1,}|[0-9]{0,})\\)")]
        public string RegExp_OutContestToArchivePage {
            get {
                return ((string)(this["RegExp_OutContestToArchivePage"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("AdminPage\\(([0-9]{1,}.[0-9]{1,}|[0-9]{1,},[0-9]{1,}|[0-9]{0,})\\)")]
        public string RegExp_OutContestToAdminPage {
            get {
                return ((string)(this["RegExp_OutContestToAdminPage"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("newKey2000")]
        public string KeyForPasswordEncrypt {
            get {
                return ((string)(this["KeyForPasswordEncrypt"]));
            }
        }
    }
}
