// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.valueholder.BusinessValueAdapter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.value.adapter;
using org.nakedobjects.application;
using org.nakedobjects.application.valueholder;

namespace org.nakedobjects.reflector.java.valueholder
{
  public class BusinessValueAdapter : AbstractNakedValue
  {
    private readonly BusinessValueHolder adaptee;

    public BusinessValueAdapter(BusinessValueHolder adaptee) => this.adaptee = adaptee;

    public override void parseTextEntry(string text)
    {
      try
      {
        this.adaptee.parseUserEntry(text);
      }
      catch (ValueParseException ex)
      {
        throw new TextEntryParseException(((Throwable) ex).getMessage(), (Throwable) ex);
      }
    }

    public override sbyte[] asEncodedString() => StringImpl.getBytes(this.adaptee.asEncodedString());

    public override void restoreFromEncodedString(sbyte[] data) => this.adaptee.restoreFromEncodedString(StringImpl.createString(data));

    public override object getObject() => (object) this.adaptee;

    public override string getIconName() => "text";

    public override string ToString() => new StringBuffer().append("POJO BusinessValueAdapter: ").append(this.adaptee.titleString()).ToString();

    public override string titleString() => this.adaptee.titleString();

    public override string getValueClass() => ObjectImpl.getClass((object) this.adaptee).getName();

    public override bool canClear() => true;

    public override void clear() => this.adaptee.clear();

    public override bool isEmpty() => this.adaptee.isEmpty();
  }
}
