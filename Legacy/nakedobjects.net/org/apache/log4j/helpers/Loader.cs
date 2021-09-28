// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.helpers.Loader
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.lang.reflect;
using java.net;
using System;
using System.ComponentModel;

namespace org.apache.log4j.helpers
{
  public class Loader
  {
    [JavaFlags(24)]
    public const string TSTR = "Caught Exception while in Loader.getResource. This may be innocuous.";
    private static bool java1;
    private static bool ignoreTCL;

    public static URL getResource(string resource)
    {
      try
      {
        if (!Loader.java1)
        {
          ClassLoader tcl = Loader.getTCL();
          if (tcl != null)
          {
            LogLog.debug(new StringBuffer().append("Trying to find [").append(resource).append("] using context classloader ").append((object) tcl).append(".").ToString());
            URL resource1 = tcl.getResource(resource);
            if (resource1 != null)
              return resource1;
          }
        }
        ClassLoader classLoader = Class.FromType(typeof (Loader)).getClassLoader();
        if (classLoader != null)
        {
          LogLog.debug(new StringBuffer().append("Trying to find [").append(resource).append("] using ").append((object) classLoader).append(" class loader.").ToString());
          URL resource2 = classLoader.getResource(resource);
          if (resource2 != null)
            return resource2;
        }
      }
      catch (Exception ex)
      {
        LogLog.warn("Caught Exception while in Loader.getResource. This may be innocuous.", ThrowableWrapper.wrapThrowable(ex));
      }
      LogLog.debug(new StringBuffer().append("Trying to find [").append(resource).append("] using ClassLoader.getSystemResource().").ToString());
      return ClassLoader.getSystemResource(resource);
    }

    public static bool isJava1() => Loader.java1;

    [JavaThrownExceptions("2;java/lang/IllegalAccessException;java/lang/reflect/InvocationTargetException;")]
    private static ClassLoader getTCL()
    {
      Method method;
      try
      {
        method = Class.FromType(typeof (Thread)).getMethod("getContextClassLoader", (Class[]) null);
      }
      catch (NoSuchMethodException ex)
      {
        return (ClassLoader) null;
      }
      return (ClassLoader) method.invoke((object) Thread.currentThread(), (object[]) null);
    }

    [JavaThrownExceptions("1;java/lang/ClassNotFoundException;")]
    public static Class loadClass(string clazz)
    {
      if (!Loader.java1)
      {
        if (!Loader.ignoreTCL)
        {
          try
          {
            return Loader.getTCL().loadClass(clazz);
          }
          catch (Exception ex)
          {
            ThrowableWrapper.wrapThrowable(ex);
            return Class.forName(clazz);
          }
        }
      }
      return Class.forName(clazz);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static Loader()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      Loader loader = this;
      ObjectImpl.clone((object) loader);
      return ((object) loader).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
