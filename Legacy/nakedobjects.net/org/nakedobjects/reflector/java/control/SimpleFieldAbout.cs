// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.control.SimpleFieldAbout
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object;
using org.nakedobjects.application.control;

namespace org.nakedobjects.reflector.java.control
{
  [JavaInterfaces("1;org/nakedobjects/application/control/FieldAbout;")]
  public class SimpleFieldAbout : AbstractAbout, FieldAbout
  {
    private bool isPersistent;
    private object[] proposedOptions;

    public SimpleFieldAbout(Session session, object @object)
      : base(session, @object)
    {
      this.isPersistent = true;
      this.proposedOptions = (object[]) null;
    }

    public virtual object[] getProposedOptions() => this.proposedOptions;

    public virtual void setProposedOptions(object[] proposedOptions) => this.proposedOptions = proposedOptions;

    public override void invisible() => base.invisible();

    public override void invisibleToUser(User user) => base.invisibleToUser(user);

    public override void invisibleToUsers(User[] users) => base.invisibleToUsers(users);

    public virtual bool isPersistent() => this.isPersistent;

    public virtual void modifiableOnlyByRole(Role role) => this.usableOnlyByRole(role);

    public virtual void modifiableOnlyByRoles(Role[] roles) => this.usableOnlyByRoles(roles);

    public virtual void modifiableOnlyByUser(User user) => this.usableOnlyByUser(user);

    public virtual void modifiableOnlyByUsers(User[] users) => this.usableOnlyByUsers(users);

    public virtual void modifiableOnlyInState(State state) => this.usableOnlyInState(state);

    public virtual void modifiableOnlyInStates(State[] states) => this.usableOnlyInStates(states);

    public virtual void nonPersistent() => this.isPersistent = false;

    public virtual void unmodifiable() => this.unusable("Cannot be modified");

    public virtual void unmodifiable(string reason) => this.unusable(reason);

    public virtual void unmodifiableByUser(User user) => this.unusableByUser(user);

    public virtual void unmodifiableByUsers(User[] users) => this.unusableByUsers(users);

    public virtual void unmodifiableInState(State state) => this.unusableInState(state);

    public virtual void unmodifiableInStates(State[] states) => this.unusableInStates(states);

    public virtual void unmodifiableOnCondition(bool conditionMet, string reasonNotMet) => this.unusableOnCondition(conditionMet, reasonNotMet);

    public override void visibleOnlyToRole(Role role) => base.visibleOnlyToRole(role);

    public override void visibleOnlyToRoles(Role[] roles) => base.visibleOnlyToRoles(roles);

    public override void visibleOnlyToUser(User user) => base.visibleOnlyToUser(user);

    public override void visibleOnlyToUsers(User[] users) => base.visibleOnlyToUsers(users);

    public override void invalid() => base.invalid();

    public override void invalid(string reason) => base.invalid(reason);

    public override void invalidOnCondition(bool condition, string reason) => base.invalidOnCondition(condition, reason);
  }
}
