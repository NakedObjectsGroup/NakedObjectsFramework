// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.control.Certificate
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;

namespace org.nakedobjects.application.control
{
  [JavaInterfaces("1;java/io/Serializable;")]
  public class Certificate : Serializable
  {
    private const long serialVersionUID = 1;
    private long id;

    public override bool Equals(object obj) => obj == this || obj is Certificate && ((Certificate) obj).id == this.id;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      Certificate certificate = this;
      ObjectImpl.clone((object) certificate);
      return ((object) certificate).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
