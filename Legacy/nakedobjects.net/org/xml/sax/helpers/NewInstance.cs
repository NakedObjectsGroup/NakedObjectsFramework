// Decompiled with JetBrains decompiler
// Type: org.xml.sax.helpers.NewInstance
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.lang.reflect;

namespace org.xml.sax.helpers
{
  [JavaFlags(32)]
  public class NewInstance
  {
    [JavaThrownExceptions("3;java/lang/ClassNotFoundException;java/lang/IllegalAccessException;java/lang/InstantiationException;")]
    [JavaFlags(8)]
    public static object newInstance(ClassLoader classLoader, string className) => (classLoader != null ? classLoader.loadClass(className) : Class.forName(className)).newInstance();

    [JavaFlags(8)]
    public static ClassLoader getClassLoader()
    {
      Method method;
      try
      {
        method = Class.FromType(typeof (Thread)).getMethod("getContextClassLoader", (Class[]) null);
      }
      catch (NoSuchMethodException ex)
      {
        return Class.FromType(typeof (NewInstance)).getClassLoader();
      }
      try
      {
        return (ClassLoader) method.invoke((object) Thread.currentThread(), (object[]) null);
      }
      catch (IllegalAccessException ex)
      {
        throw new UnknownError(((Throwable) ex).getMessage());
      }
      catch (InvocationTargetException ex)
      {
        throw new UnknownError(((Throwable) ex).getMessage());
      }
    }

    [JavaFlags(0)]
    public NewInstance()
    {
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      NewInstance newInstance = this;
      ObjectImpl.clone((object) newInstance);
      return ((object) newInstance).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
