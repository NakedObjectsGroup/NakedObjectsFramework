// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.internal.InternalOneToOneAssociation
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using java.lang.reflect;
using org.apache.log4j;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.@object.reflect;
using org.nakedobjects.@object.reflect.@internal;
using org.nakedobjects.@object.value.adapter;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.@object.reflect.@internal
{
  [JavaInterfaces("1;org/nakedobjects/object/reflect/OneToOnePeer;")]
  public class InternalOneToOneAssociation : InternalField, OneToOnePeer
  {
    private static readonly Category LOG;
    [JavaFlags(4)]
    public Method addMethod;
    [JavaFlags(4)]
    public Method removeMethod;
    [JavaFlags(4)]
    public Method setMethod;
    private readonly bool isObject;

    public InternalOneToOneAssociation(
      bool isObject,
      string className,
      string name,
      Class type,
      Method get,
      Method set,
      Method add,
      Method remove)
      : base(type, get, false)
    {
      this.isObject = isObject;
      this.setMethod = set;
      this.addMethod = add;
      this.removeMethod = remove;
      this.identifeir = (MemberIdentifier) new MemberIdentifierImpl(className, name);
    }

    public virtual TypedNakedCollection proposedOptions(NakedReference target) => (TypedNakedCollection) null;

    private bool hasAddMethod() => this.addMethod != null;

    public virtual void setValue(NakedObject inObject, object value)
    {
      if (InternalOneToOneAssociation.LOG.isDebugEnabled())
        InternalOneToOneAssociation.LOG.debug((object) new StringBuffer().append("set value ").append((object) this.getIdentifier()).append(" in ").append((object) inObject).append(" - ").append(value).ToString());
      try
      {
        if (this.setMethod == null)
          return;
        Method setMethod = this.setMethod;
        object obj = inObject.getObject();
        int length = 1;
        object[] objArray = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        objArray[0] = value;
        setMethod.invoke(obj, objArray);
      }
      catch (InvocationTargetException ex)
      {
        InternalOneToOneAssociation.LOG.error((object) new StringBuffer().append("exception executing ").append((object) this.setMethod).ToString(), ex.getTargetException());
        if (ex.getTargetException() is RuntimeException)
          throw (RuntimeException) ex.getTargetException();
        throw new RuntimeException(ex.getTargetException().getMessage());
      }
      catch (IllegalAccessException ex)
      {
        InternalOneToOneAssociation.LOG.error((object) new StringBuffer().append("illegal access of ").append((object) this.setMethod).ToString(), (Throwable) ex);
      }
    }

    public virtual void initValue(NakedObject inObject, object setValue)
    {
      if (InternalOneToOneAssociation.LOG.isDebugEnabled())
        InternalOneToOneAssociation.LOG.debug((object) new StringBuffer().append("init value ").append((object) this.getIdentifier()).append(" in ").append((object) inObject.getOid()).append(" - ").append(setValue).ToString());
      try
      {
        Method setMethod = this.setMethod;
        object obj = inObject.getObject();
        int length = 1;
        object[] objArray = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        objArray[0] = setValue;
        setMethod.invoke(obj, objArray);
      }
      catch (InvocationTargetException ex)
      {
        InternalOneToOneAssociation.LOG.error((object) new StringBuffer().append("exception executing ").append((object) this.setMethod).ToString(), ex.getTargetException());
        if (ex.getTargetException() is RuntimeException)
          throw (RuntimeException) ex.getTargetException();
        throw new RuntimeException(ex.getTargetException().getMessage());
      }
      catch (IllegalAccessException ex)
      {
        InternalOneToOneAssociation.LOG.error((object) new StringBuffer().append("illegal access of ").append((object) this.setMethod).ToString(), (Throwable) ex);
      }
    }

    public virtual void clearAssociation(NakedObject inObject, NakedObject associate)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual void debugData(DebugString debugString)
    {
      debugString.appendln("isObject", this.isObject);
      debugString.appendln("get method", (object) this.getMethod);
      debugString.appendln("set method", (object) this.setMethod);
      debugString.appendln("associate method", (object) this.addMethod);
      debugString.appendln("dissociate method", (object) this.removeMethod);
    }

    public virtual void initAssociation(NakedObject inObject, NakedObject associate)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual void setAssociation(NakedObject inObject, NakedObject associate)
    {
      // ISSUE: unable to decompile the method.
    }

    public override string ToString()
    {
      string str = new StringBuffer().append(this.getMethod != null ? "GET" : "").append(this.setMethod != null ? " SET" : "").append(this.addMethod != null ? " ADD" : "").append(this.removeMethod != null ? " REMOVE" : "").ToString();
      return new StringBuffer().append("Association [name=\"").append((object) this.getIdentifier()).append("\", method=").append((object) this.getMethod).append(", methods=").append(str).append(", type=").append((object) this.getType()).append(" ]").ToString();
    }

    public virtual Naked getAssociation(NakedObject fromObject)
    {
      try
      {
        Method getMethod = this.getMethod;
        object obj = fromObject.getObject();
        int length = 0;
        object[] objArray = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        object @object = getMethod.invoke(obj, objArray);
        if (@object != null)
          return (Naked) NakedObjects.getObjectLoader().createAdapterForValue(@object) ?? (Naked) NakedObjects.getObjectLoader().getAdapterForElseCreateAdapterForTransient(@object);
        if (this.getType().isOfType(NakedObjects.getSpecificationLoader().loadSpecification(Class.FromType(typeof (string)))))
          return (Naked) new StringAdapter("");
        throw new NakedObjectRuntimeException(this.getType().getFullName());
      }
      catch (InvocationTargetException ex)
      {
        InternalOneToOneAssociation.LOG.error((object) new StringBuffer().append("exception executing ").append((object) this.getMethod).ToString(), ex.getTargetException());
        throw new NakedObjectRuntimeException((Throwable) ex);
      }
      catch (IllegalAccessException ex)
      {
        InternalOneToOneAssociation.LOG.error((object) new StringBuffer().append("illegal access of ").append((object) this.getMethod).ToString(), (Throwable) ex);
        throw new NakedObjectRuntimeException((Throwable) ex);
      }
    }

    public override bool isEmpty(NakedObject inObject) => throw new NotImplementedException();

    public virtual bool isObject() => this.isObject;

    public virtual Consent isValueValid(NakedObject inObject, NakedValue value) => (Consent) Allow.DEFAULT;

    public virtual Consent isAssociationValid(NakedObject inObject, NakedObject value) => (Consent) Allow.DEFAULT;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static InternalOneToOneAssociation()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
