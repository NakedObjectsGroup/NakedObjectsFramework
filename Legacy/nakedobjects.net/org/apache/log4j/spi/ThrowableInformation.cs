// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.spi.ThrowableInformation
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using System;

namespace org.apache.log4j.spi
{
  [JavaInterfaces("1;java/io/Serializable;")]
  public class ThrowableInformation : Serializable
  {
    [JavaFlags(24)]
    public const long serialVersionUID = -4748765566864322735;
    [JavaFlags(130)]
    [NonSerialized]
    private Throwable throwable;
    private string[] rep;

    public ThrowableInformation(Throwable throwable) => this.throwable = throwable;

    public virtual Throwable getThrowable() => this.throwable;

    public virtual string[] getThrowableStrRep()
    {
      if (this.rep != null)
        return (string[]) \u003CCorArrayWrapper\u003E.clone((object) this.rep);
      VectorWriter vectorWriter = new VectorWriter();
      this.throwable.printStackTrace((PrintWriter) vectorWriter);
      this.rep = vectorWriter.toStringArray();
      return this.rep;
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      ThrowableInformation throwableInformation = this;
      ObjectImpl.clone((object) throwableInformation);
      return ((object) throwableInformation).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
