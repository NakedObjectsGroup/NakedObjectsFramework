// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.action.AbstractActionListDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.viewer.cli.action
{
  public abstract class AbstractActionListDispatcher
  {
    public virtual string getNames() => "actions acts";

    [JavaFlags(4)]
    public virtual void listActions(View view, Action[] actions, NakedObject target)
    {
      if (actions.Length <= 0)
        return;
      for (int index = 0; index < actions.Length; ++index)
      {
        StringBuffer stringBuffer = new StringBuffer();
        Action action = actions[index];
        if (action != null)
        {
          action.getParameterTypes();
          int length = 0;
          NakedObjectSpecification[] objectSpecificationArray = length >= 0 ? new NakedObjectSpecification[length] : throw new NegativeArraySizeException();
          if (action.getActions().Length > 0)
          {
            Action[] actions1 = actions[index].getActions();
            if (actions1 != null)
              this.listActions(view, actions1, target);
          }
          else if (action.isAuthorised() && (target == null || !action.isVisible((NakedReference) target).isVetoed() && !action.isAvailable((NakedReference) target).isVetoed()))
          {
            if (objectSpecificationArray.Length > 0)
              view.display(new StringBuffer().append(action.getName()).append(" (").append((object) stringBuffer).append(")").ToString());
            else
              view.display(action.getName());
          }
        }
      }
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      AbstractActionListDispatcher actionListDispatcher = this;
      ObjectImpl.clone((object) actionListDispatcher);
      return ((object) actionListDispatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
