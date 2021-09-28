// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.action.ClassActionSummaryDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.viewer.cli.classes;

namespace org.nakedobjects.viewer.cli.action
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public class ClassActionSummaryDispatcher : AbstractActionSummaryDispatcher, Dispatcher
  {
    public virtual void execute(Command command, Context context, View view)
    {
      ClassAgent agent = (ClassAgent) context.getAgent();
      this.summariseAction(command.getParameterAsLowerCase(0), agent.getActions(), view, agent.getNakedClass());
    }

    public virtual string getHelp() => "Lists the parameters in the specified class action";

    public virtual bool isAvailable(Context context) => context.currentAgentIs(Class.FromType(typeof (ClassAgent)));
  }
}
