// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.action.AbstractActionSummaryDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;

namespace org.nakedobjects.viewer.cli.action
{
  public abstract class AbstractActionSummaryDispatcher
  {
    [JavaFlags(4)]
    public virtual void summariseAction(
      string name,
      Action[] actions,
      View view,
      NakedObject target)
    {
      Matcher matcher = (Matcher) new ActionMatcher(actions);
      Action action = (Action) MatchAlgorithm.match(name, matcher);
      if (action == null || target != null && action.isVisible((NakedReference) target).isVetoed())
        view.error("No such action");
      else if (!action.isAuthorised())
        view.error("Not authorised to use this method");
      else if (target != null && action.isAvailable((NakedReference) target).isVetoed())
        view.error(new StringBuffer().append("Action not available: ").append(action.isAvailable((NakedReference) target).getReason()).ToString());
      else
        this.displaySummary(view, action, target);
    }

    public virtual string getNames() => "parameters params par p";

    private void displaySummary(View view, Action action, NakedObject target)
    {
      NakedObjectSpecification[] parameterTypes = action.getParameterTypes();
      ActionHelper instance = ActionHelper.createInstance((NakedReference) target, action);
      string[] parameterLabels = instance.getParameterLabels();
      object[] defaultParameterValues = instance.getDefaultParameterValues();
      view.display(action.getName());
      for (int index = 0; index < parameterTypes.Length; ++index)
      {
        string str1 = parameterLabels[index];
        string str2 = !instance.getRequiredParameters()[index] ? "" : " (required)";
        object obj = defaultParameterValues[index] != null ? (object) new StringBuffer().append(" [").append(defaultParameterValues[index]).append("]").ToString() : (object) "";
        view.display(new StringBuffer().append(index + 1).append(". ").append(str1).append(obj).append(str2).ToString());
      }
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      AbstractActionSummaryDispatcher summaryDispatcher = this;
      ObjectImpl.clone((object) summaryDispatcher);
      return ((object) summaryDispatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
