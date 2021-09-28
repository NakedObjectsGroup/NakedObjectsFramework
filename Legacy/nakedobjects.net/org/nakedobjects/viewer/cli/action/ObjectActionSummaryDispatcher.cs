// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.action.ObjectActionSummaryDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.viewer.cli.@object;

namespace org.nakedobjects.viewer.cli.action
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public class ObjectActionSummaryDispatcher : AbstractActionSummaryDispatcher, Dispatcher
  {
    public virtual void execute(Command command, Context context, View view)
    {
      if (!command.hasParameters())
        throw new IllegalDispatchException("No action specified");
      NakedObject target = ((ObjectAgent) context.getAgent()).getObject();
      this.summariseAction(command.getParameter(0), target.getSpecification().getObjectActions(Action.USER), view, target);
    }

    public virtual string getHelp() => "Lists the parameter for the specified instance action";

    public virtual bool isAvailable(Context context) => context.currentAgentIs(Class.FromType(typeof (ObjectAgent)));
  }
}
