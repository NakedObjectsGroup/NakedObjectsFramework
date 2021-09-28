// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.action.NextParameterDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.viewer.cli.action
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public class NextParameterDispatcher : Dispatcher
  {
    public virtual void execute(Command command, Context context, View view) => ((ActionAgent) context.getAgent()).nextParameter();

    public virtual string getHelp() => "Move to the next parameter";

    public virtual string getNames() => "next";

    public virtual bool isAvailable(Context context)
    {
      if (!context.currentAgentIs(Class.FromType(typeof (ActionAgent))))
        return false;
      ActionAgent agent = (ActionAgent) context.getAgent();
      return agent != null && agent.hasNextParameter();
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      NextParameterDispatcher parameterDispatcher = this;
      ObjectImpl.clone((object) parameterDispatcher);
      return ((object) parameterDispatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
