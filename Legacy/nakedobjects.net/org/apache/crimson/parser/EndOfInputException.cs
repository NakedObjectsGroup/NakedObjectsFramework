// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.parser.EndOfInputException
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;

namespace org.apache.crimson.parser
{
  [JavaFlags(32)]
  public class EndOfInputException : IOException
  {
    [JavaFlags(0)]
    public EndOfInputException()
    {
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public virtual object MemberwiseClone()
    {
      EndOfInputException ofInputException = this;
      ObjectImpl.clone((object) ofInputException);
      return ((object) ofInputException).MemberwiseClone();
    }
  }
}
