// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.helpers.PatternParser
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.text;
using java.util;
using org.apache.log4j.spi;
using System.ComponentModel;

namespace org.apache.log4j.helpers
{
  public class PatternParser
  {
    private const char ESCAPE_CHAR = '%';
    private const int LITERAL_STATE = 0;
    private const int CONVERTER_STATE = 1;
    private const int MINUS_STATE = 2;
    private const int DOT_STATE = 3;
    private const int MIN_STATE = 4;
    private const int MAX_STATE = 5;
    [JavaFlags(24)]
    public const int FULL_LOCATION_CONVERTER = 1000;
    [JavaFlags(24)]
    public const int METHOD_LOCATION_CONVERTER = 1001;
    [JavaFlags(24)]
    public const int CLASS_LOCATION_CONVERTER = 1002;
    [JavaFlags(24)]
    public const int LINE_LOCATION_CONVERTER = 1003;
    [JavaFlags(24)]
    public const int FILE_LOCATION_CONVERTER = 1004;
    [JavaFlags(24)]
    public const int RELATIVE_TIME_CONVERTER = 2000;
    [JavaFlags(24)]
    public const int THREAD_CONVERTER = 2001;
    [JavaFlags(24)]
    public const int LEVEL_CONVERTER = 2002;
    [JavaFlags(24)]
    public const int NDC_CONVERTER = 2003;
    [JavaFlags(24)]
    public const int MESSAGE_CONVERTER = 2004;
    [JavaFlags(0)]
    public int state;
    [JavaFlags(4)]
    public StringBuffer currentLiteral;
    [JavaFlags(4)]
    public int patternLength;
    [JavaFlags(4)]
    public int i;
    [JavaFlags(0)]
    public PatternConverter head;
    [JavaFlags(0)]
    public PatternConverter tail;
    [JavaFlags(4)]
    public FormattingInfo formattingInfo;
    [JavaFlags(4)]
    public string pattern;

    public PatternParser(string pattern)
    {
      this.currentLiteral = new StringBuffer(32);
      this.formattingInfo = new FormattingInfo();
      this.pattern = pattern;
      this.patternLength = StringImpl.length(pattern);
      this.state = 0;
    }

    private void addToList(PatternConverter pc)
    {
      if (this.head == null)
      {
        this.head = this.tail = pc;
      }
      else
      {
        this.tail.next = pc;
        this.tail = pc;
      }
    }

    [JavaFlags(4)]
    public virtual string extractOption()
    {
      if (this.i < this.patternLength && StringImpl.charAt(this.pattern, this.i) == '{')
      {
        int num = StringImpl.indexOf(this.pattern, 125, this.i);
        if (num > this.i)
        {
          string str = StringImpl.substring(this.pattern, this.i + 1, num);
          this.i = num + 1;
          return str;
        }
      }
      return (string) null;
    }

    [JavaFlags(4)]
    public virtual int extractPrecisionOption()
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual PatternConverter parse()
    {
      this.i = 0;
      while (this.i < this.patternLength)
      {
        string pattern = this.pattern;
        int i;
        this.i = (i = this.i) + 1;
        int num = i;
        char c = StringImpl.charAt(pattern, num);
        switch (this.state)
        {
          case 0:
            if (this.i == this.patternLength)
            {
              this.currentLiteral.append(c);
              continue;
            }
            if (c == '%')
            {
              switch (StringImpl.charAt(this.pattern, this.i))
              {
                case '%':
                  this.currentLiteral.append(c);
                  ++this.i;
                  continue;
                case 'n':
                  this.currentLiteral.append(Layout.LINE_SEP);
                  ++this.i;
                  continue;
                default:
                  if (this.currentLiteral.length() != 0)
                    this.addToList((PatternConverter) new PatternParser.LiteralPatternConverter(this.currentLiteral.ToString()));
                  this.currentLiteral.setLength(0);
                  this.currentLiteral.append(c);
                  this.state = 1;
                  this.formattingInfo.reset();
                  continue;
              }
            }
            else
            {
              this.currentLiteral.append(c);
              continue;
            }
          case 1:
            this.currentLiteral.append(c);
            switch (c)
            {
              case '-':
                this.formattingInfo.leftAlign = true;
                continue;
              case '.':
                this.state = 3;
                continue;
              default:
                if (c >= '0' && c <= '9')
                {
                  this.formattingInfo.min = (int) c - 48;
                  this.state = 4;
                  continue;
                }
                this.finalizeConverter(c);
                continue;
            }
          case 3:
            this.currentLiteral.append(c);
            if (c >= '0' && c <= '9')
            {
              this.formattingInfo.max = (int) c - 48;
              this.state = 5;
              continue;
            }
            LogLog.error(new StringBuffer().append("Error occured in position ").append(this.i).append(".\n Was expecting digit, instead got char \"").append(c).append("\".").ToString());
            this.state = 0;
            continue;
          case 4:
            this.currentLiteral.append(c);
            if (c >= '0' && c <= '9')
            {
              this.formattingInfo.min = this.formattingInfo.min * 10 + ((int) c - 48);
              continue;
            }
            if (c == '.')
            {
              this.state = 3;
              continue;
            }
            this.finalizeConverter(c);
            continue;
          case 5:
            this.currentLiteral.append(c);
            if (c >= '0' && c <= '9')
            {
              this.formattingInfo.max = this.formattingInfo.max * 10 + ((int) c - 48);
              continue;
            }
            this.finalizeConverter(c);
            this.state = 0;
            continue;
          default:
            continue;
        }
      }
      if (this.currentLiteral.length() != 0)
        this.addToList((PatternConverter) new PatternParser.LiteralPatternConverter(this.currentLiteral.ToString()));
      return this.head;
    }

    [JavaFlags(4)]
    public virtual void finalizeConverter(char c)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4)]
    public virtual void addConverter(PatternConverter pc)
    {
      this.currentLiteral.setLength(0);
      this.addToList(pc);
      this.state = 0;
      this.formattingInfo.reset();
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      PatternParser patternParser = this;
      ObjectImpl.clone((object) patternParser);
      return ((object) patternParser).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [JavaFlags(42)]
    private class BasicPatternConverter : PatternConverter
    {
      [JavaFlags(0)]
      public int type;

      [JavaFlags(0)]
      public BasicPatternConverter(FormattingInfo formattingInfo, int type)
        : base(formattingInfo)
      {
        this.type = type;
      }

      public override string convert(LoggingEvent @event)
      {
        switch (this.type)
        {
          case 2000:
            return Long.toString(@event.timeStamp - LoggingEvent.getStartTime());
          case 2001:
            return @event.getThreadName();
          case 2002:
            return @event.getLevel().ToString();
          case 2003:
            return @event.getNDC();
          case 2004:
            return @event.getRenderedMessage();
          default:
            return (string) null;
        }
      }
    }

    [JavaFlags(42)]
    private class LiteralPatternConverter : PatternConverter
    {
      private string literal;

      [JavaFlags(0)]
      public LiteralPatternConverter(string value) => this.literal = value;

      [JavaFlags(17)]
      public override sealed void format(StringBuffer sbuf, LoggingEvent @event) => sbuf.append(this.literal);

      public override string convert(LoggingEvent @event) => this.literal;
    }

    [JavaFlags(42)]
    private class DatePatternConverter : PatternConverter
    {
      private DateFormat df;
      private Date date;

      [JavaFlags(0)]
      public DatePatternConverter(FormattingInfo formattingInfo, DateFormat df)
        : base(formattingInfo)
      {
        this.date = new Date();
        this.df = df;
      }

      public override string convert(LoggingEvent @event)
      {
        // ISSUE: unable to decompile the method.
      }
    }

    [JavaFlags(42)]
    private class MDCPatternConverter : PatternConverter
    {
      private string key;

      [JavaFlags(0)]
      public MDCPatternConverter(FormattingInfo formattingInfo, string key)
        : base(formattingInfo)
      {
        this.key = key;
      }

      public override string convert(LoggingEvent @event) => (string) null;
    }

    [JavaFlags(34)]
    [Inner]
    private class LocationPatternConverter : PatternConverter
    {
      [JavaFlags(0)]
      public int type;
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private PatternParser this\u00240;

      [JavaFlags(0)]
      public LocationPatternConverter(
        PatternParser _param1,
        FormattingInfo formattingInfo,
        int type)
        : base(formattingInfo)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.type = type;
      }

      public override string convert(LoggingEvent @event)
      {
        LocationInfo locationInformation = @event.getLocationInformation();
        switch (this.type)
        {
          case 1000:
            return locationInformation.fullInfo;
          case 1001:
            return locationInformation.getMethodName();
          case 1003:
            return locationInformation.getLineNumber();
          case 1004:
            return locationInformation.getFileName();
          default:
            return (string) null;
        }
      }
    }

    [JavaFlags(1066)]
    private abstract class NamedPatternConverter : PatternConverter
    {
      [JavaFlags(0)]
      public int precision;

      [JavaFlags(0)]
      public NamedPatternConverter(FormattingInfo formattingInfo, int precision)
        : base(formattingInfo)
      {
        this.precision = precision;
      }

      [JavaFlags(1024)]
      public abstract string getFullyQualifiedName(LoggingEvent @event);

      public override string convert(LoggingEvent @event)
      {
        string fullyQualifiedName = this.getFullyQualifiedName(@event);
        if (this.precision <= 0)
          return fullyQualifiedName;
        int num1 = StringImpl.length(fullyQualifiedName);
        int num2 = num1 - 1;
        for (int precision = this.precision; precision > 0; precision += -1)
        {
          num2 = StringImpl.lastIndexOf(fullyQualifiedName, 46, num2 - 1);
          if (num2 == -1)
            return fullyQualifiedName;
        }
        return StringImpl.substring(fullyQualifiedName, num2 + 1, num1);
      }
    }

    [JavaFlags(34)]
    [Inner]
    private class ClassNamePatternConverter : PatternParser.NamedPatternConverter
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private PatternParser this\u00240;

      [JavaFlags(0)]
      public ClassNamePatternConverter(
        PatternParser _param1,
        FormattingInfo formattingInfo,
        int precision)
        : base(formattingInfo, precision)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }

      [JavaFlags(0)]
      public override string getFullyQualifiedName(LoggingEvent @event) => @event.getLocationInformation().getClassName();
    }

    [JavaFlags(34)]
    [Inner]
    private class CategoryPatternConverter : PatternParser.NamedPatternConverter
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private PatternParser this\u00240;

      [JavaFlags(0)]
      public CategoryPatternConverter(
        PatternParser _param1,
        FormattingInfo formattingInfo,
        int precision)
        : base(formattingInfo, precision)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }

      [JavaFlags(0)]
      public override string getFullyQualifiedName(LoggingEvent @event) => @event.getLoggerName();
    }
  }
}
