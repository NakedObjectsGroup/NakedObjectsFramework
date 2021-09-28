// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.DebugViewStructure
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.utility;

namespace org.nakedobjects.viewer.skylark.core
{
  [JavaInterfaces("1;org/nakedobjects/utility/DebugInfo;")]
  public class DebugViewStructure : DebugInfo
  {
    private readonly View view;

    public DebugViewStructure(View display) => this.view = display;

    public virtual void debugData(DebugString debug) => debug.append((object) this.view.debugDetails());

    public virtual string getDebugTitle() => "View Structure";

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      DebugViewStructure debugViewStructure = this;
      ObjectImpl.clone((object) debugViewStructure);
      return ((object) debugViewStructure).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
