// Decompiled with JetBrains decompiler
// Type: junit.runner.SimpleTestCollector
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;

namespace junit.runner
{
  public class SimpleTestCollector : ClassPathTestCollector
  {
    [JavaFlags(4)]
    public override bool isTestClass(string classFileName) => StringImpl.endsWith(classFileName, ".class") && StringImpl.indexOf(classFileName, 36) < 0 && StringImpl.indexOf(classFileName, "Test") > 0;
  }
}
