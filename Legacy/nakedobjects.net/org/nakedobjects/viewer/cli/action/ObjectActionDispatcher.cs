// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.action.ObjectActionDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.viewer.cli.@object;
using org.nakedobjects.viewer.cli.classes;
using org.nakedobjects.viewer.cli.field;

namespace org.nakedobjects.viewer.cli.action
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public class ObjectActionDispatcher : Dispatcher
  {
    public virtual void execute(Command command, Context context, View view)
    {
      string str1 = command.hasParameters() ? command.getParameter(0) : throw new IllegalDispatchException("No action specified");
      int num = StringImpl.lastIndexOf(str1, 46);
      string path;
      string str2;
      if (num == -1)
      {
        path = "";
        str2 = str1;
      }
      else
      {
        path = StringImpl.substring(str1, 0, num);
        str2 = StringImpl.substring(str1, num);
      }
      Agent agent = context.findAgent(path);
      if (agent is ObjectAgent)
      {
        NakedObject @object = ((ObjectAgent) agent).getObject();
        Action[] objectActions = @object.getSpecification().getObjectActions(Action.USER);
        if (objectActions.Length == 0)
          throw new IllegalDispatchException(new StringBuffer().append("No actions on object ").append((object) @object.getSpecification()).ToString());
        ActionAgent.create(objectActions, command, @object, context, view);
      }
      else
      {
        ClassAgent classAgent = (ClassAgent) agent;
        Action[] actions = classAgent.getActions();
        if (actions.Length == 0)
          throw new IllegalDispatchException(new StringBuffer().append("No actions on class ").append(agent.getName()).ToString());
        ActionAgent.create(actions, command, classAgent.getNakedClass(), context, view);
      }
    }

    public virtual string getHelp() => "Run the named action on the current instance";

    public virtual string getNames() => "action act a";

    public virtual bool isAvailable(Context context) => ((context.currentAgentIs(Class.FromType(typeof (ActionAgent))) || context.currentAgentIs(Class.FromType(typeof (AbstractFieldAgent))) ? 1 : 0) ^ 1) != 0;

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ObjectActionDispatcher actionDispatcher = this;
      ObjectImpl.clone((object) actionDispatcher);
      return ((object) actionDispatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
