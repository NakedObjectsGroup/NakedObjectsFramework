// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.action.ReferenceParameterDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.viewer.cli.@object;

namespace org.nakedobjects.viewer.cli.action
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public class ReferenceParameterDispatcher : Dispatcher
  {
    public virtual void execute(Command command, Context context, View view)
    {
      ActionAgent agent1 = (ActionAgent) context.getAgent();
      if (agent1 == null)
        return;
      if (command.hasParameters())
      {
        Agent agent2 = context.findAgent(command.getParameter(0));
        agent1.setReferenceParameter(((ObjectAgent) agent2).getObject());
      }
      else
      {
        Agent agent3 = context.getHistory().last();
        if (!(agent3 is ObjectAgent))
          return;
        ObjectAgent objectAgent = (ObjectAgent) agent3;
        agent1.setReferenceParameter(objectAgent.getObject());
      }
    }

    public virtual string getHelp() => "Set the current parameter to be the current object";

    public virtual string getNames() => "use u";

    public virtual bool isAvailable(Context context) => context.currentAgentIs(Class.FromType(typeof (ActionAgent))) && ((ActionAgent) context.getAgent()).isCollectingParameters();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ReferenceParameterDispatcher parameterDispatcher = this;
      ObjectImpl.clone((object) parameterDispatcher);
      return ((object) parameterDispatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
