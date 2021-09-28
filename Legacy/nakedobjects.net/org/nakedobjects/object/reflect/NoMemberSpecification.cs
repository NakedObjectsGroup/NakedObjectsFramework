// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.NoMemberSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.@object.reflect;

namespace org.nakedobjects.@object.reflect
{
  [JavaInterfaces("1;org/nakedobjects/object/NakedObjectSpecification;")]
  public class NoMemberSpecification : NakedObjectSpecification
  {
    private readonly string name;

    public NoMemberSpecification(string name) => this.name = name;

    public virtual void addSubclass(NakedObjectSpecification specification)
    {
    }

    public virtual void clearDirty(NakedObject @object)
    {
    }

    public virtual void deleted(NakedObject @object)
    {
    }

    public virtual NakedObjectField[] getAccessibleFields()
    {
      int length = 0;
      return length >= 0 ? new NakedObjectField[length] : throw new NegativeArraySizeException();
    }

    public virtual NakedObjectField[] getAccessibleFieldsForCollectiveView() => this.getAccessibleFields();

    public virtual Action[] getButtonActions() => (Action[]) null;

    public virtual Action getClassAction(Action.Type type, string name) => (Action) null;

    public virtual Action getClassAction(
      Action.Type type,
      string name,
      NakedObjectSpecification[] parameters)
    {
      return (Action) null;
    }

    public virtual Action[] getClassActions(Action.Type type)
    {
      int length = 0;
      return length >= 0 ? new Action[length] : throw new NegativeArraySizeException();
    }

    public virtual Hint getClassHint() => (Hint) new DefaultHint();

    public virtual object getExtension(Class cls) => (object) null;

    public virtual Class[] getExtensions()
    {
      int length = 0;
      return length >= 0 ? new Class[length] : throw new NegativeArraySizeException();
    }

    public virtual NakedObjectField getField(string name) => (NakedObjectField) null;

    public virtual object getFieldExtension(string name, Class cls) => (object) null;

    public virtual Class[] getFieldExtensions(string name)
    {
      int length = 0;
      return length >= 0 ? new Class[length] : throw new NegativeArraySizeException();
    }

    public virtual NakedObjectField[] getFields()
    {
      int length = 0;
      return length >= 0 ? new NakedObjectField[length] : throw new NegativeArraySizeException();
    }

    public virtual string getFullName() => this.name;

    public virtual Action getObjectAction(Action.Type type, string name) => (Action) null;

    public virtual Action getObjectAction(
      Action.Type type,
      string name,
      NakedObjectSpecification[] parameters)
    {
      return (Action) null;
    }

    public virtual Action[] getObjectActions(Action.Type type)
    {
      int length = 0;
      return length >= 0 ? new Action[length] : throw new NegativeArraySizeException();
    }

    public virtual string getPluralName() => this.name;

    public virtual string getShortName() => this.name;

    public virtual string getSingularName() => this.name;

    public virtual string getTitle(NakedObject naked) => "no title";

    public virtual NakedObjectField[] getVisibleFields(NakedObject @object)
    {
      int length = 0;
      return length >= 0 ? new NakedObjectField[length] : throw new NegativeArraySizeException();
    }

    public virtual bool hasSubclasses() => false;

    public virtual NakedObjectSpecification[] interfaces()
    {
      int length = 0;
      return length >= 0 ? new NakedObjectSpecification[length] : throw new NegativeArraySizeException();
    }

    public virtual void introspect()
    {
    }

    public virtual bool isAbstract() => false;

    public virtual bool isCollection() => false;

    public virtual bool isDirty(NakedObject @object) => false;

    public virtual bool isLookup() => false;

    public virtual bool isObject() => true;

    public virtual bool isOfType(NakedObjectSpecification specification) => specification == this;

    public virtual bool isValue() => false;

    public virtual void markDirty(NakedObject @object)
    {
    }

    public virtual Persistable persistable() => Persistable.TRANSIENT;

    public virtual NakedObjectSpecification[] subclasses()
    {
      int length = 0;
      return length >= 0 ? new NakedObjectSpecification[length] : throw new NegativeArraySizeException();
    }

    public virtual NakedObjectSpecification superclass() => (NakedObjectSpecification) null;

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("class", this.name);
      return toString.ToString();
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      NoMemberSpecification memberSpecification = this;
      ObjectImpl.clone((object) memberSpecification);
      return ((object) memberSpecification).MemberwiseClone();
    }
  }
}
