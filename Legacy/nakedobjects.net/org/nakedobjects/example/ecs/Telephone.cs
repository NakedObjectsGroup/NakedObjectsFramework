// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.example.ecs.Telephone
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.application;
using org.nakedobjects.application.control;
using org.nakedobjects.application.valueholder;

namespace org.nakedobjects.example.ecs
{
  public class Telephone
  {
    private readonly TextString number;
    private readonly TextString knownAs;
    private readonly Logical temporary;

    public Telephone()
    {
      this.number = new TextString();
      this.knownAs = new TextString();
      this.temporary = new Logical();
    }

    public virtual void aboutKnownAs(FieldAbout about, TextString entry) => about.unmodifiableOnCondition(this.temporary.isSet(), "Flag set");

    [JavaFlags(17)]
    public TextString getKnownAs() => this.knownAs;

    public virtual void aboutNumber(FieldAbout about, TextString entry) => about.unmodifiableOnCondition(this.temporary.isSet(), "Flag set");

    [JavaFlags(17)]
    public TextString getNumber() => this.number;

    [JavaFlags(17)]
    public Logical getUnmodifiable() => this.temporary;

    public virtual Title title() => this.knownAs.isEmpty() ? this.number.title() : this.knownAs.title();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      Telephone telephone = this;
      ObjectImpl.clone((object) telephone);
      return ((object) telephone).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
