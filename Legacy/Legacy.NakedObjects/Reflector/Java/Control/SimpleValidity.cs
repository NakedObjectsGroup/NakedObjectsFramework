// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.control.SimpleValidity
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using System.Text;
using Legacy.NakedObjects.Application.Control;

namespace Legacy.NakedObjects.Reflector.Java.Control {
    //[JavaInterfaces("1;org/nakedobjects/application/control/Validity;")]
    public class SimpleValidity : Validity {
        private readonly string fieldName;
        private readonly bool isEmpty;
        private StringBuilder unusableReason;

        public SimpleValidity(bool isEmpty, string fieldName) {
            this.isEmpty = isEmpty;
            this.fieldName = fieldName;
        }

        public virtual void cannotBeEmpty() => invalidOnCondition(isEmpty, new StringBuilder().Append(fieldName).Append(" must have a value").ToString());

        public virtual string getReason() => unusableReason.ToString();

        public virtual void invalid(string reason) {
            if (unusableReason == null) {
                unusableReason = new StringBuilder();
            }
            else {
                unusableReason.Append("; ");
            }

            unusableReason.Append(reason);
        }

        public virtual void invalidOnCondition(bool condition, string reason) {
            if (!condition) {
                return;
            }

            invalid(reason);
        }

        public virtual void invalidUnlessCondition(bool condition, string reason) {
            if (condition) {
                return;
            }

            invalid(reason);
        }

        public virtual bool isValid() => unusableReason == null;

        //[JavaFlags(4227077)]
        //[JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
        public new virtual object MemberwiseClone() {
            //var simpleValidity = this;
            //ObjectImpl.clone((object)simpleValidity);
            //return ((object)simpleValidity).MemberwiseClone();
            return null;
        }

        //[JavaFlags(4227073)]
        public override string ToString() {
            //return ObjectImpl.jloToString((object)this);
            return "";
        }
    }
}