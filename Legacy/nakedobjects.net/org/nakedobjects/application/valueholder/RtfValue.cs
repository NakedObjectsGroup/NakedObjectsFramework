// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.valueholder.RtfValue
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;

namespace org.nakedobjects.application.valueholder
{
  public class RtfValue : BusinessValueHolder
  {
    private string utf8Encoded;

    public override void clear() => this.utf8Encoded = (string) null;

    public override void copyObject(BusinessValueHolder other)
    {
      if (!(other is RtfValue))
        throw new ApplicationException("only support copying from other RTF values");
      this.copyObject((RtfValue) other);
    }

    public virtual void copyObject(RtfValue other) => this.utf8Encoded = other.utf8Encoded;

    public virtual sbyte[] getBytes()
    {
      if (this.utf8Encoded == null)
        return (sbyte[]) null;
      try
      {
        return StringImpl.getBytes(this.utf8Encoded, "UTF-8");
      }
      catch (UnsupportedEncodingException ex)
      {
        throw new ApplicationException((Throwable) ex);
      }
    }

    public override bool isEmpty() => this.utf8Encoded == null;

    public override bool isSameAs(BusinessValueHolder other) => other is RtfValue && this.isSameAs((RtfValue) other);

    public virtual bool isSameAs(RtfValue other)
    {
      if (this.utf8Encoded == null && other.utf8Encoded == null)
        return true;
      return this.utf8Encoded != null && other.utf8Encoded != null && StringImpl.equals(this.utf8Encoded, (object) other.utf8Encoded);
    }

    [JavaThrownExceptions("1;org/nakedobjects/application/ValueParseException;")]
    public override void parseUserEntry(string text)
    {
      try
      {
        this.restoreFromEncodedString(text);
      }
      catch (ApplicationException ex)
      {
        throw new ValueParseException(ex.getCause());
      }
    }

    public virtual void reset() => this.clear();

    public override void restoreFromEncodedString(string utf8Encoded)
    {
      if (utf8Encoded == null)
        this.clear();
      else
        this.utf8Encoded = utf8Encoded;
    }

    public override string asEncodedString() => this.isEmpty() ? (string) null : this.utf8Encoded;

    public virtual void setValue(string value)
    {
      if (value == null)
        this.clear();
      this.restoreFromEncodedString(value);
    }

    public override Title title() => new Title(this.titleString());

    public override string titleString() => new StringBuffer().append(this.utf8Encoded == null ? "" : "not ").append("empty").ToString();

    public override bool userChangeable() => false;
  }
}
