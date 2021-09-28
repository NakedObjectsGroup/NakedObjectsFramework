// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.command.AllInstances
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using org.nakedobjects.@object;

namespace org.nakedobjects.distribution.command
{
  public class AllInstances : AbstractRequest
  {
    private readonly string name;
    private bool includeSubclasses;

    public AllInstances(Session session, string name, bool includeSubclasses)
      : base(session)
    {
      this.name = name;
      this.includeSubclasses = includeSubclasses;
    }

    public override void execute(Distribution distribution) => this.setResponse((object) distribution.allInstances(this.session, this.name, this.includeSubclasses));

    public virtual ObjectData[] getInstances() => (ObjectData[]) this.response;

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("class", this.name);
      toString.append("subclasses", this.includeSubclasses);
      return toString.ToString();
    }
  }
}
