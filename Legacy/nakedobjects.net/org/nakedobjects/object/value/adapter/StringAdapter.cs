// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.value.adapter.StringAdapter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object.value;
using org.nakedobjects.@object.value.adapter;

namespace org.nakedobjects.@object.value.adapter
{
  [JavaInterfaces("1;org/nakedobjects/object/value/StringValue;")]
  public class StringAdapter : AbstractNakedValue, StringValue
  {
    private string text;

    public StringAdapter(string text) => this.text = text;

    public override void parseTextEntry(string text) => this.text = text;

    public override sbyte[] asEncodedString() => StringImpl.getBytes(this.text);

    public override void restoreFromEncodedString(sbyte[] data) => this.text = StringImpl.createString(data);

    public override object getObject() => (object) this.text;

    public virtual string stringValue() => this.text;

    public virtual void setValue(string value) => this.text = value;

    public override string getIconName() => "text";

    public override string ToString() => new StringBuffer().append("StringAdapter: ").append(this.text).ToString();

    public override string titleString() => this.text;

    public override string getValueClass() => Class.FromType(typeof (string)).getName();
  }
}
