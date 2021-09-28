// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.spi.LoggingEvent
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.util;
using System;
using System.ComponentModel;

namespace org.apache.log4j.spi
{
  [JavaInterfaces("1;java/io/Serializable;")]
  public class LoggingEvent : Serializable
  {
    private static long startTime;
    [JavaFlags(145)]
    [NonSerialized]
    public readonly string fqnOfCategoryClass;
    [JavaFlags(130)]
    [Obsolete(null, false)]
    [NonSerialized]
    private Category logger;
    [Obsolete(null, false)]
    public readonly string categoryName;
    [Obsolete(null, false)]
    [JavaFlags(129)]
    [NonSerialized]
    public Priority level;
    private string ndc;
    private Hashtable mdcCopy;
    private bool ndcLookupRequired;
    private bool mdcCopyLookupRequired;
    [JavaFlags(130)]
    [NonSerialized]
    private object message;
    private string renderedMessage;
    private string threadName;
    private ThrowableInformation throwableInfo;
    public readonly long timeStamp;
    private LocationInfo locationInfo;
    [JavaFlags(24)]
    public const long serialVersionUID = -868428216207166145;
    [JavaFlags(24)]
    public static readonly Integer[] PARAM_ARRAY;
    [JavaFlags(24)]
    public const string TO_LEVEL = "toLevel";
    [JavaFlags(24)]
    public static readonly Class[] TO_LEVEL_PARAMS;
    [JavaFlags(24)]
    public static readonly Hashtable methodCache;

    public LoggingEvent(
      string fqnOfCategoryClass,
      Category logger,
      Priority priority,
      object message,
      Throwable throwable)
    {
      this.ndcLookupRequired = true;
      this.mdcCopyLookupRequired = true;
      this.fqnOfCategoryClass = fqnOfCategoryClass;
      this.logger = logger;
      this.categoryName = logger.getName();
      this.level = priority;
      this.message = message;
      if (throwable != null)
        this.throwableInfo = new ThrowableInformation(throwable);
      this.timeStamp = java.lang.System.currentTimeMillis();
    }

    public LoggingEvent(
      string fqnOfCategoryClass,
      Category logger,
      long timeStamp,
      Priority priority,
      object message,
      Throwable throwable)
    {
      this.ndcLookupRequired = true;
      this.mdcCopyLookupRequired = true;
      this.fqnOfCategoryClass = fqnOfCategoryClass;
      this.logger = logger;
      this.categoryName = logger.getName();
      this.level = priority;
      this.message = message;
      if (throwable != null)
        this.throwableInfo = new ThrowableInformation(throwable);
      this.timeStamp = timeStamp;
    }

    public virtual LocationInfo getLocationInformation()
    {
      if (this.locationInfo == null)
        this.locationInfo = new LocationInfo(new Throwable(), this.fqnOfCategoryClass);
      return this.locationInfo;
    }

    public virtual Level getLevel() => (Level) this.level;

    public virtual string getLoggerName() => this.categoryName;

    public virtual object getMessage() => this.message != null ? this.message : (object) this.getRenderedMessage();

    public virtual string getNDC()
    {
      if (this.ndcLookupRequired)
      {
        this.ndcLookupRequired = false;
        this.ndc = NDC.get();
      }
      return this.ndc;
    }

    public virtual void getMDCCopy()
    {
    }

    public virtual string getRenderedMessage()
    {
      if (this.renderedMessage == null && this.message != null)
      {
        if (\u003CVerifierFix\u003E.isInstanceOfString(this.message))
        {
          this.renderedMessage = \u003CVerifierFix\u003E.genCastToString(this.message);
        }
        else
        {
          LoggerRepository hierarchy = this.logger.getHierarchy();
          this.renderedMessage = !(hierarchy is RendererSupport) ? this.message.ToString() : ((RendererSupport) hierarchy).getRendererMap().findAndRender(this.message);
        }
      }
      return this.renderedMessage;
    }

    public static long getStartTime() => LoggingEvent.startTime;

    public virtual string getThreadName()
    {
      if (this.threadName == null)
        this.threadName = Thread.currentThread().getName();
      return this.threadName;
    }

    public virtual ThrowableInformation getThrowableInformation() => this.throwableInfo;

    public virtual string[] getThrowableStrRep() => this.throwableInfo == null ? (string[]) null : this.throwableInfo.getThrowableStrRep();

    [JavaThrownExceptions("2;java/io/IOException;java/lang/ClassNotFoundException;")]
    private void readLevel(ObjectInputStream ois)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("2;java/io/IOException;java/lang/ClassNotFoundException;")]
    private void readObject(ObjectInputStream ois)
    {
      ois.defaultReadObject();
      this.readLevel(ois);
      if (this.locationInfo != null)
        return;
      this.locationInfo = new LocationInfo((Throwable) null, (string) null);
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    private void writeObject(ObjectOutputStream oos)
    {
      this.getThreadName();
      this.getRenderedMessage();
      this.getNDC();
      this.getMDCCopy();
      this.getThrowableStrRep();
      oos.defaultWriteObject();
      this.writeLevel(oos);
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    private void writeLevel(ObjectOutputStream oos)
    {
      oos.writeInt(this.level.toInt());
      Class @class = ObjectImpl.getClass((object) this.level);
      if (@class == Class.FromType(typeof (Level)))
        oos.writeObject((object) null);
      else
        oos.writeObject((object) @class.getName());
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static LoggingEvent()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      LoggingEvent loggingEvent = this;
      ObjectImpl.clone((object) loggingEvent);
      return ((object) loggingEvent).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
