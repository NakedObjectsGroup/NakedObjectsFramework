// Decompiled with JetBrains decompiler
// Type: javax.xml.parsers.FactoryConfigurationError
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace javax.xml.parsers
{
  public class FactoryConfigurationError : Error
  {
    private Exception exception;

    public FactoryConfigurationError() => this.exception = (Exception) null;

    public FactoryConfigurationError(string msg)
      : base(msg)
    {
      this.exception = (Exception) null;
    }

    public FactoryConfigurationError(Exception e)
      : base(((Throwable) e).ToString())
    {
      this.exception = e;
    }

    public FactoryConfigurationError(Exception e, string msg)
      : base(msg)
    {
      this.exception = e;
    }

    public virtual string getMessage()
    {
      string message = ((Throwable) this).getMessage();
      return message == null && this.exception != null ? ((Throwable) this.exception).getMessage() : message;
    }

    public virtual Exception getException() => this.exception;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public virtual object MemberwiseClone()
    {
      FactoryConfigurationError configurationError = this;
      ObjectImpl.clone((object) configurationError);
      return ((object) configurationError).MemberwiseClone();
    }
  }
}
