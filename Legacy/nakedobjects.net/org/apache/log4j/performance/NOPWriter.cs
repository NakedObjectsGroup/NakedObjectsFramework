// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.performance.NOPWriter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;

namespace org.apache.log4j.performance
{
  public class NOPWriter : Writer
  {
    [JavaThrownExceptions("1;java/io/IOException;")]
    public virtual void write(char[] cbuf)
    {
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public virtual void write(char[] cbuf, int off, int len)
    {
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public virtual void write(int b)
    {
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public virtual void write(string s)
    {
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public virtual void write(string s, int off, int len)
    {
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public virtual void flush()
    {
    }

    [JavaThrownExceptions("1;java/io/IOException;")]
    public virtual void close() => ((PrintStream) System.err).println("Close called.");

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public virtual object MemberwiseClone()
    {
      NOPWriter nopWriter = this;
      ObjectImpl.clone((object) nopWriter);
      return ((object) nopWriter).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public virtual string ToString() => ObjectImpl.jloToString((object) this);
  }
}
