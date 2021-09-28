// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.valueholder.Password
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;

namespace org.nakedobjects.application.valueholder
{
  public class Password : BusinessValueHolder
  {
    private const long serialVersionUID = 1;
    private int maximumLength;
    private string password;

    public Password() => this.password = "";

    public Password(int maximumLength)
      : this()
    {
      this.maximumLength = maximumLength;
    }

    public override string asEncodedString() => this.isEmpty() ? (string) null : this.password;

    public override void clear() => this.password = "";

    public override void copyObject(BusinessValueHolder @object) => this.password = ((Password) @object).password;

    public virtual int getMaximumLength() => this.maximumLength;

    public override bool isEmpty() => this.password == null;

    public override bool isSameAs(BusinessValueHolder @object)
    {
      if (@object is Password)
      {
        string password = ((Password) @object).password;
        if (password == null && this.password == null || this.password != null && StringImpl.equals(this.password, (object) password))
          return true;
      }
      return false;
    }

    [JavaThrownExceptions("1;org/nakedobjects/application/ValueParseException;")]
    public override void parseUserEntry(string text) => this.password = text;

    public virtual void reset() => this.password = (string) null;

    public override void restoreFromEncodedString(string data)
    {
      if (data == null)
        this.clear();
      else
        this.password = data;
    }

    public virtual void setMaximumLength(int maximumLength) => this.maximumLength = maximumLength;

    public virtual void setValue(string password) => this.password = password;

    public virtual string stringValue() => this.password;

    public override Title title() => new Title(this.password);
  }
}
