// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.action.ValueParameterDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.viewer.cli.action
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public class ValueParameterDispatcher : Dispatcher
  {
    public virtual void execute(Command command, Context context, View view)
    {
      if (command.getNumberOfParameters() != 1)
        throw new IllegalDispatchException("No value specified");
      ActionAgent agent = (ActionAgent) context.getAgent();
      if (agent == null)
        return;
      string parameter = command.getParameter(0);
      if (agent.canSetValueParameter(parameter))
        agent.setValueParameter(parameter);
      else
        view.error("Can't set parameter; type is wrong");
    }

    public virtual string getHelp() => "Sets the current parameter to the specified value";

    public virtual string getNames() => "value";

    public virtual bool isAvailable(Context context)
    {
      if (!context.currentAgentIs(Class.FromType(typeof (ActionAgent))))
        return false;
      ActionAgent agent = (ActionAgent) context.getAgent();
      return agent.isCollectingParameters() && agent.getParameterType().isValue();
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      ValueParameterDispatcher parameterDispatcher = this;
      ObjectImpl.clone((object) parameterDispatcher);
      return ((object) parameterDispatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
