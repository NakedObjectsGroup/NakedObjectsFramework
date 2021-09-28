// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.security.ActionAuthorisation
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using org.nakedobjects.@object;
using org.nakedobjects.@object.reflect;
using org.nakedobjects.@object.security;

namespace org.nakedobjects.@object.security
{
  public class ActionAuthorisation : AbstractActionPeer
  {
    private readonly AuthorisationManager authorisationManager;

    public ActionAuthorisation(ActionPeer decorated, AuthorisationManager authorisationManager)
      : base(decorated)
    {
      this.authorisationManager = authorisationManager;
    }

    public override bool isAuthorised(Session session) => this.authorisationManager.isVisible(session, this.getIdentifier());
  }
}
