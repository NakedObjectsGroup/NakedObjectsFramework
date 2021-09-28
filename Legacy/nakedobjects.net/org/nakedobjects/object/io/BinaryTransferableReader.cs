// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.io.BinaryTransferableReader
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.io;
using org.nakedobjects.@object.io;
using org.nakedobjects.utility;

namespace org.nakedobjects.@object.io
{
  public class BinaryTransferableReader : AbstractTransferableReader
  {
    private DataInputStream dataInputStream;

    public BinaryTransferableReader(sbyte[] data) => this.dataInputStream = new DataInputStream((InputStream) new ByteArrayInputStream(data));

    public override int readInt()
    {
      try
      {
        return this.dataInputStream.readInt();
      }
      catch (IOException ex)
      {
        throw new NakedObjectRuntimeException();
      }
    }

    public override string readString()
    {
      try
      {
        return this.dataInputStream.readUTF();
      }
      catch (IOException ex)
      {
        throw new NakedObjectRuntimeException();
      }
    }

    public override long readLong()
    {
      try
      {
        return this.dataInputStream.readLong();
      }
      catch (IOException ex)
      {
        throw new NakedObjectRuntimeException();
      }
    }

    public override void close()
    {
      try
      {
        ((FilterInputStream) this.dataInputStream).close();
      }
      catch (IOException ex)
      {
        throw new NakedObjectRuntimeException();
      }
    }
  }
}
