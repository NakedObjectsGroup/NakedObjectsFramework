// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.valueholder.Color
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.application.valueholder
{
  public class Color : Magnitude
  {
    private int color;
    private bool isNull;

    public Color() => this.clear();

    public Color(int color) => this.color = color;

    public override void clear()
    {
      this.color = 0;
      this.isNull = true;
    }

    public override void copyObject(BusinessValueHolder @object)
    {
      if (!(@object is Color))
        throw new IllegalArgumentException("Can only copy the value of  a WholeNumber object");
      this.setValue((Color) @object);
    }

    public virtual int intValue() => this.color;

    public override bool isEmpty() => this.isNull;

    public override bool isEqualTo(Magnitude number)
    {
      if (!(number is Color))
        throw new IllegalArgumentException("Parameter must be of type Color");
      if (this.isNull)
        return number.isEmpty();
      return ((Color) number).color == this.color;
    }

    public override bool isLessThan(Magnitude value)
    {
      if (!(value is Color))
        throw new IllegalArgumentException("Parameter must be of type Color");
      return !this.isNull && !value.isEmpty() && this.color < ((Color) value).color;
    }

    [JavaThrownExceptions("1;org/nakedobjects/application/ValueParseException;")]
    public override void parseUserEntry(string text)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual void reset()
    {
      this.color = 0;
      this.isNull = false;
    }

    public override void restoreFromEncodedString(string data)
    {
      if (data == null || StringImpl.equals(data, (object) "NULL"))
        this.clear();
      else
        this.setValue(Integer.valueOf(data).intValue());
    }

    public override string asEncodedString() => this.isEmpty() ? "NULL" : StringImpl.valueOf(this.intValue());

    public virtual void setValue(Color value)
    {
      if (value.isEmpty())
        this.clear();
      else
        this.setValue(value.color);
    }

    public virtual void setValue(int color)
    {
      this.color = color;
      this.isNull = false;
    }

    public override Title title()
    {
      if (this.color == 0)
        return new Title("Black");
      return this.color == 16777215 ? new Title("White") : new Title(new StringBuffer().append("0x").append(Integer.toHexString(this.color)).ToString());
    }
  }
}
