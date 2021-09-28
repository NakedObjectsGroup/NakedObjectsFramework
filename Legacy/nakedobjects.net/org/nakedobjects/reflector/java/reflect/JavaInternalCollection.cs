// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.reflect.JavaInternalCollection
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
using org.nakedobjects.application.collection;
using org.nakedobjects.reflector.java.control;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.reflector.java.reflect
{
  [JavaInterfaces("1;org/nakedobjects/object/reflect/OneToManyPeer;")]
  public class JavaInternalCollection : JavaField, OneToManyPeer
  {
    private static readonly Category LOG;
    private Method addMethod;
    private Method removeMethod;
    private Method clearMethod;

    public JavaInternalCollection(
      MemberIdentifier identifier,
      Class type,
      Method get,
      Method add,
      Method remove,
      Method about,
      bool isHidden,
      bool isHiddenInTableViews)
      : base(identifier, type, get, about, isHidden, isHiddenInTableViews, false)
    {
      this.addMethod = add;
      this.removeMethod = remove;
    }

    public virtual void debugData(DebugString debugString)
    {
      debugString.appendln("Identifier", (object) this.getIdentifier());
      debugString.appendln("Get method", (object) this.getMethod);
      if (this.addMethod != null)
        debugString.appendln("Add method", (object) this.addMethod);
      if (this.removeMethod != null)
        debugString.appendln("Remove method", (object) this.removeMethod);
      if (this.clearMethod != null)
        debugString.appendln("Clear method", (object) this.clearMethod);
      if (this.getAboutMethod() == null)
        return;
      debugString.appendln("About method", (object) this.getAboutMethod());
    }

    public virtual void addAssociation(NakedObject inObject, NakedObject associate)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual void initAssociation(NakedObject inObject, NakedObject associate)
    {
      // ISSUE: unable to decompile the method.
    }

    private Hint getHint(NakedReference @object, NakedObject element, bool add)
    {
      Method aboutMethod = this.getAboutMethod();
      if (aboutMethod == null)
        return (Hint) new DefaultHint();
      try
      {
        SimpleFieldAbout simpleFieldAbout = new SimpleFieldAbout(NakedObjects.getCurrentSession(), @object.getObject());
        object[] objArray1;
        if (aboutMethod.getParameterTypes().Length == 3)
        {
          int length = 3;
          object[] objArray2 = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          objArray2[0] = (object) simpleFieldAbout;
          objArray2[1] = element?.getObject();
          objArray2[2] = (object) new Boolean(add);
          objArray1 = objArray2;
        }
        else
        {
          int length = 1;
          object[] objArray3 = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          objArray3[0] = (object) simpleFieldAbout;
          objArray1 = objArray3;
        }
        aboutMethod.invoke(@object.getObject(), objArray1);
        return (Hint) simpleFieldAbout;
      }
      catch (InvocationTargetException ex)
      {
        JavaMember.invocationException(new StringBuffer().append("Exception executing ").append((object) aboutMethod).ToString(), ex);
      }
      catch (IllegalAccessException ex)
      {
        JavaInternalCollection.LOG.error((object) new StringBuffer().append("illegal access of ").append((object) aboutMethod).ToString(), (Throwable) ex);
      }
      return (Hint) null;
    }

    public virtual string getName() => (string) null;

    public virtual NakedCollection getAssociations(NakedObject fromObject) => (NakedCollection) this.get(fromObject);

    public override object getExtension(Class cls) => (object) null;

    public override Class[] getExtensions()
    {
      int length = 0;
      return length >= 0 ? new Class[length] : throw new NegativeArraySizeException();
    }

    public virtual void removeAllAssociations(NakedObject inObject)
    {
      try
      {
        this.clearMethod.invoke((object) inObject, (object[]) null);
      }
      catch (InvocationTargetException ex)
      {
        JavaMember.invocationException(new StringBuffer().append("Exception executing ").append((object) this.clearMethod).ToString(), ex);
      }
      catch (IllegalAccessException ex)
      {
        JavaInternalCollection.LOG.error((object) new StringBuffer().append("illegal access of ").append((object) this.clearMethod).ToString(), (Throwable) ex);
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
      return new StringBuffer().append("OneToManyAssociation [name=\"").append((object) this.getIdentifier()).append("\", method=").append((object) this.getMethod).append(",about=").append((object) this.getAboutMethod()).append(", methods=").append(str).append(", type=").append((object) this.getType()).append(" ]").ToString();
    }

    private Naked get(NakedObject fromObject)
    {
      try
      {
        Method getMethod = this.getMethod;
        object obj = fromObject.getObject();
        int length = 0;
        object[] objArray = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        InternalCollection internalCollection = (InternalCollection) getMethod.invoke(obj, objArray);
        return (Naked) (NakedObjects.getObjectLoader().getAdapterForElseCreateAdapterForCollection(fromObject, this.getIdentifier().getName(), this.getType(), (object) internalCollection) ?? throw new ReflectionException());
      }
      catch (InvocationTargetException ex)
      {
        JavaMember.invocationException(new StringBuffer().append("Exception executing ").append((object) this.getMethod).ToString(), ex);
        throw new ReflectionException((Throwable) ex);
      }
      catch (IllegalAccessException ex)
      {
        JavaInternalCollection.LOG.error((object) new StringBuffer().append("illegal access of ").append((object) this.getMethod).ToString(), (Throwable) ex);
        throw new ReflectionException((Throwable) ex);
      }
    }

    public virtual bool isEmpty(NakedObject fromObject)
    {
      try
      {
        Method getMethod = this.getMethod;
        object obj = fromObject.getObject();
        int length = 0;
        object[] objArray = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        return ((InternalCollection) getMethod.invoke(obj, objArray)).isEmpty();
      }
      catch (InvocationTargetException ex)
      {
        JavaMember.invocationException(new StringBuffer().append("Exception executing ").append((object) this.getMethod).ToString(), ex);
        throw new ReflectionException((Throwable) ex);
      }
      catch (IllegalAccessException ex)
      {
        JavaInternalCollection.LOG.error((object) new StringBuffer().append("illegal access of ").append((object) this.getMethod).ToString(), (Throwable) ex);
        throw new ReflectionException((Throwable) ex);
      }
    }

    public virtual Consent isAvailable(NakedReference target) => this.getHint(target, (NakedObject) null, true).canUse();

    public virtual void initOneToManyAssociation(NakedObject fromObject, NakedObject[] instances)
    {
      try
      {
        Method getMethod = this.getMethod;
        object obj = fromObject.getObject();
        int length = 0;
        object[] objArray = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        InternalCollection internalCollection = (InternalCollection) getMethod.invoke(obj, objArray);
        internalCollection.removeAllElements();
        for (int index = 0; index < instances.Length; ++index)
          internalCollection.add(instances[index].getObject());
      }
      catch (InvocationTargetException ex)
      {
        JavaInternalCollection.LOG.error((object) new StringBuffer().append("exception executing ").append((object) this.getMethod).ToString(), ex.getTargetException());
        throw new ReflectionException((Throwable) ex);
      }
      catch (IllegalAccessException ex)
      {
        JavaInternalCollection.LOG.error((object) new StringBuffer().append("illegal access of ").append((object) this.getMethod).ToString(), (Throwable) ex);
        throw new ReflectionException((Throwable) ex);
      }
    }

    public override NakedObjectSpecification getType() => this.type == null ? NakedObjects.getSpecificationLoader().loadSpecification(Class.FromType(typeof (object))) : base.getType();

    public virtual Consent isRemoveValid(NakedObject container, NakedObject element) => this.getHint((NakedReference) container, element, false).canUse();

    public virtual Consent isAddValid(NakedObject container, NakedObject element) => this.getHint((NakedReference) container, element, true).canUse();

    public virtual Consent isVisible(NakedReference target) => this.getHint(target, (NakedObject) null, true).canAccess();

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static JavaInternalCollection()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
