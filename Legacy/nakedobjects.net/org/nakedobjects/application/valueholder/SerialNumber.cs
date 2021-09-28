// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.valueholder.SerialNumber
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.text;
using System.ComponentModel;

namespace org.nakedobjects.application.valueholder
{
  public class SerialNumber : Magnitude
  {
    private static readonly NumberFormat FORMAT;
    private bool isNull;
    private long number;

    public override void clear() => this.isNull = true;

    public override void copyObject(BusinessValueHolder @object)
    {
      if (!(@object is SerialNumber))
        throw new IllegalArgumentException("Can only copy the value of  a WholeNumber object");
      this.setValue((SerialNumber) @object);
    }

    public override bool isEmpty() => this.isNull;

    public override bool isEqualTo(Magnitude value)
    {
      if (!(value is SerialNumber))
        throw new IllegalArgumentException("Parameter must be of type WholeNumber");
      if (this.isNull)
        return value.isEmpty();
      return ((SerialNumber) value).number == this.number;
    }

    public override bool isLessThan(Magnitude value)
    {
      if (!(value is SerialNumber))
        throw new IllegalArgumentException("Parameter must be of type WholeNumber");
      return !this.isNull && !value.isEmpty() && this.number < ((SerialNumber) value).number;
    }

    public virtual long longValue() => this.number;

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
          this.setValue(SerialNumber.FORMAT.parse(text).intValue());
        }
        catch (ParseException ex)
        {
          throw new ValueParseException("Invalid number", (Throwable) ex);
        }
      }
    }

    public virtual void reset() => this.setValue(0);

    public override void restoreFromEncodedString(string data)
    {
      if (data == null || StringImpl.equals(data, (object) "NULL"))
        this.clear();
      else
        this.setValue(Integer.valueOf(data).intValue());
    }

    public override string asEncodedString() => this.isEmpty() ? "NULL" : StringImpl.valueOf(this.longValue());

    public virtual void setValue(int number)
    {
      this.number = (long) number;
      this.isNull = false;
    }

    public virtual void setValue(SerialNumber number)
    {
      this.number = number.number;
      this.isNull = false;
    }

    public override Title title() => new Title(!this.isNull ? StringImpl.valueOf(this.number) : "");

    public virtual void next() => ++this.number;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static SerialNumber()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
