// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.telnet.TelnetInput
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.net;
using org.apache.log4j;
using System.ComponentModel;

namespace org.nakedobjects.viewer.cli.telnet
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Input;")]
  public class TelnetInput : Input
  {
    private static readonly Logger LOG;
    private string hostName;
    private DataInputStream @in;

    public TelnetInput(Socket connection)
    {
      this.hostName = connection.getInetAddress().getHostName();
      if (TelnetInput.LOG.isInfoEnabled())
        TelnetInput.LOG.info((object) new StringBuffer().append("Connection from ").append(this.hostName).ToString());
      try
      {
        this.@in = new DataInputStream(connection.getInputStream());
      }
      catch (IOException ex)
      {
        throw new ConnectionException("Failed to start connection", (Throwable) ex);
      }
    }

    public virtual string accept()
    {
      try
      {
        StringBuffer stringBuffer = new StringBuffer();
        int num;
        while ((num = this.read()) != 10)
        {
          if (num == 8)
            stringBuffer.setLength(stringBuffer.length() - 1);
          else
            stringBuffer.append((char) num);
          if (TelnetInput.LOG.isDebugEnabled())
            TelnetInput.LOG.debug((object) new StringBuffer().append("line:  '").append((object) stringBuffer).append("'").ToString());
        }
        string str = StringImpl.trim(stringBuffer.ToString());
        if (TelnetInput.LOG.isDebugEnabled())
          TelnetInput.LOG.debug((object) new StringBuffer().append(">>> Request (").append(this.hostName).append("):  '").append(str).append("'").ToString());
        return str;
      }
      catch (IOException ex)
      {
        throw new ConnectionException("Failed to accept input", (Throwable) ex);
      }
    }

    public virtual void connect()
    {
    }

    public virtual void disconnect()
    {
      if (this.@in == null)
        return;
      try
      {
        ((FilterInputStream) this.@in).close();
      }
      catch (IOException ex)
      {
        throw new ConnectionException("Failed to stop connection", (Throwable) ex);
      }
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public virtual int read()
    {
      int input = this.readKey();
      bool flag = false;
      while (input == (int) byte.MaxValue && !flag)
      {
        input = this.readKey();
        if (input != (int) byte.MaxValue)
        {
          if (TelnetInput.LOG.isWarnEnabled())
            TelnetInput.LOG.warn((object) new StringBuffer().append("Unhandled IAC ").append(input).ToString());
          input = this.readKey();
        }
        else
          flag = true;
      }
      return this.stripCR(input);
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    private int readKey()
    {
      int num = this.@in.readUnsignedByte();
      if (TelnetInput.LOG.isDebugEnabled())
        TelnetInput.LOG.debug((object) new StringBuffer().append("key:  '").append(num).append("'").ToString());
      return num;
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    private int stripCR(int input)
    {
      if (input != 13)
        return input;
      this.readKey();
      return 10;
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static TelnetInput()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      TelnetInput telnetInput = this;
      ObjectImpl.clone((object) telnetInput);
      return ((object) telnetInput).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
