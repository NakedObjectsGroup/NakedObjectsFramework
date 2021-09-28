// Decompiled with JetBrains decompiler
// Type: junit.framework.TestResult
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace junit.framework
{
  public class TestResult
  {
    [JavaFlags(4)]
    public Vector fFailures;
    [JavaFlags(4)]
    public Vector fErrors;
    [JavaFlags(4)]
    public Vector fListeners;
    [JavaFlags(4)]
    public int fRunTests;
    private bool fStop;

    public TestResult()
    {
      this.fFailures = new Vector();
      this.fErrors = new Vector();
      this.fListeners = new Vector();
      this.fRunTests = 0;
      this.fStop = false;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void addError(Test test, Throwable t)
    {
      this.fErrors.addElement((object) new TestFailure(test, t));
      Enumeration enumeration = this.cloneListeners().elements();
      while (enumeration.hasMoreElements())
        ((TestListener) enumeration.nextElement()).addError(test, t);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void addFailure(Test test, AssertionFailedError t)
    {
      this.fFailures.addElement((object) new TestFailure(test, (Throwable) t));
      Enumeration enumeration = this.cloneListeners().elements();
      while (enumeration.hasMoreElements())
        ((TestListener) enumeration.nextElement()).addFailure(test, t);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void addListener(TestListener listener) => this.fListeners.addElement((object) listener);

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void removeListener(TestListener listener) => this.fListeners.removeElement((object) listener);

    [MethodImpl(MethodImplOptions.Synchronized)]
    private Vector cloneListeners() => (Vector) this.fListeners.MemberwiseClone();

    public virtual void endTest(Test test)
    {
      Enumeration enumeration = this.cloneListeners().elements();
      while (enumeration.hasMoreElements())
        ((TestListener) enumeration.nextElement()).endTest(test);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual int errorCount() => this.fErrors.size();

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual Enumeration errors() => this.fErrors.elements();

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual int failureCount() => this.fFailures.size();

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual Enumeration failures() => this.fFailures.elements();

    [JavaFlags(4)]
    public virtual void run(TestCase test)
    {
      this.startTest((Test) test);
      Protectable p = (Protectable) new TestResult.\u0031(this, test);
      this.runProtected((Test) test, p);
      this.endTest((Test) test);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual int runCount() => this.fRunTests;

    public virtual void runProtected(Test test, Protectable p)
    {
      // ISSUE: unable to decompile the method.
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual bool shouldStop() => this.fStop;

    public virtual void startTest(Test test)
    {
      int num = test.countTestCases();
      object obj = (object) this;
      \u003CCorArrayWrapper\u003E.Enter(obj);
      try
      {
        this.fRunTests += num;
      }
      finally
      {
        Monitor.Exit(obj);
      }
      Enumeration enumeration = this.cloneListeners().elements();
      while (enumeration.hasMoreElements())
        ((TestListener) enumeration.nextElement()).startTest(test);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void stop() => this.fStop = true;

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual bool wasSuccessful() => this.failureCount() == 0 && this.errorCount() == 0;

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      TestResult testResult = this;
      ObjectImpl.clone((object) testResult);
      return ((object) testResult).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [JavaInterfaces("1;junit/framework/Protectable;")]
    [JavaFlags(32)]
    [Inner]
    public class \u0031 : Protectable
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private TestResult this\u00240;
      [JavaFlags(16)]
      public readonly TestCase test_\u003E;

      [JavaThrownExceptions("1;java/lang/Throwable;")]
      public virtual void protect() => this.test_\u003E.runBare();

      public \u0031(TestResult _param1, [In] TestCase obj1)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.test_\u003E = obj1;
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        TestResult.\u0031 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
