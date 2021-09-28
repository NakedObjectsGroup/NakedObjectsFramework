// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.cli.object.ShowObjectHistoryDispatcher
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.viewer.cli.@object;

namespace org.nakedobjects.viewer.cli.@object
{
  [JavaInterfaces("1;org/nakedobjects/viewer/cli/Dispatcher;")]
  public class ShowObjectHistoryDispatcher : Dispatcher
  {
    public virtual void execute(Command command, Context context, View view) => context.getHistory().show(view);

    public virtual string getHelp() => "Show all the previously used objects";

    public virtual string getNames() => "history his h";

    public virtual bool isAvailable(Context context) => true;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      ShowObjectHistoryDispatcher historyDispatcher = this;
      ObjectImpl.clone((object) historyDispatcher);
      return ((object) historyDispatcher).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
