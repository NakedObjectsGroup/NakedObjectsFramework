// Decompiled with JetBrains decompiler
// Type: junit.runner.ReloadingTestSuiteLoader
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace junit.runner
{
  [JavaInterfaces("1;junit/runner/TestSuiteLoader;")]
  public class ReloadingTestSuiteLoader : TestSuiteLoader
  {
    [JavaThrownExceptions("1;java/lang/ClassNotFoundException;")]
    public virtual Class load(string suiteClassName) => this.createLoader().loadClass(suiteClassName, true);

    [JavaThrownExceptions("1;java/lang/ClassNotFoundException;")]
    public virtual Class reload(Class aClass) => this.createLoader().loadClass(aClass.getName(), true);

    [JavaFlags(4)]
    public virtual TestCaseClassLoader createLoader() => new TestCaseClassLoader();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ReloadingTestSuiteLoader reloadingTestSuiteLoader = this;
      ObjectImpl.clone((object) reloadingTestSuiteLoader);
      return ((object) reloadingTestSuiteLoader).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
