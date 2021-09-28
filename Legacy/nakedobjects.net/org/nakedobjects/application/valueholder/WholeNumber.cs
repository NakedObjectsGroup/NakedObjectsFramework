// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.valueholder.WholeNumber
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using com.ms.vjsharp.util;
using java.lang;
using java.text;
using org.apache.log4j;
using System;
using System.ComponentModel;

namespace org.nakedobjects.application.valueholder
{
  public class WholeNumber : Magnitude
  {
    private static readonly NumberFormat FORMAT;
    private int whole;
    private bool isNull;
    private static readonly Logger logger;

    public WholeNumber() => this.clear();

    public WholeNumber(int whole) => this.setValue(whole);

    [Obsolete(null, false)]
    public WholeNumber(string text)
    {
      try
      {
        this.parseUserEntry(text);
      }
      catch (ValueParseException ex)
      {
      }
    }

    public WholeNumber(WholeNumber wholeNumber) => this.setValue(wholeNumber);

    public virtual void add(int whole) => this.setValue(this.whole + whole);

    public virtual void add(WholeNumber whole) => this.add(whole.whole);

    public override void clear() => this.isNull = true;

    public virtual int compareTo(int value) => this.whole - value;

    public override void copyObject(BusinessValueHolder @object)
    {
      if (!(@object is WholeNumber))
        throw new IllegalArgumentException("Can only copy the value of  a WholeNumber object");
      this.setValue((WholeNumber) @object);
    }

    public virtual void divide(int whole) => this.setValue(this.whole / whole);

    public virtual void divide(double whole) => this.setValue(Utilities.doubleToInt((double) this.whole / whole));

    public virtual void divide(WholeNumber number) => this.divide(number.whole);

    public virtual double doubleValue() => (double) this.whole;

    public override bool Equals(object @object)
    {
      if (!(@object is WholeNumber))
        return base.Equals(@object);
      return ((WholeNumber) @object).whole == this.whole;
    }

    public virtual float floatValue() => (float) this.whole;

    [Obsolete(null, false)]
    public virtual int getInt() => this.whole;

    public virtual string getObjectHelpText() => "A Whole Number object.";

    public virtual int intValue() => this.whole;

    public override bool isEmpty() => this.isNull;

    public override bool isEqualTo(Magnitude number)
    {
      if (!(number is WholeNumber))
        throw new IllegalArgumentException("Parameter must be of type WholeNumber");
      if (this.isNull)
        return number.isEmpty();
      return ((WholeNumber) number).whole == this.whole;
    }

    public override bool isLessThan(Magnitude value)
    {
      if (!(value is WholeNumber))
        throw new IllegalArgumentException("Parameter must be of type WholeNumber");
      return !this.isNull && !value.isEmpty() && this.whole < ((WholeNumber) value).whole;
    }

    public virtual bool isNegative() => this.whole < 0;

    public virtual bool isZero() => this.whole == 0;

    public virtual long longValue() => (long) this.whole;

    public virtual void multiply(int whole) => this.setValue(this.whole * whole);

    public virtual void multiply(WholeNumber number) => this.multiply(number.whole);

    [JavaThrownExceptions("1;org/nakedobjects/application/ValueParseException;")]
    public override void parseUserEntry(string text)
    {
      if (StringImpl.equals(StringImpl.trim(text), (object) ""))
      {
        this.clear();
      }
      else
      {
        try
        {
          this.setValue(WholeNumber.FORMAT.parse(text).intValue());
        }
        catch (ParseException ex)
        {
          throw new ValueParseException("Invalid number", (Throwable) ex);
        }
      }
    }

    public virtual void reset() => this.setValue(0);

    public virtual void set(int whole) => this.setValue(whole);

    [Obsolete(null, false)]
    public virtual void set(WholeNumber value) => this.setValue(value.whole);

    [Obsolete(null, false)]
    public virtual void setInt(int whole) => this.setValue(whole);

    public virtual void setValue(int whole)
    {
      this.whole = whole;
      this.isNull = false;
    }

    public virtual void setValue(WholeNumber value)
    {
      if (value.isEmpty())
        this.clear();
      else
        this.setValue(value.whole);
    }

    public virtual short shortValue() => (short) this.whole;

    public virtual void subtract(int whole) => this.add(-whole);

    public virtual void subtract(WholeNumber number) => this.subtract(number.whole);

    public virtual void abs() => this.whole = Math.abs(this.whole);

    public override Title title() => new Title(!this.isNull ? WholeNumber.FORMAT.format((long) this.whole) : "");

    public override void restoreFromEncodedString(string data)
    {
      if (data == null || StringImpl.equals(data, (object) "NULL"))
        this.clear();
      else
        this.setValue(Integer.valueOf(data).intValue());
    }

    public override string asEncodedString() => this.isEmpty() ? (string) null : StringImpl.valueOf(this.intValue());

    [JavaFlags(4)]
    public virtual Logger getLogger() => WholeNumber.logger;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static WholeNumber()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
