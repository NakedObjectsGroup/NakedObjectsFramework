// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.helpers.SyslogWriter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.net;

namespace org.apache.log4j.helpers
{
  public class SyslogWriter : Writer
  {
    [JavaFlags(16)]
    public readonly int SYSLOG_PORT;
    [JavaFlags(8)]
    public static string syslogHost;
    private InetAddress address;
    private DatagramSocket ds;

    public SyslogWriter(string syslogHost)
    {
      this.SYSLOG_PORT = 514;
      SyslogWriter.syslogHost = syslogHost;
      try
      {
        this.address = InetAddress.getByName(syslogHost);
      }
      catch (UnknownHostException ex)
      {
        LogLog.error(new StringBuffer().append("Could not find ").append(syslogHost).append(". All logging will FAIL.").ToString(), (Throwable) ex);
      }
      try
      {
        this.ds = new DatagramSocket();
      }
      catch (SocketException ex)
      {
        ((Throwable) ex).printStackTrace();
        LogLog.error(new StringBuffer().append("Could not instantiate DatagramSocket to ").append(syslogHost).append(". All logging will FAIL.").ToString(), (Throwable) ex);
      }
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public virtual void write(char[] buf, int off, int len) => this.write(StringImpl.createString(buf, off, len));

    [JavaThrownExceptions("1;java/io/IOException;")]
    public virtual void write(string @string)
    {
      sbyte[] bytes = StringImpl.getBytes(@string);
      DatagramPacket datagramPacket = new DatagramPacket(bytes, bytes.Length, this.address, 514);
      if (this.ds == null)
        return;
      this.ds.send(datagramPacket);
    }

    public virtual void flush()
    {
    }

    public virtual void close()
    {
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public virtual object MemberwiseClone()
    {
      SyslogWriter syslogWriter = this;
      ObjectImpl.clone((object) syslogWriter);
      return ((object) syslogWriter).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public virtual string ToString() => ObjectImpl.jloToString((object) this);
  }
}
