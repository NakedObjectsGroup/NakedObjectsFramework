// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.spi.NullWriter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;

namespace org.apache.log4j.spi
{
  [JavaFlags(32)]
  public class NullWriter : Writer
  {
    public virtual void close()
    {
    }

    public virtual void flush()
    {
    }

    public virtual void write(char[] cbuf, int off, int len)
    {
    }

    [JavaFlags(0)]
    public NullWriter()
    {
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public virtual object MemberwiseClone()
    {
      NullWriter nullWriter = this;
      ObjectImpl.clone((object) nullWriter);
      return ((object) nullWriter).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public virtual string ToString() => ObjectImpl.jloToString((object) this);
  }
}
