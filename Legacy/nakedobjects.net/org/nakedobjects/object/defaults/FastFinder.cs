// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.defaults.FastFinder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.defaults;
using org.nakedobjects.@object.persistence;
using org.nakedobjects.@object.reflect.@internal.about;

namespace org.nakedobjects.@object.defaults
{
  [JavaInterfaces("1;org/nakedobjects/object/InternalNakedObject;")]
  public class FastFinder : InternalNakedObject
  {
    private NakedObjectSpecification specification;
    private string term;

    public virtual string getIconName() => this.specification.getShortName();

    public virtual string getTerm() => this.term;

    public virtual void setTerm(string term) => this.term = term;

    public virtual void aboutActionFind(InternalAbout about) => about.unusableOnCondition(this.term == null || StringImpl.length(StringImpl.trim(this.term)) == 0, "Search term needed");

    public virtual Naked actionFind()
    {
      NakedCollection instances = (NakedCollection) NakedObjects.getObjectPersistor().findInstances((InstancesCriteria) new TitleCriteria(this.specification, this.term, true));
      return instances.size() == 1 ? (Naked) instances.elements().nextElement() : (Naked) instances;
    }

    public virtual void aboutFromClass(InternalAbout about) => about.unusable();

    public virtual void setFromClass(NakedObjectSpecification nakedClass) => this.specification = nakedClass;

    public virtual NakedObjectSpecification getSpecification() => this.specification;

    public virtual string titleString() => new StringBuffer().append("Search for '").append(this.term).append("'").ToString();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      FastFinder fastFinder = this;
      ObjectImpl.clone((object) fastFinder);
      return ((object) fastFinder).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
