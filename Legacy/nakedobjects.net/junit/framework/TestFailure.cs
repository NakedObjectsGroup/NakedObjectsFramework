// Decompiled with JetBrains decompiler
// Type: junit.framework.TestFailure
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;

namespace junit.framework
{
  public class TestFailure
  {
    [JavaFlags(4)]
    public Test fFailedTest;
    [JavaFlags(4)]
    public Throwable fThrownException;

    public TestFailure(Test failedTest, Throwable thrownException)
    {
      this.fFailedTest = failedTest;
      this.fThrownException = thrownException;
    }

    public virtual Test failedTest() => this.fFailedTest;

    public virtual Throwable thrownException() => this.fThrownException;

    public override string ToString()
    {
      StringBuffer stringBuffer = new StringBuffer();
      stringBuffer.append(new StringBuffer().append((object) this.fFailedTest).append(": ").append(this.fThrownException.getMessage()).ToString());
      return stringBuffer.ToString();
    }

    public virtual string trace()
    {
      StringWriter stringWriter = new StringWriter();
      PrintWriter printWriter = new PrintWriter((Writer) stringWriter);
      this.thrownException().printStackTrace(printWriter);
      return stringWriter.getBuffer().ToString();
    }

    public virtual string exceptionMessage() => this.thrownException().getMessage();

    public virtual bool isFailure() => this.thrownException() is AssertionFailedError;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      TestFailure testFailure = this;
      ObjectImpl.clone((object) testFailure);
      return ((object) testFailure).MemberwiseClone();
    }
  }
}
