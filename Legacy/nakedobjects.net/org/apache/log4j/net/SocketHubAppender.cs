// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.net.SocketHubAppender
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.util;
using org.apache.log4j.helpers;
using org.apache.log4j.spi;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace org.apache.log4j.net
{
  public class SocketHubAppender : AppenderSkeleton
  {
    [JavaFlags(24)]
    public const int DEFAULT_PORT = 4560;
    private int port;
    private Vector oosList;
    private SocketHubAppender.ServerMonitor serverMonitor;
    private bool locationInfo;

    public SocketHubAppender()
    {
      this.port = 4560;
      this.oosList = new Vector();
      this.serverMonitor = (SocketHubAppender.ServerMonitor) null;
      this.locationInfo = false;
    }

    public SocketHubAppender(int _port)
    {
      this.port = 4560;
      this.oosList = new Vector();
      this.serverMonitor = (SocketHubAppender.ServerMonitor) null;
      this.locationInfo = false;
      this.port = _port;
      this.startServer();
    }

    public override void activateOptions() => this.startServer();

    [MethodImpl(MethodImplOptions.Synchronized)]
    public override void close()
    {
      if (this.closed)
        return;
      LogLog.debug(new StringBuffer().append("closing SocketHubAppender ").append(this.getName()).ToString());
      this.closed = true;
      this.cleanUp();
      LogLog.debug(new StringBuffer().append("SocketHubAppender ").append(this.getName()).append(" closed").ToString());
    }

    public virtual void cleanUp()
    {
      LogLog.debug("stopping ServerSocket");
      this.serverMonitor.stopMonitor();
      this.serverMonitor = (SocketHubAppender.ServerMonitor) null;
      LogLog.debug("closing client connections");
      while (this.oosList.size() != 0)
      {
        ObjectOutputStream objectOutputStream = (ObjectOutputStream) this.oosList.elementAt(0);
        if (objectOutputStream != null)
        {
          try
          {
            objectOutputStream.close();
          }
          catch (IOException ex)
          {
            LogLog.error("could not close oos.", (Throwable) ex);
          }
          this.oosList.removeElementAt(0);
        }
      }
    }

    public override void append(LoggingEvent @event)
    {
      // ISSUE: unable to decompile the method.
    }

    public override bool requiresLayout() => false;

    public virtual void setPort(int _port) => this.port = _port;

    public virtual int getPort() => this.port;

    public virtual void setLocationInfo(bool _locationInfo) => this.locationInfo = _locationInfo;

    public virtual bool getLocationInfo() => this.locationInfo;

    private void startServer() => this.serverMonitor = new SocketHubAppender.ServerMonitor(this, this.port, this.oosList);

    [JavaInterfaces("1;java/lang/Runnable;")]
    [Inner]
    [JavaFlags(34)]
    private class ServerMonitor : Runnable
    {
      private int port;
      private Vector oosList;
      private bool keepRunning;
      private Thread monitorThread;
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private SocketHubAppender this\u00240;

      public ServerMonitor(SocketHubAppender _param1, int _port, Vector _oosList)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.port = _port;
        this.oosList = _oosList;
        this.keepRunning = true;
        this.monitorThread = new Thread((Runnable) this);
        this.monitorThread.setDaemon(true);
        this.monitorThread.start();
      }

      [MethodImpl(MethodImplOptions.Synchronized)]
      public virtual void stopMonitor()
      {
        if (!this.keepRunning)
          return;
        LogLog.debug("server monitor thread shutting down");
        this.keepRunning = false;
        try
        {
          this.monitorThread.join();
        }
        catch (InterruptedException ex)
        {
        }
        this.monitorThread = (Thread) null;
        LogLog.debug("server monitor thread shut down");
      }

      public virtual void run()
      {
        // ISSUE: unable to decompile the method.
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        SocketHubAppender.ServerMonitor serverMonitor = this;
        ObjectImpl.clone((object) serverMonitor);
        return ((object) serverMonitor).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
