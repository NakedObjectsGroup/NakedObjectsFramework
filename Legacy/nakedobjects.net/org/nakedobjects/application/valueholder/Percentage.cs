// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.valueholder.Percentage
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using com.ms.vjsharp.util;
using java.lang;
using java.text;
using System;
using System.ComponentModel;

namespace org.nakedobjects.application.valueholder
{
  public class Percentage : Magnitude
  {
    private const long serialVersionUID = 1;
    private static readonly NumberFormat PERCENTAGE_FORMAT;
    private static readonly NumberFormat DECIMAL_FORMAT;
    private float value;
    private bool isNull;

    public Percentage()
      : this(0.0f)
    {
      this.isNull = false;
    }

    public Percentage(float value)
    {
      this.value = value;
      this.isNull = false;
    }

    [Obsolete(null, false)]
    public Percentage(string text)
    {
      try
      {
        this.parseUserEntry(text);
        this.isNull = false;
      }
      catch (ValueParseException ex)
      {
        throw new IllegalArgumentException(new StringBuffer().append("Could not parse value: ").append(text).ToString());
      }
    }

    public Percentage(Percentage value)
    {
      this.isNull = value.isNull;
      this.value = value.value;
    }

    public virtual void add(double value) => this.value += (float) value;

    public override void clear() => this.isNull = true;

    public override void copyObject(BusinessValueHolder @object)
    {
      this.isNull = @object is Percentage ? ((Percentage) @object).isNull : throw new IllegalArgumentException("Can only copy the value of  a Percentage object");
      this.value = ((Percentage) @object).value;
    }

    public virtual void divide(double value) => this.value /= (float) value;

    public virtual double doubleValue() => (double) this.value;

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      if (!(obj is Percentage))
        return false;
      Percentage percentage = (Percentage) obj;
      return percentage.isEmpty() && this.isEmpty() || (double) percentage.value == (double) this.value;
    }

    public virtual float floatValue() => this.value;

    public virtual string getObjectHelpText() => "A floating point number object.";

    public virtual int intValue() => Utilities.floatToInt(this.value);

    public override bool isEmpty() => this.isNull;

    public override bool isEqualTo(Magnitude magnitude)
    {
      if (!(magnitude is Percentage))
        throw new IllegalArgumentException("Parameter must be of type WholeNumber");
      if (this.isNull)
        return magnitude.isEmpty();
      return (double) ((Percentage) magnitude).value == (double) this.value;
    }

    public override bool isLessThan(Magnitude magnitude)
    {
      if (!(magnitude is Percentage))
        throw new IllegalArgumentException("Parameter must be of type WholeNumber");
      return !this.isEmpty() && !magnitude.isEmpty() && (double) this.value < (double) ((Percentage) magnitude).value;
    }

    public virtual long longValue() => Utilities.floatToLong(this.value);

    public virtual void multiply(double value) => this.value *= (float) value;

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
          this.value = Percentage.PERCENTAGE_FORMAT.parse(text).floatValue();
        }
        catch (ParseException ex1)
        {
          try
          {
            this.value = Percentage.DECIMAL_FORMAT.parse(text).floatValue();
          }
          catch (ParseException ex2)
          {
            throw new ValueParseException(new StringBuffer().append("Invalid number; can;t parse '").append(text).append("'").ToString(), (Throwable) ex2);
          }
        }
      }
    }

    public virtual void reset()
    {
      this.value = 0.0f;
      this.isNull = false;
    }

    public virtual void setValue(float value)
    {
      this.value = value;
      this.isNull = false;
    }

    public virtual void setValue(Percentage value)
    {
      this.value = value.value;
      this.isNull = value.isNull;
    }

    public virtual short shortValue() => (short) Utilities.floatToInt(this.value);

    public virtual void subtract(double value) => this.add(-value);

    public override Title title() => new Title(!this.isEmpty() ? Percentage.PERCENTAGE_FORMAT.format((double) this.value) : "");

    public override void restoreFromEncodedString(string data)
    {
      if (data == null || StringImpl.equals(data, (object) "NULL"))
        this.clear();
      else
        this.setValue(Float.valueOf(data).floatValue());
    }

    public override string asEncodedString() => this.isEmpty() ? (string) null : StringImpl.valueOf(this.floatValue());

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static Percentage()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
