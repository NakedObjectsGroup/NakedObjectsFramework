// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.xat.ExpectationValue
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.xat
{
  [JavaFlags(32)]
  public class ExpectationValue
  {
    private NakedObject reference;

    public ExpectationValue(string value)
    {
    }

    public ExpectationValue(NakedObject reference) => this.reference = reference;

    public virtual NakedObject getReference() => this.reference;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      ExpectationValue expectationValue = this;
      ObjectImpl.clone((object) expectationValue);
      return ((object) expectationValue).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
