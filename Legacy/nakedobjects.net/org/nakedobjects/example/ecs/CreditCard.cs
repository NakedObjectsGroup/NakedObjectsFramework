// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.example.ecs.CreditCard
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.application;
using org.nakedobjects.application.control;
using org.nakedobjects.application.valueholder;

namespace org.nakedobjects.example.ecs
{
  [JavaInterfaces("1;org/nakedobjects/example/ecs/PaymentMethod;")]
  public class CreditCard : PaymentMethod
  {
    private readonly TextString nameOnCard;
    private readonly TextString number;
    private readonly TextString expires;
    private readonly Color color;

    public static void aboutCreditCard(ClassAbout about) => about.instancesUnavailable();

    public CreditCard()
    {
      this.expires = new TextString();
      this.nameOnCard = new TextString();
      this.number = new TextString();
      this.color = new Color();
      this.number.setMaximumLength(16);
      this.expires.setMinimumLength(4);
    }

    [JavaFlags(17)]
    public TextString getExpires() => this.expires;

    public virtual Color getColor() => this.color;

    public virtual void aboutExpires(FieldAbout about, TextString value)
    {
      if (value == null || value.isEmpty())
      {
        about.invalid("Cannot be empty");
      }
      else
      {
        string str = StringImpl.trim(value.stringValue());
        bool flag1 = true;
        bool flag2 = StringImpl.length(str) == 5 && ((flag1 & Character.isDigit(StringImpl.charAt(str, 0)) & Character.isDigit(StringImpl.charAt(str, 1)) ? 1 : 0) & ('/' == StringImpl.charAt(str, 2) ? 1 : 0)) != 0 & Character.isDigit(StringImpl.charAt(str, 3)) & Character.isDigit(StringImpl.charAt(str, 4));
        about.invalidOnCondition(((flag2 ? 1 : 0) ^ 1) != 0, "date must be MM/YY e.g. 03/05");
      }
    }

    [JavaFlags(17)]
    public TextString getNameOnCard() => this.nameOnCard;

    public virtual void validNameOnCard(Validity validity) => validity.cannotBeEmpty();

    [JavaFlags(17)]
    public TextString getNumber() => this.number;

    public virtual void aboutNumber(FieldAbout about, TextString number)
    {
      if (number == null)
      {
        about.invalid("Cannot be empty");
      }
      else
      {
        int num = StringImpl.length(number.stringValue());
        about.invalidOnCondition(num != 16, new StringBuffer().append("Card numbers are 16 digits (not ").append(num).append(")").ToString());
      }
    }

    public virtual Title title()
    {
      if (this.getNumber().isEmpty())
        return new Title();
      string str = this.getNumber().stringValue();
      int num = Math.max(0, StringImpl.length(str) - 5);
      return new Title(StringImpl.substring("*****************", 0, num)).concat(StringImpl.substring(str, num));
    }

    public virtual string titleString()
    {
      if (this.getNumber().isEmpty())
        return "";
      string str = this.getNumber().stringValue();
      int num = Math.max(0, StringImpl.length(str) - 5);
      return new StringBuffer().append(StringImpl.substring("*****************", 0, num)).append(StringImpl.substring(str, num)).ToString();
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      CreditCard creditCard = this;
      ObjectImpl.clone((object) creditCard);
      return ((object) creditCard).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
