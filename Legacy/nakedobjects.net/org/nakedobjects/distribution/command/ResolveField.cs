// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.command.ResolveField
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using org.nakedobjects.@object;

namespace org.nakedobjects.distribution.command
{
  public class ResolveField : AbstractRequest
  {
    private readonly IdentityData target;
    private readonly string field;

    public ResolveField(Session session, IdentityData target, string field)
      : base(session)
    {
      this.target = target;
      this.field = field;
    }

    public override void execute(Distribution sd) => this.setResponse((object) sd.resolveField(this.session, this.target, this.field));

    public virtual Data getUpdateData() => (Data) this.getResponse();

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("target", (object) this.target);
      toString.append("field", this.field);
      return toString.ToString();
    }
  }
}
