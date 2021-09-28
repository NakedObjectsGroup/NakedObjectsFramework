// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.logging.SmtpSnapshotAppender
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.io;
using java.lang;
using System.ComponentModel;

namespace org.nakedobjects.utility.logging
{
  public class SmtpSnapshotAppender : SnapshotAppender
  {
    private static readonly org.apache.log4j.Logger LOG;
    private string server;
    private string recipient;
    private int port;
    private string senderDomain;

    public virtual void setServer(string mailServer)
    {
      Assert.assertNotNull((object) mailServer);
      this.server = mailServer;
    }

    public virtual void setRecipient(string recipient)
    {
      Assert.assertNotNull((object) recipient);
      this.recipient = recipient;
    }

    public virtual void setPort(int port) => this.port = port;

    public virtual void setSenderDomain(string senderDomain)
    {
      Assert.assertNotNull((object) senderDomain);
      this.senderDomain = senderDomain;
    }

    [JavaFlags(4)]
    public override void writeSnapshot(string message, string details)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    private void send(BufferedReader @in, BufferedWriter @out, string s)
    {
      ((Writer) @out).write(new StringBuffer().append(s).append("\r\n").ToString());
      @out.flush();
      s = @in.readLine();
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    private void send(BufferedWriter @out, string s)
    {
      ((Writer) @out).write(new StringBuffer().append(s).append("\r\n").ToString());
      @out.flush();
    }

    public SmtpSnapshotAppender()
    {
      this.port = 25;
      this.senderDomain = "domain";
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static SmtpSnapshotAppender()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
