// Decompiled with JetBrains decompiler
// Type: javax.xml.parsers.FactoryFinder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using System.ComponentModel;

namespace javax.xml.parsers
{
  [JavaFlags(32)]
  public class FactoryFinder
  {
    private static bool debug;

    private static void debugPrintln(string msg)
    {
      if (!FactoryFinder.debug)
        return;
      ((PrintStream) java.lang.System.err).println(new StringBuffer().append("JAXP: ").append(msg).ToString());
    }

    [JavaThrownExceptions("1;javax/xml/parsers/FactoryFinder+ConfigurationError;")]
    private static ClassLoader findClassLoader() => Class.FromType(typeof (FactoryFinder)).getClassLoader();

    [JavaThrownExceptions("1;javax/xml/parsers/FactoryFinder+ConfigurationError;")]
    private static object newInstance(string className, ClassLoader classLoader)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;javax/xml/parsers/FactoryFinder+ConfigurationError;")]
    [JavaFlags(8)]
    public static object find(string factoryId, string fallbackClassName)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(0)]
    public FactoryFinder()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static FactoryFinder()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      FactoryFinder factoryFinder = this;
      ObjectImpl.clone((object) factoryFinder);
      return ((object) factoryFinder).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [JavaFlags(40)]
    public class ConfigurationError : Error
    {
      private Exception exception;

      [JavaFlags(0)]
      public ConfigurationError(string msg, Exception x)
        : base(msg)
      {
        this.exception = x;
      }

      [JavaFlags(0)]
      public virtual Exception getException() => this.exception;

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public virtual object MemberwiseClone()
      {
        FactoryFinder.ConfigurationError configurationError = this;
        ObjectImpl.clone((object) configurationError);
        return ((object) configurationError).MemberwiseClone();
      }
    }
  }
}
