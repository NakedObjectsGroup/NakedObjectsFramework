// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.persistence.PatternObjectCriteria
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
  public class PatternObjectCriteria : InstancesCriteria
  {
    private readonly bool includeSubclasses;
    private readonly NakedObject pattern;

    public PatternObjectCriteria(NakedObject pattern, bool includeSubclasses)
    {
      this.pattern = pattern;
      this.includeSubclasses = includeSubclasses;
    }

    public virtual bool matches(NakedObject @object) => this.pattern.getSpecification().Equals((object) @object.getSpecification()) && this.matchesPattern(this.pattern, @object);

    private bool matchesPattern(NakedObject pattern, NakedObject instance)
    {
      NakedObject nakedObject = instance;
      foreach (NakedObjectField field1 in nakedObject.getSpecification().getFields())
      {
        if (!field1.isDerived())
        {
          if (field1.isValue())
          {
            NakedObject field2 = (NakedObject) pattern.getField(field1);
            NakedObject field3 = (NakedObject) nakedObject.getField(field1);
            if (!field2.isEmpty(field1))
            {
              string lowerCase = StringImpl.toLowerCase(StringImpl.toString(field2.titleString()));
              if (StringImpl.indexOf(StringImpl.toLowerCase(StringImpl.toString(field3.titleString())), lowerCase) == -1)
                return false;
            }
          }
          else
          {
            NakedObject field4 = (NakedObject) pattern.getField(field1);
            NakedObject field5 = (NakedObject) nakedObject.getField(field1);
            if (field4 != null && (field5 == null || field4 != field5))
              return false;
          }
        }
      }
      return true;
    }

    public virtual NakedObjectSpecification getSpecification() => this.pattern.getSpecification();

    public virtual bool includeSubclasses() => this.includeSubclasses;

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      PatternObjectCriteria patternObjectCriteria = this;
      ObjectImpl.clone((object) patternObjectCriteria);
      return ((object) patternObjectCriteria).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
