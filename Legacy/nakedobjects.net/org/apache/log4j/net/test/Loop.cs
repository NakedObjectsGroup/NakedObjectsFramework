// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.net.test.Loop
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;

namespace org.apache.log4j.net.test
{
  public class Loop
  {
    public static void main(string[] args)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(8)]
    public static void usage(string msg)
    {
      ((PrintStream) System.err).println(msg);
      ((PrintStream) System.err).println(new StringBuffer().append("Usage: java ").append(Class.FromType(typeof (Loop)).getName()).append(" host port").ToString());
      System.exit(1);
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      Loop loop = this;
      ObjectImpl.clone((object) loop);
      return ((object) loop).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
