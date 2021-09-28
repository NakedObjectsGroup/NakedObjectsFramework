// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.value.adapter.DoubleAdapter
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
  [JavaInterfaces("1;org/nakedobjects/object/value/DoubleFloatingPointValue;")]
  public class DoubleAdapter : AbstractNakedValue, DoubleFloatingPointValue
  {
    private static NumberFormat FORMAT;
    private double value;

    public DoubleAdapter(Double value) => this.value = value.doubleValue();

    public override sbyte[] asEncodedString() => (sbyte[]) null;

    public virtual double doubleValue() => this.value;

    public override string getIconName() => "double";

    public override object getObject() => (object) new Double(this.value);

    [JavaThrownExceptions("1;org/nakedobjects/object/InvalidEntryException;")]
    public override void parseTextEntry(string entry)
    {
      if (entry != null)
      {
        if (!StringImpl.equals(StringImpl.trim(entry), (object) ""))
        {
          try
          {
            this.value = DoubleAdapter.FORMAT.parse(entry).doubleValue();
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

    public virtual void setValue(double value) => this.value = value;

    public override string titleString() => DoubleAdapter.FORMAT.format(this.value);

    public override string getValueClass() => ((Class) Double.TYPE).getName();

    public override string ToString() => new StringBuffer().append("DoubleAdapter: ").append(this.value).ToString();

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static DoubleAdapter()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
