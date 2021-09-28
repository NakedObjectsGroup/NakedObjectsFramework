// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.io.Data
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.io;

namespace org.nakedobjects.@object.io
{
  [JavaFlags(32)]
  [JavaInterfaces("2;org/nakedobjects/object/io/Transferable;java/io/Serializable;")]
  public class Data : Transferable, Serializable
  {
    private const long serialVersionUID = 1;
    [JavaFlags(16)]
    public readonly string className;
    [JavaFlags(16)]
    public readonly string resolveState;
    [JavaFlags(16)]
    public readonly Oid oid;

    public Data(Oid oid, string resolveState, string className)
    {
      this.oid = oid;
      this.className = className;
      this.resolveState = resolveState;
    }

    public override string ToString() => new StringBuffer().append(this.className).append("/").append((object) this.oid).ToString();

    public virtual Oid getOid() => this.oid;

    public virtual void writeData(TransferableWriter data)
    {
      data.writeString(this.className);
      data.writeString(this.resolveState);
      data.writeObject((Transferable) this.oid);
    }

    public Data(TransferableReader data)
    {
      this.className = data.readString();
      this.resolveState = data.readString();
      this.oid = (Oid) data.readObject();
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      Data data = this;
      ObjectImpl.clone((object) data);
      return ((object) data).MemberwiseClone();
    }
  }
}
