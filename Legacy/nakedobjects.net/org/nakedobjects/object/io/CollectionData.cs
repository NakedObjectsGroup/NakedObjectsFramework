// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.io.CollectionData
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.io;

namespace org.nakedobjects.@object.io
{
  [JavaFlags(32)]
  public class CollectionData : Data
  {
    private const long serialVersionUID = 1;
    [JavaFlags(16)]
    public readonly Data[] elements;

    public CollectionData(Oid oid, string className, Data[] elements)
      : base(oid, "", className)
    {
      this.elements = elements;
    }

    public CollectionData(TransferableReader data)
      : base(data)
    {
      int num = data.readInt();
      int length = num;
      this.elements = length >= 0 ? new Data[length] : throw new NegativeArraySizeException();
      for (int index = 0; index < num; ++index)
        this.elements[index] = (Data) data.readObject();
    }

    public override void writeData(TransferableWriter data)
    {
      base.writeData(data);
      int length = this.elements.Length;
      data.writeInt(length);
      for (int index = 0; index < length; ++index)
        data.writeObject((Transferable) this.elements[index]);
    }

    public override string ToString()
    {
      StringBuffer stringBuffer = new StringBuffer("(");
      for (int index = 0; index < this.elements.Length; ++index)
      {
        stringBuffer.append(index <= 0 ? "" : ",");
        stringBuffer.append((object) this.elements[index]);
      }
      stringBuffer.append(")");
      return stringBuffer.ToString();
    }
  }
}
