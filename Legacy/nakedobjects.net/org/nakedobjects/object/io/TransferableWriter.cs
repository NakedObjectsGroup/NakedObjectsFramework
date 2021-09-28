// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.io.TransferableWriter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object.io;

namespace org.nakedobjects.@object.io
{
  [JavaInterface]
  public interface TransferableWriter
  {
    void writeInt(int i);

    void writeObject(Transferable @object);

    void writeString(string @string);

    void writeLong(long value);

    void close();
  }
}
