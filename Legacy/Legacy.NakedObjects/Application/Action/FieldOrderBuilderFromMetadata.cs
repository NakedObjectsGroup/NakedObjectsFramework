// Decompiled with JetBrains decompiler
// Type: sdm.systems.application.action.FieldOrderBuilderFromMetadata
// Assembly: sdm.systems.application, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 99352736-2C6C-4D77-94E7-E744D03082F7
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls.part02\bin\sdm.systems.application.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;

namespace Legacy.NakedObjects.Application.Action {
    public class FieldOrderBuilderFromMetadata {
        private readonly SortedList<decimal, string> myOrderedFields;

        public FieldOrderBuilderFromMetadata() => myOrderedFields = new SortedList<decimal, string>();

        public string BuildResult() {
            var stringBuilder = new StringBuilder();
            try {
                foreach (var str in myOrderedFields.Values) {
                    stringBuilder.Append(str + ", ");
                }
            }
            finally {
                //IEnumerator<string> enumerator;
                //enumerator?.Dispose();
            }

            return stringBuilder.ToString();
        }

        public void Add(MethodInfo method) {
            if (!method.Name.StartsWith("derive") | method.Name.StartsWith("get")) {
                return;
            }

            var str = RemoveActionPrefix(method);
            var customAttributes = method.GetCustomAttributes(typeof(FieldOrderAttribute), true);
            if (customAttributes.Length == 0) {
                return;
            }

            myOrderedFields.Add(Conversions.ToDecimal(((FieldOrderAttribute)customAttributes[0]).Order), str);
        }

        public void Add(PropertyInfo prop) {
            var customAttributes = prop.GetCustomAttributes(typeof(FieldOrderAttribute), true);
            if (customAttributes.Length == 0) {
                return;
            }

            var fieldOrderAttribute = (FieldOrderAttribute)customAttributes[0];
            try {
                myOrderedFields.Add(Conversions.ToDecimal(fieldOrderAttribute.Order), prop.Name);
            }
            catch (Exception ex) {
                ProjectData.SetProjectError(ex);
                Debug.Write("Error in property " + prop.Name);
                ProjectData.ClearProjectError();
            }
        }

        private string RemoveActionPrefix(MethodInfo withPrefix) => withPrefix.Name.StartsWith("derive") ? withPrefix.Name.Remove(0, 6) : withPrefix.Name.Remove(0, 3);
    }
}