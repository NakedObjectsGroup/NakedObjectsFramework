// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.xml.examples.XMLSample
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using com.ms.vjsharp.util;
using java.io;
using java.lang;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace org.apache.log4j.xml.examples
{
  public class XMLSample
  {
    [JavaFlags(8)]
    public static Category cat;

    public static void main(string[] argv)
    {
      // ISSUE: type reference
      RuntimeHelpers.RunClassConstructor(__typeref (ObjectImpl));
      if (argv.Length == 1)
        XMLSample.init(argv[0]);
      else
        XMLSample.Usage("Wrong number of arguments.");
      XMLSample.sample();
      Utilities.cleanupAfterMainReturns();
    }

    [JavaFlags(8)]
    public static void Usage(string msg)
    {
      ((PrintStream) java.lang.System.err).println(msg);
      ((PrintStream) java.lang.System.err).println(new StringBuffer().append("Usage: java ").append(Class.FromType(typeof (XMLSample)).getName()).append("configFile").ToString());
      java.lang.System.exit(1);
    }

    [JavaFlags(8)]
    public static void init(string configFile) => DOMConfigurator.configure(configFile);

    [JavaFlags(8)]
    public static void sample()
    {
      int num1 = -1;
      Category.getRoot();
      int num2;
      XMLSample.cat.debug((object) new StringBuffer().append("Message ").append(num2 = num1 + 1).ToString());
      int num3;
      XMLSample.cat.warn((object) new StringBuffer().append("Message ").append(num3 = num2 + 1).ToString());
      int num4;
      XMLSample.cat.error((object) new StringBuffer().append("Message ").append(num4 = num3 + 1).ToString());
      Exception exception = new Exception("Just testing");
      int num5;
      XMLSample.cat.debug((object) new StringBuffer().append("Message ").append(num5 = num4 + 1).ToString(), (Throwable) exception);
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static XMLSample()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      XMLSample xmlSample = this;
      ObjectImpl.clone((object) xmlSample);
      return ((object) xmlSample).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
