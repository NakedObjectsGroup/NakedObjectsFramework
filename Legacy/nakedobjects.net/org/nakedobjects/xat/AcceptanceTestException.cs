// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.xat.AcceptanceTestException
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.xat
{
  public class AcceptanceTestException : RuntimeException
  {
    private Throwable cause;

    public AcceptanceTestException(string message)
      : base(message)
    {
    }

    public AcceptanceTestException(string message, Exception cause)
      : base(message)
    {
      this.cause = (Throwable) cause;
    }

    public virtual Throwable getCause() => this.cause;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public virtual object MemberwiseClone()
    {
      AcceptanceTestException acceptanceTestException = this;
      ObjectImpl.clone((object) acceptanceTestException);
      return ((object) acceptanceTestException).MemberwiseClone();
    }
  }
}
