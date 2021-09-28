// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.UnsupportedFindException
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.application
{
  public class UnsupportedFindException : Exception
  {
    public UnsupportedFindException()
    {
    }

    public UnsupportedFindException(string message)
      : base(message)
    {
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public virtual object MemberwiseClone()
    {
      UnsupportedFindException unsupportedFindException = this;
      ObjectImpl.clone((object) unsupportedFindException);
      return ((object) unsupportedFindException).MemberwiseClone();
    }
  }
}
