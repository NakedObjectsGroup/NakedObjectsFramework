// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.config.PropertySetterException
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.apache.log4j.config
{
  public class PropertySetterException : Exception
  {
    [JavaFlags(4)]
    public Throwable rootCause;

    public PropertySetterException(string msg)
      : base(msg)
    {
    }

    public PropertySetterException(Throwable rootCause) => this.rootCause = rootCause;

    public virtual string getMessage()
    {
      string message = ((Throwable) this).getMessage();
      if (message == null && this.rootCause != null)
        message = this.rootCause.getMessage();
      return message;
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public virtual object MemberwiseClone()
    {
      PropertySetterException propertySetterException = this;
      ObjectImpl.clone((object) propertySetterException);
      return ((object) propertySetterException).MemberwiseClone();
    }
  }
}
