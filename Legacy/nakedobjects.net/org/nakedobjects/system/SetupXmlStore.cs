// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.system.SetupXmlStore
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using com.ms.vjsharp.util;
using System.Runtime.CompilerServices;

namespace org.nakedobjects.system
{
  public sealed class SetupXmlStore : AbstractXmlStoreSystem
  {
    public static void main(string[] args)
    {
      // ISSUE: type reference
      RuntimeHelpers.RunClassConstructor(__typeref (ObjectImpl));
      new SetupXmlStore().init();
      Utilities.cleanupAfterMainReturns();
    }

    [JavaFlags(4)]
    public override void setupFixtures() => this.installFixtures("nakedobjects.xmlos.fixtures");

    [JavaFlags(4)]
    public override void displayUserInterface() => this.shutdown();
  }
}
