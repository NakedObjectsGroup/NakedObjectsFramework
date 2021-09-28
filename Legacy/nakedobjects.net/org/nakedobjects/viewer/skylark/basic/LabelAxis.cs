// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.LabelAxis
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.viewer.skylark.core;

namespace org.nakedobjects.viewer.skylark.basic
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/ViewAxis;")]
  public class LabelAxis : ViewAxis
  {
    [JavaFlags(0)]
    public CompositeView container;
    [JavaFlags(0)]
    public int width;

    public virtual void accommodateWidth(int width) => this.width = Math.max(this.width, width);

    public virtual int getWidth() => this.width;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      LabelAxis labelAxis = this;
      ObjectImpl.clone((object) labelAxis);
      return ((object) labelAxis).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
