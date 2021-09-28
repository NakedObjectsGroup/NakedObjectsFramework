// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.java.JavaClientResult
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.distribution.java
{
  [JavaInterfaces("1;org/nakedobjects/distribution/ClientActionResultData;")]
  public class JavaClientResult : ClientActionResultData
  {
    private readonly ObjectData[] madePersistent;
    private readonly Version[] changedVersion;

    public JavaClientResult(ObjectData[] madePersistent, Version[] changedVersion)
    {
      this.madePersistent = madePersistent;
      this.changedVersion = changedVersion;
    }

    public virtual ObjectData[] getPersisted() => this.madePersistent;

    public virtual Version[] getChanged() => this.changedVersion;

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      JavaClientResult javaClientResult = this;
      ObjectImpl.clone((object) javaClientResult);
      return ((object) javaClientResult).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
