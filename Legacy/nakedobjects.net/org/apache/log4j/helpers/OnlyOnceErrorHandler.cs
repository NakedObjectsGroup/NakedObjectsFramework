// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.helpers.OnlyOnceErrorHandler
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.apache.log4j.spi;

namespace org.apache.log4j.helpers
{
  [JavaInterfaces("1;org/apache/log4j/spi/ErrorHandler;")]
  public class OnlyOnceErrorHandler : ErrorHandler
  {
    [JavaFlags(16)]
    public readonly string WARN_PREFIX;
    [JavaFlags(16)]
    public readonly string ERROR_PREFIX;
    [JavaFlags(0)]
    public bool firstTime;

    public virtual void setLogger(Logger logger)
    {
    }

    public virtual void activateOptions()
    {
    }

    public virtual void error(string message, Exception e, int errorCode) => this.error(message, e, errorCode, (LoggingEvent) null);

    public virtual void error(string message, Exception e, int errorCode, LoggingEvent @event)
    {
      if (!this.firstTime)
        return;
      LogLog.error(message, (Throwable) e);
      this.firstTime = false;
    }

    public virtual void error(string message)
    {
      if (!this.firstTime)
        return;
      LogLog.error(message);
      this.firstTime = false;
    }

    public virtual void setAppender(Appender appender)
    {
    }

    public virtual void setBackupAppender(Appender appender)
    {
    }

    public OnlyOnceErrorHandler()
    {
      this.WARN_PREFIX = "log4j warning: ";
      this.ERROR_PREFIX = "log4j error: ";
      this.firstTime = true;
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      OnlyOnceErrorHandler onceErrorHandler = this;
      ObjectImpl.clone((object) onceErrorHandler);
      return ((object) onceErrorHandler).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
