﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace NeoToppas.WPF.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.11.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool AUTOUPDATE_ENABLE {
            get {
                return ((bool)(this["AUTOUPDATE_ENABLE"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\\\PCD01851\\SharedFolder\\NeoToppasLatestVersion")]
        public string AUTOUPDATE_DATA_FOLDER {
            get {
                return ((string)(this["AUTOUPDATE_DATA_FOLDER"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Users\\l057277\\OneDrive - Topcon\\ドキュメント\\各種案件\\7.TOPPAS置き換え\\3.ラベルプリンタ\\ブラザーラベルプリンタ" +
            "\\テンプレート")]
        public string LABEL_TEMPLATE_DIRECTORY {
            get {
                return ((string)(this["LABEL_TEMPLATE_DIRECTORY"]));
            }
        }
    }
}
