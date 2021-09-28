// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.special.LookupAxis
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;

namespace org.nakedobjects.viewer.skylark.special
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/ViewAxis;")]
  [JavaFlags(32)]
  public class LookupAxis : ViewAxis
  {
    private readonly ObjectContent content;
    private readonly View view;

    public LookupAxis(ObjectContent content, View originalView)
    {
      this.content = content;
      this.view = originalView;
    }

    [JavaFlags(0)]
    public virtual ObjectContent getContent() => this.content;

    public virtual View getOriginalView() => this.view;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      LookupAxis lookupAxis = this;
      ObjectImpl.clone((object) lookupAxis);
      return ((object) lookupAxis).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
