// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.value.SimpleBusinessValueAdapter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.value.adapter;
using org.nakedobjects.application;
using org.nakedobjects.application.value;

namespace org.nakedobjects.reflector.java.value
{
  [JavaInterfaces("1;org/nakedobjects/object/NakedValue;")]
  public class SimpleBusinessValueAdapter : AbstractNakedValue, NakedValue
  {
    private SimpleBusinessValue value;

    public SimpleBusinessValueAdapter(SimpleBusinessValue value) => this.value = value;

    public override string getValueClass() => ObjectImpl.getClass((object) this.value).getName();

    public virtual void setValue(SimpleBusinessValue value) => this.value = value;

    public override sbyte[] asEncodedString() => StringImpl.getBytes(this.value.asEncodedString());

    [JavaThrownExceptions("1;org/nakedobjects/object/InvalidEntryException;")]
    public override void parseTextEntry(string text)
    {
      try
      {
        this.value.parseUserEntry(text);
      }
      catch (ValueParseException ex)
      {
        throw new InvalidEntryException(new StringBuffer().append("Can't parse ").append(text).ToString(), (Throwable) ex);
      }
    }

    public override void restoreFromEncodedString(sbyte[] data) => this.value.restoreFromEncodedString(StringImpl.createString(data));

    public override string getIconName()
    {
      string valueClass = this.getValueClass();
      return StringImpl.substring(valueClass, StringImpl.lastIndexOf(valueClass, 46) + 1);
    }

    public override object getObject() => (object) this.value;

    public override string titleString() => this.value.ToString();
  }
}
