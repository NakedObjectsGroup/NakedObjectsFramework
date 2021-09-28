// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.io.AbstractTransferableReader
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object.io;

namespace org.nakedobjects.@object.io
{
  [JavaInterfaces("1;org/nakedobjects/object/io/TransferableReader;")]
  public abstract class AbstractTransferableReader : TransferableReader
  {
    public virtual Transferable readObject()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      AbstractTransferableReader transferableReader = this;
      ObjectImpl.clone((object) transferableReader);
      return ((object) transferableReader).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    public abstract void close();

    public abstract int readInt();

    public abstract long readLong();

    public abstract string readString();
  }
}
