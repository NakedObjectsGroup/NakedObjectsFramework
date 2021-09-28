// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.control.SimpleClassAbout
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object;
using org.nakedobjects.application.control;

namespace org.nakedobjects.reflector.java.control
{
  [JavaInterfaces("1;org/nakedobjects/application/control/ClassAbout;")]
  public class SimpleClassAbout : AbstractAbout, ClassAbout
  {
    public SimpleClassAbout(Session session, object @object)
      : base(session, @object)
    {
    }

    public virtual void uninstantiable() => this.unusable("");

    public virtual void uninstantiable(string reason) => this.unusable(reason);

    public virtual void instantiableOnlyByRole(Role role) => this.usableOnlyByRole(role);

    public virtual void instantiableOnlyByRoles(Role[] roles) => this.usableOnlyByRoles(roles);

    public virtual void instantiableOnlyByUser(User user) => this.usableOnlyByUser(user);

    public virtual void instantiableOnlyByUsers(User[] users) => this.usableOnlyByUsers(users);

    public virtual void instancesUnavailable() => this.invisible();

    public virtual void instancesAvailableOnlyToRole(Role role) => this.visibleOnlyToRole(role);

    public virtual void instancesAvailableOnlyToRoles(Role[] roles) => this.visibleOnlyToRoles(roles);

    public virtual void instancesAvailableOnlyToUser(User user) => this.visibleOnlyToUser(user);

    public virtual void instancesAvailableOnlyToUsers(User[] users) => this.visibleOnlyToUsers(users);
  }
}
