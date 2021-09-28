// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.helpers.CountingQuietWriter
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
  public class CountingQuietWriter : QuietWriter
  {
    [JavaFlags(4)]
    public long count;

    public CountingQuietWriter(Writer writer, ErrorHandler eh)
      : base(writer, eh)
    {
    }

    public override void write(string @string)
    {
      try
      {
        ((Writer) this.@out).write(@string);
        this.count += (long) StringImpl.length(@string);
      }
      catch (IOException ex)
      {
        this.errorHandler.error("Write failure.", (Exception) ex, 1);
      }
    }

    public virtual long getCount() => this.count;

    public virtual void setCount(long count) => this.count = count;
  }
}
