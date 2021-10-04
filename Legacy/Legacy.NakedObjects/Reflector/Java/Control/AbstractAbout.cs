// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.control.AbstractAbout
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using System.Text;
using Legacy.NakedObjects.Application.Control;
using Legacy.NakedObjects.Object.Control;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Core.Reflect;

// ReSharper disable InconsistentNaming

namespace Legacy.NakedObjects.Reflector.Java.Control {
    //[JavaInterfaces("1;org/nakedobjects/object/control/Hint;")]
    public abstract class AbstractAbout : Hint {
        private const long serialVersionUID = 1;
        private StringBuilder _debug;
        private string description;
        private StringBuilder invalidReason;
        private bool isAccessible;
        private string name;
        private readonly ISession session;
        private readonly StatefulObject statefulObject;
        private StringBuilder unusableReason;

        public AbstractAbout(ISession session, object @object) {
            _debug = new StringBuilder();
            this.session = session;
            isAccessible = true;
            description = "";
            if (!(@object is StatefulObject)) {
                return;
            }

            statefulObject = (StatefulObject)@object;
        }

        public virtual IConsent canAccess() => ConsentAbstract.GetAllow(isAccessible);

        public virtual IConsent canUse() => unusableReason == null ? new Allow(getDescription()) : new Veto(unusableReason.ToString());

        public virtual IConsent isValid() => invalidReason == null ? Allow.Default : new Veto(invalidReason.ToString());

        public virtual string debug() => _debug.ToString();

        public virtual string getDescription() => description;

        public virtual string getName() => name;

        private bool currentUserHasRole(Role role) {
            //return ((SimpleSession)session).hasRole(role);

            return session.Principal.IsInRole(role.getName().ToString());
        }

        //[JavaFlags(4)]
        public virtual void invisible() {
            concatDebug("unconditionally invisible");
            vetoAccess();
        }

        private void concatDebug(string @string) {
            _debug.Append(_debug.Length <= 0 ? "" : "; ");
            _debug.Append(@string);
        }

        //[JavaFlags(4)]
        public virtual void invisibleToUser(User user) {
            concatDebug(new StringBuilder().Append("Invisible to user ").Append(user).ToString());
            if (getCurrentUser() != user) {
                return;
            }

            vetoAccess();
        }

        //[JavaFlags(4)]
        public virtual void invisibleToUsers(User[] users) {
            concatDebug("Invisible to users ");
            for (var index = 0; index < users.Length; ++index) {
                _debug.Append(index <= 0 ? new StringBuilder().Append("").Append(users[index].getName()).ToString() : ". ");
            }

            for (var index = 0; index < users.Length; ++index) {
                if (getCurrentUser() == users[index]) {
                    vetoAccess();
                    break;
                }
            }
        }

        public virtual void setDescription(string description) => this.description = description;

        public virtual void setName(string name) => this.name = name;

        private bool stateIsSameAs(State state) {
            if (statefulObject != null) {
                return statefulObject.getState().Equals(state);
            }

            //throw new IllegalStateException("Cannot check state of object.  About not instantiated with object reference.");
            return false;
        }

        //[JavaFlags(4)]
        public virtual void unusable(string reason) {
            concatDebug("Unconditionally unusable");
            vetoUse(reason);
        }

        //[JavaFlags(4)]
        public virtual void unusableByUser(User user) {
            if (getCurrentUser() != user) {
                return;
            }

            vetoUse("Not available to current user");
        }

        private User getCurrentUser() {
            //return ((SimpleSession)session).getName();
            return null;
        }

        //[JavaFlags(4)]
        public virtual void unusableByUsers(User[] users) {
            var flag = true;
            for (var index = 0; index < users.Length; ++index) {
                if (users[index] != null) {
                    flag = flag && getCurrentUser() != users[index];
                }
            }

            if (flag) {
                return;
            }

            vetoUse("Not available to current user");
        }

        //[JavaFlags(4)]
        public virtual void unusableInState(State state) {
            concatDebug(new StringBuilder().Append("Unusable in state ").Append(state).ToString());
            if (!stateIsSameAs(state)) {
                return;
            }

            vetoUse("Unusable when object is in its current State");
        }

        //[JavaFlags(4)]
        public virtual void unusableInStates(State[] states) {
            var flag = true;
            var str = "";
            for (var index = 0; index < states.Length; ++index) {
                if (states[index] != null) {
                    flag = flag && !stateIsSameAs(states[index]);
                    str = new StringBuilder().Append(str).Append(states[index]).ToString();
                    if (index < states.Length) {
                        str = new StringBuilder().Append(str).Append(", ").ToString();
                    }
                }
            }

            if (flag) {
                return;
            }

            vetoUse(new StringBuilder().Append("Unusable when object is in any of these states: ").Append(str).ToString());
        }

        //[JavaFlags(4)]
        public virtual void unusableOnCondition(bool conditionMet, string reasonNotMet) {
            concatDebug(new StringBuilder().Append("Conditionally unusable ").Append(reasonNotMet).ToString());
            if (!conditionMet) {
                return;
            }

            vetoUse(reasonNotMet);
        }

        // [JavaFlags(4)]
        public virtual void usableOnlyByRole(Role role) {
            concatDebug(new StringBuilder().Append("Usable only for role ").Append(role.getName()).ToString());
            if (currentUserHasRole(role)) {
                return;
            }

            vetoUse("User does not have the appropriate role");
        }

        //[JavaFlags(4)]
        public virtual void usableOnlyByRoles(Role[] roles) {
            var flag = false;
            for (var index = 0; index < roles.Length; ++index) {
                if (roles[index] != null) {
                    flag = flag || currentUserHasRole(roles[index]);
                }
            }

            if (flag) {
                return;
            }

            vetoUse("User does not have the appropriate role");
        }

        //[JavaFlags(4)]
        public virtual void usableOnlyByUser(User user) {
            if (getCurrentUser() == user) {
                return;
            }

            vetoUse("Not available to current user");
        }

        //[JavaFlags(4)]
        public virtual void usableOnlyByUsers(User[] users) {
            var flag = false;
            for (var index = 0; index < users.Length; ++index) {
                if (users[index] != null) {
                    flag = flag || getCurrentUser() == users[index];
                }
            }

            if (flag) {
                return;
            }

            vetoUse("Not available to current user");
        }

        //[JavaFlags(4)]
        public virtual void usableOnlyInState(State state) {
            if (stateIsSameAs(state)) {
                return;
            }

            vetoUse(new StringBuilder().Append("Usable only when object is in the state: ").Append(state).ToString());
        }

        // [JavaFlags(4)]
        public virtual void usableOnlyInStates(State[] states) {
            concatDebug("usable only to certain roles");
            var flag = false;
            var stringBuffer = new StringBuilder();
            for (var index = 0; index < states.Length; ++index) {
                if (states[index] != null) {
                    flag = flag || stateIsSameAs(states[index]);
                    stringBuffer.Append((object)stringBuffer);
                    stringBuffer.Append(index <= 0 ? "" : ", ");
                    stringBuffer.Append(states[index]);
                }
            }

            if (flag) {
                return;
            }

            vetoUse(new StringBuilder().Append("Usable only when object is in any of these states: ").Append((object)stringBuffer).ToString());
        }

        //[JavaFlags(4)]
        public virtual void vetoAccess() {
            concatDebug("Access unconditionally vetoed");
            isAccessible = false;
        }

        //[JavaFlags(4)]
        public virtual void vetoUse(string reason) {
            concatDebug(new StringBuilder().Append("Use unconditionally vetoed; ").Append(reason).ToString());
            if (unusableReason == null) {
                unusableReason = new StringBuilder();
            }
            else {
                unusableReason.Append("; ");
            }

            unusableReason.Append(reason);
        }

        //[JavaFlags(4)]
        public virtual void visibleOnlyToRole(Role role) {
            //int length = 1;
            //Role[] roles = length >= 0 ? new Role[length] : throw new NegativeArraySizeException();
            //roles[0] = role;
            visibleOnlyToRoles(new[] { role });
        }

        //[JavaFlags(4)]
        public virtual void visibleOnlyToRoles(Role[] roles) {
            concatDebug("Visible only to roles ");
            var flag = false;
            for (var index = 0; index < roles.Length; ++index) {
                if (roles[index] != null) {
                    _debug.Append(index <= 0 ? new StringBuilder().Append("").Append(roles[index].getName()).ToString() : ", ");
                    flag = flag || currentUserHasRole(roles[index]);
                }
            }

            if (flag) {
                return;
            }

            vetoAccess();
        }

        //[JavaFlags(4)]
        public virtual void visibleOnlyToUser(User user) {
            //int length = 1;
            //User[] users = length >= 0 ? new User[length] : throw new NegativeArraySizeException();
            //users[0] = user;
            visibleOnlyToUsers(new[] { user });
        }

        //[JavaFlags(4)]
        public virtual void visibleOnlyToUsers(User[] users) {
            for (var index = 0; index < users.Length; ++index) {
                if (users[index] != null && getCurrentUser() == users[index]) {
                    return;
                }
            }

            vetoAccess();
        }

        //[JavaFlags(4)]
        public virtual void invalidOnCondition(bool conditionMet, string reasonNotMet) {
            concatDebug(new StringBuilder().Append("Conditionally invalid; ").Append(reasonNotMet).ToString());
            if (!conditionMet) {
                return;
            }

            invalid(reasonNotMet);
        }

        //[JavaFlags(4)]
        public virtual void invalid(string reason) {
            concatDebug(new StringBuilder().Append("unconditionally invalid; ").Append(reason).ToString());
            if (invalidReason == null) {
                invalidReason = new StringBuilder();
            }
            else {
                invalidReason.Append("; ");
            }

            invalidReason.Append(reason);
        }

        //[JavaFlags(4)]
        public virtual void invalid() => invalid("unconditionally invalid");

        //[JavaFlags(4227077)]
        //[JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
        public new virtual object MemberwiseClone() =>
            //AbstractAbout abstractAbout = this;
            //ObjectImpl.clone((object) abstractAbout);
            //return ((object) abstractAbout).MemberwiseClone();
            null;

        //[JavaFlags(4227073)]
        public override string ToString() =>
            //return ObjectImpl.jloToString((object)this);
            "";
    }
}