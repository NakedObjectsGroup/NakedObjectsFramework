// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.valueholder.ColorValueObjectAdapter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object.value;
using org.nakedobjects.@object.value.adapter;
using org.nakedobjects.application.valueholder;

namespace org.nakedobjects.reflector.java.valueholder
{
  [JavaInterfaces("1;org/nakedobjects/object/value/ColorValue;")]
  public class ColorValueObjectAdapter : AbstractNakedValue, ColorValue
  {
    private readonly Color adaptee;

    public ColorValueObjectAdapter(Color adaptee) => this.adaptee = adaptee;

    public virtual int color() => this.adaptee.intValue();

    public virtual void setColor(int color) => this.adaptee.setValue(color);

    public override string getIconName() => "boolean";

    public override string ToString() => new StringBuffer().append("POJO ColorAdapter: #").append(StringImpl.toUpperCase(Integer.toHexString(this.color()))).ToString();

    [JavaThrownExceptions("1;org/nakedobjects/object/InvalidEntryException;")]
    public override void parseTextEntry(string text)
    {
      // ISSUE: unable to decompile the method.
    }

    public override sbyte[] asEncodedString() => this.adaptee.isEmpty() ? StringImpl.getBytes("NULL") : StringImpl.getBytes(StringImpl.valueOf(this.color()));

    public override void restoreFromEncodedString(sbyte[] data)
    {
      string str = StringImpl.createString(data);
      if (str == null || StringImpl.equals(str, (object) "NULL"))
        this.adaptee.clear();
      else
        this.setColor(Integer.valueOf(str).intValue());
    }

    public override string titleString() => this.adaptee.titleString();

    public override object getObject() => (object) this.adaptee;

    public override string getValueClass() => ObjectImpl.getClass((object) this.adaptee).getName();
  }
}
