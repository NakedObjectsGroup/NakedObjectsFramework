// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.DistributionTypes
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;

namespace org.nakedobjects.distribution
{
  public class DistributionTypes
  {
    public const int ADD = 1;
    public const int CHANGE = 2;
    public const int DELETE = 3;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      DistributionTypes distributionTypes = this;
      ObjectImpl.clone((object) distributionTypes);
      return ((object) distributionTypes).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
