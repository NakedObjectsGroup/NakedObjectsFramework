// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.metal.AbstractButtonAction
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;

namespace org.nakedobjects.viewer.skylark.metal
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/metal/ButtonAction;")]
  public abstract class AbstractButtonAction : ButtonAction
  {
    private readonly string name;
    private readonly bool defaultButton;

    public AbstractButtonAction(string name)
      : this(name, false)
    {
    }

    public AbstractButtonAction(string name, bool defaultButton)
    {
      this.name = name;
      this.defaultButton = defaultButton;
    }

    public virtual Consent disabled(View view) => (Consent) Allow.DEFAULT;

    public virtual string getDescription(View view) => "";

    public virtual string getHelp(View view) => "No help available for button";

    public virtual string getName(View view) => this.name;

    public virtual Action.Type getType() => Action.USER;

    public virtual bool isDefault() => this.defaultButton;

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      AbstractButtonAction abstractButtonAction = this;
      ObjectImpl.clone((object) abstractButtonAction);
      return ((object) abstractButtonAction).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    public abstract void execute(Workspace workspace, View view, Location at);
  }
}
