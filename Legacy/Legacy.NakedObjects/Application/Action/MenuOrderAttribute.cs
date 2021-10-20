// Decompiled with JetBrains decompiler
// Type: sdm.systems.application.action.MenuOrderAttribute
// Assembly: sdm.systems.application, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 99352736-2C6C-4D77-94E7-E744D03082F7
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls.part02\bin\sdm.systems.application.dll

using System;
using Microsoft.VisualBasic.CompilerServices;

namespace Legacy.NakedObjects.Application.Action {
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class MenuOrderAttribute : Attribute, IComparable<MenuOrderAttribute> {
        public MenuOrderAttribute(string category = "") => Category = category;

        public string Category { get; set; }

        public string Order { get; set; }

        internal string Name { get; set; }

        public int CompareTo(MenuOrderAttribute other) {
            if (Operators.CompareString(other.Order, string.Empty, false) == 0 && Operators.CompareString(Order, string.Empty, false) == 0) {
                return Name.CompareTo(other.Name);
            }

            if (Operators.CompareString(other.Order, string.Empty, false) == 0) {
                return -1;
            }

            return Operators.CompareString(Order, string.Empty, false) == 0 ? 1 : Order.CompareTo(other.Order);
        }

        public string[] GetCategories() => Category.Split('/');
    }
}