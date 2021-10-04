// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.control.SimpleFieldAbout
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using Legacy.NakedObjects.Application.Control;
using NakedFramework.Architecture.Component;
// ReSharper disable InconsistentNaming

namespace Legacy.NakedObjects.Reflector.Java.Control {
    //[JavaInterfaces("1;org/nakedobjects/application/control/FieldAbout;")]
    public class SimpleFieldAbout : AbstractAbout, FieldAbout {
        private bool _isPersistent;
        private object[] proposedOptions;

        public SimpleFieldAbout(ISession session, object @object)
            : base(session, @object) {
            _isPersistent = true;
            proposedOptions = null;
        }

        public override void invisible() => base.invisible();

        public override void invisibleToUser(User user) => base.invisibleToUser(user);

        public override void invisibleToUsers(User[] users) => base.invisibleToUsers(users);

        public virtual bool isPersistent() => _isPersistent;

        public virtual void modifiableOnlyByRole(Role role) => usableOnlyByRole(role);

        public virtual void modifiableOnlyByRoles(Role[] roles) => usableOnlyByRoles(roles);

        public virtual void modifiableOnlyByUser(User user) => usableOnlyByUser(user);

        public virtual void modifiableOnlyByUsers(User[] users) => usableOnlyByUsers(users);

        public virtual void modifiableOnlyInState(State state) => usableOnlyInState(state);

        public virtual void modifiableOnlyInStates(State[] states) => usableOnlyInStates(states);

        public virtual void nonPersistent() => _isPersistent = false;

        public virtual void unmodifiable() => unusable("Cannot be modified");

        public virtual void unmodifiable(string reason) => unusable(reason);

        public virtual void unmodifiableByUser(User user) => unusableByUser(user);

        public virtual void unmodifiableByUsers(User[] users) => unusableByUsers(users);

        public virtual void unmodifiableInState(State state) => unusableInState(state);

        public virtual void unmodifiableInStates(State[] states) => unusableInStates(states);

        public virtual void unmodifiableOnCondition(bool conditionMet, string reasonNotMet) => unusableOnCondition(conditionMet, reasonNotMet);

        public override void invalidOnCondition(bool condition, string reason) => base.invalidOnCondition(condition, reason);

        public virtual object[] getProposedOptions() => proposedOptions;

        public virtual void setProposedOptions(object[] proposedOptions) => this.proposedOptions = proposedOptions;
    }
}