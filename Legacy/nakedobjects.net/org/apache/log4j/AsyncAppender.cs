// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.AsyncAppender
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.apache.log4j.helpers;
using org.apache.log4j.spi;
using System.Threading;

namespace org.apache.log4j
{
  [JavaInterfaces("1;org/apache/log4j/spi/AppenderAttachable;")]
  public class AsyncAppender : AppenderSkeleton, AppenderAttachable
  {
    public const int DEFAULT_BUFFER_SIZE = 128;
    [JavaFlags(0)]
    public BoundedFIFO bf;
    [JavaFlags(0)]
    public AppenderAttachableImpl aai;
    [JavaFlags(0)]
    public Dispatcher dispatcher;
    [JavaFlags(0)]
    public bool locationInfo;
    [JavaFlags(0)]
    public bool interruptedWarningMessage;

    public AsyncAppender()
    {
      this.bf = new BoundedFIFO(128);
      this.locationInfo = false;
      this.interruptedWarningMessage = false;
      this.aai = new AppenderAttachableImpl();
      this.dispatcher = new Dispatcher(this.bf, this);
      this.dispatcher.start();
    }

    public virtual void addAppender(Appender newAppender)
    {
      object aai = (object) this.aai;
      \u003CCorArrayWrapper\u003E.Enter(aai);
      try
      {
        this.aai.addAppender(newAppender);
      }
      finally
      {
        Monitor.Exit(aai);
      }
    }

    public override void append(LoggingEvent @event)
    {
      @event.getNDC();
      @event.getThreadName();
      @event.getMDCCopy();
      if (this.locationInfo)
        @event.getLocationInformation();
      object bf = (object) this.bf;
      \u003CCorArrayWrapper\u003E.Enter(bf);
      try
      {
        while (this.bf.isFull())
        {
          try
          {
            ObjectImpl.wait((object) this.bf);
          }
          catch (InterruptedException ex)
          {
            if (!this.interruptedWarningMessage)
            {
              this.interruptedWarningMessage = true;
              LogLog.warn("AsyncAppender interrupted.", (Throwable) ex);
            }
            else
              LogLog.warn("AsyncAppender interrupted again.");
          }
        }
        this.bf.put(@event);
        if (!this.bf.wasEmpty())
          return;
        ObjectImpl.notify((object) this.bf);
      }
      finally
      {
        Monitor.Exit(bf);
      }
    }

    public override void close()
    {
      object obj = (object) this;
      \u003CCorArrayWrapper\u003E.Enter(obj);
      try
      {
        if (this.closed)
          return;
        this.closed = true;
      }
      finally
      {
        Monitor.Exit(obj);
      }
      this.dispatcher.close();
      try
      {
        this.dispatcher.join();
      }
      catch (InterruptedException ex)
      {
        LogLog.error("Got an InterruptedException while waiting for the dispatcher to finish.", (Throwable) ex);
      }
      this.dispatcher = (Dispatcher) null;
      this.bf = (BoundedFIFO) null;
    }

    public virtual Enumeration getAllAppenders()
    {
      object aai = (object) this.aai;
      \u003CCorArrayWrapper\u003E.Enter(aai);
      try
      {
        return this.aai.getAllAppenders();
      }
      finally
      {
        Monitor.Exit(aai);
      }
    }

    public virtual Appender getAppender(string name)
    {
      object aai = (object) this.aai;
      \u003CCorArrayWrapper\u003E.Enter(aai);
      try
      {
        return this.aai.getAppender(name);
      }
      finally
      {
        Monitor.Exit(aai);
      }
    }

    public virtual bool getLocationInfo() => this.locationInfo;

    public virtual bool isAttached(Appender appender) => this.aai.isAttached(appender);

    public override bool requiresLayout() => false;

    public virtual void removeAllAppenders()
    {
      object aai = (object) this.aai;
      \u003CCorArrayWrapper\u003E.Enter(aai);
      try
      {
        this.aai.removeAllAppenders();
      }
      finally
      {
        Monitor.Exit(aai);
      }
    }

    public virtual void removeAppender(Appender appender)
    {
      object aai = (object) this.aai;
      \u003CCorArrayWrapper\u003E.Enter(aai);
      try
      {
        this.aai.removeAppender(appender);
      }
      finally
      {
        Monitor.Exit(aai);
      }
    }

    public virtual void removeAppender(string name)
    {
      object aai = (object) this.aai;
      \u003CCorArrayWrapper\u003E.Enter(aai);
      try
      {
        this.aai.removeAppender(name);
      }
      finally
      {
        Monitor.Exit(aai);
      }
    }

    public virtual void setLocationInfo(bool flag) => this.locationInfo = flag;

    public virtual void setBufferSize(int size) => this.bf.resize(size);

    public virtual int getBufferSize() => this.bf.getMaxSize();
  }
}
