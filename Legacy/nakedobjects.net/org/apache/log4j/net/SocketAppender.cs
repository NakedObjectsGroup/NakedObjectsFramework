// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.net.SocketAppender
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.net;
using org.apache.log4j.helpers;
using org.apache.log4j.spi;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace org.apache.log4j.net
{
  public class SocketAppender : AppenderSkeleton
  {
    [JavaFlags(24)]
    public const int DEFAULT_PORT = 4560;
    [JavaFlags(24)]
    public const int DEFAULT_RECONNECTION_DELAY = 30000;
    [JavaFlags(0)]
    public string remoteHost;
    [JavaFlags(0)]
    public InetAddress address;
    [JavaFlags(0)]
    public int port;
    [JavaFlags(0)]
    public ObjectOutputStream oos;
    [JavaFlags(0)]
    public int reconnectionDelay;
    [JavaFlags(0)]
    public bool locationInfo;
    private SocketAppender.Connector connector;
    [JavaFlags(0)]
    public int counter;
    private const int RESET_FREQUENCY = 1;

    public SocketAppender()
    {
      this.port = 4560;
      this.reconnectionDelay = 30000;
      this.locationInfo = false;
      this.counter = 0;
    }

    public SocketAppender(InetAddress address, int port)
    {
      this.port = 4560;
      this.reconnectionDelay = 30000;
      this.locationInfo = false;
      this.counter = 0;
      this.address = address;
      this.remoteHost = address.getHostName();
      this.port = port;
      this.connect(address, port);
    }

    public SocketAppender(string host, int port)
    {
      this.port = 4560;
      this.reconnectionDelay = 30000;
      this.locationInfo = false;
      this.counter = 0;
      this.port = port;
      this.address = SocketAppender.getAddressByName(host);
      this.remoteHost = host;
      this.connect(this.address, port);
    }

    public override void activateOptions() => this.connect(this.address, this.port);

    [MethodImpl(MethodImplOptions.Synchronized)]
    public override void close()
    {
      if (this.closed)
        return;
      this.closed = true;
      this.cleanUp();
    }

    public virtual void cleanUp()
    {
      if (this.oos != null)
      {
        try
        {
          this.oos.close();
        }
        catch (IOException ex)
        {
          LogLog.error("Could not close oos.", (Throwable) ex);
        }
        this.oos = (ObjectOutputStream) null;
      }
      if (this.connector == null)
        return;
      this.connector.interrupted = true;
      this.connector = (SocketAppender.Connector) null;
    }

    [JavaFlags(0)]
    public virtual void connect(InetAddress address, int port)
    {
      if (this.address == null)
        return;
      try
      {
        this.cleanUp();
        this.oos = new ObjectOutputStream(new Socket(address, port).getOutputStream());
      }
      catch (IOException ex)
      {
        LogLog.error(new StringBuffer().append("Could not connect to remote log4j server at [").append(address.getHostName()).append("]. We will try again later.").ToString(), (Throwable) ex);
        this.fireConnector();
      }
    }

    public override void append(LoggingEvent @event)
    {
      if (@event == null)
        return;
      if (this.address == null)
      {
        this.errorHandler.error(new StringBuffer().append("No remote host is set for SocketAppender named \"").append(this.name).append("\".").ToString());
      }
      else
      {
        if (this.oos == null)
          return;
        try
        {
          if (this.locationInfo)
            @event.getLocationInformation();
          this.oos.writeObject((object) @event);
          this.oos.flush();
          if (++this.counter < 1)
            return;
          this.counter = 0;
          this.oos.reset();
        }
        catch (IOException ex)
        {
          this.oos = (ObjectOutputStream) null;
          LogLog.warn(new StringBuffer().append("Detected problem with connection: ").append((object) ex).ToString());
          if (this.reconnectionDelay <= 0)
            return;
          this.fireConnector();
        }
      }
    }

    [JavaFlags(0)]
    public virtual void fireConnector()
    {
      if (this.connector != null)
        return;
      LogLog.debug("Starting a new connector thread.");
      this.connector = new SocketAppender.Connector(this);
      this.connector.setDaemon(true);
      this.connector.setPriority(1);
      this.connector.start();
    }

    [JavaFlags(8)]
    public static InetAddress getAddressByName(string host)
    {
      // ISSUE: unable to decompile the method.
    }

    public override bool requiresLayout() => false;

    public virtual void setRemoteHost(string host)
    {
      this.address = SocketAppender.getAddressByName(host);
      this.remoteHost = host;
    }

    public virtual string getRemoteHost() => this.remoteHost;

    public virtual void setPort(int port) => this.port = port;

    public virtual int getPort() => this.port;

    public virtual void setLocationInfo(bool locationInfo) => this.locationInfo = locationInfo;

    public virtual bool getLocationInfo() => this.locationInfo;

    public virtual void setReconnectionDelay(int delay) => this.reconnectionDelay = delay;

    public virtual int getReconnectionDelay() => this.reconnectionDelay;

    [JavaFlags(32)]
    [Inner]
    public class Connector : Thread
    {
      [JavaFlags(0)]
      public bool interrupted;
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private SocketAppender this\u00240;

      public virtual void run()
      {
        while (!this.interrupted)
        {
          try
          {
            Thread.sleep((long) this.this\u00240.reconnectionDelay);
            LogLog.debug(new StringBuffer().append("Attempting connection to ").append(this.this\u00240.address.getHostName()).ToString());
            Socket socket = new Socket(this.this\u00240.address, this.this\u00240.port);
            object obj = (object) this;
            \u003CCorArrayWrapper\u003E.Enter(obj);
            try
            {
              this.this\u00240.oos = new ObjectOutputStream(socket.getOutputStream());
              this.this\u00240.connector = (SocketAppender.Connector) null;
              LogLog.debug("Connection established. Exiting connector thread.");
              break;
            }
            finally
            {
              Monitor.Exit(obj);
            }
          }
          catch (InterruptedException ex)
          {
            LogLog.debug("Connector interrupted. Leaving loop.");
            break;
          }
          catch (ConnectException ex)
          {
            LogLog.debug(new StringBuffer().append("Remote host ").append(this.this\u00240.address.getHostName()).append(" refused connection.").ToString());
          }
          catch (IOException ex)
          {
            LogLog.debug(new StringBuffer().append("Could not connect to ").append(this.this\u00240.address.getHostName()).append(". Exception is ").append((object) ex).ToString());
          }
        }
      }

      [JavaFlags(0)]
      public Connector(SocketAppender _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.interrupted = false;
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public virtual object MemberwiseClone()
      {
        SocketAppender.Connector connector = this;
        ObjectImpl.clone((object) connector);
        return ((object) connector).MemberwiseClone();
      }
    }
  }
}
