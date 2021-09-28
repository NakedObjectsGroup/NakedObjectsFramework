// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.control.Allow
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object.control;
using System.ComponentModel;

namespace org.nakedobjects.@object.control
{
  public class Allow : AbstractConsent
  {
    public static readonly Allow DEFAULT;

    public Allow()
    {
    }

    public Allow(string reason)
      : base(reason)
    {
    }

    [JavaFlags(17)]
    public override sealed bool isAllowed() => true;

    [JavaFlags(17)]
    public override sealed bool isVetoed() => false;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static Allow()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
