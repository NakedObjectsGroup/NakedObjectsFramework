// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.command.ClearAssociation
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using org.nakedobjects.@object;

namespace org.nakedobjects.distribution.command
{
  public class ClearAssociation : AbstractRequest
  {
    private readonly string fieldIdentifier;
    private readonly IdentityData target;
    private readonly IdentityData associate;

    public ClearAssociation(
      Session session,
      string fieldIdentifier,
      IdentityData target,
      IdentityData associate)
      : base(session)
    {
      this.fieldIdentifier = fieldIdentifier;
      this.target = target;
      this.associate = associate;
    }

    public override void execute(Distribution distribution) => this.response = (object) distribution.clearAssociation(this.session, this.fieldIdentifier, this.target, this.associate);

    public virtual ObjectData[] getChanges() => (ObjectData[]) this.response;
  }
}
