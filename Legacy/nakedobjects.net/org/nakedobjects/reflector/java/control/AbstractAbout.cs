// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.control.AbstractAbout
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.application.control;

namespace org.nakedobjects.reflector.java.control
{
  [JavaInterfaces("1;org/nakedobjects/object/control/Hint;")]
  public abstract class AbstractAbout : Hint
  {
    private const long serialVersionUID = 1;
    private Session session;
    private string description;
    private bool isAccessible;
    private string name;
    private StatefulObject statefulObject;
    private StringBuffer unusableReason;
    private StringBuffer invalidReason;
    private StringBuffer debug;

    public AbstractAbout(Session session, object @object)
    {
      this.debug = new StringBuffer();
      this.session = session;
      this.isAccessible = true;
      this.description = "";
      if (!(@object is StatefulObject))
        return;
      this.statefulObject = (StatefulObject) @object;
    }

    public virtual Consent canAccess() => AbstractConsent.allow(this.isAccessible);

    public virtual Consent canUse() => this.unusableReason == null ? (Consent) new Allow(this.getDescription()) : (Consent) new Veto(this.unusableReason.ToString());

    public virtual Consent isValid() => this.invalidReason == null ? (Consent) Allow.DEFAULT : (Consent) new Veto(this.invalidReason.ToString());

    private bool currentUserHasRole(Role role) => ((SimpleSession) this.session).hasRole(role);

    public virtual string debug() => this.debug.ToString();

    public virtual string getDescription() => this.description;

    public virtual string getName() => this.name;

    [JavaFlags(4)]
    public virtual void invisible()
    {
      this.concatDebug("unconditionally invisible");
      this.vetoAccess();
    }

    private void concatDebug(string @string)
    {
      this.debug.append(this.debug.length() <= 0 ? "" : "; ");
      this.debug.append(@string);
    }

    [JavaFlags(4)]
    public virtual void invisibleToUser(User user)
    {
      this.concatDebug(new StringBuffer().append("Invisible to user ").append((object) user).ToString());
      if (this.getCurrentUser() != user)
        return;
      this.vetoAccess();
    }

    [JavaFlags(4)]
    public virtual void invisibleToUsers(User[] users)
    {
      this.concatDebug("Invisible to users ");
      for (int index = 0; index < users.Length; ++index)
        this.debug.append(index <= 0 ? new StringBuffer().append("").append((object) users[index].getName()).ToString() : ". ");
      for (int index = 0; index < users.Length; ++index)
      {
        if (this.getCurrentUser() == users[index])
        {
          this.vetoAccess();
          break;
        }
      }
    }

    public virtual void setDescription(string description) => this.description = description;

    public virtual void setName(string name) => this.name = name;

    private bool stateIsSameAs(State state) => this.statefulObject != null ? this.statefulObject.getState().Equals((object) state) : throw new IllegalStateException("Cannot check state of object.  About not instantiated with object reference.");

    [JavaFlags(4)]
    public virtual void unusable(string reason)
    {
      this.concatDebug("Unconditionally unusable");
      this.vetoUse(reason);
    }

    [JavaFlags(4)]
    public virtual void unusableByUser(User user)
    {
      if (this.getCurrentUser() != user)
        return;
      this.vetoUse("Not available to current user");
    }

    private User getCurrentUser() => ((SimpleSession) this.session).getName();

    [JavaFlags(4)]
    public virtual void unusableByUsers(User[] users)
    {
      bool flag = true;
      for (int index = 0; index < users.Length; ++index)
      {
        if (users[index] != null)
          flag = flag && this.getCurrentUser() != users[index];
      }
      if (flag)
        return;
      this.vetoUse("Not available to current user");
    }

    [JavaFlags(4)]
    public virtual void unusableInState(State state)
    {
      this.concatDebug(new StringBuffer().append("Unusable in state ").append((object) state).ToString());
      if (!this.stateIsSameAs(state))
        return;
      this.vetoUse("Unusable when object is in its current State");
    }

    [JavaFlags(4)]
    public virtual void unusableInStates(State[] states)
    {
      bool flag = true;
      string str = StringImpl.createString();
      for (int index = 0; index < states.Length; ++index)
      {
        if (states[index] != null)
        {
          flag = flag && !this.stateIsSameAs(states[index]);
          str = new StringBuffer().append(str).append(states[index].ToString()).ToString();
          if (index < states.Length)
            str = new StringBuffer().append(str).append(", ").ToString();
        }
      }
      if (flag)
        return;
      this.vetoUse(new StringBuffer().append("Unusable when object is in any of these states: ").append(str).ToString());
    }

    [JavaFlags(4)]
    public virtual void unusableOnCondition(bool conditionMet, string reasonNotMet)
    {
      this.concatDebug(new StringBuffer().append("Conditionally unusable ").append(reasonNotMet).ToString());
      if (!conditionMet)
        return;
      this.vetoUse(reasonNotMet);
    }

    [JavaFlags(4)]
    public virtual void usableOnlyByRole(Role role)
    {
      this.concatDebug(new StringBuffer().append("Usable only for role ").append((object) role.getName()).ToString());
      if (this.currentUserHasRole(role))
        return;
      this.vetoUse("User does not have the appropriate role");
    }

    [JavaFlags(4)]
    public virtual void usableOnlyByRoles(Role[] roles)
    {
      bool flag = false;
      for (int index = 0; index < roles.Length; ++index)
      {
        if (roles[index] != null)
          flag = flag || this.currentUserHasRole(roles[index]);
      }
      if (flag)
        return;
      this.vetoUse("User does not have the appropriate role");
    }

    [JavaFlags(4)]
    public virtual void usableOnlyByUser(User user)
    {
      if (this.getCurrentUser() == user)
        return;
      this.vetoUse("Not available to current user");
    }

    [JavaFlags(4)]
    public virtual void usableOnlyByUsers(User[] users)
    {
      bool flag = false;
      for (int index = 0; index < users.Length; ++index)
      {
        if (users[index] != null)
          flag = flag || this.getCurrentUser() == users[index];
      }
      if (flag)
        return;
      this.vetoUse("Not available to current user");
    }

    [JavaFlags(4)]
    public virtual void usableOnlyInState(State state)
    {
      if (this.stateIsSameAs(state))
        return;
      this.vetoUse(new StringBuffer().append("Usable only when object is in the state: ").append(state.ToString()).ToString());
    }

    [JavaFlags(4)]
    public virtual void usableOnlyInStates(State[] states)
    {
      this.concatDebug("usable only to certain roles");
      bool flag = false;
      StringBuffer stringBuffer = new StringBuffer();
      for (int index = 0; index < states.Length; ++index)
      {
        if (states[index] != null)
        {
          flag = flag || this.stateIsSameAs(states[index]);
          stringBuffer.append((object) stringBuffer);
          stringBuffer.append(index <= 0 ? "" : ", ");
          stringBuffer.append(states[index].ToString());
        }
      }
      if (flag)
        return;
      this.vetoUse(new StringBuffer().append("Usable only when object is in any of these states: ").append((object) stringBuffer).ToString());
    }

    [JavaFlags(4)]
    public virtual void vetoAccess()
    {
      this.concatDebug("Access unconditionally vetoed");
      this.isAccessible = false;
    }

    [JavaFlags(4)]
    public virtual void vetoUse(string reason)
    {
      this.concatDebug(new StringBuffer().append("Use unconditionally vetoed; ").append(reason).ToString());
      if (this.unusableReason == null)
        this.unusableReason = new StringBuffer();
      else
        this.unusableReason.append("; ");
      this.unusableReason.append(reason);
    }

    [JavaFlags(4)]
    public virtual void visibleOnlyToRole(Role role)
    {
      int length = 1;
      Role[] roles = length >= 0 ? new Role[length] : throw new NegativeArraySizeException();
      roles[0] = role;
      this.visibleOnlyToRoles(roles);
    }

    [JavaFlags(4)]
    public virtual void visibleOnlyToRoles(Role[] roles)
    {
      this.concatDebug("Visible only to roles ");
      bool flag = false;
      for (int index = 0; index < roles.Length; ++index)
      {
        if (roles[index] != null)
        {
          this.debug.append(index <= 0 ? new StringBuffer().append("").append((object) roles[index].getName()).ToString() : ", ");
          flag = flag || this.currentUserHasRole(roles[index]);
        }
      }
      if (flag)
        return;
      this.vetoAccess();
    }

    [JavaFlags(4)]
    public virtual void visibleOnlyToUser(User user)
    {
      int length = 1;
      User[] users = length >= 0 ? new User[length] : throw new NegativeArraySizeException();
      users[0] = user;
      this.visibleOnlyToUsers(users);
    }

    [JavaFlags(4)]
    public virtual void visibleOnlyToUsers(User[] users)
    {
      for (int index = 0; index < users.Length; ++index)
      {
        if (users[index] != null && this.getCurrentUser() == users[index])
          return;
      }
      this.vetoAccess();
    }

    [JavaFlags(4)]
    public virtual void invalidOnCondition(bool conditionMet, string reasonNotMet)
    {
      this.concatDebug(new StringBuffer().append("Conditionally invalid; ").append(reasonNotMet).ToString());
      if (!conditionMet)
        return;
      this.invalid(reasonNotMet);
    }

    [JavaFlags(4)]
    public virtual void invalid(string reason)
    {
      this.concatDebug(new StringBuffer().append("unconditionally invalid; ").append(reason).ToString());
      if (this.invalidReason == null)
        this.invalidReason = new StringBuffer();
      else
        this.invalidReason.append("; ");
      this.invalidReason.append(reason);
    }

    [JavaFlags(4)]
    public virtual void invalid() => this.invalid("unconditionally invalid");

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      AbstractAbout abstractAbout = this;
      ObjectImpl.clone((object) abstractAbout);
      return ((object) abstractAbout).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
