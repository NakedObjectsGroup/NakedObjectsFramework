// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.AbstractUserAction
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;

namespace org.nakedobjects.viewer.skylark
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/UserAction;")]
  public abstract class AbstractUserAction : UserAction
  {
    private string description;
    private string name;
    private Action.Type type;

    public AbstractUserAction(string name)
      : this(name, Action.USER)
    {
    }

    public AbstractUserAction(string name, Action.Type type)
    {
      this.name = name;
      this.type = type;
    }

    public virtual Consent disabled(View view) => (Consent) Allow.DEFAULT;

    public abstract void execute(Workspace workspace, View view, Location at);

    public virtual string getDescription(View view) => this.description;

    public virtual string getHelp(View view) => "No help available for user action";

    public virtual string getName(View view) => this.name;

    public virtual Action.Type getType() => this.type;

    public virtual void setName(string name) => this.name = name;

    public override string ToString()
    {
      string name = ObjectImpl.getClass((object) this).getName();
      return StringImpl.substring(name, StringImpl.lastIndexOf(name, 46) + 1);
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      AbstractUserAction abstractUserAction = this;
      ObjectImpl.clone((object) abstractUserAction);
      return ((object) abstractUserAction).MemberwiseClone();
    }
  }
}
