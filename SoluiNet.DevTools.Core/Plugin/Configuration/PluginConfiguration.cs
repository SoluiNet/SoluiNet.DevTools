﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// Dieser Quellcode wurde automatisch generiert von xsd, Version=4.7.3081.0.
// 
namespace SoluiNet.DevTools.Core.Plugin.Configuration {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3081.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="SoluiNet.PluginConfigurationType", Namespace="http://solui.net/PluginConfiguration.xsd")]
    [System.Xml.Serialization.XmlRootAttribute("SoluiNet.PluginConfiguration", Namespace="http://solui.net/PluginConfiguration.xsd", IsNullable=false)]
    public partial class SoluiNetPluginConfigurationType {
        
        private SoluiNetConfigurationEntryType[] soluiNetConfigurationEntryField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("SoluiNet.ConfigurationEntry")]
        public SoluiNetConfigurationEntryType[] SoluiNetConfigurationEntry {
            get {
                return this.soluiNetConfigurationEntryField;
            }
            set {
                this.soluiNetConfigurationEntryField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3081.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="SoluiNet.ConfigurationEntryType", Namespace="http://solui.net/PluginConfiguration.xsd")]
    public partial class SoluiNetConfigurationEntryType {
        
        private object itemField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("SoluiNet.Installation", typeof(SoluiNetInstallationType))]
        [System.Xml.Serialization.XmlElementAttribute("SoluiNet.Plugin", typeof(SoluiNetPluginEntryType))]
        public object Item {
            get {
                return this.itemField;
            }
            set {
                this.itemField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3081.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="SoluiNet.InstallationType", Namespace="http://solui.net/PluginConfiguration.xsd")]
    public partial class SoluiNetInstallationType {
        
        private SoluiNetPluginEntryType[] soluiNetPluginField;
        
        private string pathField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("SoluiNet.Plugin")]
        public SoluiNetPluginEntryType[] SoluiNetPlugin {
            get {
                return this.soluiNetPluginField;
            }
            set {
                this.soluiNetPluginField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string path {
            get {
                return this.pathField;
            }
            set {
                this.pathField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3081.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="SoluiNet.PluginEntryType", Namespace="http://solui.net/PluginConfiguration.xsd")]
    public partial class SoluiNetPluginEntryType {
        
        private bool enabledField;
        
        private bool enabledFieldSpecified;
        
        private string nameField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool enabled {
            get {
                return this.enabledField;
            }
            set {
                this.enabledField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool enabledSpecified {
            get {
                return this.enabledFieldSpecified;
            }
            set {
                this.enabledFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
    }
}
