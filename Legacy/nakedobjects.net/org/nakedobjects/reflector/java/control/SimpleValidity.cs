// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.control.SimpleValidity
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.application.control;

namespace org.nakedobjects.reflector.java.control
{
  [JavaInterfaces("1;org/nakedobjects/application/control/Validity;")]
  public class SimpleValidity : Validity
  {
    private string fieldName;
    private StringBuffer unusableReason;
    private bool isEmpty;

    public SimpleValidity(bool isEmpty, string fieldName)
    {
      this.isEmpty = isEmpty;
      this.fieldName = fieldName;
    }

    public virtual void cannotBeEmpty() => this.invalidOnCondition(this.isEmpty, new StringBuffer().append(this.fieldName).append(" must have a value").ToString());

    public virtual string getReason() => this.unusableReason.ToString();

    public virtual void invalid(string reason)
    {
      if (this.unusableReason == null)
        this.unusableReason = new StringBuffer();
      else
        this.unusableReason.append("; ");
      this.unusableReason.append(reason);
    }

    public virtual void invalidOnCondition(bool condition, string reason)
    {
      if (!condition)
        return;
      this.invalid(reason);
    }

    public virtual void invalidUnlessCondition(bool condition, string reason)
    {
      if (condition)
        return;
      this.invalid(reason);
    }

    public virtual bool isValid() => this.unusableReason == null;

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      SimpleValidity simpleValidity = this;
      ObjectImpl.clone((object) simpleValidity);
      return ((object) simpleValidity).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
