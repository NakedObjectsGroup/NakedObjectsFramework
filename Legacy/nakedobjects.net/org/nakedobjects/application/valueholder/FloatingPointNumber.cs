// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.valueholder.FloatingPointNumber
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using com.ms.vjsharp.util;
using java.lang;
using java.text;
using System.ComponentModel;

namespace org.nakedobjects.application.valueholder
{
  public class FloatingPointNumber : Magnitude
  {
    private static NumberFormat FORMAT;
    private const long serialVersionUID = 1;
    private bool isNull;
    private double value;

    public FloatingPointNumber()
      : this(0.0)
    {
      this.isNull = false;
    }

    public FloatingPointNumber(double value)
    {
      this.value = value;
      this.isNull = false;
    }

    public FloatingPointNumber(FloatingPointNumber value)
    {
      this.isNull = value.isNull;
      this.value = value.value;
    }

    public virtual void add(double value) => this.value += value;

    public virtual void add(FloatingPointNumber number) => this.value += number.value;

    public override void clear() => this.isNull = true;

    public override void copyObject(BusinessValueHolder @object)
    {
      this.isNull = @object is FloatingPointNumber ? ((FloatingPointNumber) @object).isNull : throw new IllegalArgumentException("Can only copy the value of  a FloatingPointNumber object");
      this.value = ((FloatingPointNumber) @object).value;
    }

    public virtual void divide(double value) => this.value /= value;

    public virtual void divide(FloatingPointNumber number) => this.value /= number.value;

    public virtual double doubleValue() => this.value;

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      if (!(obj is FloatingPointNumber))
        return false;
      FloatingPointNumber floatingPointNumber = (FloatingPointNumber) obj;
      return floatingPointNumber.isEmpty() && this.isEmpty() || floatingPointNumber.value == this.value;
    }

    public virtual float floatValue() => (float) this.value;

    public virtual string getObjectHelpText() => "A floating point number object.";

    public virtual int intValue() => Utilities.doubleToInt(this.value);

    public override bool isEmpty() => this.isNull;

    public override bool isEqualTo(Magnitude magnitude)
    {
      if (!(magnitude is FloatingPointNumber))
        throw new IllegalArgumentException("Parameter must be of type FloatingPointNumber");
      if (this.isNull)
        return magnitude.isEmpty();
      return ((FloatingPointNumber) magnitude).value == this.value;
    }

    public override bool isLessThan(Magnitude magnitude)
    {
      if (!(magnitude is FloatingPointNumber))
        throw new IllegalArgumentException("Parameter must be of type FloatingPointNumber");
      return !this.isEmpty() && !magnitude.isEmpty() && this.value < ((FloatingPointNumber) magnitude).value;
    }

    public virtual long longValue() => Utilities.doubleToLong(this.value);

    public virtual void multiply(double value) => this.value *= value;

    public virtual void multiply(FloatingPointNumber number) => this.value *= number.value;

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
          this.setValue(FloatingPointNumber.FORMAT.parse(text).doubleValue());
        }
        catch (ParseException ex)
        {
          throw new ValueParseException("Invalid number", (Throwable) ex);
        }
      }
    }

    public virtual void reset()
    {
      this.value = 0.0;
      this.isNull = false;
    }

    public override void restoreFromEncodedString(string data)
    {
      if (data == null || StringImpl.equals(data, (object) "NULL"))
        this.clear();
      else
        this.setValue(Double.valueOf(data).doubleValue());
    }

    public override string asEncodedString() => this.isNull ? "NULL" : StringImpl.valueOf(this.doubleValue());

    public virtual void setValue(double value)
    {
      this.value = value;
      this.isNull = false;
    }

    public virtual void setValue(FloatingPointNumber value)
    {
      this.value = value.value;
      this.isNull = value.isNull;
    }

    public virtual short shortValue() => (short) Utilities.doubleToInt(this.value);

    public virtual void subtract(double value) => this.add(-value);

    public virtual void subtract(FloatingPointNumber number) => this.add(-number.value);

    public override Title title() => new Title(!this.isEmpty() ? FloatingPointNumber.FORMAT.format(this.value) : "");

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static FloatingPointNumber()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
