// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.internal.about.InternalAbout
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object.control;
using org.nakedobjects.@object.reflect.@internal.about;
using System;

namespace org.nakedobjects.@object.reflect.@internal.about
{
  [JavaInterfaces("1;org/nakedobjects/object/control/Hint;")]
  [Obsolete(null, false)]
  public class InternalAbout : Hint
  {
    private string description;
    private bool invisible;
    private string name;
    private StringBuffer unusableReason;
    private bool unusable;

    public virtual Consent canAccess() => AbstractConsent.allow(((this.invisible ? 1 : 0) ^ 1) != 0);

    public virtual Consent canUse() => this.unusable ? (Consent) new Veto(this.unusableReason.ToString()) : (Consent) new Allow();

    public virtual string debug() => (string) null;

    public virtual string getDescription() => this.description;

    public virtual string getName() => this.name;

    public virtual void invisible() => this.invisible = true;

    public virtual void setDescription(string description) => this.description = description;

    public virtual void setName(string name) => this.name = name;

    public virtual void unusableOnCondition(bool condition, string reason)
    {
      if (!condition)
        return;
      this.unusable = true;
      this.appendUnusableReason(reason);
    }

    private void appendUnusableReason(string reason)
    {
      if (this.unusableReason.length() > 0)
        this.unusableReason.append("; ");
      this.unusableReason.append(reason);
    }

    public virtual void unusable() => this.unusable = true;

    public virtual void unusable(string reason)
    {
      this.unusable = true;
      this.appendUnusableReason(reason);
    }

    public virtual Consent isValid() => (Consent) Allow.DEFAULT;

    public InternalAbout() => this.unusableReason = new StringBuffer();

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      InternalAbout internalAbout = this;
      ObjectImpl.clone((object) internalAbout);
      return ((object) internalAbout).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
