﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace App.Resources.Domain {
    using System;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class OrderItem {
        
        private static System.Resources.ResourceManager resourceMan;
        
        private static System.Globalization.CultureInfo resourceCulture;
        
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal OrderItem() {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public static System.Resources.ResourceManager ResourceManager {
            get {
                if (object.Equals(null, resourceMan)) {
                    System.Resources.ResourceManager temp = new System.Resources.ResourceManager("App.Resources.Domain.OrderItem", typeof(OrderItem).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public static System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        public static string DetailNumber {
            get {
                return ResourceManager.GetString("DetailNumber", resourceCulture);
            }
        }
        
        public static string LengthMm {
            get {
                return ResourceManager.GetString("LengthMm", resourceCulture);
            }
        }
        
        public static string WidthMm {
            get {
                return ResourceManager.GetString("WidthMm", resourceCulture);
            }
        }
        
        public static string HeightMm {
            get {
                return ResourceManager.GetString("HeightMm", resourceCulture);
            }
        }
        
        public static string Area {
            get {
                return ResourceManager.GetString("Area", resourceCulture);
            }
        }
        
        public static string LinearMeter {
            get {
                return ResourceManager.GetString("LinearMeter", resourceCulture);
            }
        }
        
        public static string Amount {
            get {
                return ResourceManager.GetString("Amount", resourceCulture);
            }
        }
    }
}
