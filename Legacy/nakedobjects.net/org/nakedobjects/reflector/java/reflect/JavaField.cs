// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.reflect.JavaField
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using java.lang.reflect;
using org.nakedobjects.@object;
using org.nakedobjects.@object.reflect;

namespace org.nakedobjects.reflector.java.reflect
{
  public abstract class JavaField : JavaMember
  {
    [JavaFlags(20)]
    public readonly Method getMethod;
    private readonly bool isDerived;
    private readonly bool isHidden;
    private readonly bool isHiddenInTableViews;
    [JavaFlags(20)]
    public readonly Class type;

    public JavaField(
      MemberIdentifier identifier,
      Class type,
      Method get,
      Method about,
      bool isHidden,
      bool isHiddenInTableViews,
      bool isDerived)
      : base(identifier, about)
    {
      this.type = type;
      this.isDerived = isDerived;
      this.isHidden = isHidden;
      this.isHiddenInTableViews = isHiddenInTableViews;
      this.getMethod = get;
    }

    public virtual NakedObjectSpecification getType() => this.type == null ? (NakedObjectSpecification) null : NakedObjects.getSpecificationLoader().loadSpecification(this.type);

    public virtual bool isDerived() => this.isDerived;

    public virtual string getDescription() => "";

    public virtual bool isHidden() => this.isHidden;

    public virtual bool isHiddenInTableViews() => this.isHiddenInTableViews;

    public virtual bool isMandatory() => false;

    public virtual bool isAuthorised(Session session) => true;
  }
}
