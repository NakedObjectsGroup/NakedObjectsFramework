// Decompiled with JetBrains decompiler
// Type: junit.framework.TestCase
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using System;
using System.Reflection;

namespace junit.framework
{
  [JavaInterfaces("1;junit/framework/Test;")]
  public abstract class TestCase : Assert, Test
  {
    private string fName;

    public TestCase() => this.fName = (string) null;

    public TestCase(string name) => this.fName = name;

    public virtual int countTestCases() => 1;

    [JavaFlags(4)]
    public virtual TestResult createResult() => new TestResult();

    public virtual TestResult run()
    {
      TestResult result = this.createResult();
      this.run(result);
      return result;
    }

    public virtual void run(TestResult result) => result.run(this);

    [JavaThrownExceptions("1;java/lang/Throwable;")]
    public virtual void runBare()
    {
      this.setUp();
      try
      {
        this.runTest();
      }
      finally
      {
        this.tearDown();
      }
    }

    [JavaThrownExceptions("1;java/lang/Throwable;")]
    [JavaFlags(4)]
    public virtual void runTest()
    {
      Assert.assertNotNull((object) this.fName);
      try
      {
        Type type = this.GetType();
        string fName = this.fName;
        int length = 0;
        object[] args = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        type.InvokeMember(fName, BindingFlags.InvokeMethod, (Binder) null, (object) this, args);
      }
      catch (TargetInvocationException ex)
      {
        Exception innerException = ex.InnerException;
        Throwable throwable = innerException is Throwable ? (Throwable) innerException : throw new Throwable(new StringBuffer().append("\n").append(innerException.Message).append("\n").append(StringImpl.toString(innerException.StackTrace)).ToString());
        throwable.printStackTrace();
        throw throwable;
      }
    }

    [JavaFlags(4)]
    [JavaThrownExceptions("1;java/lang/Exception;")]
    public virtual void setUp()
    {
    }

    [JavaThrownExceptions("1;java/lang/Exception;")]
    [JavaFlags(4)]
    public virtual void tearDown()
    {
    }

    public override string ToString() => new StringBuffer().append(this.getName()).append("(").append(ObjectImpl.getClass((object) this).getName()).append(")").ToString();

    public virtual string getName() => this.fName;

    public virtual void setName(string name) => this.fName = name;
  }
}
