// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.value.PhoneNumber
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.application.value
{
  [JavaInterfaces("1;org/nakedobjects/application/value/SimpleBusinessValue;")]
  public class PhoneNumber : SimpleBusinessValue
  {
    private string number;

    public PhoneNumber(string text) => this.number = text;

    public PhoneNumber() => this.number = (string) null;

    public virtual object getValue() => (object) this.number;

    public virtual void parseUserEntry(string text)
    {
      StringBuffer stringBuffer = new StringBuffer(StringImpl.length(text));
      for (int index = 0; index < StringImpl.length(text); ++index)
      {
        char ch = StringImpl.charAt(text, index);
        if (StringImpl.indexOf("01234567890 .-()", (int) ch) >= 0)
          stringBuffer.append(ch);
      }
      this.number = stringBuffer.ToString();
    }

    public virtual void restoreFromEncodedString(string data) => this.number = data;

    public virtual string asEncodedString() => this.number;

    public override string ToString() => this.number;

    public virtual bool userChangeable() => true;

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      PhoneNumber phoneNumber = this;
      ObjectImpl.clone((object) phoneNumber);
      return ((object) phoneNumber).MemberwiseClone();
    }
  }
}
