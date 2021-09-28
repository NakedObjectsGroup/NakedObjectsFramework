// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.action.ActionMatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.viewer.cli.action
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Matcher;")]
  public class ActionMatcher : Matcher
  {
    private readonly Action[] actions;
    private int index;
    private ActionMatcher submenu;

    public ActionMatcher(Action[] actions)
    {
      this.index = 0;
      this.actions = actions;
    }

    public virtual object getElement() => this.submenu != null ? this.submenu.getElement() : (object) this.actions[this.index - 1];

    public virtual bool hasMoreElements()
    {
      if (this.submenu != null)
      {
        if (this.submenu.hasMoreElements())
          return true;
        this.submenu = (ActionMatcher) null;
      }
      return this.index < this.actions.Length;
    }

    public virtual string nextElement()
    {
      if (this.submenu != null)
      {
        string str = this.submenu.nextElement();
        if (str != null)
          return str;
      }
      Action[] actions1 = this.actions;
      int index1;
      this.index = (index1 = this.index) + 1;
      int index2 = index1;
      Action action = actions1[index2];
      if (action == null)
        return "";
      Action[] actions2 = action.getActions();
      if (actions2 == null || actions2.Length <= 0)
        return action.getName();
      this.submenu = new ActionMatcher(actions2);
      return this.nextElement();
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      ActionMatcher actionMatcher = this;
      ObjectImpl.clone((object) actionMatcher);
      return ((object) actionMatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
