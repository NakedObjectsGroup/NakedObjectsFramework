// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.classes.InstancesDispather
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.viewer.cli.@object;

namespace org.nakedobjects.viewer.cli.classes
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public class InstancesDispather : Dispatcher
  {
    public virtual void execute(Command command, Context context, View view)
    {
      ClassAgent agent = (ClassAgent) context.getAgent();
      TypedNakedCollection typedNakedCollection = agent.instances();
      context.addObject((Agent) new CollectionAgent((NakedCollection) typedNakedCollection, new StringBuffer().append(agent.getPrompt()).append(" Instances (").append(typedNakedCollection.size()).append(")").ToString()));
    }

    public virtual string getHelp() => "Lists all the the instances of the current class";

    public virtual string getNames() => "instances ins";

    public virtual bool isAvailable(Context context) => context.currentAgentIs(Class.FromType(typeof (ClassAgent)));

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      InstancesDispather instancesDispather = this;
      ObjectImpl.clone((object) instancesDispather);
      return ((object) instancesDispather).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
