// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.java.JavaNullData
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;

namespace org.nakedobjects.distribution.java
{
  [JavaInterfaces("1;org/nakedobjects/distribution/NullData;")]
  public class JavaNullData : NullData
  {
    private readonly string type;

    public JavaNullData(string type) => this.type = type;

    public virtual string getType() => this.type;

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("type", this.type);
      return toString.ToString();
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      JavaNullData javaNullData = this;
      ObjectImpl.clone((object) javaNullData);
      return ((object) javaNullData).MemberwiseClone();
    }
  }
}
