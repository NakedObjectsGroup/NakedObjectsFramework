// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.performance.NewVsSetLen
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

namespace org.apache.log4j.performance
{
  public class NewVsSetLen
  {
    [JavaFlags(8)]
    public static string s;
    [JavaFlags(8)]
    public static int BIGBUF_LEN;
    [JavaFlags(8)]
    public static int SBUF_LEN;
    [JavaFlags(8)]
    public static int RUN_LENGTH;
    [JavaFlags(8)]
    public static char[] sbuf;
    [JavaFlags(8)]
    public static char[] bigbuf;

    public static void main(string[] args)
    {
      // ISSUE: type reference
      RuntimeHelpers.RunClassConstructor(__typeref (ObjectImpl));
      int sbufLen = NewVsSetLen.SBUF_LEN;
      while (sbufLen <= NewVsSetLen.BIGBUF_LEN)
      {
        ((PrintStream) java.lang.System.@out).println(new StringBuffer().append("<td>").append(sbufLen).append("\n").ToString());
        int second = 0;
        while (second < 16)
        {
          ((PrintStream) java.lang.System.@out).println(new StringBuffer().append("SECOND loop=").append(second).append(", RUN_LENGTH=").append(NewVsSetLen.RUN_LENGTH).append(", len=").append(sbufLen).ToString());
          int num1 = Utilities.doubleToInt(NewVsSetLen.newBuffer(sbufLen, second));
          ((PrintStream) java.lang.System.@out).print(new StringBuffer().append("<td>").append(num1).ToString());
          int num2 = Utilities.doubleToInt(NewVsSetLen.setLen(sbufLen, second));
          ((PrintStream) java.lang.System.@out).println(new StringBuffer().append(" <td>").append(num2).append(" \n").ToString());
          if (second == 0)
            second = 1;
          else
            second *= 2;
        }
        sbufLen *= 4;
        NewVsSetLen.RUN_LENGTH /= 4;
      }
      Utilities.cleanupAfterMainReturns();
    }

    [JavaFlags(8)]
    public static double newBuffer(int size, int second)
    {
      long num = java.lang.System.currentTimeMillis();
      for (int index = 0; index < NewVsSetLen.RUN_LENGTH; ++index)
      {
        StringBuffer stringBuffer = new StringBuffer(NewVsSetLen.SBUF_LEN);
        stringBuffer.append(NewVsSetLen.sbuf, 0, NewVsSetLen.sbuf.Length);
        stringBuffer.append(NewVsSetLen.bigbuf, 0, size);
        NewVsSetLen.s = stringBuffer.ToString();
      }
      for (int index = 0; index < second; ++index)
      {
        StringBuffer stringBuffer = new StringBuffer(NewVsSetLen.SBUF_LEN);
        stringBuffer.append(NewVsSetLen.sbuf, 0, NewVsSetLen.SBUF_LEN);
        NewVsSetLen.s = stringBuffer.ToString();
      }
      return (double) (java.lang.System.currentTimeMillis() - num) * 1000.0 / (double) NewVsSetLen.RUN_LENGTH;
    }

    [JavaFlags(8)]
    public static double setLen(int size, int second)
    {
      long num = java.lang.System.currentTimeMillis();
      StringBuffer stringBuffer = new StringBuffer(NewVsSetLen.SBUF_LEN);
      for (int index = 0; index < NewVsSetLen.RUN_LENGTH; ++index)
      {
        stringBuffer.append(NewVsSetLen.sbuf, 0, NewVsSetLen.sbuf.Length);
        stringBuffer.append(NewVsSetLen.bigbuf, 0, size);
        NewVsSetLen.s = stringBuffer.ToString();
        stringBuffer.setLength(0);
      }
      for (int index = 0; index < second; ++index)
      {
        stringBuffer.append(NewVsSetLen.sbuf, 0, NewVsSetLen.SBUF_LEN);
        NewVsSetLen.s = stringBuffer.ToString();
        stringBuffer.setLength(0);
      }
      return (double) (java.lang.System.currentTimeMillis() - num) * 1000.0 / (double) NewVsSetLen.RUN_LENGTH;
    }

    public NewVsSetLen()
    {
      for (int index = 0; index < NewVsSetLen.SBUF_LEN; ++index)
        NewVsSetLen.sbuf[index] = (char) index;
      for (int index = 0; index < NewVsSetLen.BIGBUF_LEN; ++index)
        NewVsSetLen.bigbuf[index] = (char) index;
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static NewVsSetLen()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      NewVsSetLen newVsSetLen = this;
      ObjectImpl.clone((object) newVsSetLen);
      return ((object) newVsSetLen).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
