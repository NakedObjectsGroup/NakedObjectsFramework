// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.value.adapter.FloatAdapter
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
  [JavaInterfaces("1;org/nakedobjects/object/value/FloatingPointValue;")]
  public class FloatAdapter : AbstractNakedValue, FloatingPointValue
  {
    private static NumberFormat FORMAT;
    private float value;

    public FloatAdapter(Float value) => this.value = value.floatValue();

    public override sbyte[] asEncodedString() => (sbyte[]) null;

    public virtual float floatValue() => this.value;

    public override string getIconName() => "float";

    public override object getObject() => (object) new Float(this.value);

    [JavaThrownExceptions("1;org/nakedobjects/object/InvalidEntryException;")]
    public override void parseTextEntry(string entry)
    {
      if (entry != null)
      {
        if (!StringImpl.equals(StringImpl.trim(entry), (object) ""))
        {
          try
          {
            this.value = FloatAdapter.FORMAT.parse(entry).floatValue();
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

    public virtual void setValue(float value) => this.value = value;

    public override string titleString() => FloatAdapter.FORMAT.format((double) this.value);

    public override string ToString() => new StringBuffer().append("FloatAdapter: ").append(this.value).ToString();

    public override string getValueClass() => ((Class) Float.TYPE).getName();

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static FloatAdapter()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
