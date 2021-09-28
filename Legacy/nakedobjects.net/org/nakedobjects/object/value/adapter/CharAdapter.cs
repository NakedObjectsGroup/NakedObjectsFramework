// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.value.adapter.CharAdapter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.value;
using org.nakedobjects.@object.value.adapter;

namespace org.nakedobjects.@object.value.adapter
{
  [JavaInterfaces("1;org/nakedobjects/object/value/CharValue;")]
  public class CharAdapter : AbstractNakedValue, CharValue
  {
    private char value;

    public CharAdapter(char value) => this.value = value;

    public CharAdapter(Character value) => this.value = value.charValue();

    public override sbyte[] asEncodedString() => (sbyte[]) null;

    public virtual char charValue() => this.value;

    public override string getIconName() => "char";

    public override object getObject() => (object) new Character(this.value);

    public override string getValueClass() => ((Class) Character.TYPE).getName();

    [JavaThrownExceptions("1;org/nakedobjects/object/InvalidEntryException;")]
    public override void parseTextEntry(string entry) => this.value = entry != null && !StringImpl.equals(StringImpl.trim(entry), (object) "") && StringImpl.length(entry) == 1 ? StringImpl.charAt(entry, 0) : throw new InvalidEntryException();

    public override void restoreFromEncodedString(sbyte[] data)
    {
    }

    public virtual void setValue(char value) => this.value = value;

    public override string titleString() => new StringBuffer().append("").append(this.value).ToString();
  }
}
