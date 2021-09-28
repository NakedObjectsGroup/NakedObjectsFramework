// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.PatternLayout
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.apache.log4j.helpers;
using org.apache.log4j.spi;

namespace org.apache.log4j
{
  public class PatternLayout : Layout
  {
    public const string DEFAULT_CONVERSION_PATTERN = "%m%n";
    public const string TTCC_CONVERSION_PATTERN = "%r [%t] %p %c %x - %m%n";
    [JavaFlags(20)]
    public readonly int BUF_SIZE;
    [JavaFlags(20)]
    public readonly int MAX_CAPACITY;
    private StringBuffer sbuf;
    private string pattern;
    private PatternConverter head;
    private string timezone;

    public PatternLayout()
      : this("%m%n")
    {
    }

    public PatternLayout(string pattern)
    {
      this.BUF_SIZE = 256;
      this.MAX_CAPACITY = 1024;
      this.sbuf = new StringBuffer(256);
      this.pattern = pattern;
      this.head = this.createPatternParser(pattern != null ? pattern : "%m%n").parse();
    }

    public virtual void setConversionPattern(string conversionPattern)
    {
      this.pattern = conversionPattern;
      this.head = this.createPatternParser(conversionPattern).parse();
    }

    public virtual string getConversionPattern() => this.pattern;

    public override void activateOptions()
    {
    }

    public override bool ignoresThrowable() => true;

    [JavaFlags(4)]
    public virtual PatternParser createPatternParser(string pattern) => new PatternParser(pattern);

    public override string format(LoggingEvent @event)
    {
      if (this.sbuf.capacity() > 1024)
        this.sbuf = new StringBuffer(256);
      else
        this.sbuf.setLength(0);
      for (PatternConverter patternConverter = this.head; patternConverter != null; patternConverter = patternConverter.next)
        patternConverter.format(this.sbuf, @event);
      return this.sbuf.ToString();
    }
  }
}
