// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.valueholder.Magnitude
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace org.nakedobjects.application.valueholder
{
  public abstract class Magnitude : BusinessValueHolder
  {
    private const long serialVersionUID = 1;

    public virtual bool isBetween(Magnitude minMagnitude, Magnitude maxMagnitude) => this.isGreaterThanOrEqualTo(minMagnitude) && this.isLessThanOrEqualTo(maxMagnitude);

    public abstract bool isEqualTo(Magnitude magnitude);

    public virtual bool isGreaterThan(Magnitude magnitude) => magnitude.isLessThan(this);

    public virtual bool isGreaterThanOrEqualTo(Magnitude magnitude) => ((this.isLessThan(magnitude) ? 1 : 0) ^ 1) != 0;

    public abstract bool isLessThan(Magnitude magnitude);

    public virtual bool isLessThanOrEqualTo(Magnitude magnitude) => ((this.isGreaterThan(magnitude) ? 1 : 0) ^ 1) != 0;

    public virtual Magnitude max(Magnitude magnitude) => this.isGreaterThan(magnitude) ? this : magnitude;

    public virtual Magnitude min(Magnitude magnitude) => this.isLessThan(magnitude) ? this : magnitude;

    [JavaFlags(17)]
    public override sealed bool isSameAs(BusinessValueHolder @object) => @object is Magnitude && this.isEqualTo((Magnitude) @object);
  }
}
