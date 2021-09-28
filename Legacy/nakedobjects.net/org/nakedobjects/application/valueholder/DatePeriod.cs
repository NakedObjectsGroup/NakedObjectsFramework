// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.valueholder.DatePeriod
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.application.valueholder
{
  public class DatePeriod : BusinessValueHolder
  {
    private readonly Date end;
    private readonly Date start;

    public DatePeriod()
    {
      this.end = new Date();
      this.start = new Date();
      this.start.clear();
      this.end.clear();
    }

    public DatePeriod(DatePeriod period)
    {
      this.end = new Date();
      this.start = new Date();
      this.start.copyObject((BusinessValueHolder) period.start);
      this.end.copyObject((BusinessValueHolder) period.end);
    }

    public override void clear()
    {
      this.getStart().clear();
      this.getEnd().clear();
    }

    public virtual bool contains(Date d) => d.isGreaterThanOrEqualTo((Magnitude) this.start) && d.isLessThanOrEqualTo((Magnitude) this.end);

    public override void copyObject(BusinessValueHolder @object)
    {
      DatePeriod dp = @object is DatePeriod ? (DatePeriod) @object : throw new IllegalArgumentException();
      if (dp.isEmpty())
        this.clear();
      else
        this.setValue(dp);
    }

    public virtual bool endsAfter(DatePeriod arg) => this.getEnd().isGreaterThan((Magnitude) arg.getEnd());

    public virtual Date getEnd() => this.end;

    public virtual Date getStart() => this.start;

    public virtual bool isDatePeriod(BusinessValueHolder @object) => @object is DatePeriod;

    public override bool isEmpty() => this.getStart().isEmpty() && this.getEnd().isEmpty();

    public override bool isSameAs(BusinessValueHolder @object)
    {
      if (!(@object is DatePeriod))
        return false;
      DatePeriod datePeriod = (DatePeriod) @object;
      return this.getStart().isEqualTo((Magnitude) datePeriod.getStart()) && this.getEnd().isEqualTo((Magnitude) datePeriod.getEnd());
    }

    public virtual void leadDifference(DatePeriod arg)
    {
      if (!this.overlaps(arg))
        throw new IllegalArgumentException("No overlap");
      if (this.startsBefore(arg))
      {
        this.getEnd().setValue(arg.getStart());
        this.getEnd().add(0, 0, -1);
      }
      else
      {
        this.getEnd().setValue(this.getStart());
        this.getEnd().add(0, 0, -1);
        this.getStart().setValue(arg.getStart());
      }
    }

    public virtual void overlap(DatePeriod arg)
    {
      if (!this.overlaps(arg))
        throw new IllegalArgumentException("No overlap");
      if (arg.getStart().isGreaterThan((Magnitude) this.getStart()))
        this.getStart().setValue(arg.getStart());
      else
        this.getStart().setValue(this.getStart());
      if (arg.getEnd().isLessThan((Magnitude) this.getEnd()))
        this.getEnd().setValue(arg.getEnd());
      else
        this.getEnd().setValue(this.getEnd());
    }

    public virtual bool overlaps(DatePeriod arg) => this.getEnd().isGreaterThan((Magnitude) arg.getStart()) && this.start.isLessThan((Magnitude) arg.getEnd());

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
        if (num == -1)
          throw new ValueParseException("No tilde found");
        this.getStart().parseUserEntry(StringImpl.trim(StringImpl.substring(text, 0, num)));
        this.getEnd().parseUserEntry(StringImpl.trim(StringImpl.substring(text, num + 1)));
        if (this.getEnd().isLessThan((Magnitude) this.getStart()))
          throw new ValueParseException("End date must be before start date");
      }
    }

    public virtual void reset()
    {
      this.getStart().reset();
      this.getEnd().reset();
    }

    public override void restoreFromEncodedString(string data)
    {
      if (data == null || StringImpl.equals(data, (object) "NULL"))
      {
        this.clear();
      }
      else
      {
        int num = StringImpl.indexOf(data, 126);
        this.getStart().restoreFromEncodedString(StringImpl.substring(data, 0, num));
        this.getEnd().restoreFromEncodedString(StringImpl.substring(data, num + 1));
      }
    }

    public override string asEncodedString() => this.getStart().isEmpty() || this.getEnd().isEmpty() ? "NULL" : new StringBuffer().append(this.getStart().asEncodedString()).append("~").append(this.getEnd().asEncodedString()).ToString();

    public virtual void setValue(Date start, Date end)
    {
      this.getStart().setValue(start);
      this.getEnd().setValue(end);
    }

    public virtual void setValue(DatePeriod dp)
    {
      this.getStart().setValue(dp.getStart());
      this.getEnd().setValue(dp.getEnd());
    }

    public virtual bool startsBefore(DatePeriod arg) => this.getStart().isLessThan((Magnitude) arg.getStart());

    public virtual void tailDifference(DatePeriod arg)
    {
      if (!this.overlaps(arg))
        throw new IllegalArgumentException("No overlap");
      if (this.endsAfter(arg))
      {
        this.getStart().setValue(arg.getEnd());
        this.getStart().add(0, 0, 1);
      }
      else
      {
        this.getStart().setValue(this.getEnd());
        this.getStart().add(0, 0, 1);
        this.getEnd().setValue(arg.getEnd());
      }
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
