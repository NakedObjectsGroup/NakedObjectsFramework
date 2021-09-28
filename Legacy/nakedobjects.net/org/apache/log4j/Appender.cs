// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.Appender
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.apache.log4j.spi;

namespace org.apache.log4j
{
  [JavaInterface]
  public interface Appender
  {
    void addFilter(Filter newFilter);

    Filter getFilter();

    void clearFilters();

    void close();

    void doAppend(LoggingEvent @event);

    string getName();

    void setErrorHandler(ErrorHandler errorHandler);

    ErrorHandler getErrorHandler();

    void setLayout(Layout layout);

    Layout getLayout();

    void setName(string name);

    bool requiresLayout();
  }
}
