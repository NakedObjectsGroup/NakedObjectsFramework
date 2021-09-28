// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.command.HasInstances
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.distribution.command
{
  public class HasInstances : AbstractRequest
  {
    private readonly string name;

    public HasInstances(Session session, string name)
      : base(session)
    {
      this.name = name;
    }

    public override void execute(Distribution distribution) => this.setResponse((object) new Boolean(distribution.hasInstances(this.session, this.name)));

    public virtual bool getFlag() => ((Boolean) this.response).booleanValue();

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("class", this.name);
      return toString.ToString();
    }
  }
}
