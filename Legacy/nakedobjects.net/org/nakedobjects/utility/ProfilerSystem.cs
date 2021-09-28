// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.ProfilerSystem
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.utility
{
  public class ProfilerSystem
  {
    [JavaFlags(4)]
    public virtual long memory() => Runtime.getRuntime().totalMemory() - Runtime.getRuntime().freeMemory();

    [JavaFlags(4)]
    public virtual long time() => System.currentTimeMillis();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ProfilerSystem profilerSystem = this;
      ObjectImpl.clone((object) profilerSystem);
      return ((object) profilerSystem).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
