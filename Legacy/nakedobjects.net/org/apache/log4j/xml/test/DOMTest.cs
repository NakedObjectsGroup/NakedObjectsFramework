// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.xml.test.DOMTest
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

namespace org.apache.log4j.xml.test
{
  public class DOMTest
  {
    [JavaFlags(8)]
    public static Category cat;

    public static void main(string[] argv)
    {
      // ISSUE: type reference
      RuntimeHelpers.RunClassConstructor(__typeref (ObjectImpl));
      if (argv.Length == 1)
        DOMTest.init(argv[0]);
      else
        DOMTest.Usage("Wrong number of arguments.");
      DOMTest.test();
      Utilities.cleanupAfterMainReturns();
    }

    [JavaFlags(8)]
    public static void Usage(string msg)
    {
      ((PrintStream) java.lang.System.err).println(msg);
      ((PrintStream) java.lang.System.err).println(new StringBuffer().append("Usage: java ").append(Class.FromType(typeof (DOMTest)).getName()).append(" configFile").ToString());
      java.lang.System.exit(1);
    }

    [JavaFlags(8)]
    public static void init(string configFile) => DOMConfigurator.configure(configFile);

    [JavaFlags(8)]
    public static void test()
    {
      int num1 = -1;
      Category root = Category.getRoot();
      int num2;
      DOMTest.cat.debug((object) new StringBuffer().append("Message ").append(num2 = num1 + 1).ToString());
      root.debug((object) new StringBuffer().append("Message ").append(num2).ToString());
      int num3;
      DOMTest.cat.info((object) new StringBuffer().append("Message ").append(num3 = num2 + 1).ToString());
      root.info((object) new StringBuffer().append("Message ").append(num3).ToString());
      int num4;
      DOMTest.cat.warn((object) new StringBuffer().append("Message ").append(num4 = num3 + 1).ToString());
      root.warn((object) new StringBuffer().append("Message ").append(num4).ToString());
      int num5;
      DOMTest.cat.error((object) new StringBuffer().append("Message ").append(num5 = num4 + 1).ToString());
      root.error((object) new StringBuffer().append("Message ").append(num5).ToString());
      int num6;
      DOMTest.cat.log(Priority.FATAL, (object) new StringBuffer().append("Message ").append(num6 = num5 + 1).ToString());
      root.log(Priority.FATAL, (object) new StringBuffer().append("Message ").append(num6).ToString());
      Exception exception = new Exception("Just testing");
      int num7;
      DOMTest.cat.debug((object) new StringBuffer().append("Message ").append(num7 = num6 + 1).ToString(), (Throwable) exception);
      root.debug((object) new StringBuffer().append("Message ").append(num7).ToString(), (Throwable) exception);
      int num8;
      DOMTest.cat.error((object) new StringBuffer().append("Message ").append(num8 = num7 + 1).ToString(), (Throwable) exception);
      root.error((object) new StringBuffer().append("Message ").append(num8).ToString(), (Throwable) exception);
      Category.shutdown();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static DOMTest()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      DOMTest domTest = this;
      ObjectImpl.clone((object) domTest);
      return ((object) domTest).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
