// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.internal.InternalOneToManyAssociation
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using java.lang.reflect;
using java.util;
using org.apache.log4j;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.@object.reflect;
using org.nakedobjects.@object.reflect.@internal;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.@object.reflect.@internal
{
  [JavaInterfaces("1;org/nakedobjects/object/reflect/OneToManyPeer;")]
  public class InternalOneToManyAssociation : InternalField, OneToManyPeer
  {
    private static readonly Category LOG;
    private Method addMethod;
    private Method removeMethod;
    private Method clearMethod;

    public InternalOneToManyAssociation(
      string className,
      string name,
      Class type,
      Method get,
      Method add,
      Method remove)
      : base(type, get, false)
    {
      this.addMethod = add;
      this.removeMethod = remove;
      this.identifeir = (MemberIdentifier) new MemberIdentifierImpl(className, name);
    }

    public virtual void addAssociation(NakedObject inObject, NakedObject associate)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual void initAssociation(NakedObject inObject, NakedObject associate)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual NakedCollection getAssociations(NakedObject fromObject) => (NakedCollection) this.get(fromObject);

    public virtual void debugData(DebugString debugString)
    {
      debugString.appendln("get method", (object) this.getMethod);
      debugString.appendln("add method", (object) this.addMethod);
      debugString.appendln("remove method", (object) this.removeMethod);
      debugString.appendln("clear method", (object) this.clearMethod);
    }

    public virtual void removeAllAssociations(NakedObject inObject)
    {
      try
      {
        this.clearMethod.invoke((object) inObject, (object[]) null);
      }
      catch (InvocationTargetException ex)
      {
        InternalOneToManyAssociation.LOG.error((object) new StringBuffer().append("exception executing ").append((object) this.clearMethod).ToString(), ex.getTargetException());
        throw (RuntimeException) ex.getTargetException();
      }
      catch (IllegalAccessException ex)
      {
        InternalOneToManyAssociation.LOG.error((object) new StringBuffer().append("illegal access of ").append((object) this.clearMethod).ToString(), (Throwable) ex);
        throw new RuntimeException(((Throwable) ex).getMessage());
      }
    }

    public virtual void removeAssociation(NakedObject inObject, NakedObject associate)
    {
      // ISSUE: unable to decompile the method.
    }

    public override string ToString()
    {
      string str = new StringBuffer().append(this.getMethod != null ? "GET" : "").append(this.addMethod != null ? " ADD" : "").append(this.removeMethod != null ? " REMOVE" : "").ToString();
      return new StringBuffer().append("OneToManyAssociation [name=\"").append((object) this.getIdentifier()).append("\", method=").append((object) this.getMethod).append(", methods=").append(str).append(", type=").append((object) this.getType()).append(" ]").ToString();
    }

    private Naked get(NakedObject fromObject)
    {
      try
      {
        Method getMethod = this.getMethod;
        object obj1 = fromObject.getObject();
        int length = 0;
        object[] objArray = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        object obj2 = getMethod.invoke(obj1, objArray);
        if (obj2 == null)
          return (Naked) null;
        return obj2 is Vector ? (Naked) new InternalCollectionVectorAdapter((Vector) obj2, this.type) : throw new NakedObjectRuntimeException();
      }
      catch (InvocationTargetException ex)
      {
        InternalOneToManyAssociation.LOG.error((object) new StringBuffer().append("exception executing ").append((object) this.getMethod).ToString(), ex.getTargetException());
        throw new NakedObjectRuntimeException((Throwable) ex);
      }
      catch (IllegalAccessException ex)
      {
        InternalOneToManyAssociation.LOG.error((object) new StringBuffer().append("illegal access of ").append((object) this.getMethod).ToString(), (Throwable) ex);
        throw new NakedObjectRuntimeException((Throwable) ex);
      }
    }

    public virtual void initOneToManyAssociation(NakedObject fromObject, NakedObject[] instances)
    {
      try
      {
        Method getMethod = this.getMethod;
        object obj1 = fromObject.getObject();
        int length = 0;
        object[] objArray = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        object obj2 = getMethod.invoke(obj1, objArray);
        if (!(obj2 is Vector))
          throw new NakedObjectRuntimeException();
        ((Vector) obj2).removeAllElements();
        for (int index = 0; index < instances.Length; ++index)
          ((Vector) obj2).addElement(instances[index].getObject());
      }
      catch (InvocationTargetException ex)
      {
        InternalOneToManyAssociation.LOG.error((object) new StringBuffer().append("exception executing ").append((object) this.getMethod).ToString(), ex.getTargetException());
        throw new NakedObjectRuntimeException((Throwable) ex);
      }
      catch (IllegalAccessException ex)
      {
        InternalOneToManyAssociation.LOG.error((object) new StringBuffer().append("illegal access of ").append((object) this.getMethod).ToString(), (Throwable) ex);
        throw new NakedObjectRuntimeException((Throwable) ex);
      }
    }

    public virtual Consent isRemoveValid(NakedObject container, NakedObject element) => (Consent) Allow.DEFAULT;

    public virtual Consent isAddValid(NakedObject container, NakedObject element) => (Consent) Allow.DEFAULT;

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static InternalOneToManyAssociation()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
