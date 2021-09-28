// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.or.DefaultRenderer
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;

namespace org.apache.log4j.or
{
  [JavaFlags(32)]
  [JavaInterfaces("1;org/apache/log4j/or/ObjectRenderer;")]
  public class DefaultRenderer : ObjectRenderer
  {
    [JavaFlags(0)]
    public DefaultRenderer()
    {
    }

    public virtual string doRender(object o) => o.ToString();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      DefaultRenderer defaultRenderer = this;
      ObjectImpl.clone((object) defaultRenderer);
      return ((object) defaultRenderer).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
