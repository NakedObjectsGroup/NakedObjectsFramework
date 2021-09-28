// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.net.test.SyslogMin
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
  public class SyslogMin
  {
    [JavaFlags(8)]
    public static Category CAT;

    public static void main(string[] argv)
    {
      // ISSUE: type reference
      RuntimeHelpers.RunClassConstructor(__typeref (ObjectImpl));
      if (argv.Length == 1)
        SyslogMin.ProgramInit(argv[0]);
      else
        SyslogMin.Usage("Wrong number of arguments.");
      SyslogMin.test("someHost");
      Utilities.cleanupAfterMainReturns();
    }

    [JavaFlags(8)]
    public static void Usage(string msg)
    {
      ((PrintStream) java.lang.System.err).println(msg);
      ((PrintStream) java.lang.System.err).println(new StringBuffer().append("Usage: java ").append((object) Class.FromType(typeof (SyslogMin))).append(" configFile").ToString());
      java.lang.System.exit(1);
    }

    [JavaFlags(8)]
    public static void ProgramInit(string configFile) => PropertyConfigurator.configure(configFile);

    [JavaFlags(8)]
    public static void test(string host)
    {
      NDC.push(host);
      int num1 = 0;
      Category cat1 = SyslogMin.CAT;
      StringBuffer stringBuffer1 = new StringBuffer().append("Message ");
      int num2;
      int num3 = (num2 = num1) + 1;
      int num4 = num2;
      string str1 = stringBuffer1.append(num4).ToString();
      cat1.debug((object) str1);
      Category cat2 = SyslogMin.CAT;
      StringBuffer stringBuffer2 = new StringBuffer().append("Message ");
      int num5;
      int num6 = (num5 = num3) + 1;
      int num7 = num5;
      string str2 = stringBuffer2.append(num7).ToString();
      cat2.info((object) str2);
      Category cat3 = SyslogMin.CAT;
      StringBuffer stringBuffer3 = new StringBuffer().append("Message ");
      int num8;
      int num9 = (num8 = num6) + 1;
      int num10 = num8;
      string str3 = stringBuffer3.append(num10).ToString();
      cat3.warn((object) str3);
      Category cat4 = SyslogMin.CAT;
      StringBuffer stringBuffer4 = new StringBuffer().append("Message ");
      int num11;
      int num12 = (num11 = num9) + 1;
      int num13 = num11;
      string str4 = stringBuffer4.append(num13).ToString();
      cat4.error((object) str4);
      Category cat5 = SyslogMin.CAT;
      Priority fatal = Priority.FATAL;
      StringBuffer stringBuffer5 = new StringBuffer().append("Message ");
      int num14;
      int num15 = (num14 = num12) + 1;
      int num16 = num14;
      string str5 = stringBuffer5.append(num16).ToString();
      cat5.log(fatal, (object) str5);
      Category cat6 = SyslogMin.CAT;
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
    static SyslogMin()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      SyslogMin syslogMin = this;
      ObjectImpl.clone((object) syslogMin);
      return ((object) syslogMin).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
