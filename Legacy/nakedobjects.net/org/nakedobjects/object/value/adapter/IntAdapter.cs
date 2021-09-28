// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.value.adapter.IntAdapter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.text;
using org.nakedobjects.@object;
using org.nakedobjects.@object.value;
using org.nakedobjects.@object.value.adapter;
using System.ComponentModel;

namespace org.nakedobjects.@object.value.adapter
{
  [JavaInterfaces("1;org/nakedobjects/object/value/IntegerValue;")]
  public class IntAdapter : AbstractNakedValue, IntegerValue
  {
    private static NumberFormat FORMAT;
    private int value;

    public IntAdapter(Integer value) => this.value = value.intValue();

    public override sbyte[] asEncodedString() => (sbyte[]) null;

    public virtual int integerValue() => this.value;

    public override string getIconName() => "int";

    public override object getObject() => (object) new Integer(this.value);

    [JavaThrownExceptions("1;org/nakedobjects/object/InvalidEntryException;")]
    public override void parseTextEntry(string entry)
    {
      if (entry != null)
      {
        if (!StringImpl.equals(StringImpl.trim(entry), (object) ""))
        {
          try
          {
            this.value = IntAdapter.FORMAT.parse(entry).intValue();
            return;
          }
          catch (ParseException ex)
          {
            throw new TextEntryParseException("Invalid number", (Throwable) ex);
          }
        }
      }
      throw new InvalidEntryException();
    }

    public override void restoreFromEncodedString(sbyte[] data)
    {
    }

    public virtual void setValue(int value) => this.value = value;

    public override string titleString() => IntAdapter.FORMAT.format((long) this.value);

    public override string getValueClass() => ((Class) Float.TYPE).getName();

    public override string ToString() => new StringBuffer().append("IntAdapter: ").append(this.value).ToString();

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static IntAdapter()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
