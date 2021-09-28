// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.View
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object;

namespace org.nakedobjects.viewer.cli
{
  [JavaInterface]
  public interface View
  {
    void clear();

    void connect();

    void disconnect();

    void display(NakedObject @object);

    void display(string message);

    void displayEntry(string entry);

    void error(string message);

    void prompt(string prompt);
  }
}
