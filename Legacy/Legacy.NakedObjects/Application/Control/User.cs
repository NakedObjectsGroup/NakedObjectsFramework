// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.control.User
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using System.Collections;
using Legacy.NakedObjects.Application.ValueHolder;

// ReSharper disable InconsistentNaming

namespace Legacy.NakedObjects.Application.Control {
    //[JavaInterfaces("1;org/nakedobjects/application/TitledObject;")]
    public sealed class User : TitledObject {
        private const long serialVersionUID = 1;
        private readonly TextString name;
        private readonly ArrayList roles;
        private object rootObject;

        public User() {
            name = new TextString();
            roles = new ArrayList();
        }

        public User(string name)
            : this() {
            this.name.setValue(name);
        }

        public Title title() => name.title();

        public static string fieldOrder() => "name";

        public void aboutFieldDefault(FieldAbout about) => about.visibleOnlyToRole(Role.SYSADMIN);

        //[JavaFlags(17)]
        public TextString getName() => name;

        //[JavaFlags(17)]
        //public Vector getRoles() => this.roles;
        public ArrayList getRoles() => roles;

        //[JavaFlags(17)]
        public void addToRoles(Role role) {
            roles.Add(role);
            objectChanged();
        }

        //[JavaFlags(17)]
        public void removeFromRoles(Role role) {
            // ????
            roles.Add(role);
            objectChanged();
        }

        private void objectChanged() { }

        public object getRootObject() => rootObject;

        public void setRootObject(object rootObject) {
            this.rootObject = rootObject;
            objectChanged();
        }

        //[JavaFlags(4227077)]
        //[JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
        public new object MemberwiseClone() =>
            //User user = this;
            //ObjectImpl.clone((object) user);
            //return ((object) user).MemberwiseClone();
            null;

        //[JavaFlags(4227073)]
        public override string ToString() {
            //return ObjectImpl.jloToString((object)this);
            return "";
        }
    }
}