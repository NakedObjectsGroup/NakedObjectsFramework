// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.net.SocketNode
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.net;
using org.apache.log4j.spi;
using System.ComponentModel;

namespace org.apache.log4j.net
{
  [JavaInterfaces("1;java/lang/Runnable;")]
  public class SocketNode : Runnable
  {
    [JavaFlags(0)]
    public Socket socket;
    [JavaFlags(0)]
    public LoggerRepository hierarchy;
    [JavaFlags(0)]
    public ObjectInputStream ois;
    [JavaFlags(8)]
    public static Logger logger;

    public SocketNode(Socket socket, LoggerRepository hierarchy)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual void run()
    {
      // ISSUE: unable to decompile the method.
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static SocketNode()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      SocketNode socketNode = this;
      ObjectImpl.clone((object) socketNode);
      return ((object) socketNode).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
