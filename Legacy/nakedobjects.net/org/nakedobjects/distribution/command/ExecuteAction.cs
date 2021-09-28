// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.command.ExecuteAction
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using org.nakedobjects.@object;

namespace org.nakedobjects.distribution.command
{
  public class ExecuteAction : AbstractRequest
  {
    private readonly string actionIdentifier;
    private readonly string actionType;
    private readonly Data[] parameters;
    private readonly ReferenceData target;

    public ExecuteAction(
      Session session,
      string actionType,
      string actionIdentifier,
      ReferenceData target,
      Data[] parameters)
      : base(session)
    {
      this.actionType = actionType;
      this.actionIdentifier = actionIdentifier;
      this.target = target;
      this.parameters = parameters;
    }

    public override void execute(Distribution distribution) => this.setResponse((object) distribution.executeServerAction(this.session, this.actionType, this.actionIdentifier, this.target, this.parameters, 1));

    public virtual ServerActionResultData getActionResult() => (ServerActionResultData) this.getResponse();

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("method", this.actionIdentifier);
      toString.append("target", (object) this.target);
      return toString.ToString();
    }
  }
}
