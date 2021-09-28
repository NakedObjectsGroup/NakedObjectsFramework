// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.valueholder.Logical
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.application.valueholder
{
  public class Logical : BusinessValueHolder
  {
    public const string FALSE = "false";
    private const long serialVersionUID = 1;
    public const string TRUE = "true";
    private bool flag;
    private bool isNull;

    public Logical()
      : this(false)
    {
      this.isNull = false;
    }

    public Logical(bool flag)
    {
      this.flag = flag;
      this.isNull = false;
    }

    public virtual bool booleanValue() => this.flag;

    public override void clear() => this.isNull = true;

    public override void copyObject(BusinessValueHolder @object)
    {
      this.isNull = @object is Logical ? ((Logical) @object).isNull : throw new IllegalArgumentException("Can only copy the value of  a Logical object");
      this.flag = ((Logical) @object).flag;
    }

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      if (!(obj is Logical))
        return false;
      Logical logical = (Logical) obj;
      return logical.isEmpty() && this.isEmpty() || logical.flag == this.flag;
    }

    public virtual string getObjectHelpText() => "A Logical object containing either True or False.";

    public override bool isEmpty() => this.isNull;

    public override bool isSameAs(BusinessValueHolder @object) => @object is Logical && ((Logical) @object).flag == this.flag;

    public virtual bool isSet() => this.flag;

    [JavaThrownExceptions("1;org/nakedobjects/application/ValueParseException;")]
    public override void parseUserEntry(string text)
    {
      if (StringImpl.equals(StringImpl.trim(text), (object) ""))
      {
        this.clear();
      }
      else
      {
        if (StringImpl.startsWith("true", StringImpl.toLowerCase(text)))
          this.set();
        else
          this.reset();
        this.isNull = false;
      }
    }

    public virtual void reset() => this.flag = false;

    public override void restoreFromEncodedString(string data)
    {
      if (data == null || StringImpl.equals(data, (object) "NULL"))
        this.clear();
      else
        this.setValue(StringImpl.equals(data, (object) "true"));
    }

    public override string asEncodedString()
    {
      if (this.isEmpty())
        return "NULL";
      return this.isSet() ? "true" : "false";
    }

    public virtual void set() => this.flag = true;

    public virtual void setValue(bool set) => this.flag = set;

    public virtual void setValue(Logical value) => this.flag = value.flag;

    public override Title title() => new Title(!this.isEmpty() ? (!this.flag ? "FALSE" : "TRUE") : "");
  }
}
