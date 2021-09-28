// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.value.adapter.DateAdapter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.text;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.value;
using org.nakedobjects.@object.value.adapter;
using System.ComponentModel;

namespace org.nakedobjects.@object.value.adapter
{
  [JavaInterfaces("1;org/nakedobjects/object/value/DateValue;")]
  public class DateAdapter : AbstractNakedValue, DateValue
  {
    private static readonly DateFormat ISO_LONG;
    private static readonly DateFormat ISO_SHORT;
    private static readonly DateFormat LONG_FORMAT;
    private static readonly DateFormat MEDIUM_FORMAT;
    private const long serialVersionUID = 1;
    private static readonly DateFormat SHORT_FORMAT;
    private Date date;

    public DateAdapter(Date date) => this.date = date;

    public override sbyte[] asEncodedString()
    {
      if (this.date == null)
        return StringImpl.getBytes("NULL");
      Calendar instance = Calendar.getInstance();
      instance.setTime(this.date);
      StringBuffer stringBuffer = new StringBuffer(8);
      string str = StringImpl.valueOf(instance.get(1));
      stringBuffer.append(StringImpl.substring("0000", 0, 4 - StringImpl.length(str)));
      stringBuffer.append(str);
      int num1 = instance.get(2) + 1;
      stringBuffer.append(num1 > 9 ? "" : "0");
      stringBuffer.append(num1);
      int num2 = instance.get(5);
      stringBuffer.append(num2 > 9 ? "" : "0");
      stringBuffer.append(num2);
      return StringImpl.getBytes(stringBuffer.ToString());
    }

    public virtual Date dateValue() => new Date(this.date.getTime());

    public override string getIconName() => "date";

    public override object getObject() => (object) this.date;

    public override Oid getOid() => (Oid) null;

    public override string getValueClass() => Class.FromType(typeof (Date)).getName();

    [JavaThrownExceptions("1;org/nakedobjects/object/InvalidEntryException;")]
    public override void parseTextEntry(string entry)
    {
      if (entry == null)
        this.date = (Date) null;
      string str = StringImpl.trim(entry);
      if (StringImpl.equals(str, (object) ""))
        this.date = (Date) null;
      else if (StringImpl.equals(entry, (object) "today") || StringImpl.equals(entry, (object) "now"))
      {
        this.date = new Date();
      }
      else
      {
        int length = 5;
        DateFormat[] dateFormatArray1 = length >= 0 ? new DateFormat[length] : throw new NegativeArraySizeException();
        dateFormatArray1[0] = DateAdapter.LONG_FORMAT;
        dateFormatArray1[1] = DateAdapter.MEDIUM_FORMAT;
        dateFormatArray1[2] = DateAdapter.SHORT_FORMAT;
        dateFormatArray1[3] = DateAdapter.ISO_LONG;
        dateFormatArray1[4] = DateAdapter.ISO_SHORT;
        DateFormat[] dateFormatArray2 = dateFormatArray1;
        for (int index = 0; index < dateFormatArray2.Length; ++index)
        {
          try
          {
            this.date = dateFormatArray2[index].parse(str);
            break;
          }
          catch (ParseException ex)
          {
            if (index + 1 == dateFormatArray2.Length)
              throw new TextEntryParseException(new StringBuffer().append("Invalid date ").append(str).ToString(), (Throwable) ex);
          }
        }
      }
    }

    public override void restoreFromEncodedString(sbyte[] data)
    {
      string str = StringImpl.createString(data);
      if (StringImpl.equals(str, (object) "NULL"))
      {
        this.date = (Date) null;
      }
      else
      {
        int num1 = Integer.valueOf(StringImpl.substring(str, 0, 4)).intValue();
        int num2 = Integer.valueOf(StringImpl.substring(str, 4, 6)).intValue();
        int num3 = Integer.valueOf(StringImpl.substring(str, 6)).intValue();
        Calendar instance = Calendar.getInstance();
        instance.set(10, 0);
        instance.set(11, 0);
        instance.set(12, 0);
        instance.set(13, 0);
        instance.set(9, 0);
        instance.set(14, 0);
        instance.set(num1, num2 - 1, num3);
        this.date = instance.getTime();
      }
    }

    public virtual void setValue(Date date) => this.date = new Date(date.getTime());

    public override string titleString() => this.date == null ? "" : DateAdapter.MEDIUM_FORMAT.format(this.date);

    public override string ToString() => new StringBuffer().append("DataAdapter: ").append((object) this.date).ToString();

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static DateAdapter()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
