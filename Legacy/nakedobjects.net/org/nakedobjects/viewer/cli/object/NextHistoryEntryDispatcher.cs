// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.object.NextHistoryEntryDispatcher
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
  public class NextHistoryEntryDispatcher : Dispatcher
  {
    public virtual void execute(Command command, Context context, View view) => context.getHistory().next();

    public virtual string getHelp() => "Open next entry in history list";

    public virtual string getNames() => "forward for f";

    public virtual bool isAvailable(Context context) => context.getHistory().hasNext() && !context.currentAgentIs(Class.FromType(typeof (ActionAgent))) && !context.currentAgentIs(Class.FromType(typeof (AbstractFieldAgent)));

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      NextHistoryEntryDispatcher historyEntryDispatcher = this;
      ObjectImpl.clone((object) historyEntryDispatcher);
      return ((object) historyEntryDispatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
