// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.reflect.JavaOneToManyAssociation
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.lang.reflect;
using java.util;
using org.apache.log4j;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.@object.reflect;
using org.nakedobjects.reflector.java.control;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.reflector.java.reflect
{
  [JavaInterfaces("1;org/nakedobjects/object/reflect/OneToManyPeer;")]
  public class JavaOneToManyAssociation : JavaField, OneToManyPeer
  {
    private static readonly Category LOG;
    private Method addMethod;
    private Method removeMethod;
    private Method clearMethod;

    public JavaOneToManyAssociation(
      MemberIdentifier name,
      Class type,
      Method get,
      Method add,
      Method remove,
      Method about,
      bool isHidden,
      bool isHiddenInTableViews)
      : base(name, type, get, about, isHidden, isHiddenInTableViews, add == null && remove == null)
    {
      this.addMethod = add;
      this.removeMethod = remove;
    }

    public virtual void addAssociation(NakedObject inObject, NakedObject associate)
    {
      // ISSUE: unable to decompile the method.
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
        if (StringImpl.equals(simpleFieldAbout.getDescription(), (object) ""))
          simpleFieldAbout.setDescription(new StringBuffer().append("Add ").append(element.getObject()).append(" to field ").append((object) this.getIdentifier()).ToString());
        return (Hint) simpleFieldAbout;
      }
      catch (InvocationTargetException ex)
      {
        JavaMember.invocationException(new StringBuffer().append("Exception executing ").append((object) aboutMethod).ToString(), ex);
      }
      catch (IllegalAccessException ex)
      {
        JavaOneToManyAssociation.LOG.error((object) new StringBuffer().append("illegal access of ").append((object) aboutMethod).ToString(), (Throwable) ex);
      }
      return (Hint) null;
    }

    public virtual NakedCollection getAssociations(NakedObject fromObject) => this.get(fromObject);

    public override object getExtension(Class cls) => (object) null;

    public override Class[] getExtensions()
    {
      int length = 0;
      return length >= 0 ? new Class[length] : throw new NegativeArraySizeException();
    }

    public virtual string getName() => (string) null;

    public virtual void removeAllAssociations(NakedObject inObject)
    {
      try
      {
        if (this.clearMethod == null)
          return;
        this.clearMethod.invoke((object) inObject, (object[]) null);
      }
      catch (InvocationTargetException ex)
      {
        JavaMember.invocationException(new StringBuffer().append("Exception executing ").append((object) this.clearMethod).ToString(), ex);
      }
      catch (IllegalAccessException ex)
      {
        JavaOneToManyAssociation.LOG.error((object) new StringBuffer().append("illegal access of ").append((object) this.clearMethod).ToString(), (Throwable) ex);
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

    private NakedCollection get(NakedObject fromObject)
    {
      try
      {
        Method getMethod = this.getMethod;
        object obj = fromObject.getObject();
        int length = 0;
        object[] objArray = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        object collection = getMethod.invoke(obj, objArray);
        if (collection == null)
          return (NakedCollection) null;
        return NakedObjects.getObjectLoader().getAdapterForElseCreateAdapterForCollection(fromObject, this.getIdentifier().getName(), this.getType(), collection) ?? throw new ReflectionException("no adapter created");
      }
      catch (InvocationTargetException ex)
      {
        JavaMember.invocationException(new StringBuffer().append("Exception executing ").append((object) this.getMethod).ToString(), ex);
        throw new ReflectionException((Throwable) ex);
      }
      catch (IllegalAccessException ex)
      {
        JavaOneToManyAssociation.LOG.error((object) new StringBuffer().append("illegal access of ").append((object) this.getMethod).ToString(), (Throwable) ex);
        throw new ReflectionException((Throwable) ex);
      }
    }

    public virtual bool isEmpty(NakedObject fromObject)
    {
      try
      {
        Method getMethod = this.getMethod;
        object obj1 = fromObject.getObject();
        int length = 0;
        object[] objArray = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        object obj2 = getMethod.invoke(obj1, objArray);
        if (obj2 == null)
          return true;
        return obj2 is Vector ? ((Vector) obj2).isEmpty() : throw new ReflectionException();
      }
      catch (InvocationTargetException ex)
      {
        JavaMember.invocationException(new StringBuffer().append("Exception executing ").append((object) this.getMethod).ToString(), ex);
        throw new ReflectionException((Throwable) ex);
      }
      catch (IllegalAccessException ex)
      {
        JavaOneToManyAssociation.LOG.error((object) new StringBuffer().append("illegal access of ").append((object) this.getMethod).ToString(), (Throwable) ex);
        throw new ReflectionException((Throwable) ex);
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
        switch (obj2)
        {
          case Vector _:
            ((Vector) obj2).removeAllElements();
            for (int index = 0; index < instances.Length; ++index)
              ((Vector) obj2).addElement(instances[index].getObject());
            break;
          case object[] _:
            throw new ReflectionException("not set up to deal with arrays");
          default:
            throw new ReflectionException(new StringBuffer().append("Can't initialise ").append(obj2).ToString());
        }
      }
      catch (InvocationTargetException ex)
      {
        JavaOneToManyAssociation.LOG.error((object) new StringBuffer().append("exception executing ").append((object) this.getMethod).ToString(), ex.getTargetException());
        throw new ReflectionException((Throwable) ex);
      }
      catch (IllegalAccessException ex)
      {
        JavaOneToManyAssociation.LOG.error((object) new StringBuffer().append("illegal access of ").append((object) this.getMethod).ToString(), (Throwable) ex);
        throw new ReflectionException((Throwable) ex);
      }
    }

    public virtual Consent isAvailable(NakedReference target) => this.getHint(target, (NakedObject) null, true).canUse();

    public virtual Consent isRemoveValid(NakedObject container, NakedObject element) => this.getHint((NakedReference) container, element, false).canUse();

    public virtual Consent isAddValid(NakedObject container, NakedObject element) => this.getHint((NakedReference) container, element, true).canUse();

    public virtual Consent isVisible(NakedReference target) => this.getHint(target, (NakedObject) null, true).canAccess();

    public virtual bool isAccessible() => true;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static JavaOneToManyAssociation()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
