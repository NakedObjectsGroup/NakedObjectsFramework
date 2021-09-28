// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.control.Validity
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace org.nakedobjects.application.control
{
  [JavaInterface]
  public interface Validity
  {
    void cannotBeEmpty();

    string getReason();

    void invalid(string reason);

    void invalidOnCondition(bool condition, string reason);

    void invalidUnlessCondition(bool condition, string reason);

    bool isValid();
  }
}
