// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.valueholder.TimePeriod
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.application.valueholder
{
  public class TimePeriod : BusinessValueHolder
  {
    private readonly Time end;
    private readonly Time start;

    public TimePeriod()
    {
      this.end = new Time();
      this.start = new Time();
      this.start.setValue(43200L);
      this.end.setValue(46800L);
    }

    public TimePeriod(TimePeriod existing)
    {
      this.end = new Time();
      this.start = new Time();
      this.setValue(existing);
    }

    public override void clear()
    {
      this.start.clear();
      this.end.clear();
    }

    public override void copyObject(BusinessValueHolder @object)
    {
      TimePeriod timePeriod = @object is TimePeriod ? (TimePeriod) @object : throw new IllegalArgumentException("Can only copy the value of a TimePeriod object");
      if (timePeriod.isEmpty())
      {
        this.clear();
      }
      else
      {
        this.start.setValue(timePeriod.getStart());
        this.end.setValue(timePeriod.getEnd());
      }
    }

    public virtual bool endsAfter(TimePeriod arg) => this.end.isGreaterThan((Magnitude) arg.getEnd());

    public virtual bool entirelyContains(TimePeriod arg) => arg.getStart().isBetween((Magnitude) this.start, (Magnitude) this.end) && arg.getEnd().isBetween((Magnitude) this.start, (Magnitude) this.end);

    public virtual Time getEnd() => this.end;

    public virtual Time getStart() => this.start;

    public override bool isEmpty() => this.start.isEmpty() && this.end.isEmpty();

    public virtual bool isEqualTo(TimePeriod arg) => this.start.isEqualTo((Magnitude) arg.getStart()) && this.end.isEqualTo((Magnitude) arg.getEnd());

    public override bool isSameAs(BusinessValueHolder @object)
    {
      if (!(@object is TimePeriod))
        return false;
      TimePeriod timePeriod = (TimePeriod) @object;
      return this.start.isEqualTo((Magnitude) timePeriod.getStart()) && this.end.isEqualTo((Magnitude) timePeriod.getEnd());
    }

    public virtual TimePeriod leadDifference(TimePeriod arg)
    {
      TimePeriod timePeriod = new TimePeriod();
      if (this.startsBefore(arg))
      {
        timePeriod.getStart().setValue(this.start);
        timePeriod.getEnd().setValue(arg.getStart());
      }
      else
      {
        timePeriod.getStart().setValue(arg.getStart());
        timePeriod.getEnd().setValue(this.start);
      }
      return timePeriod;
    }

    public virtual TimePeriod overlap(TimePeriod arg)
    {
      TimePeriod timePeriod = new TimePeriod();
      timePeriod.clear();
      if (this.overlaps(arg))
      {
        if (arg.getStart().isGreaterThan((Magnitude) this.start))
          timePeriod.getStart().setValue(arg.getStart());
        else
          timePeriod.getStart().setValue(this.start);
        if (arg.getEnd().isLessThan((Magnitude) this.end))
          timePeriod.getEnd().setValue(arg.getEnd());
        else
          timePeriod.getEnd().setValue(this.end);
      }
      return timePeriod;
    }

    public virtual bool overlaps(TimePeriod arg) => this.end.isGreaterThan((Magnitude) arg.getStart()) && this.start.isLessThan((Magnitude) arg.getEnd());

    [JavaThrownExceptions("1;org/nakedobjects/application/ValueParseException;")]
    public override void parseUserEntry(string text)
    {
      if (StringImpl.equals(StringImpl.trim(text), (object) ""))
      {
        this.clear();
      }
      else
      {
        int num = StringImpl.indexOf(text, "~");
        if (num < 0)
          throw new ValueParseException("No tilde found", (Throwable) new Exception());
        this.start.parseUserEntry(StringImpl.trim(StringImpl.substring(text, 0, num)));
        this.end.parseUserEntry(StringImpl.trim(StringImpl.substring(text, num + 1)));
        if (this.end.isLessThan((Magnitude) this.start))
          throw new ValueParseException("End time before start time", (Throwable) new Exception());
      }
    }

    public virtual void reset()
    {
      this.start.reset();
      this.end.reset();
    }

    public override void restoreFromEncodedString(string data)
    {
      if (data == null || StringImpl.equals(data, (object) "NULL"))
      {
        this.clear();
      }
      else
      {
        this.start.restoreFromEncodedString(StringImpl.substring(data, 0, 3));
        this.end.restoreFromEncodedString(StringImpl.substring(data, 4, 7));
      }
    }

    public override string asEncodedString()
    {
      if (this.start.isEmpty() || this.end.isEmpty())
        return "NULL";
      StringBuffer stringBuffer = new StringBuffer(8);
      stringBuffer.append(this.start.asEncodedString());
      stringBuffer.append(this.end.asEncodedString());
      return stringBuffer.ToString();
    }

    public virtual void setValue(Time start, Time end)
    {
      this.start.setValue(start);
      this.end.setValue(end);
    }

    public virtual void setValue(TimePeriod t)
    {
      this.start.setValue(t.getStart());
      this.end.setValue(t.getEnd());
    }

    public virtual bool startsBefore(TimePeriod arg) => this.start.isLessThan((Magnitude) arg.getStart());

    public virtual TimePeriod tailDifference(TimePeriod arg)
    {
      TimePeriod timePeriod = new TimePeriod();
      if (this.endsAfter(arg))
      {
        timePeriod.getStart().setValue(arg.getEnd());
        timePeriod.getEnd().setValue(this.end);
      }
      else
      {
        timePeriod.getStart().setValue(this.end);
        timePeriod.getEnd().setValue(arg.getEnd());
      }
      return timePeriod;
    }

    public override Title title()
    {
      Title title = new Title(this.getStart() != null ? this.getStart().title().ToString() : "");
      title.append("~");
      title.append(this.getEnd() != null ? this.getEnd().title().ToString() : "");
      return title;
    }
  }
}
