// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.spi.ErrorHandler
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;

namespace org.apache.log4j.spi
{
  [JavaInterfaces("1;org/apache/log4j/spi/OptionHandler;")]
  [JavaInterface]
  public interface ErrorHandler : OptionHandler
  {
    void setLogger(Logger logger);

    void error(string message, Exception e, int errorCode);

    void error(string message);

    void error(string message, Exception e, int errorCode, LoggingEvent @event);

    void setAppender(Appender appender);

    void setBackupAppender(Appender appender);
  }
}
