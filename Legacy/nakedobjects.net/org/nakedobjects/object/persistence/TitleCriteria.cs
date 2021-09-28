// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.persistence.TitleCriteria
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.persistence;

namespace org.nakedobjects.@object.persistence
{
  [JavaInterfaces("1;org/nakedobjects/object/InstancesCriteria;")]
  public class TitleCriteria : InstancesCriteria
  {
    private readonly NakedObjectSpecification specification;
    private readonly string requiredTitle;
    private readonly bool includeSubclasses;

    public TitleCriteria(
      NakedObjectSpecification specification,
      string title,
      bool includeSubclasses)
    {
      this.specification = specification;
      this.requiredTitle = StringImpl.toLowerCase(title);
      this.includeSubclasses = includeSubclasses;
    }

    public virtual bool matches(NakedObject @object)
    {
      string lowerCase = StringImpl.toLowerCase(@object.titleString());
      return (object) lowerCase == (object) this.requiredTitle || StringImpl.indexOf(lowerCase, this.requiredTitle) >= 0;
    }

    public virtual NakedObjectSpecification getSpecification() => this.specification;

    public virtual bool includeSubclasses() => this.includeSubclasses;

    public virtual string getRequiredTitle() => this.requiredTitle;

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      TitleCriteria titleCriteria = this;
      ObjectImpl.clone((object) titleCriteria);
      return ((object) titleCriteria).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
