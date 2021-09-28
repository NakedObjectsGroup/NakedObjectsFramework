// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.action.CancelActionDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.viewer.cli.action
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public class CancelActionDispatcher : Dispatcher
  {
    public virtual void execute(Command command, Context context, View view) => context.removeAgent();

    public virtual string getHelp() => "Cancel the current set field";

    public virtual string getNames() => "cancel";

    public virtual bool isAvailable(Context context) => context.currentAgentIs(Class.FromType(typeof (ActionAgent)));

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      CancelActionDispatcher actionDispatcher = this;
      ObjectImpl.clone((object) actionDispatcher);
      return ((object) actionDispatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
