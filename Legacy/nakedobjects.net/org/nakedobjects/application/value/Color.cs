// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.value.Color
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.application.value
{
  public class Color : Magnitude
  {
    private int color;

    public Color()
      : this(0)
    {
    }

    public Color(int color) => this.color = color;

    public virtual string asEncodedString() => StringImpl.valueOf(this.intValue());

    public virtual int intValue() => this.color;

    public override bool isEqualTo(Magnitude number)
    {
      if (!(number is Color))
        throw new IllegalArgumentException("Parameter must be of type Color");
      return ((Color) number).color == this.color;
    }

    public override bool isLessThan(Magnitude value)
    {
      if (!(value is Color))
        throw new IllegalArgumentException("Parameter must be of type Color");
      return this.color < ((Color) value).color;
    }

    [JavaThrownExceptions("1;org/nakedobjects/application/ValueParseException;")]
    public virtual void parseUserEntry(string text)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual void restoreFromEncodedString(string data)
    {
      if (data == null || StringImpl.equals(data, (object) "NULL"))
        throw new ApplicationException();
      this.setValue(Integer.valueOf(data).intValue());
    }

    public virtual void setValue(Color value) => this.setValue(value.color);

    public virtual void setValue(int color) => this.color = color;

    public virtual Title title()
    {
      if (this.color == 0)
        return new Title("Black");
      return this.color == 16777215 ? new Title("White") : new Title(new StringBuffer().append("0x").append(Integer.toHexString(this.color)).ToString());
    }
  }
}
