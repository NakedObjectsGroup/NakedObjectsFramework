// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.command.Resolve
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using org.nakedobjects.@object;

namespace org.nakedobjects.distribution.command
{
  public class Resolve : AbstractRequest
  {
    private readonly IdentityData target;

    public Resolve(Session session, IdentityData target)
      : base(session)
    {
      this.target = target;
    }

    public override void execute(Distribution distribution) => this.setResponse((object) distribution.resolveImmediately(this.session, this.target));

    public virtual ObjectData getUpdateData() => (ObjectData) this.getResponse();

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("target", (object) this.target);
      return toString.ToString();
    }
  }
}
