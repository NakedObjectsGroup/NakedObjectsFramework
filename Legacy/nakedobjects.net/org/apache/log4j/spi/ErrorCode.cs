// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.spi.ErrorCode
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace org.apache.log4j.spi
{
  [JavaInterface]
  public interface ErrorCode
  {
    const int GENERIC_FAILURE = 0;
    const int WRITE_FAILURE = 1;
    const int FLUSH_FAILURE = 2;
    const int CLOSE_FAILURE = 3;
    const int FILE_OPEN_FAILURE = 4;
    const int MISSING_LAYOUT = 5;
    const int ADDRESS_PARSE_FAILURE = 6;
  }
}
