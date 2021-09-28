// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.helpers.CyclicBuffer
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.apache.log4j.spi;

namespace org.apache.log4j.helpers
{
  public class CyclicBuffer
  {
    [JavaFlags(0)]
    public LoggingEvent[] ea;
    [JavaFlags(0)]
    public int first;
    [JavaFlags(0)]
    public int last;
    [JavaFlags(0)]
    public int numElems;
    [JavaFlags(0)]
    public int maxSize;

    [JavaThrownExceptions("1;java/lang/IllegalArgumentException;")]
    public CyclicBuffer(int maxSize)
    {
      this.maxSize = maxSize >= 1 ? maxSize : throw new IllegalArgumentException(new StringBuffer().append("The maxSize argument (").append(maxSize).append(") is not a positive integer.").ToString());
      int length = maxSize;
      this.ea = length >= 0 ? new LoggingEvent[length] : throw new NegativeArraySizeException();
      this.first = 0;
      this.last = 0;
      this.numElems = 0;
    }

    public virtual void add(LoggingEvent @event)
    {
      this.ea[this.last] = @event;
      if (++this.last == this.maxSize)
        this.last = 0;
      if (this.numElems < this.maxSize)
      {
        ++this.numElems;
      }
      else
      {
        if (++this.first != this.maxSize)
          return;
        this.first = 0;
      }
    }

    public virtual LoggingEvent get(int i) => i < 0 || i >= this.numElems ? (LoggingEvent) null : this.ea[(this.first + i) % this.maxSize];

    public virtual int getMaxSize() => this.maxSize;

    public virtual LoggingEvent get()
    {
      LoggingEvent loggingEvent = (LoggingEvent) null;
      if (this.numElems > 0)
      {
        this.numElems += -1;
        loggingEvent = this.ea[this.first];
        this.ea[this.first] = (LoggingEvent) null;
        if (++this.first == this.maxSize)
          this.first = 0;
      }
      return loggingEvent;
    }

    public virtual int length() => this.numElems;

    public virtual void resize(int newSize)
    {
      if (newSize < 0)
        throw new IllegalArgumentException(new StringBuffer().append("Negative array size [").append(newSize).append("] not allowed.").ToString());
      if (newSize == this.numElems)
        return;
      int length = newSize;
      LoggingEvent[] loggingEventArray = length >= 0 ? new LoggingEvent[length] : throw new NegativeArraySizeException();
      int num = newSize >= this.numElems ? this.numElems : newSize;
      for (int index = 0; index < num; ++index)
      {
        loggingEventArray[index] = this.ea[this.first];
        this.ea[this.first] = (LoggingEvent) null;
        if (++this.first == this.numElems)
          this.first = 0;
      }
      this.ea = loggingEventArray;
      this.first = 0;
      this.numElems = num;
      this.maxSize = newSize;
      if (num == newSize)
        this.last = 0;
      else
        this.last = num;
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      CyclicBuffer cyclicBuffer = this;
      ObjectImpl.clone((object) cyclicBuffer);
      return ((object) cyclicBuffer).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
