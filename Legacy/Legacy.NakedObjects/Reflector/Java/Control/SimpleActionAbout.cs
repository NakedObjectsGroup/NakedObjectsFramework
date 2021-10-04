// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.control.SimpleActionAbout
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using Legacy.NakedObjects.Application.Control;
using NakedFramework.Architecture.Component;

namespace Legacy.NakedObjects.Reflector.Java.Control {
    //[JavaInterfaces("1;org/nakedobjects/application/control/ActionAbout;")]
    public class SimpleActionAbout : AbstractAbout, ActionAbout {
        private const long serialVersionUID = 1;
        private readonly object[][] options;
        private object[] defaultValues;
        private string[] labels;
        private bool[] required;

        public SimpleActionAbout(ISession session, object @object, object[] parameters)
            : base(session, @object) {
            var length1 = parameters.Length;
            var length2 = length1;
            labels = new string[length2];
            var length3 = length1;
            defaultValues = new object[length3];
            var length4 = length1;
            required = new bool[length4];
            var length5 = length1;
            options = new object[length5][];
        }

        public virtual object[][] getOptions() => options;

        public virtual object[] getDefaultParameterValues() => defaultValues;

        public virtual string[] getParameterLabels() => labels;

        public virtual bool[] getRequired() => required;

        public virtual void setParameter(int index, object defaultValue) {
            checkParameter(index);
            defaultValues[index] = defaultValue;
        }

        public virtual void setParameter(int index, object[] options) {
            checkParameter(index);
            this.options[index] = options;
        }

        public virtual void setParameter(int index, string label) {
            checkParameter(index);
            labels[index] = label;
        }

        public virtual void setParameter(int index, bool required) {
            checkParameter(index);
            this.required[index] = required;
        }

        public virtual void setParameter(int index, string label, object defaultValue, bool required) {
            checkParameter(index);
            labels[index] = label;
            defaultValues[index] = defaultValue;
            this.required[index] = required;
        }

        public virtual void setParameters(object[] defaultValues) {
            if (this.defaultValues.Length != defaultValues.Length) {
                //throw new IllegalArgumentException(new StringBuilder().Append("Expected ").Append(this.defaultValues.Length).Append(" defaults but got ").Append(defaultValues.Length).ToString());
            }

            this.defaultValues = defaultValues;
        }

        public virtual void setParameters(string[] labels) {
            if (this.labels.Length != labels.Length) {
                // throw new IllegalArgumentException(new StringBuilder().Append("Expected ").Append(this.labels.Length).Append(" defaults but got ").Append(labels.Length).ToString());
            }

            this.labels = labels;
        }

        public virtual void setParameters(bool[] required) {
            if (labels.Length != labels.Length) {
                // throw new IllegalArgumentException(new StringBuilder().Append("Expected ").Append(labels.Length).append(" defaults but got ").Append(labels.Length).ToString());
            }

            this.required = required;
        }

        public virtual void unusable() => base.unusable("Cannot be invoked");

        private void checkParameter(int index) {
            if (index < 0 || index >= defaultValues.Length) {
                //  throw new IllegalArgumentException(new StringBuilder().Append("No parameter index ").Append(index).ToString());
            }
        }
    }
}