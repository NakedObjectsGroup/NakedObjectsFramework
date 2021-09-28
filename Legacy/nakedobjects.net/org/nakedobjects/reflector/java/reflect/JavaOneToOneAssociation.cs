// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.reflect.JavaOneToOneAssociation
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.lang.reflect;
using org.apache.log4j;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.@object.reflect;
using org.nakedobjects.application;
using org.nakedobjects.application.valueholder;
using org.nakedobjects.reflector.java.control;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.reflector.java.reflect
{
  [JavaInterfaces("1;org/nakedobjects/object/reflect/OneToOnePeer;")]
  public class JavaOneToOneAssociation : JavaField, OneToOnePeer
  {
    private static readonly Category LOG;
    [JavaFlags(4)]
    public Method associateMethod;
    [JavaFlags(4)]
    public Method dissociateMethod;
    [JavaFlags(4)]
    public Method setMethod;
    private bool isObject;

    public JavaOneToOneAssociation(
      bool isObject,
      MemberIdentifier identifier,
      Class type,
      Method get,
      Method set,
      Method associate,
      Method dissociate,
      Method about,
      bool isHidden,
      bool isHiddenInTableViews,
      bool derived)
      : base(identifier, type, get, about, isHidden, isHiddenInTableViews, derived)
    {
      this.setMethod = set;
      this.associateMethod = associate;
      this.dissociateMethod = dissociate;
      this.isObject = isObject;
    }

    public virtual void clearAssociation(NakedObject inObject, NakedObject associate)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual void debugData(DebugString debugString)
    {
      debugString.appendln("Identifier", (object) this.getIdentifier());
      debugString.appendln("IsObject", this.isObject);
      debugString.appendln("Get method", (object) this.getMethod);
      if (this.setMethod != null)
        debugString.appendln("Set method", (object) this.setMethod);
      if (this.associateMethod != null)
        debugString.appendln("Associate method", (object) this.associateMethod);
      if (this.dissociateMethod != null)
        debugString.appendln("Dissociate method", (object) this.dissociateMethod);
      if (this.getAboutMethod() == null)
        return;
      debugString.appendln("About method", (object) this.getAboutMethod());
    }

    private Naked get(NakedObject fromObject)
    {
      try
      {
        Method getMethod = this.getMethod;
        object obj = fromObject.getObject();
        int length = 0;
        object[] objArray = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        object @object = getMethod.invoke(obj, objArray);
        return @object == null ? (Naked) null : (Naked) NakedObjects.getObjectLoader().createAdapterForValue(@object) ?? (Naked) NakedObjects.getObjectLoader().getAdapterForElseCreateAdapterForTransient(@object);
      }
      catch (InvocationTargetException ex)
      {
        JavaMember.invocationException(new StringBuffer().append("Exception executing ").append((object) this.getMethod).ToString(), ex);
        return (Naked) null;
      }
      catch (IllegalAccessException ex)
      {
        JavaOneToOneAssociation.LOG.error((object) new StringBuffer().append("illegal access of ").append((object) this.getMethod).ToString(), (Throwable) ex);
        throw new ReflectionException((Throwable) ex);
      }
    }

    public virtual Naked getAssociation(NakedObject fromObject) => this.get(fromObject);

    public override object getExtension(Class cls) => (object) null;

    public override Class[] getExtensions()
    {
      int length = 0;
      return length >= 0 ? new Class[length] : throw new NegativeArraySizeException();
    }

    public virtual string getName() => (string) null;

    private Hint getHint(NakedReference @object, Naked associate)
    {
      Method aboutMethod = this.getAboutMethod();
      Class returnType = this.getMethod.getReturnType();
      object object1 = @object.getObject();
      if (associate != null && associate.getObject() != null && !returnType.isAssignableFrom(ObjectImpl.getClass(associate.getObject())))
      {
        SimpleFieldAbout simpleFieldAbout = new SimpleFieldAbout(NakedObjects.getCurrentSession(), object1);
        simpleFieldAbout.unmodifiable(new StringBuffer().append("Invalid type: field must be set with a ").append((object) NakedObjects.getSpecificationLoader().loadSpecification(returnType.getName())).ToString());
        return (Hint) simpleFieldAbout;
      }
      if (aboutMethod == null)
        return (Hint) new DefaultHint();
      try
      {
        SimpleFieldAbout simpleFieldAbout = new SimpleFieldAbout(NakedObjects.getCurrentSession(), object1);
        object[] objArray1;
        if (aboutMethod.getParameterTypes().Length == 2)
        {
          int length = 2;
          object[] objArray2 = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          objArray2[0] = (object) simpleFieldAbout;
          objArray2[1] = associate?.getObject();
          objArray1 = objArray2;
        }
        else
        {
          int length = 1;
          object[] objArray3 = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          objArray3[0] = (object) simpleFieldAbout;
          objArray1 = objArray3;
        }
        aboutMethod.invoke(object1, objArray1);
        if (StringImpl.equals(simpleFieldAbout.getDescription(), (object) "") && associate != null)
          simpleFieldAbout.setDescription(new StringBuffer().append("Set field ").append((object) this.getIdentifier()).append(" to ").append(associate.getObject()).ToString());
        return (Hint) simpleFieldAbout;
      }
      catch (InvocationTargetException ex)
      {
        JavaMember.invocationException(new StringBuffer().append("Exception executing ").append((object) aboutMethod).ToString(), ex);
      }
      catch (IllegalAccessException ex)
      {
        JavaOneToOneAssociation.LOG.error((object) new StringBuffer().append("illegal access of ").append((object) aboutMethod).ToString(), (Throwable) ex);
      }
      return (Hint) new DefaultHint();
    }

    public virtual bool hasAddMethod() => this.associateMethod != null;

    public virtual void initAssociation(NakedObject inObject, NakedObject associate)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual void initValue(NakedObject inObject, object setValue)
    {
      if (JavaOneToOneAssociation.LOG.isDebugEnabled())
        JavaOneToOneAssociation.LOG.debug((object) new StringBuffer().append("local initValue() ").append((object) this.getIdentifier()).append(" ").append((object) inObject.getOid()).append("/").append(setValue).ToString());
      try
      {
        if (this.setMethod == null)
        {
          Method getMethod = this.getMethod;
          object obj = inObject.getObject();
          int length = 0;
          object[] objArray = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          BusinessValueHolder businessValueHolder = (BusinessValueHolder) getMethod.invoke(obj, objArray);
          if (\u003CVerifierFix\u003E.isInstanceOfString(setValue))
          {
            businessValueHolder.parseUserEntry(\u003CVerifierFix\u003E.genCastToString(setValue));
          }
          else
          {
            if (!(setValue is BusinessValueHolder))
              return;
            businessValueHolder.copyObject((BusinessValueHolder) setValue);
          }
        }
        else
        {
          Method setMethod = this.setMethod;
          object obj = inObject.getObject();
          int length = 1;
          object[] objArray = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          objArray[0] = setValue != null ? setValue : (object) null;
          setMethod.invoke(obj, objArray);
        }
      }
      catch (InvocationTargetException ex)
      {
        JavaMember.invocationException(new StringBuffer().append("Exception executing ").append((object) this.setMethod).ToString(), ex);
      }
      catch (IllegalAccessException ex)
      {
        JavaOneToOneAssociation.LOG.error((object) new StringBuffer().append("illegal access of ").append((object) this.setMethod).ToString(), (Throwable) ex);
      }
      catch (ValueParseException ex)
      {
        JavaOneToOneAssociation.LOG.error((object) new StringBuffer().append("parse error: ").append(setValue).ToString(), (Throwable) ex);
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
        return obj2 is BusinessValueHolder ? ((BusinessValueHolder) obj2).isEmpty() : obj2 == null;
      }
      catch (InvocationTargetException ex)
      {
        JavaMember.invocationException(new StringBuffer().append("Exception executing ").append((object) this.getMethod).ToString(), ex);
        throw new ReflectionException((Throwable) ex);
      }
      catch (IllegalAccessException ex)
      {
        JavaOneToOneAssociation.LOG.error((object) new StringBuffer().append("illegal access of ").append((object) this.getMethod).ToString(), (Throwable) ex);
        throw new ReflectionException((Throwable) ex);
      }
    }

    public override bool isMandatory() => false;

    public virtual bool isObject() => this.isObject;

    [JavaThrownExceptions("2;org/nakedobjects/object/TextEntryParseException;org/nakedobjects/object/InvalidEntryException;")]
    public virtual void parseTextEntry(NakedObject inObject, string text)
    {
      if (this.setMethod == null)
      {
        try
        {
          Method getMethod = this.getMethod;
          object obj = inObject.getObject();
          int length = 0;
          object[] objArray = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          ((BusinessValueHolder) getMethod.invoke(obj, objArray)).parseUserEntry(text);
          NakedObjects.getObjectPersistor().objectChanged(inObject);
        }
        catch (InvocationTargetException ex)
        {
          JavaMember.invocationException(new StringBuffer().append("Exception executing ").append((object) this.getMethod).ToString(), ex);
        }
        catch (IllegalAccessException ex)
        {
          JavaOneToOneAssociation.LOG.error((object) new StringBuffer().append("illegal access of ").append((object) this.getMethod).ToString(), (Throwable) ex);
          throw new ReflectionException(new StringBuffer().append("Illegal access of ").append((object) this.getMethod).ToString(), (Throwable) ex);
        }
        catch (ValueParseException ex)
        {
          throw new TextEntryParseException((Throwable) ex);
        }
      }
      else
      {
        try
        {
          Method setMethod = this.setMethod;
          object obj = inObject.getObject();
          int length = 1;
          object[] objArray = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          objArray[0] = (object) text;
          setMethod.invoke(obj, objArray);
        }
        catch (InvocationTargetException ex)
        {
          JavaMember.invocationException(new StringBuffer().append("Exception executing ").append((object) this.setMethod).ToString(), ex);
        }
        catch (IllegalAccessException ex)
        {
          JavaOneToOneAssociation.LOG.error((object) new StringBuffer().append("llegal access of ").append((object) this.setMethod).ToString(), (Throwable) ex);
          throw new ReflectionException(new StringBuffer().append("Illegal access of ").append((object) this.setMethod).ToString(), (Throwable) ex);
        }
      }
    }

    public virtual void setAssociation(NakedObject inObject, NakedObject associate)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual void setValue(NakedObject inObject, object setValue)
    {
      if (JavaOneToOneAssociation.LOG.isDebugEnabled())
        JavaOneToOneAssociation.LOG.debug((object) new StringBuffer().append("local setValue() ").append((object) inObject.getOid()).append("/").append((object) this.getIdentifier()).append("/").append(setValue).ToString());
      try
      {
        if (this.setMethod == null)
        {
          NakedObjects.getObjectPersistor().objectChanged(inObject);
        }
        else
        {
          Method setMethod = this.setMethod;
          object obj = inObject.getObject();
          int length = 1;
          object[] objArray = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
          objArray[0] = setValue;
          setMethod.invoke(obj, objArray);
        }
      }
      catch (InvocationTargetException ex)
      {
        JavaMember.invocationException(new StringBuffer().append("Exception executing ").append((object) this.setMethod).ToString(), ex);
      }
      catch (IllegalAccessException ex)
      {
        JavaOneToOneAssociation.LOG.error((object) new StringBuffer().append("illegal access of ").append((object) this.setMethod).ToString(), (Throwable) ex);
      }
      catch (ValueParseException ex)
      {
        ValueParseException valueParseException1 = ex;
        ((Throwable) valueParseException1).printStackTrace();
        ValueParseException valueParseException2 = valueParseException1;
        if (valueParseException2 != ex)
          throw valueParseException2;
        throw;
      }
    }

    public override string ToString()
    {
      string str = new StringBuffer().append(this.getMethod != null ? "GET" : "").append(this.setMethod != null ? " SET" : "").append(this.associateMethod != null ? " ADD" : "").append(this.dissociateMethod != null ? " REMOVE" : "").ToString();
      return new StringBuffer().append("Association [name=\"").append((object) this.getIdentifier()).append("\", method=").append((object) this.getMethod).append(",about=").append((object) this.getAboutMethod()).append(", methods=").append(str).append(", type=").append((object) this.getType()).append(" ]").ToString();
    }

    public virtual Consent isAssociationValid(NakedObject inObject, NakedObject value) => this.getHint((NakedReference) inObject, (Naked) value).canUse();

    public virtual Consent isValueValid(NakedObject inObject, NakedValue value) => this.getHint((NakedReference) inObject, (Naked) value).isValid();

    public virtual Consent isAvailable(NakedReference target) => this.getHint(target, (Naked) null).canUse();

    public virtual Consent isVisible(NakedReference target) => this.getHint(target, (Naked) null).canAccess();

    public virtual TypedNakedCollection proposedOptions(NakedReference target) => (TypedNakedCollection) null;

    public virtual bool isAccessible() => true;

    public override string getDescription() => "";

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static JavaOneToOneAssociation()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
