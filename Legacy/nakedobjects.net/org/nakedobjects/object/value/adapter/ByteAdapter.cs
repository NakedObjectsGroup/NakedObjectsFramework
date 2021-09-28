// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.value.adapter.ByteAdapter
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
  [JavaInterfaces("1;org/nakedobjects/object/value/ByteValue;")]
  public class ByteAdapter : AbstractNakedValue, ByteValue
  {
    private static NumberFormat FORMAT;
    private sbyte value;

    public ByteAdapter(Byte value) => this.value = value.byteValue();

    public override sbyte[] asEncodedString() => (sbyte[]) null;

    public virtual sbyte byteValue() => this.value;

    public override string getIconName() => "byte";

    public override object getObject() => (object) new Byte(this.value);

    public override string getValueClass() => ((Class) Byte.TYPE).getName();

    [JavaThrownExceptions("1;org/nakedobjects/object/InvalidEntryException;")]
    public override void parseTextEntry(string entry)
    {
      if (entry != null)
      {
        if (!StringImpl.equals(StringImpl.trim(entry), (object) ""))
        {
          try
          {
            this.value = ByteAdapter.FORMAT.parse(entry).byteValue();
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

    public virtual void setValue(sbyte value) => this.value = value;

    public override string titleString() => ByteAdapter.FORMAT.format((long) this.value);

    public override string ToString() => new StringBuffer().append("ByteAdapter: ").append((int) this.value).ToString();

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static ByteAdapter()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
