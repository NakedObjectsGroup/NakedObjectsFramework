// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.distribution.command.Request
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace org.nakedobjects.distribution.command
{
  [JavaInterface]
  public interface Request
  {
    void execute(Distribution distribution);

    void setResponse(object response);

    object getResponse();

    int getId();
  }
}
