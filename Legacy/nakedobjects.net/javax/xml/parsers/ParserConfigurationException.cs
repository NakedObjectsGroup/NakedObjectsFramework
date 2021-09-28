// Decompiled with JetBrains decompiler
// Type: javax.xml.parsers.ParserConfigurationException
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace javax.xml.parsers
{
  public class ParserConfigurationException : Exception
  {
    public ParserConfigurationException()
    {
    }

    public ParserConfigurationException(string msg)
      : base(msg)
    {
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public virtual object MemberwiseClone()
    {
      ParserConfigurationException configurationException = this;
      ObjectImpl.clone((object) configurationException);
      return ((object) configurationException).MemberwiseClone();
    }
  }
}
