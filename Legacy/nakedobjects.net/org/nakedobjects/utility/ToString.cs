// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.ToString
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.text;
using java.util;
using System.ComponentModel;

namespace org.nakedobjects.utility
{
  public sealed class ToString
  {
    private static readonly DateFormat dateFormat;
    private bool addComma;
    private readonly StringBuffer @string;
    private bool useLineBreaks;

    public ToString(object forObject, string text)
      : this(forObject)
    {
      this.@string.append(text);
      this.addComma = StringImpl.length(text) > 0;
    }

    public static string name(object forObject)
    {
      string name = ObjectImpl.getClass(forObject).getName();
      return StringImpl.substring(name, StringImpl.lastIndexOf(name, 46) + 1);
    }

    public ToString(object forObject)
    {
      this.addComma = false;
      this.@string = new StringBuffer();
      this.@string.append(org.nakedobjects.utility.ToString.name(forObject));
      this.@string.append("@");
      StringBuffer stringBuffer = this.@string;
      object obj = forObject;
      string hexString = Integer.toHexString(!(obj is string) ? ObjectImpl.hashCode(obj) : StringImpl.hashCode((string) obj));
      stringBuffer.append(hexString);
      this.@string.append(" [");
    }

    public ToString(object forObject, int id)
    {
      this.addComma = false;
      this.@string = new StringBuffer();
      this.@string.append(org.nakedobjects.utility.ToString.name(forObject));
      this.@string.append("#");
      this.@string.append(id);
      this.@string.append(" [");
    }

    public virtual org.nakedobjects.utility.ToString append(string text)
    {
      this.@string.append(text);
      return this;
    }

    public virtual org.nakedobjects.utility.ToString append(string name, bool flag)
    {
      this.append(name, !flag ? "false" : "true");
      return this;
    }

    public virtual org.nakedobjects.utility.ToString append(string name, sbyte number)
    {
      this.append(name, Byte.toString(number));
      return this;
    }

    public virtual org.nakedobjects.utility.ToString append(string name, double number)
    {
      this.append(name, Double.toString(number));
      return this;
    }

    public virtual org.nakedobjects.utility.ToString append(string name, float number)
    {
      this.append(name, Float.toString(number));
      return this;
    }

    public virtual org.nakedobjects.utility.ToString append(string name, int number)
    {
      this.append(name, Integer.toString(number));
      return this;
    }

    public virtual org.nakedobjects.utility.ToString append(string name, long number)
    {
      this.append(name, Long.toString(number));
      return this;
    }

    public virtual org.nakedobjects.utility.ToString append(string name, object @object)
    {
      this.append(name, @object != null ? @object.ToString() : "null");
      return this;
    }

    public virtual org.nakedobjects.utility.ToString append(string name, short number)
    {
      this.append(name, Short.toString(number));
      return this;
    }

    public virtual org.nakedobjects.utility.ToString append(string name, string @string)
    {
      if (this.addComma)
      {
        this.@string.append(',');
        if (this.useLineBreaks)
          this.@string.append("\n\t");
      }
      else
        this.addComma = true;
      this.@string.append(name);
      this.@string.append('=');
      this.@string.append(@string);
      return this;
    }

    public virtual void appendAsTimestamp(string name, Date date)
    {
      string @string = org.nakedobjects.utility.ToString.timestamp(date);
      this.append(name, @string);
    }

    public static string timestamp(Date date) => date == null ? "" : org.nakedobjects.utility.ToString.dateFormat.format(date);

    public virtual org.nakedobjects.utility.ToString appendAsHex(string name, long number)
    {
      this.append(name, new StringBuffer().append("#").append(Long.toHexString(number)).ToString());
      return this;
    }

    public virtual void appendTruncated(string name, string @string, int maxLength)
    {
      if (StringImpl.length(@string) > maxLength)
      {
        this.append(name, StringImpl.substring(@string, 0, maxLength));
        this.append("...");
      }
      else
        this.append(name, @string);
    }

    public virtual void setAddComma() => this.addComma = true;

    public virtual void setUseLineBreaks(bool useLineBreaks) => this.useLineBreaks = useLineBreaks;

    public override string ToString()
    {
      this.@string.append(']');
      return this.@string.ToString();
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static ToString()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      org.nakedobjects.utility.ToString toString = this;
      ObjectImpl.clone((object) toString);
      return ((object) toString).MemberwiseClone();
    }
  }
}
