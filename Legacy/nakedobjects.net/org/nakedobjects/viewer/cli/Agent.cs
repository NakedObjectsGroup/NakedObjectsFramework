// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.Agent
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace org.nakedobjects.viewer.cli
{
  [JavaInterface]
  public interface Agent
  {
    string debug();

    void list(View view, string[] layout);

    string getName();

    Agent findAgent(string path);

    string getPrompt();

    bool isValueEntry();

    bool isReplaceable();
  }
}
