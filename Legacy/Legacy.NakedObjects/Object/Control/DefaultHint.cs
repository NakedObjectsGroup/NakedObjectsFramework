// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.control.DefaultHint
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using NakedFramework.Architecture.Reflect;
using NakedFramework.Core.Reflect;

namespace Legacy.NakedObjects.Object.Control
{
  //[JavaInterfaces("1;org/nakedobjects/object/control/Hint;")]
  public class DefaultHint : Hint
  {
    private readonly string name;

    public DefaultHint() => this.name = (string) null;

    public DefaultHint(string name) => this.name = name;

    public virtual IConsent canAccess() => (IConsent) Allow.Default;

    public virtual IConsent canUse() => (IConsent) Allow.Default;

    public virtual string debug() => "no details (DefaultAbout)";

    public virtual string getDescription() => "";

    public virtual string getName() => this.name;

    public virtual IConsent isValid() => (IConsent) Allow.Default;

    //[JavaFlags(4227077)]
    //[JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      //DefaultHint defaultHint = this;
      //ObjectImpl.clone((object) defaultHint);
      //return ((object) defaultHint).MemberwiseClone();
      return null;
    }

    //[JavaFlags(4227073)]
    public override string ToString() {
        //return ObjectImpl.jloToString((object)this);
        return "";
    }
  }
}
