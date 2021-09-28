// Decompiled with JetBrains decompiler
// Type: junit.runner.TestSuiteLoader
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;

namespace junit.runner
{
  [JavaInterface]
  public interface TestSuiteLoader
  {
    [JavaThrownExceptions("1;java/lang/ClassNotFoundException;")]
    Class load(string suiteClassName);

    [JavaThrownExceptions("1;java/lang/ClassNotFoundException;")]
    Class reload(Class aClass);
  }
}
