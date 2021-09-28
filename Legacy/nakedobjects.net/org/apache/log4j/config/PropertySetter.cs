// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.config.PropertySetter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.beans;
using java.lang;
using java.util;
using org.apache.log4j.helpers;
using org.apache.log4j.spi;

namespace org.apache.log4j.config
{
  public class PropertySetter
  {
    [JavaFlags(4)]
    public object obj;
    [JavaFlags(4)]
    public PropertyDescriptor[] props;

    public PropertySetter(object obj) => this.obj = obj;

    [JavaFlags(4)]
    public virtual void introspect()
    {
      try
      {
        this.props = Introspector.getBeanInfo(ObjectImpl.getClass(this.obj)).getPropertyDescriptors();
      }
      catch (IntrospectionException ex)
      {
        LogLog.error(new StringBuffer().append("Failed to introspect ").append(this.obj).append(": ").append(((Throwable) ex).getMessage()).ToString());
        int length = 0;
        this.props = length >= 0 ? new PropertyDescriptor[length] : throw new NegativeArraySizeException();
      }
    }

    public static void setProperties(object obj, Properties properties, string prefix) => new PropertySetter(obj).setProperties(properties, prefix);

    public virtual void setProperties(Properties properties, string prefix)
    {
      int num = StringImpl.length(prefix);
      Enumeration enumeration = properties.propertyNames();
      while (enumeration.hasMoreElements())
      {
        string key = \u003CVerifierFix\u003E.genCastToString(enumeration.nextElement());
        if (StringImpl.startsWith(key, prefix) && StringImpl.indexOf(key, 46, num + 1) <= 0)
        {
          string andSubst = OptionConverter.findAndSubst(key, properties);
          string name = StringImpl.substring(key, num);
          if (!StringImpl.equals("layout", (object) name) || !(this.obj is Appender))
            this.setProperty(name, andSubst);
        }
      }
      this.activate();
    }

    public virtual void setProperty(string name, string value)
    {
      if (value == null)
        return;
      name = Introspector.decapitalize(name);
      PropertyDescriptor propertyDescriptor = this.getPropertyDescriptor(name);
      if (propertyDescriptor == null)
      {
        LogLog.warn(new StringBuffer().append("No such property [").append(name).append("] in ").append(ObjectImpl.getClass(this.obj).getName()).append(".").ToString());
      }
      else
      {
        try
        {
          this.setProperty(propertyDescriptor, name, value);
        }
        catch (PropertySetterException ex)
        {
          LogLog.warn(new StringBuffer().append("Failed to set property [").append(name).append("] to value \"").append(value).append("\". ").ToString(), ex.rootCause);
        }
      }
    }

    [JavaThrownExceptions("1;org/apache/log4j/config/PropertySetterException;")]
    public virtual void setProperty(PropertyDescriptor prop, string name, string value)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4)]
    public virtual object convertArg(string val, Class type)
    {
      if (val == null)
        return (object) null;
      string str = StringImpl.trim(val);
      if (Class.FromType(typeof (string)).isAssignableFrom(type))
        return (object) val;
      if (((Class) Integer.TYPE).isAssignableFrom(type))
        return (object) new Integer(str);
      if (((Class) Long.TYPE).isAssignableFrom(type))
        return (object) new Long(str);
      if (((Class) Boolean.TYPE).isAssignableFrom(type))
      {
        if (StringImpl.equalsIgnoreCase("true", str))
          return (object) Boolean.TRUE;
        if (StringImpl.equalsIgnoreCase("false", str))
          return (object) Boolean.FALSE;
      }
      else if (Class.FromType(typeof (Priority)).isAssignableFrom(type))
        return (object) OptionConverter.toLevel(str, Level.DEBUG);
      return (object) null;
    }

    [JavaFlags(4)]
    public virtual PropertyDescriptor getPropertyDescriptor(string name)
    {
      if (this.props == null)
        this.introspect();
      for (int index = 0; index < this.props.Length; ++index)
      {
        if (StringImpl.equals(name, (object) ((FeatureDescriptor) this.props[index]).getName()))
          return this.props[index];
      }
      return (PropertyDescriptor) null;
    }

    public virtual void activate()
    {
      if (!(this.obj is OptionHandler))
        return;
      ((OptionHandler) this.obj).activateOptions();
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      PropertySetter propertySetter = this;
      ObjectImpl.clone((object) propertySetter);
      return ((object) propertySetter).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
