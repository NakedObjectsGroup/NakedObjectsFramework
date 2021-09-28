// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.logging.SocketSnapshotAppender
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using System.ComponentModel;

namespace org.nakedobjects.utility.logging
{
  public class SocketSnapshotAppender : SnapshotAppender
  {
    private static readonly org.apache.log4j.Logger LOG;
    private int port;
    private string server;

    public virtual void setPort(int port) => this.port = port;

    public virtual void setServer(string mailServer)
    {
      Assert.assertNotNull((object) mailServer);
      this.server = mailServer;
    }

    [JavaFlags(4)]
    public override void writeSnapshot(string message, string details)
    {
      // ISSUE: unable to decompile the method.
    }

    public SocketSnapshotAppender() => this.port = 9289;

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static SocketSnapshotAppender()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
