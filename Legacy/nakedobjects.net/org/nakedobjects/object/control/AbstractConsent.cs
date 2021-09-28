// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.control.AbstractConsent
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using org.nakedobjects.@object.control;

namespace org.nakedobjects.@object.control
{
  [JavaInterfaces("2;java/io/Serializable;org/nakedobjects/object/control/Consent;")]
  public abstract class AbstractConsent : Serializable, Consent
  {
    private string reason;

    [JavaFlags(25)]
    public static Consent allow(bool allow) => allow ? (Consent) Allow.DEFAULT : (Consent) Veto.DEFAULT;

    [JavaFlags(25)]
    public static Consent create(bool allow, string reasonAllowed, string reasonVeteod) => allow ? (Consent) new Allow(reasonAllowed) : (Consent) new Veto(reasonVeteod);

    [JavaFlags(4)]
    public AbstractConsent()
    {
    }

    [JavaFlags(4)]
    public AbstractConsent(string reason) => this.reason = reason;

    public virtual string getReason() => this.reason == null ? "" : this.reason;

    public abstract bool isAllowed();

    public abstract bool isVetoed();

    public override string ToString() => new StringBuffer().append("Permission [type=").append(!this.isVetoed() ? "ALLOWED" : "VETOED").append(", reason=").append(this.reason).append("]").ToString();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      AbstractConsent abstractConsent = this;
      ObjectImpl.clone((object) abstractConsent);
      return ((object) abstractConsent).MemberwiseClone();
    }
  }
}
