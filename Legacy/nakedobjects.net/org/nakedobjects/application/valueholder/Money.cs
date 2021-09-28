// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.valueholder.Money
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using com.ms.vjsharp.util;
using java.lang;
using java.text;
using org.apache.log4j;
using System.ComponentModel;

namespace org.nakedobjects.application.valueholder
{
  public class Money : Magnitude
  {
    private const long serialVersionUID = 1;
    private static readonly NumberFormat CURRENCY_FORMAT;
    private static readonly NumberFormat NUMBER_FORMAT;
    private bool isNull;
    private double amount;
    private static readonly Logger logger;

    public Money() => this.setValue(0.0);

    public Money(Money money) => this.setValue(money);

    public Money(double amount) => this.setValue(amount);

    public virtual void add(Money money) => this.setValue(this.amount + money.amount);

    public override void clear() => this.isNull = true;

    public override void copyObject(BusinessValueHolder @object)
    {
      if (!(@object is Money))
        throw new IllegalArgumentException("Can only copy the value of a Money object");
      this.setValue((Money) @object);
    }

    public virtual void divideBy(double operand) => this.setValue(this.amount / operand);

    public virtual double doubleValue() => this.amount;

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      if (!(obj is Money))
        return false;
      Money money = (Money) obj;
      return money.isEmpty() && this.isEmpty() || money.amount == this.amount;
    }

    public virtual float floatValue() => (float) this.amount;

    public virtual string getObjectHelpText() => "A Money object stored as dollars/cents, pounds/pence, euro/cents.";

    public virtual int intValue() => Utilities.doubleToInt(this.amount);

    public override bool isEmpty() => this.isNull;

    public override bool isEqualTo(Magnitude magnitude)
    {
      if (!(magnitude is Money))
        throw new IllegalArgumentException("Parameter must be of type Money");
      if (this.isNull)
        return magnitude.isEmpty();
      return ((Money) magnitude).amount == this.amount;
    }

    public override bool isLessThan(Magnitude magnitude)
    {
      if (!(magnitude is Money))
        throw new IllegalArgumentException("Parameter must be of type Money");
      return !this.isEmpty() && !magnitude.isEmpty() && this.amount < ((Money) magnitude).amount;
    }

    public virtual bool isNegative() => this.amount < 0.0;

    public virtual long longValue() => Utilities.doubleToLong(this.amount);

    public virtual void multiplyBy(double operand) => this.setValue(this.amount * operand);

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
          this.setValue(Money.CURRENCY_FORMAT.parse(text).doubleValue());
        }
        catch (ParseException ex1)
        {
          try
          {
            this.setValue(Money.NUMBER_FORMAT.parse(text).doubleValue());
          }
          catch (ParseException ex2)
          {
            throw new ValueParseException("Invalid number", (Throwable) ex2);
          }
        }
      }
    }

    public virtual void reset() => this.setValue(0.0);

    public virtual void setValue(double amount)
    {
      this.amount = amount;
      this.isNull = false;
    }

    public virtual void setValue(Money value)
    {
      if (value.isEmpty())
        this.clear();
      else
        this.setValue(value.amount);
    }

    public virtual short shortValue() => (short) Utilities.doubleToInt(this.amount);

    public virtual void subtract(Money money) => this.setValue(this.amount - money.amount);

    public override Title title() => new Title(!this.isEmpty() ? Money.CURRENCY_FORMAT.format(this.amount) : "");

    public override void restoreFromEncodedString(string data)
    {
      if (data == null || StringImpl.equals(data, (object) "NULL"))
        this.clear();
      else
        this.setValue(Double.valueOf(data).doubleValue());
    }

    public override string asEncodedString() => this.isEmpty() ? "NULL" : StringImpl.valueOf(this.doubleValue());

    public virtual Logger getLogger() => Money.logger;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static Money()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
