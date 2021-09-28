// Decompiled with JetBrains decompiler
// Type: junit.framework.AssertionFailedError
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace junit.framework
{
  public class AssertionFailedError : Error
  {
    public AssertionFailedError()
    {
    }

    public AssertionFailedError(string message)
      : base(message)
    {
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public virtual object MemberwiseClone()
    {
      AssertionFailedError assertionFailedError = this;
      ObjectImpl.clone((object) assertionFailedError);
      return ((object) assertionFailedError).MemberwiseClone();
    }
  }
}
