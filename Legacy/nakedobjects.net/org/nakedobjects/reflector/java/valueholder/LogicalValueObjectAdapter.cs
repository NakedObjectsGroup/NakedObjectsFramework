// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.valueholder.LogicalValueObjectAdapter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.value;
using org.nakedobjects.@object.value.adapter;
using org.nakedobjects.application.valueholder;
using org.nakedobjects.utility;

namespace org.nakedobjects.reflector.java.valueholder
{
  [JavaInterfaces("1;org/nakedobjects/object/value/BooleanValue;")]
  public class LogicalValueObjectAdapter : AbstractNakedValue, BooleanValue
  {
    private readonly Logical adaptee;

    public LogicalValueObjectAdapter(Logical adaptee) => this.adaptee = adaptee;

    public virtual bool isSet() => this.adaptee.isSet();

    public virtual void set() => this.adaptee.set();

    public virtual void reset() => this.adaptee.reset();

    public override string getIconName() => "boolean";

    public override string ToString() => new StringBuffer().append("POJO LogicalAdapter: ").append(this.adaptee.isSet()).ToString();

    public virtual void toggle() => this.adaptee.setValue(((this.adaptee.isSet() ? 1 : 0) ^ 1) != 0);

    [JavaThrownExceptions("1;org/nakedobjects/object/InvalidEntryException;")]
    public override void parseTextEntry(string text)
    {
      if (StringImpl.startsWith("true", StringImpl.toLowerCase(text)))
      {
        this.set();
      }
      else
      {
        if (!StringImpl.startsWith("false", StringImpl.toLowerCase(text)))
          throw new InvalidEntryException();
        this.reset();
      }
    }

    public override sbyte[] asEncodedString()
    {
      int length = 1;
      sbyte[] numArray = length >= 0 ? new sbyte[length] : throw new NegativeArraySizeException();
      numArray[0] = !this.isSet() ? (sbyte) 70 : (sbyte) 84;
      return numArray;
    }

    public override void restoreFromEncodedString(sbyte[] data)
    {
      if (data.Length != 1)
        throw new NakedObjectRuntimeException(new StringBuffer().append("Invalid data for logical, expected one byte, got ").append(data.Length).ToString());
      if (data[0] == (sbyte) 84)
      {
        this.set();
      }
      else
      {
        if (data[0] != (sbyte) 70)
          throw new NakedObjectRuntimeException(new StringBuffer().append("Invalid data for logical, expected 'T' or 'F', but  got ").append((int) data[0]).ToString());
        this.reset();
      }
    }

    public override string titleString() => this.isSet() ? "True" : "False";

    public override object getObject() => (object) this.adaptee;

    public override string getValueClass() => ObjectImpl.getClass((object) this.adaptee).getName();
  }
}
