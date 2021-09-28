// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.internal.InternalField
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using java.lang.reflect;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.@object.reflect.@internal;
using org.nakedobjects.utility;

namespace org.nakedobjects.@object.reflect.@internal
{
  public abstract class InternalField : InternalMember
  {
    [JavaFlags(20)]
    public readonly Method getMethod;
    private readonly bool isDerived;
    [JavaFlags(20)]
    public readonly Class type;

    public InternalField(Class type, Method get, bool isDerived)
    {
      this.type = type;
      this.isDerived = isDerived;
      this.getMethod = get;
    }

    public override object getExtension(Class cls) => (object) null;

    public override Class[] getExtensions()
    {
      int length = 0;
      return length >= 0 ? new Class[length] : throw new NegativeArraySizeException();
    }

    public virtual string getName() => (string) null;

    public virtual NakedObjectSpecification getType() => this.type == null ? (NakedObjectSpecification) null : NakedObjects.getSpecificationLoader().loadSpecification(this.type);

    public virtual bool isAccessible() => true;

    public virtual bool isAuthorised(Session session) => true;

    public virtual bool isDerived() => this.isDerived;

    public virtual Consent isEditable(NakedObject target) => (Consent) Allow.DEFAULT;

    public virtual bool isEmpty(NakedObject inObject) => throw new NotImplementedException();

    public virtual bool isHidden() => false;

    public virtual bool isHiddenInTableViews() => false;

    public virtual bool isMandatory() => false;

    public virtual Consent isAvailable(NakedReference target) => (Consent) Allow.DEFAULT;

    public virtual Consent isVisible(NakedReference target) => (Consent) Allow.DEFAULT;
  }
}
