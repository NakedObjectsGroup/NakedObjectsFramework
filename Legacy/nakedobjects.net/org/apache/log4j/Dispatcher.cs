// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.Dispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.apache.log4j.helpers;
using org.apache.log4j.spi;
using System.Threading;

namespace org.apache.log4j
{
  [JavaFlags(32)]
  public class Dispatcher : Thread
  {
    [JavaFlags(0)]
    public BoundedFIFO bf;
    [JavaFlags(0)]
    public AppenderAttachableImpl aai;
    [JavaFlags(0)]
    public bool interrupted;
    [JavaFlags(0)]
    public AsyncAppender container;

    [JavaFlags(0)]
    public Dispatcher(BoundedFIFO bf, AsyncAppender container)
    {
      this.interrupted = false;
      this.bf = bf;
      this.container = container;
      this.aai = container.aai;
      this.setDaemon(true);
      this.setPriority(1);
      this.setName(new StringBuffer().append("Dispatcher-").append(this.getName()).ToString());
    }

    [JavaFlags(0)]
    public virtual void close()
    {
      object bf = (object) this.bf;
      \u003CCorArrayWrapper\u003E.Enter(bf);
      try
      {
        this.interrupted = true;
        if (this.bf.length() != 0)
          return;
        ObjectImpl.notify((object) this.bf);
      }
      finally
      {
        Monitor.Exit(bf);
      }
    }

    public virtual void run()
    {
      while (true)
      {
        object bf = (object) this.bf;
        \u003CCorArrayWrapper\u003E.Enter(bf);
        LoggingEvent @event;
        try
        {
          if (this.bf.length() == 0)
          {
            if (!this.interrupted)
            {
              try
              {
                ObjectImpl.wait((object) this.bf);
              }
              catch (InterruptedException ex)
              {
                LogLog.error("The dispathcer should not be interrupted.");
                break;
              }
            }
            else
              break;
          }
          @event = this.bf.get();
          if (this.bf.wasFull())
            ObjectImpl.notify((object) this.bf);
        }
        finally
        {
          Monitor.Exit(bf);
        }
        object aai = (object) this.container.aai;
        \u003CCorArrayWrapper\u003E.Enter(aai);
        try
        {
          if (this.aai != null)
          {
            if (@event != null)
              this.aai.appendLoopOnAppenders(@event);
          }
        }
        finally
        {
          Monitor.Exit(aai);
        }
      }
      this.aai.removeAllAppenders();
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public virtual object MemberwiseClone()
    {
      Dispatcher dispatcher = this;
      ObjectImpl.clone((object) dispatcher);
      return ((object) dispatcher).MemberwiseClone();
    }
  }
}
