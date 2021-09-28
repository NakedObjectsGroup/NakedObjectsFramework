// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.config.PropertyGetter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.beans;
using java.lang;
using org.apache.log4j.helpers;
using System.ComponentModel;

namespace org.apache.log4j.config
{
  public class PropertyGetter
  {
    [JavaFlags(28)]
    public static readonly object[] NULL_ARG;
    [JavaFlags(4)]
    public object obj;
    [JavaFlags(4)]
    public PropertyDescriptor[] props;

    [JavaThrownExceptions("1;java/beans/IntrospectionException;")]
    public PropertyGetter(object obj)
    {
      this.props = Introspector.getBeanInfo(ObjectImpl.getClass(obj)).getPropertyDescriptors();
      this.obj = obj;
    }

    public static void getProperties(
      object obj,
      PropertyGetter.PropertyCallback callback,
      string prefix)
    {
      try
      {
        new PropertyGetter(obj).getProperties(callback, prefix);
      }
      catch (IntrospectionException ex)
      {
        LogLog.error(new StringBuffer().append("Failed to introspect object ").append(obj).ToString(), (Throwable) ex);
      }
    }

    public virtual void getProperties(PropertyGetter.PropertyCallback callback, string prefix)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4)]
    public virtual bool isHandledType(Class type) => Class.FromType(typeof (string)).isAssignableFrom(type) || ((Class) Integer.TYPE).isAssignableFrom(type) || ((Class) Long.TYPE).isAssignableFrom(type) || ((Class) Boolean.TYPE).isAssignableFrom(type) || Class.FromType(typeof (Priority)).isAssignableFrom(type);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static PropertyGetter()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      PropertyGetter propertyGetter = this;
      ObjectImpl.clone((object) propertyGetter);
      return ((object) propertyGetter).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [JavaInterface]
    [JavaFlags(1545)]
    public interface PropertyCallback
    {
      void foundProperty(object obj, string prefix, string name, object value);
    }
  }
}
