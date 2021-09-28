// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.io.BinaryTransferableWriter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.io;
using java.lang;
using org.nakedobjects.@object.io;
using org.nakedobjects.utility;

namespace org.nakedobjects.@object.io
{
  public class BinaryTransferableWriter : AbstractTransferableWriter
  {
    private DataOutputStream dataOutputStream;
    private ByteArrayOutputStream byteArray;

    public BinaryTransferableWriter()
    {
      this.byteArray = new ByteArrayOutputStream();
      this.dataOutputStream = new DataOutputStream((OutputStream) this.byteArray);
    }

    public override void writeInt(int i)
    {
      try
      {
        this.dataOutputStream.writeInt(i);
      }
      catch (IOException ex)
      {
        throw new TransferableException((Throwable) ex);
      }
    }

    public override void writeString(string @string)
    {
      try
      {
        this.dataOutputStream.writeUTF(@string);
      }
      catch (IOException ex)
      {
        throw new TransferableException((Throwable) ex);
      }
    }

    public virtual sbyte[] getBinaryData()
    {
      sbyte[] byteArray = this.byteArray.toByteArray();
      try
      {
        ((FilterOutputStream) this.dataOutputStream).close();
      }
      catch (IOException ex)
      {
        throw new TransferableException((Throwable) ex);
      }
      return byteArray;
    }

    public override void writeLong(long value)
    {
      try
      {
        this.dataOutputStream.writeLong(value);
      }
      catch (IOException ex)
      {
        throw new TransferableException((Throwable) ex);
      }
    }

    public override void close()
    {
      try
      {
        ((FilterOutputStream) this.dataOutputStream).close();
        ((OutputStream) this.byteArray).close();
      }
      catch (IOException ex)
      {
        throw new NakedObjectRuntimeException();
      }
    }
  }
}
