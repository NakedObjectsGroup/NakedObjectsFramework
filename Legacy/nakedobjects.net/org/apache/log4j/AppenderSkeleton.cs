// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.AppenderSkeleton
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.apache.log4j.helpers;
using org.apache.log4j.spi;
using System;
using System.Runtime.CompilerServices;

namespace org.apache.log4j
{
  [JavaInterfaces("2;org/apache/log4j/Appender;org/apache/log4j/spi/OptionHandler;")]
  public abstract class AppenderSkeleton : Appender, OptionHandler
  {
    [JavaFlags(4)]
    public Layout layout;
    [JavaFlags(4)]
    public string name;
    [JavaFlags(4)]
    public Priority threshold;
    [JavaFlags(4)]
    public ErrorHandler errorHandler;
    [JavaFlags(4)]
    public Filter headFilter;
    [JavaFlags(4)]
    public Filter tailFilter;
    [JavaFlags(4)]
    public bool closed;

    public virtual void activateOptions()
    {
    }

    public virtual void addFilter(Filter newFilter)
    {
      if (this.headFilter == null)
      {
        this.headFilter = this.tailFilter = newFilter;
      }
      else
      {
        this.tailFilter.next = newFilter;
        this.tailFilter = newFilter;
      }
    }

    [JavaFlags(1028)]
    public abstract void append(LoggingEvent @event);

    public virtual void clearFilters() => this.headFilter = this.tailFilter = (Filter) null;

    public override void Finalize()
    {
      try
      {
        if (this.closed)
          return;
        LogLog.debug(new StringBuffer().append("Finalizing appender named [").append(this.name).append("].").ToString());
        this.close();
      }
      catch (Exception ex)
      {
      }
    }

    public virtual ErrorHandler getErrorHandler() => this.errorHandler;

    public virtual Filter getFilter() => this.headFilter;

    [JavaFlags(17)]
    public Filter getFirstFilter() => this.headFilter;

    public virtual Layout getLayout() => this.layout;

    [JavaFlags(17)]
    public virtual string getName() => this.name;

    public virtual Priority getThreshold() => this.threshold;

    public virtual bool isAsSevereAsThreshold(Priority priority) => this.threshold == null || priority.isGreaterOrEqual(this.threshold);

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void doAppend(LoggingEvent @event)
    {
      if (this.closed)
      {
        LogLog.error(new StringBuffer().append("Attempted to append to closed appender named [").append(this.name).append("].").ToString());
      }
      else
      {
        if (!this.isAsSevereAsThreshold((Priority) @event.getLevel()))
          return;
        Filter filter = this.headFilter;
        while (filter != null)
        {
          switch (filter.decide(@event))
          {
            case -1:
              return;
            case 0:
              filter = filter.next;
              continue;
            case 1:
              goto label_9;
            default:
              continue;
          }
        }
label_9:
        this.append(@event);
      }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void setErrorHandler(ErrorHandler eh)
    {
      if (eh == null)
        LogLog.warn("You have tried to set a null error-handler.");
      else
        this.errorHandler = eh;
    }

    public virtual void setLayout(Layout layout) => this.layout = layout;

    public virtual void setName(string name) => this.name = name;

    public virtual void setThreshold(Priority threshold) => this.threshold = threshold;

    public AppenderSkeleton()
    {
      this.errorHandler = (ErrorHandler) new OnlyOnceErrorHandler();
      this.closed = false;
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      AppenderSkeleton appenderSkeleton = this;
      ObjectImpl.clone((object) appenderSkeleton);
      return ((object) appenderSkeleton).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    public abstract void close();

    public abstract bool requiresLayout();
  }
}
