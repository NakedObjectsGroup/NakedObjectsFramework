// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.value.adapter.ShortAdapter
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
  [JavaInterfaces("1;org/nakedobjects/object/value/ShortValue;")]
  public class ShortAdapter : AbstractNakedValue, ShortValue
  {
    private static NumberFormat FORMAT;
    private short value;

    public ShortAdapter(Short value) => this.value = value.shortValue();

    public override sbyte[] asEncodedString() => (sbyte[]) null;

    public virtual short shortValue() => this.value;

    public override string getIconName() => "short";

    public override object getObject() => (object) new Short(this.value);

    [JavaThrownExceptions("1;org/nakedobjects/object/InvalidEntryException;")]
    public override void parseTextEntry(string entry)
    {
      if (entry != null)
      {
        if (!StringImpl.equals(StringImpl.trim(entry), (object) ""))
        {
          try
          {
            this.value = ShortAdapter.FORMAT.parse(entry).shortValue();
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

    public virtual void setValue(short value) => this.value = value;

    public override string titleString() => ShortAdapter.FORMAT.format((long) this.value);

    public override string getValueClass() => ((Class) Short.TYPE).getName();

    public override string ToString() => new StringBuffer().append("ShortAdapter: ").append((int) this.value).ToString();

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static ShortAdapter()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
