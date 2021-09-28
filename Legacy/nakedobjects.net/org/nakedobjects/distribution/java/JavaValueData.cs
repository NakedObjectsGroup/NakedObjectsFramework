// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.java.JavaValueData
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;

namespace org.nakedobjects.distribution.java
{
  [JavaInterfaces("1;org/nakedobjects/distribution/ValueData;")]
  public class JavaValueData : ValueData
  {
    private string type;
    private object value;

    public JavaValueData(string type, object value)
    {
      this.type = type;
      this.value = value;
    }

    public virtual object getValue() => this.value;

    public virtual string getType() => this.type;

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("type", this.type);
      toString.append("value", this.value);
      return toString.ToString();
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      JavaValueData javaValueData = this;
      ObjectImpl.clone((object) javaValueData);
      return ((object) javaValueData).MemberwiseClone();
    }
  }
}
