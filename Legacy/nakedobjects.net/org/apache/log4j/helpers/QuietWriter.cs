// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.helpers.QuietWriter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using org.apache.log4j.spi;

namespace org.apache.log4j.helpers
{
  public class QuietWriter : FilterWriter
  {
    [JavaFlags(4)]
    public ErrorHandler errorHandler;

    public QuietWriter(Writer writer, ErrorHandler errorHandler)
      : base(writer)
    {
      this.setErrorHandler(errorHandler);
    }

    public virtual void write(string @string)
    {
      try
      {
        ((Writer) this.@out).write(@string);
      }
      catch (IOException ex)
      {
        this.errorHandler.error(new StringBuffer().append("Failed to write [").append(@string).append("].").ToString(), (Exception) ex, 1);
      }
    }

    public virtual void flush()
    {
      try
      {
        ((Writer) this.@out).flush();
      }
      catch (IOException ex)
      {
        this.errorHandler.error("Failed to flush writer,", (Exception) ex, 2);
      }
    }

    public virtual void setErrorHandler(ErrorHandler eh) => this.errorHandler = eh != null ? eh : throw new IllegalArgumentException("Attempted to set null ErrorHandler.");

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public virtual object MemberwiseClone()
    {
      QuietWriter quietWriter = this;
      ObjectImpl.clone((object) quietWriter);
      return ((object) quietWriter).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public virtual string ToString() => ObjectImpl.jloToString((object) this);
  }
}
