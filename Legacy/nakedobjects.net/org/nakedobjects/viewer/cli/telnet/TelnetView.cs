// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.telnet.TelnetView
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.io;
using java.lang;
using java.net;

namespace org.nakedobjects.viewer.cli.telnet
{
  public class TelnetView : AbstractTextView
  {
    private BufferedWriter @out;
    private readonly Socket connection;

    public TelnetView(Socket connection)
    {
      this.connection = connection;
      try
      {
        this.@out = new BufferedWriter((Writer) new OutputStreamWriter(connection.getOutputStream()));
      }
      catch (IOException ex)
      {
        throw new ConnectionException("Failed to start connection", (Throwable) ex);
      }
    }

    [JavaFlags(4)]
    public override void appendln(string text) => this.send(new StringBuffer().append(text).append("\r\n").ToString());

    public override void clear() => this.send(new StringBuffer().append("").append('\f').ToString());

    public override void disconnect()
    {
      base.disconnect();
      if (this.@out == null)
        return;
      try
      {
        this.@out.close();
        this.connection.close();
      }
      catch (IOException ex)
      {
        throw new ConnectionException("Failed to stop connection", (Throwable) ex);
      }
    }

    public override void prompt(string prompt) => this.send(new StringBuffer().append(prompt).append("> ").ToString());

    private void send(string data)
    {
      try
      {
        ((Writer) this.@out).write(data);
        this.@out.flush();
      }
      catch (IOException ex)
      {
        throw new ConnectionException("Failed to send response", (Throwable) ex);
      }
    }
  }
}
