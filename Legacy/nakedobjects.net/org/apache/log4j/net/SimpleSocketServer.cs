// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.net.SimpleSocketServer
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using System.ComponentModel;

namespace org.apache.log4j.net
{
  public class SimpleSocketServer
  {
    [JavaFlags(8)]
    public static Category cat;
    [JavaFlags(8)]
    public static int port;

    public static void main(string[] argv)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(8)]
    public static void usage(string msg)
    {
      ((PrintStream) java.lang.System.err).println(msg);
      ((PrintStream) java.lang.System.err).println(new StringBuffer().append("Usage: java ").append(Class.FromType(typeof (SimpleSocketServer)).getName()).append(" port configFile").ToString());
      java.lang.System.exit(1);
    }

    [JavaFlags(8)]
    public static void init(string portStr, string configFile)
    {
      // ISSUE: unable to decompile the method.
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static SimpleSocketServer()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      SimpleSocketServer simpleSocketServer = this;
      ObjectImpl.clone((object) simpleSocketServer);
      return ((object) simpleSocketServer).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
