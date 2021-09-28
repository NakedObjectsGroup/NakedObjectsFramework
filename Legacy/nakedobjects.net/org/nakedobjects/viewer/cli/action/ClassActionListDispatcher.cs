// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.action.ClassActionListDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.viewer.cli.classes;

namespace org.nakedobjects.viewer.cli.action
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public class ClassActionListDispatcher : AbstractActionListDispatcher, Dispatcher
  {
    public virtual void execute(Command command, Context context, View view)
    {
      ClassAgent agent = (ClassAgent) context.getAgent();
      this.listActions(view, agent.getActions(), agent.getNakedClass());
    }

    public virtual string getHelp() => "Lists all the available action methods for the current class";

    public virtual bool isAvailable(Context context) => context.currentAgentIs(Class.FromType(typeof (ClassAgent)));
  }
}
