// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.action.ExecuteActionDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.viewer.cli.action
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public class ExecuteActionDispatcher : Dispatcher
  {
    public virtual void execute(Command command, Context context, View view)
    {
      ActionAgent agent = (ActionAgent) context.getAgent();
      if (agent.canExecute())
        agent.execute(context, view);
      else
        view.error("Can't execute until all parameters are set up");
    }

    public virtual string getHelp() => "Executes the current action";

    public virtual string getNames() => "ok";

    public virtual bool isAvailable(Context context)
    {
      if (context.currentAgentIs(Class.FromType(typeof (ActionAgent))))
      {
        ActionAgent agent = (ActionAgent) context.getAgent();
        if (agent != null)
          return agent.canExecute();
      }
      return false;
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ExecuteActionDispatcher actionDispatcher = this;
      ObjectImpl.clone((object) actionDispatcher);
      return ((object) actionDispatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
