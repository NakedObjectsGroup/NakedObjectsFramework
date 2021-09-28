// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.command.ExecuteClientAction
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using org.nakedobjects.@object;

namespace org.nakedobjects.distribution.command
{
  public class ExecuteClientAction : AbstractRequest
  {
    private readonly ReferenceData[] data;
    private readonly int[] types;

    public ExecuteClientAction(Session session, ReferenceData[] data, int[] types)
      : base(session)
    {
      this.data = data;
      this.types = types;
    }

    public override void execute(Distribution distribution) => this.setResponse((object) distribution.executeClientAction(this.session, this.data, this.types));

    public virtual ClientActionResultData getActionResult() => (ClientActionResultData) this.getResponse();

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("data", this.data.Length);
      return toString.ToString();
    }
  }
}
