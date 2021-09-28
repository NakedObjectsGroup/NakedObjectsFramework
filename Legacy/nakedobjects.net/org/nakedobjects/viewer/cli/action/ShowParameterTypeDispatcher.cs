// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.action.ShowParameterTypeDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.viewer.cli.action
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public class ShowParameterTypeDispatcher : Dispatcher
  {
    public virtual void execute(Command command, Context context, View view) => ((ActionAgent) context.getAgent()).type(view);

    public virtual string getHelp() => "Show the type of the current parameter";

    public virtual string getNames() => "type";

    public virtual bool isAvailable(Context context) => context.currentAgentIs(Class.FromType(typeof (ActionAgent))) && ((ActionAgent) context.getAgent()).isCollectingParameters();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ShowParameterTypeDispatcher parameterTypeDispatcher = this;
      ObjectImpl.clone((object) parameterTypeDispatcher);
      return ((object) parameterTypeDispatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
