// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.net.TelnetAppender
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.net;
using java.util;
using org.apache.log4j.spi;
using System.ComponentModel;

namespace org.apache.log4j.net
{
  public class TelnetAppender : AppenderSkeleton
  {
    private TelnetAppender.SocketHandler sh;
    private int port;

    public override bool requiresLayout() => true;

    public override void activateOptions()
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual int getPort() => this.port;

    public virtual void setPort(int port) => this.port = port;

    public override void close() => this.sh.Finalize();

    [JavaFlags(4)]
    public override void append(LoggingEvent @event)
    {
      this.sh.send(this.layout.format(@event));
      if (!this.layout.ignoresThrowable())
        return;
      string[] throwableStrRep = @event.getThrowableStrRep();
      if (throwableStrRep == null)
        return;
      int length = throwableStrRep.Length;
      for (int index = 0; index < length; ++index)
      {
        this.sh.send(throwableStrRep[index]);
        this.sh.send(Layout.LINE_SEP);
      }
    }

    public TelnetAppender() => this.port = 23;

    [Inner]
    [JavaFlags(36)]
    public class SocketHandler : Thread
    {
      private bool done;
      private Vector writers;
      private Vector connections;
      private ServerSocket serverSocket;
      private int MAX_CONNECTIONS;
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private TelnetAppender this\u00240;

      public virtual void Finalize()
      {
        // ISSUE: unable to decompile the method.
      }

      public virtual void send(string message)
      {
        Enumeration enumeration1 = this.connections.elements();
        Enumeration enumeration2 = this.writers.elements();
        while (enumeration2.hasMoreElements())
        {
          Socket socket = (Socket) enumeration1.nextElement();
          PrintWriter printWriter = (PrintWriter) enumeration2.nextElement();
          printWriter.print(message);
          if (printWriter.checkError())
          {
            this.connections.removeElement((object) socket);
            this.writers.removeElement((object) printWriter);
          }
        }
      }

      public virtual void run()
      {
        // ISSUE: unable to decompile the method.
      }

      [JavaThrownExceptions("1;java/io/IOException;")]
      public SocketHandler(TelnetAppender _param1, int port)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.done = false;
        this.writers = new Vector();
        this.connections = new Vector();
        this.MAX_CONNECTIONS = 20;
        this.serverSocket = new ServerSocket(port);
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public virtual object MemberwiseClone()
      {
        TelnetAppender.SocketHandler socketHandler = this;
        ObjectImpl.clone((object) socketHandler);
        return ((object) socketHandler).MemberwiseClone();
      }
    }
  }
}
