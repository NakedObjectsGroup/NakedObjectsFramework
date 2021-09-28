// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.object.GotoObjectDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.viewer.cli.@object;
using org.nakedobjects.viewer.cli.action;
using org.nakedobjects.viewer.cli.field;

namespace org.nakedobjects.viewer.cli.@object
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public class GotoObjectDispatcher : Dispatcher
  {
    public virtual void execute(Command command, Context context, View view)
    {
      Agent agent = context.findAgent(command.getParameter(0));
      if (agent is CollectionAgent)
        throw new IllegalDispatchException("Can't open an objects collection, list the collection or open an element within it instead");
      if (agent == null)
        return;
      context.addObject(agent);
    }

    public virtual string getHelp() => "Go to the object that matches the specified title";

    public virtual string getNames() => "goto go g";

    public virtual bool isAvailable(Context context) => ((context.currentAgentIs(Class.FromType(typeof (ActionAgent))) || context.currentAgentIs(Class.FromType(typeof (AbstractFieldAgent))) ? 1 : 0) ^ 1) != 0;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      GotoObjectDispatcher objectDispatcher = this;
      ObjectImpl.clone((object) objectDispatcher);
      return ((object) objectDispatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
