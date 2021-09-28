// Decompiled with JetBrains decompiler
// Type: org.xml.sax.SAXException
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.xml.sax
{
  public class SAXException : Exception
  {
    private Exception exception;

    public SAXException() => this.exception = (Exception) null;

    public SAXException(string message)
      : base(message)
    {
      this.exception = (Exception) null;
    }

    public SAXException(Exception e) => this.exception = e;

    public SAXException(string message, Exception e)
      : base(message)
    {
      this.exception = e;
    }

    public virtual string getMessage()
    {
      string message = ((Throwable) this).getMessage();
      return message == null && this.exception != null ? ((Throwable) this.exception).getMessage() : message;
    }

    public virtual Exception getException() => this.exception;

    public virtual string ToString() => this.exception != null ? ((Throwable) this.exception).ToString() : ((Throwable) this).ToString();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public virtual object MemberwiseClone()
    {
      SAXException saxException = this;
      ObjectImpl.clone((object) saxException);
      return ((object) saxException).MemberwiseClone();
    }
  }
}
