// Decompiled with JetBrains decompiler
// Type: junit.runner.LoadingTestCollector
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using java.lang.reflect;
using junit.framework;

namespace junit.runner
{
  public class LoadingTestCollector : ClassPathTestCollector
  {
    [JavaFlags(0)]
    public TestCaseClassLoader fLoader;

    public LoadingTestCollector() => this.fLoader = new TestCaseClassLoader();

    [JavaFlags(4)]
    public override bool isTestClass(string classFileName)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(0)]
    [JavaThrownExceptions("1;java/lang/ClassNotFoundException;")]
    public virtual Class classFromFile(string classFileName)
    {
      string name = this.classNameFromFile(classFileName);
      return !this.fLoader.isExcluded(name) ? this.fLoader.loadClass(name, false) : (Class) null;
    }

    [JavaFlags(0)]
    public virtual bool isTestClass(Class testClass) => this.hasSuiteMethod(testClass) || Class.FromType(typeof (Test)).isAssignableFrom(testClass) && Modifier.isPublic(testClass.getModifiers()) && this.hasPublicConstructor(testClass);

    [JavaFlags(0)]
    public virtual bool hasSuiteMethod(Class testClass)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(0)]
    public virtual bool hasPublicConstructor(Class testClass)
    {
      try
      {
        TestSuite.getTestConstructor(testClass);
      }
      catch (NoSuchMethodException ex)
      {
        return false;
      }
      return true;
    }
  }
}
