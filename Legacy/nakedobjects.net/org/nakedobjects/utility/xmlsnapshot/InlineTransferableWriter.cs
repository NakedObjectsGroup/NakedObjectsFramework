// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.xmlsnapshot.InlineTransferableWriter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object.io;

namespace org.nakedobjects.utility.xmlsnapshot
{
  [JavaFlags(48)]
  [JavaInterfaces("1;org/nakedobjects/object/io/TransferableWriter;")]
  public sealed class InlineTransferableWriter : TransferableWriter
  {
    public const string SEQUENCE_START = "{";
    public const string SEQUENCE_END = "}";
    private readonly StringBuffer buffer;
    private string asString;

    [JavaFlags(0)]
    public InlineTransferableWriter()
    {
      this.buffer = new StringBuffer();
      this.buffer.append("{");
    }

    public virtual void writeInt(int i) => this.buffer.append("{").append(i).append("I").append("}");

    public virtual void writeString(string str) => this.buffer.append("{").append(str).append("}");

    public virtual void writeLong(long l) => this.buffer.append("{").append(l).append("J").append("}");

    public virtual void writeObject(Transferable t)
    {
      InlineTransferableWriter transferableWriter = new InlineTransferableWriter();
      t.writeData((TransferableWriter) transferableWriter);
      transferableWriter.close();
      this.buffer.append(transferableWriter.ToString());
    }

    public virtual void close()
    {
      this.buffer.append("}");
      this.asString = this.buffer.ToString();
    }

    public override string ToString() => this.asString;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      InlineTransferableWriter transferableWriter = this;
      ObjectImpl.clone((object) transferableWriter);
      return ((object) transferableWriter).MemberwiseClone();
    }
  }
}
