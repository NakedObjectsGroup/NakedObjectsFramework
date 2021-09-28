// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.value.Magnitude
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;

namespace org.nakedobjects.application.value
{
  [JavaInterfaces("1;org/nakedobjects/application/value/BusinessValue;")]
  public abstract class Magnitude : BusinessValue
  {
    public virtual bool isBetween(Magnitude minMagnitude, Magnitude maxMagnitude) => this.isGreaterThanOrEqualTo(minMagnitude) && this.isLessThanOrEqualTo(maxMagnitude);

    public virtual bool userChangeable() => true;

    public abstract bool isEqualTo(Magnitude magnitude);

    public virtual bool isGreaterThan(Magnitude magnitude) => magnitude.isLessThan(this);

    public virtual bool isGreaterThanOrEqualTo(Magnitude magnitude) => ((this.isLessThan(magnitude) ? 1 : 0) ^ 1) != 0;

    public abstract bool isLessThan(Magnitude magnitude);

    public virtual bool isLessThanOrEqualTo(Magnitude magnitude) => ((this.isGreaterThan(magnitude) ? 1 : 0) ^ 1) != 0;

    public virtual Magnitude max(Magnitude magnitude) => this.isGreaterThan(magnitude) ? this : magnitude;

    public virtual Magnitude min(Magnitude magnitude) => this.isLessThan(magnitude) ? this : magnitude;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      Magnitude magnitude = this;
      ObjectImpl.clone((object) magnitude);
      return ((object) magnitude).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
