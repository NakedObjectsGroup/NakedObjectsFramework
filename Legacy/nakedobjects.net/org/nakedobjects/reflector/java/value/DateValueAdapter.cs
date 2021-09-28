// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.value.DateValueAdapter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.util;
using org.nakedobjects.@object.value;
using org.nakedobjects.@object.value.adapter;
using org.nakedobjects.application.value;

namespace org.nakedobjects.reflector.java.value
{
  [JavaInterfaces("1;org/nakedobjects/object/value/DateValue;")]
  public class DateValueAdapter : AbstractNakedValue, DateValue
  {
    public DateValueAdapter(Date date)
    {
    }

    public override string getValueClass() => (string) null;

    public virtual Date dateValue() => (Date) null;

    public virtual void setValue(Date date)
    {
    }

    public override sbyte[] asEncodedString() => (sbyte[]) null;

    [JavaThrownExceptions("1;org/nakedobjects/object/InvalidEntryException;")]
    public override void parseTextEntry(string text)
    {
    }

    public override void restoreFromEncodedString(sbyte[] data)
    {
    }

    public override string getIconName() => (string) null;

    public override object getObject() => (object) null;

    public override string titleString() => (string) null;
  }
}
