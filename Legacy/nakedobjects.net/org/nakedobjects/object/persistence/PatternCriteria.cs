// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.persistence.PatternCriteria
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
  public class PatternCriteria : InstancesCriteria
  {
    private bool includeSubclasses;
    private NakedObject pattern;

    public PatternCriteria(NakedObject pattern, bool includeSubclasses)
    {
      this.pattern = pattern;
      this.includeSubclasses = includeSubclasses;
    }

    public virtual NakedObjectSpecification getSpecification() => this.pattern.getSpecification();

    public virtual bool includeSubclasses() => this.includeSubclasses;

    public virtual bool matches(NakedObject @object) => this.matchesPattern(this.pattern, @object);

    private bool matchesPattern(NakedObject pattern, NakedObject instance)
    {
      NakedObject nakedObject = instance;
      foreach (NakedObjectField field in nakedObject.getSpecification().getFields())
      {
        if (!field.isDerived())
        {
          if (field.isValue())
          {
            NakedValue nakedValue1 = pattern.getValue((OneToOneAssociation) field);
            NakedValue nakedValue2 = pattern.getValue((OneToOneAssociation) field);
            if (nakedValue1.getObject() != null)
            {
              string lowerCase = StringImpl.toLowerCase(nakedValue1.titleString());
              if (StringImpl.indexOf(StringImpl.toLowerCase(nakedValue2.titleString()), lowerCase) == -1)
                return false;
            }
          }
          else
          {
            NakedObject association1 = pattern.getAssociation((OneToOneAssociation) field);
            NakedObject association2 = nakedObject.getAssociation((OneToOneAssociation) field);
            if (association1 != null && (association2 == null || !association1.getOid().Equals((object) association2.getOid())))
              return false;
          }
        }
      }
      return true;
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      PatternCriteria patternCriteria = this;
      ObjectImpl.clone((object) patternCriteria);
      return ((object) patternCriteria).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
