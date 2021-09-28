// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.spi.AppenderAttachable
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.util;

namespace org.apache.log4j.spi
{
  [JavaInterface]
  public interface AppenderAttachable
  {
    void addAppender(Appender newAppender);

    Enumeration getAllAppenders();

    Appender getAppender(string name);

    bool isAttached(Appender appender);

    void removeAllAppenders();

    void removeAppender(Appender appender);

    void removeAppender(string name);
  }
}
