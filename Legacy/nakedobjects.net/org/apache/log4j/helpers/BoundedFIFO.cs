// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.helpers.BoundedFIFO
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.apache.log4j.spi;
using System.Runtime.CompilerServices;

namespace org.apache.log4j.helpers
{
  public class BoundedFIFO
  {
    [JavaFlags(0)]
    public LoggingEvent[] buf;
    [JavaFlags(0)]
    public int numElements;
    [JavaFlags(0)]
    public int first;
    [JavaFlags(0)]
    public int next;
    [JavaFlags(0)]
    public int maxSize;

    public BoundedFIFO(int maxSize)
    {
      this.numElements = 0;
      this.first = 0;
      this.next = 0;
      this.maxSize = maxSize >= 1 ? maxSize : throw new IllegalArgumentException(new StringBuffer().append("The maxSize argument (").append(maxSize).append(") is not a positive integer.").ToString());
      int length = maxSize;
      this.buf = length >= 0 ? new LoggingEvent[length] : throw new NegativeArraySizeException();
    }

    public virtual LoggingEvent get()
    {
      if (this.numElements == 0)
        return (LoggingEvent) null;
      LoggingEvent loggingEvent = this.buf[this.first];
      this.buf[this.first] = (LoggingEvent) null;
      if (++this.first == this.maxSize)
        this.first = 0;
      this.numElements += -1;
      return loggingEvent;
    }

    public virtual void put(LoggingEvent o)
    {
      if (this.numElements == this.maxSize)
        return;
      this.buf[this.next] = o;
      if (++this.next == this.maxSize)
        this.next = 0;
      ++this.numElements;
    }

    public virtual int getMaxSize() => this.maxSize;

    public virtual bool isFull() => this.numElements == this.maxSize;

    public virtual int length() => this.numElements;

    [JavaFlags(0)]
    public virtual int min(int a, int b) => a < b ? a : b;

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void resize(int newSize)
    {
      if (newSize == this.maxSize)
        return;
      int length = newSize;
      LoggingEvent[] loggingEventArray = length >= 0 ? new LoggingEvent[length] : throw new NegativeArraySizeException();
      int num1 = this.min(this.min(this.maxSize - this.first, newSize), this.numElements);
      java.lang.System.arraycopy((object) this.buf, this.first, (object) loggingEventArray, 0, num1);
      int num2 = 0;
      if (num1 < this.numElements && num1 < newSize)
      {
        num2 = this.min(this.numElements - num1, newSize - num1);
        java.lang.System.arraycopy((object) this.buf, 0, (object) loggingEventArray, num1, num2);
      }
      this.buf = loggingEventArray;
      this.maxSize = newSize;
      this.first = 0;
      this.numElements = num1 + num2;
      this.next = this.numElements;
      if (this.next != this.maxSize)
        return;
      this.next = 0;
    }

    public virtual bool wasEmpty() => this.numElements == 1;

    public virtual bool wasFull() => this.numElements + 1 == this.maxSize;

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      BoundedFIFO boundedFifo = this;
      ObjectImpl.clone((object) boundedFifo);
      return ((object) boundedFifo).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
