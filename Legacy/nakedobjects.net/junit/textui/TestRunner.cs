// Decompiled with JetBrains decompiler
// Type: junit.textui.TestRunner
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.io;
using java.lang;
using junit.framework;
using junit.runner;

namespace junit.textui
{
  public class TestRunner : BaseTestRunner
  {
    private ResultPrinter fPrinter;
    public const int SUCCESS_EXIT = 0;
    public const int FAILURE_EXIT = 1;
    public const int EXCEPTION_EXIT = 2;

    public TestRunner()
      : this((PrintStream) System.@out)
    {
    }

    public TestRunner(PrintStream writer)
      : this(new ResultPrinter(writer))
    {
    }

    public TestRunner(ResultPrinter printer) => this.fPrinter = printer;

    public static void run(Class testClass) => TestRunner.run((Test) new TestSuite(testClass));

    public static TestResult run(Test test) => new TestRunner().doRun(test);

    public static void runAndWait(Test suite) => new TestRunner().doRun(suite, true);

    public override TestSuiteLoader getLoader() => (TestSuiteLoader) new StandardTestSuiteLoader();

    public override void testFailed(int status, Test test, Throwable t)
    {
    }

    public override void testStarted(string testName)
    {
    }

    public override void testEnded(string testName)
    {
    }

    [JavaFlags(4)]
    public virtual TestResult createTestResult() => new TestResult();

    public virtual TestResult doRun(Test test) => this.doRun(test, false);

    public virtual TestResult doRun(Test suite, bool wait)
    {
      TestResult testResult = this.createTestResult();
      testResult.addListener((TestListener) this.fPrinter);
      long num = System.currentTimeMillis();
      suite.run(testResult);
      long runTime = System.currentTimeMillis() - num;
      this.fPrinter.print(testResult, runTime);
      this.pause(wait);
      return testResult;
    }

    [JavaFlags(4)]
    public virtual void pause(bool wait)
    {
      // ISSUE: unable to decompile the method.
    }

    public static void main(string[] args)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/Exception;")]
    [JavaFlags(4)]
    public virtual TestResult start(string[] args)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4)]
    public override void runFailed(string message)
    {
      ((PrintStream) System.err).println(message);
      System.exit(1);
    }

    public virtual void setPrinter(ResultPrinter printer) => this.fPrinter = printer;
  }
}
