// Decompiled with JetBrains decompiler
// Type: junit.textui.ResultPrinter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.text;
using java.util;
using junit.framework;
using junit.runner;
using System.Runtime.CompilerServices;

namespace junit.textui
{
  [JavaInterfaces("1;junit/framework/TestListener;")]
  public class ResultPrinter : TestListener
  {
    [JavaFlags(0)]
    public PrintStream fWriter;
    [JavaFlags(0)]
    public int fColumn;

    public ResultPrinter(PrintStream writer)
    {
      this.fColumn = 0;
      this.fWriter = writer;
    }

    [JavaFlags(32)]
    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void print(TestResult result, long runTime)
    {
      this.printHeader(runTime);
      this.printErrors(result);
      this.printFailures(result);
      this.printFooter(result);
    }

    [JavaFlags(0)]
    public virtual void printWaitPrompt()
    {
      this.getWriter().println();
      this.getWriter().println("<RETURN> to continue");
    }

    [JavaFlags(4)]
    public virtual void printHeader(long runTime)
    {
      this.getWriter().println();
      this.getWriter().println(new StringBuffer().append("Time: ").append(this.elapsedTimeAsString(runTime)).ToString());
    }

    [JavaFlags(4)]
    public virtual void printErrors(TestResult result) => this.printDefects(result.errors(), result.errorCount(), "error");

    [JavaFlags(4)]
    public virtual void printFailures(TestResult result) => this.printDefects(result.failures(), result.failureCount(), "failure");

    [JavaFlags(4)]
    public virtual void printDefects(Enumeration booBoos, int count, string type)
    {
      switch (count)
      {
        case 0:
          return;
        case 1:
          this.getWriter().println(new StringBuffer().append("There was ").append(count).append(" ").append(type).append(":").ToString());
          break;
        default:
          this.getWriter().println(new StringBuffer().append("There were ").append(count).append(" ").append(type).append("s:").ToString());
          break;
      }
      int count1 = 1;
      while (booBoos.hasMoreElements())
      {
        this.printDefect((TestFailure) booBoos.nextElement(), count1);
        ++count1;
      }
    }

    public virtual void printDefect(TestFailure booBoo, int count)
    {
      this.printDefectHeader(booBoo, count);
      this.printDefectTrace(booBoo);
    }

    [JavaFlags(4)]
    public virtual void printDefectHeader(TestFailure booBoo, int count) => this.getWriter().print(new StringBuffer().append(count).append(") ").append((object) booBoo.failedTest()).ToString());

    [JavaFlags(4)]
    public virtual void printDefectTrace(TestFailure booBoo) => this.getWriter().print(BaseTestRunner.getFilteredTrace(booBoo.trace()));

    [JavaFlags(4)]
    public virtual void printFooter(TestResult result)
    {
      if (result.wasSuccessful())
      {
        this.getWriter().println();
        this.getWriter().print("OK");
        this.getWriter().println(new StringBuffer().append(" (").append(result.runCount()).append(" test").append(result.runCount() != 1 ? "s" : "").append(")").ToString());
      }
      else
      {
        this.getWriter().println();
        this.getWriter().println("FAILURES!!!");
        this.getWriter().println(new StringBuffer().append("Tests run: ").append(result.runCount()).append(",  Failures: ").append(result.failureCount()).append(",  Errors: ").append(result.errorCount()).ToString());
      }
      this.getWriter().println();
    }

    [JavaFlags(4)]
    public virtual string elapsedTimeAsString(long runTime) => NumberFormat.getInstance().format((double) runTime / 1000.0);

    public virtual PrintStream getWriter() => this.fWriter;

    public virtual void addError(Test test, Throwable t) => this.getWriter().print("E");

    public virtual void addFailure(Test test, AssertionFailedError t) => this.getWriter().print("F");

    public virtual void endTest(Test test)
    {
    }

    public virtual void startTest(Test test)
    {
      this.getWriter().print(".");
      int fColumn;
      this.fColumn = (fColumn = this.fColumn) + 1;
      if (fColumn < 40)
        return;
      this.getWriter().println();
      this.fColumn = 0;
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ResultPrinter resultPrinter = this;
      ObjectImpl.clone((object) resultPrinter);
      return ((object) resultPrinter).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
