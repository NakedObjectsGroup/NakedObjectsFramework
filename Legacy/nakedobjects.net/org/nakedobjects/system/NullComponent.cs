// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.system.NullComponent
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.system
{
  [JavaInterfaces("1;org/nakedobjects/object/NakedObjectsComponent;")]
  public class NullComponent : NakedObjectsComponent
  {
    public virtual void init()
    {
    }

    public virtual void shutdown()
    {
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      NullComponent nullComponent = this;
      ObjectImpl.clone((object) nullComponent);
      return ((object) nullComponent).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
