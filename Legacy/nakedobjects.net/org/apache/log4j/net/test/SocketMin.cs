// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.net.test.SocketMin
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

namespace org.apache.log4j.net.test
{
  public class SocketMin
  {
    [JavaFlags(8)]
    public static Category cat;
    [JavaFlags(8)]
    public static SocketAppender s;

    public static void main(string[] argv)
    {
      // ISSUE: type reference
      RuntimeHelpers.RunClassConstructor(__typeref (ObjectImpl));
      if (argv.Length == 3)
        SocketMin.init(argv[0], argv[1]);
      else
        SocketMin.usage("Wrong number of arguments.");
      NDC.push("some context");
      if (StringImpl.equals(argv[2], (object) "true"))
        SocketMin.loop();
      else
        SocketMin.test();
      SocketMin.s.close();
      Utilities.cleanupAfterMainReturns();
    }

    [JavaFlags(8)]
    public static void usage(string msg)
    {
      ((PrintStream) java.lang.System.err).println(msg);
      ((PrintStream) java.lang.System.err).println(new StringBuffer().append("Usage: java ").append((object) Class.FromType(typeof (SocketMin))).append(" host port true|false").ToString());
      java.lang.System.exit(1);
    }

    [JavaFlags(8)]
    public static void init(string host, string portStr)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(8)]
    public static void loop()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(8)]
    public static void test()
    {
      int num1 = 0;
      Category cat1 = SocketMin.cat;
      StringBuffer stringBuffer1 = new StringBuffer().append("Message ");
      int num2;
      int num3 = (num2 = num1) + 1;
      int num4 = num2;
      string str1 = stringBuffer1.append(num4).ToString();
      cat1.debug((object) str1);
      Category cat2 = SocketMin.cat;
      StringBuffer stringBuffer2 = new StringBuffer().append("Message ");
      int num5;
      int num6 = (num5 = num3) + 1;
      int num7 = num5;
      string str2 = stringBuffer2.append(num7).ToString();
      cat2.info((object) str2);
      Category cat3 = SocketMin.cat;
      StringBuffer stringBuffer3 = new StringBuffer().append("Message ");
      int num8;
      int num9 = (num8 = num6) + 1;
      int num10 = num8;
      string str3 = stringBuffer3.append(num10).ToString();
      cat3.warn((object) str3);
      Category cat4 = SocketMin.cat;
      StringBuffer stringBuffer4 = new StringBuffer().append("Message ");
      int num11;
      int num12 = (num11 = num9) + 1;
      int num13 = num11;
      string str4 = stringBuffer4.append(num13).ToString();
      cat4.error((object) str4);
      Category cat5 = SocketMin.cat;
      Priority fatal = Priority.FATAL;
      StringBuffer stringBuffer5 = new StringBuffer().append("Message ");
      int num14;
      int num15 = (num14 = num12) + 1;
      int num16 = num14;
      string str5 = stringBuffer5.append(num16).ToString();
      cat5.log(fatal, (object) str5);
      Category cat6 = SocketMin.cat;
      StringBuffer stringBuffer6 = new StringBuffer().append("Message ");
      int num17;
      int num18 = (num17 = num15) + 1;
      int num19 = num17;
      string str6 = stringBuffer6.append(num19).ToString();
      Exception exception = new Exception("Just testing.");
      cat6.debug((object) str6, (Throwable) exception);
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static SocketMin()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      SocketMin socketMin = this;
      ObjectImpl.clone((object) socketMin);
      return ((object) socketMin).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
