// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.JavaObjectFactory
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.lang.reflect;
using org.apache.log4j;
using org.nakedobjects.@object;
using org.nakedobjects.@object.reflect;
using org.nakedobjects.application;
using System;
using System.ComponentModel;

namespace org.nakedobjects.reflector.java
{
  [JavaInterfaces("1;org/nakedobjects/object/ObjectFactory;")]
  public class JavaObjectFactory : ObjectFactory
  {
    private static readonly Logger LOG;
    private BusinessObjectContainer container;

    public virtual void setContainer(BusinessObjectContainer container) => this.container = container;

    public virtual object createValueObject(NakedObjectSpecification specification)
    {
      string fullName = specification.getFullName();
      if (StringImpl.equals(fullName, (object) "boolean"))
        return (object) new Boolean(false);
      if (StringImpl.equals(fullName, (object) "char"))
        return (object) new Character(' ');
      if (StringImpl.equals(fullName, (object) "byte"))
        return (object) new Byte((sbyte) 0);
      if (StringImpl.equals(fullName, (object) "short"))
        return (object) new Short((short) 0);
      if (StringImpl.equals(fullName, (object) "int"))
        return (object) new Integer(0);
      if (StringImpl.equals(fullName, (object) "long"))
        return (object) new Long(0L);
      if (StringImpl.equals(fullName, (object) "float"))
        return (object) new Float(0.0f);
      return StringImpl.equals(fullName, (object) "double") ? (object) new Double(0.0) : this.createObject(this.classFor(specification));
    }

    public virtual void setUpAsNewLogicalObject(object @object)
    {
      Class cls = ObjectImpl.getClass(@object);
      object target = @object;
      int length1 = 0;
      Class[] parameterTypes = length1 >= 0 ? new Class[length1] : throw new NegativeArraySizeException();
      int length2 = 0;
      object[] parameters = length2 >= 0 ? new object[length2] : throw new NegativeArraySizeException();
      this.invokeMethod(cls, target, "created", parameterTypes, parameters);
    }

    public virtual object createObject(NakedObjectSpecification specification)
    {
      Class cls = this.classFor(specification);
      object @object = this.createObject(cls);
      this.setContainer(@object, cls);
      return @object;
    }

    public virtual object createFakeObject(NakedObjectSpecification specification)
    {
      Class cls = this.classFor(specification);
      object @object = this.createObject(cls);
      this.setContainer(@object, cls);
      return @object;
    }

    private Class classFor(NakedObjectSpecification specification)
    {
      string fullName = specification.getFullName();
      try
      {
        return Class.forName(fullName);
      }
      catch (ClassNotFoundException ex)
      {
        if (StringImpl.equals(fullName, (object) "boolean"))
          return (Class) Boolean.TYPE;
        if (StringImpl.equals(fullName, (object) "char"))
          return (Class) Character.TYPE;
        if (StringImpl.equals(fullName, (object) "byte"))
          return (Class) Byte.TYPE;
        if (StringImpl.equals(fullName, (object) "short"))
          return (Class) Short.TYPE;
        if (StringImpl.equals(fullName, (object) "int"))
          return (Class) Integer.TYPE;
        if (StringImpl.equals(fullName, (object) "long"))
          return (Class) Long.TYPE;
        if (StringImpl.equals(fullName, (object) "float"))
          return (Class) Float.TYPE;
        if (StringImpl.equals(fullName, (object) "double"))
          return (Class) Double.TYPE;
        throw new ReflectionException((Throwable) ex);
      }
    }

    private object createObject(Class cls)
    {
      if (Modifier.isAbstract(cls.getModifiers()))
        throw new ReflectionException(new StringBuffer().append("Cannot create an instance of an abstract class: ").append((object) cls).ToString());
      object @object;
      try
      {
        @object = cls.newInstance();
      }
      catch (InstantiationException ex)
      {
        throw new ReflectionException(new StringBuffer().append("Cannot create an instance of ").append(cls.getName()).ToString(), (Throwable) ex);
      }
      catch (IllegalAccessException ex)
      {
        throw new ReflectionException(new StringBuffer().append("Cannot access the default constructor in ").append(cls.getName()).ToString());
      }
      this.setContainer(@object, cls);
      return @object;
    }

    public virtual void initRecreatedObject(object @object)
    {
      Class cls = ObjectImpl.getClass(@object);
      this.setContainer(@object, cls);
    }

    private void setContainer(object @object, Class cls)
    {
      Class cls1 = cls;
      object target = @object;
      int length1 = 1;
      Class[] parameterTypes = length1 >= 0 ? new Class[length1] : throw new NegativeArraySizeException();
      parameterTypes[0] = Class.FromType(typeof (BusinessObjectContainer));
      int length2 = 1;
      object[] parameters = length2 >= 0 ? new object[length2] : throw new NegativeArraySizeException();
      parameters[0] = (object) this.container;
      this.invokeMethod(cls1, target, nameof (setContainer), parameterTypes, parameters);
    }

    private void invokeMethod(
      Class cls,
      object target,
      string methodName,
      Class[] parameterTypes,
      object[] parameters)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4)]
    [JavaThrownExceptions("1;java/lang/Throwable;")]
    public override void Finalize()
    {
      try
      {
        // ISSUE: explicit finalizer call
        base.Finalize();
        if (!JavaObjectFactory.LOG.isInfoEnabled())
          return;
        JavaObjectFactory.LOG.info((object) new StringBuffer().append("finalizing java business object container ").append((object) this).ToString());
      }
      catch (Exception ex)
      {
      }
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static JavaObjectFactory()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      JavaObjectFactory javaObjectFactory = this;
      ObjectImpl.clone((object) javaObjectFactory);
      return ((object) javaObjectFactory).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
