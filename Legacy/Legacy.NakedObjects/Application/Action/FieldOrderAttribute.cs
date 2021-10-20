// Decompiled with JetBrains decompiler
// Type: sdm.systems.application.action.FieldOrderAttribute
// Assembly: sdm.systems.application, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 99352736-2C6C-4D77-94E7-E744D03082F7
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls.part02\bin\sdm.systems.application.dll

using System;

namespace Legacy.NakedObjects.Application.Action {
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public sealed class FieldOrderAttribute : Attribute {
        private decimal myOrder;

        public FieldOrderAttribute(string order) => Order = order;

        public string Order {
            get => myOrder.ToString();
            set {
                if (!decimal.TryParse(value, out myOrder)) {
                    throw new InvalidCastException("Field Order must be a decimal");
                }
            }
        }
    }
}