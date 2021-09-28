// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.io.Null
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using org.nakedobjects.@object.io;

namespace org.nakedobjects.@object.io
{
  [JavaInterfaces("2;org/nakedobjects/object/io/Transferable;java/io/Serializable;")]
  [JavaFlags(32)]
  public class Null : Transferable, Serializable
  {
    private const long serialVersionUID = 5729106816298944191;

    public Null()
    {
    }

    public Null(TransferableReader data)
    {
    }

    public override string ToString() => "NULL";

    public virtual void writeData(TransferableWriter data)
    {
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      Null @null = this;
      ObjectImpl.clone((object) @null);
      return ((object) @null).MemberwiseClone();
    }
  }
}
