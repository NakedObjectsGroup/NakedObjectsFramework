// Decompiled with JetBrains decompiler
// Type: junit.framework.Test
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace junit.framework
{
  [JavaInterface]
  public interface Test
  {
    int countTestCases();

    void run(TestResult result);
  }
}
