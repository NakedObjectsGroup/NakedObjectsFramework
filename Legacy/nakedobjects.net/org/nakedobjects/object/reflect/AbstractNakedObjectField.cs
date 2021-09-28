// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.AbstractNakedObjectField
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.reflect;

namespace org.nakedobjects.@object.reflect
{
  [JavaInterfaces("1;org/nakedobjects/object/NakedObjectField;")]
  public abstract class AbstractNakedObjectField : AbstractNakedObjectMember, NakedObjectField
  {
    private readonly NakedObjectSpecification specification;

    public AbstractNakedObjectField(string name, NakedObjectSpecification type)
      : base(name)
    {
      this.specification = type != null ? type : throw new IllegalArgumentException(new StringBuffer().append("type cannot be null for ").append(name).ToString());
    }

    public abstract Naked get(NakedObject fromObject);

    public virtual NakedObjectSpecification getSpecification() => this.specification;

    public virtual bool isCollection() => false;

    public abstract bool isDerived();

    public abstract bool isEmpty(NakedObject adapter);

    public virtual bool isObject() => false;

    public virtual bool isValue() => false;

    public virtual bool isMandatory() => false;

    public abstract override Class[] getExtensions();

    public abstract bool isHidden();

    public abstract bool isHiddenInTableViews();
  }
}
