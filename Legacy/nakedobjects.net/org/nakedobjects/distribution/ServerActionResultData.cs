// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.ServerActionResultData
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.util;

namespace org.nakedobjects.distribution
{
  [JavaInterface]
  public interface ServerActionResultData
  {
    Data getReturn();

    ObjectData getPersistedTarget();

    ObjectData[] getPersistedParameters();

    ObjectData[] getUpdates();

    string[] getMessages();

    string[] getWarnings();

    Hashtable getResolvedOids();
  }
}
