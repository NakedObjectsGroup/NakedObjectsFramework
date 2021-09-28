// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.UserActionSet
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;

namespace org.nakedobjects.viewer.skylark
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/UserAction;")]
  public class UserActionSet : UserAction
  {
    private Color backgroundColor;
    private readonly string groupName;
    private readonly bool includeDebug;
    private readonly bool includeExploration;
    private Vector options;
    private readonly Action.Type type;

    public UserActionSet(bool includeExploration, bool includeDebug, Action.Type type)
    {
      this.backgroundColor = Color.DEBUG_BASELINE;
      this.options = new Vector();
      this.type = type;
      this.groupName = "";
      this.includeExploration = includeExploration;
      this.includeDebug = includeDebug;
    }

    public UserActionSet(string groupName, UserActionSet parent)
    {
      this.backgroundColor = Color.DEBUG_BASELINE;
      this.options = new Vector();
      this.groupName = groupName;
      this.includeExploration = parent.includeExploration;
      this.includeDebug = parent.includeDebug;
      this.type = parent.type;
      this.backgroundColor = parent.getColor();
    }

    public virtual void add(UserAction option)
    {
      Action.Type type = option.getType();
      if (type != UserAction.USER && (!this.includeExploration || type != UserAction.EXPLORATION) && (!this.includeDebug || type != UserAction.DEBUG))
        return;
      this.options.addElement((object) option);
    }

    public virtual Consent disabled(View view) => (Consent) Allow.DEFAULT;

    public virtual void execute(Workspace workspace, View view, Location at)
    {
    }

    public virtual Color getColor() => this.backgroundColor;

    public virtual string getDescription(View view) => "";

    public virtual string getHelp(View view) => "";

    public virtual UserAction[] getMenuOptions()
    {
      int length = this.options.size();
      UserAction[] userActionArray = length >= 0 ? new UserAction[length] : throw new NegativeArraySizeException();
      for (int index = 0; index < userActionArray.Length; ++index)
        userActionArray[index] = (UserAction) this.options.elementAt(index);
      return userActionArray;
    }

    public virtual string getName(View view) => this.groupName;

    public virtual Action.Type getType() => this.type;

    public virtual void setColor(Color color) => this.backgroundColor = color;

    public override string ToString()
    {
      StringBuffer stringBuffer = new StringBuffer("MenuOptionSet [");
      int num = 0;
      for (int index = this.options.size(); num < index; ++num)
        stringBuffer.append(new StringBuffer().append(this.options.elementAt(num)).append(",").ToString());
      stringBuffer.append("]");
      return stringBuffer.ToString();
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      UserActionSet userActionSet = this;
      ObjectImpl.clone((object) userActionSet);
      return ((object) userActionSet).MemberwiseClone();
    }
  }
}
