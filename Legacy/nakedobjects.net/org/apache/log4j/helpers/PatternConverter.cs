// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.helpers.PatternConverter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.apache.log4j.spi;
using System.ComponentModel;

namespace org.apache.log4j.helpers
{
  public abstract class PatternConverter
  {
    public PatternConverter next;
    [JavaFlags(0)]
    public int min;
    [JavaFlags(0)]
    public int max;
    [JavaFlags(0)]
    public bool leftAlign;
    [JavaFlags(8)]
    public static string[] SPACES;

    [JavaFlags(4)]
    public PatternConverter()
    {
      this.min = -1;
      this.max = int.MaxValue;
      this.leftAlign = false;
    }

    [JavaFlags(4)]
    public PatternConverter(FormattingInfo fi)
    {
      this.min = -1;
      this.max = int.MaxValue;
      this.leftAlign = false;
      this.min = fi.min;
      this.max = fi.max;
      this.leftAlign = fi.leftAlign;
    }

    [JavaFlags(1028)]
    public abstract string convert(LoggingEvent @event);

    public virtual void format(StringBuffer sbuf, LoggingEvent e)
    {
      string str = this.convert(e);
      if (str == null)
      {
        if (this.min <= 0)
          return;
        this.spacePad(sbuf, this.min);
      }
      else
      {
        int num = StringImpl.length(str);
        if (num > this.max)
          sbuf.append(StringImpl.substring(str, num - this.max));
        else if (num < this.min)
        {
          if (this.leftAlign)
          {
            sbuf.append(str);
            this.spacePad(sbuf, this.min - num);
          }
          else
          {
            this.spacePad(sbuf, this.min - num);
            sbuf.append(str);
          }
        }
        else
          sbuf.append(str);
      }
    }

    public virtual void spacePad(StringBuffer sbuf, int length)
    {
      for (; length >= 32; length -= 32)
        sbuf.append(PatternConverter.SPACES[5]);
      for (int index = 4; index >= 0; index += -1)
      {
        if ((length & 1 << index) != 0)
          sbuf.append(PatternConverter.SPACES[index]);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static PatternConverter()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      PatternConverter patternConverter = this;
      ObjectImpl.clone((object) patternConverter);
      return ((object) patternConverter).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
