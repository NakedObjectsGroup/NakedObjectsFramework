// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.help.OneToManyHelp
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using org.nakedobjects.@object.help;
using org.nakedobjects.@object.reflect;

namespace org.nakedobjects.@object.help
{
  public class OneToManyHelp : AbstractOneToManyPeer
  {
    private readonly HelpManager helpManager;

    public OneToManyHelp(OneToManyPeer local, HelpManager helpManager)
      : base(local)
    {
      this.helpManager = helpManager;
    }

    public override string getHelp() => this.helpManager == null ? "" : this.helpManager.help(this.getIdentifier());
  }
}
