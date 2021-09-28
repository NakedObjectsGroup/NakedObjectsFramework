// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.io.AbstractTransferableWriter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object.io;

namespace org.nakedobjects.@object.io
{
  [JavaInterfaces("1;org/nakedobjects/object/io/TransferableWriter;")]
  public abstract class AbstractTransferableWriter : TransferableWriter
  {
    public virtual void writeObject(Transferable @object)
    {
      this.writeString(ObjectImpl.getClass((object) @object).getName());
      @object.writeData((TransferableWriter) this);
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      AbstractTransferableWriter transferableWriter = this;
      ObjectImpl.clone((object) transferableWriter);
      return ((object) transferableWriter).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    public abstract void close();

    public abstract void writeInt(int i);

    public abstract void writeLong(long value);

    public abstract void writeString(string @string);
  }
}
